let token = null;

async function pedirToken() {
    const r = await fetch('/api/auth/token');
    if (r.redirected || !r.ok) {
        location.reload();
        throw { status: 401, data: null };
    }
    token = (await r.json()).token;
}

async function api(url, metodo = 'GET', cuerpo = null, reintentar = true) {
    if (!token) await pedirToken();

    const opciones = { method: metodo, headers: { 'Authorization': 'Bearer ' + token } };
    if (cuerpo) {
        opciones.headers['Content-Type'] = 'application/json';
        opciones.body = JSON.stringify(cuerpo);
    }

    const r = await fetch(url, opciones);

    if (r.status === 401 && reintentar) {
        token = null;
        return api(url, metodo, cuerpo, false);
    }
    if (r.status === 204) return null;

    const data = await r.json().catch(() => null);
    if (!r.ok) throw { status: r.status, data: data };
    return data;
}

function mensajesDeError(e) {
    if (e && e.data && e.data.errors) return Object.values(e.data.errors).flat();
    if (e && e.data && e.data.mensaje) return [e.data.mensaje];
    if (e && e.data && e.data.title) return [e.data.title];
    return ['Ocurrió un error inesperado.'];
}

const ESTADOS = ['Pendiente', 'EnProgreso', 'EnRevision', 'Aprobada', 'ConCorrecciones'];
const PRIORIDADES = ['Baja', 'Media', 'Alta'];

const TEXTO_ESTADO = {
    Pendiente: 'Pendiente', EnProgreso: 'En progreso', EnRevision: 'En revisión',
    Aprobada: 'Aprobada', ConCorrecciones: 'Con correcciones'
};
const CLASE_ESTADO = {
    Pendiente: 'bg-secondary', EnProgreso: 'bg-primary', EnRevision: 'bg-info text-dark',
    Aprobada: 'bg-success', ConCorrecciones: 'bg-warning text-dark'
};
const CLASE_PRIORIDAD = {
    Baja: 'bg-light text-dark border', Media: 'bg-warning text-dark', Alta: 'bg-danger'
};

function claveEnum(valor, lista) {
    return typeof valor === 'number' ? lista[valor] : valor;
}

function formVacio() {
    return { tipoTareaId: 0, fechaLimite: '', prioridad: 1, observaciones: '' };
}

const BuscadorAsync = {
    props: ['url', 'seleccionado', 'placeholder'],
    emits: ['elegir'],
    data() {
        return { texto: '', opciones: [], abierto: false, timer: null };
    },
    watch: {
        seleccionado: {
            immediate: true,
            handler(valor) { if (valor) this.texto = valor.texto; }
        }
    },
    methods: {
        alEscribir() {
            this.$emit('elegir', null);
            clearTimeout(this.timer);
            this.timer = setTimeout(() => this.buscar(), 300);
        },
        async buscar() {
            try {
                this.opciones = await api(this.url + '?q=' + encodeURIComponent(this.texto));
            } catch (e) {
                this.opciones = [];
            }
            this.abierto = true;
        },
        elegir(opcion) {
            this.texto = opcion.texto;
            this.$emit('elegir', opcion);
            this.abierto = false;
        },
        cerrarLista() {
            setTimeout(() => { this.abierto = false; }, 200);
        }
    },
    template: `
        <div class="position-relative">
            <input type="text" class="form-control" autocomplete="off"
                   v-bind:class="{ 'is-valid': seleccionado }"
                   v-bind:placeholder="placeholder"
                   v-model="texto"
                   v-on:input="alEscribir"
                   v-on:focus="buscar"
                   v-on:blur="cerrarLista" />
            <div v-if="abierto" class="list-group lista-opciones shadow">
                <button type="button" class="list-group-item list-group-item-action"
                        v-for="o in opciones" v-bind:key="o.id"
                        v-on:mousedown.prevent="elegir(o)">
                    {{ o.texto }}
                    <small class="text-muted d-block" v-if="o.detalle">{{ o.detalle }}</small>
                </button>
                <div class="list-group-item text-muted" v-if="opciones.length === 0">Sin resultados</div>
            </div>
        </div>`
};

Vue.createApp({
    components: { 'buscador-async': BuscadorAsync },

    data() {
        return {
            items: [], page: 1, pageSize: 10, totalCount: 0, cargando: false,
            filtroTexto: '', filtroEstado: '',

            tipos: [],

            mostrarForm: false, editandoId: null, claveForm: 0, guardando: false,
            form: formVacio(), escenaSel: null, animadorSel: null,

            mensaje: '', errorGeneral: '', errores: []
        };
    },

    computed: {
        totalPages() {
            return Math.max(1, Math.ceil(this.totalCount / this.pageSize));
        }
    },

    async mounted() {
        try {
            this.tipos = await api('/api/busquedas/tipos-tarea');
        } catch (e) {
            this.errorGeneral = mensajesDeError(e).join(' ');
        }
        await this.cargar();
    },

    methods: {
        async cargar() {
            this.cargando = true;
            const params = new URLSearchParams({ page: this.page });
            if (this.filtroTexto.trim()) params.append('q', this.filtroTexto.trim());
            if (this.filtroEstado) params.append('estado', this.filtroEstado);

            try {
                const r = await api('/api/asignaciones?' + params.toString());
                this.items = r.items;
                this.page = r.page;
                this.pageSize = r.pageSize;
                this.totalCount = r.totalCount;
            } catch (e) {
                this.errorGeneral = mensajesDeError(e).join(' ');
            }
            this.cargando = false;
        },
        buscar() {
            this.page = 1;
            this.cargar();
        },
        irAPagina(n) {
            this.page = n;
            this.cargar();
        },

        nuevo() {
            this.editandoId = null;
            this.form = formVacio();
            this.escenaSel = null;
            this.animadorSel = null;
            this.errores = [];
            this.mensaje = '';
            this.claveForm++;
            this.mostrarForm = true;
        },
        async editar(fila) {
            this.errores = [];
            this.mensaje = '';
            try {
                const a = await api('/api/asignaciones/' + fila.id);
                this.editandoId = a.id;
                this.form = {
                    tipoTareaId: a.tipoTareaId,
                    fechaLimite: a.fechaLimite.slice(0, 10),
                    prioridad: PRIORIDADES.indexOf(claveEnum(a.prioridad, PRIORIDADES)),
                    observaciones: a.observaciones || ''
                };
                this.escenaSel = { id: a.escenaId, texto: a.proyecto + ' — Escena ' + a.escenaNumero };
                this.animadorSel = { id: a.animadorId, texto: a.animador };
                this.claveForm++;
                this.mostrarForm = true;
            } catch (e) {
                this.errorGeneral = mensajesDeError(e).join(' ');
            }
        },
        async guardar() {
            this.errores = [];

            if (!this.escenaSel) this.errores.push('Elegí una escena de la lista.');
            if (!this.animadorSel) this.errores.push('Elegí un animador de la lista.');
            if (!this.form.tipoTareaId) this.errores.push('Elegí un tipo de tarea.');
            if (!this.form.fechaLimite) this.errores.push('Indicá la fecha límite.');
            if (this.errores.length > 0) return;

            const cuerpo = {
                escenaId: this.escenaSel.id,
                animadorId: this.animadorSel.id,
                tipoTareaId: this.form.tipoTareaId,
                fechaLimite: this.form.fechaLimite + 'T00:00:00',
                prioridad: this.form.prioridad,
                observaciones: this.form.observaciones
            };

            this.guardando = true;
            try {
                if (this.editandoId) {
                    await api('/api/asignaciones/' + this.editandoId, 'PUT', cuerpo);
                    this.mensaje = 'Asignación modificada.';
                } else {
                    await api('/api/asignaciones', 'POST', cuerpo);
                    this.mensaje = 'Asignación creada.';
                }
                this.mostrarForm = false;
                await this.cargar();
            } catch (e) {
                this.errores = mensajesDeError(e);
            }
            this.guardando = false;
        },
        cancelar() {
            this.mostrarForm = false;
            this.errores = [];
        },

        async eliminar(fila) {
            if (!confirm('¿Eliminar la asignación de ' + fila.tipoTarea + ' (escena ' + fila.escenaNumero + ')?')) return;

            this.mensaje = '';
            this.errorGeneral = '';
            try {
                await api('/api/asignaciones/' + fila.id, 'DELETE');
                if (this.items.length === 1 && this.page > 1) this.page--;
                this.mensaje = 'Asignación eliminada.';
                await this.cargar();
            } catch (e) {
                this.errorGeneral = mensajesDeError(e).join(' ');
            }
        },

        puedeEditar(a) {
            return claveEnum(a.estado, ESTADOS) !== 'Aprobada';
        },
        textoEstado(v) { return TEXTO_ESTADO[claveEnum(v, ESTADOS)]; },
        claseEstado(v) { return CLASE_ESTADO[claveEnum(v, ESTADOS)]; },
        textoPrioridad(v) { return claveEnum(v, PRIORIDADES); },
        clasePrioridad(v) { return CLASE_PRIORIDAD[claveEnum(v, PRIORIDADES)]; },
        formatoFecha(f) { return f.slice(0, 10).split('-').reverse().join('/'); }
    }
}).mount('#app');

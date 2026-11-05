-- MySQL dump 10.13  Distrib 8.0.19, for Win64 (x86_64)
--
-- Host: localhost    Database: genba_studio
-- ------------------------------------------------------
-- Server version	8.0.46

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT INTO `__efmigrationshistory` VALUES ('20261029184627_Inicial','9.0.0');
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `asignaciones`
--

DROP TABLE IF EXISTS `asignaciones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `asignaciones` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `EscenaId` int NOT NULL,
  `TipoTareaId` int NOT NULL,
  `AnimadorId` int NOT NULL,
  `FechaAsignacion` datetime(6) NOT NULL,
  `FechaLimite` datetime(6) NOT NULL,
  `Prioridad` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Estado` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Observaciones` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Asignaciones_AnimadorId_Estado` (`AnimadorId`,`Estado`),
  KEY `IX_Asignaciones_EscenaId_TipoTareaId` (`EscenaId`,`TipoTareaId`),
  KEY `IX_Asignaciones_TipoTareaId` (`TipoTareaId`),
  CONSTRAINT `FK_Asignaciones_AspNetUsers_AnimadorId` FOREIGN KEY (`AnimadorId`) REFERENCES `aspnetusers` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Asignaciones_Escenas_EscenaId` FOREIGN KEY (`EscenaId`) REFERENCES `escenas` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Asignaciones_TiposTarea_TipoTareaId` FOREIGN KEY (`TipoTareaId`) REFERENCES `tipostarea` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `asignaciones`
--

LOCK TABLES `asignaciones` WRITE;
/*!40000 ALTER TABLE `asignaciones` DISABLE KEYS */;
INSERT INTO `asignaciones` VALUES (2,4,4,7,'2026-11-04 20:46:17.419545','2026-11-26 00:00:00.000000','Media','Pendiente',NULL),(3,5,1,5,'2026-10-06 00:00:00.000000','2026-10-21 00:00:00.000000','Media','Aprobada',NULL),(4,5,2,6,'2026-10-08 00:00:00.000000','2026-10-28 00:00:00.000000','Alta','Aprobada','Cuidar el arco de la caída del farol.'),(5,6,1,5,'2026-10-16 00:00:00.000000','2026-10-31 00:00:00.000000','Media','Aprobada',NULL),(6,6,2,6,'2026-10-22 00:00:00.000000','2026-11-10 00:00:00.000000','Alta','ConCorrecciones',NULL),(7,6,3,3,'2026-10-26 00:00:00.000000','2026-11-12 00:00:00.000000','Media','EnRevision',NULL),(8,7,1,11,'2026-10-27 00:00:00.000000','2026-11-11 00:00:00.000000','Alta','EnRevision',NULL),(9,7,2,12,'2026-10-28 00:00:00.000000','2026-11-15 00:00:00.000000','Alta','EnProgreso',NULL),(10,7,5,4,'2026-11-02 00:00:00.000000','2026-11-19 00:00:00.000000','Baja','Pendiente',NULL),(11,8,1,5,'2026-10-24 00:00:00.000000','2026-11-02 00:00:00.000000','Alta','Pendiente','Fecha límite vencida (para probar el aviso).'),(12,8,4,7,'2026-11-03 00:00:00.000000','2026-11-25 00:00:00.000000','Media','Pendiente',NULL),(13,9,6,8,'2026-11-04 00:00:00.000000','2026-12-05 00:00:00.000000','Baja','Pendiente',NULL),(14,9,3,9,'2026-11-01 00:00:00.000000','2026-11-17 00:00:00.000000','Media','EnProgreso',NULL),(15,9,5,10,'2026-10-21 00:00:00.000000','2026-11-03 00:00:00.000000','Media','EnProgreso',NULL),(16,10,1,11,'2026-11-04 00:00:00.000000','2026-11-30 00:00:00.000000','Media','Pendiente',NULL),(17,12,1,5,'2026-10-30 00:00:00.000000','2026-11-13 00:00:00.000000','Alta','EnProgreso',NULL),(18,12,2,6,'2026-10-29 00:00:00.000000','2026-11-10 00:00:00.000000','Media','EnRevision',NULL);
/*!40000 ALTER TABLE `asignaciones` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetroleclaims`
--

DROP TABLE IF EXISTS `aspnetroleclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetroleclaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `RoleId` int NOT NULL,
  `ClaimType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ClaimValue` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetRoleClaims_RoleId` (`RoleId`),
  CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetroleclaims`
--

LOCK TABLES `aspnetroleclaims` WRITE;
/*!40000 ALTER TABLE `aspnetroleclaims` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetroleclaims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetroles`
--

DROP TABLE IF EXISTS `aspnetroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetroles` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `RoleNameIndex` (`NormalizedName`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetroles`
--

LOCK TABLES `aspnetroles` WRITE;
/*!40000 ALTER TABLE `aspnetroles` DISABLE KEYS */;
INSERT INTO `aspnetroles` VALUES (1,'Administrador','ADMINISTRADOR',NULL),(2,'Director','DIRECTOR',NULL),(3,'Animador','ANIMADOR',NULL);
/*!40000 ALTER TABLE `aspnetroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserclaims`
--

DROP TABLE IF EXISTS `aspnetuserclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserclaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `ClaimType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ClaimValue` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetUserClaims_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserclaims`
--

LOCK TABLES `aspnetuserclaims` WRITE;
/*!40000 ALTER TABLE `aspnetuserclaims` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetuserclaims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserlogins`
--

DROP TABLE IF EXISTS `aspnetuserlogins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserlogins` (
  `LoginProvider` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProviderKey` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProviderDisplayName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `UserId` int NOT NULL,
  PRIMARY KEY (`LoginProvider`,`ProviderKey`),
  KEY `IX_AspNetUserLogins_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserlogins`
--

LOCK TABLES `aspnetuserlogins` WRITE;
/*!40000 ALTER TABLE `aspnetuserlogins` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetuserlogins` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserroles`
--

DROP TABLE IF EXISTS `aspnetuserroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserroles` (
  `UserId` int NOT NULL,
  `RoleId` int NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IX_AspNetUserRoles_RoleId` (`RoleId`),
  CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserroles`
--

LOCK TABLES `aspnetuserroles` WRITE;
/*!40000 ALTER TABLE `aspnetuserroles` DISABLE KEYS */;
INSERT INTO `aspnetuserroles` VALUES (1,1),(2,2),(16,2),(3,3),(4,3),(5,3),(6,3),(7,3),(8,3),(9,3),(10,3),(11,3),(12,3),(13,3),(15,3);
/*!40000 ALTER TABLE `aspnetuserroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetusers`
--

DROP TABLE IF EXISTS `aspnetusers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetusers` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Apellido` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Avatar` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Especialidad` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `UserName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedUserName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Email` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedEmail` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `SecurityStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PhoneNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL,
  `LockoutEnd` datetime(6) DEFAULT NULL,
  `LockoutEnabled` tinyint(1) NOT NULL,
  `AccessFailedCount` int NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `EmailIndex` (`NormalizedEmail`),
  UNIQUE KEY `UserNameIndex` (`NormalizedUserName`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetusers`
--

LOCK TABLES `aspnetusers` WRITE;
/*!40000 ALTER TABLE `aspnetusers` DISABLE KEYS */;
INSERT INTO `aspnetusers` VALUES (1,'Administrador','Genba',NULL,NULL,'admin@genba.test','ADMIN@GENBA.TEST','admin@genba.test','ADMIN@GENBA.TEST',1,'AQAAAAIAAYagAAAAEIp/VxbW8SQZzxTLVHJ4KWgVpF9acGbsFTpHp2WBQk1G7tLq98ZfpB+SLPxB9aqoTw==','H3IGRXTFTM4V7UAIFCI6A6V3GVKORRJY','033b445c-f5eb-48a7-842d-c2fc2abdeb5f',NULL,0,0,NULL,1,0),(2,'Akiyuki','Shinbo','/uploads/avatars/c0cb794c50b14e6c872e76df373e1cbd.webp',NULL,'director@genba.test','DIRECTOR@GENBA.TEST','director@genba.test','DIRECTOR@GENBA.TEST',1,'AQAAAAIAAYagAAAAEJModmboycNAanYjOE2ZEHTt2RpwxAfFn8qwGlGVRyjb/hmyyBdvVjlw6VkZq9Rx9g==','WGO4GTKQTN3JESAAHLEEYRLFI4ETWRWZ','adf6ef4f-258f-49a4-bee1-e8562a7fd6a8',NULL,0,0,NULL,1,0),(3,'Lucas','Animador',NULL,'Cleanup','animador1@genba.test','ANIMADOR1@GENBA.TEST','animador1@genba.test','ANIMADOR1@GENBA.TEST',1,'AQAAAAIAAYagAAAAEO1P7peJc0jI9H17Z4rAqiXaM7nF8m5DaxgQ4Wi/6B06mmUQxDzl6BwNTfkD2vZJ4Q==','RPVNCNPSNXBKWS2DKPZOIWBPKA5YCMC2','48808d50-dceb-4036-bc53-1f390ee2e6c1',NULL,0,0,NULL,1,0),(4,'Mia','Animador',NULL,'Color','animador2@genba.test','ANIMADOR2@GENBA.TEST','animador2@genba.test','ANIMADOR2@GENBA.TEST',1,'AQAAAAIAAYagAAAAEHnnpQSGk5BA6JA6GMgsHJ8aov8c0LuEQYsDuwTaXUX4V8774Zq8viPkusCEbXOoow==','4N6TYEZKP64BYABE2NUIWDRETLSXBNVH','c8031274-a9d0-4a8c-9c98-09f3410cb8e8',NULL,0,0,'2026-10-29 19:17:04.284773',1,0),(5,'Sara','Animador',NULL,'Layout','animador3@genba.test','ANIMADOR3@GENBA.TEST','animador3@genba.test','ANIMADOR3@GENBA.TEST',1,'AQAAAAIAAYagAAAAEPjCQ0n7VSpLpNf4bButSwm0ZeGjR/mhv7VP+Ii4zsTHp4qobSiU7h1Vfps2Za5ctA==','MMQICG2GR3QT4QIPV6DW2XLTQOG46HJ2','07f83a2b-2abf-43c3-87b5-e075efa3bc39',NULL,0,0,NULL,1,0),(6,'Mateo','Animador',NULL,'Key Animation','animador4@genba.test','ANIMADOR4@GENBA.TEST','animador4@genba.test','ANIMADOR4@GENBA.TEST',1,'AQAAAAIAAYagAAAAEKwv/bTNxEhnIhYWs1WU4CRS/fvz3sSTKqgvNcDnSEDHt4wdXPyF1w3C6Ldm7r4dsQ==','UW5J7SO2PMIUBEQBJQDGFT6AXZMMYVIP','96f7d6cb-3890-4311-adef-5eed99aa8840',NULL,0,0,NULL,1,0),(7,'Alicia','Animador',NULL,'Inbetween','animador5@genba.test','ANIMADOR5@GENBA.TEST','animador5@genba.test','ANIMADOR5@GENBA.TEST',1,'AQAAAAIAAYagAAAAED+r3J0bvEgBxba6jc+Ueh2LTaUY333IhtxECH4kV2Ig96iAoI7ljGBZk2wmDMtvJA==','AHL6NFKFL4JCA5CSR4C4LD3BS3ITACRJ','c44cbe98-09d6-4bc1-ad95-7cb5670ecd60',NULL,0,0,NULL,1,0),(8,'Noah','Animador',NULL,'Compositing','animador6@genba.test','ANIMADOR6@GENBA.TEST','animador6@genba.test','ANIMADOR6@GENBA.TEST',1,'AQAAAAIAAYagAAAAEKZhe5SrUFgLCBC3b1cJ1ibls7k00i6dH8JAHJ1JMr0l3+WsX0aPNUp/6M1VOfafsA==','L7K55N5NTROQ2S2MXSORWHTAZZ4WEF4O','0f56f477-375c-49f6-b6e9-24d57e3e83f7',NULL,0,0,NULL,1,0),(9,'Elena','Animador',NULL,'Cleanup','animador7@genba.test','ANIMADOR7@GENBA.TEST','animador7@genba.test','ANIMADOR7@GENBA.TEST',1,'AQAAAAIAAYagAAAAEFzEXhYP+/WmmrENlLM9dVdTS3eSyc4qo4ASu5hYNo/UjV4oFHD0HAnxmIXvyxg6rg==','6D44WQOLZWSMDJWXX4LRSLM5LBTKEHSQ','ece825a2-39f4-4215-ad19-8c6d431a79ce',NULL,0,0,NULL,1,0),(10,'Pablo','Animador',NULL,'Color','animador8@genba.test','ANIMADOR8@GENBA.TEST','animador8@genba.test','ANIMADOR8@GENBA.TEST',1,'AQAAAAIAAYagAAAAENzCSEMoFHYa0Mw8FTIRf1pbHJ5nmK2mHmAuxWERNj9bFSzPWdkCKPZzAwk/vQ0feA==','3PM76AS7QWWM74NKIEGM6BXR3YPFEK77','b25907ce-47e3-4e06-8e86-0ae9aaff7076',NULL,0,0,NULL,1,0),(11,'Leona','Animador',NULL,'Layout','animador9@genba.test','ANIMADOR9@GENBA.TEST','animador9@genba.test','ANIMADOR9@GENBA.TEST',1,'AQAAAAIAAYagAAAAEDa4dyPAKbXrZj3JDwjToiV2zYiM9bmZdndV+LwIngJzSVNWP19W2rwZFH75Mp+ddg==','QJ6AU7QBUYOMUWKHBRBXX3SZLGULBQYF','fc344209-3dbd-4bdc-9793-925f814da201',NULL,0,0,NULL,1,0),(12,'Iván','Animador',NULL,'Key Animation','animador10@genba.test','ANIMADOR10@GENBA.TEST','animador10@genba.test','ANIMADOR10@GENBA.TEST',1,'AQAAAAIAAYagAAAAELNxwzGtiIn3FK3UvdD1mX8C/S6POO2X6PtRpvmTosVKfE4M+n8adgk3v/3B43ELag==','7M3OKA5XIZ4NW3H4NVVI4LVOEX2S4W3N','5a4ecd37-c979-42a5-ba4c-e97b632cd295',NULL,0,0,NULL,1,0),(13,'Carmen','Animador',NULL,'Inbetween','animador11@genba.test','ANIMADOR11@GENBA.TEST','animador11@genba.test','ANIMADOR11@GENBA.TEST',1,'AQAAAAIAAYagAAAAEGgn3NZhRnZ78+z8X9kBSaQ4iUUQ58+npHtaZd7rzHbRvlNPKzDDQKX1kQoeImwTMA==','3HALXKMZEXPOSVBKMUQQRRFU5TWJYEOQ','19d848fe-06f7-45a6-98cd-d1dd7c4ea0e4',NULL,0,0,NULL,1,0),(15,'sususss','amogus',NULL,'Color','sususe@genba.test','SUSUSE@GENBA.TEST','sususe@genba.test','SUSUSE@GENBA.TEST',1,'AQAAAAIAAYagAAAAEDNbv8yAgsbT5BMo0SPD/yslvX0qi44teLVFbeaw1OZ6P8WXnz7V8Ji+TR2M7hrqNA==','ATZN2ZMTQP2OVJ5COLDBSUY65YNGVTZU','a448a646-735e-4fba-b3f8-8054ec276a6c',NULL,0,0,NULL,1,0),(16,'Atsuko','Ishizuka',NULL,NULL,'Director2@genba.test','DIRECTOR2@GENBA.TEST','Director2@genba.test','DIRECTOR2@GENBA.TEST',1,'AQAAAAIAAYagAAAAEJwbpsYfoW8M/HyvpbIVnv0hLx3ZQ5Xx26/VWKsTVCrDuRP3lXVQC9eiczzHF36kUQ==','6IHOSTJZOGW6P5XAFXNQJ676GJJ5CNRM','1a0c7f7d-a92a-40b8-8eb0-5be34477d316',NULL,0,0,NULL,1,0);
/*!40000 ALTER TABLE `aspnetusers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetusertokens`
--

DROP TABLE IF EXISTS `aspnetusertokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetusertokens` (
  `UserId` int NOT NULL,
  `LoginProvider` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Value` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`UserId`,`LoginProvider`,`Name`),
  CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetusertokens`
--

LOCK TABLES `aspnetusertokens` WRITE;
/*!40000 ALTER TABLE `aspnetusertokens` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetusertokens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `entregas`
--

DROP TABLE IF EXISTS `entregas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `entregas` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `AsignacionId` int NOT NULL,
  `RutaArchivo` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `NombreOriginal` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Version` int NOT NULL,
  `ComentarioAnimador` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `FechaSubida` datetime(6) NOT NULL,
  `EstadoRevision` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FeedbackDirector` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Entregas_AsignacionId_Version` (`AsignacionId`,`Version`),
  CONSTRAINT `FK_Entregas_Asignaciones_AsignacionId` FOREIGN KEY (`AsignacionId`) REFERENCES `asignaciones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `entregas`
--

LOCK TABLES `entregas` WRITE;
/*!40000 ALTER TABLE `entregas` DISABLE KEYS */;
INSERT INTO `entregas` VALUES (1,3,'entregas/b5d9c41164ba4252b92508bf368909fe.zip','E1_layout_v1.zip',1,'Layout de la escena 1.','2026-10-16 15:13:34.983786','Aprobada','Encuadre aprobado.'),(2,4,'entregas/b596942c7d2449d58dd49ebc12314497.zip','E1_keyanim_v1.zip',1,'Primera pasada de poses clave.','2026-10-16 15:13:34.984840','ConCorrecciones','Suavizar el arco de la caída del farol.'),(3,4,'entregas/ad558d969cab41d7aec67d25132c7ddd.zip','E1_keyanim_v2.zip',2,'Corregí el arco de la caída.','2026-10-24 15:13:34.985839','Aprobada','Ahora sí, aprobada.'),(4,5,'entregas/49a91afb58994f8d9328d0d0c14016c2.zip','E2_layout_v1.zip',1,'Layout de la subida.','2026-10-26 15:13:34.986622','Aprobada','Perfecto.'),(5,6,'entregas/f33a740531f34f328d6a402dda673aaf.zip','E2_keyanim_v1.zip',1,'Poses de la subida.','2026-11-02 15:13:34.988104','ConCorrecciones','El peso del farolero no se siente en los escalones; exagerar la inclinación.'),(6,7,'entregas/063e168a2f0b44b0949144e120ce973c.zip','E2_cleanup_v1.zip',1,'Limpieza lista, falta revisar el pelo.','2026-11-04 15:13:34.988793','Pendiente',NULL),(7,8,'entregas/9057e361c2ed45a981c6620dc32f630e.zip','E3_layout_v1.zip',1,'Cámara con leve movimiento hacia arriba.','2026-11-03 15:13:34.989423','Pendiente',NULL),(8,9,'entregas/8da368ce44df4ddab6377a4fd9bd20b6.zip','E3_keyanim_v1.zip',1,'Primera versión de la tormenta.','2026-11-01 15:13:34.989999','ConCorrecciones','Las ráfagas de viento deberían afectar el abrigo.'),(9,18,'entregas/6c6801aa84a646a1b5ecd51fb7653f0e.zip','Neo_keyanim_v1.zip',1,'Poses de la apertura.','2026-11-03 15:13:34.990967','Pendiente',NULL);
/*!40000 ALTER TABLE `entregas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `escenas`
--

DROP TABLE IF EXISTS `escenas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `escenas` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ProyectoId` int NOT NULL,
  `Numero` int NOT NULL,
  `Descripcion` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `DuracionSegundos` int NOT NULL,
  `Estado` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `RutaStoryboard` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Escenas_ProyectoId_Numero` (`ProyectoId`,`Numero`),
  CONSTRAINT `FK_Escenas_Proyectos_ProyectoId` FOREIGN KEY (`ProyectoId`) REFERENCES `proyectos` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `escenas`
--

LOCK TABLES `escenas` WRITE;
/*!40000 ALTER TABLE `escenas` DISABLE KEYS */;
INSERT INTO `escenas` VALUES (3,3,1,NULL,5,'Pendiente','storyboards/5a9a54d0dbad4caf92d2fa1ab0b6c52e.jpg'),(4,3,2,NULL,1,'EnProduccion',NULL),(5,6,1,'Amanecer en el puerto',8,'Finalizada',NULL),(6,6,2,'El farolero sube la escalera',15,'EnProduccion',NULL),(7,6,3,'La tormenta',20,'EnProduccion',NULL),(8,6,4,'El farol se apaga',10,'Pendiente',NULL),(9,6,5,'Créditos',6,'Pendiente',NULL),(10,7,1,'Intro',20,'Pendiente','storyboards/0b6aa8fc2c23457ea5f72b0c5e34cdae.jpg'),(11,7,2,'Estribillo',25,'Pendiente',NULL),(12,8,1,'Apertura',30,'EnProduccion',NULL);
/*!40000 ALTER TABLE `escenas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `proyectos`
--

DROP TABLE IF EXISTS `proyectos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `proyectos` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `DirectorId` int NOT NULL,
  `Nombre` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Descripcion` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `FechaInicio` datetime(6) NOT NULL,
  `FechaEntrega` datetime(6) NOT NULL,
  `Estado` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Proyectos_DirectorId` (`DirectorId`),
  CONSTRAINT `FK_Proyectos_AspNetUsers_DirectorId` FOREIGN KEY (`DirectorId`) REFERENCES `aspnetusers` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `proyectos`
--

LOCK TABLES `proyectos` WRITE;
/*!40000 ALTER TABLE `proyectos` DISABLE KEYS */;
INSERT INTO `proyectos` VALUES (2,16,'No Game No Life','descripcion asd','2013-01-24 00:00:00.000000','2014-11-07 00:00:00.000000','Planificacion'),(3,2,'proyecto test 1','ads','2026-11-03 00:00:00.000000','2026-12-03 00:00:00.000000','Planificacion'),(4,2,'proyecto test 2',NULL,'2026-11-03 00:00:00.000000','2026-12-03 00:00:00.000000','EnProduccion'),(5,2,'proyecto test 3',NULL,'2026-11-03 00:00:00.000000','2026-12-03 00:00:00.000000','Finalizado'),(6,2,'El último farol','Corto de 6 minutos sobre un farolero y el último faro del puerto.','2026-09-21 00:00:00.000000','2027-01-04 00:00:00.000000','EnProduccion'),(7,2,'Marea','Videoclip de 2 minutos, todavía en preproducción.','2026-11-15 00:00:00.000000','2027-03-05 00:00:00.000000','Planificacion'),(8,16,'Neo Tokio, capítulo 1','Primer capítulo de una serie de ciencia ficción.','2026-10-16 00:00:00.000000','2027-02-03 00:00:00.000000','EnProduccion');
/*!40000 ALTER TABLE `proyectos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tipostarea`
--

DROP TABLE IF EXISTS `tipostarea`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tipostarea` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(60) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Descripcion` varchar(300) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_TiposTarea_Nombre` (`Nombre`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tipostarea`
--

LOCK TABLES `tipostarea` WRITE;
/*!40000 ALTER TABLE `tipostarea` DISABLE KEYS */;
INSERT INTO `tipostarea` VALUES (1,'Layout','Composición de cámara y posiciones de la escena'),(2,'Key animation','Poses clave de la animación'),(3,'Cleanup','Limpieza y línea final sobre las claves'),(4,'Inbetween','Intercalado entre las poses clave'),(5,'Color','Coloreado de los frames'),(6,'Compositing','Composición final de capas y efectos');
/*!40000 ALTER TABLE `tipostarea` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'genba_studio'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-11-05 16:25:58

-- phpMyAdmin SQL script for PUSL2020_PAS_DB
-- Run this script in phpMyAdmin or MySQL Workbench.

CREATE DATABASE IF NOT EXISTS `PUSL2020_PAS_DB`
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;
USE `PUSL2020_PAS_DB`;

SET FOREIGN_KEY_CHECKS = 0;

CREATE TABLE `ResearchAreas` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) NOT NULL,
  `Description` varchar(500) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `Roles` (
  `Id` varchar(255) NOT NULL,
  `Name` varchar(256) DEFAULT NULL,
  `NormalizedName` varchar(256) DEFAULT NULL,
  `ConcurrencyStamp` longtext,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `Users` (
  `Id` varchar(255) NOT NULL,
  `FirstName` varchar(100) NOT NULL,
  `LastName` varchar(100) NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `Department` longtext,
  `IsActive` tinyint(1) NOT NULL,
  `UserName` varchar(256) DEFAULT NULL,
  `NormalizedUserName` varchar(256) DEFAULT NULL,
  `Email` varchar(256) DEFAULT NULL,
  `NormalizedEmail` varchar(256) DEFAULT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext,
  `SecurityStamp` longtext,
  `ConcurrencyStamp` longtext,
  `PhoneNumber` longtext,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL,
  `LockoutEnd` datetime(6) DEFAULT NULL,
  `LockoutEnabled` tinyint(1) NOT NULL,
  `AccessFailedCount` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `RoleClaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `RoleId` varchar(255) NOT NULL,
  `ClaimType` longtext,
  `ClaimValue` longtext,
  PRIMARY KEY (`Id`),
  KEY `IX_RoleClaims_RoleId` (`RoleId`),
  CONSTRAINT `FK_RoleClaims_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `Roles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `StudentProfiles` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` varchar(255) NOT NULL,
  `Bio` varchar(500) DEFAULT NULL,
  `IsGroupLead` tinyint(1) NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_StudentProfiles_UserId` (`UserId`),
  CONSTRAINT `FK_StudentProfiles_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `SupervisorProfiles` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` varchar(255) NOT NULL,
  `Expertise` varchar(500) DEFAULT NULL,
  `MaxProjects` int NOT NULL,
  `CurrentProjectCount` int NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_SupervisorProfiles_UserId` (`UserId`),
  CONSTRAINT `FK_SupervisorProfiles_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `UserClaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` varchar(255) NOT NULL,
  `ClaimType` longtext,
  `ClaimValue` longtext,
  PRIMARY KEY (`Id`),
  KEY `IX_UserClaims_UserId` (`UserId`),
  CONSTRAINT `FK_UserClaims_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `UserLogins` (
  `LoginProvider` varchar(255) NOT NULL,
  `ProviderKey` varchar(255) NOT NULL,
  `ProviderDisplayName` longtext,
  `UserId` varchar(255) NOT NULL,
  PRIMARY KEY (`LoginProvider`,`ProviderKey`),
  KEY `IX_UserLogins_UserId` (`UserId`),
  CONSTRAINT `FK_UserLogins_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `UserRoles` (
  `UserId` varchar(255) NOT NULL,
  `RoleId` varchar(255) NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IX_UserRoles_RoleId` (`RoleId`),
  CONSTRAINT `FK_UserRoles_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `Roles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_UserRoles_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `UserTokens` (
  `UserId` varchar(255) NOT NULL,
  `LoginProvider` varchar(255) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Value` longtext,
  PRIMARY KEY (`UserId`,`LoginProvider`,`Name`),
  CONSTRAINT `FK_UserTokens_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `GroupMembers` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `StudentProfileId` int NOT NULL,
  `GroupLeadProfileId` int NOT NULL,
  `JoinedDate` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_GroupMembers_GroupLeadProfileId` (`GroupLeadProfileId`),
  KEY `IX_GroupMembers_StudentProfileId` (`StudentProfileId`),
  CONSTRAINT `FK_GroupMembers_StudentProfiles_GroupLeadProfileId` FOREIGN KEY (`GroupLeadProfileId`) REFERENCES `StudentProfiles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_GroupMembers_StudentProfiles_StudentProfileId` FOREIGN KEY (`StudentProfileId`) REFERENCES `StudentProfiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `Projects` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Title` varchar(200) NOT NULL,
  `Abstract` varchar(2000) NOT NULL,
  `TechnicalStack` varchar(500) NOT NULL,
  `ResearchAreaId` int NOT NULL,
  `StudentProfileId` int DEFAULT NULL,
  `SubmittedByUserId` varchar(255) DEFAULT NULL,
  `Status` int NOT NULL,
  `SubmittedDate` datetime(6) NOT NULL,
  `WithdrawnDate` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Projects_ResearchAreaId` (`ResearchAreaId`),
  KEY `IX_Projects_Status` (`Status`),
  KEY `IX_Projects_StudentProfileId` (`StudentProfileId`),
  KEY `IX_Projects_SubmittedByUserId` (`SubmittedByUserId`),
  CONSTRAINT `FK_Projects_ResearchAreas_ResearchAreaId` FOREIGN KEY (`ResearchAreaId`) REFERENCES `ResearchAreas` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Projects_StudentProfiles_StudentProfileId` FOREIGN KEY (`StudentProfileId`) REFERENCES `StudentProfiles` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Projects_Users_SubmittedByUserId` FOREIGN KEY (`SubmittedByUserId`) REFERENCES `Users` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `SupervisorExpertises` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `SupervisorProfileId` int NOT NULL,
  `ResearchAreaId` int NOT NULL,
  `AddedDate` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_SupervisorExpertises_ResearchAreaId` (`ResearchAreaId`),
  KEY `IX_SupervisorExpertises_SupervisorProfileId` (`SupervisorProfileId`),
  CONSTRAINT `FK_SupervisorExpertises_ResearchAreas_ResearchAreaId` FOREIGN KEY (`ResearchAreaId`) REFERENCES `ResearchAreas` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_SupervisorExpertises_SupervisorProfiles_SupervisorProfileId` FOREIGN KEY (`SupervisorProfileId`) REFERENCES `SupervisorProfiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `Matches` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ProjectId` int NOT NULL,
  `SupervisorProfileId` int NOT NULL,
  `Status` int NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `ConfirmedDate` datetime(6) DEFAULT NULL,
  `RejectedDate` datetime(6) DEFAULT NULL,
  `RejectionReason` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Matches_ProjectId` (`ProjectId`),
  KEY `IX_Matches_Status` (`Status`),
  KEY `IX_Matches_SupervisorProfileId` (`SupervisorProfileId`),
  CONSTRAINT `FK_Matches_Projects_ProjectId` FOREIGN KEY (`ProjectId`) REFERENCES `Projects` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Matches_SupervisorProfiles_SupervisorProfileId` FOREIGN KEY (`SupervisorProfileId`) REFERENCES `SupervisorProfiles` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO `ResearchAreas` (`Id`, `CreatedDate`, `Description`, `IsActive`, `Name`) VALUES
  (1, '2026-04-16 06:55:40.751757', 'Machine Learning, Deep Learning, NLP', 1, 'Artificial Intelligence'),
  (2, '2026-04-16 06:55:40.751909', 'Network Security, Cryptography, Penetration Testing', 1, 'Cybersecurity'),
  (3, '2026-04-16 06:55:40.751909', 'Frontend, Backend, Full-stack Development', 1, 'Web Development'),
  (4, '2026-04-16 06:55:40.751909', 'AWS, Azure, Google Cloud, Distributed Systems', 1, 'Cloud Computing'),
  (5, '2026-04-16 06:55:40.751909', 'Big Data, Data Analytics, Visualization', 1, 'Data Science'),
  (6, '2026-04-16 06:55:40.751910', 'Arduino, Raspberry Pi, Embedded Linux', 1, 'IoT & Embedded Systems'),
  (7, '2026-04-16 06:55:40.751910', 'Cryptocurrency, Smart Contracts, DeFi', 1, 'Blockchain'),
  (8, '2026-04-16 06:55:40.751910', 'Unity, Unreal Engine, Game Design', 1, 'Game Development');

INSERT INTO `Roles` (`Id`, `ConcurrencyStamp`, `Name`, `NormalizedName`) VALUES
  ('1', 'd3defdcb-028f-45ba-a549-5a5fe0966d60', 'Admin', 'ADMIN'),
  ('2', '941758e7-7a71-46f3-b784-60f3401b1a22', 'ModuleLeader', 'MODULELEADER'),
  ('3', '91071e1f-caf0-42a3-9cda-e2178e916aad', 'Supervisor', 'SUPERVISOR'),
  ('4', 'd41dcb4c-83ff-4b53-9c7b-2cf19e2496fa', 'Student', 'STUDENT');

SET FOREIGN_KEY_CHECKS = 1;

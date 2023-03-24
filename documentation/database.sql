-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Gép: localhost
-- Létrehozás ideje: 2023. Már 24. 07:45
-- Kiszolgáló verziója: 10.4.27-MariaDB
-- PHP verzió: 8.2.0

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Adatbázis: `car_racing_tournament`
--
CREATE DATABASE IF NOT EXISTS `car_racing_tournament` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `car_racing_tournament`;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `Drivers`
--

DROP TABLE IF EXISTS `Drivers`;
CREATE TABLE `Drivers` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Name` longtext NOT NULL,
  `RealName` longtext DEFAULT NULL,
  `Number` int(11) NOT NULL,
  `ActualTeamId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `SeasonId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `Drivers`
--

INSERT INTO `Drivers` (`Id`, `Name`, `RealName`, `Number`, `ActualTeamId`, `SeasonId`) VALUES
('02a46c7f-e8db-4bee-9594-75a6bb4311e7', 'leclerc', 'Charles Leclerc', 98, 'bb0f78a2-c96e-4995-b84c-a68c9c5105dc', 'bd8cc085-2e18-4a7d-84e1-be5de33a52de'),
('8c3d9760-861a-4697-bb35-64c299acdae6', 'sainz', '', 99, 'bb0f78a2-c96e-4995-b84c-a68c9c5105dc', 'bd8cc085-2e18-4a7d-84e1-be5de33a52de'),
('ab201ce7-573c-450b-8514-b4eebee83655', 'lewisham', 'Lewis Hamilton', 44, '960a1b8c-2504-4e0e-99c4-cf978c0b856c', 'bd8cc085-2e18-4a7d-84e1-be5de33a52de');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `Permissions`
--

DROP TABLE IF EXISTS `Permissions`;
CREATE TABLE `Permissions` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `UserId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `SeasonId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Type` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `Permissions`
--

INSERT INTO `Permissions` (`Id`, `UserId`, `SeasonId`, `Type`) VALUES
('7779214e-8f1b-4181-8ab7-c1fad97f4765', '08db26a9-840c-42ee-82c5-ceec14c2a104', 'bd8cc085-2e18-4a7d-84e1-be5de33a52de', 1),
('d1ae948b-4b54-47db-9028-07fe9084b7ff', '08db26a9-9264-4fb6-88aa-4c547e6326dc', 'bd8cc085-2e18-4a7d-84e1-be5de33a52de', 0);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `Races`
--

DROP TABLE IF EXISTS `Races`;
CREATE TABLE `Races` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Name` longtext NOT NULL,
  `DateTime` datetime(6) DEFAULT NULL,
  `SeasonId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `Races`
--

INSERT INTO `Races` (`Id`, `Name`, `DateTime`, `SeasonId`) VALUES
('085075c1-afb1-425f-babc-7a12a0bcfb3f', 'Bahrein', '2023-03-17 06:25:33.752000', 'bd8cc085-2e18-4a7d-84e1-be5de33a52de'),
('cd04e3bd-f0bb-4376-878b-7cd07b53f342', 'Australia', '2023-03-17 06:25:33.752000', 'bd8cc085-2e18-4a7d-84e1-be5de33a52de');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `Results`
--

DROP TABLE IF EXISTS `Results`;
CREATE TABLE `Results` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Position` int(11) NOT NULL,
  `Point` int(11) NOT NULL,
  `DriverId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `TeamId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `RaceId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `Results`
--

INSERT INTO `Results` (`Id`, `Position`, `Point`, `DriverId`, `TeamId`, `RaceId`) VALUES
('0f649a5f-a051-4937-8c00-f42baf0e2e1a', 2, 18, 'ab201ce7-573c-450b-8514-b4eebee83655', '960a1b8c-2504-4e0e-99c4-cf978c0b856c', '085075c1-afb1-425f-babc-7a12a0bcfb3f'),
('6693d531-826c-4bd6-bb5d-88f2cb5231e6', 3, 15, '8c3d9760-861a-4697-bb35-64c299acdae6', 'bb0f78a2-c96e-4995-b84c-a68c9c5105dc', '085075c1-afb1-425f-babc-7a12a0bcfb3f'),
('f4d7c3e4-c335-4637-9fae-3f07172170e6', 1, 25, '02a46c7f-e8db-4bee-9594-75a6bb4311e7', 'bb0f78a2-c96e-4995-b84c-a68c9c5105dc', '085075c1-afb1-425f-babc-7a12a0bcfb3f');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `Seasons`
--

DROP TABLE IF EXISTS `Seasons`;
CREATE TABLE `Seasons` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Name` longtext NOT NULL,
  `Description` longtext DEFAULT NULL,
  `IsArchived` tinyint(1) NOT NULL DEFAULT 0,
  `CreatedAt` datetime(6) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `Seasons`
--

INSERT INTO `Seasons` (`Id`, `Name`, `Description`, `IsArchived`, `CreatedAt`) VALUES
('bd8cc085-2e18-4a7d-84e1-be5de33a52de', 'F1 League', 'Lorem ipsum dolor sit amet', 0, '2023-03-17 06:42:44.952626');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `Teams`
--

DROP TABLE IF EXISTS `Teams`;
CREATE TABLE `Teams` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Name` longtext NOT NULL,
  `Color` longtext NOT NULL,
  `SeasonId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `Teams`
--

INSERT INTO `Teams` (`Id`, `Name`, `Color`, `SeasonId`) VALUES
('960a1b8c-2504-4e0e-99c4-cf978c0b856c', 'Mercedes', '#000000', 'bd8cc085-2e18-4a7d-84e1-be5de33a52de'),
('bb0f78a2-c96e-4995-b84c-a68c9c5105dc', 'Ferrari', '#FF0000', 'bd8cc085-2e18-4a7d-84e1-be5de33a52de');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `Users`
--

DROP TABLE IF EXISTS `Users`;
CREATE TABLE `Users` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Username` varchar(255) NOT NULL,
  `Email` varchar(255) NOT NULL,
  `Password` longtext NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `Users`
--

INSERT INTO `Users` (`Id`, `Username`, `Email`, `Password`) VALUES
('08db26a9-840c-42ee-82c5-ceec14c2a104', 'leventenyiro', 'nyíro.levente@gmail.com', '$2a$10$HcK9moclQc2FZ7mu9lPsJumxY1rKrkD1hGqrGXSRwKWuGpwAtAOgC'),
('08db26a9-9264-4fb6-88aa-4c547e6326dc', 'test1', 'test@gmail.com', '$2a$10$6twuIy5Y5IGmi6D8loXutu/d4MixZLvT2DJ7n2SLcVGczbIJokH6O');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `__EFMigrationsHistory`
--

DROP TABLE IF EXISTS `__EFMigrationsHistory`;
CREATE TABLE `__EFMigrationsHistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `__EFMigrationsHistory`
--

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) VALUES
('20230316182011_init', '6.0.13');

--
-- Indexek a kiírt táblákhoz
--

--
-- A tábla indexei `Drivers`
--
ALTER TABLE `Drivers`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_Drivers_ActualTeamId` (`ActualTeamId`),
  ADD KEY `IX_Drivers_SeasonId` (`SeasonId`);

--
-- A tábla indexei `Permissions`
--
ALTER TABLE `Permissions`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_Permissions_SeasonId` (`SeasonId`),
  ADD KEY `IX_Permissions_UserId` (`UserId`);

--
-- A tábla indexei `Races`
--
ALTER TABLE `Races`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_Races_SeasonId` (`SeasonId`);

--
-- A tábla indexei `Results`
--
ALTER TABLE `Results`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_Results_DriverId` (`DriverId`),
  ADD KEY `IX_Results_RaceId` (`RaceId`),
  ADD KEY `IX_Results_TeamId` (`TeamId`);

--
-- A tábla indexei `Seasons`
--
ALTER TABLE `Seasons`
  ADD PRIMARY KEY (`Id`);

--
-- A tábla indexei `Teams`
--
ALTER TABLE `Teams`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_Teams_SeasonId` (`SeasonId`);

--
-- A tábla indexei `Users`
--
ALTER TABLE `Users`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `IX_Users_Email` (`Email`),
  ADD UNIQUE KEY `IX_Users_Username` (`Username`);

--
-- A tábla indexei `__EFMigrationsHistory`
--
ALTER TABLE `__EFMigrationsHistory`
  ADD PRIMARY KEY (`MigrationId`);

--
-- Megkötések a kiírt táblákhoz
--

--
-- Megkötések a táblához `Drivers`
--
ALTER TABLE `Drivers`
  ADD CONSTRAINT `FK_Drivers_Seasons_SeasonId` FOREIGN KEY (`SeasonId`) REFERENCES `Seasons` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_Drivers_Teams_ActualTeamId` FOREIGN KEY (`ActualTeamId`) REFERENCES `Teams` (`Id`) ON DELETE SET NULL;

--
-- Megkötések a táblához `Permissions`
--
ALTER TABLE `Permissions`
  ADD CONSTRAINT `FK_Permissions_Seasons_SeasonId` FOREIGN KEY (`SeasonId`) REFERENCES `Seasons` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_Permissions_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE;

--
-- Megkötések a táblához `Races`
--
ALTER TABLE `Races`
  ADD CONSTRAINT `FK_Races_Seasons_SeasonId` FOREIGN KEY (`SeasonId`) REFERENCES `Seasons` (`Id`) ON DELETE CASCADE;

--
-- Megkötések a táblához `Results`
--
ALTER TABLE `Results`
  ADD CONSTRAINT `FK_Results_Drivers_DriverId` FOREIGN KEY (`DriverId`) REFERENCES `Drivers` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_Results_Races_RaceId` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_Results_Teams_TeamId` FOREIGN KEY (`TeamId`) REFERENCES `Teams` (`Id`) ON DELETE CASCADE;

--
-- Megkötések a táblához `Teams`
--
ALTER TABLE `Teams`
  ADD CONSTRAINT `FK_Teams_Seasons_SeasonId` FOREIGN KEY (`SeasonId`) REFERENCES `Seasons` (`Id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

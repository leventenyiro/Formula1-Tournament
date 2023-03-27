INSERT INTO `Users` (`Id`, `Username`, `Email`, `Password`) VALUES
('08db26a9-840c-42ee-82c5-ceec14c2a104', 'leventenyiro', 'nyíro.levente@gmail.com', '$2a$10$HcK9moclQc2FZ7mu9lPsJumxY1rKrkD1hGqrGXSRwKWuGpwAtAOgC'),
('08db26a9-9264-4fb6-88aa-4c547e6326dc', 'test1', 'test@gmail.com', '$2a$10$6twuIy5Y5IGmi6D8loXutu/d4MixZLvT2DJ7n2SLcVGczbIJokH6O');

INSERT INTO `Seasons` (`Id`, `Name`, `Description`, `IsArchived`, `CreatedAt`) VALUES
('bd8cc085-2e18-4a7d-84e1-be5de33a52de', 'F1 League', 'Lorem ipsum dolor sit amet', 0, '2023-03-17 06:42:44.952626');

INSERT INTO `Permissions` (`Id`, `UserId`, `SeasonId`, `Type`) VALUES
('7779214e-8f1b-4181-8ab7-c1fad97f4765', '08db26a9-840c-42ee-82c5-ceec14c2a104', 'bd8cc085-2e18-4a7d-84e1-be5de33a52de', 1),
('d1ae948b-4b54-47db-9028-07fe9084b7ff', '08db26a9-9264-4fb6-88aa-4c547e6326dc', 'bd8cc085-2e18-4a7d-84e1-be5de33a52de', 0);

INSERT INTO `Races` (`Id`, `Name`, `DateTime`, `SeasonId`) VALUES
('085075c1-afb1-425f-babc-7a12a0bcfb3f', 'Bahrein', '2023-03-17 06:25:33.752000', 'bd8cc085-2e18-4a7d-84e1-be5de33a52de'),
('cd04e3bd-f0bb-4376-878b-7cd07b53f342', 'Australia', '2023-03-17 06:25:33.752000', 'bd8cc085-2e18-4a7d-84e1-be5de33a52de');

INSERT INTO `Teams` (`Id`, `Name`, `Color`, `SeasonId`) VALUES
('960a1b8c-2504-4e0e-99c4-cf978c0b856c', 'Mercedes', '#000000', 'bd8cc085-2e18-4a7d-84e1-be5de33a52de'),
('bb0f78a2-c96e-4995-b84c-a68c9c5105dc', 'Ferrari', '#FF0000', 'bd8cc085-2e18-4a7d-84e1-be5de33a52de');

INSERT INTO `Drivers` (`Id`, `Name`, `RealName`, `Number`, `ActualTeamId`, `SeasonId`) VALUES
('02a46c7f-e8db-4bee-9594-75a6bb4311e7', 'leclerc', 'Charles Leclerc', 98, 'bb0f78a2-c96e-4995-b84c-a68c9c5105dc', 'bd8cc085-2e18-4a7d-84e1-be5de33a52de'),
('8c3d9760-861a-4697-bb35-64c299acdae6', 'sainz', '', 99, 'bb0f78a2-c96e-4995-b84c-a68c9c5105dc', 'bd8cc085-2e18-4a7d-84e1-be5de33a52de'),
('ab201ce7-573c-450b-8514-b4eebee83655', 'lewisham', 'Lewis Hamilton', 44, '960a1b8c-2504-4e0e-99c4-cf978c0b856c', 'bd8cc085-2e18-4a7d-84e1-be5de33a52de');

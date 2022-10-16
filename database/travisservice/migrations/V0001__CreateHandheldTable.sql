create table HandheldEvents(
Id int primary key auto_increment,
HandheldId varchar(50) not null,
Longitude decimal(10, 10) not null,
Latitude decimal(10, 10) not null,
Timestamp datetime not null,
DateTimeAdded datetime default current_timestamp);

create index IX_HandheldEvents_HandheldId_Timestamp on HandheldEvents(HandheldId, Timestamp);
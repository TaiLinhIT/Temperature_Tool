CREATE TABLE [dbo].[dv_BusDataTemp](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdMachine] [int] NOT NULL,
	[IdStyle] [int] NOT NULL,
	[Channel] [nvarchar](50) NOT NULL,
	[Factory] [nvarchar](100) NOT NULL,
	[Line] [nvarchar](100) NOT NULL,
	[AddressMachine] [int] NOT NULL,
	[LineCode] [nvarchar](50) NOT NULL,
	[Port] [nvarchar](50) NOT NULL,
	[Temp] [float] NOT NULL,
	[Max] [int] NOT NULL,
	[Min] [int] NOT NULL,
	[Baudrate] [int] NOT NULL,
	[UploadDate] [datetime] NOT NULL,
	[IsWarning] [bit] NOT NULL,
	[Sensor_Typeid] [int] NOT NULL,
	[Sensor_kind] [nvarchar](63) NULL,
	[Sensor_ant] [nvarchar](63) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO





CREATE TABLE [dbo].[dv_style](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NameStyle] [nvarchar](50) NOT NULL,
	[ShoesMax] [numeric](18, 6) NOT NULL,
	[ShoesMin] [numeric](18, 6) NOT NULL,
	[SoleMax] [numeric](18, 6) NOT NULL,
	[SoleMin] [numeric](18, 6) NOT NULL,
	[devid] [nvarchar](255) NULL,
	[Standard_temp] [nvarchar](255) NULL,
	[Compensate_Vaild] [numeric](18, 6) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[dv_Machine_Temp](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Port] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Line] [nvarchar](100) NOT NULL,
	[Baudrate] [int] NOT NULL,
	[Address] [int] NOT NULL,
	[LineCode] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO





create table dv_MapFactoryAddress(
    Id int primary key identity(1,1),
    Factory nvarchar(50) not null,
    Address int not null,
    Status bit not null
);


CREATE TABLE dv_Factory_Configs (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FactoryCode NVARCHAR(50) NOT NULL,
    Line NVARCHAR(100) NOT NULL,
    Address INT NOT NULL
);


CREATE TABLE dv_BusDataTemp_Configs (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Channel NVARCHAR(50) NOT NULL,
    Company NVARCHAR(100) NOT NULL,
    Port NVARCHAR(50) NOT NULL,
    CommandWrite NVARCHAR(100) NOT NULL,
	Min FLOAT NOT NULL,
	Max FLOAT NOT NULL,
	DisplayName NVARCHAR(100) NOT NULL
);
CREATE TABLE dv_Device_Configs(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Company NVARCHAR(50) NOT NULL,
    AddressMachine FLOAT NOT NULL
);


INSERT INTO dv_BusDataTemp_Configs (Channel, Company, Port, CommandWrite, Min, Max, DisplayName) VALUES
('CH1', 'VA1', 'COM4', '01 03 00 18 00 02', -60, 60, 'A1'),
('CH2', 'VA1', 'COM4', '01 03 00 38 00 03', -60, 60, 'A2'),
('CH3', 'VA1', 'COM4', '01 03 00 58 00 04', -60, 60, 'A3'),
('CH4', 'VA1', 'COM4', '01 03 00 78 00 05', -60, 60, 'A4'),
('CH5', 'VA1', 'COM4', '01 03 00 98 00 06', -60, 60, 'A5'),
('CH6', 'VA1', 'COM4', '01 03 00 118 00 07', -60, 60, 'A6'),
('CH7', 'VA1', 'COM4', '01 03 00 138 00 08', -60, 60,'A7')


CREATE TABLE dv_Machine_Configs (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Address INT NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Port NVARCHAR(100) NOT NULL,
    [Line] [nvarchar](100) NOT NULL,
    Baudrate NVARCHAR(100) NOT NULL,
    [LineCode] [nvarchar](50) NOT NULL
);

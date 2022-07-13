USE [master]
GO

/****** Object:  Database [IMTDTemplate]    Script Date: 2/17/2021 2:04:33 PM ******/
CREATE DATABASE [IMTDTemplate]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'IMTDTemplate', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\IMTDTemplate.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'IMTDTemplate_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\IMTDTemplate_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO

ALTER DATABASE [IMTDTemplate] SET COMPATIBILITY_LEVEL = 130
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [IMTDTemplate].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [IMTDTemplate] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [IMTDTemplate] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [IMTDTemplate] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [IMTDTemplate] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [IMTDTemplate] SET ARITHABORT OFF 
GO

ALTER DATABASE [IMTDTemplate] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [IMTDTemplate] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [IMTDTemplate] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [IMTDTemplate] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [IMTDTemplate] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [IMTDTemplate] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [IMTDTemplate] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [IMTDTemplate] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [IMTDTemplate] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [IMTDTemplate] SET  DISABLE_BROKER 
GO

ALTER DATABASE [IMTDTemplate] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [IMTDTemplate] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [IMTDTemplate] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [IMTDTemplate] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [IMTDTemplate] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [IMTDTemplate] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [IMTDTemplate] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [IMTDTemplate] SET RECOVERY FULL 
GO

ALTER DATABASE [IMTDTemplate] SET  MULTI_USER 
GO

ALTER DATABASE [IMTDTemplate] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [IMTDTemplate] SET DB_CHAINING OFF 
GO

ALTER DATABASE [IMTDTemplate] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [IMTDTemplate] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [IMTDTemplate] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [IMTDTemplate] SET QUERY_STORE = OFF
GO

USE [IMTDTemplate]
GO

ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO

ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO

ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO

ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO

ALTER DATABASE [IMTDTemplate] SET  READ_WRITE 
GO


USE [IMTDTemplate]
GO

/****** Object:  Table [dbo].[Employee_Term_Type]    Script Date: 2/17/2021 2:09:12 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Employee_Term_Type](
	[Employee_Term_Type_Id] [int] IDENTITY(1,1) NOT NULL,
	[Employee_Term_Type_Code] [varchar](15) NOT NULL,
	[Employee_Term_Type_English_Name] [varchar](256) NOT NULL,
	[Employee_Term_Type_French_Name] [varchar](256) NOT NULL,
	[Employee_Term_Type_Sort_Order] [int] NOT NULL,
	[Employee_Term_Type_Active_Boolean] [bit] NOT NULL,
 CONSTRAINT [PK_Employee_Term_Type] PRIMARY KEY CLUSTERED 
(
	[Employee_Term_Type_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [Employee_Type] UNIQUE NONCLUSTERED 
(
	[Employee_Term_Type_Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Employee_Term_Type] ADD  CONSTRAINT [Employee_Term_Type_Active_Boolean_ONE_DF]  DEFAULT ((1)) FOR [Employee_Term_Type_Active_Boolean]
GO

USE [IMTDTemplate]
GO

/****** Object:  Table [dbo].[Province]    Script Date: 2/17/2021 2:10:09 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Province](
	[Province_Id] [int] IDENTITY(1,1) NOT NULL,
	[Province_Code] [varchar](15) NOT NULL,
	[Province_English_Name] [varchar](256) NOT NULL,
	[Province_French_Name] [varchar](256) NOT NULL,
	[Province_Sort_Order] [int] NOT NULL,
	[Province_Active_Boolean] [bit] NOT NULL,
 CONSTRAINT [PK_Province] PRIMARY KEY CLUSTERED 
(
	[Province_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_Province] UNIQUE NONCLUSTERED 
(
	[Province_Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Province] ADD  CONSTRAINT [Province_Active_Boolean_ONE_DF]  DEFAULT ((1)) FOR [Province_Active_Boolean]
GO

USE [IMTDTemplate]
GO

/****** Object:  Table [dbo].[Employee]    Script Date: 2/17/2021 2:10:50 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Employee](
	[Employee_Id] [int] IDENTITY(1,1) NOT NULL,
	[Employee_First_Name] [varchar](50) NOT NULL,
	[Employee_Last_Name] [varchar](50) NOT NULL,
	[Employee_Term_Type_Id] [int] NOT NULL,
	[Employee_Start_Date] [datetime] NOT NULL,
	[Employee_Job_Description] [varchar](1000) NULL,
	[Employee_Province_Id] [int] NOT NULL,
	[Employee_Active_Boolean] [bit] NOT NULL,
 CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED 
(
	[Employee_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Employee] ADD  CONSTRAINT [Employee_Active_Boolean_ONE_DF]  DEFAULT ((1)) FOR [Employee_Active_Boolean]
GO

ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [Employee_Term_Employee_fk] FOREIGN KEY([Employee_Term_Type_Id])
REFERENCES [dbo].[Employee_Term_Type] ([Employee_Term_Type_Id])
GO

ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [Employee_Term_Employee_fk]
GO

ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [Province_Employee_fk] FOREIGN KEY([Employee_Province_Id])
REFERENCES [dbo].[Province] ([Province_Id])
GO

ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [Province_Employee_fk]
GO


USE [IMTDTemplate]
GO
-- add some lookups
INSERT INTO [dbo].[Province] ([Province_Code],[Province_English_Name],[Province_French_Name], [Province_Sort_Order], [Province_Active_Boolean]) VALUES ('ON', 'Ontario', 'Ontario', 1, 1)
INSERT INTO [dbo].[Province] ([Province_Code],[Province_English_Name],[Province_French_Name], [Province_Sort_Order], [Province_Active_Boolean]) VALUES ('QC', 'Quebec', 'Québec', 2, 1)
INSERT INTO [dbo].[Province] ([Province_Code],[Province_English_Name],[Province_French_Name], [Province_Sort_Order], [Province_Active_Boolean]) VALUES ('NS', 'Nova Scotia', 'Nouvelle-écosse', 3, 1)
INSERT INTO [dbo].[Province] ([Province_Code],[Province_English_Name],[Province_French_Name], [Province_Sort_Order], [Province_Active_Boolean]) VALUES ('MB', 'Manitoba', 'Manitoba', 4, 1)
INSERT INTO [dbo].[Province] ([Province_Code],[Province_English_Name],[Province_French_Name], [Province_Sort_Order], [Province_Active_Boolean]) VALUES ('BC', 'British Comlumbia', 'Colombie-Britannique', 5, 1)
INSERT INTO [dbo].[Province] ([Province_Code],[Province_English_Name],[Province_French_Name], [Province_Sort_Order], [Province_Active_Boolean]) VALUES ('PE', 'Prince Edward Island', 'Île-du-Prince-Édouard', 6, 1)
INSERT INTO [dbo].[Province] ([Province_Code],[Province_English_Name],[Province_French_Name], [Province_Sort_Order], [Province_Active_Boolean]) VALUES ('SK', 'Saskatchewan', 'Saskatchewan', 7, 1)
INSERT INTO [dbo].[Province] ([Province_Code],[Province_English_Name],[Province_French_Name], [Province_Sort_Order], [Province_Active_Boolean]) VALUES ('AB', 'Alberta', 'Alberta', 8, 1)
INSERT INTO [dbo].[Province] ([Province_Code],[Province_English_Name],[Province_French_Name], [Province_Sort_Order], [Province_Active_Boolean]) VALUES ('NL', 'Newfoundland and Labrador', 'Terre-Neuve-et-Labrador', 9, 1)
INSERT INTO [dbo].[Province] ([Province_Code],[Province_English_Name],[Province_French_Name], [Province_Sort_Order], [Province_Active_Boolean]) VALUES ('NB', 'New Brunswick', 'Nouveau-brunswick', 10, 1)

INSERT INTO [dbo].[Employee_Term_Type] ([Employee_Term_Type_Code],[Employee_Term_Type_English_Name],[Employee_Term_Type_French_Name], [Employee_Term_Type_Sort_Order], [Employee_Term_Type_Active_Boolean]) VALUES ('FT', 'Full-time', 'À temps plein', 1, 1)
INSERT INTO [dbo].[Employee_Term_Type] ([Employee_Term_Type_Code],[Employee_Term_Type_English_Name],[Employee_Term_Type_French_Name], [Employee_Term_Type_Sort_Order], [Employee_Term_Type_Active_Boolean]) VALUES ('PT', 'Part-time', 'Temps partiel', 2, 1)

-- add an emppoyee
INSERT INTO [dbo].[Employee]
           ([Employee_First_Name]
           ,[Employee_Last_Name]
           ,[Employee_Term_Type_Id]
           ,[Employee_Start_Date]
           ,[Employee_Job_Description]
           ,[Employee_Province_Id]
           ,[Employee_Active_Boolean])
     VALUES
           ('John11'
           ,'Smith22'
           ,1
           ,'2021-02-12 00:00:00.000'
           ,'.NET Developer'
           ,1
           ,1)
GO

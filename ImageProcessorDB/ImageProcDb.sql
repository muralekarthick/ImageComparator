USE [master]
GO
/****** Object:  Database [ImageProcdb]    Script Date: 21-12-2019 16:36:52 ******/
CREATE DATABASE [ImageProcdb]
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ImageProcdb].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ImageProcdb] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ImageProcdb] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ImageProcdb] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ImageProcdb] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ImageProcdb] SET ARITHABORT OFF 
GO
ALTER DATABASE [ImageProcdb] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ImageProcdb] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ImageProcdb] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ImageProcdb] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ImageProcdb] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ImageProcdb] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ImageProcdb] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ImageProcdb] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ImageProcdb] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ImageProcdb] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ImageProcdb] SET ALLOW_SNAPSHOT_ISOLATION ON 
GO
ALTER DATABASE [ImageProcdb] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ImageProcdb] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [ImageProcdb] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ImageProcdb] SET  MULTI_USER 
GO
ALTER DATABASE [ImageProcdb] SET DB_CHAINING OFF 
GO
ALTER DATABASE [ImageProcdb] SET ENCRYPTION ON
GO
ALTER DATABASE [ImageProcdb] SET QUERY_STORE = ON
GO
ALTER DATABASE [ImageProcdb] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 7), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 10, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO)
GO
USE [ImageProcdb]
GO
/****** Object:  Schema [ImgProc]    Script Date: 21-12-2019 16:37:07 ******/
CREATE SCHEMA [ImgProc]
GO
/****** Object:  Table [ImgProc].[CsxFileInfo]    Script Date: 21-12-2019 16:37:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ImgProc].[CsxFileInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [nvarchar](2048) NULL,
	[FilePath] [nvarchar](max) NULL,
	[FileStatusCode] [tinyint] NULL,
	[CreatedOn] [datetime] NULL,
	[UpdatedOn] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [ImgProc].[FileRecordInfo]    Script Date: 21-12-2019 16:37:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ImgProc].[FileRecordInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileId] [int] NOT NULL,
	[ImagePath1] [nvarchar](max) NULL,
	[ImagePath2] [nvarchar](max) NULL,
	[Score] [decimal](5, 2) NULL,
	[RecordStatusCode] [tinyint] NULL,
	[CreatedOn] [datetime] NULL,
	[UpdatedOn] [datetime] NULL,
	[TransactionId] [uniqueidentifier] NULL,
	[ElapsedTime] [decimal](6, 3) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [ImgProc].[FileRecordStatusCode]    Script Date: 21-12-2019 16:37:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ImgProc].[FileRecordStatusCode](
	[StatusCode] [tinyint] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](15) NULL,
PRIMARY KEY CLUSTERED 
(
	[StatusCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [ImgProc].[FileStatusCode]    Script Date: 21-12-2019 16:37:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ImgProc].[FileStatusCode](
	[StatusCode] [tinyint] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](15) NULL,
PRIMARY KEY CLUSTERED 
(
	[StatusCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [ImgProc].[CsxFileInfo]  WITH CHECK ADD FOREIGN KEY([FileStatusCode])
REFERENCES [ImgProc].[FileStatusCode] ([StatusCode])
GO
ALTER TABLE [ImgProc].[FileRecordInfo]  WITH CHECK ADD FOREIGN KEY([FileId])
REFERENCES [ImgProc].[CsxFileInfo] ([Id])
GO
ALTER TABLE [ImgProc].[FileRecordInfo]  WITH CHECK ADD FOREIGN KEY([RecordStatusCode])
REFERENCES [ImgProc].[FileRecordStatusCode] ([StatusCode])
GO
/****** Object:  StoredProcedure [ImgProc].[SPReadytoPick]    Script Date: 21-12-2019 16:37:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [ImgProc].[SPReadytoPick]
(
 @id int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
	declare @ImageIDs Table (Id int, ImagePath1 varchar(100), ImagePath2 varchar(100))

    SET NOCOUNT ON
    -- Insert statements for procedure here
	INSERT INTO @ImageIDs
    SELECT i.Id, 
	i.ImagePath1,
	i.ImagePath2
	from [ImgProc].FileRecordInfo i 
	where i.RecordStatusCode = 1 and i.FileId = @id

	Update [ImgProc].FileRecordInfo
	set RecordStatusCode = 2 
	where Id in (Select Id from @ImageIDs)

	SELECT * FROM @ImageIDs
END
GO
/****** Object:  StoredProcedure [ImgProc].[SPReadytoPickFile]    Script Date: 21-12-2019 16:37:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [ImgProc].[SPReadytoPickFile]
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
	declare @ImageIDs Table (Id int, FileName nvarchar(2048))

    SET NOCOUNT ON
    -- Insert statements for procedure here
	INSERT INTO @ImageIDs
    SELECT i.Id, 
	i.FileName
	from [ImgProc].CsxFileInfo i 
	where i.FileStatusCode = 1

	Update [ImgProc].CsxFileInfo
	set FileStatusCode = 2 
	where Id in (Select Id from @ImageIDs)

	SELECT * FROM @ImageIDs
END
GO
/****** Object:  StoredProcedure [ImgProc].[UpdateData]    Script Date: 21-12-2019 16:37:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [ImgProc].[UpdateData]
(
    -- Add the parameters for the stored procedure here
    @id int,
	@score decimal(5,2),
	@elapsedTime decimal(6,3),
	@status tinyint,
	@transactionid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
	SET NOCOUNT ON
	update ImgProc.FileRecordInfo
	set Score = @score,
	ElapsedTime = @elapsedTime,
	RecordStatusCode = @status,
	TransactionId = @transactionid
	Where Id = @id
END
GO
/****** Object:  StoredProcedure [ImgProc].[USP_GetFileRecords]    Script Date: 21-12-2019 16:37:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [ImgProc].[USP_GetFileRecords]	
	 @fileId int
AS
BEGIN
SELECT [ImagePath1]
      ,[ImagePath2]
      ,[Score]
      ,[ElapsedTime]
	  ,[RecordStatusCode]
      FROM [ImgProc].[FileRecordInfo] where [FileId] = @fileId
END
GO
/****** Object:  StoredProcedure [ImgProc].[USP_UpdateCsxFilePath]    Script Date: 21-12-2019 16:37:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [ImgProc].[USP_UpdateCsxFilePath]	
	 @fileId int,
	 @FilePath nvarchar(max)
AS
BEGIN
	UPDATE [ImgProc].[CsxFileInfo] SET FilePath = @FilePath, FileStatusCode = 3 where Id = @fileId
END
GO
USE [master]
GO
ALTER DATABASE [ImageProcdb] SET  READ_WRITE 
GO

/*********** [ImgProc].[FileRecordStatusCode] insert scripts ******************************/
USE [ImageProcdb]
GO
SET IDENTITY_INSERT [ImgProc].[FileRecordStatusCode] ON 
GO
INSERT [ImgProc].[FileRecordStatusCode] ([StatusCode], [Description]) VALUES (1, N'New')
GO
INSERT [ImgProc].[FileRecordStatusCode] ([StatusCode], [Description]) VALUES (2, N'In Progress')
GO
INSERT [ImgProc].[FileRecordStatusCode] ([StatusCode], [Description]) VALUES (3, N'Completed')
GO
INSERT [ImgProc].[FileRecordStatusCode] ([StatusCode], [Description]) VALUES (4, N'Failed')
GO
SET IDENTITY_INSERT [ImgProc].[FileRecordStatusCode] OFF
GO
/*********** [ImgProc].[FileStatusCode] insert scripts ******************************/
SET IDENTITY_INSERT [ImgProc].[FileStatusCode] ON 
GO
INSERT [ImgProc].[FileStatusCode] ([StatusCode], [Description]) VALUES (1, N'New')
GO
INSERT [ImgProc].[FileStatusCode] ([StatusCode], [Description]) VALUES (2, N'In Progress')
GO
INSERT [ImgProc].[FileStatusCode] ([StatusCode], [Description]) VALUES (3, N'Completed')
GO
INSERT [ImgProc].[FileStatusCode] ([StatusCode], [Description]) VALUES (4, N'Failed')
GO
SET IDENTITY_INSERT [ImgProc].[FileStatusCode] OFF
GO

/******************************************************************************
    Vodigi - Open Source Interactive Digital Signage

    Copyright (C) 2005-2012  JMC Publications, LLC

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*******************************************************************************/


USE [master]
GO
/****** Object:  Database [Vodigi]    Script Date: 03/20/2012 08:59:21 ******/
/** UPDATE on 2020-02-19 to run on docker */
CREATE DATABASE [Vodigi] ON  PRIMARY 
( NAME = N'Vodigi', FILENAME = N'/var/opt/mssql/data/Vodigi.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'Vodigi_log', FILENAME = N'/var/opt/mssql/data/Vodigi_log.ldf' , SIZE = 1536KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [Vodigi] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Vodigi].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Vodigi] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [Vodigi] SET ANSI_NULLS OFF
GO
ALTER DATABASE [Vodigi] SET ANSI_PADDING OFF
GO
ALTER DATABASE [Vodigi] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [Vodigi] SET ARITHABORT OFF
GO
ALTER DATABASE [Vodigi] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [Vodigi] SET AUTO_CREATE_STATISTICS ON
GO
ALTER DATABASE [Vodigi] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [Vodigi] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [Vodigi] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [Vodigi] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [Vodigi] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [Vodigi] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [Vodigi] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [Vodigi] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [Vodigi] SET  DISABLE_BROKER
GO
ALTER DATABASE [Vodigi] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [Vodigi] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [Vodigi] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [Vodigi] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [Vodigi] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [Vodigi] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [Vodigi] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [Vodigi] SET  READ_WRITE
GO
ALTER DATABASE [Vodigi] SET RECOVERY FULL
GO
ALTER DATABASE [Vodigi] SET  MULTI_USER
GO
ALTER DATABASE [Vodigi] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [Vodigi] SET DB_CHAINING OFF
GO
USE [Vodigi]
GO
/****** Object:  Table [dbo].[Video]    Script Date: 03/20/2012 08:59:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Video](
	[VideoID] [int] IDENTITY(1000000,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[OriginalFilename] [nvarchar](128) NOT NULL,
	[StoredFilename] [nvarchar](128) NOT NULL,
	[VideoName] [nvarchar](128) NOT NULL,
	[Tags] [nvarchar](128) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Video] PRIMARY KEY CLUSTERED 
(
	[VideoID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Video_AccountID] ON [dbo].[Video] 
(
	[AccountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Video_IsActive] ON [dbo].[Video] 
(
	[IsActive] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Video_Tags] ON [dbo].[Video] 
(
	[Tags] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Video_VideoName] ON [dbo].[Video] 
(
	[VideoName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 03/20/2012 08:59:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[UserID] [int] IDENTITY(1000000,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[Username] [nvarchar](20) NOT NULL,
	[Password] [nvarchar](20) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[EmailAddress] [nvarchar](200) NOT NULL,
	[IsAdmin] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_User_AccountID] ON [dbo].[User] 
(
	[AccountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_User_IsActive] ON [dbo].[User] 
(
	[IsActive] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_User_Password] ON [dbo].[User] 
(
	[Password] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_User_Username] ON [dbo].[User] 
(
	[Username] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SlideShowImageXref]    Script Date: 03/20/2012 08:59:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SlideShowImageXref](
	[SlideShowImageXrefID] [int] IDENTITY(1000000,1) NOT NULL,
	[SlideShowID] [int] NOT NULL,
	[ImageID] [int] NOT NULL,
	[PlayOrder] [int] NOT NULL,
 CONSTRAINT [PK_SlideShowImageXref] PRIMARY KEY CLUSTERED 
(
	[SlideShowImageXrefID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SlideShowImageXref_ImageID] ON [dbo].[SlideShowImageXref] 
(
	[ImageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SlideShowImageXref_SlideShowID] ON [dbo].[SlideShowImageXref] 
(
	[SlideShowID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SlideShow]    Script Date: 03/20/2012 08:59:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SlideShow](
	[SlideShowID] [int] IDENTITY(1000000,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[SlideShowName] [nvarchar](128) NOT NULL,
	[Tags] [nvarchar](128) NOT NULL,
	[IntervalInSecs] [int] NOT NULL,
	[TransitionType] [nvarchar](30) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_SlideShow] PRIMARY KEY CLUSTERED 
(
	[SlideShowID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SlideShow_AccountID] ON [dbo].[SlideShow] 
(
	[AccountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SlideShow_IsActive] ON [dbo].[SlideShow] 
(
	[IsActive] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SlideShow_SlideShowName] ON [dbo].[SlideShow] 
(
	[SlideShowName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SlideShow_Tags] ON [dbo].[SlideShow] 
(
	[Tags] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ScreenScreenContentXref]    Script Date: 03/20/2012 08:59:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScreenScreenContentXref](
	[ScreenScreenContentXrefID] [int] IDENTITY(1000000,1) NOT NULL,
	[ScreenID] [int] NOT NULL,
	[ScreenContentID] [int] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
 CONSTRAINT [PK_ScreenScreenContentXref] PRIMARY KEY CLUSTERED 
(
	[ScreenScreenContentXrefID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ScreenScreenContentXref_ScreenContentID] ON [dbo].[ScreenScreenContentXref] 
(
	[ScreenContentID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ScreenScreenContentXref_ScreenID] ON [dbo].[ScreenScreenContentXref] 
(
	[ScreenID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ScreenContentType]    Script Date: 03/20/2012 08:59:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScreenContentType](
	[ScreenContentTypeID] [int] IDENTITY(1000000,1) NOT NULL,
	[ScreenContentTypeName] [nvarchar](64) NOT NULL,
 CONSTRAINT [PK_ScreenContentType] PRIMARY KEY CLUSTERED 
(
	[ScreenContentTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ScreenContent]    Script Date: 03/20/2012 08:59:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScreenContent](
	[ScreenContentID] [int] IDENTITY(1000000,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[ScreenContentTypeID] [int] NOT NULL,
	[ScreenContentName] [nvarchar](64) NOT NULL,
	[ScreenContentTitle] [nvarchar](64) NOT NULL,
	[ThumbnailImageID] [int] NOT NULL,
	[CustomField1] [nvarchar](256) NOT NULL,
	[CustomField2] [nvarchar](256) NOT NULL,
	[CustomField3] [nvarchar](256) NOT NULL,
	[CustomField4] [nvarchar](256) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ScreenContent] PRIMARY KEY CLUSTERED 
(
	[ScreenContentID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ScreenContent_AccountID] ON [dbo].[ScreenContent] 
(
	[AccountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ScreenContent_ScreenContentName] ON [dbo].[ScreenContent] 
(
	[ScreenContentName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ScreenContent_ScreenContentTypeID] ON [dbo].[ScreenContent] 
(
	[ScreenContentTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Screen]    Script Date: 03/20/2012 08:59:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Screen](
	[ScreenID] [int] IDENTITY(1000000,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[ScreenName] [nvarchar](128) NOT NULL,
	[ScreenDescription] [nvarchar](1024) NOT NULL,
	[SlideShowID] [int] NOT NULL,
	[PlayListID] [int] NOT NULL,
	[IsInteractive] [bit] NOT NULL,
	[ButtonImageID] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Screen] PRIMARY KEY CLUSTERED 
(
	[ScreenID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Screen_AccountID] ON [dbo].[Screen] 
(
	[AccountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Screen_IsActive] ON [dbo].[Screen] 
(
	[IsActive] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Screen_IsInteractive] ON [dbo].[Screen] 
(
	[IsInteractive] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Screen_ScreenName] ON [dbo].[Screen] 
(
	[ScreenName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PlayListVideoXref]    Script Date: 03/20/2012 08:59:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlayListVideoXref](
	[PlayListVideoXrefID] [int] IDENTITY(1000000,1) NOT NULL,
	[PlayListID] [int] NOT NULL,
	[VideoID] [int] NOT NULL,
	[PlayOrder] [int] NOT NULL,
 CONSTRAINT [PK_PlayListVideoXref] PRIMARY KEY CLUSTERED 
(
	[PlayListVideoXrefID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PlayListVideoXref_PlayListID] ON [dbo].[PlayListVideoXref] 
(
	[PlayListID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PlayListVideoXref_VideoID] ON [dbo].[PlayListVideoXref] 
(
	[VideoID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PlayList]    Script Date: 03/20/2012 08:59:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlayList](
	[PlayListID] [int] IDENTITY(1000000,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[PlayListName] [nvarchar](128) NOT NULL,
	[Tags] [nvarchar](128) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_PlayList] PRIMARY KEY CLUSTERED 
(
	[PlayListID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PlayList_AccountID] ON [dbo].[PlayList] 
(
	[AccountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PlayList_IsActive] ON [dbo].[PlayList] 
(
	[IsActive] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PlayList_PlayListName] ON [dbo].[PlayList] 
(
	[PlayListName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PlayList_Tags] ON [dbo].[PlayList] 
(
	[Tags] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PlayerGroupSchedule]    Script Date: 03/20/2012 08:59:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlayerGroupSchedule](
	[PlayerGroupScheduleID] [int] IDENTITY(1000000,1) NOT NULL,
	[PlayerGroupID] [int] NOT NULL,
	[ScreenID] [int] NOT NULL,
	[Day] [int] NOT NULL,
	[Hour] [int] NOT NULL,
	[Minute] [int] NOT NULL,
 CONSTRAINT [PK_PlayerGroupSchedule] PRIMARY KEY CLUSTERED 
(
	[PlayerGroupScheduleID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PlayerGroupSchedule_PlayerGroupID] ON [dbo].[PlayerGroupSchedule] 
(
	[PlayerGroupID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PlayerGroupSchedule_ScreenID] ON [dbo].[PlayerGroupSchedule] 
(
	[ScreenID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PlayerGroup]    Script Date: 03/20/2012 08:59:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlayerGroup](
	[PlayerGroupID] [int] IDENTITY(1000000,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[PlayerGroupName] [nvarchar](64) NOT NULL,
	[PlayerGroupDescription] [nvarchar](1024) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_PlayerGroup] PRIMARY KEY CLUSTERED 
(
	[PlayerGroupID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PlayerGroup_AccountID] ON [dbo].[PlayerGroup] 
(
	[AccountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PlayerGroup_IsActive] ON [dbo].[PlayerGroup] 
(
	[IsActive] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PlayerGroup_PlayerGroupName] ON [dbo].[PlayerGroup] 
(
	[PlayerGroupName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Player]    Script Date: 03/20/2012 08:59:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Player](
	[PlayerID] [int] IDENTITY(1000000,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[PlayerGroupID] [int] NOT NULL,
	[PlayerName] [nvarchar](128) NOT NULL,
	[PlayerLocation] [nvarchar](128) NOT NULL,
	[PlayerDescription] [nvarchar](1024) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Player] PRIMARY KEY CLUSTERED 
(
	[PlayerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Player_AccountID] ON [dbo].[Player] 
(
	[AccountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Player_IsActive] ON [dbo].[Player] 
(
	[IsActive] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Player_PlayerGroupID] ON [dbo].[Player] 
(
	[PlayerGroupID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Player_PlayerName] ON [dbo].[Player] 
(
	[PlayerName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Image]    Script Date: 03/20/2012 08:59:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Image](
	[ImageID] [int] IDENTITY(1000000,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[OriginalFilename] [nvarchar](128) NOT NULL,
	[StoredFilename] [nvarchar](128) NOT NULL,
	[ImageName] [nvarchar](128) NOT NULL,
	[Tags] [nvarchar](128) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Image] PRIMARY KEY CLUSTERED 
(
	[ImageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Image_AccountID] ON [dbo].[Image] 
(
	[AccountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Image_ImageName] ON [dbo].[Image] 
(
	[ImageName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Image_IsActive] ON [dbo].[Image] 
(
	[IsActive] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Image_Tags] ON [dbo].[Image] 
(
	[Tags] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Account]    Script Date: 03/20/2012 08:59:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Account](
	[AccountID] [int] IDENTITY(1000000,1) NOT NULL,
	[AccountName] [nvarchar](128) NOT NULL,
	[AccountDescription] [nvarchar](1024) NOT NULL,
	[FTPServer] [nvarchar](256) NOT NULL,
	[FTPUsername] [nvarchar](32) NOT NULL,
	[FTPPassword] [nvarchar](32) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED 
(
	[AccountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Account_AccountName] ON [dbo].[Account] 
(
	[AccountName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Account_IsActive] ON [dbo].[Account] 
(
	[IsActive] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Default [DF_Video_AccountID]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Video] ADD  CONSTRAINT [DF_Video_AccountID]  DEFAULT ((0)) FOR [AccountID]
GO
/****** Object:  Default [DF_Video_OriginalFilename]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Video] ADD  CONSTRAINT [DF_Video_OriginalFilename]  DEFAULT ('') FOR [OriginalFilename]
GO
/****** Object:  Default [DF_Video_StoredFilename]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Video] ADD  CONSTRAINT [DF_Video_StoredFilename]  DEFAULT ('') FOR [StoredFilename]
GO
/****** Object:  Default [DF_Video_VideoName]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Video] ADD  CONSTRAINT [DF_Video_VideoName]  DEFAULT ('') FOR [VideoName]
GO
/****** Object:  Default [DF_Video_Tags]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Video] ADD  CONSTRAINT [DF_Video_Tags]  DEFAULT ('') FOR [Tags]
GO
/****** Object:  Default [DF_Video_IsActive]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Video] ADD  CONSTRAINT [DF_Video_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
/****** Object:  Default [DF_User_AccountID]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_AccountID]  DEFAULT ((0)) FOR [AccountID]
GO
/****** Object:  Default [DF_User_Username]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_Username]  DEFAULT ('') FOR [Username]
GO
/****** Object:  Default [DF_User_Password]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_Password]  DEFAULT ('') FOR [Password]
GO
/****** Object:  Default [DF_User_FirstName]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_FirstName]  DEFAULT ('') FOR [FirstName]
GO
/****** Object:  Default [DF_User_LastName]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_LastName]  DEFAULT ('') FOR [LastName]
GO
/****** Object:  Default [DF_User_EmailAddress]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_EmailAddress]  DEFAULT ('') FOR [EmailAddress]
GO
/****** Object:  Default [DF_User_IsAdmin]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_IsAdmin]  DEFAULT ((0)) FOR [IsAdmin]
GO
/****** Object:  Default [DF_User_IsActive]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
/****** Object:  Default [DF_SlideShow_AccountID]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[SlideShow] ADD  CONSTRAINT [DF_SlideShow_AccountID]  DEFAULT ((0)) FOR [AccountID]
GO
/****** Object:  Default [DF_SlideShow_SlideShowName]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[SlideShow] ADD  CONSTRAINT [DF_SlideShow_SlideShowName]  DEFAULT ('') FOR [SlideShowName]
GO
/****** Object:  Default [DF_SlideShow_Tags]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[SlideShow] ADD  CONSTRAINT [DF_SlideShow_Tags]  DEFAULT ('') FOR [Tags]
GO
/****** Object:  Default [DF_SlideShow_IntervalInSecs]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[SlideShow] ADD  CONSTRAINT [DF_SlideShow_IntervalInSecs]  DEFAULT ((10)) FOR [IntervalInSecs]
GO
/****** Object:  Default [DF_SlideShow_TransitionType]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[SlideShow] ADD  CONSTRAINT [DF_SlideShow_TransitionType]  DEFAULT ('') FOR [TransitionType]
GO
/****** Object:  Default [DF_SlideShow_IsActive]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[SlideShow] ADD  CONSTRAINT [DF_SlideShow_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
/****** Object:  Default [DF_ScreenScreenContentXref_ScreenID]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[ScreenScreenContentXref] ADD  CONSTRAINT [DF_ScreenScreenContentXref_ScreenID]  DEFAULT ((0)) FOR [ScreenID]
GO
/****** Object:  Default [DF_ScreenScreenContentXref_ScreenContentID]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[ScreenScreenContentXref] ADD  CONSTRAINT [DF_ScreenScreenContentXref_ScreenContentID]  DEFAULT ((0)) FOR [ScreenContentID]
GO
/****** Object:  Default [DF_ScreenScreenContentXref_DisplayOrder]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[ScreenScreenContentXref] ADD  CONSTRAINT [DF_ScreenScreenContentXref_DisplayOrder]  DEFAULT ((1)) FOR [DisplayOrder]
GO
/****** Object:  Default [DF_ScreenContent_AccountID]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[ScreenContent] ADD  CONSTRAINT [DF_ScreenContent_AccountID]  DEFAULT ((0)) FOR [AccountID]
GO
/****** Object:  Default [DF_ScreenContent_ScreenContentTypeID]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[ScreenContent] ADD  CONSTRAINT [DF_ScreenContent_ScreenContentTypeID]  DEFAULT ((0)) FOR [ScreenContentTypeID]
GO
/****** Object:  Default [DF_ScreenContent_ScreenContentName]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[ScreenContent] ADD  CONSTRAINT [DF_ScreenContent_ScreenContentName]  DEFAULT ('') FOR [ScreenContentName]
GO
/****** Object:  Default [DF_ScreenContent_ScreenContentTitle]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[ScreenContent] ADD  CONSTRAINT [DF_ScreenContent_ScreenContentTitle]  DEFAULT ('') FOR [ScreenContentTitle]
GO
/****** Object:  Default [DF_ScreenContent_ThumbnailImageID]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[ScreenContent] ADD  CONSTRAINT [DF_ScreenContent_ThumbnailImageID]  DEFAULT ((0)) FOR [ThumbnailImageID]
GO
/****** Object:  Default [DF_ScreenContent_CustomField1]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[ScreenContent] ADD  CONSTRAINT [DF_ScreenContent_CustomField1]  DEFAULT ('') FOR [CustomField1]
GO
/****** Object:  Default [DF_ScreenContent_CustomField2]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[ScreenContent] ADD  CONSTRAINT [DF_ScreenContent_CustomField2]  DEFAULT ('') FOR [CustomField2]
GO
/****** Object:  Default [DF_ScreenContent_CustomField3]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[ScreenContent] ADD  CONSTRAINT [DF_ScreenContent_CustomField3]  DEFAULT ('') FOR [CustomField3]
GO
/****** Object:  Default [DF_ScreenContent_CustomField4]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[ScreenContent] ADD  CONSTRAINT [DF_ScreenContent_CustomField4]  DEFAULT ('') FOR [CustomField4]
GO
/****** Object:  Default [DF_ScreenContent_IsActive]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[ScreenContent] ADD  CONSTRAINT [DF_ScreenContent_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
/****** Object:  Default [DF_Screen_ScreenName]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Screen] ADD  CONSTRAINT [DF_Screen_ScreenName]  DEFAULT ('') FOR [ScreenName]
GO
/****** Object:  Default [DF_Screen_ScreenDescription]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Screen] ADD  CONSTRAINT [DF_Screen_ScreenDescription]  DEFAULT ('') FOR [ScreenDescription]
GO
/****** Object:  Default [DF_Screen_IsInteractive]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Screen] ADD  CONSTRAINT [DF_Screen_IsInteractive]  DEFAULT ((0)) FOR [IsInteractive]
GO
/****** Object:  Default [DF_Screen_ButtonImageID]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Screen] ADD  CONSTRAINT [DF_Screen_ButtonImageID]  DEFAULT ((0)) FOR [ButtonImageID]
GO
/****** Object:  Default [DF_Screen_IsActive]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Screen] ADD  CONSTRAINT [DF_Screen_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
/****** Object:  Default [DF_PlayList_AccountID]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[PlayList] ADD  CONSTRAINT [DF_PlayList_AccountID]  DEFAULT ((0)) FOR [AccountID]
GO
/****** Object:  Default [DF_PlayList_PlayListName]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[PlayList] ADD  CONSTRAINT [DF_PlayList_PlayListName]  DEFAULT ('') FOR [PlayListName]
GO
/****** Object:  Default [DF_PlayList_Tags]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[PlayList] ADD  CONSTRAINT [DF_PlayList_Tags]  DEFAULT ('') FOR [Tags]
GO
/****** Object:  Default [DF_PlayList_IsActive]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[PlayList] ADD  CONSTRAINT [DF_PlayList_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
/****** Object:  Default [DF_PlayerGroup_PlayerGroupName]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[PlayerGroup] ADD  CONSTRAINT [DF_PlayerGroup_PlayerGroupName]  DEFAULT ('') FOR [PlayerGroupName]
GO
/****** Object:  Default [DF_PlayerGroup_PlayerGroupDescription]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[PlayerGroup] ADD  CONSTRAINT [DF_PlayerGroup_PlayerGroupDescription]  DEFAULT ('') FOR [PlayerGroupDescription]
GO
/****** Object:  Default [DF_PlayerGroup_IsActive]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[PlayerGroup] ADD  CONSTRAINT [DF_PlayerGroup_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
/****** Object:  Default [DF_Player_PlayerName]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Player] ADD  CONSTRAINT [DF_Player_PlayerName]  DEFAULT ('') FOR [PlayerName]
GO
/****** Object:  Default [DF_Player_PlayerLocation]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Player] ADD  CONSTRAINT [DF_Player_PlayerLocation]  DEFAULT ('') FOR [PlayerLocation]
GO
/****** Object:  Default [DF_Player_PlayerDescription]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Player] ADD  CONSTRAINT [DF_Player_PlayerDescription]  DEFAULT ('') FOR [PlayerDescription]
GO
/****** Object:  Default [DF_Player_IsActive]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Player] ADD  CONSTRAINT [DF_Player_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
/****** Object:  Default [DF_Image_AccountID]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Image] ADD  CONSTRAINT [DF_Image_AccountID]  DEFAULT ((0)) FOR [AccountID]
GO
/****** Object:  Default [DF_Image_OriginalFilename]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Image] ADD  CONSTRAINT [DF_Image_OriginalFilename]  DEFAULT ('') FOR [OriginalFilename]
GO
/****** Object:  Default [DF_Image_StoredFilename]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Image] ADD  CONSTRAINT [DF_Image_StoredFilename]  DEFAULT ('') FOR [StoredFilename]
GO
/****** Object:  Default [DF_Image_ImageName]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Image] ADD  CONSTRAINT [DF_Image_ImageName]  DEFAULT ('') FOR [ImageName]
GO
/****** Object:  Default [DF_Image_Tags]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Image] ADD  CONSTRAINT [DF_Image_Tags]  DEFAULT ('') FOR [Tags]
GO
/****** Object:  Default [DF_Image_IsActive]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Image] ADD  CONSTRAINT [DF_Image_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
/****** Object:  Default [DF_Account_AccountName]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Account] ADD  CONSTRAINT [DF_Account_AccountName]  DEFAULT ('') FOR [AccountName]
GO
/****** Object:  Default [DF_Account_AccountDescription]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Account] ADD  CONSTRAINT [DF_Account_AccountDescription]  DEFAULT ('') FOR [AccountDescription]
GO
/****** Object:  Default [DF_Account_FTPServer]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Account] ADD  CONSTRAINT [DF_Account_FTPServer]  DEFAULT ('') FOR [FTPServer]
GO
/****** Object:  Default [DF_Account_FTPUsername]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Account] ADD  CONSTRAINT [DF_Account_FTPUsername]  DEFAULT ('') FOR [FTPUsername]
GO
/****** Object:  Default [DF_Account_FTPPassword]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Account] ADD  CONSTRAINT [DF_Account_FTPPassword]  DEFAULT ('') FOR [FTPPassword]
GO
/****** Object:  Default [DF_Account_IsActive]    Script Date: 03/20/2012 08:59:22 ******/
ALTER TABLE [dbo].[Account] ADD  CONSTRAINT [DF_Account_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

USE [Vodigi]

--**********************************************
-- Add the TimelineID column to the Screen table
ALTER TABLE [dbo].[Screen] ADD [TimelineID] [int] NOT NULL DEFAULT 0 WITH VALUES


--**********************************************
-- Create the DatabaseVersion table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DatabaseVersion](
	[DatabaseVersionID] [int] IDENTITY(1,1) NOT NULL,
	[Version] [nvarchar](50) NOT NULL,
	[DateInstalled] [datetime] NOT NULL,
 CONSTRAINT [PK_DatabaseVersion] PRIMARY KEY CLUSTERED 
(
	[DatabaseVersionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DatabaseVersion] ADD  CONSTRAINT [DF_DatabaseVersion_DatabaseVersion]  DEFAULT ('') FOR [Version]
GO
ALTER TABLE [dbo].[DatabaseVersion] ADD  CONSTRAINT [DF_DatabaseVersion_DateInstalled]  DEFAULT (getutcdate()) FOR [DateInstalled]
GO

INSERT INTO DatabaseVersion (Version, DateInstalled) VALUES ('6.0', GETUTCDATE())
GO

--**********************************************
-- Create the PlayerSetting table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PlayerSetting](
	[PlayerSettingID] [int] IDENTITY(1000000,1) NOT NULL,
	[PlayerID] [int] NOT NULL,
	[PlayerSettingName] [nvarchar](64) NOT NULL,
	[PlayerSettingTypeID] [int] NOT NULL,
	[PlayerSettingValue] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_PlayerSetting] PRIMARY KEY CLUSTERED 
(
	[PlayerSettingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PlayerSetting] ADD  CONSTRAINT [DF_PlayerSetting_PlayerSettingName]  DEFAULT ('') FOR [PlayerSettingName]
GO


--**********************************************
-- Create the PlayerSettingAccountDefault table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PlayerSettingAccountDefault](
	[PlayerSettingAccountDefaultID] [int] IDENTITY(1000000,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[PlayerSettingName] [nvarchar](64) NOT NULL,
	[PlayerSettingTypeID] [int] NOT NULL,
	[PlayerSettingAccountDefaultValue] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_PlayerSettingAccountDefault] PRIMARY KEY CLUSTERED 
(
	[PlayerSettingAccountDefaultID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


--**********************************************
-- Create the PlayerSettingSystemDefault table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PlayerSettingSystemDefault](
	[PlayerSettingSystemDefaultID] [int] IDENTITY(1000000,1) NOT NULL,
	[PlayerSettingName] [nvarchar](64) NOT NULL,
	[PlayerSettingTypeID] [int] NOT NULL,
	[PlayerSettingSystemDefaultValue] [nvarchar](256) NOT NULL,
	[PlayerSettingDescription] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_PlayerSettingSystemDefault] PRIMARY KEY CLUSTERED 
(
	[PlayerSettingSystemDefaultID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PlayerSettingSystemDefault] ADD  CONSTRAINT [DF_PlayerSettingSystemDefault_PlayerSettingDescription]  DEFAULT ('') FOR [PlayerSettingDescription]
GO


--**********************************************
--  Create and Populate the PlayerSettingType table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PlayerSettingType](
	[PlayerSettingTypeID] [int] IDENTITY(1000000,1) NOT NULL,
	[PlayerSettingTypeName] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_PlayerSettingType] PRIMARY KEY CLUSTERED 
(
	[PlayerSettingTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PlayerSettingType] ADD  CONSTRAINT [DF_PlayerSettingType_PlayerSettingType]  DEFAULT ('') FOR [PlayerSettingTypeName]
GO

INSERT INTO PlayerSettingType (PlayerSettingTypeName) VALUES ('Integer')
INSERT INTO PlayerSettingType (PlayerSettingTypeName) VALUES ('String')
INSERT INTO PlayerSettingType (PlayerSettingTypeName) VALUES ('Float')
INSERT INTO PlayerSettingType (PlayerSettingTypeName) VALUES ('Boolean')
GO


--**********************************************
-- Create the Timeline table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Timeline](
	[TimelineID] [int] IDENTITY(1000000,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[TimelineName] [nvarchar](128) NOT NULL,
	[Tags] [nvarchar](128) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[DurationInSecs] [int] NOT NULL,
	[MuteMusicOnPlayback] [bit] NOT NULL,
 CONSTRAINT [PK_Timeline] PRIMARY KEY CLUSTERED 
(
	[TimelineID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Timeline] ADD  CONSTRAINT [DF_Timeline_TimelineName]  DEFAULT ('') FOR [TimelineName]
GO
ALTER TABLE [dbo].[Timeline] ADD  CONSTRAINT [DF_Timeline_Tags]  DEFAULT ('') FOR [Tags]
GO
ALTER TABLE [dbo].[Timeline] ADD  CONSTRAINT [DF_Timeline_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Timeline] ADD  CONSTRAINT [DF_Timeline_DurationInSecs]  DEFAULT ((10)) FOR [DurationInSecs]
GO
ALTER TABLE [dbo].[Timeline] ADD  CONSTRAINT [DF_Timeline_MuteMusicOnPlayback]  DEFAULT ((0)) FOR [MuteMusicOnPlayback]
GO


--**********************************************
-- Create the TimelineImageXref table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TimelineImageXref](
	[TimelineImageXrefID] [int] IDENTITY(1000000,1) NOT NULL,
	[TimelineID] [int] NOT NULL,
	[ImageID] [int] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
 CONSTRAINT [PK_TimelineImageXref] PRIMARY KEY CLUSTERED 
(
	[TimelineImageXrefID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


--**********************************************
-- Create the TimelineMusicXref table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TimelineMusicXref](
	[TimelineMusicXrefID] [int] IDENTITY(1000000,1) NOT NULL,
	[TimelineID] [int] NOT NULL,
	[MusicID] [int] NOT NULL,
	[PlayOrder] [int] NOT NULL,
 CONSTRAINT [PK_TimelineMusicXref] PRIMARY KEY CLUSTERED 
(
	[TimelineMusicXrefID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


--**********************************************
-- Create the TimelineVideoXref table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TimelineVideoXref](
	[TimelineVideoXrefID] [int] IDENTITY(1000000,1) NOT NULL,
	[TimelineID] [int] NOT NULL,
	[VideoID] [int] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
 CONSTRAINT [PK_TimelineVideoXref] PRIMARY KEY CLUSTERED 
(
	[TimelineVideoXrefID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


--**********************************************
-- Insert the Timeline Screen Content Type
SET IDENTITY_INSERT [dbo].[ScreenContentType] ON

DECLARE @RowCount INT
SET @RowCount = (SELECT COUNT(*) FROM [dbo].[ScreenContentType] WHERE ScreenContentTypeID = 1000008)
IF @RowCount = 0
	BEGIN
		INSERT INTO [dbo].[ScreenContentType]
			(ScreenContentTypeID, ScreenContentTypeName)
		VALUES
			(1000008, 'Timeline')
	END

SET IDENTITY_INSERT [dbo].[ScreenContentType] OFF


--**********************************************
-- Set up the system default player settings
SET @RowCount = (SELECT COUNT(*) FROM PlayerSettingSystemDefault WHERE PlayerSettingName = 'DownloadFolder')
IF @RowCount = 0
	BEGIN
		INSERT INTO PlayerSettingSystemDefault (PlayerSettingName, PlayerSettingTypeID, PlayerSettingSystemDefaultValue, PlayerSettingDescription)
		VALUES('DownloadFolder', 1000001, 'C:\osVodigi\', 'Set to the location where downloaded media is stored on the player - this must be a local drive/folder')
	END

SET @RowCount = (SELECT COUNT(*) FROM PlayerSettingSystemDefault WHERE PlayerSettingName = 'ShowCursor')
IF @RowCount = 0
	BEGIN
		INSERT INTO PlayerSettingSystemDefault (PlayerSettingName, PlayerSettingTypeID, PlayerSettingSystemDefaultValue, PlayerSettingDescription)
		VALUES('ShowCursor', 1000003, 'False', 'Set to True to have the cursor show in the Player (you will seldom want to do this). The default is False')
	END

SET @RowCount = (SELECT COUNT(*) FROM PlayerSettingSystemDefault WHERE PlayerSettingName = 'MediaSourceUrl')
IF @RowCount = 0
	BEGIN
		INSERT INTO PlayerSettingSystemDefault (PlayerSettingName, PlayerSettingTypeID, PlayerSettingSystemDefaultValue, PlayerSettingDescription)
		VALUES('MediaSourceUrl', 1000001, 'http://yourwebserverIP/osVodigiWeb/Media/', 'Set to the full URL of the Vodigi Administrator''s Media folder. You need to modify this setting for your environment.')
	END

SET @RowCount = (SELECT COUNT(*) FROM PlayerSettingSystemDefault WHERE PlayerSettingName = 'ButtonTextOpen')
IF @RowCount = 0
	BEGIN
		INSERT INTO PlayerSettingSystemDefault (PlayerSettingName, PlayerSettingTypeID, PlayerSettingSystemDefaultValue, PlayerSettingDescription)
		VALUES('ButtonTextOpen', 1000001, 'Open', 'Set to the text used for both text display and voice recognition for the ''Open'' button.')
	END

SET @RowCount = (SELECT COUNT(*) FROM PlayerSettingSystemDefault WHERE PlayerSettingName = 'ButtonTextClose')
IF @RowCount = 0
	BEGIN
		INSERT INTO PlayerSettingSystemDefault (PlayerSettingName, PlayerSettingTypeID, PlayerSettingSystemDefaultValue, PlayerSettingDescription)
		VALUES('ButtonTextClose', 1000001, 'Close', 'Set to the text used for both text display and voice recognition for the ''Close'' button.')
	END

SET @RowCount = (SELECT COUNT(*) FROM PlayerSettingSystemDefault WHERE PlayerSettingName = 'ButtonTextNext')
IF @RowCount = 0
	BEGIN
		INSERT INTO PlayerSettingSystemDefault (PlayerSettingName, PlayerSettingTypeID, PlayerSettingSystemDefaultValue, PlayerSettingDescription)
		VALUES('ButtonTextNext', 1000001, 'Next', 'Set to the text used for both text display and voice recognition for the ''Next'' button.')
	END

SET @RowCount = (SELECT COUNT(*) FROM PlayerSettingSystemDefault WHERE PlayerSettingName = 'ButtonTextBack')
IF @RowCount = 0
	BEGIN
		INSERT INTO PlayerSettingSystemDefault (PlayerSettingName, PlayerSettingTypeID, PlayerSettingSystemDefaultValue, PlayerSettingDescription)
		VALUES('ButtonTextBack', 1000001, 'Back', 'Set to the text used for both text display and voice recognition for the ''Back'' button.')
	END





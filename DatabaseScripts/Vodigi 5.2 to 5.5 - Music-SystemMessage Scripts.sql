

------------------------------------------------------------------------------------------
-- Music Scripts
------------------------------------------------------------------------------------------
USE [Vodigi]
GO

/****** Object:  Table [dbo].[SlideShowMusicXref]    Script Date: 10/26/2012 11:25:37 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SlideShowMusicXref](
	[SlideShowMusicXrefID] [int] IDENTITY(1000000,1) NOT NULL,
	[SlideShowID] [int] NOT NULL,
	[MusicID] [int] NOT NULL,
	[PlayOrder] [int] NOT NULL,
 CONSTRAINT [PK_SlideShowMusicXref] PRIMARY KEY CLUSTERED 
(
	[SlideShowMusicXrefID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

USE [Vodigi]
GO

/****** Object:  Index [IX_SlideShowMusicXref_SlideShowID]    Script Date: 10/26/2012 11:25:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_SlideShowMusicXref_SlideShowID] ON [dbo].[SlideShowMusicXref]
(
	[SlideShowID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


USE [Vodigi]
GO

/****** Object:  Table [dbo].[Music]    Script Date: 10/26/2012 11:26:10 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Music](
	[MusicID] [int] IDENTITY(1000000,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[OriginalFilename] [nvarchar](128) NOT NULL,
	[StoredFilename] [nvarchar](128) NOT NULL,
	[MusicName] [nvarchar](128) NOT NULL,
	[Tags] [nvarchar](128) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Music] PRIMARY KEY CLUSTERED 
(
	[MusicID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Music] ADD  CONSTRAINT [DF_Music_OriginalFilename]  DEFAULT ('') FOR [OriginalFilename]
GO

ALTER TABLE [dbo].[Music] ADD  CONSTRAINT [DF_Music_StoredFilename]  DEFAULT ('') FOR [StoredFilename]
GO

ALTER TABLE [dbo].[Music] ADD  CONSTRAINT [DF_Music_MusicName]  DEFAULT ('') FOR [MusicName]
GO

ALTER TABLE [dbo].[Music] ADD  CONSTRAINT [DF_Music_Tags]  DEFAULT ('') FOR [Tags]
GO

ALTER TABLE [dbo].[Music] ADD  CONSTRAINT [DF_Music_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Music_AccountID]    Script Date: 10/26/2012 11:26:38 AM ******/
CREATE NONCLUSTERED INDEX [IX_Music_AccountID] ON [dbo].[Music]
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Music_IsActive]    Script Date: 10/26/2012 11:26:48 AM ******/
CREATE NONCLUSTERED INDEX [IX_Music_IsActive] ON [dbo].[Music]
(
	[IsActive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Music_MusicName]    Script Date: 10/26/2012 11:26:57 AM ******/
CREATE NONCLUSTERED INDEX [IX_Music_MusicName] ON [dbo].[Music]
(
	[MusicName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Music_Tags]    Script Date: 10/26/2012 11:27:06 AM ******/
CREATE NONCLUSTERED INDEX [IX_Music_Tags] ON [dbo].[Music]
(
	[Tags] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


-------------------------------------------------------------------------------------
-- System Messages Scripts
-------------------------------------------------------------------------------------

USE [Vodigi]
GO

/****** Object:  Table [dbo].[SystemMessage]    Script Date: 9/27/2012 8:30:53 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SystemMessage](
	[SystemMessageID] [int] IDENTITY(1000000,1) NOT NULL,
	[SystemMessageTitle] [nvarchar](64) NOT NULL,
	[SystemMessageBody] [nvarchar](256) NOT NULL,
	[DisplayDateStart] [datetime] NOT NULL,
	[DisplayDateEnd] [datetime] NOT NULL,
	[Priority] [int] NOT NULL,
 CONSTRAINT [PK_SystemMessage] PRIMARY KEY CLUSTERED 
(
	[SystemMessageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SystemMessage] ADD  CONSTRAINT [DF_SystemMessage_SystemMessageTitle]  DEFAULT ('') FOR [SystemMessageTitle]
GO

ALTER TABLE [dbo].[SystemMessage] ADD  CONSTRAINT [DF_SystemMessage_SystemMessage]  DEFAULT ('') FOR [SystemMessageBody]
GO

ALTER TABLE [dbo].[SystemMessage] ADD  CONSTRAINT [DF_SystemMessage_DisplayDateStart]  DEFAULT (getutcdate()) FOR [DisplayDateStart]
GO

ALTER TABLE [dbo].[SystemMessage] ADD  CONSTRAINT [DF_SystemMessage_DisplayDateEnd]  DEFAULT (getutcdate()) FOR [DisplayDateEnd]
GO




USE [Vodigi]
GO

/****** Object:  Index [IX_Account_IsActive]    Script Date: 07/26/2012 07:49:28 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Account]') AND name = N'IX_Account_IsActive')
DROP INDEX [IX_Account_IsActive] ON [dbo].[Account] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Account_IsActive]    Script Date: 07/26/2012 07:49:28 ******/
CREATE NONCLUSTERED INDEX [IX_Account_IsActive] ON [dbo].[Account] 
(
	[IsActive] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

----------------------------------------------------------------------------------------------
-- Survey Scripts
----------------------------------------------------------------------------------------------
USE [Vodigi]
GO

/****** Object:  Table [dbo].[AnsweredSurvey]    Script Date: 07/26/2012 07:55:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AnsweredSurvey](
	[AnsweredSurveyID] [int] IDENTITY(1000000,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[SurveyID] [int] NOT NULL,
	[PlayerID] [int] NOT NULL,
	[CreatedDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_AnsweredSurvey] PRIMARY KEY CLUSTERED 
(
	[AnsweredSurveyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[AnsweredSurvey] ADD  CONSTRAINT [DF_AnsweredSurvey_CreatedDateTime]  DEFAULT (getutcdate()) FOR [CreatedDateTime]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_AnsweredSurvey_AccountID]    Script Date: 07/26/2012 07:55:55 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AnsweredSurvey]') AND name = N'IX_AnsweredSurvey_AccountID')
DROP INDEX [IX_AnsweredSurvey_AccountID] ON [dbo].[AnsweredSurvey] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_AnsweredSurvey_AccountID]    Script Date: 07/26/2012 07:55:55 ******/
CREATE NONCLUSTERED INDEX [IX_AnsweredSurvey_AccountID] ON [dbo].[AnsweredSurvey] 
(
	[AccountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_AnsweredSurvey_SurveyID]    Script Date: 07/26/2012 07:56:01 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AnsweredSurvey]') AND name = N'IX_AnsweredSurvey_SurveyID')
DROP INDEX [IX_AnsweredSurvey_SurveyID] ON [dbo].[AnsweredSurvey] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_AnsweredSurvey_SurveyID]    Script Date: 07/26/2012 07:56:01 ******/
CREATE NONCLUSTERED INDEX [IX_AnsweredSurvey_SurveyID] ON [dbo].[AnsweredSurvey] 
(
	[SurveyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

----------------------------------------------------------------------------------------------

USE [Vodigi]
GO

/****** Object:  Table [dbo].[AnsweredSurveyQuestionOption]    Script Date: 07/26/2012 07:56:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AnsweredSurveyQuestionOption](
	[AnsweredSurveyQuestionOptionID] [int] IDENTITY(1000000,1) NOT NULL,
	[AnsweredSurveyID] [int] NOT NULL,
	[SurveyQuestionOptionID] [int] NOT NULL,
	[IsSelected] [bit] NOT NULL,
 CONSTRAINT [PK_AnsweredSurveyQuestionOption] PRIMARY KEY CLUSTERED 
(
	[AnsweredSurveyQuestionOptionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

USE [Vodigi]
GO

/****** Object:  Index [IX_AnsweredSurveyQuestionOption_AnsweredSurveyID]    Script Date: 07/26/2012 07:56:16 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AnsweredSurveyQuestionOption]') AND name = N'IX_AnsweredSurveyQuestionOption_AnsweredSurveyID')
DROP INDEX [IX_AnsweredSurveyQuestionOption_AnsweredSurveyID] ON [dbo].[AnsweredSurveyQuestionOption] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_AnsweredSurveyQuestionOption_AnsweredSurveyID]    Script Date: 07/26/2012 07:56:16 ******/
CREATE NONCLUSTERED INDEX [IX_AnsweredSurveyQuestionOption_AnsweredSurveyID] ON [dbo].[AnsweredSurveyQuestionOption] 
(
	[AnsweredSurveyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
USE [Vodigi]
GO

/****** Object:  Index [IX_AnsweredSurveyQuestionOption_SurveyQuestionOptionID]    Script Date: 07/26/2012 07:56:21 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AnsweredSurveyQuestionOption]') AND name = N'IX_AnsweredSurveyQuestionOption_SurveyQuestionOptionID')
DROP INDEX [IX_AnsweredSurveyQuestionOption_SurveyQuestionOptionID] ON [dbo].[AnsweredSurveyQuestionOption] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_AnsweredSurveyQuestionOption_SurveyQuestionOptionID]    Script Date: 07/26/2012 07:56:21 ******/
CREATE NONCLUSTERED INDEX [IX_AnsweredSurveyQuestionOption_SurveyQuestionOptionID] ON [dbo].[AnsweredSurveyQuestionOption] 
(
	[SurveyQuestionOptionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

----------------------------------------------------------------------------------------------

USE [Vodigi]
GO

/****** Object:  Index [IX_Image_AccountID]    Script Date: 07/26/2012 07:50:29 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Image]') AND name = N'IX_Image_AccountID')
DROP INDEX [IX_Image_AccountID] ON [dbo].[Image] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Image_AccountID]    Script Date: 07/26/2012 07:50:29 ******/
CREATE NONCLUSTERED INDEX [IX_Image_AccountID] ON [dbo].[Image] 
(
	[AccountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Image_ImageName]    Script Date: 07/26/2012 07:50:34 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Image]') AND name = N'IX_Image_ImageName')
DROP INDEX [IX_Image_ImageName] ON [dbo].[Image] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Image_ImageName]    Script Date: 07/26/2012 07:50:34 ******/
CREATE NONCLUSTERED INDEX [IX_Image_ImageName] ON [dbo].[Image] 
(
	[ImageName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Image_IsActive]    Script Date: 07/26/2012 07:50:40 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Image]') AND name = N'IX_Image_IsActive')
DROP INDEX [IX_Image_IsActive] ON [dbo].[Image] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Image_IsActive]    Script Date: 07/26/2012 07:50:40 ******/
CREATE NONCLUSTERED INDEX [IX_Image_IsActive] ON [dbo].[Image] 
(
	[IsActive] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Image_Tags]    Script Date: 07/26/2012 07:50:47 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Image]') AND name = N'IX_Image_Tags')
DROP INDEX [IX_Image_Tags] ON [dbo].[Image] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Image_Tags]    Script Date: 07/26/2012 07:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Image_Tags] ON [dbo].[Image] 
(
	[Tags] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

----------------------------------------------------------------------------------------------

USE [Vodigi]
GO

/****** Object:  Index [IX_Player_AccountID]    Script Date: 07/26/2012 07:51:01 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Player]') AND name = N'IX_Player_AccountID')
DROP INDEX [IX_Player_AccountID] ON [dbo].[Player] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Player_AccountID]    Script Date: 07/26/2012 07:51:01 ******/
CREATE NONCLUSTERED INDEX [IX_Player_AccountID] ON [dbo].[Player] 
(
	[AccountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Player_IsActive]    Script Date: 07/26/2012 07:51:07 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Player]') AND name = N'IX_Player_IsActive')
DROP INDEX [IX_Player_IsActive] ON [dbo].[Player] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Player_IsActive]    Script Date: 07/26/2012 07:51:07 ******/
CREATE NONCLUSTERED INDEX [IX_Player_IsActive] ON [dbo].[Player] 
(
	[IsActive] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Player_PlayerGroupID]    Script Date: 07/26/2012 07:51:13 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Player]') AND name = N'IX_Player_PlayerGroupID')
DROP INDEX [IX_Player_PlayerGroupID] ON [dbo].[Player] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Player_PlayerGroupID]    Script Date: 07/26/2012 07:51:13 ******/
CREATE NONCLUSTERED INDEX [IX_Player_PlayerGroupID] ON [dbo].[Player] 
(
	[PlayerGroupID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Player_PlayerName]    Script Date: 07/26/2012 07:51:19 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Player]') AND name = N'IX_Player_PlayerName')
DROP INDEX [IX_Player_PlayerName] ON [dbo].[Player] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Player_PlayerName]    Script Date: 07/26/2012 07:51:19 ******/
CREATE NONCLUSTERED INDEX [IX_Player_PlayerName] ON [dbo].[Player] 
(
	[PlayerName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

----------------------------------------------------------------------------------------------

USE [Vodigi]
GO

/****** Object:  Index [IX_PlayerGroup_AccountID]    Script Date: 07/26/2012 07:51:27 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PlayerGroup]') AND name = N'IX_PlayerGroup_AccountID')
DROP INDEX [IX_PlayerGroup_AccountID] ON [dbo].[PlayerGroup] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_PlayerGroup_AccountID]    Script Date: 07/26/2012 07:51:27 ******/
CREATE NONCLUSTERED INDEX [IX_PlayerGroup_AccountID] ON [dbo].[PlayerGroup] 
(
	[AccountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_PlayerGroup_IsActive]    Script Date: 07/26/2012 07:51:32 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PlayerGroup]') AND name = N'IX_PlayerGroup_IsActive')
DROP INDEX [IX_PlayerGroup_IsActive] ON [dbo].[PlayerGroup] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_PlayerGroup_IsActive]    Script Date: 07/26/2012 07:51:32 ******/
CREATE NONCLUSTERED INDEX [IX_PlayerGroup_IsActive] ON [dbo].[PlayerGroup] 
(
	[IsActive] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
USE [Vodigi]
GO

/****** Object:  Index [IX_PlayerGroup_PlayerGroupName]    Script Date: 07/26/2012 07:51:36 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PlayerGroup]') AND name = N'IX_PlayerGroup_PlayerGroupName')
DROP INDEX [IX_PlayerGroup_PlayerGroupName] ON [dbo].[PlayerGroup] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_PlayerGroup_PlayerGroupName]    Script Date: 07/26/2012 07:51:36 ******/
CREATE NONCLUSTERED INDEX [IX_PlayerGroup_PlayerGroupName] ON [dbo].[PlayerGroup] 
(
	[PlayerGroupName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

----------------------------------------------------------------------------------------------

USE [Vodigi]
GO

/****** Object:  Index [IX_PlayerGroupSchedule_PlayerGroupID]    Script Date: 07/26/2012 07:51:45 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PlayerGroupSchedule]') AND name = N'IX_PlayerGroupSchedule_PlayerGroupID')
DROP INDEX [IX_PlayerGroupSchedule_PlayerGroupID] ON [dbo].[PlayerGroupSchedule] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_PlayerGroupSchedule_PlayerGroupID]    Script Date: 07/26/2012 07:51:45 ******/
CREATE NONCLUSTERED INDEX [IX_PlayerGroupSchedule_PlayerGroupID] ON [dbo].[PlayerGroupSchedule] 
(
	[PlayerGroupID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

----------------------------------------------------------------------------------------------

USE [Vodigi]
GO

/****** Object:  Index [IX_PlayList_AccountID]    Script Date: 07/26/2012 07:51:54 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PlayList]') AND name = N'IX_PlayList_AccountID')
DROP INDEX [IX_PlayList_AccountID] ON [dbo].[PlayList] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_PlayList_AccountID]    Script Date: 07/26/2012 07:51:54 ******/
CREATE NONCLUSTERED INDEX [IX_PlayList_AccountID] ON [dbo].[PlayList] 
(
	[AccountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_PlayList_IsActive]    Script Date: 07/26/2012 07:51:59 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PlayList]') AND name = N'IX_PlayList_IsActive')
DROP INDEX [IX_PlayList_IsActive] ON [dbo].[PlayList] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_PlayList_IsActive]    Script Date: 07/26/2012 07:51:59 ******/
CREATE NONCLUSTERED INDEX [IX_PlayList_IsActive] ON [dbo].[PlayList] 
(
	[IsActive] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_PlayList_PlayListName]    Script Date: 07/26/2012 07:52:04 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PlayList]') AND name = N'IX_PlayList_PlayListName')
DROP INDEX [IX_PlayList_PlayListName] ON [dbo].[PlayList] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_PlayList_PlayListName]    Script Date: 07/26/2012 07:52:04 ******/
CREATE NONCLUSTERED INDEX [IX_PlayList_PlayListName] ON [dbo].[PlayList] 
(
	[PlayListName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_PlayList_Tags]    Script Date: 07/26/2012 07:52:10 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PlayList]') AND name = N'IX_PlayList_Tags')
DROP INDEX [IX_PlayList_Tags] ON [dbo].[PlayList] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_PlayList_Tags]    Script Date: 07/26/2012 07:52:10 ******/
CREATE NONCLUSTERED INDEX [IX_PlayList_Tags] ON [dbo].[PlayList] 
(
	[Tags] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

----------------------------------------------------------------------------------------------

USE [Vodigi]
GO

/****** Object:  Index [IX_PlayListVideoXref_PlayListID]    Script Date: 07/26/2012 07:52:18 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PlayListVideoXref]') AND name = N'IX_PlayListVideoXref_PlayListID')
DROP INDEX [IX_PlayListVideoXref_PlayListID] ON [dbo].[PlayListVideoXref] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_PlayListVideoXref_PlayListID]    Script Date: 07/26/2012 07:52:18 ******/
CREATE NONCLUSTERED INDEX [IX_PlayListVideoXref_PlayListID] ON [dbo].[PlayListVideoXref] 
(
	[PlayListID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO


----------------------------------------------------------------------------------------------

USE [Vodigi]
GO

/****** Object:  Index [IX_Screen_AccountID]    Script Date: 07/26/2012 07:52:36 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Screen]') AND name = N'IX_Screen_AccountID')
DROP INDEX [IX_Screen_AccountID] ON [dbo].[Screen] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Screen_AccountID]    Script Date: 07/26/2012 07:52:36 ******/
CREATE NONCLUSTERED INDEX [IX_Screen_AccountID] ON [dbo].[Screen] 
(
	[AccountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Screen_IsActive]    Script Date: 07/26/2012 07:52:41 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Screen]') AND name = N'IX_Screen_IsActive')
DROP INDEX [IX_Screen_IsActive] ON [dbo].[Screen] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Screen_IsActive]    Script Date: 07/26/2012 07:52:41 ******/
CREATE NONCLUSTERED INDEX [IX_Screen_IsActive] ON [dbo].[Screen] 
(
	[IsActive] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Screen_ScreenName]    Script Date: 07/26/2012 07:52:46 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Screen]') AND name = N'IX_Screen_ScreenName')
DROP INDEX [IX_Screen_ScreenName] ON [dbo].[Screen] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Screen_ScreenName]    Script Date: 07/26/2012 07:52:46 ******/
CREATE NONCLUSTERED INDEX [IX_Screen_ScreenName] ON [dbo].[Screen] 
(
	[ScreenName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

----------------------------------------------------------------------------------------------

USE [Vodigi]
GO

/****** Object:  Index [IX_ScreenContent_AccountID]    Script Date: 07/26/2012 07:52:55 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScreenContent]') AND name = N'IX_ScreenContent_AccountID')
DROP INDEX [IX_ScreenContent_AccountID] ON [dbo].[ScreenContent] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_ScreenContent_AccountID]    Script Date: 07/26/2012 07:52:55 ******/
CREATE NONCLUSTERED INDEX [IX_ScreenContent_AccountID] ON [dbo].[ScreenContent] 
(
	[AccountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_ScreenContent_IsActive]    Script Date: 07/26/2012 07:52:59 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScreenContent]') AND name = N'IX_ScreenContent_IsActive')
DROP INDEX [IX_ScreenContent_IsActive] ON [dbo].[ScreenContent] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_ScreenContent_IsActive]    Script Date: 07/26/2012 07:52:59 ******/
CREATE NONCLUSTERED INDEX [IX_ScreenContent_IsActive] ON [dbo].[ScreenContent] 
(
	[IsActive] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


USE [Vodigi]
GO

/****** Object:  Index [IX_ScreenContent_ScreenContentName]    Script Date: 07/26/2012 07:53:04 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScreenContent]') AND name = N'IX_ScreenContent_ScreenContentName')
DROP INDEX [IX_ScreenContent_ScreenContentName] ON [dbo].[ScreenContent] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_ScreenContent_ScreenContentName]    Script Date: 07/26/2012 07:53:04 ******/
CREATE NONCLUSTERED INDEX [IX_ScreenContent_ScreenContentName] ON [dbo].[ScreenContent] 
(
	[ScreenContentName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

----------------------------------------------------------------------------------------------

USE [Vodigi]
GO

/****** Object:  Index [IX_ScreenScreenContentXref_ScreenID]    Script Date: 07/26/2012 07:53:21 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScreenScreenContentXref]') AND name = N'IX_ScreenScreenContentXref_ScreenID')
DROP INDEX [IX_ScreenScreenContentXref_ScreenID] ON [dbo].[ScreenScreenContentXref] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_ScreenScreenContentXref_ScreenID]    Script Date: 07/26/2012 07:53:21 ******/
CREATE NONCLUSTERED INDEX [IX_ScreenScreenContentXref_ScreenID] ON [dbo].[ScreenScreenContentXref] 
(
	[ScreenID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

----------------------------------------------------------------------------------------------

USE [Vodigi]
GO

/****** Object:  Index [IX_SlideShow_AccountID]    Script Date: 07/26/2012 07:53:29 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SlideShow]') AND name = N'IX_SlideShow_AccountID')
DROP INDEX [IX_SlideShow_AccountID] ON [dbo].[SlideShow] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_SlideShow_AccountID]    Script Date: 07/26/2012 07:53:29 ******/
CREATE NONCLUSTERED INDEX [IX_SlideShow_AccountID] ON [dbo].[SlideShow] 
(
	[AccountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_SlideShow_IsActive]    Script Date: 07/26/2012 07:53:34 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SlideShow]') AND name = N'IX_SlideShow_IsActive')
DROP INDEX [IX_SlideShow_IsActive] ON [dbo].[SlideShow] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_SlideShow_IsActive]    Script Date: 07/26/2012 07:53:34 ******/
CREATE NONCLUSTERED INDEX [IX_SlideShow_IsActive] ON [dbo].[SlideShow] 
(
	[IsActive] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_SlideShow_SlideShowName]    Script Date: 07/26/2012 07:53:39 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SlideShow]') AND name = N'IX_SlideShow_SlideShowName')
DROP INDEX [IX_SlideShow_SlideShowName] ON [dbo].[SlideShow] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_SlideShow_SlideShowName]    Script Date: 07/26/2012 07:53:39 ******/
CREATE NONCLUSTERED INDEX [IX_SlideShow_SlideShowName] ON [dbo].[SlideShow] 
(
	[SlideShowName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_SlideShow_Tags]    Script Date: 07/26/2012 07:53:44 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SlideShow]') AND name = N'IX_SlideShow_Tags')
DROP INDEX [IX_SlideShow_Tags] ON [dbo].[SlideShow] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_SlideShow_Tags]    Script Date: 07/26/2012 07:53:44 ******/
CREATE NONCLUSTERED INDEX [IX_SlideShow_Tags] ON [dbo].[SlideShow] 
(
	[Tags] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

----------------------------------------------------------------------------------------------

USE [Vodigi]
GO

/****** Object:  Index [IX_SlideShowImageXref_SlideShowID]    Script Date: 07/26/2012 07:53:53 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SlideShowImageXref]') AND name = N'IX_SlideShowImageXref_SlideShowID')
DROP INDEX [IX_SlideShowImageXref_SlideShowID] ON [dbo].[SlideShowImageXref] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_SlideShowImageXref_SlideShowID]    Script Date: 07/26/2012 07:53:53 ******/
CREATE NONCLUSTERED INDEX [IX_SlideShowImageXref_SlideShowID] ON [dbo].[SlideShowImageXref] 
(
	[SlideShowID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

----------------------------------------------------------------------------------------------

USE [Vodigi]
GO

/****** Object:  Table [dbo].[Survey]    Script Date: 07/26/2012 07:56:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Survey](
	[SurveyID] [int] IDENTITY(1000000,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[SurveyName] [nvarchar](128) NOT NULL,
	[SurveyDescription] [nvarchar](1024) NOT NULL,
	[SurveyImageID] [int] NOT NULL,
	[IsApproved] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Survey] PRIMARY KEY CLUSTERED 
(
	[SurveyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Survey] ADD  CONSTRAINT [DF_Survey_SurveyName]  DEFAULT ('') FOR [SurveyName]
GO

ALTER TABLE [dbo].[Survey] ADD  CONSTRAINT [DF_Survey_SurveyDescription]  DEFAULT ('') FOR [SurveyDescription]
GO

ALTER TABLE [dbo].[Survey] ADD  CONSTRAINT [DF_Survey_SurveyImageID]  DEFAULT ((0)) FOR [SurveyImageID]
GO

ALTER TABLE [dbo].[Survey] ADD  CONSTRAINT [DF_Survey_Approved]  DEFAULT ((1)) FOR [IsApproved]
GO

ALTER TABLE [dbo].[Survey] ADD  CONSTRAINT [DF_Survey_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO


USE [Vodigi]
GO

/****** Object:  Index [IX_Survey_AccountID]    Script Date: 07/26/2012 07:56:41 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Survey]') AND name = N'IX_Survey_AccountID')
DROP INDEX [IX_Survey_AccountID] ON [dbo].[Survey] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Survey_AccountID]    Script Date: 07/26/2012 07:56:41 ******/
CREATE NONCLUSTERED INDEX [IX_Survey_AccountID] ON [dbo].[Survey] 
(
	[AccountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Survey_IsActive]    Script Date: 07/26/2012 07:56:46 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Survey]') AND name = N'IX_Survey_IsActive')
DROP INDEX [IX_Survey_IsActive] ON [dbo].[Survey] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Survey_IsActive]    Script Date: 07/26/2012 07:56:46 ******/
CREATE NONCLUSTERED INDEX [IX_Survey_IsActive] ON [dbo].[Survey] 
(
	[IsActive] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Survey_IsApproved]    Script Date: 07/26/2012 07:56:50 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Survey]') AND name = N'IX_Survey_IsApproved')
DROP INDEX [IX_Survey_IsApproved] ON [dbo].[Survey] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Survey_IsApproved]    Script Date: 07/26/2012 07:56:50 ******/
CREATE NONCLUSTERED INDEX [IX_Survey_IsApproved] ON [dbo].[Survey] 
(
	[IsApproved] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


USE [Vodigi]
GO

/****** Object:  Index [IX_Survey_SurveyName]    Script Date: 07/26/2012 07:56:55 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Survey]') AND name = N'IX_Survey_SurveyName')
DROP INDEX [IX_Survey_SurveyName] ON [dbo].[Survey] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Survey_SurveyName]    Script Date: 07/26/2012 07:56:55 ******/
CREATE NONCLUSTERED INDEX [IX_Survey_SurveyName] ON [dbo].[Survey] 
(
	[SurveyName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

----------------------------------------------------------------------------------------------

USE [Vodigi]
GO

/****** Object:  Table [dbo].[SurveyQuestion]    Script Date: 07/26/2012 07:57:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SurveyQuestion](
	[SurveyQuestionID] [int] IDENTITY(1000000,1) NOT NULL,
	[SurveyID] [int] NOT NULL,
	[SurveyQuestionText] [nvarchar](1024) NOT NULL,
	[AllowMultiselect] [bit] NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_SurveyQuestion] PRIMARY KEY CLUSTERED 
(
	[SurveyQuestionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SurveyQuestion] ADD  CONSTRAINT [DF_SurveyQuestion_SurveyQuestionText]  DEFAULT ('') FOR [SurveyQuestionText]
GO

ALTER TABLE [dbo].[SurveyQuestion] ADD  CONSTRAINT [DF_SurveyQuestion_AllowMultiselect]  DEFAULT ((0)) FOR [AllowMultiselect]
GO

ALTER TABLE [dbo].[SurveyQuestion] ADD  CONSTRAINT [DF_SurveyQuestion_SortOrder]  DEFAULT ((1)) FOR [SortOrder]
GO


USE [Vodigi]
GO

/****** Object:  Index [IX_SurveyQuestion_SurveyID]    Script Date: 07/26/2012 07:57:11 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SurveyQuestion]') AND name = N'IX_SurveyQuestion_SurveyID')
DROP INDEX [IX_SurveyQuestion_SurveyID] ON [dbo].[SurveyQuestion] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_SurveyQuestion_SurveyID]    Script Date: 07/26/2012 07:57:11 ******/
CREATE NONCLUSTERED INDEX [IX_SurveyQuestion_SurveyID] ON [dbo].[SurveyQuestion] 
(
	[SurveyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

----------------------------------------------------------------------------------------------

USE [Vodigi]
GO

/****** Object:  Table [dbo].[SurveyQuestionOption]    Script Date: 07/26/2012 07:57:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SurveyQuestionOption](
	[SurveyQuestionOptionID] [int] IDENTITY(1000000,1) NOT NULL,
	[SurveyQuestionID] [int] NOT NULL,
	[SurveyQuestionOptionText] [nvarchar](1024) NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_SurveyQuestionOption] PRIMARY KEY CLUSTERED 
(
	[SurveyQuestionOptionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SurveyQuestionOption] ADD  CONSTRAINT [DF_SurveyQuestionOption_SurveyQuestionOptionText]  DEFAULT ('') FOR [SurveyQuestionOptionText]
GO

ALTER TABLE [dbo].[SurveyQuestionOption] ADD  CONSTRAINT [DF_SurveyQuestionOption_SortOrder]  DEFAULT ((1)) FOR [SortOrder]
GO


USE [Vodigi]
GO

/****** Object:  Index [IX_SurveyQuestionOption_SurveyQuestionID]    Script Date: 07/26/2012 08:28:24 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SurveyQuestionOption]') AND name = N'IX_SurveyQuestionOption_SurveyQuestionID')
DROP INDEX [IX_SurveyQuestionOption_SurveyQuestionID] ON [dbo].[SurveyQuestionOption] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_SurveyQuestionOption_SurveyQuestionID]    Script Date: 07/26/2012 08:28:24 ******/
CREATE NONCLUSTERED INDEX [IX_SurveyQuestionOption_SurveyQuestionID] ON [dbo].[SurveyQuestionOption] 
(
	[SurveyQuestionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

----------------------------------------------------------------------------------------------

USE [Vodigi]
GO

/****** Object:  Index [IX_User_AccountID]    Script Date: 07/26/2012 07:54:07 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[User]') AND name = N'IX_User_AccountID')
DROP INDEX [IX_User_AccountID] ON [dbo].[User] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_User_AccountID]    Script Date: 07/26/2012 07:54:07 ******/
CREATE NONCLUSTERED INDEX [IX_User_AccountID] ON [dbo].[User] 
(
	[AccountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_User_IsActive]    Script Date: 07/26/2012 07:54:12 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[User]') AND name = N'IX_User_IsActive')
DROP INDEX [IX_User_IsActive] ON [dbo].[User] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_User_IsActive]    Script Date: 07/26/2012 07:54:12 ******/
CREATE NONCLUSTERED INDEX [IX_User_IsActive] ON [dbo].[User] 
(
	[IsActive] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_User_Username]    Script Date: 07/26/2012 07:54:16 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[User]') AND name = N'IX_User_Username')
DROP INDEX [IX_User_Username] ON [dbo].[User] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_User_Username]    Script Date: 07/26/2012 07:54:16 ******/
CREATE NONCLUSTERED INDEX [IX_User_Username] ON [dbo].[User] 
(
	[Username] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_User_UsernamePassword]    Script Date: 07/26/2012 07:54:20 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[User]') AND name = N'IX_User_UsernamePassword')
DROP INDEX [IX_User_UsernamePassword] ON [dbo].[User] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_User_UsernamePassword]    Script Date: 07/26/2012 07:54:20 ******/
CREATE NONCLUSTERED INDEX [IX_User_UsernamePassword] ON [dbo].[User] 
(
	[Username] ASC,
	[Password] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

----------------------------------------------------------------------------------------------

USE [Vodigi]
GO

/****** Object:  Index [IX_Video_AccountID]    Script Date: 07/26/2012 07:54:28 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Video]') AND name = N'IX_Video_AccountID')
DROP INDEX [IX_Video_AccountID] ON [dbo].[Video] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Video_AccountID]    Script Date: 07/26/2012 07:54:28 ******/
CREATE NONCLUSTERED INDEX [IX_Video_AccountID] ON [dbo].[Video] 
(
	[AccountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Video_IsActive]    Script Date: 07/26/2012 07:54:32 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Video]') AND name = N'IX_Video_IsActive')
DROP INDEX [IX_Video_IsActive] ON [dbo].[Video] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Video_IsActive]    Script Date: 07/26/2012 07:54:32 ******/
CREATE NONCLUSTERED INDEX [IX_Video_IsActive] ON [dbo].[Video] 
(
	[IsActive] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
USE [Vodigi]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Video_Tags]    Script Date: 07/26/2012 08:30:29 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Video]') AND name = N'IX_Video_Tags')
DROP INDEX [IX_Video_Tags] ON [dbo].[Video] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Video_Tags]    Script Date: 07/26/2012 08:30:29 ******/
CREATE NONCLUSTERED INDEX [IX_Video_Tags] ON [dbo].[Video] 
(
	[Tags] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Video_VideoName]    Script Date: 07/26/2012 08:30:54 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Video]') AND name = N'IX_Video_VideoName')
DROP INDEX [IX_Video_VideoName] ON [dbo].[Video] WITH ( ONLINE = OFF )
GO

USE [Vodigi]
GO

/****** Object:  Index [IX_Video_VideoName]    Script Date: 07/26/2012 08:30:54 ******/
CREATE NONCLUSTERED INDEX [IX_Video_VideoName] ON [dbo].[Video] 
(
	[VideoName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

----------------------------------------------------------------------------------------------

DECLARE @RowCount INT
SET @RowCount = (SELECT COUNT(*) FROM ScreenContentType WHERE ScreenContentTypeID = 1000007)

IF @RowCount = 0
BEGIN

	SET IDENTITY_INSERT ScreenContentType ON
	
	INSERT INTO ScreenContentType 
		(ScreenContentTypeID, ScreenContentTypeName)
	VALUES
		(1000007, 'Survey')
	
	SET IDENTITY_INSERT ScreenContentType OFF

END


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



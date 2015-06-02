
-- Data Population Scripts
-- Vodigi Database - Should only be used on new databases created from scripts

-- Add default account
INSERT INTO [dbo].[Account]
	(
	AccountName,
	AccountDescription,
	FTPServer,
	FTPUsername,
	FTPPassword,
	IsActive
	)
VALUES
	(
	'Default Account',
	'Default Account',
	'ftp://111.111.111.11',
	'username',
	'password',
	1
	)

-- Add default User
INSERT INTO [dbo].[User]
	(
	AccountID,
	Username,
	[Password],
	FirstName,
	LastName,
	EmailAddress,
	IsAdmin,
	IsActive
	)
VALUES
	(
	1000000,
	'defaultuser',
	'defaultpassword',
	'Default',
	'User',
	'default@user.com',
	1,
	1
	)
	
-- Add Screen Content Types
INSERT INTO [dbo].[ScreenContentType] (ScreenContentTypeName) VALUES ('Image')
INSERT INTO [dbo].[ScreenContentType] (ScreenContentTypeName) VALUES ('Slide Show')
INSERT INTO [dbo].[ScreenContentType] (ScreenContentTypeName) VALUES ('Video')
INSERT INTO [dbo].[ScreenContentType] (ScreenContentTypeName) VALUES ('Play List')
INSERT INTO [dbo].[ScreenContentType] (ScreenContentTypeName) VALUES ('Web Site')
INSERT INTO [dbo].[ScreenContentType] (ScreenContentTypeName) VALUES ('Webcam')
INSERT INTO [dbo].[ScreenContentType] (ScreenContentTypeName) VALUES ('Weather')
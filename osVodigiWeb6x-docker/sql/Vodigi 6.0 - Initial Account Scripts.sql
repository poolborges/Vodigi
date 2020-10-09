-- ****************
-- Script Initial Account and User
-- ****************
USE [Vodigi]

-- Insert default Account 
INSERT INTO [dbo].[Account](AccountName,AccountDescription,FTPServer,FTPUsername,FTPPassword,IsActive) VALUES('Default Account','Default Account','ftp://111.111.111.11','username','password',1);
GO

-- Insert Default User(admin)
INSERT INTO [dbo].[User](AccountID,Username,[Password],FirstName,LastName,EmailAddress,IsAdmin,IsActive) VALUES(1000000,'admin','admin','Administrator','User','admin@example.com',1,1);
GO
CREATE LOGIN retrogamer WITH PASSWORD='abc123!@'

USE Leaderboard
GO

CREATE USER retrogamer
	FOR LOGIN retrogamer
	WITH DEFAULT_SCHEMA = dbo
GO
-- Add user to the database owner role
EXEC sp_addrolemember N'db_owner', N'retrogamer'
GO
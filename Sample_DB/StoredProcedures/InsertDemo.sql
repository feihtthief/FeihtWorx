USE [DataWorkerTest]
GO
/****** Object:  StoredProcedure [dbo].[InsertDemo]    Script Date: 03/02/2017 00:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[InsertDemo]
(
	 @ID              int            output
	,@Name            nvarchar(255)
	,@Group           nvarchar(255)
	,@Description     nvarchar(max)
	,@ReleaseDate     date
)
AS 
INSERT INTO [dbo].[Demos] (
	 [Name]
	,[Group]
	,[Description]
	,[ReleaseDate]
) VALUES (
	 @Name
	,@Group
	,@Description
	,@ReleaseDate
)
set @ID = Scope_Identity()
GO

USE [DataWorkerTest]
GO
/****** Object:  StoredProcedure [dbo].[UpdateDemo]    Script Date: 03/02/2017 00:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpdateDemo]
(
	 @ID              int
	,@Name            nvarchar(255)
	,@Group           nvarchar(255)
	,@Description     nvarchar(max)
	,@ReleaseDate     date
)
AS 
Update [dbo].[Demos] 
set	 [Name]          = @Name
	,[Group]         = @Group
	,[Description]   = @Description
	,[ReleaseDate]   = @ReleaseDate
WHERE
	[ID] = @ID
GO

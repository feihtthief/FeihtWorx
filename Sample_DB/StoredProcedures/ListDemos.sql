USE [DataWorkerTest]
GO
/****** Object:  StoredProcedure [dbo].[ListDemos]    Script Date: 03/02/2017 00:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ListDemos]
AS 
SELECT	
	 [ID]
	,[Name]
	,[Group]
	,[Description]
	,[ReleaseDate]
	,[CreatedDateTime]
	,[CreatedFromHost]
FROM 
	Demos
GO

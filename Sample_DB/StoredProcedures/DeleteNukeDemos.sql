USE [DataWorkerTest]
GO
/****** Object:  StoredProcedure [dbo].[DeleteNukeDemos]    Script Date: 03/02/2017 00:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DeleteNukeDemos]
AS 
DELETE FROM 
	Demos
WHERE [Group] LIKE '\[NUKE GROUP\]%' ESCAPE '\'
GO

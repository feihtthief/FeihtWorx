USE [DataWorkerTest]
GO
/****** Object:  StoredProcedure [dbo].[DeleteDemosByGroupName]    Script Date: 03/02/2017 00:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DeleteDemosByGroupName]
(
	 @Group           NVARCHAR(255)
)
AS 
DELETE FROM 
	Demos
WHERE [Group] = @Group
GO

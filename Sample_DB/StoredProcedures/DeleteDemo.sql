USE [DataWorkerTest]
GO
/****** Object:  StoredProcedure [dbo].[DeleteDemo]    Script Date: 03/02/2017 00:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DeleteDemo]
(
	 @ID      int
)
AS 
DELETE FROM 
	Demos
WHERE ID = @ID
GO

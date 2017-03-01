USE [DataWorkerTest]
GO
/****** Object:  StoredProcedure [dbo].[AffectNoRows]    Script Date: 03/02/2017 00:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		thief
-- Create date: 2016-Nov-14
-- Description:	Test that no rows are affected
-- =============================================
CREATE PROCEDURE [dbo].[AffectNoRows]
AS
BEGIN
	set nocount off
	update [dbo].[Samples] set ChangeCount = ChangeCount + 1 where ID = -666
END
GO

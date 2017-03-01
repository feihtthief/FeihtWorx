USE [DataWorkerTest]
GO
/****** Object:  StoredProcedure [dbo].[TestMissingAndNullInput]    Script Date: 03/02/2017 00:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		thief
-- Create date: 2012-Oct-01
-- Description:	Test Absent/Null Input
-- =============================================
CREATE PROCEDURE [dbo].[TestMissingAndNullInput]
(
	 @RequiredParam  int
	,@OptionalParam1 int = null
	,@OptionalParam2 int = 41
)
AS
BEGIN
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 
	 @RequiredParam  as RequiredParamOut
	,@OptionalParam1 as OptionalParam1Out
	,@OptionalParam2 as OptionalParam2Out
END
GO

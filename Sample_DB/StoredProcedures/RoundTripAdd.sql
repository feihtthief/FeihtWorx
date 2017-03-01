USE [DataWorkerTest]
GO
/****** Object:  StoredProcedure [dbo].[RoundTripAdd]    Script Date: 03/02/2017 00:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		thief
-- Create date: 2016-Nov-14
-- Description:	Test output on IO object
-- =============================================
CREATE PROCEDURE [dbo].[RoundTripAdd]
(
	 @ValueA	   int
	,@ValueB	   int
	,@ValueC	   int output
)
AS
BEGIN
	set @ValueC = @ValueA + @ValueB
END
GO

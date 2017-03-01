USE [DataWorkerTest]
GO
/****** Object:  StoredProcedure [dbo].[DeleteSample]    Script Date: 03/02/2017 00:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[DeleteSample]
(
	 @ID	bigint    output
)
AS
BEGIN
	Delete from Samples where ID = @id;
END
GO

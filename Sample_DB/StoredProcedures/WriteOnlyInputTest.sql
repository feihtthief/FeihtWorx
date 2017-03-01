USE [DataWorkerTest]
GO
/****** Object:  StoredProcedure [dbo].[WriteOnlyInputTest]    Script Date: 03/02/2017 00:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[WriteOnlyInputTest]
(
	 @WriteOnlyProperty nvarchar(255) = 'Default Value Set In Stored Proc'
)
AS
BEGIN
	select *,@WriteOnlyProperty as WriteOnlyPropertyOutput from samples;
END
GO

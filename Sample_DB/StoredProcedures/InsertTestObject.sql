USE [DataWorkerTest]
GO
/****** Object:  StoredProcedure [dbo].[InsertTestObject]    Script Date: 03/02/2017 00:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertTestObject]
(
	@ID	bigint    output
)
AS
BEGIN
	insert into TestObjects default values;
	set @ID = SCOPE_IDENTITY();

END
GO

USE [DataWorkerTest]
GO
/****** Object:  StoredProcedure [dbo].[ListNotSimple]    Script Date: 03/02/2017 00:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [dbo].[ListNotSimple]
(
	 @ReallyNotSimpleColumnNameThatNoOneCanEverRememberIn nvarchar(255)
)
AS
BEGIN
	select *,@ReallyNotSimpleColumnNameThatNoOneCanEverRememberIn as ReallyNotSimpleColumnNameThatNoOneCanEverRememberOut from samples;
END
GO

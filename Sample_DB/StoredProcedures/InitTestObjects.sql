USE [DataWorkerTest]
GO
/****** Object:  StoredProcedure [dbo].[InitTestObjects]    Script Date: 03/02/2017 00:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InitTestObjects]
AS
BEGIN
	truncate table TestObjects ;
	insert into TestObjects default values;
	insert into TestObjects default values;
	insert into TestObjects default values;
	insert into TestObjects default values;
	insert into TestObjects default values;

END
GO

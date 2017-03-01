USE [DataWorkerTest]
GO
/****** Object:  StoredProcedure [dbo].[InitSamples]    Script Date: 03/02/2017 00:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InitSamples]
AS
BEGIN
	truncate table Samples ;
	set identity_insert Samples on;
	insert into Samples (ID,Name,changecount) values ( 1 ,'one'   ,0);
	insert into Samples (ID,Name,changecount) values ( 2 ,'two'   ,0);
	insert into Samples (ID,Name,changecount) values ( 3 ,'three' ,0);
	insert into Samples (ID,Name,changecount) values ( 4 ,'four'  ,0);
	insert into Samples (ID,Name,changecount) values ( 5 ,'five'  ,0);
	set identity_insert Samples off;
END
GO

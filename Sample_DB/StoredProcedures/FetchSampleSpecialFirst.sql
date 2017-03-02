USE [DataWorkerTest]
GO
/****** Object:  StoredProcedure [dbo].[FetchSampleSpecial]    Script Date: 03/02/2017 00:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [dbo].[FetchSampleSpecialFirst]
AS
BEGIN

	select
		top 1 
		 ID
		,Name
		,ChangeCount
		,'[FetchSampleSpecialFirst]'   as [SpecialExtra]
	from 
		samples
	order by id

END
GO

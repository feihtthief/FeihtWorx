USE [DataWorkerTest]
GO
/****** Object:  StoredProcedure [dbo].[FetchSampleSpecial]    Script Date: 03/02/2017 00:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [dbo].[FetchSampleSpecial]
(
	 @ID int 
)
AS
BEGIN
	select 
		 ID
		,Name
		,ChangeCount
		,'yes really' as [SpecialExtra]
	from 
		samples
	where 
		ID = @id;
END
GO

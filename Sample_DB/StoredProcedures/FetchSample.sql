USE [DataWorkerTest]
GO
/****** Object:  StoredProcedure [dbo].[FetchSample]    Script Date: 03/02/2017 00:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [dbo].[FetchSample]
(
	 @ID int 
)
AS
BEGIN
	select 
		 ID
		,Name
		,ChangeCount
	from 
		samples
	where 
		ID = @id;
END
GO

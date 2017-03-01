USE [DataWorkerTest]
GO
/****** Object:  StoredProcedure [dbo].[TestAffected]    Script Date: 03/02/2017 00:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[TestAffected]
(
	@howmany int = null
)
AS
BEGIN
	set @howmany = isnull(@howmany,1);
	update samples set  changecount = changecount +1 where [id] <= @howmany;
END
GO

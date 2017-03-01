USE [DataWorkerTest]
GO
/****** Object:  StoredProcedure [dbo].[UpdateSample]    Script Date: 03/02/2017 00:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [dbo].[UpdateSample]
(
	 @ID	bigint    output
	,@Name  nvarchar(50)
)
AS
BEGIN
	update Samples 
	set Name = @Name
	where ID = @ID
	;
END
GO

USE [DataWorkerTest]
GO
/****** Object:  StoredProcedure [dbo].[InsertSample]    Script Date: 03/02/2017 00:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertSample]
(
	 @ID	bigint    output
	,@Name  nvarchar(50)
)
AS
BEGIN
	insert into Samples (Name) values (@Name)
	set @ID = SCOPE_IDENTITY();

END
GO

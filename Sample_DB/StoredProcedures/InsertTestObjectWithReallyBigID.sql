USE [DataWorkerTest]
GO
/****** Object:  StoredProcedure [dbo].[InsertTestObjectWithReallyBigID]    Script Date: 03/02/2017 00:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		thief
-- Create date: 2012-Oct-01
-- Description:	Test Max Bigint ID Insertion
-- =============================================
CREATE PROCEDURE [dbo].[InsertTestObjectWithReallyBigID]
(
	@ID	bigint    output
)
AS
BEGIN
	set identity_insert [dbo].[TestObjects] on
	insert into [dbo].[TestObjects] (ID) values (cast(9223372036854775807 as bigint));
	set identity_insert [dbo].[TestObjects] off
	set @ID = SCOPE_IDENTITY();

END
GO

USE [DataWorkerTest]
GO
/****** Object:  StoredProcedure [dbo].[ListGarbage]    Script Date: 03/02/2017 00:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ListGarbage]
(
	 @FieldThatNoClassHas nvarchar(255) = 'Default Value Set In Stored Proc'
)
AS
BEGIN
	select *,@FieldThatNoClassHas as FeedOut from samples;
END
GO

USE [DataWorkerTest]
GO
/****** Object:  StoredProcedure [dbo].[ListSamples]    Script Date: 03/02/2017 00:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ListSamplesSpecial]
(
	 @feedout          nvarchar(255) = null output
	,@FeedToTestColumn nvarchar(255) = null 
)
AS
BEGIN
	set @feedout = isnull(@feedout,'')+'_ex';
	select 
		*
		,@FeedToTestColumn as TestColumn 
		,'[ListSamplesSpecial]' as SpecialExtra
	from samples;
END
GO

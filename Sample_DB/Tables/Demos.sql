USE [DataWorkerTest]
GO
/****** Object:  Table [dbo].[Demos]    Script Date: 03/02/2017 00:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Demos](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TimeStamp] [timestamp] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Group] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[ReleaseDate] [date] NOT NULL,
	[CreatedDateTime] [datetime2](7) NOT NULL,
	[CreatedFromHost] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_Demos] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UK_Demos_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Demos] ADD  CONSTRAINT [DF_DEMOS_ReleaseDate]  DEFAULT (getdate()) FOR [ReleaseDate]
GO
ALTER TABLE [dbo].[Demos] ADD  CONSTRAINT [DF_DEMOS_CreatedDateTime]  DEFAULT (getdate()) FOR [CreatedDateTime]
GO
ALTER TABLE [dbo].[Demos] ADD  CONSTRAINT [DF_DEMOS_CreatedFromHost]  DEFAULT (host_name()) FOR [CreatedFromHost]
GO

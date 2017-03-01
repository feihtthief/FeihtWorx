USE [DataWorkerTest]
GO
/****** Object:  Table [dbo].[Samples]    Script Date: 03/02/2017 00:26:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Samples](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[ChangeCount] [int] NOT NULL,
 CONSTRAINT [PK_Samples] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Samples] ADD  CONSTRAINT [DF_Samples_ChangeCount]  DEFAULT ((0)) FOR [ChangeCount]
GO

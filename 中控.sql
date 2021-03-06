USE [NXJC]
GO
/****** Object:  Table [dbo].[shift_CenterControlRecord]    Script Date: 2016-04-20 18:06:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[shift_CenterControlRecord](
	[KeyID] [uniqueidentifier] NOT NULL,
	[ProductionPrcessId] [varchar](64) NULL,
	[ProductionPrcessName] [varchar](64) NULL,
	[RecordType] [int] NULL,
	[RecordName] [varchar](20) NULL,
	[OrganizationID] [varchar](64) NULL,
	[DatabaseID] [varchar](64) NULL,
	[TemplateUrl] [varchar](128) NULL,
	[DisplayIndex] [int] NULL,
 CONSTRAINT [PK_shift_CenterControlRecord] PRIMARY KEY CLUSTERED 
(
	[KeyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[shift_CenterControlRecordItems]    Script Date: 2016-04-20 18:06:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[shift_CenterControlRecordItems](
	[ID] [uniqueidentifier] NOT NULL,
	[KeyId] [uniqueidentifier] NULL,
	[ContrastID] [varchar](64) NULL,
	[DataType] [int] NULL,
	[DisplayIndex] [int] NULL,
	[DCSTableName] [char](30) NULL,
	[Enabled] [bit] NULL,
 CONSTRAINT [PK_shift_CenterControlRecordItems] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[shift_CenterControlRecord] ADD  CONSTRAINT [DF_shift_CenterControlRecord_KeyID]  DEFAULT (newid()) FOR [KeyID]
GO
ALTER TABLE [dbo].[shift_CenterControlRecordItems] ADD  CONSTRAINT [DF_shift_CenterControlRecordItems_ID]  DEFAULT (newid()) FOR [ID]
GO

USE [Todo]
GO
/****** Object:  Table [dbo].[Division]    Script Date: 2025/4/6 下午 05:57:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Division](
	[DivisionId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[DivisionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Employee]    Script Date: 2025/4/6 下午 05:57:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employee](
	[EmployeeId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Account] [nvarchar](max) NOT NULL,
	[Password] [nvarchar](max) NOT NULL,
	[JobTitleId] [uniqueidentifier] NOT NULL,
	[DivisionId] [uniqueidentifier] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[EmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[JobTitle]    Script Date: 2025/4/6 下午 05:57:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[JobTitle](
	[JobTitleId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[JobTitleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TodoList]    Script Date: 2025/4/6 下午 05:57:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TodoList](
	[TodoId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[InsertTime] [datetime] NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
	[Enable] [bit] NOT NULL,
	[Orders] [int] NOT NULL,
	[InsertEmployeeId] [uniqueidentifier] NOT NULL,
	[UpdateEmployeeId] [uniqueidentifier] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TodoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UploadFile]    Script Date: 2025/4/6 下午 05:57:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UploadFile](
	[UploadFileId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Src] [nvarchar](max) NOT NULL,
	[TodoId] [uniqueidentifier] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UploadFileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[Division] ([DivisionId], [Name]) VALUES (N'00000000-0000-0000-0000-000000000001', N'系統管理部')
INSERT [dbo].[Division] ([DivisionId], [Name]) VALUES (N'20304f39-4ae9-49da-ada1-4e7cbe5b778f', N'局本部')
INSERT [dbo].[Division] ([DivisionId], [Name]) VALUES (N'126ccf74-337f-4316-bb18-cf7e3d2705a0', N'資訊室')
GO
INSERT [dbo].[Employee] ([EmployeeId], [Name], [Account], [Password], [JobTitleId], [DivisionId]) VALUES (N'00000000-0000-0000-0000-000000000001', N'系統管理員', N'admin@kcg.gov.tw', N'admin', N'00000000-0000-0000-0000-000000000001', N'00000000-0000-0000-0000-000000000001')
INSERT [dbo].[Employee] ([EmployeeId], [Name], [Account], [Password], [JobTitleId], [DivisionId]) VALUES (N'59308743-99e0-4d5a-b611-b0a7facaf21e', N'王大明', N'aa@kcg.gov.tw', N'aa', N'89a43b99-8bf9-4838-a1b9-8e54d0abda1d', N'126ccf74-337f-4316-bb18-cf7e3d2705a0')
GO
INSERT [dbo].[JobTitle] ([JobTitleId], [Name]) VALUES (N'00000000-0000-0000-0000-000000000001', N'系統管理員')
INSERT [dbo].[JobTitle] ([JobTitleId], [Name]) VALUES (N'd3aa8377-7081-4eaa-8abc-1856f1eddeb9', N'主任')
INSERT [dbo].[JobTitle] ([JobTitleId], [Name]) VALUES (N'fd8b497c-e229-4f17-9aad-2fbc86570bb3', N'設計師')
INSERT [dbo].[JobTitle] ([JobTitleId], [Name]) VALUES (N'23324afe-f1bb-4b8b-ad18-5dddb9702e2d', N'分析師')
INSERT [dbo].[JobTitle] ([JobTitleId], [Name]) VALUES (N'1d57adc9-f0d0-4b0e-81da-7ffd522429c9', N'股長')
INSERT [dbo].[JobTitle] ([JobTitleId], [Name]) VALUES (N'89a43b99-8bf9-4838-a1b9-8e54d0abda1d', N'助理程式設計師')
INSERT [dbo].[JobTitle] ([JobTitleId], [Name]) VALUES (N'fae4c0e9-be3d-49a4-900d-b12fef718765', N'管理師')
GO
INSERT [dbo].[TodoList] ([TodoId], [Name], [InsertTime], [UpdateTime], [Enable], [Orders], [InsertEmployeeId], [UpdateEmployeeId]) VALUES (N'1f3012b6-71ae-4e74-88fd-018ed53ed2d3', N'去上課', CAST(N'2021-02-14T17:03:50.000' AS DateTime), CAST(N'2021-02-12T17:03:50.083' AS DateTime), 0, 2, N'00000000-0000-0000-0000-000000000001', N'59308743-99e0-4d5a-b611-b0a7facaf21e')
INSERT [dbo].[TodoList] ([TodoId], [Name], [InsertTime], [UpdateTime], [Enable], [Orders], [InsertEmployeeId], [UpdateEmployeeId]) VALUES (N'ab712606-4bc2-4ba0-b4b9-074f75006235', N'去睡覺', CAST(N'2021-02-12T17:04:19.663' AS DateTime), CAST(N'2021-02-12T17:04:19.663' AS DateTime), 1, 5, N'00000000-0000-0000-0000-000000000001', N'59308743-99e0-4d5a-b611-b0a7facaf21e')
INSERT [dbo].[TodoList] ([TodoId], [Name], [InsertTime], [UpdateTime], [Enable], [Orders], [InsertEmployeeId], [UpdateEmployeeId]) VALUES (N'02af54d7-f59d-4e76-b744-1257757e576d', N'去開會', CAST(N'2021-02-13T17:03:23.440' AS DateTime), CAST(N'2021-02-12T17:03:50.083' AS DateTime), 0, 3, N'00000000-0000-0000-0000-000000000001', N'59308743-99e0-4d5a-b611-b0a7facaf21e')
INSERT [dbo].[TodoList] ([TodoId], [Name], [InsertTime], [UpdateTime], [Enable], [Orders], [InsertEmployeeId], [UpdateEmployeeId]) VALUES (N'79714295-7dc4-4862-a422-bec66abd215a', N'新增測試資料2', CAST(N'2021-03-28T20:26:25.027' AS DateTime), CAST(N'2021-03-28T20:26:25.027' AS DateTime), 1, 1, N'00000000-0000-0000-0000-000000000001', N'00000000-0000-0000-0000-000000000001')
INSERT [dbo].[TodoList] ([TodoId], [Name], [InsertTime], [UpdateTime], [Enable], [Orders], [InsertEmployeeId], [UpdateEmployeeId]) VALUES (N'3b3c5746-e1d6-4318-a1c3-ca5fbe084046', N'打電動', CAST(N'2021-02-10T17:04:03.503' AS DateTime), CAST(N'2021-02-12T17:04:03.503' AS DateTime), 0, 4, N'00000000-0000-0000-0000-000000000001', N'59308743-99e0-4d5a-b611-b0a7facaf21e')
INSERT [dbo].[TodoList] ([TodoId], [Name], [InsertTime], [UpdateTime], [Enable], [Orders], [InsertEmployeeId], [UpdateEmployeeId]) VALUES (N'42d626a2-4aea-4d59-b5c7-d7881d407fb3', N'寫程式', CAST(N'2021-02-11T00:00:00.000' AS DateTime), CAST(N'2021-02-12T00:00:00.000' AS DateTime), 1, 1, N'00000000-0000-0000-0000-000000000001', N'59308743-99e0-4d5a-b611-b0a7facaf21e')
INSERT [dbo].[TodoList] ([TodoId], [Name], [InsertTime], [UpdateTime], [Enable], [Orders], [InsertEmployeeId], [UpdateEmployeeId]) VALUES (N'a6131ade-e04b-4ccb-83a4-ef0ed78b4bf2', N'新增測試資料', CAST(N'2021-03-11T00:00:00.000' AS DateTime), CAST(N'2021-03-12T00:00:00.000' AS DateTime), 1, 1, N'00000000-0000-0000-0000-000000000001', N'00000000-0000-0000-0000-000000000001')
GO
INSERT [dbo].[UploadFile] ([UploadFileId], [Name], [Src], [TodoId]) VALUES (N'21bec572-0930-48fe-bea4-d91023037228', N'file2', N'src/file2', N'1f3012b6-71ae-4e74-88fd-018ed53ed2d3')
INSERT [dbo].[UploadFile] ([UploadFileId], [Name], [Src], [TodoId]) VALUES (N'49e186e8-fe27-4649-907a-de9921b0b1df', N'file1', N'src/file1', N'1f3012b6-71ae-4e74-88fd-018ed53ed2d3')
GO
ALTER TABLE [dbo].[Division] ADD  DEFAULT (newid()) FOR [DivisionId]
GO
ALTER TABLE [dbo].[Employee] ADD  DEFAULT (newid()) FOR [EmployeeId]
GO
ALTER TABLE [dbo].[JobTitle] ADD  DEFAULT (newid()) FOR [JobTitleId]
GO
ALTER TABLE [dbo].[TodoList] ADD  DEFAULT (newid()) FOR [TodoId]
GO
ALTER TABLE [dbo].[TodoList] ADD  DEFAULT (getdate()) FOR [InsertTime]
GO
ALTER TABLE [dbo].[TodoList] ADD  DEFAULT (getdate()) FOR [UpdateTime]
GO
ALTER TABLE [dbo].[UploadFile] ADD  DEFAULT (newid()) FOR [UploadFileId]
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_ToTable] FOREIGN KEY([JobTitleId])
REFERENCES [dbo].[JobTitle] ([JobTitleId])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_ToTable]
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_ToTable_1] FOREIGN KEY([DivisionId])
REFERENCES [dbo].[Division] ([DivisionId])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_ToTable_1]
GO
ALTER TABLE [dbo].[TodoList]  WITH CHECK ADD  CONSTRAINT [FK_Todo_ToTable] FOREIGN KEY([InsertEmployeeId])
REFERENCES [dbo].[Employee] ([EmployeeId])
GO
ALTER TABLE [dbo].[TodoList] CHECK CONSTRAINT [FK_Todo_ToTable]
GO
ALTER TABLE [dbo].[TodoList]  WITH CHECK ADD  CONSTRAINT [FK_Todo_ToTable_1] FOREIGN KEY([UpdateEmployeeId])
REFERENCES [dbo].[Employee] ([EmployeeId])
GO
ALTER TABLE [dbo].[TodoList] CHECK CONSTRAINT [FK_Todo_ToTable_1]
GO
ALTER TABLE [dbo].[UploadFile]  WITH CHECK ADD  CONSTRAINT [FK_File_ToTable] FOREIGN KEY([TodoId])
REFERENCES [dbo].[TodoList] ([TodoId])
GO
ALTER TABLE [dbo].[UploadFile] CHECK CONSTRAINT [FK_File_ToTable]
GO

DELETE FROM MemberMembers
DELETE FROM Messages
DELETE FROM Members

INSERT INTO [dbo].[Members] ([Pseudo], [Password], [Profile], [IsAdmin], [PicturePath]) VALUES (N'admin', N'admin', N'Je suis l''admin !!!', 1, N'admin1450366290.png')
INSERT INTO [dbo].[Members] ([Pseudo], [Password], [Profile], [IsAdmin], [PicturePath]) VALUES (N'angelina', N'angelina', NULL, 0, NULL)
INSERT INTO [dbo].[Members] ([Pseudo], [Password], [Profile], [IsAdmin], [PicturePath]) VALUES (N'audrey', N'audrey', NULL, 0, NULL)
INSERT INTO [dbo].[Members] ([Pseudo], [Password], [Profile], [IsAdmin], [PicturePath]) VALUES (N'ben', N'ben', N'Je suis Benoît.', 0, N'ben1479646086.png')
INSERT INTO [dbo].[Members] ([Pseudo], [Password], [Profile], [IsAdmin], [PicturePath]) VALUES (N'beyonce', N'beyonce', NULL, 0, NULL)
INSERT INTO [dbo].[Members] ([Pseudo], [Password], [Profile], [IsAdmin], [PicturePath]) VALUES (N'bob', N'bob', NULL, 0, NULL)
INSERT INTO [dbo].[Members] ([Pseudo], [Password], [Profile], [IsAdmin], [PicturePath]) VALUES (N'brad', N'brad', NULL, 0, NULL)
INSERT INTO [dbo].[Members] ([Pseudo], [Password], [Profile], [IsAdmin], [PicturePath]) VALUES (N'caro', N'caro', N'Caroline', 0, NULL)
INSERT INTO [dbo].[Members] ([Pseudo], [Password], [Profile], [IsAdmin], [PicturePath]) VALUES (N'dany', N'dany', NULL, 0, NULL)
INSERT INTO [dbo].[Members] ([Pseudo], [Password], [Profile], [IsAdmin], [PicturePath]) VALUES (N'donald', N'donald', NULL, 0, NULL)
INSERT INTO [dbo].[Members] ([Pseudo], [Password], [Profile], [IsAdmin], [PicturePath]) VALUES (N'fred', N'fred', NULL, 0, NULL)
INSERT INTO [dbo].[Members] ([Pseudo], [Password], [Profile], [IsAdmin], [PicturePath]) VALUES (N'george', N'george', NULL, 0, NULL)
INSERT INTO [dbo].[Members] ([Pseudo], [Password], [Profile], [IsAdmin], [PicturePath]) VALUES (N'guest', N'guest', NULL, 0, NULL)
INSERT INTO [dbo].[Members] ([Pseudo], [Password], [Profile], [IsAdmin], [PicturePath]) VALUES (N'marilyn', N'marilyn', NULL, 0, NULL)
INSERT INTO [dbo].[Members] ([Pseudo], [Password], [Profile], [IsAdmin], [PicturePath]) VALUES (N'uma', N'uma', NULL, 0, NULL)
INSERT INTO [dbo].[Members] ([Pseudo], [Password], [Profile], [IsAdmin], [PicturePath]) VALUES (N'will', N'will', NULL, 0, NULL)

INSERT INTO [dbo].[MemberMembers] ([Member_Pseudo], [Member_Pseudo1]) VALUES (N'bob', N'ben')
INSERT INTO [dbo].[MemberMembers] ([Member_Pseudo], [Member_Pseudo1]) VALUES (N'caro', N'ben')
INSERT INTO [dbo].[MemberMembers] ([Member_Pseudo], [Member_Pseudo1]) VALUES (N'ben', N'caro')
INSERT INTO [dbo].[MemberMembers] ([Member_Pseudo], [Member_Pseudo1]) VALUES (N'ben', N'fred')
INSERT INTO [dbo].[MemberMembers] ([Member_Pseudo], [Member_Pseudo1]) VALUES (N'caro', N'fred')
INSERT INTO [dbo].[MemberMembers] ([Member_Pseudo], [Member_Pseudo1]) VALUES (N'admin', N'guest')
INSERT INTO [dbo].[MemberMembers] ([Member_Pseudo], [Member_Pseudo1]) VALUES (N'ben', N'guest')

SET IDENTITY_INSERT [dbo].[Messages] ON
INSERT INTO [dbo].[Messages] ([MessageId], [DateTime], [Body], [IsPrivate], [Recipient_Pseudo], [Author_Pseudo]) VALUES (2, N'2018-07-09 10:11:33', N'message 1', 0, N'ben', N'ben')
INSERT INTO [dbo].[Messages] ([MessageId], [DateTime], [Body], [IsPrivate], [Recipient_Pseudo], [Author_Pseudo]) VALUES (3, N'2018-07-09 10:12:59', N'message 2', 0, N'ben', N'ben')
INSERT INTO [dbo].[Messages] ([MessageId], [DateTime], [Body], [IsPrivate], [Recipient_Pseudo], [Author_Pseudo]) VALUES (5, N'2018-07-09 10:14:03', N'message de caro', 0, N'ben', N'caro')
INSERT INTO [dbo].[Messages] ([MessageId], [DateTime], [Body], [IsPrivate], [Recipient_Pseudo], [Author_Pseudo]) VALUES (8, N'2018-07-09 10:58:10', N'test', 1, N'ben', N'ben')
INSERT INTO [dbo].[Messages] ([MessageId], [DateTime], [Body], [IsPrivate], [Recipient_Pseudo], [Author_Pseudo]) VALUES (9, N'2018-07-09 10:58:15', N'test', 0, N'ben', N'ben')
INSERT INTO [dbo].[Messages] ([MessageId], [DateTime], [Body], [IsPrivate], [Recipient_Pseudo], [Author_Pseudo]) VALUES (19, N'2018-07-09 11:29:20', N'myself', 0, N'caro', N'caro')
INSERT INTO [dbo].[Messages] ([MessageId], [DateTime], [Body], [IsPrivate], [Recipient_Pseudo], [Author_Pseudo]) VALUES (47, N'2018-07-09 11:34:44', N'a longer message for caro in order to see how it wrapped around in the message table.', 0, N'caro', N'ben')
INSERT INTO [dbo].[Messages] ([MessageId], [DateTime], [Body], [IsPrivate], [Recipient_Pseudo], [Author_Pseudo]) VALUES (48, N'2018-07-09 18:15:27', N'this is a message to fred', 0, N'fred', N'ben')
INSERT INTO [dbo].[Messages] ([MessageId], [DateTime], [Body], [IsPrivate], [Recipient_Pseudo], [Author_Pseudo]) VALUES (49, N'2018-07-09 18:15:36', N'this is a private message to fred', 1, N'fred', N'ben')
INSERT INTO [dbo].[Messages] ([MessageId], [DateTime], [Body], [IsPrivate], [Recipient_Pseudo], [Author_Pseudo]) VALUES (58, N'2018-07-19 00:16:01', N'hello', 0, N'fred', N'ben')
INSERT INTO [dbo].[Messages] ([MessageId], [DateTime], [Body], [IsPrivate], [Recipient_Pseudo], [Author_Pseudo]) VALUES (59, N'2018-07-19 00:17:41', N'aaa', 0, N'fred', N'ben')
INSERT INTO [dbo].[Messages] ([MessageId], [DateTime], [Body], [IsPrivate], [Recipient_Pseudo], [Author_Pseudo]) VALUES (61, N'2018-10-30 11:32:37', N'test', 0, N'admin', N'admin')
INSERT INTO [dbo].[Messages] ([MessageId], [DateTime], [Body], [IsPrivate], [Recipient_Pseudo], [Author_Pseudo]) VALUES (86, N'2018-12-16 12:50:29', N'ben to caro', 0, N'caro', N'ben')
SET IDENTITY_INSERT [dbo].[Messages] OFF

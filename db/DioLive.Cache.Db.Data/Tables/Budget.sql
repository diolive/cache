﻿CREATE TABLE [dbo].[Budget] (
    [Id]         UNIQUEIDENTIFIER NOT NULL,
    [AuthorId]   CHAR (36)        NOT NULL,
    [Name]       NVARCHAR (200)   NOT NULL,
    [Version]    TINYINT          DEFAULT (2) NOT NULL,
	[CurrencyId] CHAR(3)          NOT NULL
    CONSTRAINT [PK_Budget] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Budget_Users_AuthorId] FOREIGN KEY ([AuthorId]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [FK_Budget_Currency_CurrencyId] FOREIGN KEY ([CurrencyId]) REFERENCES [dbo].[Currency] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Budget_AuthorId]
    ON [dbo].[Budget]([AuthorId] ASC);


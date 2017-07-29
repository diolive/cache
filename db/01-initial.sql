USE [BlackMint]
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;
SET NUMERIC_ROUNDABORT OFF;
GO

PRINT N'Creating [dbo].[Users]...';

CREATE TABLE [dbo].[Users] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [DisplayName] NVARCHAR (100) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

PRINT N'Creating [dbo].[Currencies]...';

CREATE TABLE [dbo].[Currencies] (
    [Code]   CHAR (3)      NOT NULL,
    [Name]   NVARCHAR (25) NOT NULL,
    [Format] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Code] ASC)
);

PRINT N'Creating [dbo].[UserIdentities]...';

CREATE TABLE [dbo].[UserIdentities] (
    [NameIdentity] NVARCHAR (100) NOT NULL,
    [UserId]       INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([NameIdentity] ASC),
    CONSTRAINT [FK_UserIdentities_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);

PRINT N'Creating [dbo].[Books]...';

CREATE TABLE [dbo].[Books] (
    [Id]       INT            NOT NULL IDENTITY,
    [Name]     NVARCHAR (100) NOT NULL,
    [AuthorId] INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Books_Authors] FOREIGN KEY ([AuthorId]) REFERENCES [dbo].[Users] ([Id])
);

PRINT N'Creating [dbo].[BookAccess]...';

CREATE TABLE [dbo].[BookAccess] (
    [BookId] INT           NOT NULL,
    [UserId] INT           NOT NULL,
    [Role]   NVARCHAR (10) NOT NULL,
    PRIMARY KEY CLUSTERED ([BookId] ASC, [UserId] ASC),
    CONSTRAINT [FK_BookAccess_Books] FOREIGN KEY ([BookId]) REFERENCES [dbo].[Books] ([Id]),
    CONSTRAINT [FK_BookAccess_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);

PRINT N'Creating [dbo].[Plans]...';

CREATE TABLE [dbo].[Plans] (
    [Id]          INT             IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (100)  NOT NULL,
    [BookId]      INT             NOT NULL,
    [TargetSave]  DECIMAL (18, 2) NOT NULL,
    [CurrentSave] DECIMAL (18, 2) NOT NULL,
    [Currency]    CHAR (3)        NOT NULL,
    [Done]        BIT             NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Plans_Currencies] FOREIGN KEY ([Currency]) REFERENCES [dbo].[Currencies] ([Code]),
    CONSTRAINT [FK_Plans_Books] FOREIGN KEY ([BookId]) REFERENCES [dbo].[Books] ([Id])
);

PRINT N'Creating [dbo].[Purchases]...';

CREATE TABLE [dbo].[Purchases] (
    [Id]        INT             IDENTITY (1, 1) NOT NULL,
    [BookId]    INT             NOT NULL,
    [Seller]    NVARCHAR (100)  NOT NULL,
    [Date]      DATE            NOT NULL,
    [TotalCost] DECIMAL (18, 2) NOT NULL,
    [Currency]  CHAR (3)        NOT NULL,
    [Comments]  NVARCHAR (MAX)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Purchases_Currencies] FOREIGN KEY ([Currency]) REFERENCES [dbo].[Currencies] ([Code]),
    CONSTRAINT [FK_Purchases_Books] FOREIGN KEY ([BookId]) REFERENCES [dbo].[Books] ([Id])
);

PRINT N'Creating [dbo].[Incomes]...';

CREATE TABLE [dbo].[Incomes] (
    [Id]       INT             IDENTITY (1, 1) NOT NULL,
    [BookId]   INT             NOT NULL,
    [Source]   NVARCHAR (100)  NOT NULL,
    [Date]     DATE            NOT NULL,
    [Value]    DECIMAL (18, 2) NOT NULL,
    [Currency] CHAR (3)        NOT NULL,
    [Comments] NVARCHAR (MAX)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Incomes_Books] FOREIGN KEY ([BookId]) REFERENCES [dbo].[Books] ([Id]),
    CONSTRAINT [FK_Incomes_Currencies] FOREIGN KEY ([Currency]) REFERENCES [dbo].[Currencies] ([Code])
);

PRINT N'Creating [dbo].[PurchasesLog]...';

CREATE TABLE [dbo].[PurchasesLog] (
    [PurchaseId] INT           NOT NULL,
    [DateTime]   DATETIME2 (2) NOT NULL,
    [UserId]     INT           NOT NULL,
    PRIMARY KEY CLUSTERED ([PurchaseId] ASC, [DateTime] ASC),
    CONSTRAINT [FK_PurchasesLog_Purchases] FOREIGN KEY ([PurchaseId]) REFERENCES [dbo].[Purchases] ([Id]),
    CONSTRAINT [FK_PurchasesLog_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);

PRINT N'Creating [dbo].[IncomesLog]...';

CREATE TABLE [dbo].[IncomesLog] (
    [IncomeId] INT           NOT NULL,
    [DateTime] DATETIME2 (2) NOT NULL,
    [UserId]   INT           NOT NULL,
    PRIMARY KEY CLUSTERED ([IncomeId] ASC, [DateTime] ASC),
    CONSTRAINT [FK_IncomesLog_Incomes] FOREIGN KEY ([IncomeId]) REFERENCES [dbo].[Incomes] ([Id]),
    CONSTRAINT [FK_IncomesLog_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);

PRINT N'Creating [dbo].[PurchaseItems]...';

CREATE TABLE [dbo].[PurchaseItems] (
    [Id]         INT             IDENTITY (1, 1) NOT NULL,
    [PurchaseId] INT             NOT NULL,
    [Name]       NVARCHAR (100)  NOT NULL,
    [Price]      DECIMAL (18, 2) NOT NULL,
    [Currency]   CHAR (3)        NOT NULL,
    [Count]      DECIMAL (18, 2) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PurchaseItems_Purchases] FOREIGN KEY ([PurchaseId]) REFERENCES [dbo].[Purchases] ([Id]),
    CONSTRAINT [FK_PurchaseItems_Currencies] FOREIGN KEY ([Currency]) REFERENCES [dbo].[Currencies] ([Code])
);

PRINT N'Creating [dbo].[PurchaseTags]...';

CREATE TABLE [dbo].[PurchaseTags] (
    [PurchaseItemId] INT           NOT NULL,
    [Tag]            NVARCHAR (20) NOT NULL,
    PRIMARY KEY CLUSTERED ([PurchaseItemId] ASC, [Tag] ASC),
    CONSTRAINT [FK_PurchaseTags_PurchaseItems] FOREIGN KEY ([PurchaseItemId]) REFERENCES [dbo].[PurchaseItems] ([Id])
);

PRINT N'Creating [dbo].[IncomeTags]...';

CREATE TABLE [dbo].[IncomeTags] (
    [IncomeId] INT           NOT NULL,
    [Tag]      NVARCHAR (20) NOT NULL,
    PRIMARY KEY CLUSTERED ([IncomeId] ASC, [Tag] ASC),
    CONSTRAINT [FK_IncomeTags_Incomes] FOREIGN KEY ([IncomeId]) REFERENCES [dbo].[Incomes] ([Id])
);

PRINT N'Inserting initial data into [dbo].[Currencies]';

INSERT INTO [dbo].[Currencies] ([Code], [Name], [Format])
VALUES
	('RUB', N'Russian Ruble', N'#,###.## ₽'),
	('USD', N'US Dollar', N'$#,###.##;($#,###.##)'),
	('EUR', N'Euro', N'€#,###.##')

PRINT N'Update complete.';
GO
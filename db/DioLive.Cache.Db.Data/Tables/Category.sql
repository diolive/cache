CREATE TABLE [dbo].[Category] (
    [Id]       INT              IDENTITY (1, 1) NOT NULL,
    [Name]     NVARCHAR (300)   NOT NULL,
    [OwnerId]  CHAR (36)        NULL,
    [BudgetId] UNIQUEIDENTIFIER NULL,
    [Color]    INT              DEFAULT (abs(checksum(newid())%(16777216))) NOT NULL,
    [ParentId] INT              NULL,
    CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Category_Users_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [FK_Category_Budget_BudgetId] FOREIGN KEY ([BudgetId]) REFERENCES [dbo].[Budget] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Category_Category_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[Category] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Category_OwnerId]
    ON [dbo].[Category]([OwnerId] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Category_BudgetId_Name]
    ON [dbo].[Category]([BudgetId] ASC, [Name] ASC) WHERE ([BudgetId] IS NOT NULL AND [Name] IS NOT NULL);


GO
CREATE NONCLUSTERED INDEX [IX_Category_ParentId]
    ON [dbo].[Category]([ParentId] ASC);


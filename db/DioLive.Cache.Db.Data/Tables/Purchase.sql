CREATE TABLE [dbo].[Purchase] (
    [Id]           UNIQUEIDENTIFIER NOT NULL,
    [CategoryId]   INT              NOT NULL,
    [Date]         DATE             NOT NULL,
    [Name]         NVARCHAR (300)   NOT NULL,
    [Cost]         INT              NOT NULL,
    [Shop]         NVARCHAR (200)   NULL,
    [AuthorId]     CHAR(36)         NOT NULL,
    [Comments]     NVARCHAR (500)   NULL,
    [CreateDate]   DATETIME2 (7)    NOT NULL,
    [BudgetId]     UNIQUEIDENTIFIER NOT NULL,
    [LastEditorId] CHAR (36)        NULL,
    CONSTRAINT [PK_Purchase] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Purchase_Users_AuthorId] FOREIGN KEY ([AuthorId]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [FK_Purchase_Users_LastEditorId] FOREIGN KEY ([LastEditorId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_Purchase_Budget_BudgetId] FOREIGN KEY ([BudgetId]) REFERENCES [dbo].[Budget] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Purchase_Category_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Category] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Purchase_CategoryId]
    ON [dbo].[Purchase]([CategoryId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Purchase_BudgetId]
    ON [dbo].[Purchase]([BudgetId] ASC);

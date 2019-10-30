CREATE TABLE [dbo].[Plan] (
    [Id]       INT              IDENTITY (1, 1) NOT NULL,
    [AuthorId] CHAR (36)        NOT NULL,
    [BudgetId] UNIQUEIDENTIFIER NOT NULL,
    [BuyDate]  DATETIME2 (7)    NULL,
    [BuyerId]  CHAR (36)        NULL,
    [Comments] NVARCHAR (500)   NULL,
    [Name]     NVARCHAR (300)   NOT NULL,
    CONSTRAINT [PK_Plan] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Plan_Users_AuthorId] FOREIGN KEY ([AuthorId]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [FK_Plan_Users_BuyerId] FOREIGN KEY ([BuyerId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_Plan_Budget_BudgetId] FOREIGN KEY ([BudgetId]) REFERENCES [dbo].[Budget] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Plan_AuthorId]
    ON [dbo].[Plan]([AuthorId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Plan_BudgetId]
    ON [dbo].[Plan]([BudgetId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Plan_BuyerId]
    ON [dbo].[Plan]([BuyerId] ASC);


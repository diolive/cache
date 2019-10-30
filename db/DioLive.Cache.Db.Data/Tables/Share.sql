CREATE TABLE [dbo].[Share] (
    [BudgetId] UNIQUEIDENTIFIER NOT NULL,
    [UserId]   CHAR (36)        NOT NULL,
    [Access]   TINYINT          NOT NULL,
    CONSTRAINT [PK_Share] PRIMARY KEY CLUSTERED ([BudgetId] ASC, [UserId] ASC),
    CONSTRAINT [FK_Share_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Share_Budget_BudgetId] FOREIGN KEY ([BudgetId]) REFERENCES [dbo].[Budget] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Share_UserId]
    ON [dbo].[Share]([UserId] ASC);


CREATE TABLE [dbo].[Options] (
    [UserId]           CHAR (36) NOT NULL,
    [PurchaseGrouping] INT       NOT NULL,
    [ShowPlanList]     BIT       DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Options] PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [FK_Options_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
);


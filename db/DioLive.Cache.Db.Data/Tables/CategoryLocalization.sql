CREATE TABLE [dbo].[CategoryLocalization] (
    [CategoryId] INT           NOT NULL,
    [Culture]    NVARCHAR (10) NOT NULL,
    [Name]       NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_CategoryLocalization] PRIMARY KEY CLUSTERED ([CategoryId] ASC, [Culture] ASC),
    CONSTRAINT [FK_CategoryLocalization_Category_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Category] ([Id]) ON DELETE CASCADE
);


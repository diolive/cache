CREATE TABLE [dbo].[Users] (
    [Id]   CHAR (36)      NOT NULL,
    [Name] NVARCHAR (256) NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Name]
    ON [dbo].[Users]([Name] ASC);

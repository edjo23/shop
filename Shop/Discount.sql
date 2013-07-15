CREATE TABLE [dbo].[Discount]
(
	[Id] UNIQUEIDENTIFIER NOT NULL , 
    [Description] NVARCHAR(MAX) NOT NULL, 
    CONSTRAINT [PK_Discount] PRIMARY KEY ([Id])
)

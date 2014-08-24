CREATE TABLE [dbo].[Product]
(
	[Id] UNIQUEIDENTIFIER NOT NULL , 
	[Code] NVARCHAR(MAX) NOT NULL,
	[Group] NVARCHAR(MAX) NOT NULL,
    [Description] NVARCHAR(MAX) NOT NULL, 
    [Cost] MONEY NULL,
    [Price] MONEY NOT NULL,
	[QuantityOnHand] INT NOT NULL, 
    CONSTRAINT [PK_Product] PRIMARY KEY ([Id])    
)

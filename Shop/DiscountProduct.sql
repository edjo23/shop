CREATE TABLE [dbo].[DiscountProduct]
(
	[Id] UNIQUEIDENTIFIER NOT NULL , 
    [DiscountId] UNIQUEIDENTIFIER NOT NULL, 
    [ProductId] UNIQUEIDENTIFIER NOT NULL, 
    [Discount] DECIMAL(5, 2) NOT NULL, 
    CONSTRAINT [PK_DiscountProduct] PRIMARY KEY ([Id])
)

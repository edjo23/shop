CREATE TABLE [dbo].[InvoiceItem]
(
    [Id] UNIQUEIDENTIFIER NOT NULL, 
	[InvoiceId] UNIQUEIDENTIFIER NOT NULL , 
    [ItemNumber] INT NOT NULL, 
    [ProductId] UNIQUEIDENTIFIER NOT NULL, 
	[Price] MONEY NOT NULL,
    [Quantity] INT NOT NULL, 
    CONSTRAINT [PK_InvoiceItem] PRIMARY KEY ([Id]) 
)

GO

CREATE UNIQUE INDEX [IX_InvoiceItem_InvoiceId] ON [dbo].[InvoiceItem] ([InvoiceId], [ItemNumber])

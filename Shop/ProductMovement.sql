CREATE TABLE [dbo].[ProductMovement]
(
	[Id] UNIQUEIDENTIFIER NOT NULL , 
	[ProductId] UNIQUEIDENTIFIER NOT NULL,
	[MovementType] INT NOT NULL,
    [Quantity] INT NOT NULL,
    [DateTime] DATETIMEOFFSET NOT NULL,
    [SourceId] UNIQUEIDENTIFIER NULL, 
	[SourceItemNumber] INT NULL,
    CONSTRAINT [PK_ProductMovement] PRIMARY KEY ([Id])        
)

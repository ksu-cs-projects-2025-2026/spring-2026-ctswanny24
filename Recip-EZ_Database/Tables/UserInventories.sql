CREATE TABLE [dbo].[UserInventories] (
    [UserInventoryId] INT           IDENTITY (1, 1) NOT NULL,
    [UserId]          INT           NOT NULL,
    [IngredientId]    INT           NOT NULL,
    [Unit]            INT           NOT NULL,
    [Quantity]        FLOAT (53)    NOT NULL,
    [DateAdded]       DATETIME2 (7) NOT NULL,
    [ExpirationDate]  DATETIME2 (7) NULL,
    CONSTRAINT [PK_UserInventories] PRIMARY KEY CLUSTERED ([UserInventoryId] ASC)
);


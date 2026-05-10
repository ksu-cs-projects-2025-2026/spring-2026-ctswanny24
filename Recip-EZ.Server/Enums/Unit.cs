namespace Recip_EZ.Server.Enums
{
    /// <summary>
    /// Enum of measurement units for ingredients in the user's inventory and recipes
    /// </summary>
    public enum Unit
    {
        // Count / discrete items
        Piece = 0,
        Item = 1,
        Slice = 2,
        Clove = 3,

        // Volume
        Teaspoon = 10,
        Tablespoon = 11,
        FluidOunce = 12,
        Cup = 13,
        Pint = 14,
        Quart = 15,
        Gallon = 16,
        Milliliter = 17,
        Liter = 18,

        // Weight / mass
        Ounce = 30,
        Pound = 31,
        Gram = 32,
        Kilogram = 33,

        // Packaging / common grocery units
        Can = 50,
        Bottle = 51,
        Jar = 52,
        Package = 53,
        Bag = 54,
        Box = 55,

        // Misc / fallback
        ToTaste = 90,
        Unknown = 99
    }
}

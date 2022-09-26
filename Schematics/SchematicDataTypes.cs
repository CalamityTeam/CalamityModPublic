namespace CalamityMod.Schematics
{
    public struct ChestItem
    {
        internal int Type;
        internal int Stack;
        internal ChestItem(int type, int stack)
        {
            Type = type;
            Stack = stack;
        }
    }

    // The numbers assigned to these anchor types correspond to the layout of a numpad:
    // 7 8 9
    // 4 5 6
    // 1 2 3
    public enum SchematicAnchor
    {
        Default = 0, // default is treated as equivalent to top left, but top left can be explicitly specified
        TopLeft = 7,
        TopCenter = 8,
        TopRight = 9,
        CenterLeft = 4,
        Center = 5,
        CenterRight = 6,
        BottomLeft = 1,
        BottomCenter = 2,
        BottomRight = 3,
    }
}

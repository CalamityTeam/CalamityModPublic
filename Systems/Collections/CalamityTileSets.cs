using CalamityMod.Tiles.SunkenSea;
using CalamityMod.Walls;
using CalamityMod.Walls.UnsafeWalls;
using ReLogic.Reflection;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Systems.Collections
{
    [ReinitializeDuringResizeArrays]
    public static class CalamityTileSets
    {
        private static SetFactory Factory = TileID.Sets.Factory;

        /// <summary>
        /// Should only contain modded tiles. If <see langword="true"/> for a tile type, then that tile can be replaced by the Abyss during world generation.<br/>
        /// Unused by Calamity itself, and is only used for external mods.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        internal static bool[] CanBeReplacedByAbyssGeneration = Factory.CreateNamedSet("CanBeReplacedByAbyssGeneration")
            .Description("Allows the tile to be replaced by the Abyss during world generation.")
            .RegisterBoolSet();

        /// <summary>
        /// If <see langword="true"/> for a tile type, it will not perform its BlendMerge in the PostDraw hook, and instead will be drawn after every solid tile has been drawn to the screen.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        internal static bool[] DrawBlendMergeAfterSolidTile = Factory.CreateNamedSet("DrawBlendMergeAfterSolidTile")
            .Description("Causes this tile to have blending be drawn after all solid tiles.")
            .RegisterBoolSet(TileType<SeaPrism>());

        /// <summary>
        /// If <see langword="true"/> for a tile type, it will be considered an Abyss background wall.<br/>
        /// Used for spawning water when breaking Abyss tiles, discouraging Teleportation Potion teleporting, and spawning Primordial Wyrm when using Potion of Return.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        internal static bool[] IsAbyssWall = Factory.CreateNamedSet("IsAbyssWall")
            .Description("Labels this wall as an Abyss wall.")
            .RegisterBoolSet(WallType<UnsafeSulphurousShaleWall>(), WallType<UnsafeAbyssGravelWall>(), WallType<PyreMantleWall>(), WallType<UnsafeVoidstoneWall>(),
                WallType<HardenedSulphurousSandstoneWall>(), WallType<UnsafeSulphurousSandstoneWall>());
    }
}

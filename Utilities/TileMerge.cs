using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Utilities
{
    public static class TileMerge
    {
        public static void MergeTile(int type, int mergeType, bool merge = true)
        {
            if (type != mergeType)
            {
                Main.tileMerge[type][mergeType] = merge;
                Main.tileMerge[mergeType][type] = merge;
            }
        }

        /// <summary>
        /// Makes the tile merge with the most common types of naturally generated tiles
        /// </summary>
        /// <param name="type"></param>
        public static void MergeGeneralTiles(int type)
        {
            Mod mod = ModContent.GetInstance<CalamityMod>();
            //Soils
            MergeTile(type, TileID.ClayBlock);
            MergeTile(type, TileID.Dirt);
            MergeTile(type, TileID.Mud);
            //Sands
            MergeTile(type, TileID.Sand);
            MergeTile(type, TileID.Ebonsand);
            MergeTile(type, TileID.Crimsand);
            MergeTile(type, TileID.Pearlsand);
            //Snows
            MergeTile(type, TileID.SnowBlock);
            //Stones
            MergeTile(type, TileID.Stone);
            MergeTile(type, TileID.Ebonstone);
            MergeTile(type, TileID.Crimstone);
            MergeTile(type, TileID.Pearlstone);
            //Calam general tiles
            MergeTile(type, mod.TileType("AstralDirt"));
            MergeTile(type, mod.TileType("AstralStone"));
            MergeTile(type, mod.TileType("Navystone"));
            MergeTile(type, mod.TileType("EutrophicSand"));
            MergeTile(type, mod.TileType("AbyssGravel"));
            MergeTile(type, mod.TileType("Voidstone"));
        }

        public static void MergeOreTiles(int type)
        {
            Mod mod = ModContent.GetInstance<CalamityMod>();
            //Vanilla Ores
            MergeTile(type, TileID.Copper);
            MergeTile(type, TileID.Tin);
            MergeTile(type, TileID.Iron);
            MergeTile(type, TileID.Lead);
            MergeTile(type, TileID.Silver);
            MergeTile(type, TileID.Tungsten);
            MergeTile(type, TileID.Gold);
            MergeTile(type, TileID.Demonite);
            MergeTile(type, TileID.Crimtane);
            MergeTile(type, TileID.Cobalt);
            MergeTile(type, TileID.Palladium);
            MergeTile(type, TileID.Mythril);
            MergeTile(type, TileID.Platinum);
            MergeTile(type, TileID.Orichalcum);
            MergeTile(type, TileID.Adamantite);
            MergeTile(type, TileID.Titanium);
            MergeTile(type, TileID.LunarOre);
            //Calam Ores
            MergeTile(type, mod.TileType("AerialiteOre"));
            MergeTile(type, mod.TileType("CryonicOre"));
            MergeTile(type, mod.TileType("PerennialOre"));
            MergeTile(type, mod.TileType("CharredOre"));
            MergeTile(type, mod.TileType("ChaoticOre"));
            MergeTile(type, mod.TileType("AstralOre"));
            MergeTile(type, mod.TileType("UelibloomOre"));
            MergeTile(type, mod.TileType("AuricOre"));
        }

        /// <summary>
        /// Makes the tile merge with all the tile types that generate within various types of desert, sunken sea and related tiles
        /// </summary>
        /// <param name="type"></param>
        public static void MergeDesertTiles(int type)
        {
            Mod mod = ModContent.GetInstance<CalamityMod>();
            //Sands
            MergeTile(type, TileID.Sand);
            MergeTile(type, TileID.Ebonsand);
            MergeTile(type, TileID.Crimsand);
            MergeTile(type, TileID.Pearlsand);
            //Hardened Sands
            MergeTile(type, TileID.HardenedSand);
            MergeTile(type, TileID.CorruptHardenedSand);
            MergeTile(type, TileID.CrimsonHardenedSand);
            MergeTile(type, TileID.HallowHardenedSand);
            //Sandstones
            MergeTile(type, TileID.Sandstone);
            MergeTile(type, TileID.CorruptSandstone);
            MergeTile(type, TileID.CrimsonSandstone);
            MergeTile(type, TileID.HallowSandstone);
            //Misc relevant desert tiles
            MergeTile(type, TileID.FossilOre);
            MergeTile(type, TileID.DesertFossil);
            //Calam desert tiles
            MergeTile(type, mod.TileType("AstralSand"));
            MergeTile(type, mod.TileType("HardenedAstralSand"));
            MergeTile(type, mod.TileType("AstralSandstone"));
            //Sunken Sea tiles
            MergeTile(type, mod.TileType("EutrophicSand"));
            MergeTile(type, mod.TileType("Navystone"));
            MergeTile(type, mod.TileType("SeaPrism"));
        }

        /// <summary>
        /// Makes the tile merge with all the tile types that generate within various types of snow or snow biome related tiles
        /// </summary>
        /// <param name="type"></param>
        public static void MergeSnowTiles(int type)
        {
            Mod mod = ModContent.GetInstance<CalamityMod>();
            //Snow
            MergeTile(type, TileID.SnowBlock);
            //Ices
            MergeTile(type, TileID.IceBlock);
            MergeTile(type, TileID.CorruptIce);
            MergeTile(type, TileID.FleshIce);
            MergeTile(type, TileID.HallowedIce);
            //Calam snow tiles
            MergeTile(type, mod.TileType("AstralIce"));
        }

        /// <summary>
        /// Makes the tile merge with all the tile types that generate within various types of hell or hell biome related tiles
        /// </summary>
        /// <param name="type"></param>
        public static void MergeHellTiles(int type)
        {
            Mod mod = ModContent.GetInstance<CalamityMod>();
            //Terrain
            MergeTile(type, TileID.Ash);
            MergeTile(type, TileID.Hellstone);
            //Houses
            MergeTile(type, TileID.ObsidianBrick);
            MergeTile(type, TileID.HellstoneBrick);
            //Crag tiles
            MergeTile(type, mod.TileType("BrimstoneSlag"));
        }

        /// <summary>
        /// Makes the tile merge with all the tile types that generate within various types of abyss or abyss biome related tiles
        /// </summary>
        /// <param name="type"></param>
        public static void MergeAbyssTiles(int type)
        {
            Mod mod = ModContent.GetInstance<CalamityMod>();
            //Sulphurous Sea
            MergeTile(type, mod.TileType("SulphurousSand"));
            //Abyss
            MergeTile(type, mod.TileType("AbyssGravel"));
            MergeTile(type, mod.TileType("Voidstone"));
            MergeTile(type, mod.TileType("PlantyMush"));
            MergeTile(type, mod.TileType("Tenebris"));
            MergeTile(type, mod.TileType("ChaoticOre"));
        }

        /// <summary>
        /// Makes the tile merge with all the tile types that generate within various types of astral tiles
        /// </summary>
        /// <param name="type"></param>
        public static void MergeAstralTiles(int type)
        {
            Mod mod = ModContent.GetInstance<CalamityMod>();
            //Astral
            MergeTile(type, mod.TileType("AstralDirt"));
            MergeTile(type, mod.TileType("AstralStone"));
            MergeTile(type, mod.TileType("AstralMonolith"));
            //Astral Desert
            MergeTile(type, mod.TileType("AstralSand"));
            MergeTile(type, mod.TileType("HardenedAstralSand"));
            MergeTile(type, mod.TileType("AstralSandstone"));
            //Astral Snow
            MergeTile(type, mod.TileType("AstralIce"));
        }

        /// <summary>
        /// Makes the tile merge with all the decorative 'smooth' tiles
        /// </summary>
        /// <param name="type"></param>
        public static void MergeSmoothTiles(int type)
        {
            Mod mod = ModContent.GetInstance<CalamityMod>();
            //Vanilla
            MergeTile(type, TileID.MarbleBlock);
            MergeTile(type, TileID.GraniteBlock);
            //Calam
            MergeTile(type, mod.TileType("SmoothNavystone"));
            MergeTile(type, mod.TileType("SmoothBrimstoneSlag"));
            MergeTile(type, mod.TileType("SmoothAbyssGravel"));
            MergeTile(type, mod.TileType("SmoothVoidstone"));
        }

        /// <summary>
        /// Makes the tile merge with other mergable decorative tiles
        /// </summary>
        /// <param name="type"></param>
        public static void MergeDecorativeTiles(int type)
        {
            Mod mod = ModContent.GetInstance<CalamityMod>();
            //Vanilla decor
            Main.tileBrick[type] = true;
            //Calam
            MergeTile(type, mod.TileType("CryonicBrick"));
            MergeTile(type, mod.TileType("PerennialBrick"));
            MergeTile(type, mod.TileType("UelibloomBrick"));
            MergeTile(type, mod.TileType("OccultStone"));
            MergeTile(type, mod.TileType("ProfanedSlab"));
            MergeTile(type, mod.TileType("RunicProfanedBrick"));
            MergeTile(type, mod.TileType("AshenSlab"));
            MergeTile(type, mod.TileType("VoidstoneSlab"));
        }
    }
}

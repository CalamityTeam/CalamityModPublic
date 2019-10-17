using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod
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
            MergeTile(type, ModContent.TileType<Tiles.AstralDirt>());
            MergeTile(type, ModContent.TileType<Tiles.AstralStone>());
            MergeTile(type, ModContent.TileType<Tiles.Navystone>());
            MergeTile(type, ModContent.TileType<Tiles.EutrophicSand>());
            MergeTile(type, ModContent.TileType<Tiles.AbyssGravel>());
            MergeTile(type, ModContent.TileType<Tiles.Voidstone>());
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
            MergeTile(type, ModContent.TileType<Tiles.AerialiteOre>());
            MergeTile(type, ModContent.TileType<Tiles.CryonicOre>());
            MergeTile(type, ModContent.TileType<Tiles.PerennialOre>());
            MergeTile(type, ModContent.TileType<Tiles.CharredOre>());
            MergeTile(type, ModContent.TileType<Tiles.ChaoticOre>());
            MergeTile(type, ModContent.TileType<Tiles.AstralOre>());
            MergeTile(type, ModContent.TileType<Tiles.UelibloomOre>());
            MergeTile(type, ModContent.TileType<Tiles.AuricOre>());
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
            MergeTile(type, ModContent.TileType<Tiles.AstralSand>());
            MergeTile(type, ModContent.TileType<Tiles.HardenedAstralSand>());
            MergeTile(type, ModContent.TileType<Tiles.AstralSandstone>());
            //Sunken Sea tiles
            MergeTile(type, ModContent.TileType<Tiles.EutrophicSand>());
            MergeTile(type, ModContent.TileType<Tiles.Navystone>());
            MergeTile(type, ModContent.TileType<Tiles.SeaPrism>());
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
            MergeTile(type, ModContent.TileType<Tiles.AstralIce>());
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
            MergeTile(type, ModContent.TileType<Tiles.BrimstoneSlag>());
        }

        /// <summary>
        /// Makes the tile merge with all the tile types that generate within various types of abyss or abyss biome related tiles
        /// </summary>
        /// <param name="type"></param>
        public static void MergeAbyssTiles(int type)
        {
            Mod mod = ModContent.GetInstance<CalamityMod>();
            //Sulphurous Sea
            MergeTile(type, ModContent.TileType<Tiles.SulphurousSand>());
            //Abyss
            MergeTile(type, ModContent.TileType<Tiles.AbyssGravel>());
            MergeTile(type, ModContent.TileType<Tiles.Voidstone>());
            MergeTile(type, ModContent.TileType<Tiles.PlantyMush>());
            MergeTile(type, ModContent.TileType<Tiles.Tenebris>());
            MergeTile(type, ModContent.TileType<Tiles.ChaoticOre>());
        }

        /// <summary>
        /// Makes the tile merge with all the tile types that generate within various types of astral tiles
        /// </summary>
        /// <param name="type"></param>
        public static void MergeAstralTiles(int type)
        {
            Mod mod = ModContent.GetInstance<CalamityMod>();
            //Astral
            MergeTile(type, ModContent.TileType<Tiles.AstralDirt>());
            MergeTile(type, ModContent.TileType<Tiles.AstralStone>());
            MergeTile(type, ModContent.TileType<Tiles.AstralMonolith>());
            //Astral Desert
            MergeTile(type, ModContent.TileType<Tiles.AstralSand>());
            MergeTile(type, ModContent.TileType<Tiles.HardenedAstralSand>());
            MergeTile(type, ModContent.TileType<Tiles.AstralSandstone>());
            //Astral Snow
            MergeTile(type, ModContent.TileType<Tiles.AstralIce>());
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
            MergeTile(type, ModContent.TileType<Tiles.SmoothNavystone>());
            MergeTile(type, ModContent.TileType<Tiles.SmoothBrimstoneSlag>());
            MergeTile(type, ModContent.TileType<Tiles.SmoothAbyssGravel>());
            MergeTile(type, ModContent.TileType<Tiles.SmoothVoidstone>());
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
            MergeTile(type, ModContent.TileType<Tiles.CryonicBrick>());
            MergeTile(type, ModContent.TileType<Tiles.PerennialBrick>());
            MergeTile(type, ModContent.TileType<Tiles.UelibloomBrick>());
            MergeTile(type, ModContent.TileType<Tiles.OccultStone>());
            MergeTile(type, ModContent.TileType<Tiles.ProfanedSlab>());
            MergeTile(type, ModContent.TileType<Tiles.RunicProfanedBrick>());
            MergeTile(type, ModContent.TileType<Tiles.AshenSlab>());
            MergeTile(type, ModContent.TileType<Tiles.VoidstoneSlab>());
        }
    }
}

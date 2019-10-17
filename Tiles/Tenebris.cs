
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader; using CalamityMod.Dusts;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Dusts; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Tiles
{
    public class Tenebris : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;

            TileMerge.MergeGeneralTiles(Type);
            TileMerge.MergeAbyssTiles(Type);

            dustType = 44;
            drop = ModContent.ItemType<Items.Tenebris>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Tenebris");
            AddMapEntry(new Color(0, 100, 100), name);
            mineResist = 3f;
            minPick = 200;
            soundType = 21;
        }

        public override bool CanExplode(int i, int j)
        {
            return NPC.downedPlantBoss || CalamityWorld.downedCalamitas;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!closer && j < Main.maxTilesY - 205)
            {
                if (Main.tile[i, j].liquid <= 0)
                {
                    Main.tile[i, j].liquid = 255;
                    Main.tile[i, j].lava(false);
                }
            }
        }

        public override void RandomUpdate(int i, int j)
        {
            if (NPC.downedPlantBoss || CalamityWorld.downedCalamitas)
            {
                int random = WorldGen.genRand.Next(4);
                if (random == 0)
                {
                    i++;
                }
                if (random == 1)
                {
                    i--;
                }
                if (random == 2)
                {
                    j++;
                }
                if (random == 3)
                {
                    j--;
                }
                if (Main.tile[i, j] != null)
                {
                    if (Main.tile[i, j].active())
                    {
                        if (Main.tile[i, j].type == ModContent.TileType<PlantyMush>() && WorldGen.genRand.Next(5) == 0 && !Main.tile[i, j].lava())
                        {
                            Main.tile[i, j].type = (ushort)ModContent.TileType<Tenebris>();
                            WorldGen.SquareTileFrame(i, j, true);
                            if (Main.netMode == 2)
                            {
                                NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
                            }
                        }
                    }
                }
            }
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            CustomTileFraming.FrameTileForCustomMerge(i, j, Type, ModContent.TileType<AbyssGravel>(), false, false, false, false, resetFrame);
            return false;
        }
    }
}

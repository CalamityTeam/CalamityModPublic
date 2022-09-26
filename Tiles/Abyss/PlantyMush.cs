using CalamityMod.Walls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Abyss
{
    public class PlantyMush : ModTile
    {
        public static readonly SoundStyle MineSound = new("CalamityMod/Sounds/Custom/PlantyMushMine", 3);
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithAbyss(Type);

            DustType = 2;
            ItemDrop = ModContent.ItemType<Items.Placeables.PlantyMush>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Planty Mush");
            AddMapEntry(new Color(0, 120, 0), name);
            MineResist = 1f;
            HitSound = MineSound;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!closer && j < Main.maxTilesY - 205)
            {
                if (Main.tile[i, j].LiquidAmount <= 0)
                {
                    Main.tile[i, j].LiquidAmount = 255;
                    Main.tile[i, j].Get<LiquidData>().LiquidType = LiquidID.Water;
                }
            }
        }

        public override void RandomUpdate(int i, int j)
        {
            int num8 = WorldGen.genRand.Next((int)Main.rockLayer, (int)(Main.rockLayer + (double)Main.maxTilesY * 0.143));
            if (Main.tile[i, j + 1] != null)
            {
                if (!Main.tile[i, j + 1].HasTile && Main.tile[i, j + 1].TileType != (ushort)ModContent.TileType<ViperVines>())
                {
                    if (Main.tile[i, j + 1].LiquidAmount == 255 &&
                        Main.tile[i, j + 1].WallType == (ushort)ModContent.WallType<AbyssGravelWall>() &&
                        Main.tile[i, j + 1].LiquidType != LiquidID.Lava)
                    {
                        bool flag13 = false;
                        for (int num52 = num8; num52 > num8 - 10; num52--)
                        {
                            if (Main.tile[i, num52].BottomSlope)
                            {
                                flag13 = false;
                                break;
                            }
                            if (Main.tile[i, num52].HasTile && !Main.tile[i, num52].BottomSlope)
                            {
                                flag13 = true;
                                break;
                            }
                        }
                        if (flag13)
                        {
                            int num53 = i;
                            int num54 = j + 1;
                            Main.tile[num53, num54].TileType = (ushort)ModContent.TileType<ViperVines>();
                            Main.tile[num53, num54].Get<TileWallWireStateData>().HasTile = true;
                            WorldGen.SquareTileFrame(num53, num54, true);
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendTileSquare(-1, num53, num54, 3, TileChangeType.None);
                            }
                        }
                    }
                }
            }
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, ModContent.TileType<AbyssGravel>(), false, false, false, false, resetFrame);
            return false;
        }
    }
}

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
    public class VernalSoil : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);

            TileID.Sets.CanBeDugByShovel[Type] = true;

            DustType = 38;
            ItemDrop = ModContent.ItemType<Items.Placeables.VernalSoil>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Vernal Soil");
            AddMapEntry(new Color(80, 120, 0), name);
            HitSound = SoundID.Dig;
        }

        public override void RandomUpdate(int i, int j)
        {
            int j2 = j - 1;
            if (j2 < 10)
                j2 = 10;

            if (Main.tile[i, j2].LiquidAmount == 0)
            {
                if (WorldGen.genRand.NextBool(15))
                {
                    bool placeBulb = true;
                    int minDistanceFromOtherBulbs = 5;
                    for (int k = i - minDistanceFromOtherBulbs; k < i + minDistanceFromOtherBulbs; k += 2)
                    {
                        for (int l = j - minDistanceFromOtherBulbs; l < j + minDistanceFromOtherBulbs; l += 2)
                        {
                            if (k > 1 && k < Main.maxTilesX - 2 && l > 1 && l < Main.maxTilesY - 2 && Main.tile[k, l].HasTile && Main.tile[k, l].TileType == TileID.PlanteraBulb)
                            {
                                placeBulb = false;
                                break;
                            }
                        }
                    }

                    /*if (placeBulb)
                    {
                        WorldGen.Place2x2(i, j2, TileID.PlanteraBulb, 0);
                        WorldGen.SquareTileFrame(i, j2);
                        WorldGen.SquareTileFrame(i + 2, j2);
                        WorldGen.SquareTileFrame(i - 1, j2);
                        if (Main.tile[i, j2].TileType == TileID.PlanteraBulb && Main.netMode == NetmodeID.Server)
                            NetMessage.SendTileSquare(-1, i, j2, 5);
                    }*/
                }
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}

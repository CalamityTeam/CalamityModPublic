using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.AstralDesert
{
    public class AstralPalmSapling : ModTile
    {
        //All of this code is taken directly from Example Mod.
        //Cheers Blushie

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.CommonSapling[Type] = true;
            TileID.Sets.TreeSapling[Type] = true;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorValidTiles = new[] { ModContent.TileType<AstralSand>() };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawFlipHorizontal = true;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.RandomStyleRange = 3;
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Sapling");
            AddMapEntry(new Color(200, 200, 200), name);
            DustType = ModContent.DustType<AstralBasic>();
            AdjTiles = new int[] { TileID.Saplings };
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public static bool EmptyTileCheck(int startX, int endX, int startY, int endY, int ignoreID = -1)
        {
            //Checks if inworld
            if (startX < 0)
                return false;
            if (endX >= Main.maxTilesX)
                return false;
            if (startY < 0)
                return false;
            if (endY >= Main.maxTilesY)
                return false;


            bool flag = false;
            if (ignoreID != -1 && TileID.Sets.CommonSapling[ignoreID])
                flag = true;

            for (int i = startX; i < endX + 1; i++)
            {
                for (int j = startY; j < endY + 1; j++)
                {
                    //Check for empty tiles
                    if (!Main.tile[i, j].HasTile)
                        continue;

                    switch (ignoreID)
                    {
                        case -1:
                            return false;
                        case 11:
                            if (Main.tile[i, j].TileType == 11)
                                continue;
                            return false;
                        case 71:
                            if (Main.tile[i, j].TileType == 71)
                                continue;
                            return false;
                    }

                    if (flag)
                    {
                        if (TileID.Sets.CommonSapling[Main.tile[i, j].TileType])
                            break;

                        /*
						switch (Main.tile[i, j].type) {
							case 3:
							case 24:
							case 32:
							case 61:
							case 62:
							case 69:
							case 71:
							case 73:
							case 74:
							case 82:
							case 83:
							case 84:
							case 110:
							case 113:
							case 201:
							case 233:
							case 352:
							case 485:
							case 529:
							case 530:
								continue;
						}
						*/

                        if (TileID.Sets.IgnoredByGrowingSaplings[Main.tile[i, j].TileType])
                            continue;

                        return false;
                    }
                }
            }

            return true;
        }

        public override void RandomUpdate(int i, int j)
        {
            if (WorldGen.genRand.Next(20) == 0 || true)
            {
                bool isPlayerNear = WorldGen.PlayerLOS(i, j);

                //This is debug code ripped from tml source. I'll remove it when they fix modpalmtrees

                int num = j;

                while (TileID.Sets.TreeSapling[Main.tile[i, num].TileType])
                {
                    num++;
                    if (Main.tile[i, num] == null)
                        return;
                }

                Tile tile = Main.tile[i, num];

                if (tile.TileType != 53 && tile.TileType != 234 && tile.TileType != 116 && tile.TileType != 112 && !TileLoader.CanGrowModPalmTree(tile.TileType))
                    return;

                //Check if the very base of the palm tree is occupied (the 2 tiles above the ground should be sapling tiles)
                if (!EmptyTileCheck(i, i, num - 2, num - 1, 20))
                    return;

                //Check if theres clear space on the upper portion of the palm tree.
                if (!EmptyTileCheck(i - 1, i + 1, num - 30, num - 3, 20))
                    return;

                bool success = WorldGen.GrowPalmTree(i, j);

                if (success && isPlayerNear)
                {
                    WorldGen.TreeGrowFXCheck(i, j);
                }
            }
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects effects)
        {
            if (i % 2 == 1)
            {
                effects = SpriteEffects.FlipHorizontally;
            }
        }
    }
}

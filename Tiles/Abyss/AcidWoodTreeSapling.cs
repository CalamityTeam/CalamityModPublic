using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Abyss
{
    public class AcidWoodTreeSapling : ModTile
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
			TileID.Sets.SwaysInWindBasic[Type] = true;
			TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorValidTiles = new[] { ModContent.TileType<SulphurousSand>(), ModContent.TileType<HardenedSulphurousSandstone>(), ModContent.TileType<SulphurousSandstone>() };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawFlipHorizontal = true;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.RandomStyleRange = 3;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(113, 90, 71), Language.GetText("MapObject.Sapling"));
            DustType = (int)CalamityDusts.SulfurousSeaAcid;
            AdjTiles = new int[] { TileID.Saplings };
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void RandomUpdate(int i, int j)
        {
            if (WorldGen.genRand.NextBool(20))
            {
                int trueStartingPositionY = j;
                while (TileID.Sets.TreeSapling[Main.tile[i, trueStartingPositionY].TileType])
                {
                    trueStartingPositionY++;
                }
                Tile tileAtPosition = Main.tile[i, trueStartingPositionY];
                Tile tileAbovePosition = Main.tile[i, trueStartingPositionY - 1];
                if (!tileAtPosition.HasTile || tileAtPosition.IsHalfBlock || tileAtPosition.Slope != 0)
                {
                    return;
                }
                if (tileAbovePosition.WallType != 0 || tileAbovePosition.LiquidAmount != 0)
                {
                    return;
                }
                if (!WorldGen.EmptyTileCheck(i - 1, i + 1, trueStartingPositionY - 30, trueStartingPositionY - 1, 20))
                {
                    return;
                }
                int treeHeight = WorldGen.genRand.Next(10, 21);
                int frameYIdeal = WorldGen.genRand.Next(-8, 9);
                frameYIdeal *= 2;
                short frameY = 0;
                for (int k = 0; k < treeHeight; k++)
                {
                    tileAtPosition = Main.tile[i, trueStartingPositionY - 1 - k];
                    if (k == 0)
                    {
                        tileAtPosition.Get<TileWallWireStateData>().HasTile = true;
                        tileAtPosition.TileType = TileID.PalmTree;
                        tileAtPosition.TileFrameX = 66;
                        tileAtPosition.TileFrameY = 0;
                    }
                    else if (k == treeHeight - 1)
                    {
                        tileAtPosition.Get<TileWallWireStateData>().HasTile = true;
                        tileAtPosition.TileType = TileID.PalmTree;
                        tileAtPosition.TileFrameX = (short)(22 * WorldGen.genRand.Next(4, 7));
                        tileAtPosition.TileFrameY = frameY;
                    }
                    else
                    {
                        if (frameY != frameYIdeal)
                        {
                            float heightRatio = k / (float)treeHeight;
                            bool increaseFrameY = heightRatio >= 0.25f && ((heightRatio < 0.5f && WorldGen.genRand.Next(13) == 0) || (heightRatio < 0.7f && WorldGen.genRand.NextBool(9)) || heightRatio >= 0.95f || WorldGen.genRand.Next(5) != 0 || true);
                            if (increaseFrameY)
                                frameY += (short)(Math.Sign(frameYIdeal) * 2);
                        }
                        tileAtPosition.Get<TileWallWireStateData>().HasTile = true;
                        tileAtPosition.TileType = TileID.PalmTree;
                        tileAtPosition.TileFrameX = (short)(22 * WorldGen.genRand.Next(0, 3));
                        tileAtPosition.TileFrameY = frameY;
                    }
                }

                bool isPlayerNear = WorldGen.PlayerLOS(i, j);
                WorldGen.RangeFrame(i - 2, trueStartingPositionY - treeHeight - 1, i + 2, trueStartingPositionY + 1);
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendTileSquare(-1, i, (int)((double)trueStartingPositionY - (double)treeHeight * 0.5), treeHeight + 1, TileChangeType.None);
                if (isPlayerNear)
                    WorldGen.TreeGrowFXCheck(i, j);
            }
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects effects)
        {
            if (i % 2 == 1)
                effects = SpriteEffects.FlipHorizontally;
        }
    }
}

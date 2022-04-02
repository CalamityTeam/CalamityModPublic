using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Abyss
{
    public class AcidWoodTreeSapling : ModTile
    {
        //All of this code is taken directly from Example Mod.
        //Cheers Blushie

        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorValidTiles = new[] { ModContent.TileType<SulphurousSand>(), ModContent.TileType<SulphurousSandNoWater>(), ModContent.TileType<HardenedSulphurousSandstone>(), ModContent.TileType<SulphurousSandstone>() };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawFlipHorizontal = true;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.RandomStyleRange = 3;
            TileObjectData.addTile(Type);
            sapling = true;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Sapling");
            AddMapEntry(new Color(113, 90, 71), name);
            dustType = (int)CalamityDusts.SulfurousSeaAcid;
            adjTiles = new int[] { TileID.Saplings };
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void RandomUpdate(int i, int j)
        {
            if (WorldGen.genRand.Next(20) == 0)
            {
                int trueStartingPositionY = j;
                while (TileLoader.IsSapling((int)Main.tile[i, trueStartingPositionY].type))
                {
                    trueStartingPositionY++;
                }
                Tile tileAtPosition = Main.tile[i, trueStartingPositionY];
                Tile tileAbovePosition = Main.tile[i, trueStartingPositionY - 1];
                if (!tileAtPosition.active() || tileAtPosition.halfBrick() || tileAtPosition.slope() != 0)
                {
                    return;
                }
                if (tileAbovePosition.wall != 0 || tileAbovePosition.liquid != 0)
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
                        tileAtPosition.active(true);
                        tileAtPosition.type = TileID.PalmTree;
                        tileAtPosition.frameX = 66;
                        tileAtPosition.frameY = 0;
                    }
                    else if (k == treeHeight - 1)
                    {
                        tileAtPosition.active(true);
                        tileAtPosition.type = TileID.PalmTree;
                        tileAtPosition.frameX = (short)(22 * WorldGen.genRand.Next(4, 7));
                        tileAtPosition.frameY = frameY;
                    }
                    else
                    {
                        if (frameY != frameYIdeal)
                        {
                            float heightRatio = k / (float)treeHeight;
                            bool increaseFrameY = heightRatio >= 0.25f && ((heightRatio < 0.5f && WorldGen.genRand.Next(13) == 0) || (heightRatio < 0.7f && WorldGen.genRand.Next(9) == 0) || heightRatio >= 0.95f || WorldGen.genRand.Next(5) != 0 || true);
                            if (increaseFrameY)
                            {
                                frameY += (short)(Math.Sign(frameYIdeal) * 2);
                            }
                        }
                        tileAtPosition.active(true);
                        tileAtPosition.type = TileID.PalmTree;
                        tileAtPosition.frameX = (short)(22 * WorldGen.genRand.Next(0, 3));
                        tileAtPosition.frameY = frameY;
                    }
                }
                bool isPlayerNear = WorldGen.PlayerLOS(i, j);
                WorldGen.RangeFrame(i - 2, trueStartingPositionY - treeHeight - 1, i + 2, trueStartingPositionY + 1);
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendTileSquare(-1, i, (int)((double)trueStartingPositionY - (double)treeHeight * 0.5), treeHeight + 1, TileChangeType.None);
                }
                if (isPlayerNear)
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

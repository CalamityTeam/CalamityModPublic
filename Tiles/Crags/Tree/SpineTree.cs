using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Crags.Tree
{
    internal class SpineTree : ModTile
    {
        /*
        What each frame of the tree means
        (refer to the sprite sheet for the colors)
        X frame 0 (orange) = bottom segment
        X frame 18 (dark purple) = main tree segment, each of the three y-frame variants draws a different tree segment variant
        X frame 36 (light purple) = top of the tree
        X frame 54 (teal) = small rib branch segments
        X frame 72 (lime) = medium rib branch segments
        X frame 90 (yellow) = large rib branch segments
        
        each of the branch frames have a pattern where
        y-frame 1 = left branch
        y-frame 2 = right branch
        y-frame 3 = both left and right branch
        */

        public Asset<Texture2D> BottomTexture;
        public Asset<Texture2D> SegmentsTexture;

        public Asset<Texture2D> Rib1LeftTexture;
        public Asset<Texture2D> Rib1RightTexture;

        public Asset<Texture2D> Rib2LeftTexture;
        public Asset<Texture2D> Rib2RightTexture;

        public Asset<Texture2D> Rib3LeftTexture;
        public Asset<Texture2D> Rib3RightTexture;

        public Asset<Texture2D> TopTexture;

        public override void SetStaticDefaults()
        {
            TileID.Sets.IsATreeTrunk[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileAxe[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = false;
            Main.tileBlockLight[Type] = false;
            AddMapEntry(new Color(38, 25, 27), CreateMapEntryName());
            DustType = DustID.Ambient_DarkBrown;
            HitSound = SoundID.DD2_SkeletonHurt;
            RegisterItemDrop(ModContent.ItemType<Items.Placeables.Crags.ScorchedBone>());
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            resetFrame = false;
            noBreak = true;
            return false;
        }

        public static bool SolidTile(int i, int j)
        {
            return Framing.GetTileSafely(i, j).HasTile && Main.tileSolid[Framing.GetTileSafely(i, j).TileType];
        }

        public static bool SolidTopTile(int i, int j)
        {
            return Framing.GetTileSafely(i, j).HasTile && (Main.tileSolidTop[Framing.GetTileSafely(i, j).TileType] ||
            Main.tileSolid[Framing.GetTileSafely(i, j).TileType]);
        }

        public static bool Spawn(int i, int j, int minSize = 5, int maxSize = 18, bool saplingExists = false)
        {
            //set the minimum and maximum height the tree can grow to
            int height = Main.rand.Next(minSize, maxSize);
            for (int k = 1; k < height; ++k)
            {
                if (SolidTile(i, j - k))
                {
                    height = k - 2;
                    break;
                }
            }

            if (height < minSize)
            {
                return false;
            }

            // something is blocking from growing the tree...
            var sapplingType = ModContent.TileType<SpineSapling>();
            if (!WorldGen.EmptyTileCheck(i - 2, i + 2, j - height, j, sapplingType))
            {
                return false;
            }

            //if this tree grew from a sapling, then kill the sapling its growing from
            if (saplingExists)
            {
                WorldGen.KillTile(i, j, false, false, true);
                WorldGen.KillTile(i, j - 1, false, false, true);
            }

            //make sure the block is valid for the tree to place on
            if ((SolidTopTile(i, j + 1) || SolidTile(i, j + 1)) && !Framing.GetTileSafely(i, j).HasTile)
            {
                WorldGen.PlaceTile(i, j, ModContent.TileType<SpineTree>(), true);
                Framing.GetTileSafely(i, j).TileFrameY = (short)(WorldGen.genRand.Next(3) * 18);
            }
            //otherwise dont allow the tree to grow
            else
            {
                return false;
            }

            int branchSegmentDelay = 0;

            for (int k = 1; k < height; k++)
            {
                WorldGen.PlaceTile(i, j - k, ModContent.TileType<SpineTree>(), true);

                if (branchSegmentDelay > 0)
                {
                    branchSegmentDelay--;
                }

                //chance to place a branch segment
                //also dont place branches below a certain threshold
                if (Main.rand.NextBool() && branchSegmentDelay == 0 && k > 5)
                {
                    if (k > 1 && k < 10)
                    {
                        Framing.GetTileSafely(i, j - k).TileFrameX = 3 * 18;
                    }
                    if (k >= 10 && k < 17)
                    {
                        Framing.GetTileSafely(i, j - k).TileFrameX = 4 * 18;
                    }
                    if (k >= 17)
                    {
                        Framing.GetTileSafely(i, j - k).TileFrameX = 5 * 18;
                    }

                    Framing.GetTileSafely(i, j - k).TileFrameY = (short)(Main.rand.Next(3) * 18);

                    branchSegmentDelay = 3;
                }
                //otherwise place a normal segment
                else
                {
                    Framing.GetTileSafely(i, j - k).TileFrameX = 18;
                    Framing.GetTileSafely(i, j - k).TileFrameY = (short)(Main.rand.Next(3) * 18);
                }

                //once the top is reached, place the top segment
                if (k == height - 1)
                {
                    Framing.GetTileSafely(i, j - k).TileFrameX = 2 * 18;
                    Framing.GetTileSafely(i, j - k).TileFrameY = (short)(Main.rand.Next(3) * 18);
                }
            }

            return true;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            // Kill the tree tiles if there are no tiles below it or if a Sapling is below it
            // The Sapling check exists for Axe of Regrowth sapling replanting
            Tile belowTile = Framing.GetTileSafely(i, j + 1);
            if (!belowTile.HasTile || belowTile.TileType == ModContent.TileType<SpineSapling>())
            {
                WorldGen.KillTile(i, j);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j);
            }
        }


        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            //drop seeds from the top of the tree
            if (Framing.GetTileSafely(i, j).TileFrameX == 36)
            {
                int totalSeeds = Main.rand.Next(1, 3);
                for (int numSeed = 0; numSeed < totalSeeds; numSeed++)
                {
                    yield return new Item(ModContent.ItemType<Items.Placeables.Crags.SpineSapling>());
                }
            }

            //chance to drop extra wood
            if (Main.rand.NextBool())
            {
                yield return new Item(ModContent.ItemType<Items.Placeables.Crags.ScorchedBone>());
            }
        }

        //this checks the entire tree from bottom to top
        /*private void CheckEntireTree(ref int x, ref int y)
        {
            while (Main.tile[x, y].TileType == Type)
            {
                y--;
            }

            y++;

            if (Main.tile[x, y].TileFrameX == 16)
            {
                for (int k = 0; k < WorldGen.numTreeShakes; k++)
                {
                    if (WorldGen.treeShakeX[k] == x && WorldGen.treeShakeY[k] == y)
                    {
                        return;
                    }
                }

                WorldGen.treeShakeX[WorldGen.numTreeShakes] = x;
                WorldGen.treeShakeY[WorldGen.numTreeShakes] = y;
                WorldGen.numTreeShakes++;

                //this is where you can make stuff happen when the tree is shaken like in vanilla
                //for example, when trees drop a fruit when you begin to break it
            }
        }*/

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            /*if (fail && !effectOnly && !noItem)
            {
                (int x, int y) = (i, j);
                CheckEntireTree(ref x, ref y);
            }*/

            if (fail)
            {
                return;
            }

            int belowFrame = Framing.GetTileSafely(i, j + 1).TileFrameX;

            SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt, (new Vector2(i, j) * 16));

            //here is where you can make effects happen when the tree segments break, like adding dust, dropping gore, ect

            /*
            //TODO: save this since this tree will also produce bone gores when broken (gores for this tree still need assets)
            Gore.NewGore(new EntitySource_TileInteraction(Main.LocalPlayer, i, j), (new Vector2(i, j - 2) * 16),
            new Vector2(Main.rand.Next(-3, 3), Main.rand.Next(-3, 3)), ModContent.Find<ModGore>("CalamityMod/WhateverGore").Type);
            */

            //TODO: spawn all the gores here

            //bottom segment
            if (Framing.GetTileSafely(i, j).TileFrameX == 0)
            {
            }

            //regular segment
            if (Framing.GetTileSafely(i, j).TileFrameX == 18)
            {
            }

            //small branch
            if (Framing.GetTileSafely(i, j).TileFrameX == 54)
            {
            }

            //medium branch
            if (Framing.GetTileSafely(i, j).TileFrameX == 72)
            {
            }

            //large branch
            if (Framing.GetTileSafely(i, j).TileFrameX == 90)
            {
            }

            //top segment
            if (Framing.GetTileSafely(i, j).TileFrameX == 36)
            {
            }
        }

        public static Vector2 TileOffset => Lighting.LegacyEngine.Mode > 1 && Main.GameZoomTarget == 1 ? Vector2.Zero : Vector2.One * 12;

        public static Vector2 TileCustomPosition(int i, int j, Vector2? off = null)
        {
            return ((new Vector2(i, j) + TileOffset) * 16) - Main.screenPosition - (off ?? new Vector2(0, -2));
        }

        internal static void DrawTreeSegments(int i, int j, Texture2D tex, Rectangle? source, Vector2? offset = null, Vector2? origin = null, bool Glow = false)
        {
            Vector2 drawPos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + (offset ?? new Vector2(0, -2));
            Color color = Lighting.GetColor(i, j);

            Main.spriteBatch.Draw(tex, drawPos, source, Glow ? Color.White : color, 0, origin ?? source.Value.Size() / 3f, 1f, SpriteEffects.None, 0f);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Main.tile[i, j].IsTileActuallyInvisible())
                return false;

            Tile tile = Framing.GetTileSafely(i, j);
            float xOff = (float)Math.Sin((j * 19) * 0.04f) * 1.2f;

            if (xOff == 1 && (j / 4f) == 0)
            {
                xOff = 0;
            }

            int frameOff = 0;

            Vector2 baseSegmentOffset = new Vector2((xOff * 2) - (frameOff / 2) + 26, 14);
            Vector2 treeSegmentOffset = new Vector2((xOff * 2) - (frameOff / 2) + 25, 14);
            Vector2 topSegmentOffset = new Vector2((xOff * 2) - (frameOff / 2) + 20, 16);

            BottomTexture ??= ModContent.Request<Texture2D>("CalamityMod/Tiles/Crags/Tree/SpineBottom");
            SegmentsTexture ??= ModContent.Request<Texture2D>("CalamityMod/Tiles/Crags/Tree/SpineSegments");
            var segmentTex = SegmentsTexture.Value;

            //draw the base of the tree
            if (Framing.GetTileSafely(i, j).TileFrameX == 0)
            {
                int frame = tile.TileFrameY / 18;

                DrawTreeSegments(i, j, BottomTexture.Value, new Rectangle(34 * frame, 0, 32, 20), TileOffset.ToWorldCoordinates(), baseSegmentOffset, false);
            }

            //draw the different segments of the tree
            if (Framing.GetTileSafely(i, j).TileFrameX == 18)
            {
                int frame = tile.TileFrameY / 18;

                DrawTreeSegments(i, j, segmentTex, new Rectangle(34 * frame, 0, 32, 20), TileOffset.ToWorldCoordinates(), treeSegmentOffset, false);
            }

            //draw small branch segments
            if (Framing.GetTileSafely(i, j).TileFrameX == 54)
            {
                int frame = tile.TileFrameY / 18;

                //branch drawing stuff
                Rib1LeftTexture ??= ModContent.Request<Texture2D>("CalamityMod/Tiles/Crags/Tree/SpineRib1Left");
                Rib1RightTexture ??= ModContent.Request<Texture2D>("CalamityMod/Tiles/Crags/Tree/SpineRib1Right");

                Vector2 leftBranchOffset = new Vector2((xOff * 2) - (frameOff / 2) + 50, 14);
                Vector2 rightBranchOffset = new Vector2((xOff * 2) - (frameOff / 2) + 4, 14);

                //ok so, terraria just does not want to cooperate when i draw a single branch texture on the side of the tree segments
                //i changed them into a horitonzal sheet with blanks in it so it gives the illusion of branches being random
                //not a good idea if branches are meant to be separate tiles, but here they are purely visual so it doesnt matter
                DrawTreeSegments(i, j, Rib1LeftTexture.Value, new Rectangle(38 * frame, 0, 38, 40), TileOffset.ToWorldCoordinates(), leftBranchOffset, false);
                DrawTreeSegments(i, j, Rib1RightTexture.Value, new Rectangle(38 * frame, 0, 38, 40), TileOffset.ToWorldCoordinates(), rightBranchOffset, false);

                //draw segments
                DrawTreeSegments(i, j, segmentTex, new Rectangle(34 * frame, 0, 32, 20), TileOffset.ToWorldCoordinates(), treeSegmentOffset, false);
            }

            //draw medium branch segments
            if (Framing.GetTileSafely(i, j).TileFrameX == 72)
            {
                int frame = tile.TileFrameY / 18;

                //branch drawing stuff
                Rib2LeftTexture ??= ModContent.Request<Texture2D>("CalamityMod/Tiles/Crags/Tree/SpineRib2Left");
                Rib2RightTexture ??= ModContent.Request<Texture2D>("CalamityMod/Tiles/Crags/Tree/SpineRib2Right");

                Vector2 leftBranchOffset = new Vector2((xOff * 2) - (frameOff / 2) + 60, 14);
                Vector2 rightBranchOffset = new Vector2((xOff * 2) - (frameOff / 2) + 4, 14);

                DrawTreeSegments(i, j, Rib2LeftTexture.Value, new Rectangle(50 * frame, 0, 50, 54), TileOffset.ToWorldCoordinates(), leftBranchOffset, false);
                DrawTreeSegments(i, j, Rib2RightTexture.Value, new Rectangle(50 * frame, 0, 50, 54), TileOffset.ToWorldCoordinates(), rightBranchOffset, false);

                //draw segments
                DrawTreeSegments(i, j, segmentTex, new Rectangle(34 * frame, 0, 32, 20), TileOffset.ToWorldCoordinates(), treeSegmentOffset, false);
            }

            //draw large branch segments
            if (Framing.GetTileSafely(i, j).TileFrameX == 90)
            {
                int frame = tile.TileFrameY / 18;

                //branch drawing stuff
                Rib3LeftTexture ??= ModContent.Request<Texture2D>("CalamityMod/Tiles/Crags/Tree/SpineRib3Left");
                Rib3RightTexture ??= ModContent.Request<Texture2D>("CalamityMod/Tiles/Crags/Tree/SpineRib3Right");

                Vector2 leftBranchOffset = new Vector2((xOff * 2) - (frameOff / 2) + 75, 14);
                Vector2 rightBranchOffset = new Vector2((xOff * 2) - (frameOff / 2) + 4, 14);

                DrawTreeSegments(i, j, Rib3LeftTexture.Value, new Rectangle(62 * frame, 0, 62, 60), TileOffset.ToWorldCoordinates(), leftBranchOffset, false);
                DrawTreeSegments(i, j, Rib3RightTexture.Value, new Rectangle(62 * frame, 0, 62, 60), TileOffset.ToWorldCoordinates(), rightBranchOffset, false);

                //draw segments
                DrawTreeSegments(i, j, segmentTex, new Rectangle(34 * frame, 0, 32, 20), TileOffset.ToWorldCoordinates(), treeSegmentOffset, false);
            }

            //draw top segment
            if (Framing.GetTileSafely(i, j).TileFrameX == 36)
            {
                TopTexture ??= ModContent.Request<Texture2D>("CalamityMod/Tiles/Crags/Tree/SpineTop");
                int frame = tile.TileFrameY / 18;

                DrawTreeSegments(i, j - 1, TopTexture.Value, new Rectangle(26 * frame, 0, 24, 24), TileOffset.ToWorldCoordinates(), topSegmentOffset, false);

                //draw segments
                DrawTreeSegments(i, j, segmentTex, new Rectangle(34 * frame, 0, 32, 20), TileOffset.ToWorldCoordinates(), treeSegmentOffset, false);
            }

            /*
            //keep this here for debugging
            Texture2D treeTex = ModContent.Request<Texture2D>("CalamityMod/Tiles/Crags/Tree/SpineTree").Value;

            spriteBatch.Draw(treeTex, pos, new Rectangle(tile.TileFrameX + frameOff, tile.TileFrameY, frameSize, frameSizeY), 
            new Color(col.R, col.G, col.B, 255), 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            */

            return false;
        }
    }
}

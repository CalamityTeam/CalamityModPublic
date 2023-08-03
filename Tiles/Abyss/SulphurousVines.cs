using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Abyss
{
    public class SulphurousVines : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileNoSunLight[Type] = false;
            AddMapEntry(new Color(0, 50, 0));
            HitSound = SoundID.Grass;
            DustType = 2;
			TileID.Sets.IsVine[Type] = true;
			TileID.Sets.ReplaceTileBreakDown[Type] = true;
			TileID.Sets.VineThreads[Type] = true;
			TileID.Sets.DrawFlipMode[Type] = 1;
			TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			Tile tileAbove = Framing.GetTileSafely(i, j - 1);

			if (!tileAbove.HasTile)
            {
                WorldGen.KillTile(i, j);
				return true;
			}

			return true;
		}

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            // GIVE VINE ROPE IF SPECIAL VINE BOOK
            if (WorldGen.genRand.NextBool(2) && Main.player[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].cordage)
                Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i * 16 + 8f, j * 16 + 8f), ItemID.VineRope);

            if (Main.tile[i, j + 1] != null)
            {
                if (Main.tile[i, j + 1].HasTile)
                {
                    if (Main.tile[i, j + 1].TileType == ModContent.TileType<SulphurousVines>())
                    {
                        WorldGen.KillTile(i, j + 1, false, false, false);
                        if (!Main.tile[i, j + 1].HasTile && Main.netMode != NetmodeID.SinglePlayer)
                            NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, (float)i, (float)j + 1, 0f, 0, 0, 0);
                    }
                }
            }
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer && Main.rand.NextBool(200) && j > Main.worldSurface)
            {
                Dust dust;
                dust = Main.dust[Dust.NewDust(new Vector2(i * 16f, j * 16f), 274, 279, 304, 0.23255825f, 10f, 0, new Color(22, 255, 0), 1.5116279f)];
                dust.noGravity = true;
                dust.noLight = true;
                dust.fadeIn = 2.5813954f;
            }
        }

        // Ozzatron 01JUL2022: heavily refactored this code to not suck. also, sulphurous vines won't grow in honey anymore.
        private const int MaxVineHeight = 10;

        public override void RandomUpdate(int i, int j)
        {
            Tile below = Main.tile[i, j + 1];
            if (!below.HasTile && below.LiquidType == LiquidID.Water && below.LiquidAmount >= 128)
            {
                bool growVine = false;
                for (int vineOriginYPos = j; vineOriginYPos > j - MaxVineHeight; vineOriginYPos--)
                {
                    Tile consideredVineOrigin = Main.tile[i, vineOriginYPos];
                    // Vines won't grow if they are coming out of a bottom-sloped block (which they shouldn't be able to anyway)
                    if (consideredVineOrigin.BottomSlope)
                    {
                        growVine = false;
                        break;
                    }
                    // Vines can continue to grow unimpeded out of any solid block that isn't bottom-sloped.
                    if (Main.tile[i, vineOriginYPos].HasTile && !Main.tile[i, vineOriginYPos].BottomSlope && Main.tileSolid[Main.tile[i, vineOriginYPos].TileType])
                    {
                        growVine = true;
                        break;
                    }
                }

                if (growVine)
                {
                    int x = i;
                    int y = j + 1;

                    // Zero faith that code using local variables works with struct tiles, from experience.

                    // Spawn the new vine.
                    Main.tile[x, y].TileType = (ushort)ModContent.TileType<SulphurousVines>();
                    Main.tile[x, y].TileFrameX = (short)(WorldGen.genRand.Next(8) * 18);
                    Main.tile[x, y].TileFrameY = 4 * 18;
                    Main.tile[x, y].Get<TileWallWireStateData>().HasTile = true; // .HasTile = true; refuses to work.

                    // Pick a new sprite for the current vine.
                    Main.tile[i, j].TileFrameX = (short)(WorldGen.genRand.Next(12) * 18);
                    Main.tile[i, j].TileFrameY = (short)(WorldGen.genRand.Next(4) * 18);

                    // Reframe both vines the correct vanilla way.
                    WorldGen.SquareTileFrame(x, y, true);
                    WorldGen.SquareTileFrame(i, j, true);

                    // Send update packets as needed.
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendTileSquare(-1, x, y, 3, TileChangeType.None);
                }
            }
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            float brightness = 0.7f;
            brightness *= (float)MathF.Sin(-j / 40f + Main.GameUpdateCount * 0.01f + i);
            brightness += 0.5f;
            r = 0.68f;
            g = 1f;
            b = 0.78f;
            r *= brightness;
            g *= brightness;
            b *= brightness;
        }
    }
}

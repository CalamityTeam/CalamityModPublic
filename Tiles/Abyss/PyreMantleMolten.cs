using CalamityMod.Tiles.Abyss.AbyssAmbient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Abyss
{
    public class PyreMantleMolten : ModTile
    {
        public static readonly SoundStyle MineSound = new("CalamityMod/Sounds/Custom/VoidstoneMine", 3) { Volume = 0.4f };
        internal static Texture2D GlowTexture;

        public byte[,] tileAdjacency;
        public byte[,] secondTileAdjacency;
        public byte[,] thirdTileAdjacency;
        public byte[,] fourthTileAdjacency;

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                GlowTexture = ModContent.Request<Texture2D>("CalamityMod/Tiles/Abyss/PyreMantleMolten_Glowmask", AssetRequestMode.ImmediateLoad).Value;

            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithAbyss(Type);

            TileID.Sets.ChecksForMerge[Type] = true;
            HitSound = MineSound;
            MineResist = 10f;
            MinPick = 180;
            AddMapEntry(new Color(113, 49, 16));

            TileFraming.SetUpUniversalMerge(Type, TileID.Dirt, out tileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.Stone, out secondTileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, ModContent.TileType<AbyssGravel>(), out thirdTileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, ModContent.TileType<PyreMantle>(), out fourthTileAdjacency);
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 162, 0f, 0f, 1, new Color(128, 128, 128), 1f);
            return false;
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void RandomUpdate(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            Tile up = Main.tile[i, j - 1];
            Tile up2 = Main.tile[i, j - 2];

            // Place PhoviamareHalm
            if (WorldGen.genRand.NextBool(12) && !up.HasTile && !up2.HasTile && up.LiquidAmount > 0 && up2.LiquidAmount > 0 && !tile.LeftSlope && !tile.RightSlope && !tile.IsHalfBlock)
            {
                up.TileType = (ushort)ModContent.TileType<PhoviamareHalm>();
                up.HasTile = true;
                up.TileFrameY = 0;

                // 16 different frames, choose a random one
                up.TileFrameX = (short)(WorldGen.genRand.Next(16) * 18);
                WorldGen.SquareTileFrame(i, j - 1, true);

                if (Main.netMode == NetmodeID.Server) 
                    NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (GlowTexture is null)
                return;

            int xPos = Main.tile[i, j].TileFrameX;
            int yPos = Main.tile[i, j].TileFrameY;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + zero;
            Color drawColour = GetDrawColour(i, j, new Color(200, 200, 200, 200));
            Tile trackTile = Main.tile[i, j];
            float glowbrightness = 1f;
            float glowspeed = Main.GameUpdateCount * 0.01f;
            glowbrightness *= (float)MathF.Sin(i / 60f + glowspeed);
            drawColour *= glowbrightness;
            double num6 = Main.time * 0.08;

            TileFraming.SlopedGlowmask(i, j, 0, GlowTexture, drawOffset, null, GetDrawColour(i, j, drawColour), default);
            glowbrightness += 0.3f;

            TileFraming.DrawUniversalMergeFrames(i, j, fourthTileAdjacency, "CalamityMod/Tiles/Merges/PyreMantleMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, thirdTileAdjacency, "CalamityMod/Tiles/Merges/AbyssGravelMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, secondTileAdjacency, "CalamityMod/Tiles/Merges/StoneMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, tileAdjacency, "CalamityMod/Tiles/Merges/DirtMerge");
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            float brightness = 0.7f;
            float lightspeed = Main.GameUpdateCount * 0.01f;
            brightness *= (float)MathF.Sin(i / 60f + lightspeed);
            brightness += 0.3f;
            r = 1f;
            g = 0.33f;
            b = 0f;
            r *= brightness;
            g *= brightness;
            b *= brightness;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.GetAdjacencyData(i, j, TileID.Dirt, out tileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, TileID.Stone, out secondTileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, ModContent.TileType<AbyssGravel>(), out thirdTileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, ModContent.TileType<PyreMantle>(), out fourthTileAdjacency[i, j]);
            return true;
        }

        private Color GetDrawColour(int i, int j, Color colour)
        {
            int colType = Main.tile[i, j].TileColor;
            Color paintCol = WorldGen.paintColor(colType);
            if (colType >= 13 && colType <= 24)
            {
                colour.R = (byte)(paintCol.R / 255f * colour.R);
                colour.G = (byte)(paintCol.G / 255f * colour.G);
                colour.B = (byte)(paintCol.B / 255f * colour.B);
            }
            return colour;
        }
    }
}

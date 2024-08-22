using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class VoidstoneWallUnsafe : ModWall
    {
        internal static FramedGlowMask GlowMask;

        public override void SetStaticDefaults()
        {
            // We basically have same copy in VoidstoneWall
            // But leaving this in case of changing to Unsafe variant specific glowmask
            GlowMask = new("CalamityMod/Walls/VoidstoneWall_Glowmask", 36, 36);

            DustType = 187;
            AddMapEntry(new Color(0, 0, 0));
        }

        public override void RandomUpdate(int i, int j)
        {
            if (Main.tile[i, j].LiquidAmount == 0 && j < Main.maxTilesY - 205)
            {
                Main.tile[i, j].Get<LiquidData>().LiquidType = LiquidID.Water;
                Main.tile[i, j].LiquidAmount = byte.MaxValue;
                WorldGen.SquareTileFrame(i, j);
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.sendWater(i, j);
            }
        }

        public static void DrawWallGlow(int wallType, int i, int j, SpriteBatch spriteBatch)
        {
            if (GlowMask.Texture is null)
                return;

            Tile tile = Main.tile[i, j];
            int xLength = 32;
            int xOff = 0;

            int xPos = tile.WallFrameX + xOff;
            int yPos = tile.WallFrameY;

            Rectangle frame = new Rectangle(xPos, yPos, xLength, 32);
            Color drawcolor;
            drawcolor = WorldGen.paintColor(tile.WallColor);
            drawcolor.A = 255;
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

            if (Main.drawToScreen)
                zero = Vector2.Zero;

            Vector2 pos = new Vector2((i * 16 - (int)Main.screenPosition.X), (j * 16 - (int)Main.screenPosition.Y)) + zero;
            spriteBatch.Draw(TextureAssets.Wall[wallType].Value, pos + new Vector2(-8 + xOff, -8), frame, Lighting.GetColor(i, j, Color.White), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            if (GlowMask.HasContentInFramePos(xPos, yPos))
            {
                float brightness = 1f;
                float declareThisHereToPreventRunningTheSameCalculationMultipleTimes = Main.GameUpdateCount * 0.007f;
                brightness *= (float)MathF.Sin(i / 18f + declareThisHereToPreventRunningTheSameCalculationMultipleTimes);
                brightness *= (float)MathF.Sin(j / 18f + declareThisHereToPreventRunningTheSameCalculationMultipleTimes);
                brightness *= (float)MathF.Sin(i * 18f + declareThisHereToPreventRunningTheSameCalculationMultipleTimes);
                brightness *= (float)MathF.Sin(j * 18f + declareThisHereToPreventRunningTheSameCalculationMultipleTimes);
                drawcolor *= brightness;
                Color glowColor = drawcolor * 0.4f;

                // For now checking for glowing frames greatly reducing the bottleneck
                // But maybe we could squeeze bit more by removing the loop
                for (int k = 0; k < 3; k++)
                {
                    Vector2 offset = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)) * 0.2f * k;
                    spriteBatch.Draw(GlowMask.Texture, pos + offset + new Vector2(-8 + xOff, -8), frame, glowColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            DrawWallGlow(Type, i, j, spriteBatch);
            return false;
        }

        public override void KillWall(int i, int j, ref bool fail) => fail = true;

        public override bool CanExplode(int i, int j) => false;

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}

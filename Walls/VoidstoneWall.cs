using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;

namespace CalamityMod.Walls
{
    public class VoidstoneWall : ModWall
    {
        internal static Texture2D GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            GlowTexture = ModContent.Request<Texture2D>("CalamityMod/Walls/VoidstoneWall_Glowmask", AssetRequestMode.ImmediateLoad).Value;

            AddMapEntry(new Color(0, 0, 0));
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 180, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public static void DrawWallGlow(int wallType, int i, int j, SpriteBatch spriteBatch)
        {
            if (GlowTexture is null)
                return;

            Tile tile = Main.tile[i, j];
            int xLength = 32;
            int xOff = 0;

            Rectangle frame = new Rectangle(tile.WallFrameX + xOff, tile.WallFrameY, xLength, 32);
            Color drawcolor;
            drawcolor = WorldGen.paintColor(tile.WallColor);
            drawcolor.A = 255;
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

            float brightness = 1f;
            float declareThisHereToPreventRunningTheSameCalculationMultipleTimes = Main.GameUpdateCount * 0.007f;
            brightness *= (float)MathF.Sin(i / 18f + declareThisHereToPreventRunningTheSameCalculationMultipleTimes);
            brightness *= (float)MathF.Sin(j / 18f + declareThisHereToPreventRunningTheSameCalculationMultipleTimes);
            brightness *= (float)MathF.Sin(i * 18f + declareThisHereToPreventRunningTheSameCalculationMultipleTimes);
            brightness *= (float)MathF.Sin(j * 18f + declareThisHereToPreventRunningTheSameCalculationMultipleTimes);
            drawcolor *= brightness;

            if (Main.drawToScreen)
                zero = Vector2.Zero;

            Vector2 pos = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;
            Main.spriteBatch.Draw(TextureAssets.Wall[wallType].Value, pos + new Vector2(-8 + xOff, -8), frame, Lighting.GetColor(i, j, Color.White), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            Color glowColor = drawcolor * 0.4f;
            for (int k = 0; k < 3; k++)
            {
                Vector2 offset = new Vector2(Main.rand.NextFloat(-1, 1f), Main.rand.NextFloat(-1, 1f)) * 0.2f * k;
                Main.spriteBatch.Draw(GlowTexture, pos + offset + new Vector2(-8 + xOff, -8), frame, glowColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            DrawWallGlow(Type, i, j, spriteBatch);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}

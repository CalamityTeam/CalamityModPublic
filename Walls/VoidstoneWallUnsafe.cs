using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class VoidstoneWallUnsafe : ModWall
    {
        internal static Texture2D GlowTexture;

        public override void SetStaticDefaults()
        {
            GlowTexture = ModContent.Request<Texture2D>("CalamityMod/Walls/VoidstoneWall_Glowmask", AssetRequestMode.ImmediateLoad).Value;
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
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.sendWater(i, j);
            }
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

            Vector2 pos = new Vector2((i * 16 - (int)Main.screenPosition.X), (j * 16 - (int)Main.screenPosition.Y)) + zero;
            Main.spriteBatch.Draw(TextureAssets.Wall[wallType].Value, pos + new Vector2(-8 + xOff, -8), frame, Lighting.GetColor(i, j, Color.White), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            Color glowColor = drawcolor * 0.4f;
            for (int k = 0; k < 3; k++)
            {
                Vector2 offset = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)) * 0.2f * k;
                Main.spriteBatch.Draw(GlowTexture, pos + offset + new Vector2(-8 + xOff, -8), frame, glowColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
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

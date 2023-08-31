using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureMonolith
{
    public class MonolithCandelabra : ModTile
    {
        public override void SetStaticDefaults() => this.SetUpCandelabra(ModContent.ItemType<Items.Placeables.FurnitureMonolith.MonolithCandelabra>(), true);

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, ModContent.DustType<AstralBasic>(), 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int xPos = Main.tile[i, j].TileFrameX;
            int yPos = Main.tile[i, j].TileFrameY;
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/Tiles/FurnitureMonolith/MonolithCandelabraGlow").Value;
            Color drawColour = GetDrawColour(i, j, new Color(100, 100, 100, 100));
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + zero;
            Main.spriteBatch.Draw(glowmask, drawOffset, new Rectangle?(new Rectangle(xPos, yPos, 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);

            Texture2D glowmaskBright = ModContent.Request<Texture2D>("CalamityMod/Tiles/FurnitureMonolith/MonolithCandelabraFlame").Value;
            Main.spriteBatch.Draw(glowmaskBright, drawOffset, new Rectangle?(new Rectangle(xPos, yPos, 18, 18)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
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

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j].TileFrameX < 18)
            {
                r = 0.8f;
                g = 0.9f;
                b = 1f;
            }
            else
            {
                r = 0f;
                g = 0f;
                b = 0f;
            }
        }

        public override void HitWire(int i, int j)
        {
            CalamityUtils.LightHitWire(Type, i, j, 2, 2);
        }
    }
}

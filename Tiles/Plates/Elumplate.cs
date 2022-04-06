
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.Plates
{
    public class Elumplate : ModTile
    {
        internal static Texture2D GlowTexture;
        internal static Texture2D PulseTexture;
        internal static Color[] PulseColors;
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                PulseTexture = ModContent.Request<Texture2D>("CalamityMod/Tiles/Plates/ElumplatePulse");
                PulseColors = new Color[PulseTexture.Width];
                PulseTexture.GetData(PulseColors);
                GlowTexture = ModContent.Request<Texture2D>("CalamityMod/Tiles/Plates/ElumplateGlow");
            }
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);

            SoundType = SoundID.Tink;
            MineResist = 1f;
            ItemDrop = ModContent.ItemType<Items.Placeables.Plates.Elumplate>();
            AddMapEntry(new Color(121, 180, 212));
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 247, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(100, 100, 100), 1f);
            return false;
        }

        public override void RandomUpdate(int i, int j)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 247, 0f, 0f, 1, new Color(255, 255, 255), 1f);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            // If the cached textures don't exist for some reason, don't bother using them.
            if (GlowTexture is null || PulseTexture is null)
                return;

            int xPos = Main.tile[i, j].TileFrameX;
            int yPos = Main.tile[i, j].TileFrameY;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + zero;

            // Glowmask 'pulse' effect
            int factor = (int)Main.time % PulseTexture.Width;
            float brightness = PulseColors[factor].R / 255f;
            int drawBrightness = (int)(40 * brightness) + 10;
            Color drawColour = GetDrawColour(i, j, new Color(drawBrightness, drawBrightness, drawBrightness, drawBrightness));

            // If these tiles cause lag, comment out the pulse effect code and uncomment this:
            //Color drawColour = GetDrawColour(i, j, new Color(50, 50, 50, 50));

            Tile trackTile = Main.tile[i, j];
            if (!trackTile.IsHalfBlock && trackTile.Slope == 0)
            {
                Main.spriteBatch.Draw(GlowTexture, drawOffset, new Rectangle?(new Rectangle(xPos, yPos, 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
            else if (trackTile.IsHalfBlock)
            {
                Main.spriteBatch.Draw(GlowTexture, drawOffset + new Vector2(0f, 8f), new Rectangle?(new Rectangle(xPos, yPos, 18, 8)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
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

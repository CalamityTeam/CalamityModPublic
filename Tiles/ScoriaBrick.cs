using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.Metadata;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles
{
    [LegacyName("ChaoticBrick")]
    public class ScoriaBrick : ModTile
    {
        int subsheetHeight = 72;
        int subsheetWidth = 216;
        internal static Texture2D GlowTexture;
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>("CalamityMod/Tiles/ScoriaBrickGlow", AssetRequestMode.ImmediateLoad).Value;
            }
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            DustType = 105;
            ItemDrop = ModContent.ItemType<Items.Placeables.ScoriaBrick>();
            AddMapEntry(new Color(85, 87, 101));
            HitSound = SoundID.Tink;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.04f;
            g = 0.00f;
            b = 0.00f;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            int yPos = j % 2;
            frameYOffset = yPos * subsheetHeight;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CompactFraming(i, j, resetFrame);
            return false;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            // If the cached textures don't exist for some reason, don't bother using them.
            if (GlowTexture is null)
                return;

            Tile tile = CalamityUtils.ParanoidTileRetrieval(i, j);
            int xPos = tile.TileFrameX;
            int frameOffset = j % 2 * 72;
            int yPos = tile.TileFrameY + frameOffset;
            Vector2 drawOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawPosition = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + drawOffset;

            if (!tile.IsHalfBlock && tile.Slope == 0)
            {
                spriteBatch.Draw(GlowTexture, drawPosition, new Rectangle?(new Rectangle(xPos, yPos, 18, 18)), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            else if (tile.IsHalfBlock)
            {
                spriteBatch.Draw(GlowTexture, drawPosition + new Vector2(0f, 8f), new Rectangle?(new Rectangle(xPos, yPos, 18, 8)), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}

using CalamityMod.Items.Placeables.FurnitureExo;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureExo
{
    public class ExoPlatingTile : ModTile
    {
        internal static Texture2D GlowTexture;

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>("CalamityMod/Tiles/FurnitureExo/ExoPlatingTileGlow");
            }

            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeDecorativeTiles(Type);

            MineResist = 3f;
            SoundType = SoundID.Tink;
            ItemDrop = ModContent.ItemType<ExoPlating>();
            AddMapEntry(new Color(52, 67, 78));
            AnimationFrameHeight = 90;
        }

        public override bool CanExplode(int i, int j) => false;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 107, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            int yPos = j % 2;
            frameYOffset = yPos * AnimationFrameHeight;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            // If the cached textures don't exist for some reason, don't bother using them.
            if (GlowTexture is null)
                return;

            Tile tile = CalamityUtils.ParanoidTileRetrieval(i, j);
            int xPos = tile.TileFrameX;
            int frameOffset = j % 2 * AnimationFrameHeight;
            int yPos = tile.TileFrameY + frameOffset;
            Vector2 drawOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawPosition = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + drawOffset;

            if (!tile.IsHalfBlock && tile.slope() == 0)
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

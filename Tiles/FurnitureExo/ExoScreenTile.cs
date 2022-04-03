using CalamityMod.Items.Placeables.FurnitureExo;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.FurnitureExo
{
    public class ExoScreenTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Screen");
            AddMapEntry(new Color(71, 95, 114), name);
            DustType = 8;
            AnimationFrameHeight = 36;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            int frameAmt = 4;
            frameCounter++;
            // Switch screens every 3 seconds
            if (frameCounter >= 180)
            {
                // Switches to a randomized screen
                frame = (frame + Main.rand.Next(1, frameAmt)) % frameAmt;
                frameCounter = 0;
            }
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameYOffset = this.GetAnimationOffset(i, j, 4, 18, 18, 3, 2, animationFrameHeight);
        }

        public override bool CanExplode(int i, int j) => false;

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 16, 32, ModContent.ItemType<ExoScreen>());
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/Tiles/FurnitureExo/ExoScreenGlow");
            Vector2 drawOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            Vector2 drawPosition = new Vector2(i * 16 - (int)Main.screenPosition.X / 2f, j * 16 - (int)Main.screenPosition.Y + yOffset) + drawOffset;
            Color drawColour = Color.White;
            Main.spriteBatch.Draw(glowmask, drawPosition, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), drawColour, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}

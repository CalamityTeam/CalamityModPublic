using System;
using CalamityMod.Items.Placeables.FurnitureSacrilegious;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureSacrilegious
{
    public class SacrilegiousClockTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            // This particular clock emits light
            Main.tileLighted[Type] = true;
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

            this.SetUpClock(ModContent.ItemType<SacrilegiousClock>(), true);
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 60, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 8, 0f, 0f, 1, new Color(100, 100, 100), 1f);
            return false;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 1.2f;
            g = 0.2f;
            b = 0.2f;
        }

        public override bool RightClick(int x, int y) => CalamityUtils.ClockRightClick();

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer)
            {
                Main.SceneMetrics.HasClock = true;
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void MouseOver(int i, int j) => CalamityUtils.MouseOver(i, j, ModContent.ItemType<SacrilegiousClock>());

        // For drawing the floating clock icon
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Tiles/FurnitureSacrilegious/SacrilegiousClockTile_Icon").Value;
            Tile tile = Main.tile[i, j];
            int xPos = tile.TileFrameX;
            int yPos = tile.TileFrameY;

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            float yOffset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi / 5f) * 2f;
            Vector2 correction = new Vector2(16f , -10f);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y + yOffset) + zero + correction;

            Rectangle rect = new Rectangle(xPos, yPos, texture.Width, texture.Height);
            Color color = new Color(100, 100, 100, 0);
            Vector2 origin = rect.Size() / 2f;

            for (int c = 0; c < 5; c++)
            {
                spriteBatch.Draw(texture, drawOffset, rect, color, 0f, origin, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}

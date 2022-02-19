using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.UI
{
    public class ModeIndicatorUI
    {
        public static void Draw(SpriteBatch spriteBatch)
        {
            // The mode indicator should only be displayed when the inventory is open, to prevent obstruction.
            if (!Main.playerInventory)
                return;

            Texture2D indicatorTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/ModeIndicator");
            int indicatorFrame = 0;
            if (CalamityWorld.revenge)
                indicatorFrame = 1;
            if (CalamityWorld.death)
                indicatorFrame = 2;
            if (CalamityWorld.malice)
                indicatorFrame = 3;

            Rectangle mouseRectangle = Utils.CenteredRectangle(Main.MouseScreen, Vector2.One * 2f);
            Rectangle indicatorFrameArea = indicatorTexture.Frame(1, 4, 0, indicatorFrame);
            Vector2 drawCenter = new Vector2(Main.screenWidth - 400f, 72f) + indicatorFrameArea.Size() * 0.5f;

            spriteBatch.Draw(indicatorTexture, drawCenter, indicatorFrameArea, Color.White, 0f, indicatorFrameArea.Size() * 0.5f, 1f, SpriteEffects.None, 0f);

            // Draw active indication text.
            bool renderingText = false;
            if (mouseRectangle.Intersects(Utils.CenteredRectangle(drawCenter, indicatorFrameArea.Size())))
            {
                string modeToDisplay = "Revengeance";
                bool modeIsActive = CalamityWorld.revenge;
                if (CalamityWorld.death)
                {
                    modeToDisplay = "Death";
                    modeIsActive = CalamityWorld.death;
                }
                if (CalamityWorld.malice)
                {
                    modeToDisplay = "Malice";
                    modeIsActive = CalamityWorld.malice;
                }

                Main.instance.MouseText($"{modeToDisplay} Mode is {(modeIsActive ? "active" : "not active")}.");
                renderingText = true;
            }

            if (renderingText)
                Main.instance.MouseTextHackZoom(string.Empty);
        }
    }
}

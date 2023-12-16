using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace CalamityMod.UI.DraedonSummoning
{
    public class CodebreakerUIScroller
    {
        public bool IsBeingDragged
        {
            get;
            set;
        }

        public float PositionYInterpolant
        {
            get;
            set;
        }

        public void Draw(float top, float bottom, float x, float scale, float opacity)
        {
            Color scrollerColor = Color.White;
            Texture2D scrollerTexture = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonSummoning/Scroller").Value;

            // Update the top and bottom position to account for the texture origin.
            top -= scrollerTexture.Height * scale * 0.5f;
            bottom += scrollerTexture.Height * scale * 0.5f;

            // Calculate the scroller position and area.
            Vector2 scrollerPosition = new Vector2(x, MathHelper.Lerp(top, bottom, PositionYInterpolant));
            Rectangle scrollerArea = Utils.CenteredRectangle(scrollerPosition, scrollerTexture.Size() * scale);

            // Calculate the mouse screen area.
            Rectangle mouseArea = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 2, 2);

            // Check if the user is dragging the scroller.
            bool pressingDownOnScroller = Main.mouseLeft && scrollerArea.Intersects(mouseArea);
            if (pressingDownOnScroller)
                IsBeingDragged = true;

            // Check if the user is done dragging the scroller.
            bool releaseDown = Main.mouseLeftRelease;
            if (IsBeingDragged && releaseDown)
                IsBeingDragged = false;

            // Update the Y position interpolant if the scroller is being dragged based on mouse position.
            if (IsBeingDragged)
                PositionYInterpolant = Utils.GetLerpValue(top, bottom, Main.MouseScreen.Y, true);

            // Make the scroller a bit darker when being dragged.
            if (IsBeingDragged)
                scrollerColor = Color.DarkSlateGray;

            // Draw the scroller.
            Vector2 scrollerOrigin = scrollerTexture.Size() * 0.5f;
            Main.spriteBatch.Draw(scrollerTexture, scrollerPosition, null, scrollerColor * opacity, 0f, scrollerOrigin, scale, 0, 0f);
        }

        public void Reset()
        {
            IsBeingDragged = false;
            PositionYInterpolant = 0f;
        }
    }
}

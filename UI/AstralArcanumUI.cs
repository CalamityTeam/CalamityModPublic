using CalamityMod.CalPlayer;
using CalamityMod.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.UI
{
    // TODO -- This can be made into a ModSystem with simple OnModLoad and Unload hooks.
    public static class AstralArcanumUI
    {
        enum CircleStyle
        {
            Normal,
            Selected
        }

        private const int WindowBorder = 50;
        private const int CircleTextureSize = 40; //don't change unless textures change size
        private const int CircleOffset = 36;

        private static Vector2 CenterPoint;
        private static bool Open;

        private static Texture2D CircleTextures;

        private static int LastHovered;

        public static void Load(Mod mod)
        {
            CircleTextures = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/AstralArcanumCircles", AssetRequestMode.ImmediateLoad).Value;
        }

        public static void Unload()
        {
            CircleTextures = null;
        }

        public static void Toggle()
        {
            Open = !Open;

            if (Open)
            {
                CenterPoint = new Vector2(Main.mouseX, Main.mouseY);
                //Clamp center so UI doesn't go off screen
                CenterPoint.X = MathHelper.Clamp(CenterPoint.X, WindowBorder, Main.screenWidth - WindowBorder);
                CenterPoint.Y = MathHelper.Clamp(CenterPoint.Y, WindowBorder, Main.screenHeight - WindowBorder);
            }
            else
            {
                // Closed, do stuff in here if need be.
            }
        }

        public static void UpdateAndDraw(SpriteBatch sb)
        {
            // Don't do anything if not open.
            if (!Open)
                return;

            // Draw center circle
            DrawCircle(sb, CenterPoint, 0, CircleStyle.Normal);

            Vector2 centerToMouse = Main.MouseScreen - CenterPoint;
            float rotation = centerToMouse.ToRotation();

            // Draw the arrow that points towards the mouse.
            sb.Draw(CircleTextures, CenterPoint, new Rectangle(0, CircleTextureSize, 24, 10), Color.White, rotation, new Vector2(12, 5f), 1f, SpriteEffects.None, 0f);

            int current = 0;
            int selectedCircle = -1;
            int offset = CircleOffset * 2;
            int radius = CircleTextureSize / 2;

            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    Vector2 center = new Vector2(
                        CenterPoint.X - CircleOffset + x * offset,
                        CenterPoint.Y - CircleOffset + y * offset);

                    Circle c = new Circle(center, radius);

                    CircleStyle style = CircleStyle.Normal;

                    // If the mouse is in the circle.
                    if (c.Contains(Main.MouseScreen))
                    {
                        style = CircleStyle.Selected;

                        Main.LocalPlayer.mouseInterface = true;

                        selectedCircle = current;

                        // If left clicked.
                        if ((Main.mouseLeft && Main.mouseLeftRelease))
                        {
                            Main.mouseLeftRelease = false;
                            Main.mouseLeft = false;

                            DoTeleportation(current);
                        }
                    }

                    DrawCircle(sb, center, 1 + current, style);

                    current++;
                }
            }

            // Literally so the sound plays.
            if (LastHovered != selectedCircle)
            {
                LastHovered = selectedCircle;
                if (LastHovered != -1)
                    SoundEngine.PlaySound(SoundID.MenuTick);
            }

            // Default to "Select"
            string text = Language.GetTextValue("LegacyMisc.53");

            switch (selectedCircle)
            {
                case 0:
                    text = Language.GetTextValue("Bestiary_Biomes.TheUnderworld");
                    break;
                case 1:
                    text = Language.GetTextValue("Bestiary_Biomes.TheDungeon");
                    break;
                case 2:
                    text = Language.GetTextValue("Bestiary_Biomes.Jungle");
                    break;
                case 3: // Random
                    text = Language.GetTextValue("LegacyMenu.27");
                    break;
                default:
                    break;
            }

            Vector2 size = FontAssets.MouseText.Value.MeasureString(text);
            Utils.DrawBorderStringFourWay(sb, FontAssets.MouseText.Value, text, CenterPoint.X - size.X / 2f, CenterPoint.Y + CircleOffset + CircleTextureSize / 2 + 4, Color.White, Color.Black, default);
        }

        public static void DoTeleportation(int circle)
        {
            Open = false;

            Player p = Main.LocalPlayer;

            switch (circle)
            {
                case 0:
					Vector2? underworld = CalamityPlayer.GetUnderworldPosition(p);
					if (!underworld.HasValue)
						return;
					CalamityPlayer.ModTeleport(p, underworld.Value, true, TeleportationStyleID.DemonConch);
                    break;
                case 1:
                    CalamityPlayer.ModTeleport(p, new Vector2(Main.dungeonX * 16, Main.dungeonY * 16 - 8));
                    break;
                case 2:
					Vector2? jungle = CalamityPlayer.GetJunglePosition(p);
					if (!jungle.HasValue)
						return;
					CalamityPlayer.ModTeleport(p, jungle.Value);
                    break;
                case 3:
					if (Main.netMode == NetmodeID.SinglePlayer)
					{
						p.TeleportationPotion();
						SoundEngine.PlaySound(SoundID.Item6, p.position);
					}
					else if (Main.netMode == NetmodeID.MultiplayerClient)
					{
						NetMessage.SendData(MessageID.RequestTeleportationByServer, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
					}
                    break;
                default:
                    break;
            }
        }

        private static void DrawCircle(SpriteBatch sb, Vector2 center, int circle, CircleStyle style)
        {
            sb.Draw(CircleTextures, center, new Rectangle(circle * CircleTextureSize, (int)style * CircleTextureSize, CircleTextureSize, CircleTextureSize), Color.White, 0f, new Vector2(CircleTextureSize / 2), 1f, SpriteEffects.None, 0f);
        }

        private class Circle
        {
            public Vector2 Center;
            public float Radius;
            public Circle(Vector2 C, float R)
            {
                this.Center = C;
                this.Radius = R;
            }
            public bool Contains(Vector2 point)
            {
                return Vector2.Distance(Center, point) < Radius;
            }
            public bool Contains(Point point)
            {
                return Contains(point.ToVector2());
            }
        }
    }
}

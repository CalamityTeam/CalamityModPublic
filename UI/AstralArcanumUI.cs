using CalamityMod.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using ReLogic.Content;

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
        private static string[] CircleNames;

        private static int LastHovered;

        public static void Load(Mod mod)
        {
            CircleTextures = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/AstralArcanumCircles", AssetRequestMode.ImmediateLoad).Value;
            CircleNames = new string[]
            {
                "Underworld",
                "Dungeon",
                "Jungle",
                "Random"
            };
        }

        public static void Unload()
        {
            CircleNames = null;
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
                //Closed, do stuff in here if need be
            }
        }

        public static void UpdateAndDraw(SpriteBatch sb)
        {
            //Don't do anything if not open
            bool forceOpenAndTeleport = BossRushEvent.BossRushStage < BossRushEvent.Bosses.Count - 1 && BossRushEvent.CurrentlyFoughtBoss == NPCID.WallofFlesh &&
                !CalamityUtils.AnyBossNPCS() && !Main.player[Main.myPlayer].ZoneUnderworldHeight;
            if (forceOpenAndTeleport)
            {
                Open = true;
            }
            else if (BossRushEvent.BossRushActive)
                Open = false;
            if (!Open)
                return;

            //Draw center circle
            DrawCircle(sb, CenterPoint, 0, CircleStyle.Normal);

            Vector2 centerToMouse = Main.MouseScreen - CenterPoint;
            float rotation = centerToMouse.ToRotation();

            //Draw the arrow that points towards the mouse
            sb.Draw(CircleTextures, CenterPoint, new Rectangle(0, CircleTextureSize, 24, 10), Color.White, rotation, new Vector2(12, 5f), 1f, SpriteEffects.None, 0f);

            int current = 0;
            int selectedCircle = -1;
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    Vector2 center = new Vector2(
                        CenterPoint.X - CircleOffset + x * CircleOffset * 2,
                        CenterPoint.Y - CircleOffset + y * CircleOffset * 2);

                    Circle c = new Circle(center, CircleTextureSize / 2);

                    CircleStyle style = CircleStyle.Normal;

                    //If the mouse is in the circle
                    if (c.Contains(Main.MouseScreen) || forceOpenAndTeleport)
                    {
                        style = CircleStyle.Selected;

                        Main.LocalPlayer.mouseInterface = true;

                        if (forceOpenAndTeleport)
                        {
                            current = 0;
                        }

                        selectedCircle = current;

                        //If left clicked
                        if ((Main.mouseLeft && Main.mouseLeftRelease) || forceOpenAndTeleport)
                        {
                            Main.mouseLeftRelease = false;
                            Main.mouseLeft = false;

                            DoTeleportation(current, forceOpenAndTeleport);
                        }
                    }

                    DrawCircle(sb, center, 1 + current, style);

                    current++;
                }
            }

            //Literally so the sound plays
            if (LastHovered != selectedCircle)
            {
                LastHovered = selectedCircle;
                if (LastHovered != -1)
                    SoundEngine.PlaySound(SoundID.MenuTick);
            }

            string text = "Select";
            if (selectedCircle != -1)
            {
                text = CircleNames[selectedCircle];
            }

            Vector2 size = FontAssets.MouseText.Value.MeasureString(text);
            Utils.DrawBorderStringFourWay(sb, FontAssets.MouseText.Value, text, CenterPoint.X - size.X / 2f, CenterPoint.Y + CircleOffset + CircleTextureSize / 2 + 4, Color.White, Color.Black, default);
        }

        public static void DoTeleportation(int circle, bool forcedByBossRush)
        {
            Open = false;

            Player p = Main.LocalPlayer;

            if (forcedByBossRush)
                SoundEngine.PlaySound(BossRushEvent.TeleportSound with { Volume = 1.6f }, p.position);

            if (circle == 3)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    p.TeleportationPotion();
                    if (!forcedByBossRush)
                        SoundEngine.PlaySound(SoundID.Item6, p.position);
                }
                else if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.RequestTeleportationByServer, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
                }
            }
            else
            {
                Main.LocalPlayer.Calamity().HandleTeleport(circle, false, 0);
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

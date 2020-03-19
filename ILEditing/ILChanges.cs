using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using static Mono.Cecil.Cil.OpCodes;

namespace CalamityMod.ILEditing
{
    public class ILChanges
    {
        /// <summary>
        /// Loads all IL Editing changes in the mod.
        /// </summary>
        public static void Initialize()
        {
            IL.Terraria.Main.DrawInvasionProgress += (il) =>
            {
                var cursor = new ILCursor(il)
                {
                    Index = 1
                };
                cursor.EmitDelegate<Action>(() =>
                {
                    if (Main.invasionProgressMode == 2 && (Main.invasionProgressNearInvasion || AcidRainEvent.NearInvasionCheck(7500)) && Main.invasionProgressDisplayLeft < 160)
                    {
                        Main.invasionProgressDisplayLeft = 160;
                    }
                    if (!Main.gamePaused && Main.invasionProgressDisplayLeft > 0)
                    {
                        Main.invasionProgressDisplayLeft--;
                    }
                    if (Main.invasionProgressDisplayLeft > 0)
                    {
                        Main.invasionProgressAlpha += 0.05f;
                    }
                    else
                    {
                        Main.invasionProgressAlpha -= 0.05f;
                    }
                    if (Main.invasionProgressAlpha < 0f)
                    {
                        Main.invasionProgressAlpha = 0f;
                    }
                    if (Main.invasionProgressAlpha > 1f)
                    {
                        Main.invasionProgressAlpha = 1f;
                    }
                    if (CalamityWorld.rainingAcid)
                    {
                        Main.invasionProgressAlpha = 1f;
                    }
                    if (Main.invasionProgressAlpha <= 0f)
                    {
                        return;
                    }
                    bool draw = CalamityWorld.rainingAcid || CalamityWorld.acidRainExtraDrawTime > 0;
                    if ((draw && !Main.LocalPlayer.Calamity().ZoneSulphur) ||
                        (!draw && Main.LocalPlayer.Calamity().ZoneSulphur))
                        return;

                    float progressAlpha = 0.5f + Main.invasionProgressAlpha * 0.5f;
                    if (CalamityWorld.acidRainExtraDrawTime > 0)
                    {
                        progressAlpha = 0.5f + CalamityWorld.acidRainExtraDrawTime / 40f * 0.5f;
                        CalamityWorld.acidRainExtraDrawTime--;
                    }
                    Vector2 screenCoords = new Vector2(Main.screenWidth - 120, Main.screenHeight - 40);
                    int sizeX = (int)(200f * progressAlpha);
                    int sizeY = (int)(45f * progressAlpha);
                    Rectangle screenCoordsRectangle =
                        new Rectangle((int)screenCoords.X - sizeX / 2,
                        (int)screenCoords.Y - sizeY / 2, sizeX, sizeY);
                    Texture2D barTexture = Main.colorBarTexture;

                    // If you touch the netcode behind this without a good reason your life will come to an abrupt end -Dominic
                    if (CalamityWorld.rainingAcid || CalamityWorld.acidRainExtraDrawTime > 0)
                    {
                        Utils.DrawInvBG(Main.spriteBatch, screenCoordsRectangle, new Color(63, 65, 151, 255) * 0.785f);

                        float progressRatio = 1f - CalamityWorld.AcidRainCompletionRatio;
                        string progressText = $"{((int)(100 * progressRatio)).ToString()}%";
                        progressText = Language.GetTextValue("Game.WaveCleared", progressText);
                        Main.spriteBatch.Draw(barTexture, screenCoords, null, Color.White * Main.invasionProgressAlpha, 0f, new Vector2(barTexture.Width / 2, 0f), progressAlpha, SpriteEffects.None, 0f);
                        Vector2 textMeasurement = Main.fontMouseText.MeasureString(progressText);
                        float adjustedAlpha = progressAlpha;
                        if (textMeasurement.Y > 22f)
                        {
                            adjustedAlpha *= 22f / textMeasurement.Y;
                        }
                        float barDrawOffsetX = 169f * progressAlpha;
                        float scaleY = 8f * progressAlpha;
                        Vector2 barDrawPosition = screenCoords + Vector2.UnitY * scaleY + Vector2.UnitX * 1f;
                        Utils.DrawBorderString(Main.spriteBatch, progressText, barDrawPosition + new Vector2(0f, -4f), Color.White * Main.invasionProgressAlpha, adjustedAlpha, 0.5f, 1f, -1);
                        barDrawPosition += Vector2.UnitX * (progressRatio - 0.5f) * barDrawOffsetX;
                        Main.spriteBatch.Draw(Main.magicPixel, barDrawPosition, new Rectangle?(new Rectangle(0, 0, 1, 1)), new Color(255, 241, 51) * Main.invasionProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(barDrawOffsetX * progressRatio, scaleY), SpriteEffects.None, 0f);
                        Main.spriteBatch.Draw(Main.magicPixel, barDrawPosition, new Rectangle?(new Rectangle(0, 0, 1, 1)), new Color(255, 165, 0, 127) * Main.invasionProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(2f, scaleY), SpriteEffects.None, 0f);
                        Main.spriteBatch.Draw(Main.magicPixel, barDrawPosition, new Rectangle?(new Rectangle(0, 0, 1, 1)), Color.Black * Main.invasionProgressAlpha, 0f, new Vector2(0f, 0.5f), new Vector2(barDrawOffsetX * (1f - progressRatio), scaleY), SpriteEffects.None, 0f);

                        string invasionNameText = "Acid Rain";
                        Vector2 textMeasurement2 = Main.fontMouseText.MeasureString(invasionNameText);
                        float x = 120f;
                        if (textMeasurement2.X > 200f)
                        {
                            x += textMeasurement2.X - 200f;
                        }
                        Texture2D iconTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/AcidRainIcon");
                        Rectangle iconRectangle = Utils.CenteredRectangle(new Vector2(Main.screenWidth - x, Main.screenHeight - 80), (textMeasurement2 + new Vector2(iconTexture.Width + 12, 6f)) * progressAlpha);
                        Utils.DrawInvBG(Main.spriteBatch, iconRectangle, AcidRainEvent.TextColor * 0.5f);
                        Main.spriteBatch.Draw(iconTexture, iconRectangle.Left() + Vector2.UnitX * progressAlpha * 8f, null, Color.White * Main.invasionProgressAlpha, 0f, new Vector2(0f, iconTexture.Height / 2), progressAlpha * 0.8f, SpriteEffects.None, 0f);
                        Utils.DrawBorderString(Main.spriteBatch, invasionNameText, iconRectangle.Right() + Vector2.UnitX * progressAlpha * -22f, Color.White * Main.invasionProgressAlpha, progressAlpha * 0.9f, 1f, 0.4f, -1);
                    }
                    else
                    {
                        if (Main.invasionProgress == -1)
                        {
                            return;
                        }
                        Texture2D iconTexture = Main.extraTexture[9];
                        string invasionNameText = "";
                        Color drawColor = Color.White;
                        if (Main.invasionProgressIcon == 1)
                        {
                            iconTexture = Main.extraTexture[8];
                            invasionNameText = Language.GetTextValue("LegacyInterface.83");
                            drawColor = new Color(64, 109, 164) * 0.5f;
                        }
                        else if (Main.invasionProgressIcon == 2)
                        {
                            iconTexture = Main.extraTexture[12];
                            invasionNameText = Language.GetTextValue("LegacyInterface.84");
                            drawColor = new Color(112, 86, 114) * 0.5f;
                        }
                        else if (Main.invasionProgressIcon == 3)
                        {
                            iconTexture = Main.extraTexture[79];
                            invasionNameText = Language.GetTextValue("DungeonDefenders2.InvasionProgressTitle");
                            drawColor = new Color(88, 0, 160) * 0.5f;
                        }
                        else if (Main.invasionProgressIcon == 7)
                        {
                            iconTexture = Main.extraTexture[10];
                            invasionNameText = Language.GetTextValue("LegacyInterface.85");
                            drawColor = new Color(165, 160, 155) * 0.5f;
                        }
                        else if (Main.invasionProgressIcon == 6)
                        {
                            iconTexture = Main.extraTexture[11];
                            invasionNameText = Language.GetTextValue("LegacyInterface.86");
                            drawColor = new Color(148, 122, 72) * 0.5f;
                        }
                        else if (Main.invasionProgressIcon == 5)
                        {
                            iconTexture = Main.extraTexture[7];
                            invasionNameText = Language.GetTextValue("LegacyInterface.87");
                            drawColor = new Color(173, 135, 140) * 0.5f;
                        }
                        else if (Main.invasionProgressIcon == 4)
                        {
                            iconTexture = Main.extraTexture[9];
                            invasionNameText = Language.GetTextValue("LegacyInterface.88");
                            drawColor = new Color(94, 72, 131) * 0.5f;
                        }
                        else return;
                        if (Main.invasionProgressWave > 0)
                        {
                            Utils.DrawInvBG(Main.spriteBatch, screenCoordsRectangle, new Color(63, 65, 151, 255) * 0.785f);
                            string pointsText;
                            if (Main.invasionProgressMax == 0)
                            {
                                pointsText = Language.GetTextValue("Game.InvasionPoints", Main.invasionProgress);
                            }
                            else
                            {
                                pointsText = ((int)((float)Main.invasionProgress * 100f / (float)Main.invasionProgressMax)).ToString() + "%";
                            }
                            pointsText = Language.GetTextValue("Game.WaveMessage", Main.invasionProgressWave, pointsText);
                            float progressRatio = MathHelper.Clamp(Main.invasionProgress / (float)Main.invasionProgressMax, 0f, 1f);
                            if (Main.invasionProgressMax == 0)
                            {
                                progressRatio = 1f;
                            }
                            float barDrawOffsetX = 169f * progressAlpha;
                            float scaleY = 8f * progressAlpha;
                            Vector2 barDrawPosition = screenCoords + Vector2.UnitY * scaleY + Vector2.UnitX * 1f;
                            Utils.DrawBorderString(Main.spriteBatch, pointsText, barDrawPosition, Color.White * Main.invasionProgressAlpha, progressAlpha, 0.5f, 1f, -1);
                            Main.spriteBatch.Draw(barTexture, screenCoords, null, Color.White * Main.invasionProgressAlpha, 0f, new Vector2(barTexture.Width / 2, 0f), progressAlpha, SpriteEffects.None, 0f);
                            barDrawPosition += Vector2.UnitX * (progressRatio - 0.5f) * barDrawOffsetX;
                            Main.spriteBatch.Draw(Main.magicPixel, barDrawPosition, new Rectangle?(new Rectangle(0, 0, 1, 1)), new Color(255, 241, 51) * Main.invasionProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(barDrawOffsetX * progressRatio, scaleY), SpriteEffects.None, 0f);
                            Main.spriteBatch.Draw(Main.magicPixel, barDrawPosition, new Rectangle?(new Rectangle(0, 0, 1, 1)), new Color(255, 165, 0, 127) * Main.invasionProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(2f, scaleY), SpriteEffects.None, 0f);
                            Main.spriteBatch.Draw(Main.magicPixel, barDrawPosition, new Rectangle?(new Rectangle(0, 0, 1, 1)), Color.Black * Main.invasionProgressAlpha, 0f, new Vector2(0f, 0.5f), new Vector2(barDrawOffsetX * (1f - progressRatio), scaleY), SpriteEffects.None, 0f);
                        }
                        else
                        {
                            Vector2 vectorIconRectangle = new Vector2(Main.screenWidth - 120, Main.screenHeight - 40);
                            Utils.DrawInvBG(Main.spriteBatch, screenCoordsRectangle, new Color(63, 65, 151, 255) * 0.785f);
                            string progressText;
                            if (Main.invasionProgressMax == 0)
                            {
                                progressText = Main.invasionProgress.ToString();
                            }
                            else
                            {
                                progressText = ((int)(Main.invasionProgress * 100f / Main.invasionProgressMax)).ToString() + "%";
                            }
                            progressText = Language.GetTextValue("Game.WaveCleared", progressText);
                            if (Main.invasionProgressMax != 0)
                            {
                                Main.spriteBatch.Draw(barTexture, vectorIconRectangle, null, Color.White * Main.invasionProgressAlpha, 0f, new Vector2((float)(barTexture.Width / 2), 0f), progressAlpha, SpriteEffects.None, 0f);
                                float progressRatio = MathHelper.Clamp(Main.invasionProgress / (float)Main.invasionProgressMax, 0f, 1f);
                                Vector2 textMeasurement = Main.fontMouseText.MeasureString(progressText);
                                float adjustedAlpha = progressAlpha;
                                if (textMeasurement.Y > 22f)
                                {
                                    adjustedAlpha *= 22f / textMeasurement.Y;
                                }
                                float barDrawOffsetX = 169f * progressAlpha;
                                float scaleY = 8f * progressAlpha;
                                Vector2 barDrawPosition = vectorIconRectangle + Vector2.UnitY * scaleY + Vector2.UnitX * 1f;
                                Utils.DrawBorderString(Main.spriteBatch, progressText, barDrawPosition + new Vector2(0f, -4f), Color.White * Main.invasionProgressAlpha, adjustedAlpha, 0.5f, 1f, -1);
                                barDrawPosition += Vector2.UnitX * (progressRatio - 0.5f) * barDrawOffsetX;
                                Main.spriteBatch.Draw(Main.magicPixel, barDrawPosition, new Rectangle?(new Rectangle(0, 0, 1, 1)), new Color(255, 241, 51) * Main.invasionProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(barDrawOffsetX * progressRatio, scaleY), SpriteEffects.None, 0f);
                                Main.spriteBatch.Draw(Main.magicPixel, barDrawPosition, new Rectangle?(new Rectangle(0, 0, 1, 1)), new Color(255, 165, 0, 127) * Main.invasionProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(2f, scaleY), SpriteEffects.None, 0f);
                                Main.spriteBatch.Draw(Main.magicPixel, barDrawPosition, new Rectangle?(new Rectangle(0, 0, 1, 1)), Color.Black * Main.invasionProgressAlpha, 0f, new Vector2(0f, 0.5f), new Vector2(barDrawOffsetX * (1f - progressRatio), scaleY), SpriteEffects.None, 0f);
                            }
                        }
                        Vector2 textMeasurement2 = Main.fontMouseText.MeasureString(invasionNameText);
                        float x = 120f;
                        if (textMeasurement2.X > 200f)
                        {
                            x += textMeasurement2.X - 200f;
                        }
                        Rectangle iconRectangle = Utils.CenteredRectangle(new Vector2(Main.screenWidth - x, Main.screenHeight - 80), (textMeasurement2 + new Vector2(iconTexture.Width + 12, 6f)) * progressAlpha);
                        Utils.DrawInvBG(Main.spriteBatch, iconRectangle, drawColor);
                        Main.spriteBatch.Draw(iconTexture, iconRectangle.Left() + Vector2.UnitX * progressAlpha * 8f, null, Color.White * Main.invasionProgressAlpha, 0f, new Vector2(0f, iconTexture.Height / 2), progressAlpha * 0.8f, SpriteEffects.None, 0f);
                        Utils.DrawBorderString(Main.spriteBatch, invasionNameText, iconRectangle.Right() + Vector2.UnitX * progressAlpha * -22f, Color.White * Main.invasionProgressAlpha, progressAlpha * 0.9f, 1f, 0.4f, -1);
                    }
                });
                cursor.Emit(Pop);
                cursor.Emit(Ret);
            };

            IL.Terraria.WorldGen.MakeDungeon += (il) =>
            {
                var cursor = new ILCursor(il)
                {
                    Index = 45
                };
                cursor.EmitDelegate<Action>(() =>
                {
                    WorldGen.dungeonX += (WorldGen.dungeonX < Main.maxTilesX / 2).ToDirectionInt() * 450;

                    if (WorldGen.dungeonX < 200)
                        WorldGen.dungeonX = 200;
                    if (WorldGen.dungeonX > Main.maxTilesX - 200)
                        WorldGen.dungeonX = Main.maxTilesX - 200;
                });
            };
        }
    }
}

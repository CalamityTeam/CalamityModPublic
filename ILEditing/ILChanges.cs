using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using MonoMod.Cil;
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
                // 201 was retrieved by searching the instructions in dnspy
                var cursor = new ILCursor(il)
                {
                    Index = 201
                };
                cursor.EmitDelegate<Action>(() =>
				{
					if ((CalamityWorld.rainingAcid && !Main.LocalPlayer.Calamity().ZoneSulphur) ||
						(!CalamityWorld.rainingAcid && Main.LocalPlayer.Calamity().ZoneSulphur))
						return;
					float progressAlpha = 0.5f + Main.invasionProgressAlpha * 0.5f;
                    if (CalamityWorld.rainingAcid && Main.LocalPlayer.Calamity().ZoneSulphur &&
                        Main.LocalPlayer.Center.Y < (int)(Main.worldSurface * 16) + 1600)
                    {
                        int sizeX = (int)(200f * progressAlpha);
                        int sizeY = (int)(45f * progressAlpha);
                        Vector2 screenCoords = new Vector2(Main.screenWidth - 120, Main.screenHeight - 40);
                        Rectangle screenCoordsRectangle = new Rectangle((int)screenCoords.X - sizeX / 2, (int)screenCoords.Y - sizeY / 2, sizeX, sizeY);
                        Utils.DrawInvBG(Main.spriteBatch, screenCoordsRectangle, new Color(63, 65, 151, 255) * 0.785f);

                        float progressRatio = 1f - MathHelper.Clamp(Main.invasionSize / (float)Main.invasionSizeStart, 0f, 1f);
                        string progressText = $"{((int)(100 * progressRatio)).ToString()}%";
                        progressText = Language.GetTextValue("Game.WaveCleared", progressText);
                        Texture2D barTexture = Main.colorBarTexture;
                        Texture2D blipTexture = Main.colorBlipTexture;
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
                        Main.spriteBatch.Draw(iconTexture, iconRectangle.Left() + Vector2.UnitX * progressAlpha * 8f, null, Color.White * Main.invasionProgressAlpha, 0f, new Vector2(0f, (float)(iconTexture.Height / 2)), progressAlpha * 0.8f, SpriteEffects.None, 0f);
                        Utils.DrawBorderString(Main.spriteBatch, invasionNameText, iconRectangle.Right() + Vector2.UnitX * progressAlpha * -22f, Color.White * Main.invasionProgressAlpha, progressAlpha * 0.9f, 1f, 0.4f, -1);
                    }
                    // I don't have the energy to clean this up. If you want to do so, go ahead.
                    else
                    {
                        Texture2D texture2D = Main.extraTexture[9];
                        string text = "";
                        Color c = Color.White;
                        if (Main.invasionProgressIcon == 1)
                        {
                            texture2D = Main.extraTexture[8];
                            text = Lang.inter[83].Value;
                            c = new Color(64, 109, 164) * 0.5f;
                        }
                        else if (Main.invasionProgressIcon == 2)
                        {
                            texture2D = Main.extraTexture[12];
                            text = Lang.inter[84].Value;
                            c = new Color(112, 86, 114) * 0.5f;
                        }
                        else if (Main.invasionProgressIcon == 3)
                        {
                            texture2D = Main.extraTexture[79];
                            text = Language.GetTextValue("DungeonDefenders2.InvasionProgressTitle");
                            c = new Color(88, 0, 160) * 0.5f;
                        }
                        else if (Main.invasionProgressIcon == 7)
                        {
                            texture2D = Main.extraTexture[10];
                            text = Lang.inter[85].Value;
                            c = new Color(165, 160, 155) * 0.5f;
                        }
                        else if (Main.invasionProgressIcon == 6)
                        {
                            texture2D = Main.extraTexture[11];
                            text = Lang.inter[86].Value;
                            c = new Color(148, 122, 72) * 0.5f;
                        }
                        else if (Main.invasionProgressIcon == 5)
                        {
                            texture2D = Main.extraTexture[7];
                            text = Lang.inter[87].Value;
                            c = new Color(173, 135, 140) * 0.5f;
                        }
                        else if (Main.invasionProgressIcon == 4)
                        {
                            texture2D = Main.extraTexture[9];
                            text = Lang.inter[88].Value;
                            c = new Color(94, 72, 131) * 0.5f;
                        }
                        if (Main.invasionProgressWave > 0)
                        {
                            int num2 = (int)(200f * progressAlpha);
                            int num3 = (int)(45f * progressAlpha);
                            Vector2 vector = new Vector2((float)(Main.screenWidth - 120), (float)(Main.screenHeight - 40));
                            Rectangle r = new Rectangle((int)vector.X - num2 / 2, (int)vector.Y - num3 / 2, num2, num3);
                            Utils.DrawInvBG(Main.spriteBatch, r, new Color(63, 65, 151, 255) * 0.785f);
                            string text2;
                            if (Main.invasionProgressMax == 0)
                            {
                                text2 = Language.GetTextValue("Game.InvasionPoints", Main.invasionProgress);
                            }
                            else
                            {
                                text2 = ((int)((float)Main.invasionProgress * 100f / (float)Main.invasionProgressMax)).ToString() + "%";
                            }
                            text2 = Language.GetTextValue("Game.WaveMessage", Main.invasionProgressWave, text2);
                            Texture2D texture2D2 = Main.colorBarTexture;
                            Texture2D texture2D4 = Main.colorBlipTexture;
                            float num4 = MathHelper.Clamp((float)Main.invasionProgress / (float)Main.invasionProgressMax, 0f, 1f);
                            if (Main.invasionProgressMax == 0)
                            {
                                num4 = 1f;
                            }
                            float num5 = 169f * progressAlpha;
                            float num6 = 8f * progressAlpha;
                            Vector2 vector2 = vector + Vector2.UnitY * num6 + Vector2.UnitX * 1f;
                            Utils.DrawBorderString(Main.spriteBatch, text2, vector2, Color.White * Main.invasionProgressAlpha, progressAlpha, 0.5f, 1f, -1);
                            Main.spriteBatch.Draw(texture2D2, vector, null, Color.White * Main.invasionProgressAlpha, 0f, new Vector2((float)(texture2D2.Width / 2), 0f), progressAlpha, SpriteEffects.None, 0f);
                            vector2 += Vector2.UnitX * (num4 - 0.5f) * num5;
                            Main.spriteBatch.Draw(Main.magicPixel, vector2, new Rectangle?(new Rectangle(0, 0, 1, 1)), new Color(255, 241, 51) * Main.invasionProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(num5 * num4, num6), SpriteEffects.None, 0f);
                            Main.spriteBatch.Draw(Main.magicPixel, vector2, new Rectangle?(new Rectangle(0, 0, 1, 1)), new Color(255, 165, 0, 127) * Main.invasionProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(2f, num6), SpriteEffects.None, 0f);
                            Main.spriteBatch.Draw(Main.magicPixel, vector2, new Rectangle?(new Rectangle(0, 0, 1, 1)), Color.Black * Main.invasionProgressAlpha, 0f, new Vector2(0f, 0.5f), new Vector2(num5 * (1f - num4), num6), SpriteEffects.None, 0f);
                        }
                        else
                        {
                            int num7 = (int)(200f * progressAlpha);
                            int num8 = (int)(45f * progressAlpha);
                            Vector2 vector3 = new Vector2((float)(Main.screenWidth - 120), (float)(Main.screenHeight - 40));
                            Rectangle r2 = new Rectangle((int)vector3.X - num7 / 2, (int)vector3.Y - num8 / 2, num7, num8);
                            Utils.DrawInvBG(Main.spriteBatch, r2, new Color(63, 65, 151, 255) * 0.785f);
                            string text3;
                            if (Main.invasionProgressMax == 0)
                            {
                                text3 = Main.invasionProgress.ToString();
                            }
                            else
                            {
                                text3 = ((int)((float)Main.invasionProgress * 100f / (float)Main.invasionProgressMax)).ToString() + "%";
                            }
                            text3 = Language.GetTextValue("Game.WaveCleared", text3);
                            Texture2D texture2D3 = Main.colorBarTexture;
                            Texture2D texture2D5 = Main.colorBlipTexture;
                            if (Main.invasionProgressMax != 0)
                            {
                                Main.spriteBatch.Draw(texture2D3, vector3, null, Color.White * Main.invasionProgressAlpha, 0f, new Vector2((float)(texture2D3.Width / 2), 0f), progressAlpha, SpriteEffects.None, 0f);
                                float num9 = MathHelper.Clamp((float)Main.invasionProgress / (float)Main.invasionProgressMax, 0f, 1f);
                                Vector2 vector4 = Main.fontMouseText.MeasureString(text3);
                                float num10 = progressAlpha;
                                if (vector4.Y > 22f)
                                {
                                    num10 *= 22f / vector4.Y;
                                }
                                float num11 = 169f * progressAlpha;
                                float num12 = 8f * progressAlpha;
                                Vector2 vector5 = vector3 + Vector2.UnitY * num12 + Vector2.UnitX * 1f;
                                Utils.DrawBorderString(Main.spriteBatch, text3, vector5 + new Vector2(0f, -4f), Color.White * Main.invasionProgressAlpha, num10, 0.5f, 1f, -1);
                                vector5 += Vector2.UnitX * (num9 - 0.5f) * num11;
                                Main.spriteBatch.Draw(Main.magicPixel, vector5, new Rectangle?(new Rectangle(0, 0, 1, 1)), new Color(255, 241, 51) * Main.invasionProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(num11 * num9, num12), SpriteEffects.None, 0f);
                                Main.spriteBatch.Draw(Main.magicPixel, vector5, new Rectangle?(new Rectangle(0, 0, 1, 1)), new Color(255, 165, 0, 127) * Main.invasionProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(2f, num12), SpriteEffects.None, 0f);
                                Main.spriteBatch.Draw(Main.magicPixel, vector5, new Rectangle?(new Rectangle(0, 0, 1, 1)), Color.Black * Main.invasionProgressAlpha, 0f, new Vector2(0f, 0.5f), new Vector2(num11 * (1f - num9), num12), SpriteEffects.None, 0f);
                            }
                        }
                        Vector2 value = Main.fontMouseText.MeasureString(text);
                        float num13 = 120f;
                        if (value.X > 200f)
                        {
                            num13 += value.X - 200f;
                        }
                        Rectangle r3 = Utils.CenteredRectangle(new Vector2((float)Main.screenWidth - num13, (float)(Main.screenHeight - 80)), (value + new Vector2((float)(texture2D.Width + 12), 6f)) * progressAlpha);
                        Utils.DrawInvBG(Main.spriteBatch, r3, c);
                        Main.spriteBatch.Draw(texture2D, r3.Left() + Vector2.UnitX * progressAlpha * 8f, null, Color.White * Main.invasionProgressAlpha, 0f, new Vector2(0f, (float)(texture2D.Height / 2)), progressAlpha * 0.8f, SpriteEffects.None, 0f);
                        Utils.DrawBorderString(Main.spriteBatch, text, r3.Right() + Vector2.UnitX * progressAlpha * -22f, Color.White * Main.invasionProgressAlpha, progressAlpha * 0.9f, 1f, 0.4f, -1);
                    }
                });
				cursor.Emit(Pop);
				cursor.Emit(Ret);
			};
        }
    }
}

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.DrawLayers
{
    public class GemTechGemLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.BackAcc);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            return drawInfo.shadow == 0f && !drawPlayer.dead && modPlayer.GemTechSet && drawPlayer.Calamity().andromedaState == AndromedaPlayerState.Inactive;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            float opacity = MathHelper.Lerp(0.85f, 1.05f, (float)Math.Cos(Main.GlobalTimeWrappedHourly * 2.3f) * 0.5f + 0.5f);
            float time = Main.GlobalTimeWrappedHourly * 0.61f;
            float gemTime = Main.GlobalTimeWrappedHourly * 3.41f;
            for (int i = 5; i >= 0; i--)
            {
                Texture2D gemTexture;
                float pulseFactor = 1.8f;
                GemTechArmorGemType gemType;
                switch (i)
                {
                    case 0:
                    default:
                        gemTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/GemTechRedGem").Value;
                        gemType = GemTechArmorGemType.Rogue;
                        break;
                    case 1:
                        gemTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/GemTechYellowGem").Value;
                        gemType = GemTechArmorGemType.Melee;
                        break;
                    case 2:
                        gemTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/GemTechGreenGem").Value;
                        gemType = GemTechArmorGemType.Ranged;
                        break;
                    case 3:
                        gemTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/GemTechBlueGem").Value;
                        gemType = GemTechArmorGemType.Summoner;
                        break;
                    case 4:
                        gemTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/GemTechPurpleGem").Value;
                        gemType = GemTechArmorGemType.Magic;
                        break;
                    case 5:
                        gemTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/GemTechPinkGem").Value;
                        gemType = GemTechArmorGemType.Base;
                        pulseFactor = 2.5f;
                        break;
                }

                // Don't bother doing anything else if the gem is inactive.
                if (!modPlayer.GemTechState.GemIsActive(gemType))
                    continue;

                float drawOffsetAngle = modPlayer.GemTechState.CalculateGemOffsetAngle(gemType, gemTime);
                float gemOpacity = opacity;

                // Incorporate a sinusoidal pulse into the pulse factor.
                pulseFactor *= (float)Math.Cos(time + drawOffsetAngle);

                // Somewhat unorthodox way of creating an illusion of gems being drawn behind the player via orbiting.
                // Instead of actually messing with a Z position it simply fades away when the sine of the draw
                // offset angle is in the low negatives.
                gemOpacity *= Utils.GetLerpValue(-0.75f, -0.51f, (float)Math.Sin(drawOffsetAngle), true);

                Vector2 baseDrawPosition = modPlayer.GemTechState.CalculateGemPosition(gemType) - Main.screenPosition;

                // Draw back afterimages.
                float afterimageTime = Main.GlobalTimeWrappedHourly * 0.47f;
                float backAfterImageOpacity = gemOpacity * 0.24f;
                Vector2 origin = gemTexture.Size() * 0.5f;
                for (int j = 0; j < 5; j++)
                {
                    Color backAfterimageColor = Main.hslToRgb((afterimageTime + j / 5f) % 1f, 1f, 0.67f);
                    backAfterimageColor = Color.Lerp(backAfterimageColor, Color.White, 0.64f) * backAfterImageOpacity;
                    Vector2 drawPosition = baseDrawPosition + (MathHelper.TwoPi * j / 5f).ToRotationVector2() * pulseFactor;
                    DrawData gemBackDrawData = new DrawData(gemTexture, drawPosition, null, backAfterimageColor, 0f, origin, 1f, SpriteEffects.None, 0);
                    drawInfo.DrawDataCache.Add(gemBackDrawData);
                }

                // Draw the main gem.
                Color baseGemColor = Main.hslToRgb(Main.GlobalTimeWrappedHourly * 0.51f % 1f, 1f, 0.67f);
                baseGemColor = Color.Lerp(baseGemColor, Color.White, 0.56f);
                baseGemColor.A = 105;
                baseGemColor *= gemOpacity;
                DrawData gemDrawData = new DrawData(gemTexture, baseDrawPosition, null, baseGemColor, 0f, gemTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
                drawInfo.DrawDataCache.Add(gemDrawData);
            }
        }
    }
}

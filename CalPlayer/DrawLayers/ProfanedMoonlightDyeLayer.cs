using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Items.Dyes;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.DrawLayers
{
    public class ProfanedMoonlightDyeLayer : PlayerDrawLayer
    {
        public static readonly List<Color> MoonlightDyeDayColors = new()
        {
            new Color(255, 163, 56),
            new Color(235, 30, 19),
            new Color(242, 48, 187),
        };

        public static readonly List<Color> MoonlightDyeNightColors = new()
        {
            new Color(24, 134, 198),
            new Color(130, 40, 150),
            new Color(40, 64, 150),
        };

        public static void DetermineMoonlightDyeColors(out Color drawColor, Color dayColor, Color nightColor)
        {
            int totalTime = Main.dayTime ? 54000 : 32400;
            float transitionTime = 5400;
            float interval = Utils.GetLerpValue(0f, transitionTime, (float)Main.time, true) + Utils.GetLerpValue(totalTime - transitionTime, totalTime, (float)Main.time, true);
            if (Main.dayTime)
            {
                // Dusk.
                if (Main.time >= totalTime - transitionTime)
                    drawColor = Color.Lerp(dayColor, nightColor, Utils.GetLerpValue(totalTime - transitionTime, totalTime, (float)Main.time, true));
                // Dawn.
                else if (Main.time <= transitionTime)
                    drawColor = Color.Lerp(nightColor, dayColor, interval);
                else
                    drawColor = dayColor;
            }
            else drawColor = nightColor;
        }

        public static Color GetCurrentMoonlightDyeColor(float angleOffset = 0f)
        {
            float interval = (float)Math.Cos(Main.GlobalTimeWrappedHourly * 0.6f + angleOffset) * 0.5f + 0.5f;
            interval = MathHelper.Clamp(interval, 0f, 0.995f);
            Color dayColorToUse = CalamityUtils.MulticolorLerp(interval, MoonlightDyeDayColors.ToArray());
            Color nightColorToUse = CalamityUtils.MulticolorLerp(interval, MoonlightDyeNightColors.ToArray());
            DetermineMoonlightDyeColors(out Color drawColor, dayColorToUse, nightColorToUse);
            return drawColor;
        }

        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.EyebrellaCloud);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f)
                return false;

            Player drawPlayer = drawInfo.drawPlayer;
            int totalMoonlightDyes = drawPlayer.dye.Count(dyeItem => dyeItem.type == ModContent.ItemType<ProfanedMoonlightDye>());
            return totalMoonlightDyes > 0;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            int totalMoonlightDyes = drawPlayer.dye.Count(dyeItem => dyeItem.type == ModContent.ItemType<ProfanedMoonlightDye>());
            drawInfo.DrawDataCache.AddRange(CalamityUtils.DrawAuroras(drawPlayer, 5 + (int)MathHelper.Clamp(totalMoonlightDyes, 0f, 4f) * 2, MathHelper.Clamp(totalMoonlightDyes / 3f, 0f, 1f), GetCurrentMoonlightDyeColor()));
        }
    }
}

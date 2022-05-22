using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class LightingEffectsSystem : ModSystem
    {
        public const float MaxSignusDarkness = -0.4f;
        public const float MaxAbyssDarkness = -0.7f;
        public override void ModifyLightingBrightness(ref float scale)
        {
            // Apply the calculated darkness value for the local player.
            CalamityPlayer modPlayer = Main.LocalPlayer.Calamity();
            float darkRatio = MathHelper.Clamp(modPlayer.caveDarkness, 0f, 1f);

            if (modPlayer.ZoneAbyss)
                scale += MaxAbyssDarkness * darkRatio;

            if (CalamityWorld.revenge)
            {
                if (CalamityGlobalNPC.signus != -1)
                {
                    if (Main.npc[CalamityGlobalNPC.signus].active)
                    {
                        if (Vector2.Distance(Main.LocalPlayer.Center, Main.npc[CalamityGlobalNPC.signus].Center) <= 5200f)
                        {
                            float signusLifeRatio = 1f - (Main.npc[CalamityGlobalNPC.signus].life / Main.npc[CalamityGlobalNPC.signus].lifeMax);

                            // Reduce the power of Signus darkness based on your light level.
                            float multiplier = 1f;
                            switch (Main.LocalPlayer.GetCurrentAbyssLightLevel())
                            {
                                case 0:
                                    break;
                                case 1:
                                case 2:
                                    multiplier = 0.75f;
                                    break;
                                case 3:
                                case 4:
                                    multiplier = 0.5f;
                                    break;
                                case 5:
                                case 6:
                                    multiplier = 0.25f;
                                    break;
                                default:
                                    multiplier = 0f;
                                    break;
                            }

                            // Total darkness
                            float signusDarkness = signusLifeRatio * multiplier;
                            darkRatio = MathHelper.Clamp(signusDarkness, 0f, 1f);
                            scale += MaxSignusDarkness * darkRatio;
                        }
                    }
                }
            }
        }

        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            if (Main.gameMenu)
                BossRushEvent.StartTimer = 0;

            float bossRushWhiteFade = BossRushEvent.StartTimer / (float)BossRushEvent.StartEffectTotalTime;
            if (BossRushSky.ShouldDrawRegularly)
                bossRushWhiteFade = 1f;

            if (BossRushEvent.BossRushActive || BossRushEvent.StartTimer > 0 || BossRushSky.ShouldDrawRegularly)
            {
                backgroundColor = Color.Lerp(backgroundColor, Color.LightGray, bossRushWhiteFade);
                tileColor = Color.Lerp(tileColor, Color.LightGray, bossRushWhiteFade);
                Main.ColorOfTheSkies = Color.Lerp(Main.ColorOfTheSkies, Color.Gray, bossRushWhiteFade);
            }
            else if (SkyManager.Instance["CalamityMod:ExoMechs"].IsActive())
            {
                float intensity = SkyManager.Instance["CalamityMod:ExoMechs"].Opacity;
                backgroundColor = Color.Lerp(backgroundColor, Color.DarkGray, intensity * 0.9f);
                backgroundColor = Color.Lerp(backgroundColor, Color.Black, intensity * 0.67f);
                tileColor = Color.Lerp(tileColor, Color.DarkGray, intensity * 0.8f);
                tileColor = Color.Lerp(tileColor, Color.Black, intensity * 0.3f);
                Main.ColorOfTheSkies = Color.Lerp(Main.ColorOfTheSkies, Color.DarkGray, intensity * 0.9f);
                Main.ColorOfTheSkies = Color.Lerp(Main.ColorOfTheSkies, Color.Black, intensity * 0.65f);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CalamityMod.CalPlayer.DrawLayers;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer
{
    public partial class CalamityPlayer : ModPlayer
    {
        #region Draw Hooks
        
        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            if (Player is null)
                return;

            // Remove shoe drawing effects if special legs are meant to be drawn.
            if (CalamityLists.legOverrideList.Contains(Player.legs))
            {
                PlayerDrawLayers.Shoes.Hide();
            }

            if (drawInfo.drawPlayer.Calamity().andromedaState != AndromedaPlayerState.Inactive)
            {
                foreach (var layer in PlayerDrawLayerLoader.Layers)
                {
                    if (layer != PlayerDrawLayers.BackAcc)
                        layer.Hide();
                }
            }

            if (Player.HeldItem.ModItem is IHideFrontArm amputator && amputator.ShouldHideArm(Player))
            {
                PlayerDrawLayers.ArmOverItem.Hide();
            }
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (Player.Calamity().andromedaState != AndromedaPlayerState.Inactive)
                AndromedaMechLayer.DrawTheStupidFuckingRobot(ref drawInfo);

            CalamityPlayer calamityPlayer = Player.Calamity();

            // Dust modifications while high.
            if (calamityPlayer.trippy)
            {
                if (Main.myPlayer == Player.whoAmI)
                {
                    Rectangle screenArea = new Rectangle((int)Main.screenPosition.X - 500, (int)Main.screenPosition.Y - 50, Main.screenWidth + 1000, Main.screenHeight + 100);
                    int dustDrawn = 0;
                    float maxShroomDust = Main.maxDustToDraw / 2;
                    Color shroomColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0);
                    for (int i = 0; i < Main.maxDustToDraw; i++)
                    {
                        Dust dust = Main.dust[i];
                        if (dust.active)
                        {
                            // Only draw dust near the screen, for performance reasons.
                            if (new Rectangle((int)dust.position.X, (int)dust.position.Y, 4, 4).Intersects(screenArea))
                            {
                                dust.color = shroomColor;
                                for (int j = 0; j < 4; j++)
                                {
                                    Vector2 dustDrawPosition = dust.position;
                                    Vector2 dustCenter = dustDrawPosition + new Vector2(4f);

                                    float distanceX = Math.Abs(dustCenter.X - Player.Center.X);
                                    float distanceY = Math.Abs(dustCenter.Y - Player.Center.Y);
                                    if (j == 0 || j == 2)
                                        dustDrawPosition.X = Player.Center.X + distanceX;
                                    else
                                        dustDrawPosition.X = Player.Center.X - distanceX;

                                    dustDrawPosition.X -= 4f;

                                    if (j == 0 || j == 1)
                                        dustDrawPosition.Y = Player.Center.Y + distanceY;
                                    else
                                        dustDrawPosition.Y = Player.Center.Y - distanceY;

                                    dustDrawPosition.Y -= 4f;
                                    Main.spriteBatch.Draw(TextureAssets.Dust.Value, dustDrawPosition - Main.screenPosition, dust.frame, dust.color, dust.rotation, new Vector2(4f), dust.scale, SpriteEffects.None, 0f);
                                    dustDrawn++;
                                }

                                // Break if too many dust clones have been drawn
                                if (dustDrawn > maxShroomDust)
                                    break;
                            }
                        }
                    }
                }
            }

            bool noRogueStealth = calamityPlayer.rogueStealth == 0f || Player.townNPCs > 2f || !CalamityConfig.Instance.StealthInvisibility;
            if (calamityPlayer.rogueStealth > 0f && calamityPlayer.rogueStealthMax > 0f && Player.townNPCs < 3f && CalamityConfig.Instance.StealthInvisibility)
            {
                // A translucent orchid color, the rogue class color
                float colorValue = calamityPlayer.rogueStealth / calamityPlayer.rogueStealthMax * 0.9f; //0 to 0.9
                r *= 1f - (colorValue * 0.89f); //255 to 50
                g *= 1f - colorValue; //255 to 25
                b *= 1f - (colorValue * 0.89f); //255 to 50
                a *= 1f - colorValue; //255 to 25
                Player.armorEffectDrawOutlines = false;
                Player.armorEffectDrawShadow = false;
                Player.armorEffectDrawShadowSubtle = false;
            }

            if (calamityPlayer.tRegen)
            {
                if (Main.rand.NextBool(10) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, 107, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.75f;
                    Main.dust[dust].velocity.Y -= 0.35f;
                    drawInfo.DustCache.Add(dust);
                }
                if (noRogueStealth)
                {
                    r *= 0.025f;
                    g *= 0.15f;
                    b *= 0.035f;
                    fullBright = true;
                }
            }
            if (calamityPlayer.tracersDust)
            {
                if (!Player.StandingStill() && !Player.mount.Active)
                {
                    if (Main.rand.NextBool(2) && drawInfo.shadow == 0f)
                    {
                        int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, 229, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 0.5f;
                        drawInfo.DustCache.Add(dust);
                    }
                    if (noRogueStealth)
                    {
                        r *= 0.05f;
                        g *= 0.05f;
                        b *= 0.05f;
                        fullBright = true;
                    }
                }
            }
            if (calamityPlayer.elysianWingsDust)
            {
                if (!Player.StandingStill() && !Player.mount.Active)
                {
                    if (Main.rand.NextBool(2) && drawInfo.shadow == 0f)
                    {
                        int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, 246, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 0.5f;
                        drawInfo.DustCache.Add(dust);
                    }
                    if (noRogueStealth)
                    {
                        r *= 0.75f;
                        g *= 0.55f;
                        b *= 0f;
                        fullBright = true;
                    }
                }
            }
            if (calamityPlayer.dsSetBonus)
            {
                if (Player != null && !Player.dead)
                {
                    Lighting.AddLight((int)Player.Center.X / 16, (int)Player.Center.Y / 16, 100 / 235f, 1 / 235f, 250 / 235f);
                    if (!Player.StandingStill() && !Player.mount.Active)
                    {
                        if (Main.rand.NextBool(2) && drawInfo.shadow == 0f)
                        {
                            int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, 27, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1.5f);
                            Main.dust[dust].noGravity = true;
                            Main.dust[dust].velocity *= 0.5f;
                            drawInfo.DustCache.Add(dust);
                        }
                    }
                    if (noRogueStealth)
                    {
                        r *= 0.15f;
                        g *= 0.025f;
                        b *= 0.1f;
                        fullBright = true;
                    }
                }
            }
            if (calamityPlayer.auricSet)
            {
                if (Player != null && !Player.dead)
                {
                    Lighting.AddLight((int)Player.Center.X / 16, (int)Player.Center.Y / 16, 31 / 235f, 170 / 235f, 222 / 235f);
                    if (!Player.mount.Active)
                    {
                        if (Main.rand.NextBool(14) && drawInfo.shadow == 0f)
                        {
                            int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 8, Player.height + 8, 206, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1.5f);
                            Main.dust[dust].noGravity = true;
                            Main.dust[dust].velocity *= 1f;
                            drawInfo.DustCache.Add(dust);
                        }
                        if (noRogueStealth)
                        {
                            r *= 0f;
                            g *= 0.55f;
                            b *= 0.6f;
                            fullBright = true;
                        }
                    }
                    if (!Player.StandingStill() && !Player.mount.Active)
                    {
                        if (Main.rand.NextBool(8) && drawInfo.shadow == 0f)
                        {
                            int de_dust2 = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, Main.rand.NextBool(2) ? 204 : 213, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1.5f);
                            Main.dust[de_dust2].noGravity = true;
                            Main.dust[de_dust2].velocity *= 0.5f;
                            drawInfo.DustCache.Add(de_dust2);
                        }
                    }
                }
            }
            if (calamityPlayer.bFlames)
            {
                if (Main.rand.NextBool(abaddon ? 4 : 2) && drawInfo.shadow == 0f) //looks weaker if you have Abaddon equipped
                {
                    Dust dust = Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-20f, 20f)), Main.rand.NextBool(3) ? 114 : ModContent.DustType<BrimstoneFlame>(), new Vector2(0, Main.rand.NextFloat(-3f, -5f)) + Player.velocity, 0, default, abaddon ? 1.1f : 1.6f);
                    dust.noGravity = true;
                    for (int i = 0; i < 3; i++)
                    {
                        Dust dust2 = Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.NextFloat(-10f, 10f), 19), Main.rand.NextBool(2) ? 90 : ModContent.DustType<BrimstoneFlame>(), new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-1f, -3f)) + Player.velocity, 0, default, abaddon ? 0.4f : 1.4f);
                        dust2.noGravity = true;
                    }

                    if (noRogueStealth)
                    {
                        r *= 0.25f;
                        g *= 0.01f;
                        b *= 0.01f;
                        fullBright = true;
                    }
                }
            }
            if (calamityPlayer.shadowflame)
            {
                if (Main.rand.Next(5) < 4 && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, 27, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1.1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.75f;
                    Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.75f;
                    Main.dust[dust].velocity.Y = Main.dust[dust].velocity.Y - 3f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.3f;
                    }
                }
            }
            if (calamityPlayer.sulphurPoison)
            {
                if (Main.rand.Next(5) < 4 && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, 298, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 0.6f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.75f;
                    Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.75f;
                    Main.dust[dust].velocity.Y = Main.dust[dust].velocity.Y - 1f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.2f;
                    }
                    if (Main.rand.NextBool(5))
                    {
                        DirectionalPulseRing pulse = new DirectionalPulseRing(Player.Center + new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-20f, 20f)), new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-3f, -4f)), Main.rand.NextBool(2) ? Color.OliveDrab : Color.GreenYellow, new Vector2(0.8f, 1), 0, 0.09f, 0f, 45);
                        GeneralParticleHandler.SpawnParticle(pulse);
                    }
                }
                if (noRogueStealth)
                {
                    r *= 0.65f;
                    b *= 0.75f;
                    fullBright = true;
                }
            }

            // Dust while Rage Mode is active
            if (calamityPlayer.rageModeActive)
            {
                if (drawInfo.shadow == 0f)
                {
                    ragePulseTimer++;
                    int dustID = heartOfDarkness ? 240 : 114;

                    if (shatteredCommunity && Main.rand.NextBool(2))
                        dustID = 112; //special dust visual for Shattered Community

                    if (heartOfDarkness && !shatteredCommunity && Main.rand.NextBool(2))
                        dustID = 90; //special dust visual for Heart of Darkness

                    if (ragePulseTimer == 60)
                    {
                        PlayerCenteredPulseRing pulse = new PlayerCenteredPulseRing(Player, Vector2.Zero, Color.Red, new Vector2(1, 1), 0, 0f, 0.23f, 40);
                        GeneralParticleHandler.SpawnParticle(pulse);
                        ragePulse = true;
                    }

                    if (ragePulse)
                    {
                        ragePulseVisualTimer++;
                        if (ragePulseVisualTimer >= 30)
                        {
                            PlayerCenteredPulseRing pulse = new PlayerCenteredPulseRing(Player, Vector2.Zero, (shatteredCommunity ? Color.MediumPurple : Color.Red), new Vector2(1, 1), 0, 0f, 0.18f, 30);
                            GeneralParticleHandler.SpawnParticle(pulse);
                            ragePulseVisualTimer = 0;
                            ragePulse = false;
                            ragePulseTimer = 0;
                        }
                    }

                    Dust dust = Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-20f, 20f)), dustID);
                    dust.scale = Main.rand.NextFloat(0.3f, 0.45f);
                    if (dustID == 112)
                        dust.scale = Main.rand.NextFloat(0.7f, 0.8f);
                    if (dustID == 240)
                        dust.scale = Main.rand.NextFloat(0.8f, 0.95f);
                    dust.velocity = -Player.velocity / 3;
                    dust.noGravity = true;

                }
            }

            // Light and dust while Adrenaline Mode is active
            if (calamityPlayer.adrenalineModeActive)
            {
                // 23SEP2023: Ozzatron: Adrenaline emits light directly. Color lifted from AdrenDust
                Vector3 adrenDustLight = new Vector3(0.255f, 0.185f, 0.094f);
                Lighting.AddLight(Player.Center, adrenDustLight * 2);
                
                for (int i = 0; i < 4; i++)
                {
                    int dustID = ModContent.DustType<AdrenDust>();
                    Vector2 dustVel = Player.velocity * 0.5f;
                    int idx = Dust.NewDust(Main.rand.NextBool(5) ? drawInfo.Position : drawInfo.Position -Player.velocity * 1.5f, Player.width, Player.height, dustID, dustVel.X, dustVel.Y);
                    Dust d = Main.dust[idx];
                    d.scale = Main.rand.NextFloat(0.4f, 1.2f);
                    d.noGravity = true;
                    d.noLight = false;
                    drawInfo.DustCache.Add(idx);
                }
            }

            if (calamityPlayer.gsInferno)
            {
                if (Main.rand.NextBool(1) && drawInfo.shadow == 0f)
                {
                    SparkParticle spark = new SparkParticle(Player.Center + new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-20f, 20f)), new Vector2(0, Main.rand.NextFloat(-5f, 5f)), false, Main.rand.Next(11, 13), Main.rand.NextFloat(0.2f, 0.5f), Main.rand.NextBool(7) ? Color.Aqua : Color.Fuchsia);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                if (noRogueStealth)
                {
                    r *= 0.25f;
                    g *= 0.01f;
                    b *= 0.01f;
                    fullBright = true;
                }
            }
            if (calamityPlayer.astralInfection)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    DirectionalPulseRing pulse = new DirectionalPulseRing(Player.Center + new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-20f, 20f)), Vector2.Zero, Main.rand.NextBool(2) ? Color.DarkTurquoise : Color.Coral, new Vector2(1, 1), 0, 0.08f, 0f, 20);
                    GeneralParticleHandler.SpawnParticle(pulse);
                    Particle orb = new GenericBloom(Player.Center + new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-20f, 20f)), Vector2.Zero, Main.rand.NextBool(2) ? Color.DarkTurquoise : Color.Coral, 0.055f, 8);
                    GeneralParticleHandler.SpawnParticle(orb);
                }
            }
            if (calamityPlayer.hFlames || calamityPlayer.hInferno || calamityPlayer.banishingFire) //Sun stone makes it look weaker
            {
                if (Main.rand.NextBool(reducedHolyFlamesDamage && !calamityPlayer.hInferno ? 4 : 2) && drawInfo.shadow == 0f)
                {
                    Vector2 Vect = new Vector2(0f, Main.rand.NextBool(4) ? -5f : -9f).RotatedByRandom(MathHelper.ToRadians(25f)) * Main.rand.NextFloat(0.1f, 1.9f);
                    CritSpark spark = new CritSpark(Player.Center + new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-20f, 20f)), Vect, Main.rand.NextBool(2) ? Color.Coral : Color.OrangeRed, Color.Orange, (reducedHolyFlamesDamage && !calamityPlayer.hInferno ? 0.4f : 0.8f), 15, 2f, 1.9f);
                    GeneralParticleHandler.SpawnParticle(spark);
                }

                if (Main.rand.NextBool(reducedHolyFlamesDamage && !calamityPlayer.hInferno ? 4 : 2) && drawInfo.shadow == 0f)
                {
                    Vector2 dustCorner = Player.position - 2f * Vector2.One;
                    Vector2 dustVel = Player.velocity + new Vector2(0f, Main.rand.NextFloat(-5f, -1f));
                    int d = Dust.NewDust(dustCorner, Player.width + 4, Player.height + 4, 87, dustVel.X, dustVel.Y);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].scale = reducedHolyFlamesDamage && !calamityPlayer.hInferno ? Main.rand.NextFloat(0.3f, 0.5f) : Main.rand.NextFloat(0.7f, 1.2f);
                    Main.dust[d].alpha = 235;
                }
                if (noRogueStealth)
                {
                    r *= 0.25f;
                    g *= 0.25f;
                    b *= 0.1f;
                    fullBright = true;
                }
            }
            else if (calamityPlayer.icarusFolly)
            {
                if (Main.rand.NextBool(12) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, (int)CalamityDusts.ProfanedFire, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    drawInfo.DustCache.Add(dust);
                }
            }
            if (calamityPlayer.dragonFire)
            {
                for (int i = 0; i < 2; ++i)
                {
                    Vector2 Vect2 = new Vector2(0f, Main.rand.NextBool(4) ? -2f : -8f).RotatedByRandom(MathHelper.ToRadians(Main.rand.NextBool(3) ? 10 : 35f)) * Main.rand.NextFloat(0.1f, 1.9f);
                    SparkParticle spark = new SparkParticle(Player.Center + new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-20f, 20f)), new Vector2(Vect2.X - Player.velocity.X * 0.3f, Vect2.Y), false, 10, Main.rand.NextFloat(0.4f, 0.5f), Main.rand.NextBool(2) ? Color.OrangeRed : Color.Orange);
                    GeneralParticleHandler.SpawnParticle(spark);

                    if (Main.rand.NextBool(3))
                    {
                        Vector2 Vect = new Vector2(0f, Main.rand.NextBool(2) ? -3f : -14f).RotatedByRandom(MathHelper.ToRadians(25f)) * Main.rand.NextFloat(0.1f, 1.9f);
                        SmallSmokeParticle smoke = new SmallSmokeParticle(Player.Center + new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-20f, 20f)), Vect, Color.DimGray, Main.rand.NextBool(2) ? Color.Black : Color.DimGray, Main.rand.NextFloat(0.2f, 1.2f), 100);
                        GeneralParticleHandler.SpawnParticle(smoke);
                    }
                }
            }
            if (calamityPlayer.miracleBlight)
            {
                Color sparkColor;
                switch (Main.rand.Next(4))
                {
                    case 0:
                        sparkColor = Color.DarkRed;
                        break;
                    case 1:
                        sparkColor = Color.MediumTurquoise;
                        break;
                    case 2:
                        sparkColor = Color.Orange;
                        break;
                    default:
                        sparkColor = Color.LawnGreen;
                        break;
                }

                DirectionalPulseRing pulse = new DirectionalPulseRing(Player.Center + new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-20f, 20f)), new Vector2(5, 5).RotatedByRandom(360), sparkColor, new Vector2(Main.rand.NextFloat(1f, 5f), Main.rand.NextFloat(1f, 5f)), 0, Main.rand.NextFloat(0.07f, 0.1f), 0f, 18);
                GeneralParticleHandler.SpawnParticle(pulse);
                
                float numberOfDusts = 3f;
                float rotFactor = 360f / numberOfDusts;
                if (Player.miscCounter % 4 == 0)
                {
                    for (int i = 0; i < numberOfDusts; i++)
                    {
                        int DustType;
                        switch (Main.rand.Next(4))
                        {
                            case 0:
                                DustType = 219;
                                break;
                            case 1:
                                DustType = 220;
                                break;
                            case 2:
                                DustType = 226;
                                break;
                            default:
                                DustType = 222;
                                break;
                        }

                        float rot = MathHelper.ToRadians(i * rotFactor);
                        Vector2 offset = new Vector2(0.3f, 0).RotatedBy(rot * Main.rand.NextFloat(0.2f, 0.3f));
                        Vector2 velOffset = CalamityUtils.RandomVelocity(100f, 70f, 150f, 0.04f);
                        Dust dust = Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-20f, 20f)) + offset, DustType, new Vector2(velOffset.X, velOffset.Y));
                        dust.noGravity = true;
                        dust.velocity = velOffset;
                        velOffset *= 10;
                        dust.position = Player.Center - velOffset;
                        dust.scale = Main.rand.NextFloat(0.7f, 0.8f);
                    }
                }

            }
            if (calamityPlayer.pFlames) //Plague debuff
            {
                float numberOfDusts = 2f;
                float rotFactor = 360f / numberOfDusts;
                if (Player.miscCounter % 4 == 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        DirectionalPulseRing pulse = new DirectionalPulseRing(Player.Center + new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-20f, 20f)), Vector2.Zero, Main.rand.NextBool(3) ? Color.LimeGreen : Color.Green, new Vector2(1, 1), 0, Main.rand.NextFloat(0.07f, 0.18f), 0f, 35);
                        GeneralParticleHandler.SpawnParticle(pulse);
                    }

                    for (int i = 0; i < 7; i++)
                    {
                        int DustID = Main.rand.NextBool(30) ? 220 : 89;
                        float rot = MathHelper.ToRadians(i * rotFactor);
                        Vector2 offset = new Vector2(0.3f, 0).RotatedBy(rot * Main.rand.NextFloat(0.2f, 0.3f));
                        Dust dust2 = Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-20f, 20f)) + offset, DustID);
                        dust2.scale = Main.rand.NextFloat(0.3f, 0.4f);
                        if (DustID == 220)
                            dust2.scale = Main.rand.NextFloat(1f, 1.2f);
                    }
                }

                if (noRogueStealth)
                {
                    r *= 0.07f;
                    g *= 0.15f;
                    b *= 0.01f;
                    fullBright = true;
                }
            }
            if (calamityPlayer.nightwither) //Moon stone makes it look weaker
            {
                if (Main.rand.NextBool(reducedNightwitherDamage ? 4 : 2) && drawInfo.shadow == 0f)
                {
                    Vector2 Vect = new Vector2(0f, Main.rand.NextBool(4) ? -5f : -9f).RotatedByRandom(MathHelper.ToRadians(25f)) * Main.rand.NextFloat(0.1f, 1.9f);
                    CritSpark spark = new CritSpark(Player.Center + new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-20f, 20f)), Vect, Main.rand.NextBool(2) ? Color.Cyan : Color.DarkBlue, Color.DodgerBlue, (reducedNightwitherDamage ? 0.4f : 0.8f), 15, 2f, 1.9f);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                for (int i = 0; i < (reducedNightwitherDamage ? 1 : 2); i++)
                {
                    Vector2 dustCorner = Player.position - 2f * Vector2.One;
                    Vector2 dustVel = Player.velocity + new Vector2(0f, Main.rand.NextFloat(-11f, -2f));
                    int d = Dust.NewDust(dustCorner, Player.width + 4, Player.height + 4, Main.rand.NextBool(4) ? 160 : 206, dustVel.X, dustVel.Y);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].scale = reducedNightwitherDamage ? Main.rand.NextFloat(0.3f, 0.4f) : Main.rand.NextFloat(0.5f, 0.7f);
                    Main.dust[d].alpha = 235;
                }
                if (noRogueStealth)
                {
                    r *= 0.25f;
                    g *= 0.25f;
                    b *= 0.1f;
                    fullBright = true;
                }
            }
            if (calamityPlayer.vaporfied)
            {
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                246,
                242,
                229,
                226,
                247,
                187,
                234
                });
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, dustType, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                    drawInfo.DustCache.Add(dust);
                }
                if (noRogueStealth)
                {
                    r *= 0.25f;
                    g *= 0.25f;
                    b *= 0.1f;
                    fullBright = true;
                }
            }
            if (calamityPlayer.gState || calamityPlayer.cDepth || calamityPlayer.eutrophication)
            {
                if (Main.rand.NextBool(30) && cDepth)
                {
                    int bloodLifetime = Main.rand.Next(22, 36);
                    float bloodScale = Main.rand.NextFloat(0.6f, 0.8f);
                    Color bloodColor = Color.Lerp(Color.Red, Color.DarkRed, Main.rand.NextFloat());
                    bloodColor = Color.Lerp(bloodColor, new Color(51, 22, 94), Main.rand.NextFloat(0.65f));

                    if (Main.rand.NextBool(15))
                        bloodScale *= 1.3f;

                    float randomSpeedMultiplier = Main.rand.NextFloat(1.25f, 1.5f);
                    Vector2 bloodVelocity = Main.rand.NextVector2Unit() * 2 * randomSpeedMultiplier;
                    bloodVelocity.Y -= 5f;
                    BloodParticle blood = new BloodParticle(Player.Center, bloodVelocity, bloodLifetime, bloodScale, bloodColor);
                    GeneralParticleHandler.SpawnParticle(blood);
                }
                int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, Main.rand.NextBool(3) ? 104 : 186, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1.4f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.75f;
                Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.75f;
                Main.dust[dust].velocity.Y = Main.dust[dust].velocity.Y - 1f;
                if (Main.rand.NextBool(4) & cDepth)
                {
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].scale *= 0.2f;
                }
                if (noRogueStealth)
                {
                    r *= 0f;
                    g *= 0.05f;
                    b *= 0.3f;
                    fullBright = true;
                }
            }
            if (calamityPlayer.rTide)
            {
                if (Main.rand.NextBool(7) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, 165, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1f);
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].velocity *= 1.2f;
                    Main.dust[dust].velocity.Y += 0.8f;
                    drawInfo.DustCache.Add(dust);
                }
            }
            if (calamityPlayer.bBlood)
            {
                if (Main.rand.NextBool(11))
                {
                    int bloodLifetime = Main.rand.Next(22, 36);
                    float bloodScale = Main.rand.NextFloat(0.6f, 0.8f);
                    Color bloodColor = Color.Lerp(Color.Red, Color.DarkRed, Main.rand.NextFloat());
                    bloodColor = Color.Lerp(bloodColor, new Color(51, 22, 94), Main.rand.NextFloat(0.65f));

                    if (Main.rand.NextBool(15))
                        bloodScale *= 1.3f;

                    float randomSpeedMultiplier = Main.rand.NextFloat(1.25f, 1.5f);
                    Vector2 bloodVelocity = Main.rand.NextVector2Unit() * 2 * randomSpeedMultiplier;
                    bloodVelocity.Y -= 5f;
                    BloodParticle blood = new BloodParticle(Player.Center, bloodVelocity, bloodLifetime, bloodScale, bloodColor);
                    GeneralParticleHandler.SpawnParticle(blood);
                }
                for (int i = 0; i < 2; i++)
                {
                    float rot = MathHelper.ToRadians(i * 280);
                    Vector2 offset = new Vector2(0.1f, 0).RotatedBy(rot * Main.rand.NextFloat(0.08f, 0.05f));
                    Dust dust2 = Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-20f, 20f)) + offset, 5);
                    dust2.scale = Main.rand.NextFloat(0.6f, 0.7f);
                }
                if (noRogueStealth)
                {
                    r *= 0.15f;
                    g *= 0.01f;
                    b *= 0.01f;
                    fullBright = true;
                }
            }
            if (calamityPlayer.mushy)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    Dust dust = Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-20f, 20f)), 56, Vector2.Zero, 100, default, 0.9f);
                    dust.noGravity = true;
                    dust.velocity *= 0.5f;
                    dust.velocity.Y -= 0.1f;
                    dust.alpha = 200;
                }
                if (Main.rand.NextBool(15))
                {
                    Dust dust2 = Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.NextFloat(-10f, 10f), 19), Main.rand.NextBool(3) ? 41 : 56, new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, -2f)) + Player.velocity / 3, 0, default, 0.9f);
                    dust2.alpha = 145;
                }
                
                if (noRogueStealth)
                {
                    r *= 0.15f;
                    g *= 0.01f;
                    b *= 0.01f;
                    fullBright = true;
                }
            }
            if (calamityPlayer.PinkJellyRegen)
            {
                if (Main.rand.NextBool(24) && drawInfo.shadow == 0f)
                {
                    Particle Plus = new HealingPlus(Player.Center, Main.rand.NextFloat(0.5f, 1.2f), Color.HotPink, Color.LightPink, Main.rand.Next(10, 15));
                    GeneralParticleHandler.SpawnParticle(Plus);
                }
            }
            if (calamityPlayer.GreenJellyRegen)
            {
                if (Main.rand.NextBool(16) && drawInfo.shadow == 0f)
                {
                    Particle Plus = new HealingPlus(Player.Center, Main.rand.NextFloat(0.6f, 1.3f), Color.Lime, Color.LimeGreen, Main.rand.Next(10, 15));
                    GeneralParticleHandler.SpawnParticle(Plus);
                }
            }
            if (calamityPlayer.AbsorberRegen)
            {
                if (Main.rand.NextBool(11) && drawInfo.shadow == 0f)
                {
                    Particle Plus = new HealingPlus(Player.Center, Main.rand.NextFloat(0.7f, 1.4f), Color.DarkSeaGreen, Color.DarkSeaGreen, Main.rand.Next(10, 15));
                    GeneralParticleHandler.SpawnParticle(Plus);
                }
            }
            if (calamityPlayer.bloodfinBoost)
            {
                if (Main.rand.NextBool(3) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, Main.rand.NextBool(8) ? 296 : 5, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1.25f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.3f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    drawInfo.DustCache.Add(dust);
                }
                if (Main.rand.NextBool(16) && drawInfo.shadow == 0f)
                { 
                    Particle Plus = new HealingPlus(Player.Center - new Vector2(4, 0), Main.rand.NextFloat(0.4f, 0.8f), Color.Red, Color.DarkRed, Main.rand.Next(10, 15));
                    GeneralParticleHandler.SpawnParticle(Plus);    
                }    
                if (noRogueStealth)
                {
                    r *= 0.5f;
                    g *= 0f;
                    b *= 0f;
                    fullBright = true;
                }
            }
            if ((calamityPlayer.ladHearts > 0) && !Player.loveStruck && Main.netMode != NetmodeID.Server)
            {
                if (Main.rand.NextBool(5) && drawInfo.shadow == 0f)
                {
                    Vector2 velocity = Main.rand.NextVector2Unit();
                    velocity.X *= 0.66f;
                    velocity *= Main.rand.NextFloat(1f, 2f);

                    int heart = Gore.NewGore(Player.GetSource_FromThis(), drawInfo.Position + new Vector2(Main.rand.Next(Player.width + 1), Main.rand.Next(Player.height + 1)), velocity, 331, Main.rand.NextFloat(0.4f, 1.2f));
                    Main.gore[heart].sticky = false;
                    Main.gore[heart].velocity *= 0.4f;
                    Main.gore[heart].velocity.Y -= 0.6f;
                    drawInfo.GoreCache.Add(heart);
                }
            }
            if (calamityPlayer.shadow && CalamityConfig.Instance.StealthInvisibility)
            {
                r *= 0f;
                g *= 0f;
                b *= 0f;
            }
        }
        #endregion

        #region Profaned Moonlight Dye Colors

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
            int totalTime = Main.dayTime ? (int)Main.dayLength : (int)Main.nightLength;
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
        #endregion Profaned Moonlight Dye Colors

        #region Tanks/Backpacks
        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f)
                return;

            Player drawPlayer = drawInfo.drawPlayer;
            Item item = drawPlayer.ActiveItem();

            if (!drawPlayer.frozen &&
                (item.IsAir || item.type > ItemID.None) &&
                !drawPlayer.dead &&
                (!drawPlayer.wet || !item.noWet) &&
                (drawPlayer.wings == 0 || drawPlayer.velocity.Y == 0f))
            {
                //Make sure the lists are in the same order
                List<int> tankItems = new List<int>()
                {
                    ModContent.ItemType<FlurrystormCannon>(),
                    ModContent.ItemType<BlightSpewer>(),
                    ModContent.ItemType<HavocsBreath>(),
                    ModContent.ItemType<SparkSpreader>(),
                    ModContent.ItemType<HalleysInferno>(),
                    ModContent.ItemType<CleansingBlaze>(),
                    ModContent.ItemType<ElementalEruption>(),
                    ModContent.ItemType<DeadSunsWind>(),
                    ModContent.ItemType<Meowthrower>(),
                    ModContent.ItemType<OverloadedBlaster>(),
                    ModContent.ItemType<TerraFlameburster>(),
                    ModContent.ItemType<Photoviscerator>(),
                    ModContent.ItemType<Shadethrower>(),
                    ModContent.ItemType<BloodBoiler>(),
                    ModContent.ItemType<PristineFury>(),
                    ModContent.ItemType<AuroraBlazer>()
                };
                List<Texture2D> tankTextures = new List<Texture2D>()
                {
                    ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_FlurrystormCannon").Value,
                    ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_BlightSpewer").Value,
                    ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_HavocsBreath").Value,
                    ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_SparkSpreader").Value,
                    ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_HalleysInferno").Value,
                    ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_CleansingBlaze").Value,
                    ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_ElementalEruption").Value,
                    ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_TheEmpyrean").Value,
                    ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_Meowthrower").Value,
                    ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_OverloadedBlaster").Value,
                    ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_TerraFlameburster").Value,
                    ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_Photoviscerator").Value,
                    ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_Shadethrower").Value,
                    ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_BloodBoiler").Value,
                    ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_PristineFury").Value,
                    ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_AuroraBlazer").Value
                };
                if (tankItems.Contains(item.type) || drawPlayer.Calamity().plaguebringerCarapace)
                {
                    Texture2D thingToDraw = null;
                    if (tankItems.Contains(item.type))
                    {
                        for (int i = 0; i < tankItems.Count; i++)
                        {
                            if (item.type == tankItems[i])
                            {
                                thingToDraw = tankTextures[i];
                                break;
                            }
                        }
                    }
                    else if (drawPlayer.Calamity().plaguebringerCarapace)
                        thingToDraw = ModContent.Request<Texture2D>("CalamityMod/Items/Armor/Plaguebringer/PlaguebringerCarapace_Back").Value;

                    if (thingToDraw is null)
                        return;

                    SpriteEffects spriteEffects = Player.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                    float num25 = -4f;
                    float num24 = -8f;
                    DrawData howDoIDrawThings = new DrawData(thingToDraw,
                        new Vector2((int)(drawPlayer.position.X - Main.screenPosition.X + (drawPlayer.width / 2) - (9 * drawPlayer.direction)) + num25 * drawPlayer.direction, (int)(drawPlayer.position.Y - Main.screenPosition.Y + (drawPlayer.height / 2) + 2f * drawPlayer.gravDir + num24 * drawPlayer.gravDir)),
                        new Rectangle(0, 0, thingToDraw.Width, thingToDraw.Height),
                        drawInfo.colorArmorBody,
                        drawPlayer.bodyRotation,
                        new Vector2(thingToDraw.Width / 2, thingToDraw.Height / 2),
                        1f,
                        spriteEffects,
                        0);
                    howDoIDrawThings.shader = 0;
                    drawInfo.DrawDataCache.Add(howDoIDrawThings);
                }
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
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
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, ModContent.DustType<BrimstoneFlame>(), Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    drawInfo.DustCache.Add(dust);
                }
                if (noRogueStealth)
                {
                    r *= 0.25f;
                    g *= 0.01f;
                    b *= 0.01f;
                    fullBright = true;
                }
            }
            if (calamityPlayer.shadowflame)
            {
                if (Main.rand.Next(5) < 4 && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, 27, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1.95f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.75f;
                    Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.75f;
                    Main.dust[dust].velocity.Y = Main.dust[dust].velocity.Y - 1f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
            }
            if (calamityPlayer.sulphurPoison)
            {
                if (Main.rand.Next(5) < 4 && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, 46, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1.95f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.75f;
                    Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.75f;
                    Main.dust[dust].velocity.Y = Main.dust[dust].velocity.Y - 1f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
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
                    int dustCount = Main.rand.NextBool() ? 2 : 3;
                    for (int i = 0; i < dustCount; ++i)
                    {
                        int dustID = ModContent.DustType<BrimstoneFlame>();
                        if (shatteredCommunity && Main.rand.NextBool(3))
                            dustID = DustID.DemonTorch;

                        Vector2 dustVel = Player.velocity * 0.5f;
                        int idx = Dust.NewDust(drawInfo.Position, Player.width, Player.height, dustID, dustVel.X, dustVel.Y);
                        Dust d = Main.dust[idx];
                        d.fadeIn = 0.4f;
                        d.scale = Main.rand.NextFloat(0.8f, 1.5f);
                        d.noGravity = true;
                        d.noLight = false;
                        drawInfo.DustCache.Add(idx);
                    }
                }
            }

            // Dust while Adrenaline Mode is active
            if (calamityPlayer.adrenalineModeActive)
            {
                if (drawInfo.shadow == 0f)
                {
                    int dustID = 132;
                    Vector2 dustVel = Player.velocity * 0.5f;
                    int idx = Dust.NewDust(drawInfo.Position, Player.width, Player.height, dustID, dustVel.X, dustVel.Y);
                    Dust d = Main.dust[idx];
                    d.fadeIn = 1.1f;
                    d.scale = Main.rand.NextFloat(0.8f, 1.5f);
                    d.noGravity = Main.rand.NextBool(4);
                    d.noLight = false;
                    drawInfo.DustCache.Add(idx);
                }
            }

            if (calamityPlayer.gsInferno)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, 173, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    drawInfo.DustCache.Add(dust);
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
                    int dustType = Main.rand.NextBool(2) ? ModContent.DustType<AstralOrange>() : ModContent.DustType<AstralBlue>();
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, dustType, Player.velocity.X * 0.2f, Player.velocity.Y * 0.2f, 100, default, 0.7f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.2f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.dust[dust].color = new Color(255, 255, 255, 0);
                    drawInfo.DustCache.Add(dust);
                }
            }
            if (calamityPlayer.hFlames || calamityPlayer.hInferno || calamityPlayer.banishingFire)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, ModContent.DustType<HolyFireDust>(), Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
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
            if (calamityPlayer.absorberAffliction)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, 63, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, Color.DarkSeaGreen, 1.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
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
            else if (calamityPlayer.icarusFolly || calamityPlayer.dragonFire)
            {
                if (Main.rand.NextBool(calamityPlayer.dragonFire ? 6 : 12) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, (int)CalamityDusts.ProfanedFire, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    drawInfo.DustCache.Add(dust);
                }
            }
            if (calamityPlayer.miracleBlight)
            {
                // Violent dust fountain
                for (int i = 0; i < 2; ++i)
                {
                    Vector2 dustCorner = Player.position - 2f * Vector2.One;
                    Vector2 dustVel = Player.velocity + new Vector2(0f, Main.rand.NextFloat(-15f, -12f));
                    int d =  Dust.NewDust(dustCorner, Player.width + 4, Player.height + 4, (int)CalamityDusts.MiracleBlight, dustVel.X, dustVel.Y);
                    Main.dust[d].noGravity = true;
                    drawInfo.DustCache.Add(d);
                }
            }
            if (calamityPlayer.pFlames)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, 89, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.2f;
                    Main.dust[dust].velocity.Y -= 0.15f;
                    drawInfo.DustCache.Add(dust);
                }
                if (noRogueStealth)
                {
                    r *= 0.07f;
                    g *= 0.15f;
                    b *= 0.01f;
                    fullBright = true;
                }
            }
            if (calamityPlayer.nightwither)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, 176, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
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
                if (noRogueStealth)
                {
                    r *= 0f;
                    g *= 0.05f;
                    b *= 0.3f;
                    fullBright = true;
                }
            }
            if (calamityPlayer.bBlood)
            {
                if (Main.rand.NextBool(6) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, 5, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    drawInfo.DustCache.Add(dust);
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
                if (Main.rand.NextBool(6) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, 56, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 2f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.5f;
                    Main.dust[dust].velocity.Y -= 0.1f;
                    drawInfo.DustCache.Add(dust);
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
                if (Main.rand.NextBool(6) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, 5, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    drawInfo.DustCache.Add(dust);
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
                    ModContent.ItemType<GodsBellows>(),
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

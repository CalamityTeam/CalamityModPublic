using System;
using System.Collections.Generic;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer.DrawLayers;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer
{
    public partial class CalamityPlayer : ModPlayer
    {
        internal Vector2 RandomDebuffVisualSpot => Player.Center + new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-20f, 20f));

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

            // TODO -- rogue stealth visuals are an utter catastrophe and should be fully destroyed on next stealth rework
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

            #region Flight Visuals
            // Celestial Tracers, Elysian Tracers, Seraph Tracers
            if (calamityPlayer.tracersDust && drawInfo.shadow == 0f)
            {
                if (!Player.StandingStill() && !Player.mount.Active)
                {
                    if (Main.rand.NextBool())
                    {
                        int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, 229, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 0.5f;
                        drawInfo.DustCache.Add(dust);
                    }
                }
            }

            // Elysian Wings, Elysian TRACERS?! and SERAPH TRACERS?!
            if (calamityPlayer.elysianWingsDust && drawInfo.shadow == 0f)
            {
                if (!Player.StandingStill() && !Player.mount.Active)
                {
                    if (Main.rand.NextBool())
                    {
                        int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, 246, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 0.5f;
                        drawInfo.DustCache.Add(dust);
                    }
                }
            }
            #endregion

            #region Armor Visuals
            // Demonshade Armor
            if (calamityPlayer.dsSetBonus && drawInfo.shadow == 0f)
            {
                if (Player != null && !Player.dead)
                {
                    Lighting.AddLight((int)Player.Center.X / 16, (int)Player.Center.Y / 16, 100 / 235f, 1 / 235f, 250 / 235f);
                    if (!Player.StandingStill() && !Player.mount.Active)
                    {
                        if (Main.rand.NextBool())
                        {
                            int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, 27, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1.5f);
                            Main.dust[dust].noGravity = true;
                            Main.dust[dust].velocity *= 0.5f;
                            drawInfo.DustCache.Add(dust);
                        }
                    }
                }
            }

            // Auric Armor
            else if (calamityPlayer.auricSet && drawInfo.shadow == 0f)
            {
                if (Player != null && !Player.dead)
                {
                    Lighting.AddLight((int)Player.Center.X / 16, (int)Player.Center.Y / 16, 31 / 235f, 170 / 235f, 222 / 235f);
                    if (!Player.mount.Active)
                    {
                        if (Main.rand.NextBool(14))
                        {
                            int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 8, Player.height + 8, 206, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1.5f);
                            Main.dust[dust].noGravity = true;
                            Main.dust[dust].velocity *= 1f;
                            drawInfo.DustCache.Add(dust);
                        }
                    }
                    if (!Player.StandingStill() && !Player.mount.Active)
                    {
                        if (Main.rand.NextBool(8))
                        {
                            int de_dust2 = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, Main.rand.NextBool() ? 204 : 213, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1.5f);
                            Main.dust[de_dust2].noGravity = true;
                            Main.dust[de_dust2].velocity *= 0.5f;
                            drawInfo.DustCache.Add(de_dust2);
                        }
                    }
                }
            }
            #endregion

            #region Ripper Visuals
            if (calamityPlayer.rageModeActive && drawInfo.shadow == 0f)
                RageMode.DrawEffects(drawInfo);

            if (calamityPlayer.adrenalineModeActive && drawInfo.shadow == 0f)
                AdrenalineMode.DrawEffects(drawInfo);
            #endregion

            #region Buff and Debuff Visuals
            // Buff and debuff visuals. Alphabetical order as per usual, please

            if (calamityPlayer.astralInfection && drawInfo.shadow == 0f)
                AstralInfectionDebuff.DrawEffects(drawInfo);

            if (calamityPlayer.bBlood && drawInfo.shadow == 0f)
                BurningBlood.DrawEffects(drawInfo);

            if (calamityPlayer.bFlames && drawInfo.shadow == 0f)
            {
                bool resistsBrimstoneFlames = abaddon; // Looks weaker if you have Abaddon equipped
                BrimstoneFlames.DrawEffects(drawInfo, resistsBrimstoneFlames);
            }

            if (calamityPlayer.brainRot && drawInfo.shadow == 0f)
                BrainRot.DrawEffects(drawInfo);

            if (calamityPlayer.cDepth && drawInfo.shadow == 0f)
                CrushDepth.DrawEffects(drawInfo);

            if (calamityPlayer.dragonFire && drawInfo.shadow == 0f)
                Dragonfire.DrawEffects(drawInfo);

            if (calamityPlayer.elementalMix && drawInfo.shadow == 0f)
                ElementalMix.DrawEffects(drawInfo);

            if (calamityPlayer.eutrophication && drawInfo.shadow == 0f)
                Eutrophication.DrawEffects(drawInfo);

            if (calamityPlayer.gState && drawInfo.shadow == 0f)
            {
                // These lines cannot be moved to Glacial State's own file
                r *= 0.13f;
                g *= 0.66f;

                GlacialState.DrawEffects(drawInfo);
            }

            if (calamityPlayer.gsInferno && drawInfo.shadow == 0f)
                GodSlayerInferno.DrawEffects(drawInfo);

            // Holy Flames, Holy Inferno and Banishing Fire share the same visual effects
            if (drawInfo.shadow == 0f && (calamityPlayer.hFlames || calamityPlayer.hInferno || calamityPlayer.banishingFire))
            {
                // You cannot "resist" Holy Inferno or Banishing Fire, so if you have either of those the visuals are always full strength
                bool resistsHolyFlames = !calamityPlayer.hInferno && !calamityPlayer.banishingFire && reducedHolyFlamesDamage;
                HolyFlames.DrawEffects(drawInfo, resistsHolyFlames);
            }

            // Icarus' Folly has visual effects but they are mutually exclusive with all Holy Flames variations to prevent visual clutter
            else if (calamityPlayer.icarusFolly && drawInfo.shadow == 0f)
                IcarusFolly.DrawEffects(drawInfo);

            if (calamityPlayer.miracleBlight && drawInfo.shadow == 0f)
                MiracleBlight.DrawEffects(drawInfo);

            // Mushy buff from Crabulon and Crabulon accessories
            if (calamityPlayer.mushy && drawInfo.shadow == 0f)
                Mushy.DrawEffects(drawInfo);

            if (calamityPlayer.nightwither && drawInfo.shadow == 0f) // Looks weaker if you have Moon Stone equipped
                Nightwither.DrawEffects(drawInfo, reducedNightwitherDamage);

            if (calamityPlayer.pFlames && drawInfo.shadow == 0f)
                Plague.DrawEffects(drawInfo);

            if (calamityPlayer.rTide && drawInfo.shadow == 0f)
                RiptideDebuff.DrawEffects(drawInfo);

            if (calamityPlayer.shadowflame && drawInfo.shadow == 0f)
                Shadowflame.DrawEffects(drawInfo);

            if (calamityPlayer.sulphurPoison && drawInfo.shadow == 0f)
                SulphuricPoisoning.DrawEffects(drawInfo);

            // Tarragon life regen
            if (calamityPlayer.tRegen && drawInfo.shadow == 0f)
                TarraLifeRegen.DrawEffects(drawInfo);

            if (calamityPlayer.vaporfied && drawInfo.shadow == 0f)
                Vaporfied.DrawEffects(drawInfo);
            #endregion

            if (calamityPlayer.PinkJellyRegen && drawInfo.shadow == 0f)
            {
                if (Main.rand.NextBool(24))
                {
                    Particle Plus = new HealingPlus(Player.Center, Main.rand.NextFloat(0.5f, 1.2f), new Vector2(0, Main.rand.NextFloat(-2f, -3.5f)) + Player.velocity, Color.HotPink, Color.LightPink, Main.rand.Next(10, 15));
                    GeneralParticleHandler.SpawnParticle(Plus);
                }
            }
            if (calamityPlayer.GreenJellyRegen && drawInfo.shadow == 0f)
            {
                if (Main.rand.NextBool(16))
                {
                    Particle Plus = new HealingPlus(Player.Center, Main.rand.NextFloat(0.6f, 1.3f), new Vector2(0, Main.rand.NextFloat(-2f, -3.5f)) + Player.velocity, Color.Lime, Color.LimeGreen, Main.rand.Next(10, 15));
                    GeneralParticleHandler.SpawnParticle(Plus);
                }
            }
            if (calamityPlayer.AbsorberRegen && drawInfo.shadow == 0f)
            {
                if (Main.rand.NextBool(11))
                {
                    Particle Plus = new HealingPlus(Player.Center, Main.rand.NextFloat(0.7f, 1.4f), new Vector2(0, Main.rand.NextFloat(-2f, -3.5f)) + Player.velocity, Color.DarkSeaGreen, Color.DarkSeaGreen, Main.rand.Next(10, 15));
                    GeneralParticleHandler.SpawnParticle(Plus);
                }
            }
            if (calamityPlayer.bloodfinBoost && drawInfo.shadow == 0f)
            {
                if (Main.rand.NextBool(3))
                {
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, Main.rand.NextBool(8) ? 296 : 5, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1.25f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.3f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    drawInfo.DustCache.Add(dust);
                }
                if (Main.rand.NextBool(16))
                {
                    Particle Plus = new HealingPlus(Player.Center - new Vector2(4, 0), Main.rand.NextFloat(0.4f, 0.8f), new Vector2(0, Main.rand.NextFloat(-2f, -3.5f)) + Player.velocity, Color.Red, Color.DarkRed, Main.rand.Next(10, 15));
                    GeneralParticleHandler.SpawnParticle(Plus);
                }
            }

            // Some extraneous and probably undocumented visual effect caused by the heart lad pet thing
            if ((calamityPlayer.ladHearts > 0) && !Player.loveStruck && Main.netMode != NetmodeID.Server && drawInfo.shadow == 0f)
            {
                if (Main.rand.NextBool(5))
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

                    DrawData howDoIDrawThings = new DrawData(thingToDraw,
                        new Vector2((int)(drawPlayer.position.X - Main.screenPosition.X + (drawPlayer.width / 2) - (9 * drawPlayer.direction)) - 4f * drawPlayer.direction, (int)(drawPlayer.position.Y - Main.screenPosition.Y + (drawPlayer.height / 2) + 2f * drawPlayer.gravDir - 8f * drawPlayer.gravDir)),
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

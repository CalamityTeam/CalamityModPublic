using System;
using System.Collections.Generic;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer.DrawLayers;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Systems.Collections;
using CalamityMod.Systems.Graphic.PixelationSystem;
using CalamityMod.Utilities.Daybreak;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
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
            if (LegOverrideList.Includes(Player.legs))
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

            if (Starshield > 0 && drawInfo.shadow == 0)
            {
                var color = Color.Lerp(Color.DeepSkyBlue, Color.LightSkyBlue, (StratusStarburst / (float)MaxStratusStarburst));
                var opacity = MathHelper.Min(MathHelper.Min(Starshield / 30f, 1f), (CalamityUtils.MinutesToFrames(10) - Starshield) / 30f);
                float size = 80 + 32 * (StratusStarburst / (float)MaxStratusStarburst);

                Vector2 drawPosition = Player.Center + new Vector2(0, Player.gfxOffY) - Main.screenPosition;

                PixelationManager.AddPixelatedDrawer(drawLayer: Enums.GeneralDrawLayer.AfterProjectiles, drawAction: (matrix) =>
                {
                    #region AoE
                    //Draw the bloom circle

                    Texture2D telegraphBase = StratusBlackHole.GetTransparentBloomTex();

                    //Draw the inner particles
                    Main.spriteBatch.EnterShaderRegion(matrix: matrix);
                    GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].UseOpacity(0.5f);
                    GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].UseSaturation(0.2f);
                    GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/MeltyNoiseHighContrast"), 1);
                    GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].Apply();
                    Main.EntitySpriteDraw(telegraphBase, drawPosition, null, Color.DarkSlateBlue * opacity, 0, telegraphBase.Size() / 2f, size * 1.5f * opacity / telegraphBase.Width, 0, 0);

                    //Draw the outer particles
                    Main.spriteBatch.EnterShaderRegion(matrix: matrix);
                    GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].UseOpacity(0.25f);
                    GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].UseSaturation(0.1f);
                    GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Neurons"), 1);
                    GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].Apply();
                    telegraphBase = ModContent.Request<Texture2D>("CalamityMod/Particles/HighResFoggyCircleHardEdge").Value;
                    Main.EntitySpriteDraw(telegraphBase, drawPosition, null, Color.SkyBlue * opacity, 0, telegraphBase.Size() / 2f, size * opacity / telegraphBase.Width, 0, 0);
                    Main.spriteBatch.ExitShaderRegion(matrix);
                    #endregion
                });
            }
            //DoG Boss Cursor
            DevourerofGodsHead DoG = null;
            foreach (var item in Main.ActiveNPCs)
            {
                if (item.type == ModContent.NPCType<DevourerofGodsHead>())
                {
                    DoG = item.ModNPC<DevourerofGodsHead>();
                    break;
                }
            }
            if (DoG != null && Main.mapStyle != 2 && !Main.hideUI)
            {
                int drawCount = calamityPlayer.trippy ? 4 : 1;

                for (int i = 0; i < drawCount; i++)
                {
                    var rift = DoG.GetRiftLocation();
                    bool drawingRift = rift != Vector2.Zero && !Main.zenithWorld;

                    Vector2 targetCenter = drawingRift ? rift : DoG.NPC.Center;
                    float diffX = targetCenter.X - Player.Center.X;
                    float diffY = targetCenter.Y - Player.Center.Y;

                    // Mirror the offset based on i's index
                    if (calamityPlayer.trippy)
                    {
                        switch (i)
                        {
                            case 0: diffX = -Math.Abs(diffX); diffY = -Math.Abs(diffY); break; // Top Left
                            case 1: diffX = Math.Abs(diffX); diffY = -Math.Abs(diffY); break; // Top Right
                            case 2: diffX = Math.Abs(diffX); diffY = Math.Abs(diffY); break; // Bottom Right
                            case 3: diffX = -Math.Abs(diffX); diffY = Math.Abs(diffY); break; // Bottom Left
                        }
                    }

                    Vector2 virtualTargetPos = Player.Center + new Vector2(diffX, diffY);
                    float dist = Player.Distance(virtualTargetPos);
                    Vector2 directionToTarget = Player.DirectionTo(virtualTargetPos);

                    if (drawingRift)
                    {
                        var tex = ModContent.Request<Texture2D>("Terraria/Images/Extra_173").Value;
                        float opacity = 0.9f * Math.Clamp(MathHelper.Lerp(0, 1, (dist - 300) / 600), 0, 1);

                        Color drawColor = calamityPlayer.trippy ? Main.DiscoColor : Color.White * 0.9f;

                        Main.spriteBatch.Draw(tex, Player.Center + directionToTarget * 196 * Math.Min(dist / 2400f, 2) - Main.screenPosition,
                            null, drawColor * opacity, 0, tex.Size() / 2f, 0.9f, SpriteEffects.FlipHorizontally, 0);
                    }
                    else
                    {
                        var dis = Player.Distance(virtualTargetPos);
                        if ((DoG.NPC.ai[3] < 3 || !DoG.Phase2Started) && DoG.NPC.Opacity > 0.5f && !DoG.Dying && !drawInfo.drawPlayer.isDisplayDollOrInanimate)
                        {
                            int headIconIndex = -1;
                            DoG.BossHeadSlot(ref headIconIndex);
                            if (headIconIndex > -1)
                            {
                                var tex = TextureAssets.NpcHeadBoss[headIconIndex].Value;

                                float baseRotation = DoG.NPC.rotation;

                                if (calamityPlayer.trippy)
                                {
                                    Vector2 rotVec = baseRotation.ToRotationVector2();
                                    switch (i)
                                    {
                                        case 0:
                                            rotVec.X *= -1;
                                            rotVec.Y *= -1;
                                            break;
                                        case 1:
                                            rotVec.Y *= -1;
                                            break;
                                        case 2:
                                            break;
                                        case 3:
                                            rotVec.X *= -1;
                                            break;
                                    }
                                    baseRotation = rotVec.ToRotation();
                                }

                                float opacity = 0.9f * Math.Clamp(MathHelper.Lerp(0, 1, (dis - 600) / 300), 0, 1);
                                Color drawColor = calamityPlayer.trippy ? new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, (int)(255 * 0.9f)) : Color.White * 0.9f;

                                Main.spriteBatch.Draw(tex, Player.Center + directionToTarget * 196 * Math.Min(dis / 2400f, 2) - Main.screenPosition, null, drawColor * opacity, baseRotation, tex.Size() / 2f, 1, SpriteEffects.None, 0);
                            }
                        }
                    }
                }
            }

            //Charge animation for Thread of Eradication
            if (Player.HeldItem.type == ModContent.ItemType<ThreadOfEradication>() && !Player.ItemTimeIsZero && drawInfo.shadow == 0f)
            {
                var color = Color.Fuchsia;
                float scale = (1 - (Player.itemTime - 7) / 50f) * 0.2f;
                if (Player.itemTime < 7)
                {
                    scale = ((Player.itemTime) / 7f) * 0.2f;
                }
                if (Player.itemTime > 60)
                {
                    scale = (1 - (Player.itemTime - 70) / 110f) * 0.5f;
                    if (Player.itemTime < 70)
                    {
                        scale = ((Player.itemTime - 60) / 10f) * 0.5f;
                    }
                    color = Color.Cyan;
                }

                if (CalamityClientConfig.Instance.Photosensitivity)
                    color = color * 0.2f;
                var bloomTex = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
                var circleTex = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/BasicCircle").Value;
                using (Main.spriteBatch.Scope())
                {
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                    Main.spriteBatch.Draw(bloomTex, Player.Center + (Vector2.UnitX * Player.direction).RotatedBy(Player.itemRotation) * (48 + scale * 96) - Main.screenPosition, null, color, 0, bloomTex.Size() * 0.5f, scale * 2.0f, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(bloomTex, Player.Center + (Vector2.UnitX * Player.direction).RotatedBy(Player.itemRotation) * (48 + scale * 96) - Main.screenPosition, null, color, 0, bloomTex.Size() * 0.5f, scale * 2.0f, SpriteEffects.None, 0);
                    Main.spriteBatch.End();
                }
                for (var i = 0; i < 5; i++)
                    Main.spriteBatch.Draw(circleTex, Player.Center + (Vector2.UnitX * Player.direction).RotatedBy(Player.itemRotation) * (48 + scale * 96) - Main.screenPosition, null, Color.Black * ((i + 1) / 5f) * (CalamityClientConfig.Instance.Photosensitivity ? 0.2f : 1f), 0, circleTex.Size() * 0.5f, scale * 2.2f * (0.5f + 0.5f * (1 - (i) / 5f)), SpriteEffects.None, 0);
            }

            // Drawing for Odd Mushroom's clone effects
            if (calamityPlayer.trippy)
            {
                if (Main.myPlayer == Player.whoAmI)
                {
                    // Dust
                    Rectangle screenArea = new Rectangle((int)Main.screenPosition.X - 500, (int)Main.screenPosition.Y - 50, Main.screenWidth + 1000, Main.screenHeight + 100);
                    int dustDrawn = 0;
                    float maxShroomDust = Main.maxDustToDraw / 2;
                    Color shroomColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, Main.DiscoR);
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

                    // NPCs
                    foreach (NPC n in Main.ActiveNPCs)
                    {
                        Color rainbow = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, Main.DiscoR);
                        Color alphaColor = n.GetAlpha(rainbow);
                        float RGBMult = 0.99f;
                        alphaColor.R = (byte)(alphaColor.R * RGBMult);
                        alphaColor.G = (byte)(alphaColor.G * RGBMult);
                        alphaColor.B = (byte)(alphaColor.B * RGBMult);
                        alphaColor.A = (byte)(alphaColor.A * RGBMult);

                        for (int i = 0; i < 4; i++)
                        {
                            Vector2 position = n.position;
                            float distanceFromTargetX = Math.Abs(n.Center.X - Main.LocalPlayer.Center.X);
                            float distanceFromTargetY = Math.Abs(n.Center.Y - Main.LocalPlayer.Center.Y);

                            switch (i)
                            {
                                case 0:
                                    position.X = Main.LocalPlayer.Center.X - distanceFromTargetX;
                                    position.Y = Main.LocalPlayer.Center.Y - distanceFromTargetY;
                                    break;

                                case 1:
                                    position.X = Main.LocalPlayer.Center.X + distanceFromTargetX;
                                    position.Y = Main.LocalPlayer.Center.Y - distanceFromTargetY;
                                    break;

                                case 2:
                                    position.X = Main.LocalPlayer.Center.X + distanceFromTargetX;
                                    position.Y = Main.LocalPlayer.Center.Y + distanceFromTargetY;
                                    break;

                                case 3:
                                    position.X = Main.LocalPlayer.Center.X - distanceFromTargetX;
                                    position.Y = Main.LocalPlayer.Center.Y + distanceFromTargetY;
                                    break;

                                default:
                                    break;
                            }

                            Vector2 posDiff = n.Center - position;
                            Main.instance.DrawNPCDirect(Main.spriteBatch, n, n.behindTiles, Main.screenPosition + posDiff);
                        }
                    }

                    // Projectiles
                    foreach (Projectile p in Main.ActiveProjectiles)
                    {
                        Texture2D texture = TextureAssets.Projectile[p.type].Value;
                        Color rainbow = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, Main.DiscoR);
                        Color alphaColor = p.GetAlpha(rainbow);
                        float RGBMult = 0.99f;
                        alphaColor.R = (byte)(alphaColor.R * RGBMult);
                        alphaColor.G = (byte)(alphaColor.G * RGBMult);
                        alphaColor.B = (byte)(alphaColor.B * RGBMult);
                        alphaColor.A = (byte)(alphaColor.A * RGBMult);

                        Vector2 storedProjPos = p.Center;
                        for (int i = 0; i < 4; i++)
                        {
                            Vector2 position = p.position;
                            float distanceFromTargetX = Math.Abs(p.Center.X - Main.LocalPlayer.Center.X);
                            float distanceFromTargetY = Math.Abs(p.Center.Y - Main.LocalPlayer.Center.Y);

                            switch (i)
                            {
                                case 0:
                                    position.X = Main.LocalPlayer.Center.X - distanceFromTargetX;
                                    position.Y = Main.LocalPlayer.Center.Y - distanceFromTargetY;
                                    break;

                                case 1:
                                    position.X = Main.LocalPlayer.Center.X + distanceFromTargetX;
                                    position.Y = Main.LocalPlayer.Center.Y - distanceFromTargetY;
                                    break;

                                case 2:
                                    position.X = Main.LocalPlayer.Center.X + distanceFromTargetX;
                                    position.Y = Main.LocalPlayer.Center.Y + distanceFromTargetY;
                                    break;

                                case 3:
                                    position.X = Main.LocalPlayer.Center.X - distanceFromTargetX;
                                    position.Y = Main.LocalPlayer.Center.Y + distanceFromTargetY;
                                    break;

                                default:
                                    break;
                            }

                            // Unfortunately unlike NPCs, there is no public function for drawing a projectile with a position parameter to spoof.
                            // So we have to spoof it by directly changing the projectile's position, then resetting it at the end.
                            p.Center = position;
                            Main.instance.DrawProjDirect(p);
                        }

                        p.Center = storedProjPos;
                    }
                }
            }
            else
            {
                // Mana Burn VFX disabled when hih
                if (Player.statMana < 0 && Player.Calamity().ChaosStone)
                {
                    float compactness = Player.width * 0.6f;
                    if (compactness < 10f)
                        compactness = 10f;
                    float power = Player.height / 100f;
                    if (power > 2.75f)
                        power = 2.75f;
                    var color = Color.Blue;
                    if (ManaBurnFireDrawer is null || ManaBurnFireDrawer.LocalTimer >= ManaBurnFireDrawer.SetLifetime)
                        ManaBurnFireDrawer = new FireParticleSet(60 - (Player.statMana / 4), 1, color * 1.25f, color, compactness, power);
                    else
                        ManaBurnFireDrawer.DrawSet(Player.Bottom - Vector2.UnitY * (12f - Player.gfxOffY));
                }
                else
                    ManaBurnFireDrawer = null;
            }

            // TODO -- rogue stealth visuals are an utter catastrophe and should be fully destroyed on next stealth rework
            if (calamityPlayer.rogueStealth > 0f && calamityPlayer.rogueStealthMax > 0f && Player.townNPCs < 3f && CalamityClientConfig.Instance.StealthInvisibility)
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
            // Moon Walkers, Void Striders, Seraph Tracers
            if (calamityPlayer.tracersDust && drawInfo.shadow == 0f)
            {
                if (!Player.StandingStill() && !Player.mount.Active)
                {
                    if (Main.rand.NextBool())
                    {
                        Dust dust = Dust.NewDustDirect(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, DustID.Vortex, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1f);
                        dust.noGravity = true;
                        dust.velocity *= 0.5f;
                        drawInfo.DustCache.Add(dust.dustIndex);
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
                            Dust dust = Dust.NewDustDirect(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, DustID.Shadowflame, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1.5f);
                            dust.noGravity = true;
                            dust.velocity *= 0.5f;
                            drawInfo.DustCache.Add(dust.dustIndex);
                        }
                    }
                }
            }

            // Auric Armor
            else if (calamityPlayer.auricSet && drawInfo.shadow == 0f)
            {
                if (Player != null && !Player.dead)
                {
                    Lighting.AddLight(Player.Center, Color.Lerp(Color.Cyan, Color.White, 0.7f).ToVector3());
                    if (!Player.StandingStill() && !Player.mount.Active)
                    {
                        if (Main.rand.NextBool())
                        {
                            Vector2 velocity = -Player.velocity.SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(1, 3);
                            Particle nanoDust = new NanoParticle(drawInfo.Position + new Vector2(Main.rand.Next(Player.width + 1), Main.rand.Next(Player.height + 1)), velocity, (Main.rand.NextBool(3) ? Color.RoyalBlue : Color.Cyan) * 0.9f, Main.rand.NextFloat(0.2f, 0.7f), 9, false, true);
                            GeneralParticleHandler.SpawnParticle(nanoDust);
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

            if (calamityPlayer.auricRebuke && drawInfo.shadow == 0f)
                AuricRebuke.DrawEffects(drawInfo);

            if (calamityPlayer.burningBlood && drawInfo.shadow == 0f)
                BurningBlood.DrawEffects(drawInfo);

            if (calamityPlayer.brimstoneFlames && drawInfo.shadow == 0f)
            {
                BrimstoneFlames.DrawEffects(drawInfo);
            }

            if (calamityPlayer.brainRot && drawInfo.shadow == 0f)
                BrainRot.DrawEffects(drawInfo);

            if (calamityPlayer.crushDepth && drawInfo.shadow == 0f)
                CrushDepth.DrawEffects(drawInfo);

            if (calamityPlayer.daybroken && drawInfo.shadow == 0f)
                Daybroken.DrawEffects(drawInfo);

            if (calamityPlayer.demonicFlames && drawInfo.shadow == 0f)
                DemonicFlames.DrawEffects(drawInfo);

            if (calamityPlayer.dragonFire && drawInfo.shadow == 0f)
                Dragonfire.DrawEffects(drawInfo);

            if (calamityPlayer.elementalMix && drawInfo.shadow == 0f)
                ElementalMix.DrawEffects(drawInfo);

            if (calamityPlayer.eutrophication && drawInfo.shadow == 0f)
                Eutrophication.DrawEffects(drawInfo);

            if (calamityPlayer.godSlayerInferno && drawInfo.shadow == 0f)
                GodSlayerInferno.DrawEffects(drawInfo);

            if (calamityPlayer.heavybleeding && drawInfo.shadow == 0f)
                HeavyBleeding.DrawEffects(drawInfo);

            // Holy Flames, Holy Inferno and Banishing Fire share the same visual effects
            if (drawInfo.shadow == 0f && (calamityPlayer.holyFlames || calamityPlayer.holyInferno || calamityPlayer.banishingFire))
                HolyFlames.DrawEffects(drawInfo);

            if (calamityPlayer.hadopelagicPressure && drawInfo.shadow == 0f)
                HadopelagicPressure.DrawEffects(drawInfo);

            // Icarus' Folly has visual effects but they are mutually exclusive with all Holy Flames variations to prevent visual clutter
            else if (calamityPlayer.icarusFolly && drawInfo.shadow == 0f)
                IcarusFolly.DrawEffects(drawInfo);

            if (calamityPlayer.laceration && drawInfo.shadow == 0f)
                Laceration.DrawEffects(drawInfo);

            if (calamityPlayer.miracleBlight && drawInfo.shadow == 0f)
                MiracleBlight.DrawEffects(drawInfo);

            // Mushy buff from Crabulon and Crabulon accessories
            if (calamityPlayer.mushy && drawInfo.shadow == 0f)
                Mushy.DrawEffects(drawInfo);

            if (calamityPlayer.nightwither && drawInfo.shadow == 0f)
                Nightwither.DrawEffects(drawInfo);

            if (calamityPlayer.plague && drawInfo.shadow == 0f)
                Plague.DrawEffects(drawInfo);

            if (calamityPlayer.riptide && drawInfo.shadow == 0f)
                RiptideDebuff.DrawEffects(drawInfo);

            if (calamityPlayer.shadowflame && drawInfo.shadow == 0f)
                Shadowflame.DrawEffects(drawInfo);

            if (calamityPlayer.staticDischarge && drawInfo.shadow == 0f)
                StaticDischarge.DrawEffects(drawInfo);

            if (calamityPlayer.sulphurPoison && drawInfo.shadow == 0f)
                SulphuricPoisoning.DrawEffects(drawInfo);

            // Tarragon life regen
            if (calamityPlayer.tRegen && drawInfo.shadow == 0f)
                TarraLifeRegen.DrawEffects(drawInfo);

            if (calamityPlayer.trueVHex && drawInfo.shadow == 0f)
                TrueVulnerabilityHex.DrawEffects(drawInfo);

            if (calamityPlayer.vaporfied && drawInfo.shadow == 0f)
                Vaporfied.DrawEffects(drawInfo);

            if (calamityPlayer.vermillionFlux && drawInfo.shadow == 0f)
                VermillionFlux.DrawEffects(drawInfo);

            if (calamityPlayer.voidfrost && drawInfo.shadow == 0f)
                Voidfrost.DrawEffects(drawInfo);

            if (calamityPlayer.vHex && drawInfo.shadow == 0f)
                VulnerabilityHex.DrawEffects(drawInfo);

            if (calamityPlayer.windChilled && drawInfo.shadow == 0f)
                WindChilled.DrawEffects(drawInfo);
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
            if (calamityPlayer.bloomStoneBuffedHealRateTimer > 0 && drawInfo.shadow == 0f)
            {
                if (Main.rand.NextBool(10))
                {
                    MediumMistParticle pollenCloud = new(Player.Center, Main.rand.NextVector2Circular(1f, 1f), Color.Yellow, Color.Gold, 0.85f, 100f);
                    GeneralParticleHandler.SpawnParticle(pollenCloud);
                }
                if (Main.rand.NextBool(4))
                {
                    Dust pollenDust = Dust.NewDustDirect(Player.position, Player.width, Player.height, ModContent.DustType<LightDust>(), newColor: Color.Gold, Scale: 0.4f);
                    pollenDust.noLightEmittence = true;
                    pollenDust.noGravity = true;
                }
            }
            if (calamityPlayer.bloodfinBoost && drawInfo.shadow == 0f)
            {
                if (Main.rand.NextBool(3))
                {
                    Dust dust = Dust.NewDustDirect(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, Main.rand.NextBool(8) ? 296 : 5, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1.25f);
                    dust.noGravity = true;
                    dust.velocity *= 1.3f;
                    dust.velocity.Y -= 0.5f;
                    drawInfo.DustCache.Add(dust.dustIndex);
                }
                if (Main.rand.NextBool(16))
                {
                    Particle Plus = new HealingPlus(Player.Center - new Vector2(4, 0), Main.rand.NextFloat(0.4f, 0.8f), new Vector2(0, Main.rand.NextFloat(-2f, -3.5f)) + Player.velocity, Color.Red, Color.DarkRed, Main.rand.Next(10, 15));
                    GeneralParticleHandler.SpawnParticle(Plus);
                }
            }

            // Prideful Hunter's Planar Ripper movement speed boost
            if (calamityPlayer.planarSpeedBoost > 0 && drawInfo.shadow == 0f)
            {
                int spawnChance = (int)(13 - (calamityPlayer.planarSpeedBoost / 2));
                if (Main.rand.NextBool(spawnChance))
                {
                    Vector2 sparkVelocity = -(Vector2.UnitY * Main.rand.NextFloat(2.5f, 5f)).RotatedByRandom(MathHelper.Pi / 10);
                    Vector2 sparkPos = new Vector2(Player.position.X + Main.rand.NextFloat(-8f, 40f), Player.position.Y + Main.rand.NextFloat(-8f, 56f));
                    Particle movementSpark = new AltLineParticle(sparkPos, sparkVelocity, false, 20, Main.rand.NextFloat(0.375f, 0.5f), new Color(130, 255, 255));
                    GeneralParticleHandler.SpawnParticle(movementSpark);

                    sparkVelocity = -(Vector2.UnitY * Main.rand.NextFloat(3f, 5.5f)).RotatedByRandom(MathHelper.Pi / 6);
                    sparkPos = new Vector2(Player.position.X + Main.rand.NextFloat(-8f, 40f), Player.position.Y + Main.rand.NextFloat(-8f, 56f));
                    Particle addSparks = new CustomPulse(sparkPos, sparkVelocity, new Color(180, 255, 255), "CalamityMod/Particles/ElectricSpark", Vector2.One, 0f, 0.5f, 0.65f, 20);
                    GeneralParticleHandler.SpawnParticle(addSparks);
                }
            }

            // Some extraneous and probably undocumented visual effect caused by the heart lad pet thing
            if ((calamityPlayer.ladHearts > 0) && !Player.loveStruck && !Main.dedServ && drawInfo.shadow == 0f)
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

        #region Vanity Accessories and Tanks/Backpacks
        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f)
                return;

            Player drawPlayer = drawInfo.drawPlayer;
            Item item = drawPlayer.HeldItem;

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
                    ModContent.ItemType<ChromaticEruption>(),
                    ModContent.ItemType<DeadSunsWind>(),
                    ModContent.ItemType<Meowthrower>(),
                    ModContent.ItemType<OverloadedBlaster>(),
                    ModContent.ItemType<WildfireBloom>(),
                    ModContent.ItemType<Photoviscerator>(),
                    ModContent.ItemType<Shadethrower>(),
                    ModContent.ItemType<BloodBoiler>(),
                    ModContent.ItemType<PristineFury>(),
                    ModContent.ItemType<AuroraBlazer>(),
                    ModContent.ItemType<PurgeGuzzler>()
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
                    ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_DeadSunsWind").Value,
                    ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_Meowthrower").Value,
                    ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_OverloadedBlaster").Value,
                    ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_WildfireBloom").Value,
                    ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_Photoviscerator").Value,
                    ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_Shadethrower").Value,
                    ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_BloodBoiler").Value,
                    ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_PristineFury").Value,
                    ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_AuroraBlazer").Value,
                    ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_PurgeGuzzler").Value
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

                    int xOffset = 9;
                    // Photoviscerator's tank is extended a bit more out
                    if (thingToDraw == ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/Backpack_Photoviscerator").Value)
                    {
                        xOffset = 16;
                    }
                    DrawData howDoIDrawThings = new DrawData(thingToDraw,
                        new Vector2((int)(drawPlayer.position.X - Main.screenPosition.X + (drawPlayer.width / 2) - (xOffset * drawPlayer.direction)) - 4f * drawPlayer.direction, (int)(drawPlayer.position.Y - Main.screenPosition.Y + (drawPlayer.height / 2) + 2f * drawPlayer.gravDir - 8f * drawPlayer.gravDir + drawPlayer.gfxOffY)),
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

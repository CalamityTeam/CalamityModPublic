using CalamityMod.CalPlayer.DrawLayers;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.NPCs.AdultEidolonWyrm;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.Calamitas;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.Yharon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
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
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (Player.Calamity().andromedaState != AndromedaPlayerState.Inactive)
                AndromedaMechLayer.DrawTheStupidFuckingRobot(ref drawInfo);

            CalamityPlayer calamityPlayer = Player.Calamity();
            if (Main.myPlayer == Player.whoAmI && !Main.gameMenu && CalamityConfig.Instance.SpeedrunTimer)
            {
                string formatStr = @"hh\:mm\:ss\.ff";
                string formatStrDays = @"d\:hh\:mm\:ss\.ff";
                TimeSpan totalTime = CalamityMod.SpeedrunTimer.Elapsed.Add(calamityPlayer.previousSessionTotal);
                string text = totalTime.ToString(totalTime.Days > 0 ? formatStrDays : formatStr);
                float scale = 2f;
                float xOffset = CalamityConfig.Instance.SpeedrunTimerPosX;
                float yOffset = CalamityConfig.Instance.SpeedrunTimerPosY;

                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, text, Main.screenWidth / 2f - xOffset, yOffset, Color.White, Color.Black, default, scale);

                if (calamityPlayer.lastSplitType > -1)
                {
                    TimeSpan split = calamityPlayer.lastSplit;
                    text = split.ToString(split.Days > 0 ? formatStrDays : formatStr);
                    scale = 1f;
                    yOffset += 44f;
                    
                    Texture2D texture = null;
                    switch (calamityPlayer.lastSplitType)
                    {
                        // King Slime
                        case 1:
                            texture = TextureAssets.NpcHeadBoss[7].Value;
                            break;

                        case 2:
                            texture = TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<DesertScourgeHead>()]].Value;
                            break;

                        // Eye of Cthulhu
                        case 3:
                            texture = TextureAssets.NpcHeadBoss[1].Value;
                            break;

                        case 4:
                            texture = TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<CrabulonIdle>()]].Value;
                            break;

                        // Eater of Worlds
                        case 5:
                            texture = TextureAssets.NpcHeadBoss[2].Value;
                            break;

                        // Brain of Cthulhu
                        case 6:
                            texture = TextureAssets.NpcHeadBoss[23].Value;
                            break;

                        case 7:
                            texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/HiveMind/HiveMindP2_Head_Boss").Value;
                            break;

                        case 8:
                            texture = TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<PerforatorHive>()]].Value;
                            break;

                        // Queen Bee
                        case 9:
                            texture = TextureAssets.NpcHeadBoss[14].Value;
                            break;

                        // Skeletron
                        case 10:
                            texture = TextureAssets.NpcHeadBoss[19].Value;
                            break;

                        case 11:
                            texture = TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<SlimeGodCore>()]].Value;
                            break;

                        // Wall of Flesh
                        case 12:
                            texture = TextureAssets.NpcHeadBoss[22].Value;
                            break;

                        case 13:
                            texture = TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<Cryogen>()]].Value;
                            break;

                        // The Twins
                        case 14:
                            texture = TextureAssets.NpcHeadBoss[21].Value;
                            break;

                        case 15:
                            texture = TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<AquaticScourgeHead>()]].Value;
                            break;

                        // The Destroyer
                        case 16:
                            texture = TextureAssets.NpcHeadBoss[25].Value;
                            break;

                        case 17:
                            texture = TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<BrimstoneElemental>()]].Value;
                            break;

                        // Skeletron Prime
                        case 18:
                            texture = TextureAssets.NpcHeadBoss[18].Value;
                            break;

                        case 19:
                            texture = TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<CalamitasRun3>()]].Value;
                            break;

                        // Plantera
                        case 20:
                            texture = TextureAssets.NpcHeadBoss[12].Value;
                            break;

                        case 21:
                            texture = TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<Leviathan>()]].Value;
                            break;

                        case 22:
                            texture = TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<AstrumAureus>()]].Value;
                            break;

                        // Golem
                        case 23:
                            texture = TextureAssets.NpcHeadBoss[5].Value;
                            break;

                        case 24:
                            texture = TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<PlaguebringerGoliath>()]].Value;
                            break;

                        // Duke Fishron
                        case 25:
                            texture = TextureAssets.NpcHeadBoss[4].Value;
                            break;

                        case 26:
                            texture = TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<RavagerBody>()]].Value;
                            break;

                        // Lunatic Cultist
                        case 27:
                            texture = TextureAssets.NpcHeadBoss[31].Value;
                            break;

                        case 28:
                            texture = TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<AstrumDeusHeadSpectral>()]].Value;
                            break;

                        // Moon Lord
                        case 29:
                            texture = TextureAssets.NpcHeadBoss[8].Value;
                            break;

                        case 30:
                            texture = TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<ProfanedGuardianBoss>()]].Value;
                            break;

                        case 31:
                            texture = TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<Bumblefuck>()]].Value;
                            break;

                        case 32:
                            texture = TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<Providence>()]].Value;
                            break;

                        case 33:
                            texture = TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<CeaselessVoid>()]].Value;
                            break;

                        case 34:
                            texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/StormWeaver/StormWeaverHeadNaked_Head_Boss").Value;
                            break;

                        case 35:
                            texture = TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<Signus>()]].Value;
                            break;

                        case 36:
                            texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Polterghast/Necroplasm_Head_Boss").Value;
                            break;

                        case 37:
                            texture = TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<OldDuke>()]].Value;
                            break;

                        case 38:
                            texture = TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<DevourerofGodsHead>()]].Value;
                            break;

                        case 39:
                            texture = TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<Yharon>()]].Value;
                            break;

                        case 40:
                            texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/SupremeCalamitas/HoodlessHeadIcon").Value;
                            break;

                        case 41:
                            texture = TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<AresBody>()]].Value;
                            break;

                        case 42:
                            texture = TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<EidolonWyrmHeadHuge>()]].Value;
                            break;

                        // Queen Slime
                        case 43:
                            texture = TextureAssets.NpcHeadBoss[38].Value;
                            break;

                        // Empress of Light
                        case 44:
                            texture = TextureAssets.NpcHeadBoss[37].Value;
                            break;

                        // Deerclops
                        case 45:
                            texture = TextureAssets.NpcHeadBoss[39].Value;
                            break;

                        default:
                            break;
                    }

                    xOffset -= 58f;

                    if (texture != null)
                        Main.spriteBatch.Draw(texture, new Vector2(Main.screenWidth / 2f - xOffset - texture.Width - 4f, yOffset), null, Color.White, 0f, default, 1f, SpriteEffects.None, 0f);

                    Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, text, Main.screenWidth / 2f - xOffset, yOffset, Color.White, Color.Black, default, scale);
                }
            }

            // Dust modifications while high.
            if (calamityPlayer.trippy)
            {
                if (Main.myPlayer == Player.whoAmI)
                {
                    Rectangle screenArea = new Rectangle((int)Main.screenPosition.X - 500, (int)Main.screenPosition.Y - 50, Main.screenWidth + 1000, Main.screenHeight + 100);
                    int dustDrawn = 0;
                    float maxShroomDust = Main.maxDustToDraw / 2;
                    for (int i = 0; i < Main.maxDustToDraw; i++)
                    {
                        Dust dust = Main.dust[i];
                        if (dust.active)
                        {
                            // Only draw dust near the screen, for performance reasons.
                            if (new Rectangle((int)dust.position.X, (int)dust.position.Y, 4, 4).Intersects(screenArea))
                            {
                                dust.color = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0);
                                for (int j = 0; j < 4; j++)
                                {
                                    Vector2 dustDrawPosition = dust.position;
                                    Vector2 dustCenter = dustDrawPosition + new Vector2(4f);

                                    float distanceX = Math.Abs(dustCenter.X - Player.Center.X);
                                    float distanceY = Math.Abs(dustCenter.Y - Player.Center.Y);
                                    if (j == 0 || j == 2)
                                        dustDrawPosition.X = Player.Center.X + distanceX;
                                    else dustDrawPosition.X = Player.Center.X - distanceX;

                                    dustDrawPosition.X -= 4f;

                                    if (j == 0 || j == 1)
                                        dustDrawPosition.Y = Player.Center.Y + distanceY;

                                    else dustDrawPosition.Y = Player.Center.Y - distanceY;

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

            bool noRogueStealth = calamityPlayer.rogueStealth == 0f || Player.townNPCs > 2f || !CalamityConfig.Instance.StealthInvisbility;
            if (calamityPlayer.rogueStealth > 0f && calamityPlayer.rogueStealthMax > 0f && Player.townNPCs < 3f && CalamityConfig.Instance.StealthInvisbility)
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

            Asset<Texture2D> heart3 = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Heart3");
            Asset<Texture2D> heart4 = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Heart4");
            Asset<Texture2D> heart5 = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Heart5");
            Asset<Texture2D> heart6 = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Heart6");
            Asset<Texture2D> heartOriginal = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/HeartOriginal"); // Life fruit
            Asset<Texture2D> heartOriginal2 = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/HeartOriginal2"); // Life crystal

            int totalFruit =
                (calamityPlayer.mFruit ? 1 : 0) +
                (calamityPlayer.bOrange ? 1 : 0) +
                (calamityPlayer.eBerry ? 1 : 0) +
                (calamityPlayer.dFruit ? 1 : 0);

            TextureAssets.Heart2 = totalFruit switch
            {
                4 => heart6,
                3 => heart5,
                2 => heart4,
                1 => heart3,
                _ => heartOriginal,
            };
            TextureAssets.Heart = heartOriginal2;
            if (calamityPlayer.revivify)
            {
                if (Main.rand.NextBool(2) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, 91, Player.velocity.X * 0.2f, Player.velocity.Y * 0.2f, 100, default, 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    drawInfo.DustCache.Add(dust);
                }
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
            if (calamityPlayer.IBoots)
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
            if (calamityPlayer.elysianFire)
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
                if (!Player.StandingStill() && !Player.mount.Active)
                {
                    if (Main.rand.NextBool(2) && drawInfo.shadow == 0f)
                    {
                        int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, 27, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 0.5f;
                        drawInfo.DustCache.Add(dust);
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
                if (!Player.StandingStill() && !Player.mount.Active)
                {
                    if (drawInfo.shadow == 0f)
                    {
                        int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, Main.rand.NextBool(2) ? 57 : 244, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 0.5f;
                        drawInfo.DustCache.Add(dust);
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
            if (calamityPlayer.bFlames || calamityPlayer.aFlames || calamityPlayer.rageModeActive)
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
            if (calamityPlayer.adrenalineModeActive)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, 206, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    drawInfo.DustCache.Add(dust);
                }
                if (noRogueStealth)
                {
                    r *= 0.01f;
                    g *= 0.15f;
                    b *= 0.1f;
                    fullBright = true;
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
            else if (calamityPlayer.eGravity || calamityPlayer.dragonFire)
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
            if (calamityPlayer.eFreeze || calamityPlayer.gState || calamityPlayer.cDepth || calamityPlayer.eutrophication)
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
            if ((calamityPlayer.cadence || calamityPlayer.ladHearts > 0) && !Player.loveStruck && Main.netMode != NetmodeID.Server)
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
                    ModContent.ItemType<MepheticSprayer>(),
                    ModContent.ItemType<BrimstoneFlameblaster>(),
                    ModContent.ItemType<BrimstoneFlamesprayer>(),
                    ModContent.ItemType<SparkSpreader>(),
                    ModContent.ItemType<HalleysInferno>(),
                    ModContent.ItemType<CleansingBlaze>(),
                    ModContent.ItemType<ElementalEruption>(),
                    ModContent.ItemType<TheEmpyrean>(),
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
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_FlurrystormCannon").Value,
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_BlightSpewer").Value,
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_BrimstoneFlameblaster").Value,
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_HavocsBreath").Value,
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_SparkSpreader").Value,
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_HalleysInferno").Value,
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_CleansingBlaze").Value,
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_ElementalEruption").Value,
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_TheEmpyrean").Value,
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_Meowthrower").Value,
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_OverloadedBlaster").Value,
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_TerraFlameburster").Value,
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_Photoviscerator").Value,
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_Shadethrower").Value,
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_BloodBoiler").Value,
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_PristineFury").Value,
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_AuroraBlazer").Value
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
                        thingToDraw = ModContent.Request<Texture2D>("CalamityMod/Items/Armor/PlaguebringerCarapace_Back").Value;

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

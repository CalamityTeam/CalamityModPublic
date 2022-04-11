using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;

namespace CalamityMod.NPCs.SlimeGod
{
    [AutoloadBossHead]
    public class SlimeGodCore : ModNPC
    {
        private bool slimesSpawned = false;
        private int buffedSlime = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Slime God");
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();
            NPC.npcSlots = 10f;
            NPC.width = 44;
            NPC.height = 44;
            NPC.defense = 6;
            NPC.LifeMaxNERB(2100, 2500, 250000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 25, 0, 0);
            NPC.Opacity = 0.8f;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            Music = CalamityMod.Instance.GetMusicFromMusicMod("SlimeGod") ?? MusicID.Boss1;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.dontTakeDamage);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            writer.Write(buffedSlime);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.dontTakeDamage = reader.ReadBoolean();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            buffedSlime = reader.ReadInt32();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            CalamityGlobalNPC.slimeGod = NPC.whoAmI;

            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            NPC.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive && malice;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            Vector2 vectorCenter = NPC.Center;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, vectorCenter) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (Main.netMode != NetmodeID.MultiplayerClient && !slimesSpawned)
            {
                slimesSpawned = true;
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)vectorCenter.X, (int)vectorCenter.Y, ModContent.NPCType<SlimeGod>());
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)vectorCenter.X, (int)vectorCenter.Y, ModContent.NPCType<SlimeGodRun>());
            }

            // Emit dust
            int randomDust = Main.rand.NextBool(2) ? 173 : 260;
            int num658 = Dust.NewDust(NPC.position, NPC.width, NPC.height, randomDust, NPC.velocity.X, NPC.velocity.Y, 255, new Color(0, 80, 255, 80), NPC.scale * 1.5f);
            Main.dust[num658].noGravity = true;
            Main.dust[num658].velocity *= 0.5f;

            NPC.dontTakeDamage = false;

            // Set damage
            NPC.damage = NPC.defDamage;

            // Enrage based on large slimes
            bool phase2 = lifeRatio < 0.4f || malice;
            bool hyperMode = true;
            bool purpleSlimeAlive = false;
            bool redSlimeAlive = false;

            if (CalamityGlobalNPC.slimeGodPurple != -1)
            {
                if (Main.npc[CalamityGlobalNPC.slimeGodPurple].active)
                {
                    if (buffedSlime == 1)
                        Main.npc[CalamityGlobalNPC.slimeGodPurple].localAI[1] = 1f;
                    else
                        Main.npc[CalamityGlobalNPC.slimeGodPurple].localAI[1] = 0f;

                    calamityGlobalNPC.newAI[0] = Main.npc[CalamityGlobalNPC.slimeGodPurple].Center.X;
                    calamityGlobalNPC.newAI[1] = Main.npc[CalamityGlobalNPC.slimeGodPurple].Center.Y;

                    purpleSlimeAlive = true;
                    phase2 = lifeRatio < 0.2f;
                    hyperMode = false;
                }
            }

            if (CalamityGlobalNPC.slimeGodRed != -1)
            {
                if (Main.npc[CalamityGlobalNPC.slimeGodRed].active)
                {
                    if (buffedSlime == 2)
                        Main.npc[CalamityGlobalNPC.slimeGodRed].localAI[1] = 1f;
                    else
                        Main.npc[CalamityGlobalNPC.slimeGodRed].localAI[1] = 0f;

                    NPC.localAI[2] = Main.npc[CalamityGlobalNPC.slimeGodRed].Center.X;
                    NPC.localAI[3] = Main.npc[CalamityGlobalNPC.slimeGodRed].Center.Y;

                    redSlimeAlive = true;
                    phase2 = lifeRatio < 0.2f;
                    hyperMode = false;
                }
            }

            // Despawn
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    if (NPC.velocity.Y < -3f)
                        NPC.velocity.Y = -3f;
                    NPC.velocity.Y += 0.2f;
                    if (NPC.velocity.Y > 16f)
                        NPC.velocity.Y = 16f;

                    if (NPC.position.Y > Main.worldSurface * 16.0)
                    {
                        for (int x = 0; x < Main.maxNPCs; x++)
                        {
                            if (Main.npc[x].type == ModContent.NPCType<SlimeGod>() || Main.npc[x].type == ModContent.NPCType<SlimeGodSplit>() ||
                                Main.npc[x].type == ModContent.NPCType<SlimeGodRun>() || Main.npc[x].type == ModContent.NPCType<SlimeGodRunSplit>())
                            {
                                Main.npc[x].active = false;
                                Main.npc[x].netUpdate = true;
                            }
                        }
                        NPC.active = false;
                        NPC.netUpdate = true;
                    }

                    if (NPC.ai[0] != 0f || NPC.ai[1] != 0f)
                    {
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        NPC.localAI[0] = 0f;
                        NPC.localAI[1] = 0f;
                        NPC.netUpdate = true;
                    }
                    return;
                }
            }
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            float ai1 = malice ? 210f : hyperMode ? 270f : 360f;

            // Hide inside large slime
            if (!hyperMode && NPC.ai[1] < ai1)
            {
                if (calamityGlobalNPC.newAI[2] == 0f && NPC.life > 0)
                {
                    calamityGlobalNPC.newAI[2] = NPC.lifeMax;
                }
                if (NPC.life > 0)
                {
                    int num660 = (int)(NPC.lifeMax * 0.05);
                    if ((NPC.life + num660) < calamityGlobalNPC.newAI[2])
                    {
                        calamityGlobalNPC.newAI[2] = NPC.life;
                        calamityGlobalNPC.newAI[3] = 1f;
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/SlimeGodPossession"), (int)NPC.position.X, (int)NPC.position.Y);
                    }
                }

                if (calamityGlobalNPC.newAI[3] == 1f)
                {
                    NPC.dontTakeDamage = true;

                    NPC.rotation = NPC.velocity.X * 0.1f;

                    if (buffedSlime == 0)
                    {
                        if (purpleSlimeAlive && redSlimeAlive)
                            buffedSlime = Main.rand.Next(2) + 1;
                        else if (purpleSlimeAlive)
                            buffedSlime = 1;
                        else if (redSlimeAlive)
                            buffedSlime = 2;
                    }

                    Vector2 purpleSlimeVector = new Vector2(calamityGlobalNPC.newAI[0], calamityGlobalNPC.newAI[1]);
                    Vector2 redSlimeVector = new Vector2(NPC.localAI[2], NPC.localAI[3]);
                    Vector2 goToVector = buffedSlime == 1 ? purpleSlimeVector : redSlimeVector;

                    Vector2 goToPosition = goToVector - vectorCenter;
                    NPC.velocity = Vector2.Normalize(goToPosition) * 24f;

                    // Reduce velocity to 0 to avoid spastic movement when inside big slime.
                    if (Vector2.Distance(NPC.Center, goToVector) < 24f)
                    {
                        NPC.velocity = Vector2.Zero;

                        NPC.Opacity -= 0.2f;
                        if (NPC.Opacity < 0f)
                            NPC.Opacity = 0f;
                    }

                    bool slimeDead = false;
                    if (goToVector == purpleSlimeVector)
                        slimeDead = CalamityGlobalNPC.slimeGodPurple < 0 || !Main.npc[CalamityGlobalNPC.slimeGodPurple].active;
                    else
                        slimeDead = CalamityGlobalNPC.slimeGodRed < 0 || !Main.npc[CalamityGlobalNPC.slimeGodRed].active;

                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= 600f || slimeDead)
                    {
                        NPC.TargetClosest();
                        NPC.ai[2] = 0f;
                        calamityGlobalNPC.newAI[3] = 0f;
                        NPC.velocity = Vector2.UnitY * -12f;
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/SlimeGodExit"), (int)NPC.position.X, (int)NPC.position.Y);
                        for (int i = 0; i < 20; i++)
                        {
                            int dust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 4, 0f, 0f, 100, default, 2f);
                            Main.dust[dust].velocity *= 3f;
                            if (Main.rand.NextBool(2))
                            {
                                Main.dust[dust].scale = 0.5f;
                                Main.dust[dust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                            }
                        }
                        for (int j = 0; j < 30; j++)
                        {
                            int dust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 4, 0f, 0f, 100, default, 3f);
                            Main.dust[dust].noGravity = true;
                            Main.dust[dust].velocity *= 5f;
                            dust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 4, 0f, 0f, 100, default, 2f);
                            Main.dust[dust].velocity *= 2f;
                        }
                    }

                    return;
                }

                NPC.Opacity += 0.2f;
                if (NPC.Opacity > 0.8f)
                    NPC.Opacity = 0.8f;

                buffedSlime = 0;
            }
            else if (NPC.ai[1] < ai1)
            {
                NPC.Opacity += 0.2f;
                if (NPC.Opacity > 0.8f)
                    NPC.Opacity = 0.8f;
            }

            // Spin and shoot orbs
            if (phase2)
            {
                NPC.ai[1] += 1f;
                if (revenge)
                {
                    if (NPC.ai[1] >= ai1)
                    {
                        if (NPC.localAI[1] == 0f)
                        {
                            // Slow down, rotation
                            NPC.rotation = NPC.velocity.X * 0.1f;

                            // Set teleport location, turn invisible, spin direction
                            NPC.Opacity -= 0.2f;
                            if (NPC.Opacity <= 0f)
                            {
                                NPC.Opacity = 0f;
                                NPC.velocity.Normalize();

                                int teleportX = player.velocity.X < 0f ? -20 : 20;
                                int teleportY = player.velocity.Y < 0f ? -10 : 10;
                                int playerPosX = (int)player.Center.X / 16 + teleportX;
                                int playerPosY = (int)player.Center.Y / 16 - teleportY;

                                NPC.ai[2] = playerPosX;
                                NPC.ai[3] = playerPosY;
                                NPC.localAI[1] = 1f;
                                NPC.netUpdate = true;
                            }
                        }
                        else if (NPC.localAI[1] == 1f)
                        {
                            // Rotation
                            NPC.rotation = NPC.velocity.X * 0.1f;

                            // Teleport to location
                            if (NPC.Opacity == 0f)
                            {
                                Vector2 position = new Vector2(NPC.ai[2] * 16f - (NPC.width / 2), NPC.ai[3] * 16f - (NPC.height / 2));
                                NPC.position = position;
                            }

                            // Turn visible
                            NPC.Opacity += 0.2f;
                            if (NPC.Opacity >= 0.8f)
                            {
                                NPC.Opacity = 0.8f;
                                NPC.localAI[0] = vectorCenter.X - player.Center.X < 0 ? 1f : -1f;
                                NPC.localAI[1] = 2f;
                            }
                            NPC.netUpdate = true;
                        }
                        else
                        {
                            // No damage while spinning
                            NPC.damage = 0;

                            // Rotation
                            NPC.rotation += NPC.direction * 0.3f;

                            // Velocity boost
                            if (NPC.localAI[1] == 2f)
                            {
                                NPC.localAI[1] = 3f;
                                NPC.velocity *= 12f;
                            }

                            // Spin velocity
                            float velocity = MathHelper.TwoPi / (180f - (NPC.ai[1] - ai1));
                            NPC.velocity = NPC.velocity.RotatedBy(-(double)velocity * NPC.localAI[0]);

                            // Reset and charge at target
                            if (NPC.ai[1] >= ai1 + 100f)
                            {
                                NPC.ai[1] = 0f;
                                NPC.ai[2] = 0f;
                                NPC.ai[3] = 0f;
                                NPC.localAI[0] = 0f;
                                NPC.localAI[1] = 0f;
                                float chargeVelocity = death ? 12f : 9f;
                                NPC.velocity = Vector2.Normalize(player.Center + (malice ? player.velocity * 20f : Vector2.Zero) - vectorCenter) * chargeVelocity;
                                NPC.TargetClosest();
                                return;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float divisor = malice ? 10f : 15f;
                                if (NPC.ai[1] % divisor == 0f && Vector2.Distance(player.Center, vectorCenter) > 160f)
                                {
                                    float num179 = expertMode ? 9f : 7.5f;
                                    Vector2 value9 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
                                    float num180 = player.position.X + player.width * 0.5f - value9.X;
                                    float num181 = Math.Abs(num180) * 0.1f;
                                    float num182 = player.position.Y + player.height * 0.5f - value9.Y - num181;
                                    float num183 = (float)Math.Sqrt(num180 * num180 + num182 * num182);
                                    num183 = num179 / num183;
                                    num180 *= num183;
                                    num182 *= num183;
                                    int type = Main.rand.NextBool(2) ? ModContent.ProjectileType<AbyssBallVolley>() : ModContent.ProjectileType<AbyssBallVolley2>();
                                    int damage = NPC.GetProjectileDamage(type);
                                    value9.X += num180;
                                    value9.Y += num182;
                                    num180 = player.position.X + player.width * 0.5f - value9.X;
                                    num182 = player.position.Y + player.height * 0.5f - value9.Y;
                                    num183 = (float)Math.Sqrt(num180 * num180 + num182 * num182);
                                    num183 = num179 / num183;
                                    num180 *= num183;
                                    num182 *= num183;
                                    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), value9.X, value9.Y, num180, num182, type, damage, 0f, Main.myPlayer, 0f, 0f);
                                }
                            }
                        }
                        return;
                    }
                }
                else
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && Vector2.Distance(player.Center, vectorCenter) > 160f)
                    {
                        if (NPC.ai[1] % 40f == 0f)
                        {
                            float num179 = expertMode ? 12f : 10f;
                            Vector2 value9 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
                            float num180 = player.position.X + player.width * 0.5f - value9.X;
                            float num181 = Math.Abs(num180) * 0.1f;
                            float num182 = player.position.Y + player.height * 0.5f - value9.Y - num181;
                            float num183 = (float)Math.Sqrt(num180 * num180 + num182 * num182);
                            num183 = num179 / num183;
                            num180 *= num183;
                            num182 *= num183;
                            int type = Main.rand.NextBool(2) ? ModContent.ProjectileType<AbyssBallVolley>() : ModContent.ProjectileType<AbyssBallVolley2>();
                            int damage = NPC.GetProjectileDamage(type);
                            value9.X += num180;
                            value9.Y += num182;
                            int totalProjectiles = expertMode ? 3 : 2;
                            int spread = expertMode ? 45 : 30;
                            for (int num186 = 0; num186 < totalProjectiles; num186++)
                            {
                                num180 = player.position.X + player.width * 0.5f - value9.X;
                                num182 = player.position.Y + player.height * 0.5f - value9.Y;
                                num183 = (float)Math.Sqrt(num180 * num180 + num182 * num182);
                                num183 = num179 / num183;
                                num180 += Main.rand.Next(-spread, spread + 1);
                                num182 += Main.rand.Next(-spread, spread + 1);
                                num180 *= num183;
                                num182 *= num183;
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), value9.X, value9.Y, num180, num182, type, damage, 0f, Main.myPlayer, 0f, 0f);
                            }
                        }
                    }
                }
            }

            float num1372 = death ? 14f : revenge ? 11f : expertMode ? 8.5f : 6f;
            if (phase2)
            {
                num1372 = revenge ? 18f : expertMode ? 16f : 14f;
            }
            if (hyperMode || malice)
            {
                num1372 *= 1.25f;
            }

            Vector2 vector167 = new Vector2(vectorCenter.X + (NPC.direction * 20), vectorCenter.Y + 6f);
            float num1373 = player.position.X + player.width * 0.5f - vector167.X;
            float num1374 = player.Center.Y - vector167.Y;
            float num1375 = (float)Math.Sqrt(num1373 * num1373 + num1374 * num1374);
            float num1376 = num1372 / num1375;
            num1373 *= num1376;
            num1374 *= num1376;

            NPC.ai[0] -= 1f;
            if (num1375 < 200f || NPC.ai[0] > 0f)
            {
                if (num1375 < 200f)
                {
                    NPC.ai[0] = 20f;
                }
                if (NPC.velocity.X < 0f)
                {
                    NPC.direction = -1;
                }
                else
                {
                    NPC.direction = 1;
                }
                NPC.rotation += NPC.direction * 0.3f;
                return;
            }

            NPC.velocity.X = (NPC.velocity.X * 50f + num1373) / 51f;
            NPC.velocity.Y = (NPC.velocity.Y * 50f + num1374) / 51f;
            if (num1375 < 350f)
            {
                NPC.velocity.X = (NPC.velocity.X * 10f + num1373) / 11f;
                NPC.velocity.Y = (NPC.velocity.Y * 10f + num1374) / 11f;
            }
            if (num1375 < 300f)
            {
                NPC.velocity.X = (NPC.velocity.X * 7f + num1373) / 8f;
                NPC.velocity.Y = (NPC.velocity.Y * 7f + num1374) / 8f;
            }
            NPC.rotation = NPC.velocity.X * 0.1f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Color color24 = NPC.GetAlpha(drawColor);
            Color color25 = Lighting.GetColor((int)((double)NPC.position.X + (double)NPC.width * 0.5) / 16, (int)(((double)NPC.position.Y + (double)NPC.height * 0.5) / 16.0));
            Texture2D texture2D3 = TextureAssets.Npc[NPC.type].Value;
            int num156 = TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type];
            int y3 = num156 * (int)NPC.frameCounter;
            Rectangle rectangle = new Rectangle(0, y3, texture2D3.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            int num157 = 8;
            int num158 = 2;
            int num159 = 1;
            float num160 = 0f;
            int num161 = num159;
            spriteBatch.Draw(texture2D3, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, color24, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
            while (((num158 > 0 && num161 < num157) || (num158 < 0 && num161 > num157)) && CalamityConfig.Instance.Afterimages)
            {
                Color color26 = NPC.GetAlpha(color25);
                {
                    goto IL_6899;
                }
                IL_6881:
                num161 += num158;
                continue;
                IL_6899:
                float num164 = (float)(num157 - num161);
                if (num158 < 0)
                {
                    num164 = (float)(num159 - num161);
                }
                color26 *= num164 / ((float)NPCID.Sets.TrailCacheLength[NPC.type] * 1.5f);
                Vector2 value4 = NPC.oldPos[num161];
                float num165 = NPC.rotation;
                Main.spriteBatch.Draw(texture2D3, value4 + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, num165 + NPC.rotation * num160 * (float)(num161 - 1) * -(float)spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), origin2, NPC.scale, spriteEffects, 0f);
                goto IL_6881;
            }
            return false;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        public static void RealOnKill(NPC npc)
        {
            CalamityGlobalNPC.SetNewBossJustDowned(npc);

            CalamityGlobalNPC.SetNewShopVariable(new int[] { ModContent.NPCType<THIEF>() }, DownedBossSystem.downedSlimeGod);

            // Mark the Slime God as dead
            DownedBossSystem.downedSlimeGod = true;
            CalamityNetcode.SyncWorld();
        }

        public override void OnKill()
        {
            if (LastSlimeGodStanding())
                RealOnKill(NPC);
        }

        public static bool LastSlimeGodStanding()
        {
            int slimeGodCount = NPC.CountNPCS(ModContent.NPCType<SlimeGod>()) +
                NPC.CountNPCS(ModContent.NPCType<SlimeGodRun>()) +
                NPC.CountNPCS(ModContent.NPCType<SlimeGodSplit>()) +
                NPC.CountNPCS(ModContent.NPCType<SlimeGodRunSplit>()) +
                NPC.CountNPCS(ModContent.NPCType<SlimeGodCore>());

            return slimeGodCount <= 1;
        }

        public static void DefineSlimeGodLoot(NPCLoot npcLoot)
        {
            // Every Slime God piece drops Gel, even if it's not the last one.
            npcLoot.Add(ItemID.Gel, 1, 32, 48);

            var mainDrops = npcLoot.DefineConditionalDropSet(LastSlimeGodStanding);
            mainDrops.Add(ItemDropRule.BossBag(ModContent.ItemType<SlimeGodBag>()));

            // Purified Jam is once per player, but drops for all players.
            npcLoot.AddIf(() =>
            {
                if (!LastSlimeGodStanding())
                    return false;

                CalamityPlayer mp = Main.LocalPlayer.Calamity();
                if (!mp.revJamDrop)
                {
                    mp.revJamDrop = true;
                    return !DownedBossSystem.downedSlimeGod;
                }
                return false;
            }, ModContent.ItemType<PurifiedJam>(), 1, 6, 8);

            // Normal drops: Everything that would otherwise be in the bag
            LeadingConditionRule normalOnly = new LeadingConditionRule(new Conditions.NotExpert());
            mainDrops.Add(normalOnly);
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<OverloadedBlaster>(),
                    ModContent.ItemType<AbyssalTome>(),
                    ModContent.ItemType<EldritchTome>(),
                    ModContent.ItemType<CorroslimeStaff>(),
                    ModContent.ItemType<CrimslimeStaff>(),
                    ModContent.ItemType<SlimePuppetStaff>(),
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, weapons));

                // Materials
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<PurifiedGel>(), 1, 30, 45));

                // Vanity
                normalOnly.Add(ModContent.ItemType<SlimeGodMask>(), 7);
                normalOnly.Add(ModContent.ItemType<SlimeGodMask2>(), 7);

                // Equipment
                normalOnly.Add(ModContent.ItemType<ManaOverloader>());
            }

            mainDrops.Add(ModContent.ItemType<SlimeGodTrophy>(), 10);

            // Lore
            npcLoot.AddConditionalPerPlayer(() => LastSlimeGodStanding() && !DownedBossSystem.downedSlimeGod, ModContent.ItemType<KnowledgeSlimeGod>());
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => DefineSlimeGodLoot(npcLoot);

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = 40;
                NPC.height = 40;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 4, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 4, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 4, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * bossLifeScale);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.VortexDebuff, 120, true);
            player.AddBuff(BuffID.Weak, 120, true);
            player.AddBuff(BuffID.Darkness, 120, true);
        }
    }
}

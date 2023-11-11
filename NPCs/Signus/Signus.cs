using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;

namespace CalamityMod.NPCs.Signus
{
    [AutoloadBossHead]
    public class Signus : ModNPC
    {
        private int spawnX = 750;
        private int spawnY = 120;
        private int lifeToAlpha = 0;
        private int stealthTimer = 0;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                PortraitPositionYOverride = 10f,
                Scale = 0.4f,
                PortraitScale = 0.5f,
            };
            value.Position.X += 6f;
            value.Position.Y += 10f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 32f;
            NPC.GetNPCDamage();
            NPC.width = 130;
            NPC.height = 130;
            NPC.defense = 60;
            NPC.LifeMaxNERB(300000, 360000, 320000);
            NPC.value = Item.buyPrice(2, 0, 0, 0);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit49;
            NPC.DeathSound = SoundID.NPCDeath51;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Signus")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(spawnX);
            writer.Write(spawnY);
            writer.Write(lifeToAlpha);
            writer.Write(stealthTimer);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            spawnX = reader.ReadInt32();
            spawnY = reader.ReadInt32();
            lifeToAlpha = reader.ReadInt32();
            stealthTimer = reader.ReadInt32();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            CalamityGlobalNPC.signus = NPC.whoAmI;

            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool expertMode = Main.expertMode || bossRush;

            Vector2 vectorCenter = NPC.Center;

            double lifeRatio = NPC.life / (double)NPC.lifeMax;

            lifeToAlpha = (int)((Main.getGoodWorld ? 200D : 100D) * (1D - lifeRatio));
            int maxCharges = death ? 1 : revenge ? 2 : expertMode ? 3 : 4;
            int maxTeleports = (death && lifeRatio < 0.9) ? 1 : revenge ? 2 : expertMode ? 3 : 4;
            float inertia = bossRush ? 9f : death ? 10f : revenge ? 11f : expertMode ? 12f : 14f;
            float chargeVelocity = bossRush ? 16f : death ? 14f : revenge ? 13f : expertMode ? 12f : 10f;
            if (Main.getGoodWorld)
            {
                inertia *= 0.5f;
                chargeVelocity *= 1.15f;
            }

            bool phase2 = lifeRatio < 0.75f && expertMode;
            bool phase3 = lifeRatio < 0.5f;
            bool phase4 = lifeRatio < 0.33f;

            NPC.damage = NPC.defDamage;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, vectorCenter) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (!player.active || player.dead || Vector2.Distance(player.Center, vectorCenter) > 6400f)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead || Vector2.Distance(player.Center, vectorCenter) > 6400f)
                {
                    NPC.rotation = NPC.velocity.X * 0.04f;

                    if (NPC.velocity.Y > 3f)
                        NPC.velocity.Y = 3f;
                    NPC.velocity.Y -= 0.15f;
                    if (NPC.velocity.Y < -12f)
                        NPC.velocity.Y = -12f;

                    if (NPC.timeLeft > 60)
                        NPC.timeLeft = 60;

                    if (NPC.ai[0] != 0f)
                    {
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        calamityGlobalNPC.newAI[0] = 0f;
                        calamityGlobalNPC.newAI[1] = 0f;
                        spawnY = 120;
                        NPC.netUpdate = true;
                    }
                    return;
                }
            }
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            if (lifeToAlpha < (Main.getGoodWorld ? 100 : 50) && NPC.ai[0] != 1f)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (Main.rand.Next(3) < 1)
                    {
                        int cosmiliteDust = Dust.NewDust(vectorCenter - new Vector2(70f), 70 * 2, 70 * 2, (int)CalamityDusts.PurpleCosmilite, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 90, default, 1.5f);
                        Main.dust[cosmiliteDust].noGravity = true;
                        Main.dust[cosmiliteDust].velocity *= 0.2f;
                        Main.dust[cosmiliteDust].fadeIn = 1f;
                    }
                }
            }

            // Zenith seed stealth strike stuff
            int stealthSoundGate = 300;
            int maxStealth = 360;

            if (Main.zenithWorld)
            {
                if (stealthTimer < maxStealth)
                {
                    stealthTimer++;
                }
                if (stealthTimer == stealthSoundGate)
                {
                    SoundEngine.PlaySound(CalPlayer.CalamityPlayer.RogueStealthSound, NPC.Center);
                }
                if (stealthTimer >= stealthSoundGate && stealthTimer < maxStealth)
                {
                    NPC.alpha = 0;
                    NPC.knockBackResist = 0f;
                    NPC.rotation = NPC.rotation.AngleLerp(0f, 0.2f);
                    NPC.velocity *= 0.3f;
                    return;
                }
            }

            if (NPC.ai[0] <= 2f)
            {
                NPC.rotation = NPC.velocity.X * 0.04f;
                float playerLocation = vectorCenter.X - player.Center.X;
                NPC.direction = playerLocation < 0f ? 1 : -1;
                NPC.spriteDirection = NPC.direction;

                NPC.knockBackResist = 0.05f;
                if (expertMode)
                    NPC.knockBackResist *= Main.RegisteredGameModes[GameModeID.Expert].KnockbackToEnemiesMultiplier;
                if (phase3 || revenge)
                    NPC.knockBackResist = 0f;

                float speed = bossRush ? 20f : revenge ? 15f : expertMode ? 14f : 12f;
                if (expertMode)
                    speed += death ? 6f * (float)(1D - lifeRatio) : 4f * (float)(1D - lifeRatio);

                float playerXDist = player.Center.X - vectorCenter.X;
                float playerYDist = player.Center.Y - vectorCenter.Y;
                float playerDistance = (float)Math.Sqrt(playerXDist * playerXDist + playerYDist * playerYDist);
                playerDistance = speed / playerDistance;
                playerXDist *= playerDistance;
                playerYDist *= playerDistance;

                float inertia2 = 50f;
                if (Main.getGoodWorld)
                    inertia2 *= 0.5f;

                NPC.velocity.X = (NPC.velocity.X * inertia2 + playerXDist) / (inertia2 + 1f);
                NPC.velocity.Y = (NPC.velocity.Y * inertia2 + playerYDist) / (inertia2 + 1f);
            }
            else
                NPC.knockBackResist = 0f;

            if (NPC.ai[0] == -1f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int phase;
                    do phase = Main.rand.Next(5);
                    while (phase == NPC.ai[1] || (phase == 0 && phase4) || phase == 1 || phase == 2);

                    NPC.ai[0] = phase;
                    NPC.ai[1] = 0f;

                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 0f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.localAI[1] += bossRush ? 1.5f : 1f;

                    if (expertMode)
                        NPC.localAI[1] += death ? 3f * (float)(1D - lifeRatio) : 2f * (float)(1D - lifeRatio);

                    if (NPC.localAI[1] >= (Main.getGoodWorld ? 0f : 120f))
                    {
                        NPC.localAI[1] = 0f;

                        NPC.TargetClosest();

                        int maxTeleportTries = 0;
                        int playerTileX;
                        int playerTileY;
                        while (true)
                        {
                            maxTeleportTries++;
                            playerTileX = (int)player.Center.X / 16;
                            playerTileY = (int)player.Center.Y / 16;

                            int min = 14;
                            int max = 18;

                            if (Main.rand.NextBool())
                                playerTileX += Main.rand.Next(min, max);
                            else
                                playerTileX -= Main.rand.Next(min, max);

                            if (Main.rand.NextBool())
                                playerTileY += Main.rand.Next(min, max);
                            else
                                playerTileY -= Main.rand.Next(min, max);

                            if (!WorldGen.SolidTile(playerTileX, playerTileY))
                                break;

                            if (maxTeleportTries > 100)
                                return;
                        }

                        NPC.ai[0] = 1f;
                        NPC.ai[1] = playerTileX;
                        NPC.ai[2] = playerTileY;

                        NPC.netUpdate = true;

                        return;
                    }
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                Vector2 position = new Vector2(NPC.ai[1] * 16f - (NPC.width / 2), NPC.ai[2] * 16f - (NPC.height / 2));
                for (int m = 0; m < 5; m++)
                {
                    int dust = Dust.NewDust(position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 90, default, 2f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].fadeIn = 1f;
                }

                NPC.alpha += bossRush ? 3 : 2;
                if (expertMode)
                    NPC.alpha += death ? (int)Math.Round(4.5D * (1D - lifeRatio)) : (int)Math.Round(3D * (1D - lifeRatio));

                if (NPC.alpha >= 255)
                {
                    SoundEngine.PlaySound(SoundID.Item8, vectorCenter);

                    NPC.alpha = 255;

                    NPC.position = position;

                    for (int n = 0; n < 15; n++)
                    {
                        int cosmiliteDusty = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 90, default, 3f);
                        Main.dust[cosmiliteDusty].noGravity = true;
                    }

                    NPC.ai[0] = 2f;

                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 2f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.alpha -= 50;
                if (NPC.alpha <= lifeToAlpha)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && revenge)
                    {
                        SoundEngine.PlaySound(SoundID.Item122, NPC.Center);

                        int cosmicMineSpawn = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(player.position.X + 750f), (int)player.position.Y, ModContent.NPCType<CosmicMine>());
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, cosmicMineSpawn, 0f, 0f, 0f, 0, 0, 0);

                        int cosmicMineSpawn2 = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(player.position.X - 750f), (int)player.position.Y, ModContent.NPCType<CosmicMine>());
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, cosmicMineSpawn2, 0f, 0f, 0f, 0, 0, 0);

                        if (stealthTimer >= maxStealth)
                        {
                            int stealthCosmicMine = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(player.position.X + 950f), (int)player.position.Y, ModContent.NPCType<CosmicMine>());
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, stealthCosmicMine, 0f, 0f, 0f, 0, 0, 0);

                            int stealthCosmicMine2 = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(player.position.X - 950f), (int)player.position.Y, ModContent.NPCType<CosmicMine>());
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, stealthCosmicMine2, 0f, 0f, 0f, 0, 0, 0);

                            SoundEngine.PlaySound(RaidersTalisman.StealthHitSound, NPC.Center);
                            stealthTimer = 0;
                        }

                        for (int i = 0; i < 5; i++)
                        {
                            int teleportDust = Dust.NewDust(new Vector2(player.position.X + 750f, player.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                            Main.dust[teleportDust].velocity *= 3f;
                            Main.dust[teleportDust].noGravity = true;
                            if (Main.rand.NextBool())
                            {
                                Main.dust[teleportDust].scale = 0.5f;
                                Main.dust[teleportDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                            }
                            int j = Dust.NewDust(new Vector2(player.position.X - 750f, player.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                            Main.dust[j].velocity *= 3f;
                            Main.dust[j].noGravity = true;
                            if (Main.rand.NextBool())
                            {
                                Main.dust[j].scale = 0.5f;
                                Main.dust[j].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                            }
                        }

                        for (int j = 0; j < 20; j++)
                        {
                            int teleportDust2 = Dust.NewDust(new Vector2(player.position.X + 750f, player.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
                            Main.dust[teleportDust2].noGravity = true;
                            Main.dust[teleportDust2].velocity *= 5f;
                            teleportDust2 = Dust.NewDust(new Vector2(player.position.X + 750f, player.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                            Main.dust[teleportDust2].velocity *= 2f;
                            int teleportDusty = Dust.NewDust(new Vector2(player.position.X - 750f, player.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
                            Main.dust[teleportDusty].noGravity = true;
                            Main.dust[teleportDusty].velocity *= 5f;
                            teleportDusty = Dust.NewDust(new Vector2(player.position.X - 750f, player.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                            Main.dust[teleportDusty].velocity *= 2f;
                        }
                    }

                    NPC.ai[3] += 1f;
                    NPC.alpha = lifeToAlpha;
                    if (NPC.ai[3] >= maxTeleports)
                    {
                        NPC.ai[0] = -1f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                    }
                    else
                        NPC.ai[0] = 0f;

                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 3f)
            {
                NPC.rotation = NPC.velocity.X * 0.04f;
                float playerLocation = vectorCenter.X - player.Center.X;
                NPC.direction = playerLocation < 0f ? 1 : -1;
                NPC.spriteDirection = NPC.direction;

                float divisor = expertMode ? (bossRush ? 10f : death ? 12f : revenge ? 15f : 20f) - (float)Math.Ceiling(5D * (1D - lifeRatio)) : 20f;
                float scytheBarrageTime = divisor * 3f;
                float scytheBarrageCooldown = divisor * 3f;

                NPC.ai[1] += 1f;
                if (NPC.ai[2] > 0f)
                    NPC.ai[2] -= 1f;
                else
                    NPC.ai[2] = scytheBarrageTime + scytheBarrageCooldown;

                if (NPC.ai[2] <= scytheBarrageTime)
                {
                    if (NPC.ai[1] % divisor == divisor - 1f)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float scytheXDist = player.Center.X - vectorCenter.X;
                            float scytheYDist = player.Center.Y - vectorCenter.Y;
                            float scytheDistance = (float)Math.Sqrt(scytheXDist * scytheXDist + scytheYDist * scytheYDist);
                            scytheDistance = 15f / scytheDistance;
                            scytheXDist *= scytheDistance;
                            scytheYDist *= scytheDistance;
                            int type = ModContent.ProjectileType<SignusScythe>();
                            int damage = NPC.GetProjectileDamage(type);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), vectorCenter.X, vectorCenter.Y, scytheXDist, scytheYDist, type, damage, 0f, Main.myPlayer, 0f, NPC.target + 1);
                            if (stealthTimer >= maxStealth)
                            {
                                damage *= 2;
                                SoundEngine.PlaySound(RaidersTalisman.StealthHitSound, NPC.Center);
                                for (int i = 0; i < 4; i++)
                                {
                                    Vector2 offset = new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, 6));
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), vectorCenter.X, vectorCenter.Y, scytheXDist + offset.X, scytheYDist + offset.Y, type, damage, 0f, Main.myPlayer, 0f, NPC.target + 1);
                                }
                                stealthTimer = 0;
                            }
                        }
                    }
                }

                float maxVelocityY = bossRush ? 1.5f : death ? 2.5f : 3f;
                float maxVelocityX = bossRush ? 5f : death ? 7f : 8f;

                if (NPC.position.Y > player.position.Y - 250f)
                {
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y *= 0.9f;

                    NPC.velocity.Y -= death ? 0.12f : 0.1f;

                    if (NPC.velocity.Y > maxVelocityY)
                        NPC.velocity.Y = maxVelocityY;
                }
                else if (NPC.position.Y < player.position.Y - 350f)
                {
                    if (NPC.velocity.Y < 0f)
                        NPC.velocity.Y *= 0.9f;

                    NPC.velocity.Y += death ? 0.12f : 0.1f;

                    if (NPC.velocity.Y < -maxVelocityY)
                        NPC.velocity.Y = -maxVelocityY;
                }

                if (vectorCenter.X > player.Center.X + 600f)
                {
                    if (NPC.velocity.X > 0f)
                        NPC.velocity.X *= 0.9f;

                    NPC.velocity.X -= death ? 0.12f : 0.1f;

                    if (NPC.velocity.X > maxVelocityX)
                        NPC.velocity.X = maxVelocityX;
                }

                if (vectorCenter.X < player.Center.X - 600f)
                {
                    if (NPC.velocity.X < 0f)
                        NPC.velocity.X *= 0.9f;

                    NPC.velocity.X += death ? 0.12f : 0.1f;

                    if (NPC.velocity.X < -maxVelocityX)
                        NPC.velocity.X = -maxVelocityX;
                }

                if (NPC.ai[1] >= divisor * 20f)
                {
                    NPC.ai[0] = -1f;
                    NPC.ai[1] = 3f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.TargetClosest();
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 4f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int totalLamps = (Main.getGoodWorld && !Main.zenithWorld) ? 10 : 5;
                    if (NPC.CountNPCS(ModContent.NPCType<CosmicLantern>()) < totalLamps)
                    {
                        bool buffed = false;
                        if (stealthTimer >= maxStealth)
                        {
                            SoundEngine.PlaySound(RaidersTalisman.StealthHitSound, NPC.Center);
                            buffed = true;
                        }
                        for (int x = 0; x < totalLamps; x++)
                        {
                            int type = ModContent.NPCType<CosmicLantern>();
                            if (Main.rand.NextBool(10) && Main.zenithWorld)
                            {
                                type = ModContent.NPCType<CosmicMine>();
                            }
                            int cosmicMineSpawn = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(player.position.X + spawnX), (int)(player.position.Y + spawnY), type);
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, cosmicMineSpawn, 0f, 0f, 0f, 0, 0, 0);

                            int cosmicMineSpawn2 = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(player.position.X - spawnX), (int)(player.position.Y + spawnY), type);
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, cosmicMineSpawn2, 0f, 0f, 0f, 0, 0, 0);

                            if (buffed)
                            {
                                int stealthCosmicMine = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(player.position.X + spawnX + spawnX / 2), (int)(player.position.Y + spawnY), ModContent.NPCType<CosmicLantern>());
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, stealthCosmicMine, 0f, 0f, 0f, 0, 0, 0);

                                int stealthCosmicMine2 = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(player.position.X - spawnX - spawnX / 2), (int)(player.position.Y + spawnY), ModContent.NPCType<CosmicLantern>());
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, stealthCosmicMine2, 0f, 0f, 0f, 0, 0, 0);
                            }

                            spawnY -= 60;
                        }
                        if (buffed)
                        {
                            stealthTimer = 0;
                        }
                        spawnY = 120;
                    }
                }

                NPC.rotation = NPC.velocity.ToRotation();

                if (Math.Sign(NPC.velocity.X) != 0)
                    NPC.spriteDirection = -Math.Sign(NPC.velocity.X);

                if (NPC.rotation < -MathHelper.PiOver2)
                    NPC.rotation += MathHelper.Pi;
                if (NPC.rotation > MathHelper.PiOver2)
                    NPC.rotation -= MathHelper.Pi;

                NPC.spriteDirection = Math.Sign(NPC.velocity.X);

                if (calamityGlobalNPC.newAI[0] == 0f) // Line up the charge
                {
                    float velocity = bossRush ? 18f : revenge ? 16f : expertMode ? 15f : 14f;
                    if (expertMode)
                        velocity += death ? 6f * (float)(1D - lifeRatio) : 4f * (float)(1D - lifeRatio);

                    Vector2 playerCenterDist = player.Center - vectorCenter;
                    Vector2 playerHoverAboveDist = playerCenterDist - Vector2.UnitY * 300f;

                    playerCenterDist = Vector2.Normalize(playerCenterDist) * velocity;
                    playerHoverAboveDist = Vector2.Normalize(playerHoverAboveDist) * velocity;

                    bool canLineUpCharge = Collision.CanHit(vectorCenter, 1, 1, player.Center, 1, 1) || NPC.ai[3] >= 120f;
                    canLineUpCharge = canLineUpCharge && playerCenterDist.ToRotation() > MathHelper.Pi / 8f && playerCenterDist.ToRotation() < MathHelper.Pi - MathHelper.Pi / 8f;
                    if (playerCenterDist.Length() > 1400f || !canLineUpCharge)
                    {
                        NPC.velocity = (NPC.velocity * (inertia - 1f) + playerHoverAboveDist) / inertia;

                        if (!canLineUpCharge)
                        {
                            NPC.ai[3] += 1f;
                            if (NPC.ai[3] == 120f)
                                NPC.netUpdate = true;
                        }
                        else
                            NPC.ai[3] = 0f;
                    }
                    else
                    {
                        calamityGlobalNPC.newAI[0] = 1f;
                        NPC.ai[2] = playerCenterDist.X;
                        NPC.ai[3] = playerCenterDist.Y;
                        NPC.netUpdate = true;
                    }
                }
                else if (calamityGlobalNPC.newAI[0] == 1f) // Pause before charge
                {
                    NPC.velocity *= 0.8f;

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] >= 5f)
                    {
                        calamityGlobalNPC.newAI[0] = 2f;

                        NPC.netUpdate = true;

                        Vector2 velocity = new Vector2(NPC.ai[2], NPC.ai[3]);
                        velocity.Normalize();
                        velocity *= chargeVelocity;
                        NPC.velocity = velocity;

                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                    }
                }
                else if (calamityGlobalNPC.newAI[0] == 2f) // Charging
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        bool buffed = false;
                        if (stealthTimer >= maxStealth && NPC.ai[1] == 0)
                        {
                            SoundEngine.PlaySound(RaidersTalisman.StealthHitSound, NPC.Center);
                            buffed = true;
                        }
                        NPC.ai[2] += 1f;
                        if ((phase2 || buffed) && NPC.ai[2] % 3f == 0f)
                        {
                            SoundEngine.PlaySound(SoundID.Item73, NPC.Center);
                            int type = (CalamityWorld.LegendaryMode && revenge) ? ModContent.ProjectileType<PeanutRocket>() : ModContent.ProjectileType<EssenceDust>();
                            int damage = (CalamityWorld.LegendaryMode && revenge) ? 60 : NPC.GetProjectileDamage(type);
                            Vector2 velocity = Main.zenithWorld ? new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11)) : Vector2.Zero;
                            if (Main.getGoodWorld && !Main.zenithWorld)
                            {
                                velocity = new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, 6));
                            }
                            int ai = buffed ? 69 : 0;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), vectorCenter, velocity, type, damage, 0f, Main.myPlayer, ai);
                        }
                    }

                    NPC.ai[1] += 1f;
                    bool shouldEndCharge = vectorCenter.Y + 50f > player.Center.Y;
                    if ((NPC.ai[1] >= 90f && shouldEndCharge) || NPC.velocity.Length() < 8f)
                    {
                        calamityGlobalNPC.newAI[0] = 3f;
                        NPC.ai[1] = 30f;
                        NPC.ai[2] = 0f;
                        if (stealthTimer >= maxStealth)
                        {
                            stealthTimer = 0;
                        }
                        NPC.velocity /= 2f;
                        NPC.netUpdate = true;
                    }
                    else
                    {
                        Vector2 distFromPlayerCenter = player.Center - vectorCenter;
                        distFromPlayerCenter.Normalize();
                        if (distFromPlayerCenter.HasNaNs())
                            distFromPlayerCenter = new Vector2(NPC.direction, 0f);

                        NPC.velocity = (NPC.velocity * (inertia - 1f) + distFromPlayerCenter * (NPC.velocity.Length() + 0.111111117f * inertia)) / inertia;
                    }
                }
                else if (calamityGlobalNPC.newAI[0] == 3f) // Slow down after charging and reset
                {
                    if (stealthTimer >= maxStealth)
                    {
                        stealthTimer = 0;
                    }
                    NPC.ai[1] -= 1f;
                    if (NPC.ai[1] <= 0f)
                    {
                        NPC.TargetClosest();
                        calamityGlobalNPC.newAI[1] += 1f;
                        if (calamityGlobalNPC.newAI[1] >= maxCharges)
                        {
                            NPC.ai[0] = -1f;
                            NPC.ai[1] = 4f;
                            NPC.ai[2] = 0f;
                            NPC.ai[3] = 0f;
                            calamityGlobalNPC.newAI[1] = 0f;
                        }
                        else
                        {
                            NPC.ai[1] = 0f;
                        }
                        calamityGlobalNPC.newAI[0] = 0f;
                        NPC.netUpdate = true;
                    }
                    NPC.velocity *= 0.97f;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += NPC.IsABestiaryIconDummy ? 1.65 : 1.0;
            if (NPC.ai[0] == 4f)
            {
                if (NPC.frameCounter > 72.0) //12
                {
                    NPC.frameCounter = 0.0;
                }
            }
            else
            {
                int frameY = 196;
                if (NPC.frameCounter > 72.0)
                {
                    NPC.frameCounter = 0.0;
                }
                NPC.frame.Y = frameY * (int)(NPC.frameCounter / 12.0); //1 to 6
                if (NPC.frame.Y >= frameHeight * 6)
                {
                    NPC.frame.Y = 0;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D NPCTexture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glowMaskTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Signus/SignusGlow").Value;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            int afterimageAmt = 5;
            Rectangle frame = NPC.frame;
            int frameCount = Main.npcFrameCount[NPC.type];

            if (NPC.ai[0] == 4f)
            {
                NPCTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Signus/SignusAlt2").Value;
                glowMaskTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Signus/SignusAlt2Glow").Value;
                afterimageAmt = 10;
                int frameY = 94 * (int)(NPC.frameCounter / 12.0);
                if (frameY >= 94 * 6)
                    frameY = 0;
                frame = new Rectangle(0, frameY, NPCTexture.Width, NPCTexture.Height / frameCount);
            }
            else if (NPC.ai[0] == 3f)
            {
                NPCTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Signus/SignusAlt").Value;
                glowMaskTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Signus/SignusAltGlow").Value;
                afterimageAmt = 7;
            }
            else
            {
                NPCTexture = TextureAssets.Npc[NPC.type].Value;
                glowMaskTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Signus/SignusGlow").Value;
            }

            Vector2 halfSizeTexture = new Vector2(NPCTexture.Width / 2, NPCTexture.Height / frameCount / 2);
            float scale = NPC.scale;
            float rotation = NPC.rotation;
            float offsetY = NPC.gfxOffY;
            float transparency = 1;
            if (stealthTimer >= 300)
            {
                transparency = (100 - (stealthTimer - 300)) * 0.01f;
            }

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageAmt; i += 2)
                {
                    Color afterimageColor = drawColor;
                    afterimageColor = Color.Lerp(afterimageColor, Color.White, 0.5f);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (afterimageAmt - i) / 15f;
                    Vector2 afterimagePos = NPC.oldPos[i] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    afterimagePos -= new Vector2(NPCTexture.Width, NPCTexture.Height / frameCount) * scale / 2f;
                    afterimagePos += halfSizeTexture * scale + new Vector2(0f, 4f + offsetY);
                    spriteBatch.Draw(NPCTexture, afterimagePos, new Rectangle?(frame), afterimageColor * transparency, rotation, halfSizeTexture, scale, spriteEffects, 0f);
                }
            }

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(NPCTexture.Width, NPCTexture.Height / frameCount) * scale / 2f;
            drawLocation += halfSizeTexture * scale + new Vector2(0f, 4f + offsetY);
            spriteBatch.Draw(NPCTexture, drawLocation, new Rectangle?(frame), NPC.GetAlpha(drawColor) * transparency, rotation, halfSizeTexture, scale, spriteEffects, 0f);

            Color eyeGlowColor = Color.Lerp(Color.White, Color.Fuchsia, 0.5f);
            if (Main.zenithWorld)
            {
                eyeGlowColor = Color.MediumBlue;
            }

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int j = 1; j < afterimageAmt; j++)
                {
                    Color eyeAfterimageColor = eyeGlowColor;
                    eyeAfterimageColor = Color.Lerp(eyeAfterimageColor, Color.White, 0.5f);
                    eyeAfterimageColor = NPC.GetAlpha(eyeAfterimageColor);
                    eyeAfterimageColor *= (afterimageAmt - j) / 15f;
                    Vector2 eyeAfterimagePos = NPC.oldPos[j] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    eyeAfterimagePos -= new Vector2(glowMaskTexture.Width, glowMaskTexture.Height / frameCount) * scale / 2f;
                    eyeAfterimagePos += halfSizeTexture * scale + new Vector2(0f, 4f + offsetY);
                    spriteBatch.Draw(glowMaskTexture, eyeAfterimagePos, new Rectangle?(frame), eyeAfterimageColor, rotation, halfSizeTexture, scale, spriteEffects, 0f);
                }
            }

            if (Main.zenithWorld) // make Sig's eyes more visible in the zenith seed due to the color change
            {
                CalamityUtils.EnterShaderRegion(spriteBatch);
                Color outlineColor = Color.Lerp(Color.Blue, Color.White, 0.4f);
                Vector3 outlineHSL = Main.rgbToHsl(outlineColor); //BasicTint uses the opposite hue i guess? or smth is fucked with the way shaders get their colors. anyways, we invert it
                float outlineThickness = MathHelper.Clamp(0.5f, 0f, 1f);

                GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(1f);
                GameShaders.Misc["CalamityMod:BasicTint"].UseColor(Main.hslToRgb(1 - outlineHSL.X, outlineHSL.Y, outlineHSL.Z));
                GameShaders.Misc["CalamityMod:BasicTint"].Apply();

                for (float i = 0; i < 1; i += 0.125f)
                {
                    spriteBatch.Draw(glowMaskTexture, drawLocation + (i * MathHelper.TwoPi).ToRotationVector2() * outlineThickness, new Rectangle?(frame), outlineColor, rotation, halfSizeTexture, scale, spriteEffects, 0f);
                }
                CalamityUtils.ExitShaderRegion(spriteBatch);
            }

            spriteBatch.Draw(glowMaskTexture, drawLocation, new Rectangle?(frame), eyeGlowColor, rotation, halfSizeTexture, scale, spriteEffects, 0f);

            return false;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<SupremeHealingPotion>();
        }

        public static bool LastSentinelKilled() => !DownedBossSystem.downedSignus && DownedBossSystem.downedStormWeaver && DownedBossSystem.downedCeaselessVoid;

        public override void OnKill()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);
            DownedBossSystem.downedSignus = true;
            CalamityNetcode.SyncWorld();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<SignusBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            LeadingConditionRule normalOnly = new LeadingConditionRule(new Conditions.NotExpert());
            npcLoot.Add(normalOnly);
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<CosmicKunai>(),
                    ModContent.ItemType<Cosmilamp>(),
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, weapons));

                // Materials
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<TwistingNether>(), 1, 5, 7));

                // Equipment
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<SpectralVeil>()));

                // Vanity
                normalOnly.Add(ModContent.ItemType<SignusMask>(), 7);
                var godSlayerVanity = ItemDropRule.Common(ModContent.ItemType<AncientGodSlayerHelm>(), 20);
                godSlayerVanity.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AncientGodSlayerChestplate>()));
                godSlayerVanity.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AncientGodSlayerLeggings>()));
                normalOnly.Add(godSlayerVanity);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
            }

            npcLoot.Add(ModContent.ItemType<SignusTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<SignusRelic>());

            // GFB Nanotech and Ethereal Talisman drops
            var GFBOnly = npcLoot.DefineConditionalDropSet(DropHelper.GFB);
            {
                GFBOnly.Add(ModContent.ItemType<Nanotech>(), hideLootReport: true);
                GFBOnly.Add(ModContent.ItemType<EtherealTalisman>(), hideLootReport: true);
            }

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedSignus, ModContent.ItemType<LoreSignus>(), desc: DropHelper.FirstKillText);
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = 200;
                NPC.height = 150;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int i = 0; i < 40; i++)
                {
                    int teleportDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[teleportDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[teleportDust].scale = 0.5f;
                        Main.dust[teleportDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 60; j++)
                {
                    int teleportDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
                    Main.dust[teleportDust2].noGravity = true;
                    Main.dust[teleportDust2].velocity *= 5f;
                    teleportDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[teleportDust2].velocity *= 2f;
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    float randomSpread = Main.rand.Next(-200, 201) / 100f;
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Signus").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Signus2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Signus3").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Signus4").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Signus5").Type, 1f);
                }
            }
        }

        // Can only hit the target if within certain distance
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            Rectangle targetHitbox = target.Hitbox;

            float hitboxTopLeft = Vector2.Distance(NPC.Center, targetHitbox.TopLeft());
            float hitboxTopRight = Vector2.Distance(NPC.Center, targetHitbox.TopRight());
            float hitboxBotLeft = Vector2.Distance(NPC.Center, targetHitbox.BottomLeft());
            float hitboxBotRight = Vector2.Distance(NPC.Center, targetHitbox.BottomRight());

            float minDist = hitboxTopLeft;
            if (hitboxTopRight < minDist)
                minDist = hitboxTopRight;
            if (hitboxBotLeft < minDist)
                minDist = hitboxBotLeft;
            if (hitboxBotRight < minDist)
                minDist = hitboxBotRight;

            return minDist <= 60f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<WhisperingDeath>(), 420, true);
        }
    }
}

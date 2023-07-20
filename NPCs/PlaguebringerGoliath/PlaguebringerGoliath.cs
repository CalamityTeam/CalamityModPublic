using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
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
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;

namespace CalamityMod.NPCs.PlaguebringerGoliath
{
    [AutoloadBossHead]
    public class PlaguebringerGoliath : ModNPC
    {
        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;
        private const float MissileAngleSpread = 60;
        private const int MissileProjectiles = 8;
        private int MissileCountdown = 0;
        private int despawnTimer = 120;
        private int chargeDistance = 0;
        private bool charging = false;
        private bool halfLife = false;
        private bool canDespawn = false;
        private bool flyingFrame2 = false;
        private int curTex = 1;

        public static readonly SoundStyle NukeWarningSound = new("CalamityMod/Sounds/Custom/PlagueSounds/PBGNukeWarning");
        public static readonly SoundStyle AttackSwitchSound = new("CalamityMod/Sounds/Custom/PlagueSounds/PBGAttackSwitch", 2);
        public static readonly SoundStyle DashSound = new("CalamityMod/Sounds/Custom/PlagueSounds/PBGDash");
        public static readonly SoundStyle BarrageLaunchSound = new("CalamityMod/Sounds/Custom/PlagueSounds/PBGBarrageLaunch");

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Scale = 0.4f,
                PortraitScale = 0.5f,
                PortraitPositionXOverride = -56f,
                PortraitPositionYOverride = -8f,
                SpriteDirection = -1
            };
            value.Position.X -= 48f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
			NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();
            NPC.npcSlots = 64f;
            NPC.width = 198;
            NPC.height = 198;
            NPC.defense = 50;
            NPC.DR_NERD(0.3f);
            NPC.LifeMaxNERB(87500, 105000, 370000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.boss = true;
            NPC.value = Item.buyPrice(0, 75, 0, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundJungle,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.PlaguebringerGoliath")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(biomeEnrageTimer);
            writer.Write(halfLife);
            writer.Write(canDespawn);
            writer.Write(flyingFrame2);
            writer.Write(MissileCountdown);
            writer.Write(despawnTimer);
            writer.Write(chargeDistance);
            writer.Write(charging);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            biomeEnrageTimer = reader.ReadInt32();
            halfLife = reader.ReadBoolean();
            canDespawn = reader.ReadBoolean();
            flyingFrame2 = reader.ReadBoolean();
            MissileCountdown = reader.ReadInt32();
            despawnTimer = reader.ReadInt32();
            chargeDistance = reader.ReadInt32();
            charging = reader.ReadBoolean();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            // Drawcode adjustments for the new sprite
            NPC.gfxOffY = charging ? -40 : -50;
            NPC.width = NPC.frame.Width / 2;
            NPC.height = (int)(NPC.frame.Height * (charging ? 1.5f : 1.8f));

            // Mode variables
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool expertMode = Main.expertMode || bossRush;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases
            bool phase2 = lifeRatio < 0.75f;
            bool phase3 = lifeRatio < 0.5f;
            bool phase4 = lifeRatio < 0.25f;
            bool phase5 = lifeRatio < 0.1f;

            // Adjusts how 'challenging' the projectiles and enemies are to deal with
            float challengeAmt = (1f - lifeRatio) * 100f;
            float nukeBarrageChallengeAmt = (0.5f - lifeRatio) * 200f;

            if (Main.getGoodWorld)
            {
                challengeAmt *= 1.5f;
                nukeBarrageChallengeAmt *= 1.5f;
            }

            // Adjust slowing debuff immunity
            bool immuneToSlowingDebuffs = NPC.ai[0] == 0f || NPC.ai[0] == 4f;
            NPC.buffImmune[ModContent.BuffType<GlacialState>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<TemporalSadness>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<KamiFlu>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<Eutrophication>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<TimeDistortion>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<GalvanicCorrosion>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<Vaporfied>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[BuffID.Slow] = immuneToSlowingDebuffs;
            NPC.buffImmune[BuffID.Webbed] = immuneToSlowingDebuffs;

            // Light
            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0.3f, 0.7f, 0f);

            // Show message
            if (!halfLife && phase3 && expertMode)
            {
                string key = "Mods.CalamityMod.Status.Boss.PlagueBossText";
                Color messageColor = Color.Lime;
                CalamityUtils.DisplayLocalizedText(key, messageColor);
                SoundEngine.PlaySound(NukeWarningSound, NPC.Center);

                halfLife = true;
            }

            // Missile countdown
            if (halfLife && MissileCountdown == 0)
                MissileCountdown = (CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 300 : 600;
            if (MissileCountdown > 1)
                MissileCountdown--;

            Vector2 vectorCenter = NPC.Center;

            // Count nearby players
            int num1038 = 0;
            for (int num1039 = 0; num1039 < Main.maxPlayers; num1039++)
            {
                if (Main.player[num1039].active && !Main.player[num1039].dead && (vectorCenter - Main.player[num1039].Center).Length() < 1000f)
                    num1038++;
            }

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            // Distance from target
            Vector2 distFromPlayer = player.Center - vectorCenter;

            // Enrage
            if (!player.ZoneJungle && !bossRush)
            {
                if (biomeEnrageTimer > 0)
                    biomeEnrageTimer--;
            }
            else
                biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

            bool biomeEnraged = biomeEnrageTimer <= 0 || bossRush;

            float enrageScale = death ? 0.5f : 0f;
            if (biomeEnraged)
            {
                NPC.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 1.5f;
            }

            if (enrageScale > 1.5f)
                enrageScale = 1.5f;

            if (Main.getGoodWorld)
                enrageScale += 0.5f;

            if (bossRush)
                enrageScale = 2f;

            bool diagonalDash = (revenge && phase2) || bossRush;

            if (NPC.ai[0] != 0f && NPC.ai[0] != 4f)
                NPC.rotation = NPC.velocity.X * 0.02f;

            // Despawn
            if (!player.active || player.dead || Vector2.Distance(player.Center, vectorCenter) > 5600f)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead || Vector2.Distance(player.Center, vectorCenter) > 5600f)
                {
                    if (despawnTimer > 0)
                        despawnTimer--;
                }
            }
            else
                despawnTimer = 120;

            canDespawn = despawnTimer <= 0;
            if (canDespawn)
            {
                if (NPC.velocity.Y > 3f)
                    NPC.velocity.Y = 3f;
                NPC.velocity.Y -= 0.2f;
                if (NPC.velocity.Y < -16f)
                    NPC.velocity.Y = -16f;

                if (NPC.timeLeft > 60)
                    NPC.timeLeft = 60;

                if (NPC.ai[0] != -1f)
                {
                    NPC.ai[0] = -1f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    MissileCountdown = 0;
                    chargeDistance = 0;
                    NPC.netUpdate = true;
                }
                return;
            }

            // Always start in enemy spawning phase
            if (calamityGlobalNPC.newAI[3] == 0f)
            {
                calamityGlobalNPC.newAI[3] = 1f;
                NPC.ai[0] = 2f;
                NPC.netUpdate = true;
            }

            // Phase switch
            if (NPC.ai[0] == -1f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int num596;
                    do num596 = MissileCountdown == 1 ? 4 : Main.rand.Next(4);
                    while (num596 == NPC.ai[1] || num596 == 1);

                    if (num596 == 0 && diagonalDash && distFromPlayer.Length() < 1800f)
                    {
                        do
                        {
                            switch (Main.rand.Next(3))
                            {
                                case 0:
                                    chargeDistance = 0;
                                    break;
                                case 1:
                                    chargeDistance = 400;
                                    break;
                                case 2:
                                    chargeDistance = -400;
                                    break;
                            }
                        }
                        while (chargeDistance == NPC.ai[3]);

                        NPC.ai[3] = -chargeDistance;
                    }
                    NPC.ai[0] = num596;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.TargetClosest();
                    NPC.netUpdate = true;

                    // Prevent netUpdate from being blocked by the spam counter.
                    // A phase switch sync is a critical operation that must be synced.
                    if (NPC.netSpam >= 10)
                        NPC.netSpam = 9;

                    SoundEngine.PlaySound(AttackSwitchSound, NPC.Center);
                }
            }

            // Charge phase
            else if (NPC.ai[0] == 0f)
            {
                float num1044 = revenge ? 28f : 26f;
                if (phase2)
                    num1044 += 1f;
                if (phase3)
                    num1044 += 1f;
                if (phase4)
                    num1044 += 1f;
                if (phase5)
                    num1044 += 1f;

                num1044 += 2f * enrageScale;

                int num1043 = (int)Math.Ceiling(2f + enrageScale);
                if ((NPC.ai[1] > (2 * num1043) && NPC.ai[1] % 2f == 0f) || distFromPlayer.Length() > 1800f)
                {
                    NPC.ai[0] = -1f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.netUpdate = true;

                    // Prevent netUpdate from being blocked by the spam counter.
                    // A phase switch sync is a critical operation that must be synced.
                    if (NPC.netSpam >= 10)
                        NPC.netSpam = 9;

                    SoundEngine.PlaySound(AttackSwitchSound, NPC.Center);

                    return;
                }

                // Charge
                if (NPC.ai[1] % 2f == 0f)
                {
                    float playerLocation = vectorCenter.X - player.Center.X;

                    float num620 = 20f;
                    num620 += 20f * enrageScale;

                    if (Math.Abs(NPC.Center.Y - (player.Center.Y - chargeDistance)) < num620)
                    {
                        if (diagonalDash)
                        {
                            switch (Main.rand.Next(3))
                            {
                                case 0:
                                    chargeDistance = 0;
                                    break;
                                case 1:
                                    chargeDistance = 400;
                                    break;
                                case 2:
                                    chargeDistance = -400;
                                    break;
                            }
                        }

                        charging = true;
                        NPC.frameCounter = 4;

                        NPC.ai[1] += 1f;
                        NPC.ai[2] = 0f;

                        float num1045 = player.position.X + (player.width / 2) - vectorCenter.X;
                        float num1046 = player.position.Y + (player.height / 2) - vectorCenter.Y;
                        float num1047 = (float)Math.Sqrt(num1045 * num1045 + num1046 * num1046);

                        num1047 = num1044 / num1047;
                        NPC.velocity.X = num1045 * num1047;
                        NPC.velocity.Y = num1046 * num1047;
                        NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);

                        calamityGlobalNPC.newAI[1] = NPC.velocity.X;
                        calamityGlobalNPC.newAI[2] = NPC.velocity.Y;

                        NPC.direction = playerLocation < 0 ? 1 : -1;
                        NPC.spriteDirection = NPC.direction;
                        if (NPC.spriteDirection != 1)
                            NPC.rotation += (float)Math.PI;

                        NPC.netUpdate = true;
                        NPC.netSpam -= 5;

                        SoundEngine.PlaySound(DashSound, NPC.Center);
                        return;
                    }

                    NPC.rotation = NPC.velocity.X * 0.02f;
                    charging = false;

                    float num1048 = revenge ? 14f : 12f;
                    float num1049 = revenge ? 0.25f : 0.22f;
                    if (phase2)
                    {
                        num1048 += 1f;
                        num1049 += 0.05f;
                    }
                    if (phase4)
                    {
                        num1048 += 1f;
                        num1049 += 0.05f;
                    }
                    num1048 += 1.5f * enrageScale;
                    num1049 += 0.25f * enrageScale;

                    if (vectorCenter.Y < (player.Center.Y - chargeDistance))
                        NPC.velocity.Y += num1049;
                    else
                        NPC.velocity.Y -= num1049;

                    if (NPC.velocity.Y < -num1048)
                        NPC.velocity.Y = -num1048;
                    if (NPC.velocity.Y > num1048)
                        NPC.velocity.Y = num1048;

                    if (Math.Abs(vectorCenter.X - player.Center.X) > 650f)
                        NPC.velocity.X += num1049 * NPC.direction;
                    else if (Math.Abs(vectorCenter.X - player.Center.X) < 500f)
                        NPC.velocity.X -= num1049 * NPC.direction;
                    else
                        NPC.velocity.X *= 0.8f;

                    if (NPC.velocity.X < -num1048)
                        NPC.velocity.X = -num1048;
                    if (NPC.velocity.X > num1048)
                        NPC.velocity.X = num1048;

                    NPC.direction = playerLocation < 0 ? 1 : -1;
                    NPC.spriteDirection = NPC.direction;

                    NPC.netUpdate = true;
                    NPC.netSpam -= 5;
                }

                // Slow down after charge
                else
                {
                    if (NPC.velocity.X < 0f)
                        NPC.direction = -1;
                    else
                        NPC.direction = 1;

                    NPC.spriteDirection = NPC.direction;

                    int num1050 = revenge ? 525 : 550;
                    if (phase4)
                        num1050 = revenge ? 450 : 475;
                    else if (phase3)
                        num1050 = revenge ? 475 : 500;
                    else if (phase2)
                        num1050 = revenge ? 500 : 525;
                    num1050 -= (int)(25f * enrageScale);

                    int num1051 = 1;
                    if (vectorCenter.X < player.Center.X)
                        num1051 = -1;

                    if (NPC.direction == num1051 && (Math.Abs(vectorCenter.X - player.Center.X) > num1050 || Math.Abs(vectorCenter.Y - player.Center.Y) > num1050))
                        NPC.ai[2] = 1f;

                    if (enrageScale > 0 && NPC.ai[2] == 1f)
                        NPC.velocity *= 0.95f;

                    if (NPC.ai[2] != 1f)
                    {
                        charging = true;
                        NPC.frameCounter = 4;

                        // Velocity fix if PBG slowed
                        if (NPC.velocity.Length() < num1044)
                            NPC.velocity = new Vector2(calamityGlobalNPC.newAI[1], calamityGlobalNPC.newAI[2]);

                        calamityGlobalNPC.newAI[0] += 1f;
                        if (calamityGlobalNPC.newAI[0] > 90f)
                            NPC.velocity *= 1.01f;

                        // Spawn honey in legendary rev+
                        if (CalamityWorld.LegendaryMode && CalamityWorld.revenge && calamityGlobalNPC.newAI[0] % 6f == 0f)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                try
                                {
                                    int num430 = (int)(NPC.Center.X / 16f);
                                    int num431 = (int)(NPC.Center.Y / 16f);
                                    if (!WorldGen.SolidTile(num430, num431) && Main.tile[num430, num431].LiquidAmount == 0)
                                    {
                                        Main.tile[num430, num431].LiquidAmount = (byte)Main.rand.Next(50, 150);
                                        Main.tile[num430, num431].Get<LiquidData>().LiquidType = LiquidID.Honey;
                                        WorldGen.SquareTileFrame(num430, num431);
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }

                        NPC.netUpdate = true;
                        return;
                    }

                    float playerLocation = vectorCenter.X - player.Center.X;
                    NPC.direction = playerLocation < 0 ? 1 : -1;
                    NPC.spriteDirection = NPC.direction;

                    NPC.rotation = NPC.velocity.X * 0.02f;
                    charging = false;

                    NPC.velocity *= 0.9f;
                    float num1052 = revenge ? 0.12f : 0.1f;
                    if (phase2)
                    {
                        NPC.velocity *= 0.98f;
                        num1052 += 0.05f;
                    }
                    if (phase3)
                    {
                        NPC.velocity *= 0.98f;
                        num1052 += 0.05f;
                    }
                    if (phase4)
                    {
                        NPC.velocity *= 0.98f;
                        num1052 += 0.05f;
                    }
                    if (enrageScale > 0)
                        NPC.velocity *= 0.95f;

                    if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < num1052)
                    {
                        NPC.ai[2] = 0f;
                        NPC.ai[1] += 1f;
                        calamityGlobalNPC.newAI[0] = 0f;
                    }
                }
            }

            // Move closer if too far away
            else if (NPC.ai[0] == 2f)
            {
                float playerLocation = vectorCenter.X - player.Center.X;
                NPC.direction = playerLocation < 0 ? 1 : -1;
                NPC.spriteDirection = NPC.direction;

                float num1055 = player.position.X + (player.width / 2) - vectorCenter.X;
                float num1056 = player.position.Y + (player.height / 2) - 200f - vectorCenter.Y;
                float num1057 = (float)Math.Sqrt(num1055 * num1055 + num1056 * num1056);

                calamityGlobalNPC.newAI[0] += 1f;
                if (num1057 < 600f || calamityGlobalNPC.newAI[0] >= 180f)
                {
                    NPC.ai[0] = (phase3 || bossRush) ? 5f : 1f;
                    NPC.ai[1] = 0f;
                    calamityGlobalNPC.newAI[0] = 0f;
                    NPC.netUpdate = true;

                    // Prevent netUpdate from being blocked by the spam counter.
                    // A phase switch sync is a critical operation that must be synced.
                    if (NPC.netSpam >= 10)
                        NPC.netSpam = 9;

                    SoundEngine.PlaySound(AttackSwitchSound, NPC.Center);

                    return;
                }

                // Move closer
                Movement(100f, 350f, 450f, player, enrageScale);
            }

            // Spawn less missiles
            else if (NPC.ai[0] == 1f)
            {
                charging = false;
                Vector2 vector119 = new Vector2(NPC.direction == 1 ? NPC.getRect().BottomLeft().X : NPC.getRect().BottomRight().X, NPC.getRect().Bottom().Y + 20f);
                vector119.X += NPC.direction * 120;
                float num1058 = player.position.X + (player.width / 2) - vectorCenter.X;
                float num1059 = player.position.Y + (player.height / 2) - vectorCenter.Y;
                float num1060 = (float)Math.Sqrt(num1058 * num1058 + num1059 * num1059);

                NPC.ai[1] += 1f;
                NPC.ai[1] += num1038 / 2;
                if (phase2)
                    NPC.ai[1] += 0.5f;

                bool flag103 = false;
                if (NPC.ai[1] > 40f - 12f * enrageScale)
                {
                    NPC.ai[1] = 0f;
                    NPC.ai[2] += 1f;
                    flag103 = true;
                }

                if (flag103)
                {
                    SoundEngine.PlaySound(SoundID.NPCHit8, NPC.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (expertMode && NPC.CountNPCS(ModContent.NPCType<PlagueMine>()) < 2)
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)vector119.X, (int)vector119.Y, ModContent.NPCType<PlagueMine>(), 0, 0f, 0f, 0f, challengeAmt);

                        float npcSpeed = (revenge ? 9f : 7f) + enrageScale * 2f;

                        float num1071 = player.position.X + player.width * 0.5f - vector119.X;
                        float num1072 = player.position.Y + player.height * 0.5f - vector119.Y;
                        float num1073 = (float)Math.Sqrt(num1071 * num1071 + num1072 * num1072);

                        num1073 = npcSpeed / num1073;
                        num1071 *= num1073;
                        num1072 *= num1073;

                        int num1062 = NPC.NewNPC(NPC.GetSource_FromAI(), (int)vector119.X, (int)vector119.Y, ModContent.NPCType<PlagueHomingMissile>(), 0, 0f, 0f, 0f, challengeAmt);
                        Main.npc[num1062].velocity.X = num1071;
                        Main.npc[num1062].velocity.Y = num1072;
                        Main.npc[num1062].netUpdate = true;
                    }
                }

                // Move closer if too far away
                if (num1060 > 600f)
                    Movement(100f, 350f, 450f, player, enrageScale);
                else
                    NPC.velocity *= 0.9f;

                float playerLocation = vectorCenter.X - player.Center.X;
                NPC.direction = playerLocation < 0 ? 1 : -1;
                NPC.spriteDirection = NPC.direction;

                if (NPC.ai[2] > 3f)
                {
                    NPC.ai[0] = -1f;
                    NPC.ai[1] = 2f;
                    NPC.ai[2] = 0f;
                    NPC.netUpdate = true;

                    // Prevent netUpdate from being blocked by the spam counter.
                    // A phase switch sync is a critical operation that must be synced.
                    if (NPC.netSpam >= 10)
                        NPC.netSpam = 9;

                    SoundEngine.PlaySound(AttackSwitchSound, NPC.Center);
                }
            }

            // Missile spawn
            else if (NPC.ai[0] == 5f)
            {
                charging = false;
                Vector2 vector119 = new Vector2(NPC.direction == 1 ? NPC.getRect().BottomLeft().X : NPC.getRect().BottomRight().X, NPC.getRect().Bottom().Y + 20f);
                vector119.X += NPC.direction * 120;
                float num1058 = player.position.X + (player.width / 2) - vectorCenter.X;
                float num1059 = player.position.Y + (player.height / 2) - vectorCenter.Y;
                float num1060 = (float)Math.Sqrt(num1058 * num1058 + num1059 * num1059);

                NPC.ai[1] += 1f;
                NPC.ai[1] += num1038 / 2;
                bool flag103 = false;
                if (phase4)
                    NPC.ai[1] += 0.5f;
                if (phase5)
                    NPC.ai[1] += 0.5f;

                if (NPC.ai[1] % 20f == 19f)
                    NPC.netUpdate = true;

                if (NPC.ai[1] > 30f - 12f * enrageScale)
                {
                    NPC.ai[1] = 0f;
                    NPC.ai[2] += 1f;
                    flag103 = true;
                }

                if (flag103)
                {
                    SoundEngine.PlaySound(SoundID.Item88,NPC.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (expertMode && NPC.CountNPCS(ModContent.NPCType<PlagueMine>()) < 3)
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)vector119.X, (int)vector119.Y, ModContent.NPCType<PlagueMine>(), 0, 0f, 0f, 0f, challengeAmt);

                        float npcSpeed = (revenge ? 11f : 9f) + enrageScale * 2f;

                        float num1071 = player.position.X + player.width * 0.5f - vector119.X;
                        float num1072 = player.position.Y + player.height * 0.5f - vector119.Y;
                        float num1073 = (float)Math.Sqrt(num1071 * num1071 + num1072 * num1072);

                        num1073 = npcSpeed / num1073;
                        num1071 *= num1073;
                        num1072 *= num1073;
                        num1071 += Main.rand.Next(-20, 21) * 0.05f;
                        num1072 += Main.rand.Next(-20, 21) * 0.05f;

                        int num1062 = NPC.NewNPC(NPC.GetSource_FromAI(), (int)vector119.X, (int)vector119.Y, ModContent.NPCType<PlagueHomingMissile>(), 0, 0f, 0f, 0f, challengeAmt);
                        Main.npc[num1062].velocity.X = num1071;
                        Main.npc[num1062].velocity.Y = num1072;
                        Main.npc[num1062].netUpdate = true;
                    }
                }

                // Move closer if too far away
                if (num1060 > 600f)
                    Movement(100f, 350f, 450f, player, enrageScale);
                else
                    NPC.velocity *= 0.9f;

                float playerLocation = vectorCenter.X - player.Center.X;
                NPC.direction = playerLocation < 0 ? 1 : -1;
                NPC.spriteDirection = NPC.direction;

                if (NPC.ai[2] > ((CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 3f : 5f))
                {
                    NPC.ai[0] = -1f;
                    NPC.ai[1] = 2f;
                    NPC.ai[2] = 0f;
                    NPC.netUpdate = true;

                    // Prevent netUpdate from being blocked by the spam counter.
                    // A phase switch sync is a critical operation that must be synced.
                    if (NPC.netSpam >= 10)
                        NPC.netSpam = 9;

                    SoundEngine.PlaySound(AttackSwitchSound, NPC.Center);
                }
            }

            // Stinger phase
            else if (NPC.ai[0] == 3f)
            {
                Vector2 vector121 = new Vector2(NPC.direction == 1 ? NPC.getRect().BottomLeft().X : NPC.getRect().BottomRight().X, NPC.getRect().Bottom().Y + 20f);
                vector121.X += NPC.direction * 120;

                NPC.ai[1] += 1f;
                int num650 = phase5 ? 20 : (phase3 ? 25 : 30);
                num650 -= (int)Math.Ceiling(5f * enrageScale);

                if (NPC.ai[1] % num650 == (num650 - 1) && vectorCenter.Y < player.position.Y)
                {
                    SoundEngine.PlaySound(SoundID.Item42, NPC.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float projectileSpeed = revenge ? 6f : 5f;
                        projectileSpeed += 2f * enrageScale;

                        float num1071 = player.position.X + player.width * 0.5f - vector121.X;
                        float num1072 = player.position.Y + player.height * 0.5f - vector121.Y;
                        float num1073 = (float)Math.Sqrt(num1071 * num1071 + num1072 * num1072);
                        num1073 = projectileSpeed / num1073;
                        num1071 *= num1073;
                        num1072 *= num1073;

                        int type = ModContent.ProjectileType<PlagueStingerGoliathV2>();
                        switch ((int)NPC.ai[2])
                        {
                            case 0:
                            case 1:
                                break;
                            case 2:
                            case 3:
                                if (expertMode)
                                    type = ModContent.ProjectileType<PlagueStingerGoliath>();
                                break;
                            case 4:
                                type = ModContent.ProjectileType<HiveBombGoliath>();
                                break;
                        }

                        if (Main.zenithWorld)
                        {
                            type = ModContent.ProjectileType<HiveBombGoliath>();
                        }

                        int damage = NPC.GetProjectileDamage(type);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), vector121.X, vector121.Y, num1071, num1072, type, damage, 0f, Main.myPlayer, challengeAmt, player.position.Y);
                        NPC.netUpdate = true;
                    }

                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] > 4f)
                        NPC.ai[2] = 0f;
                }

                Movement(100f, 400f, 500f, player, enrageScale);

                float playerLocation = vectorCenter.X - player.Center.X;
                NPC.direction = playerLocation < 0 ? 1 : -1;
                NPC.spriteDirection = NPC.direction;

                if (NPC.ai[1] > num650 * 10f)
                {
                    NPC.ai[0] = -1f;
                    NPC.ai[1] = 3f;
                    NPC.ai[2] = 0f;
                    NPC.netUpdate = true;

                    // Prevent netUpdate from being blocked by the spam counter.
                    // A phase switch sync is a critical operation that must be synced.
                    if (NPC.netSpam >= 10)
                        NPC.netSpam = 9;

                    SoundEngine.PlaySound(AttackSwitchSound, NPC.Center);
                }
            }

            // Missile charge
            else if (NPC.ai[0] == 4f)
            {
                float num1044 = revenge ? 28f : 26f;

                num1044 += 3f * enrageScale;

                int num1043 = (int)Math.Ceiling(2f + enrageScale);
                if (NPC.ai[1] > (2 * num1043) && NPC.ai[1] % 2f == 0f)
                {
                    MissileCountdown = 0;
                    NPC.ai[0] = -1f;
                    NPC.ai[1] = -1f;
                    NPC.ai[2] = 0f;
                    NPC.netUpdate = true;

                    // Prevent netUpdate from being blocked by the spam counter.
                    // A phase switch sync is a critical operation that must be synced.
                    if (NPC.netSpam >= 10)
                        NPC.netSpam = 9;
                    SoundEngine.PlaySound(AttackSwitchSound, NPC.Center);
                    return;
                }

                // Charge
                if (NPC.ai[1] % 2f == 0f)
                {
                    float playerLocation = vectorCenter.X - player.Center.X;

                    float num620 = 20f;
                    num620 += 20 * enrageScale;

                    if (Math.Abs(vectorCenter.Y - (player.Center.Y - 500f)) < num620)
                    {
                        if (MissileCountdown == 1)
                        {
                            SoundEngine.PlaySound(BarrageLaunchSound, NPC.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float speed = revenge ? 6f : 5f;
                                speed += 2f * enrageScale;

                                bool gaussMode = false;

                                int type = ModContent.ProjectileType<HiveBombGoliath>();
                                int damage = NPC.GetProjectileDamage(type);

                                Vector2 baseVelocity = player.Center - vectorCenter;
                                baseVelocity.Normalize();
                                baseVelocity *= speed;

                                if (Main.rand.NextBool(10) && Main.zenithWorld)
                                {
                                    type = ModContent.ProjectileType<AresGaussNukeProjectile>();
                                    baseVelocity *= 0.75f;
                                    gaussMode = true;
                                }
                                else if (Main.rand.NextBool(2) && Main.zenithWorld)
                                {
                                    type = ModContent.ProjectileType<PeanutRocket>();
                                    baseVelocity *= 0.4f;
                                }

                                int missiles = bossRush ? 16 : MissileProjectiles;
                                int spread = bossRush ? 18 : 24;
                                if (!gaussMode)
                                {
                                    for (int i = 0; i < missiles; i++)
                                    {
                                        Vector2 spawn = vectorCenter; // Normal = 96, Boss Rush = 144
                                        spawn.X += i * (int)(spread * 1.125) - (missiles * (spread / 2)); // Normal = -96 to 93, Boss Rush = -144 to 156
                                        Vector2 velocity = baseVelocity.RotatedBy(MathHelper.ToRadians(-MissileAngleSpread / 2 + (MissileAngleSpread * i / missiles)));
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), spawn, velocity, type, damage, 0f, Main.myPlayer, nukeBarrageChallengeAmt, player.position.Y);
                                    }
                                }
                                else
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, baseVelocity, type, damage, 0f, Main.myPlayer);
                                }
                            }
                        }

                        charging = true;

                        NPC.ai[1] += 1f;
                        NPC.ai[2] = 0f;

                        float num1045 = player.position.X + (player.width / 2) - vectorCenter.X;
                        float num1046 = player.position.Y - 500f + (player.height / 2) - vectorCenter.Y;
                        float num1047 = (float)Math.Sqrt(num1045 * num1045 + num1046 * num1046);

                        num1047 = num1044 / num1047;
                        NPC.velocity.X = num1045 * num1047;
                        NPC.velocity.Y = num1046 * num1047;
                        NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);

                        NPC.direction = playerLocation < 0 ? 1 : -1;
                        NPC.spriteDirection = NPC.direction;
                        if (NPC.spriteDirection != 1)
                            NPC.rotation += (float)Math.PI;

                        NPC.netUpdate = true;

                        return;
                    }

                    NPC.rotation = NPC.velocity.X * 0.02f;
                    charging = false;

                    float num1048 = revenge ? 16f : 14f;
                    float num1049 = revenge ? 0.2f : 0.18f;
                    num1048 += 1.5f * enrageScale;
                    num1049 += 0.25f * enrageScale;

                    if (vectorCenter.Y < player.Center.Y - 500f)
                        NPC.velocity.Y += num1049;
                    else
                        NPC.velocity.Y -= num1049;

                    if (NPC.velocity.Y < -num1048)
                        NPC.velocity.Y = -num1048;
                    if (NPC.velocity.Y > num1048)
                        NPC.velocity.Y = num1048;

                    if (Math.Abs(vectorCenter.X - player.Center.X) > 600f)
                        NPC.velocity.X += num1049 * NPC.direction;
                    else if (Math.Abs(vectorCenter.X - player.Center.X) < 300f)
                        NPC.velocity.X -= num1049 * NPC.direction;
                    else
                        NPC.velocity.X *= 0.8f;

                    if (NPC.velocity.X < -num1048)
                        NPC.velocity.X = -num1048;
                    if (NPC.velocity.X > num1048)
                        NPC.velocity.X = num1048;

                    NPC.direction = playerLocation < 0 ? 1 : -1;
                    NPC.spriteDirection = NPC.direction;
                }

                // Slow down after charge
                else
                {
                    if (NPC.velocity.X < 0f)
                        NPC.direction = -1;
                    else
                        NPC.direction = 1;

                    NPC.spriteDirection = NPC.direction;

                    int num1050 = 600;
                    int num1051 = 1;

                    if (vectorCenter.X < player.Center.X)
                        num1051 = -1;
                    if (NPC.direction == num1051 && Math.Abs(vectorCenter.X - player.Center.X) > num1050)
                        NPC.ai[2] = 1f;
                    if (enrageScale > 0 && NPC.ai[2] == 1f)
                        NPC.velocity *= 0.95f;

                    if (NPC.ai[2] != 1f)
                    {
                        charging = true;

                        // Velocity fix if PBG slowed
                        if (NPC.velocity.Length() < num1044)
                            NPC.velocity.X = num1044 * NPC.direction;

                        calamityGlobalNPC.newAI[0] += 1f;
                        if (calamityGlobalNPC.newAI[0] > 90f)
                            NPC.velocity.X *= 1.01f;

                        return;
                    }

                    NPC.rotation = NPC.velocity.X * 0.02f;
                    charging = false;

                    NPC.velocity *= 0.9f;
                    float num1052 = revenge ? 0.12f : 0.1f;
                    if (phase3)
                    {
                        NPC.velocity *= 0.9f;
                        num1052 += 0.05f;
                    }
                    if (phase4)
                    {
                        NPC.velocity *= 0.9f;
                        num1052 += 0.05f;
                    }
                    if (phase5)
                    {
                        NPC.velocity *= 0.9f;
                        num1052 += 0.05f;
                    }
                    if (enrageScale > 0)
                        NPC.velocity *= 0.95f;

                    if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < num1052)
                    {
                        NPC.ai[2] = 0f;
                        NPC.ai[1] += 1f;
                        calamityGlobalNPC.newAI[0] = 0f;
                    }
                }
            }
        }

        private void Movement(float xPos, float yPos, float yPos2, Player player, float enrageScale)
        {
            Vector2 acceleration = new Vector2(0.1f, 0.15f);
            Vector2 velocity = new Vector2(8f, 5f);
            float deceleration = 0.9f;

            acceleration *= 0.1f * enrageScale + 1f;
            velocity *= 1f - enrageScale * 0.1f;
            if (BossRushEvent.BossRushActive)
                velocity *= 0.5f;
            deceleration *= 1f - enrageScale * 0.1f;

            if (NPC.position.Y > player.position.Y - yPos)
            {
                if (NPC.velocity.Y > 0f)
                    NPC.velocity.Y *= deceleration;
                NPC.velocity.Y -= acceleration.Y;
                if (NPC.velocity.Y > velocity.Y)
                    NPC.velocity.Y = velocity.Y;
            }
            else if (NPC.position.Y < player.position.Y - yPos2)
            {
                if (NPC.velocity.Y < 0f)
                    NPC.velocity.Y *= deceleration;
                NPC.velocity.Y += acceleration.Y;
                if (NPC.velocity.Y < -velocity.Y)
                    NPC.velocity.Y = -velocity.Y;
            }

            if (NPC.position.X + (NPC.width / 2) > player.position.X + (player.width / 2) + xPos)
            {
                if (NPC.velocity.X > 0f)
                    NPC.velocity.X *= deceleration;
                NPC.velocity.X -= acceleration.X;
                if (NPC.velocity.X > velocity.X)
                    NPC.velocity.X = velocity.X;
            }
            if (NPC.position.X + (NPC.width / 2) < player.position.X + (player.width / 2) - xPos)
            {
                if (NPC.velocity.X < 0f)
                    NPC.velocity.X *= deceleration;
                NPC.velocity.X += acceleration.X;
                if (NPC.velocity.X < -velocity.X)
                    NPC.velocity.X = -velocity.X;
            }
        }

        public override bool CheckActive() => canDespawn;

        // Can only hit the target if within certain distance
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            Rectangle targetHitbox = target.Hitbox;

            float dist1 = Vector2.Distance(NPC.Center, targetHitbox.TopLeft());
            float dist2 = Vector2.Distance(NPC.Center, targetHitbox.TopRight());
            float dist3 = Vector2.Distance(NPC.Center, targetHitbox.BottomLeft());
            float dist4 = Vector2.Distance(NPC.Center, targetHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            return minDist <= 100f * NPC.scale;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 2; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Plague, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 1; i < 7; i++)
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("PlaguebringerGoliathGore" + i).Type, NPC.scale);
                }
                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = 200;
                NPC.height = 200;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Plague, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Plague, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Plague, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glowTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/PlaguebringerGoliath/PlaguebringerGoliathGlow").Value;
            if (curTex != (charging ? 2 : 1))
            {
                NPC.frame.X = 0;
                NPC.frame.Y = 0;
            }
            if (charging)
            {
                curTex = 2;
                texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/PlaguebringerGoliath/PlaguebringerGoliathChargeTex").Value;
                glowTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/PlaguebringerGoliath/PlaguebringerGoliathChargeTexGlow").Value;
            }
            else
            {
                curTex = 1;
            }

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            int frameCount = 3;
            Rectangle rectangle = new Rectangle(NPC.frame.X, NPC.frame.Y, texture.Width / 2, texture.Height / frameCount);
            Vector2 vector11 = rectangle.Size() / 2f;
            Vector2 posOffset = new Vector2(charging ? 175 : 125, 0);
            Color color36 = Color.White;
            float amount9 = 0.5f;
            int num153 = 10;
            if (NPC.ai[0] != 0f && NPC.ai[0] != 4f)
                num153 = 7;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += 2)
                {
                    Color color38 = drawColor;
                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = NPC.GetAlpha(color38);
                    color38 *= (num153 - num155) / 15f;
                    Vector2 vector41 = NPC.oldPos[num155] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    vector41 -= new Vector2(texture.Width, texture.Height / frameCount) * NPC.scale / 2f;
                    vector41 += vector11 * NPC.scale + posOffset;
                    spriteBatch.Draw(texture, vector41, new Rectangle?(rectangle), color38, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 vector43 = NPC.Center - screenPos;
            vector43 -= new Vector2(texture.Width, texture.Height / frameCount) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + posOffset;
            spriteBatch.Draw(texture, vector43, new Rectangle?(rectangle), NPC.GetAlpha(drawColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            Color color37 = Color.Lerp(Color.White, Color.Red, 0.5f);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num163 = 1; num163 < num153; num163++)
                {
                    Color color41 = color37;
                    color41 = Color.Lerp(color41, color36, amount9);
                    color41 *= (num153 - num163) / 15f;
                    Vector2 vector44 = NPC.oldPos[num163] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    vector44 -= new Vector2(glowTexture.Width, glowTexture.Height / frameCount) * NPC.scale / 2f;
                    vector44 += vector11 * NPC.scale + posOffset;
                    spriteBatch.Draw(glowTexture, vector44, new Rectangle?(rectangle), color41, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(glowTexture, vector43, new Rectangle?(rectangle), color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            int width = !charging ? (532 / 2) : (644 / 2);
            int height = !charging ? (768 / 3) : (636 / 3);
            NPC.frameCounter += 1.0;

            if (NPC.frameCounter > 4.0)
            {
                NPC.frame.Y = NPC.frame.Y + height;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= height * 3)
            {
                NPC.frame.Y = 0;
                NPC.frame.X = NPC.frame.X == 0 ? width : 0;
                if (charging)
                {
                   flyingFrame2 = !flyingFrame2;
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void OnKill()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            // Mark PBG as dead
            DownedBossSystem.downedPlaguebringer = true;
            CalamityNetcode.SyncWorld();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<PlaguebringerGoliathBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<Virulence>(),
                    ModContent.ItemType<DiseasedPike>(),
                    ModContent.ItemType<Pandemic>(),
                    ModContent.ItemType<Malevolence>(),
                    ModContent.ItemType<PestilentDefiler>(),
                    ModContent.ItemType<TheHive>(),
                    ModContent.ItemType<BlightSpewer>(),
                    ModContent.ItemType<PlagueStaff>(),
                    ModContent.ItemType<FuelCellBundle>(),
                    ModContent.ItemType<InfectedRemote>(),
                    ModContent.ItemType<TheSyringe>(),
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, weapons));
                normalOnly.Add(ModContent.ItemType<Malachite>(), 10);

                // Materials
                normalOnly.Add(ItemID.Stinger, 1, 3, 5);
                normalOnly.Add(ModContent.ItemType<PlagueCellCanister>(), 1, 15, 20);
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<InfectedArmorPlating>(), 1, 25, 30));

                // Equipment
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<ToxicHeart>()));

                // Vanity
                normalOnly.Add(ModContent.ItemType<PlaguebringerGoliathMask>(), 7);
                normalOnly.Add(ModContent.ItemType<PlagueCaller>(), 10);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
            }

            npcLoot.Add(ModContent.ItemType<PlaguebringerGoliathTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<PlaguebringerGoliathRelic>());

            // GFB Honey Bucket drop
            npcLoot.DefineConditionalDropSet(DropHelper.GFB).Add(ItemID.BottomlessHoneyBucket);

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedPlaguebringer, ModContent.ItemType<LorePlaguebringerGoliath>(), desc: DropHelper.FirstKillText);
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
            {
                if (Main.zenithWorld) // it is the plague, you get very sick.
                {
                    target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 480, true);
                    target.AddBuff(BuffID.Poisoned, 480, true);
                    target.AddBuff(BuffID.Venom, 480, true);
                }
                target.AddBuff(ModContent.BuffType<Plague>(), 480, true);
            }
        }
    }
}

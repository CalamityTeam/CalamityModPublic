using System;
using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.Paintings;
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
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.PlaguebringerGoliath
{
    [AutoloadBossHead]
    public class PlaguebringerGoliath : ModNPC
    {
        // Use for all projectiles and spawned enemies.
        public static Color BackglowColor => new Color(255, 100, 24, 80);

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

        public static Asset<Texture2D> ChargeTexture;
        public static Asset<Texture2D> Texture_Glow;
        public static Asset<Texture2D> ChargeTexture_Glow;

        public static readonly SoundStyle NukeWarningSound = new("CalamityMod/Sounds/Custom/PlagueSounds/PBGNukeWarning");
        public static readonly SoundStyle AttackSwitchSound = new("CalamityMod/Sounds/Custom/PlagueSounds/PBGAttackSwitch", 2);
        public static readonly SoundStyle DashSound = new("CalamityMod/Sounds/Custom/PlagueSounds/PBGDash");
        public static readonly SoundStyle BarrageLaunchSound = new("CalamityMod/Sounds/Custom/PlagueSounds/PBGBarrageLaunch");

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
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
            if (!Main.dedServ)
            {
                ChargeTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/PlaguebringerGoliath/PlaguebringerGoliathChargeTex", AssetRequestMode.AsyncLoad);
                Texture_Glow = ModContent.Request<Texture2D>("CalamityMod/NPCs/PlaguebringerGoliath/PlaguebringerGoliathGlow", AssetRequestMode.AsyncLoad);
                ChargeTexture_Glow = ModContent.Request<Texture2D>("CalamityMod/NPCs/PlaguebringerGoliath/PlaguebringerGoliathChargeTexGlow", AssetRequestMode.AsyncLoad);
            }
        }

        public static int StingerDamage = 25; // 100
        public static int NukeDamage = 30; // 120; Also applies to GFB Peanuts and Gauss Nukes

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 75; // 150
            NPC.npcSlots = 64f;
            NPC.width = 198;
            NPC.height = 198;
            NPC.defense = 50;
            NPC.DR_NERD(0.3f);
            NPC.LifeMaxNERB(70000, 105000, 370000);
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.boss = true;
            NPC.value = Item.buyPrice(gold: 25);
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
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

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

            // Light
            Lighting.AddLight((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f), 0.3f, 0.7f, 0f);

            // Show message
            if (!halfLife && phase3 && expertMode)
            {
                string key = "Mods.CalamityMod.Status.Boss.PlagueBossText";
                Color messageColor = Color.Lime;
                CalamityUtils.BroadcastLocalizedText(key, messageColor);
                SoundEngine.PlaySound(NukeWarningSound, NPC.Center);

                halfLife = true;
            }

            // Missile countdown
            if (halfLife && MissileCountdown == 0)
                MissileCountdown = Main.getGoodWorld ? 300 : 600;
            if (MissileCountdown > 1)
                MissileCountdown--;

            // Count nearby players
            int activePlayers = Main.CurrentFrameFlags.ActivePlayersCount;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            // Distance from target
            Vector2 distFromPlayer = player.Center - NPC.Center;

            // Enrage
            if (!player.ZoneJungle && !BossRushEvent.BossRushActive)
            {
                if (biomeEnrageTimer > 0)
                    biomeEnrageTimer--;
            }
            else
                biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

            bool biomeEnraged = biomeEnrageTimer <= 0;

            float enrageScale = death ? 0.5f : 0f;
            if (biomeEnraged)
            {
                NPC.Calamity().CurrentlyEnraged = true;
                enrageScale += 1.5f;
            }

            if (enrageScale > 1.5f)
                enrageScale = 1.5f;

            if (Main.getGoodWorld)
                enrageScale += 0.5f;

            bool diagonalDash = (revenge && phase2);

            if (NPC.ai[0] != 0f && NPC.ai[0] != 4f)
                NPC.rotation = NPC.velocity.X * 0.02f;

            // Despawn
            if (!player.active || player.dead || Vector2.Distance(player.Center, NPC.Center) > 5600f)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead || Vector2.Distance(player.Center, NPC.Center) > 5600f)
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
                // Avoid cheap bullshit
                NPC.damage = 0;

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
                    int attackSwitch;
                    do attackSwitch = MissileCountdown == 1 ? 4 : Main.rand.Next(4);
                    while (attackSwitch == NPC.ai[1] || attackSwitch == 1);

                    if (attackSwitch == 0 && diagonalDash && distFromPlayer.Length() < 1800f)
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
                    NPC.ai[0] = attackSwitch;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.TargetClosest();

                    // Prevent netUpdate from being blocked by the spam counter.
                    // A phase switch sync is a critical operation that must be synced.
                    NPC.ForceNetUpdate(false);

                    SoundEngine.PlaySound(AttackSwitchSound, NPC.Center);
                }
            }

            // Charge phase
            else if (NPC.ai[0] == 0f)
            {
                int chargeDistanceX = revenge ? 525 : 550;
                if (phase4)
                    chargeDistanceX = revenge ? 450 : 475;
                else if (phase3)
                    chargeDistanceX = revenge ? 475 : 500;
                else if (phase2)
                    chargeDistanceX = revenge ? 500 : 525;
                chargeDistanceX -= (int)(25f * enrageScale);

                float chargeSpeed = revenge ? 28f : 26f;
                if (phase2)
                    chargeSpeed += 1f;
                if (phase3)
                    chargeSpeed += 1f;
                if (phase4)
                    chargeSpeed += 1f;
                if (phase5)
                    chargeSpeed += 1f;

                chargeSpeed += 2f * enrageScale;

                int phaseSwitchTimer = (int)Math.Ceiling(2f + enrageScale);
                if ((NPC.ai[1] > (2 * phaseSwitchTimer) && NPC.ai[1] % 2f == 0f) || distFromPlayer.Length() > 1800f)
                {
                    NPC.ai[0] = -1f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;

                    // Prevent netUpdate from being blocked by the spam counter.
                    // A phase switch sync is a critical operation that must be synced.
                    NPC.ForceNetUpdate(false);

                    SoundEngine.PlaySound(AttackSwitchSound, NPC.Center);

                    return;
                }

                // Charge
                if (NPC.ai[1] % 2f == 0f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    float playerLocation = NPC.Center.X - player.Center.X;

                    float chargeDistanceY = 20f;
                    chargeDistanceY += 20f * enrageScale;

                    float distanceFromTargetX = Math.Abs(NPC.Center.X - player.Center.X);
                    float distanceFromTargetY = Math.Abs(NPC.Center.Y - (player.Center.Y - chargeDistance));
                    if (distanceFromTargetY < chargeDistanceY && distanceFromTargetX >= chargeDistanceX)
                    {
                        // Set damage
                        NPC.damage = NPC.defDamage;

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

                        float targetX = player.Center.X - NPC.Center.X;
                        float targetY = player.Center.Y - NPC.Center.Y;
                        float targetDistance = (float)Math.Sqrt(targetX * targetX + targetY * targetY);

                        targetDistance = chargeSpeed / targetDistance;
                        NPC.velocity.X = targetX * targetDistance;
                        NPC.velocity.Y = targetY * targetDistance;
                        NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);

                        calamityGlobalNPC.newAI[1] = NPC.velocity.X;
                        calamityGlobalNPC.newAI[2] = NPC.velocity.Y;

                        NPC.direction = playerLocation < 0 ? 1 : -1;
                        NPC.spriteDirection = NPC.direction;
                        if (NPC.spriteDirection != 1)
                            NPC.rotation += (float)Math.PI;

                        NPC.ForceNetUpdate();
                        SoundEngine.PlaySound(DashSound, NPC.Center);

                        return;
                    }

                    NPC.rotation = NPC.velocity.X * 0.02f;
                    charging = false;

                    float maxLineUpSpeed = revenge ? 20f : 16f;
                    float lineUpAccel = revenge ? 0.4f : 0.3f;
                    if (phase2)
                    {
                        maxLineUpSpeed += 3f;
                        lineUpAccel += 0.15f;
                    }
                    if (phase4)
                    {
                        maxLineUpSpeed += 3f;
                        lineUpAccel += 0.15f;
                    }
                    maxLineUpSpeed += 7f * enrageScale;
                    lineUpAccel += 0.35f * enrageScale;

                    if (NPC.Center.Y < (player.Center.Y - chargeDistance - chargeDistanceY))
                        NPC.velocity.Y += lineUpAccel;
                    else if (NPC.Center.Y > (player.Center.Y - chargeDistance + chargeDistanceY))
                        NPC.velocity.Y -= lineUpAccel;
                    else
                        NPC.velocity.Y *= 0.7f;

                    if (NPC.velocity.Y < -maxLineUpSpeed)
                        NPC.velocity.Y = -maxLineUpSpeed;
                    if (NPC.velocity.Y > maxLineUpSpeed)
                        NPC.velocity.Y = maxLineUpSpeed;

                    float distanceXMax = 100f;
                    float distanceXMin = 20f;
                    if (distanceFromTargetX > chargeDistanceX + distanceXMax)
                        NPC.velocity.X += lineUpAccel * NPC.direction;
                    else if (distanceFromTargetX < chargeDistanceX + distanceXMin)
                        NPC.velocity.X -= lineUpAccel * NPC.direction;
                    else
                        NPC.velocity.X *= 0.7f;

                    if (NPC.velocity.X < -maxLineUpSpeed)
                        NPC.velocity.X = -maxLineUpSpeed;
                    if (NPC.velocity.X > maxLineUpSpeed)
                        NPC.velocity.X = maxLineUpSpeed;

                    NPC.direction = playerLocation < 0 ? 1 : -1;
                    NPC.spriteDirection = NPC.direction;
                    NPC.ForceNetUpdate();
                }

                // Slow down after charge
                else
                {
                    // Set damage
                    NPC.damage = NPC.defDamage;

                    if (NPC.velocity.X < 0f)
                        NPC.direction = -1;
                    else
                        NPC.direction = 1;

                    NPC.spriteDirection = NPC.direction;

                    int chargeDirectionXSign = 1;
                    if (NPC.Center.X < player.Center.X)
                        chargeDirectionXSign = -1;

                    if (NPC.direction == chargeDirectionXSign && Math.Abs(NPC.Center.X - player.Center.X) > chargeDistanceX)
                        NPC.ai[2] = 1f;
                    if (Math.Abs(NPC.Center.Y - player.Center.Y) > chargeDistanceX * 1.5f)
                        NPC.ai[2] = 1f;

                    if (enrageScale > 0 && NPC.ai[2] == 1f)
                        NPC.velocity *= 0.95f;

                    if (NPC.ai[2] != 1f)
                    {
                        charging = true;
                        NPC.frameCounter = 4;

                        // Velocity fix if PBG slowed
                        if (NPC.velocity.Length() < chargeSpeed)
                            NPC.velocity = new Vector2(calamityGlobalNPC.newAI[1], calamityGlobalNPC.newAI[2]);

                        calamityGlobalNPC.newAI[0] += 1f;
                        if (calamityGlobalNPC.newAI[0] > 90f)
                            NPC.velocity *= 1.01f;

                        // Spawn honey in the stupid seed
                        if (Main.zenithWorld && calamityGlobalNPC.newAI[0] % 6f == 0f)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                try
                                {
                                    int tilePositionX = (int)(NPC.Center.X / 16f);
                                    int tilePositionY = (int)(NPC.Center.Y / 16f);
                                    if (!WorldGen.SolidTile(tilePositionX, tilePositionY) && Main.tile[tilePositionX, tilePositionY].LiquidAmount == 0)
                                    {
                                        Main.tile[tilePositionX, tilePositionY].LiquidAmount = (byte)Main.rand.Next(50, 150);
                                        Main.tile[tilePositionX, tilePositionY].Get<LiquidData>().LiquidType = LiquidID.Honey;
                                        WorldGen.SquareTileFrame(tilePositionX, tilePositionY);
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

                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    float playerLocation = NPC.Center.X - player.Center.X;
                    NPC.direction = playerLocation < 0 ? 1 : -1;
                    NPC.spriteDirection = NPC.direction;

                    NPC.rotation = NPC.velocity.X * 0.02f;
                    charging = false;

                    NPC.velocity *= 0.9f;
                    float slowedVelocityThreshold = revenge ? 0.12f : 0.1f;
                    if (phase2)
                    {
                        NPC.velocity *= 0.98f;
                        slowedVelocityThreshold += 0.05f;
                    }
                    if (phase3)
                    {
                        NPC.velocity *= 0.98f;
                        slowedVelocityThreshold += 0.05f;
                    }
                    if (phase4)
                    {
                        NPC.velocity *= 0.98f;
                        slowedVelocityThreshold += 0.05f;
                    }
                    if (enrageScale > 0)
                        NPC.velocity *= 0.95f;

                    if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < slowedVelocityThreshold)
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
                // Avoid cheap bullshit
                NPC.damage = 0;

                float playerLocation = NPC.Center.X - player.Center.X;
                NPC.direction = playerLocation < 0 ? 1 : -1;
                NPC.spriteDirection = NPC.direction;

                // Move closer
                bool canHitTarget = Collision.CanHit(NPC.Center, 1, 1, player.position, player.width, player.height);
                float distanceAboveTarget = !canHitTarget ? 0f : 400f;
                float distanceAwayFromTargetX = !canHitTarget ? 80f : 240f;
                float distanceAwayFromTargetY = player.Center.Y - NPC.Center.Y;
                float distanceAwayFromTargetYLeeway = !canHitTarget ? 16f : 80f;
                bool tooFarX = Math.Abs(player.Center.X - NPC.Center.X) > distanceAwayFromTargetX;
                bool tooFarY = distanceAwayFromTargetY > distanceAboveTarget + distanceAwayFromTargetYLeeway || distanceAwayFromTargetY < distanceAboveTarget - distanceAwayFromTargetYLeeway;
                bool tooFar = tooFarX || tooFarY;

                calamityGlobalNPC.newAI[0] += 1f;
                if ((Vector2.Distance(NPC.Center, player.Center) < 640f && canHitTarget) || calamityGlobalNPC.newAI[0] >= 180f)
                {
                    NPC.ai[0] = phase3 ? 5f : 1f;
                    NPC.ai[1] = 0f;
                    calamityGlobalNPC.newAI[0] = 0f;

                    // Prevent netUpdate from being blocked by the spam counter.
                    // A phase switch sync is a critical operation that must be synced.
                    NPC.ForceNetUpdate(false);

                    SoundEngine.PlaySound(AttackSwitchSound, NPC.Center);

                    return;
                }

                if (tooFar)
                    Movement(distanceAboveTarget, player, enrageScale);
            }

            // Spawn less missiles
            else if (NPC.ai[0] == 1f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                charging = false;

                Vector2 missileSpawnPos = new Vector2(NPC.direction == 1 ? NPC.getRect().BottomLeft().X : NPC.getRect().BottomRight().X, NPC.getRect().Bottom().Y + 20f);
                missileSpawnPos.X += NPC.direction * 120;
                Vector2 missileSpawnCollisionLocation = new Vector2(missileSpawnPos.X, missileSpawnPos.Y - 30f);
                bool canHitTarget = Collision.CanHit(missileSpawnCollisionLocation, 1, 1, player.position, player.width, player.height);

                NPC.ai[1] += 1f;
                NPC.ai[1] += activePlayers / 2;
                if (phase2)
                    NPC.ai[1] += 0.5f;

                bool shouldSpawnMissiles = false;
                if (NPC.ai[1] > 40f - 12f * enrageScale)
                {
                    NPC.ai[1] = 0f;
                    NPC.ai[2] += 1f;
                    shouldSpawnMissiles = true;
                }

                if (shouldSpawnMissiles)
                {
                    SoundEngine.PlaySound(SoundID.NPCHit8, NPC.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (expertMode && NPC.CountNPCS(ModContent.NPCType<PlagueMine>()) < 2)
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)missileSpawnPos.X, (int)missileSpawnPos.Y, ModContent.NPCType<PlagueMine>(), 0, 0f, 0f, 0f, challengeAmt);

                        float npcSpeed = (revenge ? 9f : 7f) + enrageScale * 2f;

                        float projXDist = player.Center.X - missileSpawnPos.X;
                        float projYDist = player.Center.Y - missileSpawnPos.Y;
                        float projDistance = (float)Math.Sqrt(projXDist * projXDist + projYDist * projYDist);

                        projDistance = npcSpeed / projDistance;
                        projXDist *= projDistance;
                        projYDist *= projDistance;

                        int plagueMissile = NPC.NewNPC(NPC.GetSource_FromAI(), (int)missileSpawnPos.X, (int)missileSpawnPos.Y, ModContent.NPCType<PlagueHomingMissile>(), 0, 0f, 0f, 0f, challengeAmt);
                        Main.npc[plagueMissile].velocity.X = projXDist;
                        Main.npc[plagueMissile].velocity.Y = projYDist;
                        Main.npc[plagueMissile].netUpdate = true;
                    }
                }

                // Move closer
                float distanceAboveTarget = !canHitTarget ? 0f : 400f;
                float distanceAwayFromTargetX = !canHitTarget ? 80f : 240f;
                float distanceAwayFromTargetY = player.Center.Y - NPC.Center.Y;
                float distanceAwayFromTargetYLeeway = !canHitTarget ? 16f : 80f;
                bool tooFarX = Math.Abs(player.Center.X - NPC.Center.X) > distanceAwayFromTargetX;
                bool tooFarY = distanceAwayFromTargetY > distanceAboveTarget + distanceAwayFromTargetYLeeway || distanceAwayFromTargetY < distanceAboveTarget - distanceAwayFromTargetYLeeway;
                bool tooFar = tooFarX || tooFarY;
                if (tooFar)
                    Movement(distanceAboveTarget, player, enrageScale);

                float playerLocation = NPC.Center.X - player.Center.X;
                NPC.direction = playerLocation < 0 ? 1 : -1;
                NPC.spriteDirection = NPC.direction;

                if (NPC.ai[2] > 3f)
                {
                    NPC.ai[0] = -1f;
                    NPC.ai[1] = 2f;
                    NPC.ai[2] = 0f;

                    // Prevent netUpdate from being blocked by the spam counter.
                    // A phase switch sync is a critical operation that must be synced.
                    NPC.ForceNetUpdate(false);

                    SoundEngine.PlaySound(AttackSwitchSound, NPC.Center);
                }
            }

            // Missile spawn
            else if (NPC.ai[0] == 5f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                charging = false;
                Vector2 missileSpawnPos = new Vector2(NPC.direction == 1 ? NPC.getRect().BottomLeft().X : NPC.getRect().BottomRight().X, NPC.getRect().Bottom().Y + 20f);
                missileSpawnPos.X += NPC.direction * 120;
                Vector2 missileSpawnCollisionLocation = new Vector2(missileSpawnPos.X, missileSpawnPos.Y - 30f);
                bool canHitTarget = Collision.CanHit(missileSpawnCollisionLocation, 1, 1, player.position, player.width, player.height);

                NPC.ai[1] += 1f;
                NPC.ai[1] += activePlayers / 2;
                bool shouldSpawnMissiles = false;
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
                    shouldSpawnMissiles = true;
                }

                if (shouldSpawnMissiles)
                {
                    SoundEngine.PlaySound(SoundID.Item88, NPC.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (expertMode && NPC.CountNPCS(ModContent.NPCType<PlagueMine>()) < 3)
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)missileSpawnPos.X, (int)missileSpawnPos.Y, ModContent.NPCType<PlagueMine>(), 0, 0f, 0f, 0f, challengeAmt);

                        float npcSpeed = (revenge ? 10f : 8f) + enrageScale * 2f;

                        float projXDist = player.Center.X - missileSpawnPos.X;
                        float projYDist = player.Center.Y - missileSpawnPos.Y;
                        float projDistance = (float)Math.Sqrt(projXDist * projXDist + projYDist * projYDist);

                        projDistance = npcSpeed / projDistance;
                        projXDist *= projDistance;
                        projYDist *= projDistance;
                        projXDist += Main.rand.Next(-20, 21) * 0.05f;
                        projYDist += Main.rand.Next(-20, 21) * 0.05f;

                        int plagueMissile = NPC.NewNPC(NPC.GetSource_FromAI(), (int)missileSpawnPos.X, (int)missileSpawnPos.Y, ModContent.NPCType<PlagueHomingMissile>(), 0, 0f, 0f, 0f, challengeAmt);
                        Main.npc[plagueMissile].velocity.X = projXDist;
                        Main.npc[plagueMissile].velocity.Y = projYDist;
                        Main.npc[plagueMissile].netUpdate = true;
                    }
                }

                // Move closer
                float distanceAboveTarget = !canHitTarget ? 0f : 400f;
                float distanceAwayFromTargetX = !canHitTarget ? 80f : 240f;
                float distanceAwayFromTargetY = player.Center.Y - NPC.Center.Y;
                float distanceAwayFromTargetYLeeway = !canHitTarget ? 16f : 80f;
                bool tooFarX = Math.Abs(player.Center.X - NPC.Center.X) > distanceAwayFromTargetX;
                bool tooFarY = distanceAwayFromTargetY > distanceAboveTarget + distanceAwayFromTargetYLeeway || distanceAwayFromTargetY < distanceAboveTarget - distanceAwayFromTargetYLeeway;
                bool tooFar = tooFarX || tooFarY;
                if (tooFar)
                    Movement(distanceAboveTarget, player, enrageScale);

                float playerLocation = NPC.Center.X - player.Center.X;
                NPC.direction = playerLocation < 0 ? 1 : -1;
                NPC.spriteDirection = NPC.direction;

                if (NPC.ai[2] > 5f)
                {
                    NPC.ai[0] = -1f;
                    NPC.ai[1] = 2f;
                    NPC.ai[2] = 0f;

                    // Prevent netUpdate from being blocked by the spam counter.
                    // A phase switch sync is a critical operation that must be synced.
                    NPC.ForceNetUpdate(false);

                    SoundEngine.PlaySound(AttackSwitchSound, NPC.Center);
                }
            }

            // Stinger phase
            else if (NPC.ai[0] == 3f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                Vector2 stingerSpawnPos = new Vector2(NPC.direction == 1 ? NPC.getRect().BottomLeft().X : NPC.getRect().BottomRight().X, NPC.getRect().Bottom().Y + 20f);
                stingerSpawnPos.X += NPC.direction * 120;
                bool canHitTarget = Collision.CanHit(new Vector2(stingerSpawnPos.X, stingerSpawnPos.Y - 30f), 1, 1, player.position, player.width, player.height);

                NPC.ai[1] += 1f;
                int stingerFireDelay = phase5 ? 20 : (phase3 ? 25 : 30);
                stingerFireDelay -= (int)Math.Ceiling(5f * enrageScale);

                if (NPC.ai[1] % stingerFireDelay == (stingerFireDelay - 1) && NPC.Center.Y < player.position.Y)
                {
                    SoundEngine.PlaySound(SoundID.Item42, NPC.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float projectileSpeed = revenge ? 6f : 5f;
                        projectileSpeed += 2f * enrageScale;

                        float projXDist = player.Center.X - stingerSpawnPos.X;
                        float projYDist = player.Center.Y - stingerSpawnPos.Y;
                        float projDistance = (float)Math.Sqrt(projXDist * projXDist + projYDist * projYDist);
                        projDistance = projectileSpeed / projDistance;
                        projXDist *= projDistance;
                        projYDist *= projDistance;

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
                            type = ModContent.ProjectileType<HiveBombGoliath>();

                        int damage = type == ModContent.ProjectileType<HiveBombGoliath>() ? NukeDamage : StingerDamage;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), stingerSpawnPos.X, stingerSpawnPos.Y, projXDist, projYDist, type, damage, 0f, Main.myPlayer, challengeAmt, player.position.Y);
                        NPC.netUpdate = true;
                    }

                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] > 4f)
                        NPC.ai[2] = 0f;
                }

                // Move closer
                float distanceAboveTarget = !canHitTarget ? 0f : 400f;
                float distanceAwayFromTargetX = !canHitTarget ? 80f : 240f;
                float distanceAwayFromTargetY = player.Center.Y - NPC.Center.Y;
                float distanceAwayFromTargetYLeeway = !canHitTarget ? 16f : 80f;
                bool tooFarX = Math.Abs(player.Center.X - NPC.Center.X) > distanceAwayFromTargetX;
                bool tooFarY = distanceAwayFromTargetY > distanceAboveTarget + distanceAwayFromTargetYLeeway || distanceAwayFromTargetY < distanceAboveTarget - distanceAwayFromTargetYLeeway;
                bool tooFar = tooFarX || tooFarY;
                if (tooFar)
                    Movement(distanceAboveTarget, player, enrageScale);

                float playerLocation = NPC.Center.X - player.Center.X;
                NPC.direction = playerLocation < 0 ? 1 : -1;
                NPC.spriteDirection = NPC.direction;

                if (NPC.ai[1] > stingerFireDelay * 10f)
                {
                    NPC.ai[0] = -1f;
                    NPC.ai[1] = 3f;
                    NPC.ai[2] = 0f;

                    // Prevent netUpdate from being blocked by the spam counter.
                    // A phase switch sync is a critical operation that must be synced.
                    NPC.ForceNetUpdate(false);

                    SoundEngine.PlaySound(AttackSwitchSound, NPC.Center);
                }
            }

            // Missile charge
            else if (NPC.ai[0] == 4f)
            {
                float missileVelocity = revenge ? 6f : 5f;
                missileVelocity += 2f * enrageScale;

                int type = ModContent.ProjectileType<HiveBombGoliath>();
                int damage = NukeDamage;

                int chargeDistanceX = 600;
                float chargeSpeed = revenge ? 28f : 26f;
                chargeSpeed += 3f * enrageScale;

                int phaseSwitchTimer = (int)Math.Ceiling(2f + enrageScale);
                if (NPC.ai[1] > (2 * phaseSwitchTimer) && NPC.ai[1] % 2f == 0f)
                {
                    MissileCountdown = 0;
                    NPC.ai[0] = -1f;
                    NPC.ai[1] = -1f;
                    NPC.ai[2] = 0f;

                    // Prevent netUpdate from being blocked by the spam counter.
                    // A phase switch sync is a critical operation that must be synced.
                    NPC.ForceNetUpdate(false);

                    SoundEngine.PlaySound(AttackSwitchSound, NPC.Center);

                    return;
                }

                // Charge
                if (NPC.ai[1] % 2f == 0f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    float playerLocation = NPC.Center.X - player.Center.X;

                    float chargeDistanceY = 20f;
                    chargeDistanceY += 20f * enrageScale;

                    float distanceFromTargetX = Math.Abs(NPC.Center.X - player.Center.X);
                    float distanceFromTargetY = Math.Abs(NPC.Center.Y - (player.Center.Y - 500f));
                    if (distanceFromTargetY < chargeDistanceY && distanceFromTargetX >= chargeDistanceX)
                    {
                        // Set damage
                        NPC.damage = NPC.defDamage;

                        if (MissileCountdown == 1)
                        {
                            SoundEngine.PlaySound(BarrageLaunchSound, NPC.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                bool gaussMode = false;

                                Vector2 baseVelocity = player.Center - NPC.Center;
                                baseVelocity.Normalize();
                                baseVelocity *= missileVelocity;

                                if (Main.rand.NextBool(10) && Main.zenithWorld)
                                {
                                    type = ModContent.ProjectileType<AresGaussNukeProjectile>();
                                    baseVelocity *= 0.75f;
                                    gaussMode = true;
                                }
                                else if (Main.rand.NextBool() && Main.zenithWorld)
                                {
                                    type = ModContent.ProjectileType<PeanutRocket>();
                                    baseVelocity *= 0.4f;
                                }

                                int spread = 24;
                                if (!gaussMode)
                                {
                                    for (int i = 0; i < MissileProjectiles; i++)
                                    {
                                        Vector2 spawn = NPC.Center;
                                        spawn.X += i * (int)(spread * 1.125) - (MissileProjectiles * (spread / 2));
                                        Vector2 velocity = baseVelocity.RotatedBy(MathHelper.ToRadians(-MissileAngleSpread / 2 + (MissileAngleSpread * i / MissileProjectiles)));
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), spawn, velocity, type, damage, 0f, Main.myPlayer, nukeBarrageChallengeAmt, player.position.Y);
                                    }
                                }
                                else
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, baseVelocity, type, damage, 0f, Main.myPlayer);
                            }
                        }

                        charging = true;

                        NPC.ai[1] += 1f;
                        NPC.ai[2] = 0f;

                        float targetX = player.Center.X - NPC.Center.X;
                        float targetY = player.Center.Y - 500f - NPC.Center.Y;
                        float targetDistance = (float)Math.Sqrt(targetX * targetX + targetY * targetY);

                        targetDistance = chargeSpeed / targetDistance;
                        NPC.velocity.X = targetX * targetDistance;
                        NPC.velocity.Y = targetY * targetDistance;
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

                    float maxLineUpSpeed = revenge ? 26f : 22f;
                    float lineUpAccel = revenge ? 0.7f : 0.6f;
                    maxLineUpSpeed += 7f * enrageScale;
                    lineUpAccel += 0.35f * enrageScale;

                    if (NPC.Center.Y < (player.Center.Y - 500f - chargeDistanceY))
                        NPC.velocity.Y += lineUpAccel;
                    else if (NPC.Center.Y > (player.Center.Y - 500f + chargeDistanceY))
                        NPC.velocity.Y -= lineUpAccel;
                    else
                        NPC.velocity.Y *= 0.7f;

                    if (NPC.velocity.Y < -maxLineUpSpeed)
                        NPC.velocity.Y = -maxLineUpSpeed;
                    if (NPC.velocity.Y > maxLineUpSpeed)
                        NPC.velocity.Y = maxLineUpSpeed;

                    float distanceXMax = 100f;
                    float distanceXMin = 20f;
                    if (distanceFromTargetX > chargeDistanceX + distanceXMax)
                        NPC.velocity.X += lineUpAccel * NPC.direction;
                    else if (distanceFromTargetX < chargeDistanceX + distanceXMin)
                        NPC.velocity.X -= lineUpAccel * NPC.direction;
                    else
                        NPC.velocity.X *= 0.7f;

                    if (NPC.velocity.X < -maxLineUpSpeed)
                        NPC.velocity.X = -maxLineUpSpeed;
                    if (NPC.velocity.X > maxLineUpSpeed)
                        NPC.velocity.X = maxLineUpSpeed;

                    NPC.direction = playerLocation < 0 ? 1 : -1;
                    NPC.spriteDirection = NPC.direction;
                }

                // Slow down after charge
                else
                {
                    // Set damage
                    NPC.damage = NPC.defDamage;

                    if (NPC.velocity.X < 0f)
                        NPC.direction = -1;
                    else
                        NPC.direction = 1;

                    NPC.spriteDirection = NPC.direction;

                    int chargeDirectionXSign = 1;

                    if (NPC.Center.X < player.Center.X)
                        chargeDirectionXSign = -1;
                    if (NPC.direction == chargeDirectionXSign && Math.Abs(NPC.Center.X - player.Center.X) > chargeDistanceX)
                        NPC.ai[2] = 1f;
                    if (Math.Abs(NPC.Center.Y - player.Center.Y) > chargeDistanceX * 3f)
                        NPC.ai[2] = 1f;
                    if (enrageScale > 0 && NPC.ai[2] == 1f)
                        NPC.velocity *= 0.95f;

                    if (NPC.ai[2] != 1f)
                    {
                        charging = true;

                        // Velocity fix if PBG slowed
                        if (NPC.velocity.Length() < chargeSpeed)
                            NPC.velocity.X = chargeSpeed * NPC.direction;

                        calamityGlobalNPC.newAI[0] += 1f;
                        if (calamityGlobalNPC.newAI[0] > 90f)
                            NPC.velocity.X *= 1.01f;

                        // Fire missiles in death mode during charge
                        if (death)
                        {
                            float missileGateValue = 20f;
                            bool fireMissile = calamityGlobalNPC.newAI[0] % missileGateValue == 0f && Collision.CanHit(NPC.Center, 1, 1, player.position, player.width, player.height);
                            if (fireMissile)
                            {
                                SoundEngine.PlaySound(SoundID.Item42, NPC.Center);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * missileVelocity, type, damage, 0f, Main.myPlayer, nukeBarrageChallengeAmt, player.position.Y);
                            }
                        }

                        return;
                    }

                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    NPC.rotation = NPC.velocity.X * 0.02f;
                    charging = false;

                    NPC.velocity *= 0.9f;
                    float slowedVelocityThreshold = revenge ? 0.12f : 0.1f;
                    if (phase3)
                    {
                        NPC.velocity *= 0.9f;
                        slowedVelocityThreshold += 0.05f;
                    }
                    if (phase4)
                    {
                        NPC.velocity *= 0.9f;
                        slowedVelocityThreshold += 0.05f;
                    }
                    if (phase5)
                    {
                        NPC.velocity *= 0.9f;
                        slowedVelocityThreshold += 0.05f;
                    }
                    if (enrageScale > 0)
                        NPC.velocity *= 0.95f;

                    if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < slowedVelocityThreshold)
                    {
                        NPC.ai[2] = 0f;
                        NPC.ai[1] += 1f;
                        calamityGlobalNPC.newAI[0] = 0f;
                    }
                }
            }
        }

        private void Movement(float distanceAboveTarget, Player player, float enrageScale)
        {
            float acceleration = (NPC.ai[0] == 1f || NPC.ai[0] == 5f) ? 0.12f : (NPC.ai[0] == 2f ? 0.15f : 0.18f); // Reduce acceleration while spawning minions
            float velocity = (NPC.ai[0] == 1f || NPC.ai[0] == 5f) ? 12f : (NPC.ai[0] == 2f ? 15f : 18f); // Reduce velocity while preparing to spawn minions and spawning minions
            acceleration *= 0.5f * enrageScale + 1f;
            velocity *= 1f + enrageScale * 0.5f;
            Vector2 hoverDestination = player.Center - Vector2.UnitY * distanceAboveTarget;
            Vector2 idealVelocity = NPC.SafeDirectionTo(hoverDestination) * velocity;
            NPC.SimpleFlyMovement(idealVelocity, acceleration);
        }

        public override bool CheckActive() => canDespawn;

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

            return minDist <= 100f * NPC.scale;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 2; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Plague, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                if (!Main.dedServ)
                {
                    for (int i = 1; i < 7; i++)
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("PlaguebringerGoliathGore" + i).Type, NPC.scale);
                }

                NPC.position = NPC.Center;
                NPC.width = NPC.height = 200;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);

                for (int i = 0; i < 40; i++)
                {
                    int plagueDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Plague, 0f, 0f, 100, default, 2f);
                    Main.dust[plagueDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[plagueDust].scale = 0.5f;
                        Main.dust[plagueDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }

                for (int j = 0; j < 70; j++)
                {
                    int plagueDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Plague, 0f, 0f, 100, default, 3f);
                    Main.dust[plagueDust2].noGravity = true;
                    Main.dust[plagueDust2].velocity *= 5f;
                    plagueDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Plague, 0f, 0f, 100, default, 2f);
                    Main.dust[plagueDust2].velocity *= 2f;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Texture2D glowTexture = Texture_Glow.Value;
            if (curTex != (charging ? 2 : 1))
            {
                NPC.frame.X = 0;
                NPC.frame.Y = 0;
            }
            if (charging)
            {
                curTex = 2;
                texture = ChargeTexture.Value;
                glowTexture = ChargeTexture_Glow.Value;
            }
            else
                curTex = 1;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            int frameCount = 3;
            Rectangle rectangle = new Rectangle(NPC.frame.X, NPC.frame.Y, texture.Width / 2, texture.Height / frameCount);
            Vector2 halfSizeTexture = rectangle.Size() / 2f;
            Vector2 posOffset = new Vector2(charging ? 175 : 125, 0);
            int chargeAfterimageAmount = 10;

            if (CalamityClientConfig.Instance.Afterimages)
            {
                if ((NPC.ai[0] == 0f || NPC.ai[0] == 4f) && charging)
                {
                    for (int j = 1; j < chargeAfterimageAmount; j += 2)
                    {
                        Color afterimageColor = drawColor;
                        afterimageColor = Color.Lerp(afterimageColor, Color.White, 0.5f);
                        afterimageColor = NPC.GetAlpha(afterimageColor);
                        afterimageColor *= (chargeAfterimageAmount - j) / 15f;
                        Vector2 afterimagePos = NPC.oldPos[j] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                        afterimagePos -= new Vector2(texture.Width, texture.Height / frameCount) * NPC.scale / 2f;
                        afterimagePos += halfSizeTexture * NPC.scale + posOffset;
                        spriteBatch.Draw(texture, afterimagePos, new Rectangle?(rectangle), afterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                    }
                }
            }

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(texture.Width, texture.Height / frameCount) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + posOffset;
            spriteBatch.Draw(texture, drawLocation, new Rectangle?(rectangle), NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            Color redLerpColor = Color.Lerp(Color.White, Color.Red, 0.5f);

            if (CalamityClientConfig.Instance.Afterimages)
            {
                if ((NPC.ai[0] == 0f || NPC.ai[0] == 4f) && charging)
                {
                    for (int k = 1; k < chargeAfterimageAmount; k++)
                    {
                        Color otherAfterimageColor = redLerpColor;
                        otherAfterimageColor = Color.Lerp(otherAfterimageColor, Color.White, 0.5f);
                        otherAfterimageColor *= (chargeAfterimageAmount - k) / 15f;
                        Vector2 otherAfterimagePos = NPC.oldPos[k] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                        otherAfterimagePos -= new Vector2(glowTexture.Width, glowTexture.Height / frameCount) * NPC.scale / 2f;
                        otherAfterimagePos += halfSizeTexture * NPC.scale + posOffset;
                        spriteBatch.Draw(glowTexture, otherAfterimagePos, new Rectangle?(rectangle), otherAfterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                    }
                }
            }

            spriteBatch.Draw(glowTexture, drawLocation, new Rectangle?(rectangle), redLerpColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

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
                    flyingFrame2 = !flyingFrame2;
            }
        }

        public override void BossLoot(ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void OnKill()
        {
            // Don't bother running any of this in Boss Rush.
            if (BossRushEvent.BossRushActive)
                return;

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
                    ModContent.ItemType<Malevolence>(),
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
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<InfectedArmorPlating>(), 1, 30, 40));

                // Equipment
                // 16NOV2025: Ozzatron: item has been chosen as the "Expert gatekept" item for this Calamity boss
                // normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<ToxicHeart>()));

                // Vanity
                normalOnly.Add(ModContent.ItemType<PlaguebringerGoliathMask>(), 7);
                normalOnly.Add(ModContent.ItemType<PlagueCaller>(), 10);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
            }

            npcLoot.Add(ModContent.ItemType<PlaguebringerGoliathTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<PlaguebringerGoliathRelic>());

            // GFB Honey Bucket drop
            npcLoot.DefineConditionalDropSet(DropHelper.GFB).Add(DropHelper.PerPlayer(ItemID.BottomlessHoneyBucket), hideLootReport: true);

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedPlaguebringer, ModContent.ItemType<LorePlaguebringerGoliath>(), desc: DropHelper.FirstKillText);
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
            {
                if (Main.zenithWorld) // it is the plague, you get very sick.
                {
                    target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 480);
                    target.AddBuff(BuffID.Poisoned, 480);
                    target.AddBuff(BuffID.Venom, 480);
                }
                target.AddBuff(ModContent.BuffType<Plague>(), 240);
            }
        }
    }
}

using System;
using System.IO;
using CalamityMod.Events;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Sounds;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.ExoMechs.Thanatos
{
    [LongDistanceNetSync(SyncWith = typeof(ThanatosHead))]
    public class ThanatosTail : ModNPC
    {
        public static int normalIconIndex;
        public static int vulnerableIconIndex;

        internal static void LoadHeadIcons()
        {
            string normalIconPath = "CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosNormalTail";
            string vulnerableIconPath = "CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosVulnerableTail";

            CalamityMod.Instance.AddBossHeadTexture(normalIconPath, -1);
            normalIconIndex = ModContent.GetModBossHeadSlot(normalIconPath);

            CalamityMod.Instance.AddBossHeadTexture(vulnerableIconPath, -1);
            vulnerableIconIndex = ModContent.GetModBossHeadSlot(vulnerableIconPath);
        }

        // Whether the tail is venting heat or not, it is vulnerable to damage during venting
        private bool vulnerable = false;

        public ThanatosSmokeParticleSet SmokeDrawer = new ThanatosSmokeParticleSet(-1, 3, 0f, 16f, 1.5f);

        // Default life ratio for the other mechs
        private const float defaultLifeRatio = 5f;

        // Timer to prevent Thanatos from dealing contact damage for a bit
        private int noContactDamageTimer = 0;

        private const float timeToOpenAndFireLasers = 36f;

        private const float segmentCloseTimerDecrement = 0.2f;

        public static Asset<Texture2D> GlowTexture;

        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.ThanatosHead.DisplayName");
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            Main.npcFrameCount[NPC.type] = 5;
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 5f;
            NPC.GetNPCDamage();
            NPC.width = 76;
            NPC.height = 110;
            NPC.defense = 100;
            NPC.DR_NERD(0.9999f);
            NPC.Calamity().unbreakableDR = true;
            NPC.LifeMaxNERB(960000, 1150000, 600000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.Opacity = 0f;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.DeathSound = CommonCalamitySounds.ExoDeathSound;
            NPC.netAlways = true;
            NPC.dontCountMe = true;
            NPC.chaseable = false;
            NPC.boss = true;
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
        }

        public override void BossHeadSlot(ref int index)
        {
            if (Main.npc[(int)NPC.ai[2]].Calamity().newAI[1] == (float)ThanatosHead.SecondaryPhase.PassiveAndImmune)
                index = -1;
            else if (vulnerable)
                index = vulnerableIconIndex;
            else
                index = normalIconIndex;
        }

        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.chaseable);
            writer.Write(NPC.dontTakeDamage);
            writer.Write(noContactDamageTimer);
            writer.Write(vulnerable);
            writer.Write(NPC.localAI[0]);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.chaseable = reader.ReadBoolean();
            NPC.dontTakeDamage = reader.ReadBoolean();
            noContactDamageTimer = reader.ReadInt32();
            vulnerable = reader.ReadBoolean();
            NPC.localAI[0] = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

        public override void AI()
        {
            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            if (NPC.life > Main.npc[(int)NPC.ai[1]].life)
                NPC.life = Main.npc[(int)NPC.ai[1]].life;

            // Difficulty modes
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool expertMode = Main.expertMode || bossRush;

            // Check if other segments are still alive, if not, die
            bool shouldDespawn = !NPC.AnyNPCs(ModContent.NPCType<ThanatosHead>());
            if (!shouldDespawn)
            {
                if (NPC.ai[1] <= 0f)
                    shouldDespawn = true;
                else if (Main.npc[(int)NPC.ai[1]].life <= 0)
                    shouldDespawn = true;
            }
            if (shouldDespawn)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.checkDead();
                NPC.active = false;
                return;
            }

            // Set vulnerable to false by default
            vulnerable = false;

            NPC head = Main.npc[(int)NPC.ai[2]];
            CalamityGlobalNPC calamityGlobalNPC_Head = head.Calamity();

            bool invisiblePhase = calamityGlobalNPC_Head.newAI[1] == (float)ThanatosHead.SecondaryPhase.PassiveAndImmune;
            NPC.dontTakeDamage = Main.npc[(int)NPC.ai[2]].dontTakeDamage;
            if (!invisiblePhase)
            {
                if (Main.npc[(int)NPC.ai[1]].Opacity > 0.5f)
                {
                    if (noContactDamageTimer > 0)
                        noContactDamageTimer--;

                    NPC.Opacity += 0.2f;
                    if (NPC.Opacity > 1f)
                        NPC.Opacity = 1f;
                }
                else
                {
                    // Deal no contact damage for 3 seconds after becoming visible
                    noContactDamageTimer = 185;
                }
            }
            else
            {
                // Deal no contact damage for 3 seconds after becoming visible
                noContactDamageTimer = 185;

                NPC.Opacity -= 0.05f;
                if (NPC.Opacity < 0f)
                    NPC.Opacity = 0f;
            }

            // Number of body segments
            int numSegments = ThanatosHead.minLength;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Check if the other exo mechs are alive
            int otherExoMechsAlive = 0;
            bool exoPrimeAlive = false;
            bool exoTwinsAlive = false;
            if (CalamityGlobalNPC.draedonExoMechPrime != -1)
            {
                if (Main.npc[CalamityGlobalNPC.draedonExoMechPrime].active)
                {
                    otherExoMechsAlive++;
                    exoPrimeAlive = true;
                }
            }
            if (CalamityGlobalNPC.draedonExoMechTwinGreen != -1)
            {
                if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].active)
                {
                    otherExoMechsAlive++;
                    exoTwinsAlive = true;
                }
            }

            // Set the AI to become more aggressive if head is berserk
            bool berserk = lifeRatio < 0.4f || (otherExoMechsAlive == 0 && lifeRatio < 0.7f);
            bool lastMechAlive = berserk && otherExoMechsAlive == 0;

            // These are 5 by default to avoid triggering passive phases after the other mechs are dead
            float exoPrimeLifeRatio = defaultLifeRatio;
            float exoTwinsLifeRatio = defaultLifeRatio;
            if (exoPrimeAlive)
                exoPrimeLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechPrime].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechPrime].lifeMax;
            if (exoTwinsAlive)
                exoTwinsLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].lifeMax;

            // If Thanatos doesn't go berserk
            bool otherMechIsBerserk = exoPrimeLifeRatio < 0.4f || exoTwinsLifeRatio < 0.4f;

            // Whether Thanatos should be buffed while in berserk phase
            bool shouldGetBuffedByBerserkPhase = berserk && !otherMechIsBerserk;

            // Use to close the segments quicker in later phases
            float fasterSegmentClosingVar = lastMechAlive ? 0.2f : shouldGetBuffedByBerserkPhase ? 0.1f : 0f;

            bool shootLasers = (calamityGlobalNPC_Head.newAI[0] == (float)ThanatosHead.Phase.Charge || calamityGlobalNPC_Head.newAI[0] == (float)ThanatosHead.Phase.UndergroundLaserBarrage) && calamityGlobalNPC_Head.newAI[2] > 0f;
            if (shootLasers && !invisiblePhase)
            {
                // Only charge up lasers if not venting or firing lasers
                if (NPC.Calamity().newAI[0] == 0f)
                    NPC.ai[3] += 1f;

                double numSegmentsAbleToFire = bossRush ? 42D : death ? 36D : revenge ? 34D : expertMode ? 30D : 24D;
                if (shouldGetBuffedByBerserkPhase)
                    numSegmentsAbleToFire *= 1.25;

                float segmentDivisor = (float)Math.Round(numSegments / numSegmentsAbleToFire);

                if (calamityGlobalNPC_Head.newAI[0] == (float)ThanatosHead.Phase.Charge)
                {
                    float divisor = lastMechAlive ? 45f : shouldGetBuffedByBerserkPhase ? 60f : 75f;
                    if ((NPC.ai[3] % divisor == 0f && NPC.ai[0] % segmentDivisor == 0f) || NPC.Calamity().newAI[0] > 0f)
                    {
                        // Body is vulnerable while firing lasers
                        vulnerable = true;

                        if (NPC.Calamity().newAI[1] == 0f)
                        {
                            NPC.Calamity().newAI[0] += 1f;
                            if (NPC.Calamity().newAI[0] >= timeToOpenAndFireLasers)
                            {
                                NPC.ai[3] = 0f;
                                NPC.Calamity().newAI[1] = 1f;

                                int maxTargets = 3;
                                int[] whoAmIArray = new int[maxTargets];
                                Vector2[] targetCenterArray = new Vector2[maxTargets];
                                int numProjectiles = 0;
                                float maxDistance = 2400f;

                                foreach (Player plr in Main.ActivePlayers)
                                {
                                    if (plr.dead)
                                        continue;

                                    Vector2 playerCenter = plr.Center;
                                    float distance = Vector2.Distance(playerCenter, NPC.Center);
                                    if (distance < maxDistance)
                                    {
                                        whoAmIArray[numProjectiles] = plr.whoAmI;
                                        targetCenterArray[numProjectiles] = playerCenter;
                                        int projectileLimit = numProjectiles + 1;
                                        numProjectiles = projectileLimit;
                                        if (projectileLimit >= targetCenterArray.Length)
                                            break;
                                    }
                                }

                                SoundEngine.PlaySound(CommonCalamitySounds.ExoLaserShootSound with { Volume = 0.1f * CommonCalamitySounds.ExoLaserShootSound.Volume }, NPC.Center);

                                for (int i = 0; i < numProjectiles; i++)
                                {
                                    // Normal laser
                                    int type = ModContent.ProjectileType<ThanatosLaser>();
                                    int damage = NPC.GetProjectileDamage(type);

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, targetCenterArray[i], type, damage, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                                }
                            }
                        }
                        else
                        {
                            NPC.Calamity().newAI[0] -= segmentCloseTimerDecrement + fasterSegmentClosingVar;
                            if (NPC.Calamity().newAI[0] <= 0f)
                            {
                                NPC.Calamity().newAI[0] = 0f;
                                NPC.Calamity().newAI[1] = 0f;
                            }
                        }
                    }
                }
                else
                {
                    float divisor = NPC.ai[0] * (lastMechAlive ? 1f : shouldGetBuffedByBerserkPhase ? 2f : 3f); // Ranges from 3 to 300
                    if ((NPC.ai[3] == divisor && NPC.ai[0] % segmentDivisor == 0f) || NPC.Calamity().newAI[0] > 0f)
                    {
                        // Body is vulnerable while firing lasers
                        vulnerable = true;

                        if (NPC.Calamity().newAI[1] == 0f)
                        {
                            NPC.Calamity().newAI[0] += 1f;
                            if (NPC.Calamity().newAI[0] >= timeToOpenAndFireLasers)
                            {
                                NPC.ai[3] = 0f;
                                NPC.Calamity().newAI[1] = 1f;

                                int maxTargets = 3;
                                int[] whoAmIArray = new int[maxTargets];
                                Vector2[] targetCenterArray = new Vector2[maxTargets];
                                int numProjectiles = 0;
                                float maxDistance = 2400f;

                                foreach (Player plr in Main.ActivePlayers)
                                {
                                    if (plr.dead)
                                        continue;

                                    Vector2 playerCenter = plr.Center;
                                    float distance = Vector2.Distance(playerCenter, NPC.Center);
                                    if (distance < maxDistance)
                                    {
                                        whoAmIArray[numProjectiles] = plr.whoAmI;
                                        targetCenterArray[numProjectiles] = playerCenter;
                                        int projectileLimit = numProjectiles + 1;
                                        numProjectiles = projectileLimit;
                                        if (projectileLimit >= targetCenterArray.Length)
                                            break;
                                    }
                                }

                                float predictionAmt = bossRush ? 24f : death ? 20f : revenge ? 18f : expertMode ? 16f : 12f;
                                if (NPC.ai[0] % 3f == 0f)
                                    predictionAmt *= 0.5f;

                                int type = ModContent.ProjectileType<ThanatosLaser>();
                                int damage = NPC.GetProjectileDamage(type);
                                SoundEngine.PlaySound(CommonCalamitySounds.ExoLaserShootSound, NPC.Center);
                                for (int i = 0; i < numProjectiles; i++)
                                {
                                    // Fire normal lasers if head is in passive state
                                    if (calamityGlobalNPC_Head.newAI[1] == (float)ThanatosHead.SecondaryPhase.Passive)
                                    {
                                        // Normal laser
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, targetCenterArray[i], type, damage, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                                    }
                                    else
                                    {
                                        // Normal laser
                                        if (shouldGetBuffedByBerserkPhase && NPC.ai[0] % 3f == 0f)
                                        {
                                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, targetCenterArray[i], type, damage, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                                        }

                                        // Predictive laser
                                        Vector2 projectileDestination = targetCenterArray[i] + Main.player[whoAmIArray[i]].velocity * predictionAmt;
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projectileDestination, type, damage, 0f, Main.myPlayer, 0f, NPC.whoAmI);

                                        // Opposite laser
                                        projectileDestination = targetCenterArray[i] - Main.player[whoAmIArray[i]].velocity * predictionAmt;
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projectileDestination, type, damage, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                                    }
                                }
                            }
                        }
                        else
                        {
                            NPC.Calamity().newAI[0] -= segmentCloseTimerDecrement + fasterSegmentClosingVar;
                            if (NPC.Calamity().newAI[0] <= 0f)
                            {
                                NPC.Calamity().newAI[0] = 0f;
                                NPC.Calamity().newAI[1] = 0f;
                            }
                        }
                    }
                }
            }
            else
            {
                if (NPC.ai[3] > 0f)
                    NPC.ai[3] = 0f;

                NPC.Calamity().newAI[0] -= segmentCloseTimerDecrement + fasterSegmentClosingVar;
                if (NPC.Calamity().newAI[0] <= 0f)
                {
                    NPC.Calamity().newAI[0] = 0f;
                    NPC.Calamity().newAI[1] = 0f;
                }
                else
                {
                    // Body is vulnerable while venting
                    vulnerable = true;
                }
            }

            // Do not deal contact damage for 5 seconds after spawning
            if (NPC.Calamity().newAI[2] == 0f)
                noContactDamageTimer = 300;

            if (NPC.Calamity().newAI[2] < ThanatosHead.immunityTime)
                NPC.Calamity().newAI[2] += 1f;

            // Homing only works if vulnerable is true
            NPC.chaseable = vulnerable;

            // Adjust DR based on vulnerable
            NPC.Calamity().DR = vulnerable ? 0f : 0.9999f;
            NPC.Calamity().unbreakableDR = !vulnerable;

            // Vent noise and steam
            SmokeDrawer.ParticleSpawnRate = 9999999;
            if (vulnerable)
            {
                // Noise
                float volume = calamityGlobalNPC_Head.newAI[0] == (float)ThanatosHead.Phase.Charge ? 0.1f : 1f;
                if (NPC.localAI[0] == 0f)
                    SoundEngine.PlaySound(ThanatosHead.VentSound with { Volume = volume * ThanatosHead.VentSound.Volume }, NPC.Center);

                // Steam
                NPC.localAI[0] += 1f;
                float actualVentDuration = lastMechAlive ? 90f : shouldGetBuffedByBerserkPhase ? 120f : ThanatosHead.ventDuration;
                if (NPC.localAI[0] < actualVentDuration)
                {
                    SmokeDrawer.BaseMoveRotation = NPC.rotation - MathHelper.PiOver2;
                    SmokeDrawer.ParticleSpawnRate = ThanatosHead.ventCloudSpawnRate;
                }
            }
            else
                NPC.localAI[0] = 0f;

            SmokeDrawer.Update();

            Player player = Main.player[head.target];

            Vector2 npcCenter = NPC.Center;
            float targetCenterX = player.Center.X;
            float targetCenterY = player.Center.Y;
            targetCenterX = (int)(targetCenterX / 16f) * 16;
            targetCenterY = (int)(targetCenterY / 16f) * 16;
            npcCenter.X = (int)(npcCenter.X / 16f) * 16;
            npcCenter.Y = (int)(npcCenter.Y / 16f) * 16;
            targetCenterX -= npcCenter.X;
            targetCenterY -= npcCenter.Y;

            float newPosition = (float)Math.Sqrt(targetCenterX * targetCenterX + targetCenterY * targetCenterY);
            if (NPC.ai[1] > 0f && NPC.ai[1] < Main.npc.Length)
            {
                try
                {
                    npcCenter = NPC.Center;
                    targetCenterX = Main.npc[(int)NPC.ai[1]].Center.X - npcCenter.X;
                    targetCenterY = Main.npc[(int)NPC.ai[1]].Center.Y - npcCenter.Y;
                }
                catch
                {
                }

                NPC.rotation = (float)Math.Atan2(targetCenterY, targetCenterX) + MathHelper.PiOver2;
                newPosition = (float)Math.Sqrt(targetCenterX * targetCenterX + targetCenterY * targetCenterY);
                newPosition = (newPosition - NPC.width) / newPosition;
                targetCenterX *= newPosition;
                targetCenterY *= newPosition;
                NPC.velocity = Vector2.Zero;
                NPC.position.X += targetCenterX;
                NPC.position.Y += targetCenterY;

                if (targetCenterX < 0f)
                    NPC.spriteDirection = -1;
                else if (targetCenterX > 0f)
                    NPC.spriteDirection = 1;
            }

            bool speedUp = head.localAI[3] < 180f;

            // Distance from target
            float distanceFromTarget = Vector2.Distance(head.Center, speedUp ? (player.Center + new Vector2(0f, 2400f)) : player.Center);

            // Increase speed if too far from target
            float increaseSpeedMult = 1f;
            float increaseSpeedGateValue = 600f;
            if (distanceFromTarget > increaseSpeedGateValue)
            {
                float distanceAmount = MathHelper.Clamp((distanceFromTarget - increaseSpeedGateValue) / (CalamityGlobalNPC.CatchUpDistance350Tiles - increaseSpeedGateValue), 0f, 1f);
                increaseSpeedMult = MathHelper.Lerp(1f, 3.5f, distanceAmount);
            }

            // Velocity and turn speed values
            float baseVelocityMult = (shouldGetBuffedByBerserkPhase ? 0.15f : 0f) + (bossRush ? 1.25f : death ? 1.2f : revenge ? 1.175f : expertMode ? 1.15f : 1.1f);
            float baseVelocity = 10f * baseVelocityMult;

            // Increase top velocity if target is dead or if Thanatos is uncoiling
            if (player.dead || speedUp)
                baseVelocity *= 4f;
            else
                baseVelocity *= increaseSpeedMult;

            if (Main.getGoodWorld)
                baseVelocity *= 1.15f;

            // Calculate contact damage based on velocity
            float minimalContactDamageVelocity = baseVelocity * 0.25f;
            float minimalDamageVelocity = baseVelocity * 0.5f;
            float bodyAndTailVelocity = (NPC.position - NPC.oldPosition).Length();
            if (bodyAndTailVelocity <= minimalContactDamageVelocity)
            {
                NPC.damage = 0;
            }
            else
            {
                float velocityDamageScalar = MathHelper.Clamp((bodyAndTailVelocity - minimalContactDamageVelocity) / minimalDamageVelocity, 0f, 1f);
                NPC.damage = (int)MathHelper.Lerp(0f, NPC.defDamage, velocityDamageScalar);
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;

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

            return minDist <= 45f && NPC.Opacity == 1f && noContactDamageTimer <= 0;
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (NPC.Calamity().newAI[2] < ThanatosHead.immunityTime)
                modifiers.SourceDamage *= 0.01f;
        }

        public override void FindFrame(int frameHeight) // 5 total frames
        {
            if (!Main.npc[(int)NPC.ai[2]].active || Main.npc[(int)NPC.ai[2]].life <= 0)
                return;

            // Swap between venting and non-venting frames
            CalamityGlobalNPC calamityGlobalNPC_Head = Main.npc[(int)NPC.ai[2]].Calamity();
            bool invisiblePhase = calamityGlobalNPC_Head.newAI[1] == (float)ThanatosHead.SecondaryPhase.PassiveAndImmune;
            bool shootLasers = (calamityGlobalNPC_Head.newAI[0] == (float)ThanatosHead.Phase.Charge || calamityGlobalNPC_Head.newAI[0] == (float)ThanatosHead.Phase.UndergroundLaserBarrage) && calamityGlobalNPC_Head.newAI[2] > 0f;
            NPC.frameCounter += 1D;
            if (NPC.Calamity().newAI[0] > 0f)
            {
                if (NPC.frameCounter >= 6D)
                {
                    NPC.frame.Y += frameHeight;
                    NPC.frameCounter = 0D;
                }
                int finalFrame = Main.npcFrameCount[NPC.type] - 1;
                if (NPC.frame.Y > frameHeight * finalFrame)
                    NPC.frame.Y = frameHeight * finalFrame;
            }
            else
            {
                if (NPC.frameCounter >= 6D)
                {
                    NPC.frame.Y -= frameHeight;
                    NPC.frameCounter = 0D;
                }
                if (NPC.frame.Y < 0)
                    NPC.frame.Y = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Vector2 vector = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2);

            Vector2 center = NPC.Center - screenPos;
            center -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            center += vector * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture, center, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, vector, NPC.scale, spriteEffects, 0f);

            texture = GlowTexture.Value;
            spriteBatch.Draw(texture, center, NPC.frame, Color.White * NPC.Opacity, NPC.rotation, vector, NPC.scale, spriteEffects, 0f);

            SmokeDrawer.DrawSet(NPC.Center);

            return false;
        }

        public override bool CheckActive() => false;

        public override void ModifyTypeName(ref string typeName)
        {
            int index = (int)NPC.ai[2];
            if (index < 0 || index >= Main.maxNPCs || Main.npc[index] is null)
                return;
            if (Main.npc[index].type != ModContent.NPCType<ThanatosHead>())
                return;

            if (Main.npc[index].ModNPC<ThanatosHead>().exoMechdusa)
            {
                typeName = CalamityUtils.GetTextValue("NPCs.ThanatosHead.HekateName");
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.soundDelay == 0)
            {
                if (vulnerable)
                {
                    NPC.soundDelay = 8;
                    SoundEngine.PlaySound(ThanatosHead.ThanatosHitSoundOpen, NPC.Center);
                }
                else
                {
                    NPC.soundDelay = 3;
                    SoundEngine.PlaySound(ThanatosHead.ThanatosHitSoundClosed, NPC.Center);
                }
            }

            int baseDust = vulnerable ? 3 : 1;
            for (int k = 0; k < baseDust; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TerraBlade, 0f, 0f, 100, new Color(0, 255, 255), 1f);

            if (NPC.life <= 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TerraBlade, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
                }
                for (int j = 0; j < 20; j++)
                {
                    int plasmaDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TerraBlade, 0f, 0f, 0, new Color(0, 255, 255), 2.5f);
                    Main.dust[plasmaDust].noGravity = true;
                    Main.dust[plasmaDust].velocity *= 3f;
                    plasmaDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TerraBlade, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
                    Main.dust[plasmaDust].velocity *= 2f;
                    Main.dust[plasmaDust].noGravity = true;
                }

                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ThanatosTail").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ThanatosTail2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ThanatosTail3").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ThanatosTail4").Type, 1f);
                }
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }
    }
}

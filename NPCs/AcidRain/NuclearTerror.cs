using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Enemy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Melee;

namespace CalamityMod.NPCs.AcidRain
{
    public class NuclearTerror : ModNPC
    {
        public enum SpecialAttackState
        {
            DivergingBullets,
            ConeStreamOfBullets,
            ShotgunBurstOfBullets
        }

        public int AttackIndex = 0;
        public int DelayTime = 0;
        public bool Dying = false;
        public bool Walking = false;
        public float JumpTimer = 0f;
        public Vector2 ShootPosition;
        public Player Target => Main.player[npc.target];
        public ref float AttackTime => ref npc.ai[0];
        public ref float TeleportCountdown => ref npc.ai[1];
        public Vector2 TeleportLocation
        {
            get => new Vector2(npc.ai[2], npc.ai[3]);
            set
            {
                npc.ai[2] = value.X;
                npc.ai[3] = value.Y;
            }
        }
        public ref float HorizontalCollisionCounterDelay => ref npc.localAI[0];
        public ref float HorizontalCollisionSpamCounter => ref npc.localAI[1];
        public static readonly SpecialAttackState[] PhaseArray = new SpecialAttackState[]
        {
            SpecialAttackState.ShotgunBurstOfBullets,
            SpecialAttackState.DivergingBullets,
            SpecialAttackState.ConeStreamOfBullets,
            SpecialAttackState.ConeStreamOfBullets,
            SpecialAttackState.ShotgunBurstOfBullets,
            SpecialAttackState.ConeStreamOfBullets,
            SpecialAttackState.DivergingBullets,
            SpecialAttackState.ShotgunBurstOfBullets,
            SpecialAttackState.ConeStreamOfBullets,
            SpecialAttackState.ConeStreamOfBullets,
            SpecialAttackState.DivergingBullets,
            SpecialAttackState.ConeStreamOfBullets,
            SpecialAttackState.ShotgunBurstOfBullets,
            SpecialAttackState.ConeStreamOfBullets,
            SpecialAttackState.DivergingBullets,
            SpecialAttackState.ShotgunBurstOfBullets,
            SpecialAttackState.ConeStreamOfBullets
        };
        public const int AttackCycleTime = 520;
        public const int SpecialAttackTime = 240;
        public const float TeleportTime = 60f;
        public const float TeleportFadeinTime = 10f;
        public const float TeleportCooldown = 60f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nuclear Terror");
            Main.npcFrameCount[npc.type] = 14;
            NPCID.Sets.TrailCacheLength[npc.type] = 6;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.Calamity().canBreakPlayerDefense = true;

            npc.width = 176;
            npc.height = 138;
            npc.aiStyle = aiType = -1;

            npc.lifeMax = 90000;
            npc.defense = 50;
            npc.damage = 135;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 20, 0, 0);
            npc.DR_NERD(0.3f);
            npc.lavaImmune = false;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit56;
            npc.DeathSound = SoundID.NPCDeath60;
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.dontTakeDamage);
            writer.Write(Dying);
            writer.Write(Walking);
            writer.Write(AttackIndex);
            writer.Write(DelayTime);
            writer.Write(JumpTimer);
            writer.WriteVector2(ShootPosition);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.dontTakeDamage = reader.ReadBoolean();
            Dying = reader.ReadBoolean();
            Walking = reader.ReadBoolean();
            AttackIndex = reader.ReadInt32();
            DelayTime = reader.ReadInt32();
            JumpTimer = reader.ReadSingle();
            ShootPosition = reader.ReadVector2();
        }

        public override void AI()
        {
            Lighting.AddLight(npc.Center, (Dying ? Color.Lime.ToVector3() : Color.White.ToVector3()) * 2f);
            if (Dying)
                return;

            bool phase2 = npc.life / (float)npc.lifeMax < 0.5f;
            if (DelayTime > 0)
            {
                DelayTime--;
                npc.velocity.X *= 0.9f;
                if (npc.velocity.Y < 18f)
                    npc.velocity.Y += 0.35f;
                return;
            }

            if (npc.target < 0 || npc.target >= 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest(false);
                npc.netUpdate = true;
            }

            if (TeleportCountdown > -TeleportCooldown)
                TeleportEffects();

            npc.defDamage = 170;
            npc.damage = Dying ? 0 : npc.defDamage;
            TeleportCheck();

            AttackTime++;
            float wrappedAttackTime = AttackTime % AttackCycleTime;

            Walking = false;

            // Teleport if spam-collisions are done, they are pretty good indicators of being stuck.
            if (npc.collideX)
            {
                if (HorizontalCollisionCounterDelay > 0)
                    HorizontalCollisionSpamCounter++;
                HorizontalCollisionCounterDelay = 20f;
            }

            if (HorizontalCollisionCounterDelay > 0)
                HorizontalCollisionCounterDelay--;

            if (wrappedAttackTime < 240f)
            {
                if (npc.velocity.Y == 0f)
                {
                    JumpTimer++;
                    npc.velocity.X *= 0.8f;

                    // Jump towards the target if enough time has passed or they're not in this enemy's line of sight.
                    if (JumpTimer >= 70f || !Collision.CanHit(npc.position, npc.width, npc.height, Target.position, Target.width, Target.height))
                    {
                        JumpTimer = 0f;
                        npc.velocity.Y -= MathHelper.Clamp(Math.Abs(Target.Center.Y - npc.Center.Y) / 12.5f, 8f, 18f);
                        npc.velocity.X = npc.SafeDirectionTo(Target.Center).X * 18f;
                        npc.netUpdate = true;
                    }
                    else
                    {
                        if (Walking != Math.Abs(npc.velocity.X) > 4f)
                        {
                            Walking = Math.Abs(npc.velocity.X) > 4f;
                            npc.netUpdate = true;
                        }

                        // Force a jump the next frame to overcome any horizontal obstacles if they exist.
                        if (npc.collideX)
                        {
                            JumpTimer = 50;
                            npc.netUpdate = true;
                        }

                        // Otherwise walk towards the target if they're not super close.
                        else if (Math.Abs(Target.Center.X - npc.Center.X) > 125f)
                        {
                            npc.velocity.X += Math.Sign(npc.SafeDirectionTo(Target.Center).X) * 3f;
                            npc.velocity.X = MathHelper.Clamp(npc.velocity.X, -28f, 28f);
                        }

                        // If they are close though, slow down horizontally.
                        else
                            npc.velocity.X *= 0.99f;
                    }
                }
                npc.spriteDirection = (npc.velocity.X < 0).ToDirectionInt();
            }
            else
            {
                npc.velocity.X *= 0.96f;
                if (wrappedAttackTime == 255f)
                {
                    ShootPosition = Target.Center;
                    npc.netUpdate = true;
                    npc.spriteDirection = (ShootPosition.X - npc.Center.X < 0).ToDirectionInt();
                }

                PerformSpecialAttack(wrappedAttackTime);

                if (wrappedAttackTime == AttackCycleTime - 1f)
                {
                    DelayTime = phase2 ? 45 : 75;
                    AttackIndex++;
                    AttackIndex %= PhaseArray.Length;
                }
            }
        }
        public void TeleportCheck()
        {
            float distanceFromTarget = npc.Distance(Target.Center);
            bool targetIsFarOff = distanceFromTarget > 900f;
            bool targetNotInLineOfSight = !Collision.CanHit(npc.position, npc.width, npc.height, Target.position, Target.width, Target.height);
            if (TeleportCountdown <= -TeleportCooldown)
            {
                if (distanceFromTarget <= 2700f && !(targetIsFarOff && targetNotInLineOfSight) && !StuckOnPlatform() && !npc.wet && HorizontalCollisionSpamCounter <= 5f)
                    return;

                Point playerPositionTileCoords = Target.position.ToTileCoordinates();
                Point npcPositionTileCoords = npc.position.ToTileCoordinates();

                for (int tries = 0; tries < 250; tries++)
                {
                    int maxTeleportDistance = 30 + tries / 3;
                    int x = Main.rand.Next(playerPositionTileCoords.X - maxTeleportDistance, playerPositionTileCoords.X + maxTeleportDistance);
                    int yStart = Main.rand.Next(playerPositionTileCoords.Y - maxTeleportDistance, playerPositionTileCoords.Y + maxTeleportDistance);

                    // Have a downward bias for the teleport if stuck on a platform above the target.
                    if (StuckOnPlatform())
                        yStart = Main.rand.Next(playerPositionTileCoords.Y, playerPositionTileCoords.Y + maxTeleportDistance * 4);

                    for (int y = yStart; y < playerPositionTileCoords.Y + maxTeleportDistance; y++)
                    {
                        Tile tileBelow = CalamityUtils.ParanoidTileRetrieval(x, y - 1);
                        bool veryCloseToTarget = Math.Abs(y - playerPositionTileCoords.Y) < 12 || Math.Abs(x - playerPositionTileCoords.X) < 12;
                        bool veryCloseToSelf = Math.Abs(y - npcPositionTileCoords.Y) < 12 || Math.Abs(x - npcPositionTileCoords.X) < 12;
                        bool solidGround = (Main.tileSolid[tileBelow.type] || Main.tileSolidTop[tileBelow.type]) && tileBelow.active();
                        if (!veryCloseToTarget && !veryCloseToSelf && solidGround)
                        {
                            // If the below tile has lava, skip it.
                            if (CalamityUtils.ParanoidTileRetrieval(x, y - 1).lava())
                                continue;

                            // If there's any tiles in the way, skip it.
                            if (Collision.SolidTiles(x - 12, x + 12, y - 7, y - 7))
                                continue;

                            // If there's any liquid near the tile, skip it.
                            for (int dy = y - 8; dy <= y + 8; dy++)
                            {
                                if (CalamityUtils.ParanoidTileRetrieval(x, dy).liquid > 0)
                                    goto Continue;
                            }

                            TeleportCountdown = TeleportTime;
                            TeleportLocation = new Vector2(x, y - 6f);
                            HorizontalCollisionSpamCounter = 0f;
                            npc.netUpdate = true;

                            return;
                        Continue:
                            continue;
                        }
                    }
                }
            }
        }

        public void TeleportEffects()
        {
            if (TeleportCountdown > TeleportTime)
                TeleportCountdown = TeleportTime;
            TeleportCountdown--;
            if (TeleportCountdown >= 0f)
            {
                if (TeleportCountdown == 0f && TeleportLocation != Vector2.Zero)
                {
                    npc.position = TeleportLocation.ToWorldCoordinates(8f, 0f) - npc.Size;
                    npc.netUpdate = true;
                    npc.velocity = Vector2.Zero;
                }
                else
                {
                    npc.Opacity = TeleportCountdown / TeleportTime;

                    int totalDust = (int)(30 * npc.alpha / 255f);
                    for (int i = 0; i < totalDust; i++)
                    {
                        Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid);
                        dust.noGravity = true;
                        dust.velocity = npc.DirectionFrom(dust.position) * 2f;
                        dust.scale = 1.6f;
                    }

                    // Fall and slow down horizontally.
                    npc.velocity.X *= 0.95f;
                    if (npc.velocity.Y < 18f)
                        npc.velocity.Y += 0.35f;
                }
                return;
            }

            // Release some dust before going back to normal.
            if (TeleportCountdown >= -TeleportFadeinTime)
            {
                npc.Opacity = TeleportCountdown / -TeleportFadeinTime;
                if (TeleportCountdown == -TeleportFadeinTime)
                {
                    for (int i = 0; i < 48; i++)
                    {
                        Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid);
                        dust.noGravity = true;
                        dust.velocity = npc.DirectionFrom(dust.position) * Main.rand.NextFloat(2f, 3.6f);
                        dust.scale = 1.8f;
                    }
                }
            }
        }

        public bool StuckOnPlatform()
        {
            for (int i = -12; i < 12; i++)
            {
                Point bottom = (npc.Bottom + Vector2.UnitY * i).ToTileCoordinates();
                if (TileID.Sets.Platforms[CalamityUtils.ParanoidTileRetrieval(bottom.X, bottom.Y).type] && Target.Top.Y > npc.Bottom.Y + 48)
                    return true;
            }
            return false;
        }

        public void PerformSpecialAttack(float wrappedAttackTime)
        {
            Vector2 mouthPosition = npc.Center - Vector2.UnitY * 26f;
            mouthPosition.X += npc.spriteDirection * -54f;

            TeleportCountdown = -TeleportCooldown;
            Vector2 directionToTarget = (Target.Center - mouthPosition).SafeNormalize(Vector2.UnitX * npc.spriteDirection);
            Vector2 directionToShootPosition = (ShootPosition - mouthPosition).SafeNormalize(Vector2.UnitX * npc.spriteDirection);
            switch (PhaseArray[AttackIndex])
            {
                case SpecialAttackState.DivergingBullets:
                    npc.velocity.X *= 0.9f;
                    float shootAdjustedTime = wrappedAttackTime - (AttackCycleTime - SpecialAttackTime);
                    bool shouldShoot = shootAdjustedTime >= 20f;
                    shouldShoot &= (shootAdjustedTime - 20f) % 30f < 12f;
                    shouldShoot &= shootAdjustedTime <= 152f;
                    shouldShoot &= shootAdjustedTime % 3f == 0f;

                    if (shouldShoot)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float angle = (wrappedAttackTime - (AttackCycleTime - SpecialAttackTime + 20f)) % 12f / 12f * MathHelper.ToRadians(15f) - MathHelper.ToRadians(7.5f);
                            int bullet = Projectile.NewProjectile(mouthPosition, directionToShootPosition.RotatedBy(angle) * 14f, ModContent.ProjectileType<NuclearBulletLarge>(), 48, 4f);
                            Main.projectile[bullet].localAI[0] = angle;
                        }
                        npc.spriteDirection = (ShootPosition.X - npc.Center.X < 0).ToDirectionInt();
                        Main.PlaySound(SoundID.NPCDeath13, mouthPosition);
                    }
                    if (wrappedAttackTime >= (AttackCycleTime - SpecialAttackTime + 35f) && AttackTime % 10f == 9f)
                        Projectile.NewProjectile(mouthPosition, directionToTarget * 12f, ModContent.ProjectileType<NuclearBulletLarge>(), 48, 3f);
                    break;
                case SpecialAttackState.ConeStreamOfBullets:
                    if (wrappedAttackTime >= AttackCycleTime - SpecialAttackTime + 35f && AttackTime % 4f == 3f)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float angle = MathHelper.Lerp(MathHelper.ToRadians(35f), MathHelper.ToRadians(5f), (wrappedAttackTime - (AttackCycleTime - SpecialAttackTime + 35f)) / (SpecialAttackTime + 35f));
                            Projectile.NewProjectile(mouthPosition, directionToTarget.RotatedBy(angle) * 16f, ModContent.ProjectileType<NuclearBulletLarge>(), 48, 4.5f);
                            Projectile.NewProjectile(mouthPosition, directionToTarget.RotatedBy(-angle) * 16f, ModContent.ProjectileType<NuclearBulletLarge>(), 48, 4.5f);
                        }
                        npc.spriteDirection = (Target.Center.X - npc.Center.X < 0).ToDirectionInt();
                    }
                    break;
                case SpecialAttackState.ShotgunBurstOfBullets:
                    if (wrappedAttackTime >= AttackCycleTime - SpecialAttackTime + 35f && AttackTime % 20f == 19f)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                float angle = MathHelper.Lerp(-0.5f, 0.5f, i / 3f);
                                Projectile.NewProjectile(mouthPosition, directionToShootPosition.RotatedBy(angle) * 13f, ModContent.ProjectileType<NuclearBulletMedium>(), 48, 4f);
                            }
                        }
                        npc.spriteDirection = (ShootPosition.X - npc.Center.X < 0).ToDirectionInt();
                    }
                    break;
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.85f);
        }
        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            int frameChangeRate = Dying ? 7 : 6;

            // Walk faster the faster this thing is moving.
            if (Walking)
                frameChangeRate = 8 - (int)Math.Ceiling(Math.Abs(npc.velocity.X) / 5f);

            if (npc.frameCounter >= frameChangeRate)
            {
                npc.frame.Y += frameHeight;
                npc.frameCounter = 0;
            }
            if (Dying)
            {
                if (npc.frame.Y < frameHeight * 8)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            int type = Main.rand.NextBool(4) ? ModContent.ProjectileType<SulphuricAcidMist>() : ModContent.ProjectileType<NuclearBulletLarge>();
                            float angle = MathHelper.TwoPi / 16f * i;
                            Projectile.NewProjectile(npc.Center, angle.ToRotationVector2() * Main.rand.NextFloat(4f, 11f), type, 48, 3f);
                        }
                    }
                    for (int i = 0; i < 60; i++)
                    {
                        Dust dust = Dust.NewDustDirect(npc.Center, 45, 45, (int)CalamityDusts.SulfurousSeaAcid);
                        dust.velocity = Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(4f, 15f);
                        dust.noGravity = true;
                        dust.scale = Main.rand.NextFloat(2f, 3f);
                    }
                    npc.frame.Y = frameHeight * 8;
                }
                if (npc.frame.Y >= frameHeight * Main.npcFrameCount[npc.type])
                    npc.StrikeNPCNoInteraction(9999, 0f, 0);
            }
            else if (npc.frame.Y >= (Walking ? 8 : 4) * frameHeight)
                npc.frame.Y = Walking ? 4 * frameHeight : 0;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (npc.velocity.Length() > 0f)
            {
                Color endColor = Color.DarkOliveGreen;
                endColor.A = Color.Transparent.A;
                CalamityGlobalNPC.DrawAfterimage(npc, spriteBatch, drawColor, endColor, directioning: true, invertedDirection: true);
            }
            CalamityGlobalNPC.DrawGlowmask(npc, spriteBatch, null, true);
            return false;
        }

        public override bool CheckDead()
        {
            if (!Dying)
            {
                Dying = true;
                npc.active = true;
                npc.life = 1;
                npc.dontTakeDamage = true;
                npc.velocity = Vector2.Zero;
                npc.netUpdate = true;
                return false;
            }
            return Dying;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 10; k++)
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit) => target.AddBuff(ModContent.BuffType<Irradiated>(), 300);

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<GammaHeart>(), 3);
            DropHelper.DropItemChance(npc, ModContent.ItemType<PhosphorescentGauntlet>(), 3);
        }
    }
}

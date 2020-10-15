using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class MechwormHead : ModProjectile
    {
        internal enum AttackState : byte
        {
            PortalGateCharge,
            LaserCharge
        }
        internal int DustProduceCounter = 3;
        internal int AttackStateTimer = 0;
        internal int EndRiftGateUUID = -1;
        internal Vector2 TeleportStartingPoint;
        internal Vector2 TeleportEndingPoint;
        internal AttackState CurrentAttackState = AttackState.LaserCharge;
        internal const int MaxSegmentsToCountForScaling = 50;
        internal const int AttackStateShiftTime = 320;
        internal const float MaxAttackFlySpeed = 33f;

        internal ref float Time => ref projectile.ai[1];
        internal ref float TotalWormSegments => ref projectile.localAI[0];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mechworm");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            ProjectileID.Sets.NeedsUUID[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.netImportant = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 18000;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
            projectile.hide = true;
        }

        #region Syncing
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((byte)CurrentAttackState);
            writer.Write(AttackStateTimer);
            writer.Write(EndRiftGateUUID);
            writer.WriteVector2(TeleportStartingPoint);
            writer.WriteVector2(TeleportEndingPoint);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            CurrentAttackState = (AttackState)reader.ReadByte();
            AttackStateTimer = reader.ReadInt32();
            EndRiftGateUUID = reader.ReadInt32();
            TeleportStartingPoint = reader.ReadVector2();
            TeleportEndingPoint = reader.ReadVector2();
        }
        #endregion

        #region AI
        public override void AI()
        {
            if (projectile.alpha <= 128)
                Lighting.AddLight(projectile.Center, Color.DarkMagenta.ToVector3());

            projectile.Center = Vector2.Clamp(projectile.Center, new Vector2(160f), new Vector2(Main.maxTilesX - 10, Main.maxTilesY - 10) * 16);

            Player owner = Main.player[projectile.owner];

            // Produce some dust when the worm is summoned.
            if (DustProduceCounter > 0 && !Main.dedServ)
            {
                for (int i = 0; i < 50; i++)
                {
                    Dust purpleElectricity = Dust.NewDustDirect(projectile.position + Vector2.UnitY * 16f, projectile.width, projectile.height - 16, 234, 0f, 0f, 0, default, 1f);
                    purpleElectricity.velocity *= 2f;
                    purpleElectricity.scale *= 1.15f;
                }
                DustProduceCounter--;
            }
            CalamityPlayer modPlayer = owner.Calamity();

            // Minion stuff.

            owner.AddBuff(ModContent.BuffType<Mechworm>(), 3600);
            if (owner.dead)
                modPlayer.mWorm = false;

            if (modPlayer.mWorm)
                projectile.timeLeft = 2;

            Time++;

            if (!Main.projectile.IndexInRange(EndRiftGateUUID))
            {
                // Very rapidly fade-in.
                projectile.alpha = Utils.Clamp(projectile.alpha - 42, 0, 255);
            }
            else if (projectile.Hitbox.Intersects(Main.projectile[EndRiftGateUUID].Hitbox))
            {
                // Disappear if touching the mechworm portal.
                // It will look like it's teleporting, when in reality, it's
                // just an invisible, uninteractable projectile for the time being.

                if (projectile.alpha != 255)
                {
                    for (int i = 0; i < 35; i++)
                    {
                        Dust burstDust = Dust.NewDustPerfect(projectile.Center, (int)CalamityDusts.PurpleCosmolite);
                        burstDust.velocity = (MathHelper.TwoPi * i / 35f).ToRotationVector2().RotatedByRandom(0.035f) * 12f;
                        burstDust.noGravity = true;

                        burstDust = Dust.NewDustDirect(projectile.Center, 70, 70, (int)CalamityDusts.PurpleCosmolite);
                        burstDust.velocity = Main.rand.NextVector2CircularEdge(6f, 6f);
                        burstDust.scale = Main.rand.NextFloat(1.3f, 1.75f);
                    }

                    Main.PlaySound(SoundID.Item8, projectile.Center);
                    projectile.alpha = 255;
                }
            }

            // Sync the entire worm every 2 seconds
            // This has potential to cause a packet storm, but only if the player is cheating in some way by freezing time.
            if ((int)Main.time % 120 == 0)
                projectile.netUpdate = true;

            NPC potentialTarget = projectile.Center.MinionHoming(AttackStateTimer > 0 ? 999999f : 2200f, owner);

            // Make sure that a corresponding tail exists with this head projectile.
            // If it doesn't, kill the head (and all associated segments as a result).
            if (!WormTailCheck())
            {
                projectile.Kill();
                return;
            }

            // Teleport to the player if the worm is far from them.
            if (projectile.Distance(owner.Center) > 2700f)
            {
                projectile.Center = owner.Center;
                projectile.netUpdate = true;
            }

            // Special movement.
            if ((potentialTarget != null || AttackStateTimer > 1) && Time > 150f)
            {
                AttackMovement(potentialTarget);

                if (AttackStateTimer++ >= AttackStateShiftTime)
                {
                    if (CurrentAttackState == AttackState.PortalGateCharge)
                    {
                        // Delete any remaining projectile spawned by this worm, just in case.
                        int portalType = ModContent.ProjectileType<MechwormTeleportRift>();
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            Projectile proj = Main.projectile[i];
                            if (proj.type != portalType || !proj.active || proj.owner != projectile.owner)
                                continue;

                            proj.Kill();
                            if (!Main.dedServ)
                            {
                                for (int j = 0; j < 16; j++)
                                {
                                    Dust.NewDustDirect(proj.position, 45, 45, (int)CalamityDusts.PurpleCosmolite);
                                }
                            }
                        }
                    }
                    CurrentAttackState = CurrentAttackState == AttackState.LaserCharge ? AttackState.PortalGateCharge : AttackState.LaserCharge;
                    AttackStateTimer = 0;
                }
            }
            else
                PlayerFollowMovement(owner);

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // Update the segment direction based on the velocity.
            int previousDirection = projectile.direction;
            projectile.direction = projectile.spriteDirection = (projectile.velocity.X > 0f).ToDirectionInt();

            // If it changed for some reason, sync the head.
            if (previousDirection != projectile.direction)
            {
                // Negate the anti-spam for this particular sync.
                // Spamming may be required in this case.
                projectile.netSpam -= 5;
                projectile.netUpdate = true;
            }
        }

        public bool WormTailCheck()
        {
            int tailType = ModContent.ProjectileType<MechwormTail>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].type != tailType || !Main.projectile[i].active || Main.projectile[i].owner != projectile.owner)
                    continue;
                return true;
            }
            return false;
        }

        public void PlayerFollowMovement(Player owner)
        {
            // Reset the gate UUID from any previous teleports.
            if (EndRiftGateUUID != -1)
            {
                EndRiftGateUUID = -1;
                projectile.netUpdate = true;
            }

            AttackStateTimer = 0;

            float hoverAcceleration = 0.2f;
            float distanceFromOwner = projectile.Distance(owner.Center);
            if (distanceFromOwner < 200f)
                hoverAcceleration = 0.12f;
            if (distanceFromOwner < 140f)
                hoverAcceleration = 0.06f;

            if (distanceFromOwner > 100f)
            {
                if (Math.Abs(owner.Center.X - projectile.Center.X) > 20f)
                    projectile.velocity.X += hoverAcceleration * Math.Sign(owner.Center.X - projectile.Center.X);
                if (Math.Abs(owner.Center.Y - projectile.Center.Y) > 10f)
                    projectile.velocity.Y += hoverAcceleration * Math.Sign(owner.Center.Y - projectile.Center.Y);
            }
            else if (projectile.velocity.Length() > 1f)
                projectile.velocity *= 0.96f;

            if (Math.Abs(projectile.velocity.Y) < 1f)
                projectile.velocity.Y -= 0.1f;

            float maxSpeed = Time < 150f ? 13f : 25f;
            if (projectile.velocity.Length() > maxSpeed)
                projectile.velocity = Vector2.Normalize(projectile.velocity) * maxSpeed;
        }

        public void AttackMovement(NPC target)
        {
            if (target is null)
            {
                AttackStateTimer = 0;
                return;
            }
            if (CurrentAttackState == AttackState.LaserCharge)
            {
                int chargeTime = 45;
                int redirectTime = 30;
                if (AttackStateTimer % (chargeTime + redirectTime) < redirectTime)
                {
                    float angularTurnSpeed = MathHelper.ToRadians(18f);
                    float newSpeed = MathHelper.Lerp(projectile.velocity.Length(), 24f, 0.35f);

                    if (projectile.Distance(target.Center) > 1100f)
                        newSpeed = MathHelper.Lerp(projectile.velocity.Length(), 38f, 0.35f);

                    projectile.velocity = projectile.velocity.ToRotation().AngleTowards(projectile.AngleTo(target.Center), angularTurnSpeed).ToRotationVector2() * newSpeed;

                    // If the worm is very close to aiming directly at the target, jump
                    // immediately to the next charge phase.
                    if (Vector2.Dot(projectile.velocity.SafeNormalize(Vector2.Zero), projectile.DirectionTo(target.Center)) > 0.86f)
                        AttackStateTimer += redirectTime - (AttackStateTimer % chargeTime);
                }
                if (AttackStateTimer % (chargeTime + redirectTime) == chargeTime)
                {
                    // Charge and release a spread of lasers.
                    projectile.velocity = projectile.DirectionTo(target.Center) * MaxAttackFlySpeed;

                    if (Main.myPlayer == projectile.owner)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 perturbedSpeed = projectile.velocity.RotatedBy(MathHelper.Lerp(-0.15f, 0.15f, i / 3f)) * 0.3f;
                            Projectile.NewProjectile(projectile.Center, perturbedSpeed, ModContent.ProjectileType<MechwormLaser>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                        }
                    }

                    Main.PlaySound(SoundID.Item12, projectile.Center);
                    projectile.netUpdate = true;
                }

                // Reset the gate UUID from any previous teleports.
                if (EndRiftGateUUID != -1)
                {
                    EndRiftGateUUID = -1;
                    projectile.netUpdate = true;
                }
            }
            else
            {
                int chargeTime = (int)MathHelper.Min(36 + TotalWormSegments, 70);
                if (AttackStateTimer % chargeTime == 0)
                {
                    Vector2 offsetBounds = Vector2.Max(target.Size, new Vector2(425f + TotalWormSegments * 8f));
                    Vector2 offset = Main.rand.NextVector2CircularEdge(offsetBounds.X, offsetBounds.Y) * 0.65f;

                    // Dont teleport on the very first portal summon. Fly into a portal and THEN teleport later.
                    if (AttackStateTimer != 0)
                        TeleportStartingPoint = target.Center + offset;

                    if (AttackStateTimer != 0)
                        TeleportEndingPoint = target.Center - offset;
                    else
                        TeleportEndingPoint = projectile.Center + projectile.velocity * chargeTime / 2f;

                    if (Main.myPlayer == projectile.owner)
                    {
                        Projectile.NewProjectile(TeleportStartingPoint, Vector2.Zero, ModContent.ProjectileType<MechwormTeleportRift>(), 0, 0f, projectile.owner);
                        int endGateIndex = Projectile.NewProjectile(TeleportEndingPoint, Vector2.Zero, ModContent.ProjectileType<MechwormTeleportRift>(), 0, 0f, projectile.owner);
                        EndRiftGateUUID = Projectile.GetByUUID(projectile.owner, endGateIndex);

                        Main.projectile[EndRiftGateUUID].ai[0] = chargeTime;
                        Main.projectile[EndRiftGateUUID].timeLeft = chargeTime;
                    }

                    // Dont teleport on the very first portal summon.
                    if (AttackStateTimer != 0)
                        projectile.Center = TeleportStartingPoint;

                    // Reset the alpha and position across the entire worm for the next charge.
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];
                        bool isProjectileMechwormSegment =
                            proj.type == ModContent.ProjectileType<MechwormHead>() ||
                            proj.type == ModContent.ProjectileType<MechwormBody>() ||
                            proj.type == ModContent.ProjectileType<MechwormTail>();

                        if (proj.active && proj.owner == projectile.owner && isProjectileMechwormSegment)
                        {
                            proj.alpha = 0;
                            if (AttackStateTimer != 0)
                            {
                                proj.Center = projectile.Center;
                                proj.netUpdate = true;
                            }
                        }
                    }

                    projectile.alpha = 0;
                    projectile.velocity = projectile.DirectionTo(TeleportEndingPoint) * (MaxAttackFlySpeed + target.velocity.Length() * 0.45f);
                    projectile.netUpdate = true;
                }
            }
        }
        #endregion

        #region Drawing

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.alpha > 200)
                return;

            Vector2 origin = new Vector2(21f, 25f);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Summon/MechwormHeadGlow"), projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindProjectiles.Add(index);
        }

        #endregion
    }
}

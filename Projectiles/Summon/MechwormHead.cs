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
        internal int AttackStateTimer = 0;
        internal int EndRiftGateUUID = -1;
        internal Vector2 TeleportStartingPoint;
        internal Vector2 TeleportEndingPoint;
        internal AttackState CurrentAttackState = AttackState.LaserCharge;

        internal const int MaxSegmentsToCountForScaling = 50;
        internal const int AttackStateShiftTime = 320;
        internal const int StartupLethargy = 150;
        internal const int LaserChargeFrames = 45;
        internal const int LaserRedirectFrames = 30;
        internal const float MaxAttackFlySpeed = 33f;

        internal ref float Time => ref projectile.ai[1];
        internal ref float TotalWormSegments => ref projectile.localAI[0];

        private static bool Use_TML_0_11_7_7_Hacky_Netcode = true;


        // Helper functions because Mechworm does a lot of checking for either itself or its target being near the edge of the world.
        private static Vector2 WorldTopLeft(int tileDist = 15) => new Vector2(tileDist * 16f);
        private static Vector2 WorldBottomRight(int tileDist = 15) => new Vector2(Main.maxTilesX - tileDist, Main.maxTilesY - tileDist) * 16f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mechworm");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            ProjectileID.Sets.NeedsUUID[projectile.type] = true;

            if (ModLoader.version.CompareTo(new Version(0, 11, 7, 7)) > 0)
            {
                Use_TML_0_11_7_7_Hacky_Netcode = false;
                mod.Logger.Info("ModLoader version > 0.11.7.7 detected. Mechworm UUID workaround is disabled.");
            }
            else
                mod.Logger.Info("ModLoader version <= 0.11.7.7 detected. Mechworm UUID workaround is enabled.");
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
            projectile.localNPCHitCooldown = 10;
            projectile.hide = true;
        }

        #region Syncing
        public override void SendExtraAI(BinaryWriter writer)
        {
            // TODO -- remove when TML updates to 0.11.7.8
            // TML 0.11.7.7 SPECIFIC FIX (because they were too slow to update): Write an extra UUID here.
            // This is necessary because NetMessage case 27 and MessageBuffer case 27 are out of order with each other.
            if (Use_TML_0_11_7_7_Hacky_Netcode)
                writer.Write((short)projectile.projUUID);

            byte enumByte = (byte)CurrentAttackState;
            writer.Write(enumByte);
            writer.Write(AttackStateTimer);
            // localAI and alpha are not normally synced, so sync those
            writer.Write(TotalWormSegments);
            writer.Write(projectile.alpha);
            writer.Write(EndRiftGateUUID);
            writer.WriteVector2(TeleportStartingPoint);
            writer.WriteVector2(TeleportEndingPoint);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            byte enumByte = reader.ReadByte();
            CurrentAttackState = (AttackState)enumByte;
            AttackStateTimer = reader.ReadInt32();
            TotalWormSegments = reader.ReadSingle();
            projectile.alpha = reader.ReadInt32();
            EndRiftGateUUID = reader.ReadInt32();
            TeleportStartingPoint = reader.ReadVector2();
            TeleportEndingPoint = reader.ReadVector2();

            // TODO -- remove when TML updates to 0.11.7.8
            // TML 0.11.7.7 SPECIFIC FIX (because they were too slow to update): Read and dump an extra UUID here.
            // This is necessary because NetMessage case 27 and MessageBuffer case 27 are out of order with each other.
            if (Use_TML_0_11_7_7_Hacky_Netcode)
                _ = reader.ReadInt16();
        }
        #endregion

        #region AI
        public override void AI()
        {
            // If the mechworm is opaque enough, produce light.
            if (projectile.alpha <= 128)
                Lighting.AddLight(projectile.Center, Color.DarkMagenta.ToVector3());

            // Stops the mechworm from getting too close to the world boundary. Projectiles can instantly cause crashes when they cross the world boundary.
            projectile.Center = Vector2.Clamp(projectile.Center, WorldTopLeft(10), WorldBottomRight(10));

            Player owner = Main.player[projectile.owner];

            // Produce some dust when the worm is summoned.
            if (Time < 3 && !Main.dedServ)
                for (int i = 0; i < 50; i++)
                {
                    Dust purpleElectricity = Dust.NewDustDirect(projectile.position + Vector2.UnitY * 16f, projectile.width, projectile.height - 16, 234, 0f, 0f, 0, default, 1f);
                    purpleElectricity.velocity *= 2f;
                    purpleElectricity.scale *= 1.15f;
                }
            CalamityPlayer modPlayer = owner.Calamity();

            // Maintain or remove the Mechworm buff from the owner.
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
                        Dust burstDust = Dust.NewDustPerfect(projectile.Center, (int)CalamityDusts.PurpleCosmilite);
                        burstDust.velocity = (MathHelper.TwoPi * i / 35f).ToRotationVector2().RotatedByRandom(0.035f) * 12f;
                        burstDust.noGravity = true;

                        burstDust = Dust.NewDustDirect(projectile.Center, 70, 70, (int)CalamityDusts.PurpleCosmilite);
                        burstDust.velocity = Main.rand.NextVector2CircularEdge(6f, 6f);
                        burstDust.scale = Main.rand.NextFloat(1.3f, 1.75f);
                    }

                    Main.PlaySound(SoundID.Item8, projectile.Center);
                    projectile.alpha = 255;
                }
            }

            // Mechworm has an extremely generous default aggro range of 2200, but if it's already attacking, its bloodlust is insatiable.
            NPC potentialTarget = projectile.Center.MinionHoming(AttackStateTimer > 0 ? 999999f : 2200f, owner);

            // Teleport to the player if the worm is very far away from them.
            if (projectile.Distance(owner.Center) > 2700f)
            {
                projectile.Center = owner.Center;
                // Reset the worm's velocity when it returns to the player so that it doesn't instantly yeet off somewhere.
                projectile.velocity = Main.rand.NextVector2CircularEdge(3f, 3f);
                projectile.netUpdate = true;
            }

            // Don't bother attacking if the target is close to the world edge, to prevent issues.
            if (potentialTarget != null && Time > StartupLethargy && TargetInSafeBoundaries(potentialTarget))
            {
                if (CurrentAttackState == AttackState.LaserCharge)
                    LaserAttackMovement(potentialTarget);
                else
                    PortalAttackMovement(potentialTarget);
                UpdateAttackStates();
            }
            // Attacking movement can be canceled, so if it was, run the passive movement instead.
            else
                PlayerFollowMovement(owner);

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // Update the segment direction based on the velocity.
            int previousDirection = projectile.direction;
            projectile.direction = projectile.spriteDirection = (projectile.velocity.X > 0f).ToDirectionInt();

            // If it changed for some reason, fire a net update. This update cannot be blocked by netSpam.
            if (previousDirection != projectile.direction)
            {
                projectile.netUpdate = true;
                if (projectile.netSpam > 59)
                    projectile.netSpam = 59;
            }
        }

        private bool TailExists()
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

        private bool TargetInSafeBoundaries(NPC target) => target?.Center.Between(WorldTopLeft(), WorldBottomRight()) ?? true;

        private void PlayerFollowMovement(Player owner)
        {
            // Reset the gate UUID from any previous teleports.
            if (EndRiftGateUUID != -1)
            {
                EndRiftGateUUID = -1;
                projectile.netUpdate = true;
            }

            // If any attack was in use previously, send a net update now that attack mode is off.
            if (AttackStateTimer != 0)
            {
                AttackStateTimer = 0;
                projectile.netUpdate = true;
            }

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

            // The worm's max speed is more strictly capped for the first few seconds.
            float maxSpeed = Time < StartupLethargy ? 13f : 25f;
            if (projectile.velocity.Length() > maxSpeed)
                projectile.velocity = Vector2.Normalize(projectile.velocity) * maxSpeed;
        }

        private void LaserAttackMovement(NPC target)
        {
            // Reset the gate UUID from any previous teleports.
            if (EndRiftGateUUID != -1)
            {
                EndRiftGateUUID = -1;
                projectile.netUpdate = true;
            }

            // If the timer indicates the worm is in redirect mode, then angle towards the target.
            if (AttackStateTimer % (LaserChargeFrames + LaserRedirectFrames) < LaserRedirectFrames)
            {
                float angularTurnSpeed = MathHelper.ToRadians(18f);
                float newSpeed = MathHelper.Lerp(projectile.velocity.Length(), 24f, 0.35f);

                if (projectile.Distance(target.Center) > 1100f)
                    newSpeed = MathHelper.Lerp(projectile.velocity.Length(), 38f, 0.35f);

                projectile.velocity = projectile.velocity.ToRotation().AngleTowards(projectile.AngleTo(target.Center), angularTurnSpeed).ToRotationVector2() * newSpeed;

                // If the worm is very close to aiming directly at the target, immediately switch from redirecting to charging.
                if (Vector2.Dot(projectile.velocity.SafeNormalize(Vector2.Zero), projectile.SafeDirectionTo(target.Center)) > 0.86f)
                {
                    AttackStateTimer += LaserRedirectFrames - (AttackStateTimer % LaserChargeFrames);
                    projectile.netUpdate = true;
                }
            }

            // On the exact frame the worm enters charge mode, fire 3 lasers and send a net update.
            if (AttackStateTimer % (LaserChargeFrames + LaserRedirectFrames) == LaserChargeFrames)
            {
                // Charge and fire three lasers.
                projectile.velocity = projectile.SafeDirectionTo(target.Center) * MaxAttackFlySpeed;

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

            // If neither of the above if-statements trigger, the worm just moves forwards in a straight line and this AI function does nothing.
        }

        private void PortalAttackMovement(NPC target)
        {
            // Instantly abort and switch to laser mode if the target is too close to the edges of the world.
            if (!target.Center.Between(WorldTopLeft(37), WorldBottomRight(37)))
            {
                CurrentAttackState = AttackState.LaserCharge;
                EndRiftGateUUID = -1;
                projectile.netUpdate = true;
                return;
            }

            int chargeTime = (int)MathHelper.Min(36 + TotalWormSegments, 70);
            if (AttackStateTimer % chargeTime == 0)
            {
                Vector2 offsetBounds = Vector2.Max(target.Size, new Vector2(425f + TotalWormSegments * 8f));
                Vector2 offset = Main.rand.NextVector2CircularEdge(offsetBounds.X, offsetBounds.Y) * 0.65f;

                // Dont teleport on the very first portal summon. Fly into a portal and THEN teleport later.
                if (AttackStateTimer != 0)
                {
                    TeleportStartingPoint = target.Center + offset;
                    TeleportEndingPoint = target.Center - offset;
                }
                else
                    TeleportEndingPoint = projectile.Center + projectile.velocity * chargeTime / 2f;

                // On the starting frame of a teleport, spawn portals.
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
                    Projectile otherProj = Main.projectile[i];
                    if (!otherProj.active || otherProj.owner != projectile.owner || i == projectile.whoAmI)
                        continue;

                    if (otherProj.type == ModContent.ProjectileType<MechwormBody>() || otherProj.type == ModContent.ProjectileType<MechwormTail>())
                    {
                        otherProj.alpha = 0;
                        if (AttackStateTimer != 0)
                            otherProj.Center = projectile.Center;
                        // There is no need to set the other projectiles to net update. They will do so when the head does.
                    }
                }

                projectile.alpha = 0;
                projectile.velocity = projectile.SafeDirectionTo(TeleportEndingPoint) * (MaxAttackFlySpeed + target.velocity.Length() * 0.45f);
                projectile.netUpdate = true;
            }
        }

        private void UpdateAttackStates()
        {
            // If the current attack state is out of time, pick a new one.
            if (++AttackStateTimer >= AttackStateShiftTime)
            {
                // When leaving portal-charge state, delete any remaining portals spawned by this worm.
                if (CurrentAttackState == AttackState.PortalGateCharge)
                    CleanUpMechwormPortals();

                CurrentAttackState = CurrentAttackState == AttackState.LaserCharge ? AttackState.PortalGateCharge : AttackState.LaserCharge;
                AttackStateTimer = 0;
                projectile.netUpdate = true;
            }
        }

        private void CleanUpMechwormPortals()
        {
            int portalType = ModContent.ProjectileType<MechwormTeleportRift>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.type != portalType || !proj.active || proj.owner != projectile.owner)
                    continue;

                proj.Kill();
                // Spawn a little bit of dust when the portals are destroyed.
                if (!Main.dedServ)
                    for (int j = 0; j < 16; j++)
                        Dust.NewDustDirect(proj.position, 45, 45, (int)CalamityDusts.PurpleCosmilite);
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

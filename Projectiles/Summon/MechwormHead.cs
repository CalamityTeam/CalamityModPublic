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
using Terraria.Audio;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Summon
{
    public class MechwormHead : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
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

        internal ref float Time => ref Projectile.ai[1];
        internal ref float TotalWormSegments => ref Projectile.localAI[0];

        // Helper functions because Mechworm does a lot of checking for either itself or its target being near the edge of the world.
        private static Vector2 WorldTopLeft(int tileDist = 15) => new Vector2(tileDist * 16f);
        private static Vector2 WorldBottomRight(int tileDist = 15) => new Vector2(Main.maxTilesX - tileDist, Main.maxTilesY - tileDist) * 16f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.hide = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        #region Syncing
        public override void SendExtraAI(BinaryWriter writer)
        {
            byte enumByte = (byte)CurrentAttackState;
            writer.Write(enumByte);
            writer.Write(AttackStateTimer);
            // localAI and alpha are not normally synced, so sync those
            writer.Write(TotalWormSegments);
            writer.Write(Projectile.alpha);
            writer.Write(EndRiftGateUUID);
            writer.WriteVector2(TeleportStartingPoint);
            writer.WriteVector2(TeleportEndingPoint);
            writer.Write(Projectile.extraUpdates);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            byte enumByte = reader.ReadByte();
            CurrentAttackState = (AttackState)enumByte;
            AttackStateTimer = reader.ReadInt32();
            TotalWormSegments = reader.ReadSingle();
            Projectile.alpha = reader.ReadInt32();
            EndRiftGateUUID = reader.ReadInt32();
            TeleportStartingPoint = reader.ReadVector2();
            TeleportEndingPoint = reader.ReadVector2();
            Projectile.extraUpdates = reader.ReadInt32();
        }
        #endregion

        #region AI
        public override void AI()
        {
            // If the mechworm is opaque enough, produce light.
            if (Projectile.alpha <= 128)
                Lighting.AddLight(Projectile.Center, Color.DarkMagenta.ToVector3());

            // Stops the mechworm from getting too close to the world boundary. Projectiles can instantly cause crashes when they cross the world boundary.
            Projectile.Center = Vector2.Clamp(Projectile.Center, WorldTopLeft(10), WorldBottomRight(10));

            Player owner = Main.player[Projectile.owner];

            // Produce some dust when the worm is summoned.
            if (Time < 3 && !Main.dedServ)
            {
                for (int i = 0; i < 50; i++)
                {
                    Dust purpleElectricity = Dust.NewDustDirect(Projectile.position + Vector2.UnitY * 16f, Projectile.width, Projectile.height - 16, 234, 0f, 0f, 0, default, 1f);
                    purpleElectricity.velocity *= 2f;
                    purpleElectricity.scale *= 1.15f;
                }
            }
            CalamityPlayer modPlayer = owner.Calamity();

            // Maintain or remove the Mechworm buff from the owner.
            owner.AddBuff(ModContent.BuffType<Mechworm>(), 3600);
            if (owner.dead)
                modPlayer.mWorm = false;
            if (modPlayer.mWorm)
                Projectile.timeLeft = 2;

            Time++;

            if (!Main.projectile.IndexInRange(EndRiftGateUUID))
            {
                // Very rapidly fade-in.
                Projectile.alpha = Utils.Clamp(Projectile.alpha - 42, 0, 255);
            }
            else if (Projectile.Hitbox.Intersects(Main.projectile[EndRiftGateUUID].Hitbox))
            {
                // Disappear if touching the mechworm portal.
                // It will look like it's teleporting, when in reality, it's
                // just an invisible, uninteractable projectile for the time being.

                if (Projectile.alpha != 255)
                {
                    for (int i = 0; i < 35; i++)
                    {
                        Dust burstDust = Dust.NewDustPerfect(Projectile.Center, (int)CalamityDusts.PurpleCosmilite);
                        burstDust.velocity = (MathHelper.TwoPi * i / 35f).ToRotationVector2().RotatedByRandom(0.035f) * 12f;
                        burstDust.noGravity = true;

                        burstDust = Dust.NewDustDirect(Projectile.Center, 70, 70, (int)CalamityDusts.PurpleCosmilite);
                        burstDust.velocity = Main.rand.NextVector2CircularEdge(6f, 6f);
                        burstDust.scale = Main.rand.NextFloat(1.3f, 1.75f);
                    }

                    SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
                    Projectile.alpha = 255;
                }
            }

            // Mechworm has an extremely generous default aggro range of 2200, but if it's already attacking, its bloodlust is insatiable.
            NPC potentialTarget = Projectile.Center.MinionHoming(AttackStateTimer > 0 ? 999999f : 2800f, owner);

            // Teleport to the player if the worm is very far away from them.
            if (Projectile.Distance(owner.Center) > 3850f)
            {
                Projectile.Center = owner.Center;
                // Reset the worm's velocity when it returns to the player so that it doesn't instantly yeet off somewhere.
                Projectile.velocity = Main.rand.NextVector2CircularEdge(3f, 3f);
                Projectile.netUpdate = true;
            }

            // Don't bother attacking if the target is close to the world edge, to prevent issues.
            if (potentialTarget != null && Time > StartupLethargy && TargetInSafeBoundaries(potentialTarget))
            {
                if (CurrentAttackState == AttackState.LaserCharge)
                    LaserAttackMovement(potentialTarget);
                else
                    PortalAttackMovement(potentialTarget);
                UpdateAttackStates();
                Projectile.extraUpdates = 1;
            }
            // Attacking movement can be canceled, so if it was, run the passive movement instead.
            else
                PlayerFollowMovement(owner);

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // Update the segment direction based on the velocity.
            int previousDirection = Projectile.direction;
            Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0f).ToDirectionInt();

            // If it changed for some reason, fire a net update. This update cannot be blocked by netSpam.
            if (previousDirection != Projectile.direction)
            {
                Projectile.netUpdate = true;
                if (Projectile.netSpam > 59)
                    Projectile.netSpam = 59;
            }
        }

        private static bool TargetInSafeBoundaries(NPC target) => target?.Center.Between(WorldTopLeft(), WorldBottomRight()) ?? true;

        private void PlayerFollowMovement(Player owner)
        {
            Projectile.extraUpdates = 0;

            // Reset the gate UUID from any previous teleports.
            if (EndRiftGateUUID != -1)
            {
                EndRiftGateUUID = -1;
                Projectile.netUpdate = true;
            }

            // If any attack was in use previously, send a net update now that attack mode is off.
            if (AttackStateTimer != 0)
            {
                AttackStateTimer = 0;
                Projectile.netUpdate = true;
            }

            float hoverAcceleration = 0.2f;
            float distanceFromOwner = Projectile.Distance(owner.Center);
            if (distanceFromOwner < 200f)
                hoverAcceleration = 0.12f;
            if (distanceFromOwner < 140f)
                hoverAcceleration = 0.06f;

            if (distanceFromOwner > 100f)
            {
                if (Math.Abs(owner.Center.X - Projectile.Center.X) > 20f)
                    Projectile.velocity.X += hoverAcceleration * Math.Sign(owner.Center.X - Projectile.Center.X);
                if (Math.Abs(owner.Center.Y - Projectile.Center.Y) > 10f)
                    Projectile.velocity.Y += hoverAcceleration * Math.Sign(owner.Center.Y - Projectile.Center.Y);
            }
            else if (Projectile.velocity.Length() > 1f)
                Projectile.velocity *= 0.96f;

            if (Math.Abs(Projectile.velocity.Y) < 1f)
                Projectile.velocity.Y -= 0.1f;

            // The worm's max speed is more strictly capped for the first few seconds.
            float maxSpeed = Time < StartupLethargy ? 6.5f : 25f;
            if (Projectile.velocity.Length() > maxSpeed)
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * maxSpeed;
        }

        private void LaserAttackMovement(NPC target)
        {
            // Reset the gate UUID from any previous teleports.
            if (EndRiftGateUUID != -1)
            {
                EndRiftGateUUID = -1;
                Projectile.netUpdate = true;
            }

            // If the timer indicates the worm is in redirect mode, then angle towards the target.
            if (AttackStateTimer % (LaserChargeFrames + LaserRedirectFrames) < LaserRedirectFrames)
            {
                float angularTurnSpeed = MathHelper.ToRadians(18f);
                float newSpeed = MathHelper.Lerp(Projectile.velocity.Length(), 24f, 0.35f);

                if (Projectile.Distance(target.Center) > 1100f)
                    newSpeed = MathHelper.Lerp(Projectile.velocity.Length(), 38f, 0.35f);

                Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(Projectile.AngleTo(target.Center), angularTurnSpeed).ToRotationVector2() * newSpeed;

                // If the worm is very close to aiming directly at the target, immediately switch from redirecting to charging.
                if (Vector2.Dot(Projectile.velocity.SafeNormalize(Vector2.Zero), Projectile.SafeDirectionTo(target.Center)) > 0.86f)
                {
                    AttackStateTimer += LaserRedirectFrames - (AttackStateTimer % LaserChargeFrames);
                    Projectile.netUpdate = true;
                }
            }

            // On the exact frame the worm enters charge mode, fire 3 lasers and send a net update.
            if (AttackStateTimer % (LaserChargeFrames + LaserRedirectFrames) == LaserChargeFrames)
            {
                // Charge and fire three lasers.
                Projectile.velocity = Projectile.SafeDirectionTo(target.Center) * MaxAttackFlySpeed;

                if (Main.myPlayer == Projectile.owner)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 perturbedSpeed = Projectile.velocity.RotatedBy(MathHelper.Lerp(-0.15f, 0.15f, i / 3f)) * 0.3f;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, perturbedSpeed, ModContent.ProjectileType<MechwormLaser>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }

                SoundEngine.PlaySound(SoundID.Item12, Projectile.Center);
                Projectile.netUpdate = true;
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
                Projectile.netUpdate = true;
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
                    TeleportEndingPoint = Projectile.Center + Projectile.velocity * chargeTime / 2f;

                // On the starting frame of a teleport, spawn portals.
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), TeleportStartingPoint, Vector2.Zero, ModContent.ProjectileType<MechwormTeleportRift>(), 0, 0f, Projectile.owner);
                    int endGateIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), TeleportEndingPoint, Vector2.Zero, ModContent.ProjectileType<MechwormTeleportRift>(), 0, 0f, Projectile.owner);
                    EndRiftGateUUID = Projectile.GetByUUID(Projectile.owner, endGateIndex);

                    Main.projectile[EndRiftGateUUID].ai[0] = chargeTime;
                    Main.projectile[EndRiftGateUUID].timeLeft = chargeTime;
                }

                // Dont teleport on the very first portal summon.
                if (AttackStateTimer != 0)
                    Projectile.Center = TeleportStartingPoint;

                // Reset the alpha and position across the entire worm for the next charge.
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile otherProj = Main.projectile[i];
                    if (!otherProj.active || otherProj.owner != Projectile.owner || i == Projectile.whoAmI)
                        continue;

                    if (otherProj.type == ModContent.ProjectileType<MechwormBody>() || otherProj.type == ModContent.ProjectileType<MechwormTail>())
                    {
                        otherProj.alpha = 0;
                        if (AttackStateTimer != 0)
                            otherProj.Center = Projectile.Center;
                        // There is no need to set the other projectiles to net update. They will do so when the head does.
                    }
                }

                Projectile.alpha = 0;
                Projectile.velocity = Projectile.SafeDirectionTo(TeleportEndingPoint) * (MaxAttackFlySpeed + target.velocity.Length() * 0.45f);
                Projectile.netUpdate = true;
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
                Projectile.netUpdate = true;
            }
        }

        private void CleanUpMechwormPortals()
        {
            int portalType = ModContent.ProjectileType<MechwormTeleportRift>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.type != portalType || !proj.active || proj.owner != Projectile.owner)
                    continue;

                proj.Kill();
                // Spawn a little bit of dust when the portals are destroyed.
                if (!Main.dedServ)
                    for (int j = 0; j < 16; j++)
                        Dust.NewDustDirect(proj.position, 45, 45, (int)CalamityDusts.PurpleCosmilite);
            }
        }
        #endregion

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180);

        #region Drawing

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            if (Projectile.alpha > 200)
                return;

            Vector2 origin = new Vector2(21f, 25f);
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/MechwormHeadGlow").Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, 1f, SpriteEffects.None, 0);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        #endregion
    }
}

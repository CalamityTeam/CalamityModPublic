using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Buffs.Summon;
using System.IO;
using Terraria.GameContent.Drawing;
using CalamityMod.Dusts;

namespace CalamityMod.Projectiles.Summon
{
    public class MoonFist : ModProjectile
    {
        public int DelayUntilNextPunch;

        public int FistIndex => (int)Projectile.ai[0];

        public ref float AttackTimer => ref Projectile.ai[1];

        public ref float FrameTimer => ref Projectile.localAI[0];

        public float FistInterpolant
        {
            get
            {
                float projectileCounts = Owner.ownedProjectileCounts[Type];

                // Use a midway interpolant if the projectile count is one. This makes the fist use middle positions instead of
                // sitting awkwardly to the left.
                if (projectileCounts <= 1f)
                    return 0.5f;

                return FistIndex / (projectileCounts - 1f);
            }
        }

        public Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Fist");
            Main.projFrames[Type] = 18;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.alpha = 255;
            Projectile.minionSlots = WarloksMoonFist.SlotCount;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.MaxUpdates * 10;
            Projectile.netImportant = true;
            Projectile.timeLeft = 90000;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(DelayUntilNextPunch);

        public override void ReceiveExtraAI(BinaryReader reader) => DelayUntilNextPunch = reader.ReadInt32();

        public override void AI()
        {
            ApplyMinionBuffs();

            NPC potentialTarget = Projectile.Center.MinionHoming(1650f, Owner);
            if (potentialTarget is null)
                HoverNearOwner();
            else
                AttackTarget(potentialTarget);

            FrameTimer++;
            Projectile.frameCounter++;
            if (DelayUntilNextPunch > 0)
                DelayUntilNextPunch--;

            // Fade in.
            Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity + 0.1f, 0f, 1f);
        }

        public void ApplyMinionBuffs()
        {
            // Maintain or remove the Mechworm buff from the owner.
            Owner.AddBuff(ModContent.BuffType<MoonFistBuff>(), 3600);
            if (Owner.dead)
                Owner.Calamity().MoonFist = false;
            if (Owner.Calamity().MoonFist)
                Projectile.timeLeft = 2;
        }

        public void HoverNearOwner()
        {
            float hoverOffsetAngle = MathHelper.Lerp(-MathHelper.PiOver2, MathHelper.PiOver2, FistInterpolant);
            Vector2 hoverDestination = Owner.Center - Vector2.UnitY.RotatedBy(hoverOffsetAngle) * (Owner.height + 70f);
            Projectile.velocity *= 0.7f;
            Projectile.Center = Vector2.Lerp(Projectile.Center, hoverDestination, 0.13333f);

            // Use idle frames.
            float frameInterpolant = (FrameTimer / 50f + FistInterpolant) % 1f;
            Projectile.frame = (int)Math.Round(MathHelper.Lerp(0f, 5f, frameInterpolant));

            // Determine direction and rotation.
            Projectile.rotation = Projectile.rotation.AngleTowards(0f, 0.333f);
            Projectile.spriteDirection = Owner.direction;

            // Reset the attack timer.
            AttackTimer = 0f;
        }

        public void AttackTarget(NPC target)
        {
            int reelBackTime = 32;
            int homeDelay = 27;
            int attackTime = 150;
            int attackCycleInterval = reelBackTime + attackTime;

            // Teleport to the enemy if it's far away.
            if (!Projectile.WithinRange(target.Center, 1200f))
            {
                Vector2 teleportDestination = target.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(250f, 300f);
                DoTeleport(teleportDestination);
            }

            Projectile.rotation = Projectile.AngleTo(target.Center);
            Projectile.spriteDirection = (target.Center.X < Projectile.Center.X).ToDirectionInt();
            if (Projectile.spriteDirection == -1)
                Projectile.rotation += MathHelper.Pi;

            // Clench the fist and hover near the target.
            if (AttackTimer <= reelBackTime)
            {
                Projectile.frame = (int)Math.Round(MathHelper.Lerp(6f, 11f, AttackTimer / reelBackTime));

                // Reel back and look at the target.
                float hoverOffset = MathHelper.Lerp(200f, 480f, AttackTimer / reelBackTime);
                hoverOffset += MathHelper.Lerp(-10f, 75f, Projectile.identity % 9f / 1f);
                float spin = MathHelper.Lerp(-0.27f, 0.27f, Projectile.identity / 7f % 1f);
                Vector2 hoverDestination = target.Center - target.SafeDirectionTo(target.Center, Vector2.UnitY).RotatedBy(spin) * hoverOffset;
                Vector2 idealVelocity = Projectile.SafeDirectionTo(hoverDestination) * MathHelper.Min(25f, Projectile.Distance(hoverDestination));

                Projectile.Center = Vector2.Lerp(Projectile.Center, hoverDestination, 0.02f);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, idealVelocity, 0.03f).MoveTowards(idealVelocity, 0.5f);

                // Fly towards the target at an incredibly fast speed.
                if (AttackTimer == reelBackTime)
                {
                    SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown, Projectile.Center);
                    Projectile.velocity = CalamityUtils.CalculatePredictiveAimToTarget(Projectile.Center, target, 40f);
                    Projectile.netUpdate = true;
                }
            }
            else if (DelayUntilNextPunch <= 0)
            {
                // Home towards the target briefly after the initial charge.
                if (AttackTimer >= reelBackTime + homeDelay)
                    Projectile.velocity = Projectile.SuperhomeTowardsTarget(target, 34f, 24f);
            }

            // Increment the attack timer.
            AttackTimer = (AttackTimer + 1f) % attackCycleInterval;
        }

        public void DoTeleport(Vector2 end)
        {
            Vector2 start = Projectile.Center;
            for (int i = 0; i < 75; i++)
            {
                Vector2 dustDrawPosition = Vector2.Lerp(start, end, i / 74f);

                Dust magic = Dust.NewDustPerfect(dustDrawPosition, 267);
                magic.velocity = -Vector2.UnitY * Main.rand.NextFloat(0.2f, 0.235f);
                magic.color = Color.LightCyan;
                magic.color.A = 0;
                magic.scale = 0.8f;
                magic.fadeIn = 1.4f;
                magic.noGravity = true;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            for (int i = 0; i < 6; i++)
            {
                int magic = Projectile.NewProjectile(Projectile.GetSource_FromAI(), start, Vector2.Zero, ModContent.ProjectileType<MoonFistTeleportVisual>(), 0, 0f);
                Main.projectile[magic].timeLeft -= i * 2;
                magic = Projectile.NewProjectile(Projectile.GetSource_FromAI(), end, Vector2.Zero, ModContent.ProjectileType<MoonFistTeleportVisual>(), 0, 0f);
                Main.projectile[magic].timeLeft -= i * 2;
            }
            Projectile.Center = end;
            Projectile.netUpdate = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Rebound on collision.
            if (DelayUntilNextPunch > 0 || AttackTimer <= 0f)
                return;

            // Create some cool particles to go with the hit.
            Vector2 impactPoint = Vector2.Lerp(Projectile.Center, target.Hitbox.ClosestPointInRect(Projectile.Center), 0.5f);
            for (int i = 0; i < 7; i++)
            {
                ParticleOrchestrator.RequestParticleSpawn(false, ParticleOrchestraType.StardustPunch, new()
                {
                    PositionInWorld = impactPoint + Vector2.UnitY * Main.rand.NextFloatDirection() * 10f,
                    MovementVector = Main.rand.NextVector2Unit() * Main.rand.NextFloat(3f, 9.6f)
                });
            }
            for (int i = 0; i < 15; i++)
            {
                int dustID = Main.rand.NextBool() ? (int)CalamityDusts.BlueCosmilite : (int)CalamityDusts.PurpleCosmilite;

                Dust cosmicDust = Dust.NewDustPerfect(impactPoint + Main.rand.NextVector2Circular(10f, 10f), dustID);
                cosmicDust.velocity = Projectile.velocity.RotatedByRandom(0.6f) * 0.2f;
                cosmicDust.scale = Main.rand.NextFloat(1f, 1.4f);
                cosmicDust.noGravity = true;
                if (Main.rand.NextBool(5))
                    cosmicDust.scale += 0.45f;
            }

            SoundEngine.PlaySound(SoundID.Item74 with { Pitch = -0.36f }, Projectile.Center);
            DelayUntilNextPunch = WarloksMoonFist.PunchCooldownTime - 10;
            Projectile.velocity = Projectile.SafeDirectionTo(target.Center, -Vector2.UnitY).RotatedBy(0.16f) * Projectile.velocity.Length() * -0.45f;
            Projectile.netUpdate = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (AttackTimer != 0)
                direction |= SpriteEffects.FlipVertically;
            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, direction, 0);
            return false;
        }
    }
}

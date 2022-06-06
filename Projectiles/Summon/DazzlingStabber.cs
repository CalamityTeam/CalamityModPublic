using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class DazzlingStabber : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];
        public ref float AttackDelay => ref Projectile.ai[0];
        public ref float RestOffsetAngle => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dazzling Stabber");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 25;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 58;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 90000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.light = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3());

            ApplyPlayerBuffs();
            UpdateFrames();

            NPC potentialTarget = Projectile.Center.MinionHoming(1100f, Owner);

            if (potentialTarget != null)
                SliceTarget(potentialTarget);
            else
                ReturnToRestingPosition();
        }

        public void ApplyPlayerBuffs()
        {
            Owner.AddBuff(ModContent.BuffType<DazzlingStabberBuff>(), 3600);
            if (Projectile.type == ModContent.ProjectileType<DazzlingStabber>())
            {
                if (Owner.dead)
                    Owner.Calamity().providenceStabber = false;
                if (Owner.Calamity().providenceStabber)
                    Projectile.timeLeft = 2;
            }
        }

        public void UpdateFrames()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 6 == 0)
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
        }

        public void SliceTarget(NPC target)
        {
            // Don't do anything if the attack delay is still passing.
            if (AttackDelay > -60f)
            {
                AttackDelay--;
                if (AttackDelay > 0f)
                    return;
            }

            // Reset the velocity to fly upward if there is very little motion to ensure
            // that the summon does not get stuck.
            if (Projectile.velocity.Length() < 3f)
                Projectile.velocity = Vector2.UnitY * -12f;

            // If close to the target, slow down dramatically.
            if (Projectile.velocity.Length() > 5f && Projectile.WithinRange(target.Center, 90f))
                Projectile.velocity *= 0.93f;

            // Otherwise, if not close, speed up dramatically.
            else if (Projectile.velocity.Length() < 40f)
                Projectile.velocity *= 1.03f;

            float angularTurnSpeed = 0.35f;
            float angleToTargetCoords = Projectile.AngleTo(target.Center);

            if (!Projectile.WithinRange(target.Center, 200f))
                Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(angleToTargetCoords, angularTurnSpeed).ToRotationVector2() * Projectile.velocity.Length();

            // If not super close to the target but the target is very much in the line of sight of the summon, charge.
            if (!Projectile.WithinRange(target.Center, 75f) && Vector2.Dot(Projectile.SafeDirectionTo(target.Center), Projectile.velocity.SafeNormalize(Vector2.Zero)) > 0.85f)
            {
                Projectile.velocity = Projectile.SafeDirectionTo(target.Center) * 36f;
                AttackDelay = 15f;

                Projectile.netUpdate = true;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public void ReturnToRestingPosition()
        {
            Projectile.rotation = Projectile.rotation.AngleTowards(RestOffsetAngle, 0.25f);

            // Rapidly approach the resting position.
            Vector2 destination = Owner.Center + Vector2.UnitY.RotatedBy(RestOffsetAngle) * -120f;
            Projectile.velocity = (destination - Projectile.Center) / 10f;
        }

        public override void PostDraw(Color lightColor)
        {
            for (int i = 1; i < Projectile.oldPos.Length; i++)
                CalamityUtils.DistanceClamp(ref Projectile.oldPos[i - 1], ref Projectile.oldPos[i], 6f);

            Texture2D trailSegmentTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/StarProj").Value;
            for (int i = 2; i < Projectile.oldPos.Length; i++)
            {
                float completionRatio = i / (float)Projectile.oldPos.Length;
                float rotation = (Projectile.oldPos[i - 1] - Projectile.oldPos[i]).ToRotation() + MathHelper.PiOver2;
                float scale = MathHelper.Lerp(0.7f, 0.1f, completionRatio) * Projectile.scale;
                Color color = Color.Lerp(Color.LightPink, Color.Goldenrod, completionRatio * 3f % 1f) * 1.5f;

                // Become dimmer the slower the projectile is moving.
                color *= Utils.GetLerpValue(1f, 8f, Projectile.velocity.Length(), true);

                // As well as how close the projectile is to pointing away from its rotation.
                color *= Utils.GetLerpValue(0.65f, 0.45f, Projectile.velocity.AngleBetween(-(Projectile.rotation + MathHelper.PiOver2).ToRotationVector2()) / MathHelper.Pi, true);

                Main.EntitySpriteDraw(trailSegmentTexture,
                                 Projectile.oldPos[i] + Projectile.Size * 0.5f + new Vector2(0f, 8f).RotatedBy(Projectile.rotation) - Main.screenPosition,
                                 null,
                                 color,
                                 rotation,
                                 trailSegmentTexture.Size() * 0.5f,
                                 scale,
                                 SpriteEffects.None,
                                 0);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }
    }
}

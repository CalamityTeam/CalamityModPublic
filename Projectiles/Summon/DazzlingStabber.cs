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
        public Player Owner => Main.player[projectile.owner];
        public ref float AttackDelay => ref projectile.ai[0];
        public ref float RestOffsetAngle => ref projectile.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dazzling Stabber");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 25;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 58;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 90000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.minion = true;
            projectile.light = 1f;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, Color.White.ToVector3());

            ApplyPlayerBuffs();
            AdjustDamageDynamically();
            UpdateFrames();

            NPC potentialTarget = projectile.Center.MinionHoming(1100f, Owner);

            if (potentialTarget != null)
                SliceTarget(potentialTarget);
            else
                ReturnToRestingPosition();
        }

        public void ApplyPlayerBuffs()
        {
            Owner.AddBuff(ModContent.BuffType<DazzlingStabberBuff>(), 3600);
            if (projectile.type == ModContent.ProjectileType<DazzlingStabber>())
            {
                if (Owner.dead)
                    Owner.Calamity().providenceStabber = false;
                if (Owner.Calamity().providenceStabber)
                    projectile.timeLeft = 2;
            }
        }

        public void AdjustDamageDynamically()
        {
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = Owner.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] = 1f;
            }
            if (Owner.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue / projectile.Calamity().spawnedPlayerMinionDamageValue * Owner.MinionDamage());
                projectile.damage = trueDamage;
            }
        }

        public void UpdateFrames()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter % 6 == 0)
                projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
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

            // Don't do anything if very close to the target.
            if (projectile.WithinRange(target.Center, 60f))
                return;

            // Reset the velocity to fly upward if there is very little motion to ensure
            // that the summon does not get stuck.
            if (projectile.velocity.Length() < 1.6f)
                projectile.velocity = Vector2.UnitY * 5f;

            // If close to the target, slow down dramatically.
            if (projectile.velocity.Length() > 5f && projectile.WithinRange(target.Center, 90f))
                projectile.velocity *= 0.8f;

            // Otherwise, if not close, speed up dramatically.
            else if (projectile.velocity.Length() < 40f)
                projectile.velocity *= 1.03f;

            float angularTurnSpeed = 0.35f;
            float angleToTargetCoords = projectile.AngleTo(target.Center);

            projectile.velocity = projectile.velocity.ToRotation().AngleTowards(angleToTargetCoords, angularTurnSpeed).ToRotationVector2() * projectile.velocity.Length();

            // If not super close to the target but the target is very much in the line of sight of the summon, charge.
            if (!projectile.WithinRange(target.Center, 75f) && Vector2.Dot(projectile.SafeDirectionTo(target.Center), projectile.velocity.SafeNormalize(Vector2.Zero)) > 0.85f)
            {
                projectile.velocity = projectile.SafeDirectionTo(target.Center) * 36f;
                AttackDelay = 15f;

                projectile.netUpdate = true;
            }

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public void ReturnToRestingPosition()
        {
            projectile.rotation = projectile.rotation.AngleTowards(RestOffsetAngle, 0.25f);

            // Rapidly approach the resting position.
            Vector2 destination = Owner.Center + Vector2.UnitY.RotatedBy(RestOffsetAngle) * -120f;
            projectile.velocity = (destination - projectile.Center) / 10f;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            for (int i = 1; i < projectile.oldPos.Length; i++)
                CalamityUtils.DistanceClamp(ref projectile.oldPos[i - 1], ref projectile.oldPos[i], 6f);

            Texture2D trailSegmentTexture = ModContent.GetTexture("CalamityMod/Projectiles/StarProj");
            for (int i = 2; i < projectile.oldPos.Length; i++)
            {
                float completionRatio = i / (float)projectile.oldPos.Length;
                float rotation = (projectile.oldPos[i - 1] - projectile.oldPos[i]).ToRotation() + MathHelper.PiOver2;
                float scale = MathHelper.Lerp(0.7f, 0.1f, completionRatio) * projectile.scale;
                Color color = Color.Lerp(Color.LightPink, Color.Goldenrod, completionRatio * 3f % 1f) * 1.5f;

                // Become dimmer the slower the projectile is moving.
                color *= Utils.InverseLerp(1f, 8f, projectile.velocity.Length(), true);

                // As well as how close the projectile is to pointing away from its rotation.
                color *= Utils.InverseLerp(0.65f, 0.45f, projectile.velocity.AngleBetween(-(projectile.rotation + MathHelper.PiOver2).ToRotationVector2()) / MathHelper.Pi, true);

                spriteBatch.Draw(trailSegmentTexture,
                                 projectile.oldPos[i] + projectile.Size * 0.5f + new Vector2(0f, 8f).RotatedBy(projectile.rotation) - Main.screenPosition,
                                 null,
                                 color,
                                 rotation,
                                 trailSegmentTexture.Size() * 0.5f,
                                 scale,
                                 SpriteEffects.None,
                                 0f);
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

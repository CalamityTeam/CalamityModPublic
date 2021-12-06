using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class PrinceFlameLarge : ModProjectile
    {
        public ref float Time => ref projectile.ai[0];
        public const int Lifetime = 60;
        public const int FadeoutTime = 25;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Fireball");
            Main.projFrames[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 40;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = Lifetime;
            projectile.penetrate = 4;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 11;
            projectile.magic = true;
        }

        public override void AI()
        {
            // Create rose petals.
            if (projectile.localAI[0] == 0f)
            {
                for (int i = 0; i < 10; i++)
                {
                    Dust rose = Dust.NewDustPerfect(projectile.Center, ModContent.DustType<RosePiece>());
                    rose.velocity += projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.61f) * 2.5f;
                    rose.velocity.Y += Main.rand.NextFloat(-2.4f, 1.6f);
                    rose.velocity *= 0.4f;
                    rose.scale = Main.rand.NextFloat(1.2f, 1.7f);
                    rose.noGravity = Main.rand.NextBool();
                }
                projectile.localAI[0] = 1f;
            }

            // Explode before dissipating.
            if (projectile.timeLeft == FadeoutTime)
                ExplodeIntoFireballs();

            bool dissipating = projectile.timeLeft < FadeoutTime;

            for (int i = 0; i < (dissipating ? 2 : 1); i++)
            {
                Dust fire = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, (int)CalamityDusts.ProfanedFire);
                fire.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f, 6f);
                fire.scale *= Main.rand.NextFloat(1.15f, 1.7f);
                fire.noGravity = Main.rand.NextBool();
            }

            // Dissipate at the end of the projectile's lifetime.
            if (dissipating)
            {
                projectile.frame = (int)Math.Round(MathHelper.Lerp(4f, 7f, Utils.InverseLerp(FadeoutTime, 0f, projectile.timeLeft, true)));
                projectile.velocity *= 0.95f;
                return;
            }

            // Create bursts of fire dust.
            if (Time % 8f == 7f)
            {
                for (int i = 0; i < 12; i++)
                {
                    Vector2 dustSpawnOffset = Vector2.UnitX * -projectile.width / 2f;
                    dustSpawnOffset += -Vector2.UnitY.RotatedBy(i * MathHelper.TwoPi / 12f) * new Vector2(8f, 16f);
                    dustSpawnOffset = dustSpawnOffset.RotatedBy(projectile.rotation - MathHelper.PiOver2);

                    Dust holyFire = Dust.NewDustDirect(projectile.Center, 0, 0, (int)CalamityDusts.ProfanedFire, 0f, 0f, 160, default, 1f);
                    holyFire.scale = 1.1f;
                    holyFire.noGravity = true;
                    holyFire.position = projectile.Center + dustSpawnOffset;
                    holyFire.velocity = projectile.velocity * 0.1f;
                    holyFire.velocity = (projectile.Center - projectile.velocity * 3f - holyFire.position).SafeNormalize(Vector2.Zero) * 1.25f;
                }
            }

            Time++;
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
            projectile.frameCounter++;
            projectile.frame = projectile.frameCounter / 5 % 4;
        }

        public void ExplodeIntoFireballs()
        {
            // Play a fizzle sound.
            Main.PlaySound(SoundID.DD2_KoboldIgnite, projectile.Center);
            if (Main.myPlayer != projectile.owner)
                return;

            // And explode into a burst of fire.
            int damage = (int)(projectile.damage * 0.66f);
            float kb = projectile.knockBack * 0.4f;
            float offsetAngle = Main.rand.NextFloatDirection() * 0.31f;
            for (int i = 0; i < ThePrince.FlameSplitCount; i++)
            {
                Vector2 shootVelocity = (MathHelper.TwoPi * i / ThePrince.FlameSplitCount + offsetAngle).ToRotationVector2() * 8f;
                Vector2 flameSpawnPosition = projectile.Center + shootVelocity;
                Projectile.NewProjectile(flameSpawnPosition, shootVelocity, ModContent.ProjectileType<PrinceFlameSmall>(), damage, kb, projectile.owner);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            lightColor = Color.Lerp(lightColor, Color.White, 0.8f);
            lightColor.A /= 4;
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (timeLeft > FadeoutTime)
                ExplodeIntoFireballs();

            for (int i = 0; i < 30; i++)
            {
                Dust fire = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, (int)CalamityDusts.ProfanedFire);
                fire.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(3f, 8f);
                fire.position += fire.velocity.RotatedBy(MathHelper.PiOver2) * 2f;
                fire.scale *= Main.rand.NextFloat(1.15f, 1.7f);
                fire.noGravity = true;
            }
        }
    }
}

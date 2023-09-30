using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class PrinceFlameLarge : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public ref float Time => ref Projectile.ai[0];
        public const int Lifetime = 60;
        public const int FadeoutTime = 25;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = Lifetime;
            Projectile.penetrate = 4;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 11;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            // Create rose petals.
            if (Projectile.localAI[0] == 0f)
            {
                for (int i = 0; i < 10; i++)
                {
                    Dust rose = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<RosePiece>());
                    rose.velocity += Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.61f) * 2.5f;
                    rose.velocity.Y += Main.rand.NextFloat(-2.4f, 1.6f);
                    rose.velocity *= 0.4f;
                    rose.scale = Main.rand.NextFloat(1.2f, 1.7f);
                    rose.noGravity = Main.rand.NextBool();
                }
                Projectile.localAI[0] = 1f;
            }

            // Explode before dissipating.
            if (Projectile.timeLeft == FadeoutTime)
                ExplodeIntoFireballs();

            bool dissipating = Projectile.timeLeft < FadeoutTime;

            for (int i = 0; i < (dissipating ? 2 : 1); i++)
            {
                Dust fire = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.ProfanedFire);
                fire.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f, 6f);
                fire.scale *= Main.rand.NextFloat(1.15f, 1.7f);
                fire.noGravity = Main.rand.NextBool();
            }

            // Dissipate at the end of the projectile's lifetime.
            if (dissipating)
            {
                Projectile.frame = (int)Math.Round(MathHelper.Lerp(4f, 7f, Utils.GetLerpValue(FadeoutTime, 0f, Projectile.timeLeft, true)));
                Projectile.velocity *= 0.95f;
                return;
            }

            // Create bursts of fire dust.
            if (Time % 8f == 7f)
            {
                for (int i = 0; i < 12; i++)
                {
                    Vector2 dustSpawnOffset = Vector2.UnitX * -Projectile.width / 2f;
                    dustSpawnOffset += -Vector2.UnitY.RotatedBy(i * MathHelper.TwoPi / 12f) * new Vector2(8f, 16f);
                    dustSpawnOffset = dustSpawnOffset.RotatedBy(Projectile.rotation - MathHelper.PiOver2);

                    Dust holyFire = Dust.NewDustDirect(Projectile.Center, 0, 0, (int)CalamityDusts.ProfanedFire, 0f, 0f, 160, default, 1f);
                    holyFire.scale = 1.1f;
                    holyFire.noGravity = true;
                    holyFire.position = Projectile.Center + dustSpawnOffset;
                    holyFire.velocity = Projectile.velocity * 0.1f;
                    holyFire.velocity = (Projectile.Center - Projectile.velocity * 3f - holyFire.position).SafeNormalize(Vector2.Zero) * 1.25f;
                }
            }

            Time++;
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 5 % 4;
        }

        public void ExplodeIntoFireballs()
        {
            // Play a fizzle sound.
            SoundEngine.PlaySound(SoundID.DD2_KoboldIgnite, Projectile.Center);
            if (Main.myPlayer != Projectile.owner)
                return;

            // And explode into a burst of fire.
            int damage = (int)(Projectile.damage * 0.66f);
            float kb = Projectile.knockBack * 0.4f;
            float offsetAngle = Main.rand.NextFloatDirection() * 0.31f;
            for (int i = 0; i < ThePrince.FlameSplitCount; i++)
            {
                Vector2 shootVelocity = (MathHelper.TwoPi * i / ThePrince.FlameSplitCount + offsetAngle).ToRotationVector2() * 8f;
                Vector2 flameSpawnPosition = Projectile.Center + shootVelocity;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), flameSpawnPosition, shootVelocity, ModContent.ProjectileType<PrinceFlameSmall>(), damage, kb, Projectile.owner);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.Lerp(lightColor, Color.White, 0.8f);
            lightColor.A /= 4;
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (timeLeft > FadeoutTime)
                ExplodeIntoFireballs();

            for (int i = 0; i < 30; i++)
            {
                Dust fire = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.ProfanedFire);
                fire.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(3f, 8f);
                fire.position += fire.velocity.RotatedBy(MathHelper.PiOver2) * 2f;
                fire.scale *= Main.rand.NextFloat(1.15f, 1.7f);
                fire.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }
    }
}

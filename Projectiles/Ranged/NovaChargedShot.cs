using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class NovaChargedShot : ModProjectile, ILocalizedModType
    {
        public static readonly SoundStyle ChargeImpact = new("CalamityMod/Sounds/Item/ArcNovaDiffuserChargeImpact") { Volume = 0.3f };
        public int Time = 0;
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 20;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 1200;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
            Projectile.extraUpdates = 24;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Time++;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(-3, 3), 107);
            dust.noGravity = true;
            dust.scale = 1.2f;
            if (Time < 120)
            {
                if (Main.rand.NextBool())
                {
                    Vector2 trailPos = Projectile.Center + Main.rand.NextVector2Circular(10, 10);
                    float trailScale = Main.rand.NextFloat(0.8f, 1.2f);
                    Color trailColor = Main.rand.NextBool(3) ? Color.Chartreuse : Color.Lime;
                    Particle Trail = new SparkParticle(trailPos, Projectile.velocity * 0.2f, false, 60, trailScale, trailColor);
                    GeneralParticleHandler.SpawnParticle(Trail);
                }
            }
            

        }
        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(50, 255, 50, Projectile.alpha);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, 900);
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i <= 19; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 107, new Vector2(0, -18).RotatedByRandom(MathHelper.ToRadians(35f)) * Main.rand.NextFloat(0.1f, 1.9f));
                dust.noGravity = false;
                dust.scale = Main.rand.NextFloat(0.8f, 1.5f);
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center, 107, new Vector2(0, -7).RotatedByRandom(MathHelper.ToRadians(35f)) * Main.rand.NextFloat(0.1f, 1.9f));
                dust2.noGravity = false;
                dust2.scale = Main.rand.NextFloat(0.8f, 1.5f);
            }
            SoundEngine.PlaySound(ChargeImpact, Projectile.position);
            Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Color.Lime, new Vector2(2f, 2f), Main.rand.NextFloat(12f, 25f), 0.1f, 0.6f, 20);
            GeneralParticleHandler.SpawnParticle(pulse);
            Particle pulse2 = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Color.Chartreuse, new Vector2(2f, 2f), Main.rand.NextFloat(12f, 25f), 0.1f, 0.5f, 16);
            GeneralParticleHandler.SpawnParticle(pulse2);

            for (int i = 0; i < 25; ++i)
            {
                int bloodLifetime = Main.rand.Next(22, 36);
                float bloodScale = Main.rand.NextFloat(0.6f, 0.8f);
                Color bloodColor = Color.Lerp(Color.Lime, Color.Chartreuse, Main.rand.NextFloat());
                bloodColor = Color.Lerp(bloodColor, new Color(51, 22, 94), Main.rand.NextFloat(0.65f));

                if (Main.rand.NextBool(20))
                    bloodScale *= 2f;

                float randomSpeedMultiplier = Main.rand.NextFloat(1.25f, 2.25f);
                Vector2 bloodVelocity = Main.rand.NextVector2Unit() * 5 * randomSpeedMultiplier;
                bloodVelocity.Y -= 5f;
                BloodParticle blood = new BloodParticle(Projectile.Center, bloodVelocity, bloodLifetime, bloodScale, bloodColor);
                GeneralParticleHandler.SpawnParticle(blood);
            }
        }
        public override bool? CanDamage() => base.CanDamage();
    }
}

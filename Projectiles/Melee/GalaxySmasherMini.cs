using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Particles;

namespace CalamityMod.Projectiles.Melee
{
    public class GalaxySmasherMini : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/Melee/GalaxySmasherMini";
        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/Item/WulfrumKnifeTileHit2") { Volume = 0.5f, PitchVariance = 0.7f };
        public float rotatehammer = 20f;

        public override void SetDefaults()
        {
            Projectile.width = 86;
            Projectile.height = 72;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.alpha = 80;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.scale = 0.7f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        public override void AI()
        {
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
            Projectile.rotation += MathHelper.ToRadians(rotatehammer) * Projectile.direction;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(HitSound, Projectile.Center);
            float damageInterpolant = Utils.GetLerpValue(950f, 2000f, hit.Damage, true);
            float impactAngularVelocity = MathHelper.Lerp(0.08f, 0.2f, damageInterpolant);
            float impactParticleScale = MathHelper.Lerp(0.6f, 1f, damageInterpolant);
            impactAngularVelocity *= Main.rand.NextBool().ToDirectionInt() * Main.rand.NextFloat(0.75f, 1.25f);

            Color impactColor = Color.Lerp(Color.Fuchsia, Color.Aqua, Main.rand.NextFloat(0.5f));
            Vector2 impactPoint = Vector2.Lerp(Projectile.Center, target.Center, 0.65f);
            Vector2 bloodSpawnPosition = target.Center + Main.rand.NextVector2Circular(target.width, target.height) * 0.04f;
            Vector2 splatterDirection = (Projectile.Center - bloodSpawnPosition).SafeNormalize(Vector2.UnitY);
            for (int i = 0; i < 3; i++)
            {
                int sparkLifetime = Main.rand.Next(9, 12);
                float sparkScale = Main.rand.NextFloat(0.8f, 1f) + damageInterpolant * 0.85f;
                Color sparkColor = Color.Lerp(Color.Fuchsia, Color.Aqua, Main.rand.NextFloat(0.7f));
                sparkColor = Color.Lerp(sparkColor, Color.Fuchsia, Main.rand.NextFloat());
                Vector2 sparkVelocity = splatterDirection.RotatedByRandom(0.6f) * Main.rand.NextFloat(12f, 25f);
                sparkVelocity.Y -= 6f;
                SparkParticle spark = new SparkParticle(impactPoint, sparkVelocity, true, sparkLifetime, sparkScale, sparkColor);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            //Create an impact point particle.
            ImpactParticle impactParticle = new ImpactParticle(impactPoint, impactAngularVelocity, 20, impactParticleScale, impactColor);
            GeneralParticleHandler.SpawnParticle(impactParticle);
        }
    }
}

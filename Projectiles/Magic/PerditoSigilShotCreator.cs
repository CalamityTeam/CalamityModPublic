using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class PerditoSigilShotCreator : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public ref float FiringTimer => ref Projectile.ai[0];
        public ref float ShotsFiredCount => ref Projectile.ai[1];

        private const int TotalShots = 18;
        private const int DelayBetweenShots = 3;
        private const float ShotSpeed = 17f;
        private const float SpawnRadius = 150f; // Radius around the cursor where shots spawn

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = DelayBetweenShots * TotalShots + 10;
        }

        public override void AI()
        {
            Projectile.Center = Main.MouseWorld;
            Vector2 targetCenter = Projectile.Center;

            FiringTimer++;

            if (FiringTimer % DelayBetweenShots == 0 && ShotsFiredCount < TotalShots)
            {
                ShotsFiredCount++;
                if (ShotsFiredCount == TotalShots)
                {
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/UnstableCastersGauntlet/PerditoSigilHit2") { Volume = 0.9f, PitchVariance = 0.1f }, Projectile.Center);
                }
                else
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/UnstableCastersGauntlet/PerditoSigilHit1") { Volume = 0.8f, PitchVariance = 0.1f }, Projectile.Center);

                float startAngle = Main.rand.NextFloat(MathHelper.TwoPi);
                Vector2 spawnOffset = startAngle.ToRotationVector2() * SpawnRadius;
                Vector2 spawnPosition = targetCenter + spawnOffset;

                Vector2 velocity = (targetCenter - spawnPosition).SafeNormalize(Vector2.UnitX) * ShotSpeed;

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition, velocity, ModContent.ProjectileType<PerditoSigilShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            if (ShotsFiredCount >= TotalShots)
            {
                Projectile.Kill();
            }
        }

        public override bool? CanDamage() => false;
    }
}

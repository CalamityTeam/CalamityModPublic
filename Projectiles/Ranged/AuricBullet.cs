using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityMod.Projectiles.Ranged
{
    public class AuricBullet : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public int Heat = 2;
        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/Item/AuricBulletHit") { Volume = 0.7f };
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 15;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
            Projectile.tileCollide = false;
            Projectile.ArmorPenetration = 50;
            Projectile.alpha = 255;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesFromEdge(Projectile, 0, lightColor);
            return false;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);

            Vector3 DustLight = new Vector3(0.255f, 0.230f, 0.000f);
            Lighting.AddLight(Projectile.Center, DustLight * 2);

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            Projectile.spriteDirection = Projectile.direction;
            if (Projectile.timeLeft < 595)
                CalamityUtils.HomeInOnNPC(Projectile, true, 250f, 7.5f, 10f);
            if (targetDist < 1400f && Projectile.timeLeft < 599 && Projectile.timeLeft % 2 == 0)
            {
                if (Projectile.timeLeft < 585)
                    Heat = 5;
                if (Projectile.timeLeft < 580)
                    Heat = 10;
                if (Projectile.timeLeft < 575)
                    Heat = 20;
                int positionVariation = Projectile.timeLeft < 590 ? 17 : 7;
                LineParticle spark = new LineParticle(Projectile.Center + Main.rand.NextVector2Circular(positionVariation, positionVariation), -Projectile.velocity * Main.rand.NextFloat(0.3f, 1.1f), false, 4, 1.45f, Main.rand.NextBool(Heat) ? Projectile.timeLeft < 570 ? Color.Goldenrod : Color.OrangeRed : Projectile.timeLeft > 590 ? Color.Red : Color.DarkGoldenrod);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            if (Main.rand.NextBool(7))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10, 10), Main.rand.NextBool() ? 311 : 292, -Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.1f, 0.6f));
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.7f, 1.1f);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 300);
            SoundEngine.PlaySound(HitSound with { PitchVariance = 0.15f}, Projectile.Center);
            GenericSparkle sparker = new GenericSparkle(Projectile.Center, Vector2.Zero, Color.Gold, Color.Cyan, Main.rand.NextFloat(1.8f, 2.5f), 5, Main.rand.NextFloat(-0.01f, 0.01f), 1.68f);
            GeneralParticleHandler.SpawnParticle(sparker);

            Vector2 bloodSpawnPosition = target.Center + Main.rand.NextVector2Circular(target.width, target.height) * 0.04f;
            Vector2 splatterDirection = (Projectile.Center - bloodSpawnPosition).SafeNormalize(Vector2.UnitY);

            int sparkLifetime = Main.rand.Next(9, 12);
            float sparkScale = Main.rand.NextFloat(0.9f, 1.3f) * 0.85f;
            Color sparkColor = Color.Lerp(Color.DarkGoldenrod, Color.Gold, Main.rand.NextFloat(0.7f));
            Vector2 sparkVelocity = splatterDirection.RotatedByRandom(0.6f) * Main.rand.NextFloat(22f, 45f);
            sparkVelocity.Y -= 6f;
            if (Main.rand.NextBool())
            {
                SparkParticle spark = new SparkParticle(target.Center, sparkVelocity, false, sparkLifetime, sparkScale, sparkColor);
                GeneralParticleHandler.SpawnParticle(spark);
            }

            for (int i = 0; i <= 6; i++)
            {
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center, 226, new Vector2(2, 2).RotatedByRandom(100f) * Main.rand.NextFloat(0.1f, 2.9f));
                dust2.noGravity = false;
                dust2.scale = Main.rand.NextFloat(0.3f, 0.9f);
            }
        }
    }
}

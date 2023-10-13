using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using CalamityMod.Items.Weapons.Ranged;

namespace CalamityMod.Projectiles.Ranged
{
    public class HalleysComet : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public int Time = 0;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 5;
            Projectile.extraUpdates = 10;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Time++;
            if (Projectile.scale <= 1.5f)
            {
                Projectile.scale *= 1.01f;
            }
            if (Time > 6 && Time < 180)
            {
                SparkParticle spark = new SparkParticle(Projectile.Center, Projectile.velocity, false, 11, 1.9f, Color.MediumBlue);
                GeneralParticleHandler.SpawnParticle(spark);
                SparkParticle spark2 = new SparkParticle(Projectile.Center, Projectile.velocity, false, 11, 1f, Color.Aqua);
                GeneralParticleHandler.SpawnParticle(spark2);
            }
            if (Time == 5)
            {
                Particle pulse = new DirectionalPulseRing(Projectile.Center, Projectile.velocity * 0.75f, Color.Aqua, new Vector2(1f, 2.5f), Projectile.velocity.ToRotation(), 0.4f, 0.05f, 20);
                GeneralParticleHandler.SpawnParticle(pulse);
                Particle pulse2 = new DirectionalPulseRing(Projectile.Center, Projectile.velocity * 0.4f, Color.DodgerBlue, new Vector2(1f, 2.5f), Projectile.velocity.ToRotation(), 0.2f, 0.05f, 35);
                GeneralParticleHandler.SpawnParticle(pulse2);
                for (int i = 0; i <= 25; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 172 : 206, Projectile.velocity);
                    dust.scale = Main.rand.NextFloat(1.5f, 2.3f);
                    dust.velocity = Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.3f, 2.1f);
                    dust.noGravity = true;
                }
            }

            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.35f / 255f, (255 - Projectile.alpha) * 0f / 255f, (255 - Projectile.alpha) * 0.45f / 255f);

            Projectile.rotation += 0.3f * (float)Projectile.direction;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 450);
            SoundEngine.PlaySound(HalleysInferno.Hit, Projectile.Center);
            for (int i = 0; i <= 13; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 172 : 206, Projectile.velocity);
                dust.scale = Main.rand.NextFloat(1.1f, 1.9f);
                dust.velocity = Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.2f, 2.1f);
                dust.noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 450);
        }
    }
}

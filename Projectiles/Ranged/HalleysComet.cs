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

        public ref float Time => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 5;
            Projectile.MaxUpdates = 10;
            Projectile.timeLeft = 15 * Projectile.MaxUpdates;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Time++;
            if (Time > 6f)
            {
                SparkParticle spark = new SparkParticle(Projectile.Center, Projectile.velocity, false, 11, 1.9f, Color.MediumBlue);
                GeneralParticleHandler.SpawnParticle(spark);
                SparkParticle spark2 = new SparkParticle(Projectile.Center, Projectile.velocity, false, 11, 1f, Color.Aqua);
                GeneralParticleHandler.SpawnParticle(spark2);
            }
            else if (Time == 5f)
            {
                Particle pulse = new DirectionalPulseRing(Projectile.Center, Projectile.velocity * 0.75f, Color.Aqua, new Vector2(1f, 2.5f), Projectile.rotation, 0.2f, 0.03f, 20);
                GeneralParticleHandler.SpawnParticle(pulse);
                Particle pulse2 = new DirectionalPulseRing(Projectile.Center, Projectile.velocity * 0.4f, Color.DodgerBlue, new Vector2(1f, 2.5f), Projectile.rotation, 0.1f, 0.025f, 35);
                GeneralParticleHandler.SpawnParticle(pulse2);
                for (int i = 0; i <= 25; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 172 : 206, Projectile.velocity);
                    dust.scale = Main.rand.NextFloat(1.6f, 2.5f);
                    dust.velocity = Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.3f, 1.6f);
                    dust.noGravity = true;
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 172 : 206, Projectile.velocity);
                    dust2.scale = Main.rand.NextFloat(1.35f, 2.1f);
                    dust2.velocity = Projectile.velocity.RotatedByRandom(0.06f) * Main.rand.NextFloat(0.8f, 3.1f);
                    dust2.noGravity = true;
                }
            }

            Lighting.AddLight(Projectile.Center, Color.MediumBlue.ToVector3() * 0.4f);
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

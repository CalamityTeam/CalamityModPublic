using System;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class DeadSunExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public ref float ExplosionRadius => ref Projectile.ai[0];
        public Color color1 = Color.LightGreen;
        public Color color2 = Color.Black;
        public override void SetDefaults()
        {
            //These shouldn't matter because its circular
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        public override void AI()
        {
            Particle explosion = new DetailedExplosion(Projectile.Center, Vector2.Zero, color1, Vector2.One, Main.rand.NextFloat(-5, 5), 0f, ExplosionRadius * 0.0065f + 0.1f, Main.rand.Next(15, 22));
            GeneralParticleHandler.SpawnParticle(explosion);
            Particle explosion2 = new DetailedExplosion(Projectile.Center, Vector2.Zero, Color.Black, Vector2.One, Main.rand.NextFloat(-5, 5), 0f, ExplosionRadius * 0.0045f + 0.1f, Main.rand.Next(15, 22), false);
            GeneralParticleHandler.SpawnParticle(explosion2);
            Particle explosion3 = new DetailedExplosion(Projectile.Center, Vector2.Zero, Color.Black, Vector2.One, Main.rand.NextFloat(-5, 5), 0f, ExplosionRadius * 0.0030f + 0.1f, Main.rand.Next(15, 22), false);
            GeneralParticleHandler.SpawnParticle(explosion3);
            Particle orb = new GenericBloom(Projectile.Center, Projectile.velocity, color1, ExplosionRadius * 0.0085f + 0.05f, 10, true);
            GeneralParticleHandler.SpawnParticle(orb);
            Particle orb2 = new GenericBloom(Projectile.Center, Projectile.velocity, color2, ExplosionRadius * 0.006f + 0.05f, 10, true, true);
            GeneralParticleHandler.SpawnParticle(orb2);
            float numberOfDusts = ExplosionRadius * 0.1f + 10;
            float rotFactor = 360f / numberOfDusts;
            for (int i = 0; i < numberOfDusts; i++)
            {
                float rot = MathHelper.ToRadians(i * rotFactor);
                Vector2 offset = (Vector2.UnitX * Main.rand.NextFloat(ExplosionRadius * 0.2f, 3.1f)).RotatedBy(rot * Main.rand.NextFloat(1.1f, 9.1f));
                Vector2 velOffset = (Vector2.UnitX * Main.rand.NextFloat(ExplosionRadius * 0.2f, 3.1f)).RotatedBy(rot * Main.rand.NextFloat(1.1f, 9.1f));
                Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, Main.rand.NextBool(4) ? 263 : Projectile.ai[1] == 5 ? 278 : 66, velOffset);
                dust.noGravity = dust.type == 278? false : true;
                dust.color = color1;
                dust.velocity = velOffset;
                dust.scale = dust.type == 278 ? Main.rand.NextFloat(0.7f, 1.3f) : Main.rand.NextFloat(1.6f, 2.2f);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, ExplosionRadius + 20, targetHitbox);
    }
}

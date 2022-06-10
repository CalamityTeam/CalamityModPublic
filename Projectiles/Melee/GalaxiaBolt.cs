using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Projectiles.Melee
{
    public class GalaxiaBolt : ModProjectile
    {
        public NPC target;
        public Player Owner => Main.player[Projectile.owner];

        public ref float Hue => ref Projectile.ai[0];
        public ref float HomingStrenght => ref Projectile.ai[1];

        Particle Head;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galaxia Bolt");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 80;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Head == null)
            {
                Head = new GenericSparkle(Projectile.Center, Vector2.Zero, Color.White, Main.hslToRgb(Hue, 100, 50), 1.2f, 2, 0.06f, 3);
                GeneralParticleHandler.SpawnParticle(Head);
            }
            else
            {
                Head.Position = Projectile.Center + Projectile.velocity * 0.5f;
                Head.Time = 0;
                Head.Scale += (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6) * 0.02f * Projectile.scale;
            }


            if (target == null)
                target = Projectile.Center.ClosestNPCAt(812f, true);

            else if (CalamityUtils.AngleBetween(Projectile.velocity, target.Center - Projectile.Center) < MathHelper.PiOver2) //Home in
            {
                float idealDirection = Projectile.AngleTo(target.Center);
                float updatedDirection = Projectile.velocity.ToRotation().AngleTowards(idealDirection, HomingStrenght);
                Projectile.velocity = updatedDirection.ToRotationVector2() * Projectile.velocity.Length() * 0.995f;
            }


            Lighting.AddLight(Projectile.Center, 0.75f, 1f, 0.24f);

            if (Main.rand.Next(2) == 0)
            {
                Particle smoke = new HeavySmokeParticle(Projectile.Center, Projectile.velocity * 0.5f, Color.Lerp(Color.MidnightBlue, Color.Indigo, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f)), 30, Main.rand.NextFloat(0.6f, 1.2f) * Projectile.scale, 0.8f, 0, false, 0, true);
                GeneralParticleHandler.SpawnParticle(smoke);

                if (Main.rand.Next(3) == 0)
                {
                    Particle smokeGlow = new HeavySmokeParticle(Projectile.Center, Projectile.velocity * 0.5f, Main.hslToRgb(Hue, 1, 0.7f), 20, Main.rand.NextFloat(0.4f, 0.7f) * Projectile.scale, 0.8f, 0, true, 0.005f, true);
                    GeneralParticleHandler.SpawnParticle(smokeGlow);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}

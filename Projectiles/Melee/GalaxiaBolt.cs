using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public Player Owner => Main.player[projectile.owner];

        public ref float Hue => ref projectile.ai[0];
        public ref float HomingStrenght => ref projectile.ai[1];

        Particle Head;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galaxia Bolt");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 30;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 80;
            projectile.melee = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Head == null)
            {
                Head = new GenericSparkle(projectile.Center, Vector2.Zero, Color.White, Main.hslToRgb(Hue, 100, 50), 1.2f, 2, 0.06f, 3);
                GeneralParticleHandler.SpawnParticle(Head);
            }
            else
            {
                Head.Position = projectile.Center + projectile.velocity * 0.5f;
                Head.Time = 0;
                Head.Scale += (float)Math.Sin(Main.GlobalTime * 6) * 0.02f * projectile.scale;
            }


            if (target == null)
                target = projectile.Center.ClosestNPCAt(812f, true);

            else if (CalamityUtils.AngleBetween(projectile.velocity, target.Center - projectile.Center) < MathHelper.PiOver2) //Home in
            {
                float idealDirection = projectile.AngleTo(target.Center);
                float updatedDirection = projectile.velocity.ToRotation().AngleTowards(idealDirection, HomingStrenght);
                projectile.velocity = updatedDirection.ToRotationVector2() * projectile.velocity.Length() * 0.995f;
            }


            Lighting.AddLight(projectile.Center, 0.75f, 1f, 0.24f);

            if (Main.rand.Next(2) == 0)
            {
                Particle smoke = new HeavySmokeParticle(projectile.Center, projectile.velocity * 0.5f, Color.Lerp(Color.MidnightBlue, Color.Indigo, (float)Math.Sin(Main.GlobalTime * 6f)), 30, Main.rand.NextFloat(0.6f, 1.2f) * projectile.scale, 0.8f, 0, false, 0, true);
                GeneralParticleHandler.SpawnParticle(smoke);

                if (Main.rand.Next(3) == 0)
                {
                    Particle smokeGlow = new HeavySmokeParticle(projectile.Center, projectile.velocity * 0.5f, Main.hslToRgb(Hue, 1, 0.7f), 20, Main.rand.NextFloat(0.4f, 0.7f) * projectile.scale, 0.8f, 0, true, 0.005f, true);
                    GeneralParticleHandler.SpawnParticle(smokeGlow);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
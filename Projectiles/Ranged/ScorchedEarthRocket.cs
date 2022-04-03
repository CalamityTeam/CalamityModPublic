using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Ranged
{
    public class ScorchedEarthRocket : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rocket");
            Main.projFrames[Projectile.type] = 10;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            //Lighting
            Lighting.AddLight(Projectile.Center, 1f, 0.79f, 0.3f);

            //Animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter > (Projectile.frame == 1 ? 10 : 7))
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 6;
            }

            CalamityGlobalProjectile.HomeInOnNPC(Projectile, true, 400f, 5f, 30f);

            //Rotation
            Projectile.rotation = Projectile.velocity.ToRotation();

            float xVel = Projectile.velocity.X * 0.5f;
            float yVel = Projectile.velocity.Y * 0.5f;
            int d = Dust.NewDust(new Vector2(Projectile.position.X + 3f + xVel, Projectile.position.Y + 3f + yVel) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 244, 0f, 0f, 100, default, 1f);
            Main.dust[d].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
            Main.dust[d].velocity *= 0.2f;
            Main.dust[d].noGravity = true;
            d = Dust.NewDust(new Vector2(Projectile.position.X + 3f + xVel, Projectile.position.Y + 3f + yVel) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 244, 0f, 0f, 100, default, 0.5f);
            Main.dust[d].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
            Main.dust[d].velocity *= 0.05f;
        }

        public override void Kill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                CalamityGlobalProjectile.ExpandHitboxBy(Projectile, 300);
                SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
                CalamityUtils.ExplosionGores(Projectile.Center, 10);

                Projectile.NewProjectile(Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ScorchedEarthBlast>(), Projectile.damage, Projectile.knockBack * 2f, Projectile.owner);
                for (int j = 0; j < 5; j++)
                {
                    Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(8f, 10f);
                    Projectile.NewProjectile(Projectile.Center, velocity, ModContent.ProjectileType<ScorchedEarthClusterBomb>(), (int)(Projectile.damage * 0.25), Projectile.knockBack * 0.25f, Projectile.owner);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}

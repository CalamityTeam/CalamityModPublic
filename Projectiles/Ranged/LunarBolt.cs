using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class LunarBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 180;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            //Rotation
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi); 
            
            if (projectile.alpha < 170)
            {
                for (int d = 0; d < 5; d++)
                {
                    Vector2 dspeed = -projectile.velocity * 0.5f;
                    int index = Dust.NewDust(projectile.Center, 1, 1, 206, 0f, 0f, 0, default, 1.2f);
                    Main.dust[index].alpha = projectile.alpha;
                    Main.dust[index].velocity = dspeed;
                    Main.dust[index].noGravity = true;
                }
                for (int d = 0; d < 5; d++)
                {
                    Vector2 dspeed2 = -projectile.velocity * 0.5f;
                    int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, 107, 0f, 0f, 0, default, 0.7f);
                    Main.dust[index].alpha = projectile.alpha;
                    Main.dust[index].velocity = dspeed2;
                    Main.dust[index].noGravity = true;
                }
            }
            if (projectile.alpha > 50)
            {
                projectile.alpha -= 25;
            }
            if (projectile.alpha < 50)
            {
                projectile.alpha = 50;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.ai[0] == 0f)
            {
                Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
                Main.PlaySound(SoundID.Item10, projectile.Center);
                projectile.ai[0]++;
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color color = new Color(168, 247, 239);
            return color;
        }
    }
}

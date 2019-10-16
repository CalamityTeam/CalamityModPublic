using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class LunarBolt2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.aiStyle = 1;
            projectile.scale = 1.25f;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 180;
            aiType = 357;
        }

        public override void AI()
        {
            if (projectile.alpha < 170)
            {
                for (int num134 = 0; num134 < 10; num134++)
                {
                    float x = projectile.position.X - projectile.velocity.X / 10f * (float)num134;
                    float y = projectile.position.Y - projectile.velocity.Y / 10f * (float)num134;
                    int num135 = Dust.NewDust(new Vector2(x, y), 1, 1, 107, 0f, 0f, 0, default, 0.2f);
                    Main.dust[num135].alpha = projectile.alpha;
                    Main.dust[num135].position.X = x;
                    Main.dust[num135].position.Y = y;
                    Main.dust[num135].velocity *= 0f;
                    Main.dust[num135].noGravity = true;
                }
            }
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 25;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 10);
            return false;
        }
    }
}

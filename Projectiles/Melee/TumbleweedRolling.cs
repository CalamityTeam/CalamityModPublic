using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class TumbleweedRolling : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/TumbleweedFlail";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tumbleweed");
        }

        public override void SetDefaults()
        {
            projectile.width = 44;
            projectile.height = 44;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 8;
            projectile.timeLeft = 300;
            projectile.melee = true;
        }

        public override void AI()
        {
            if ((projectile.velocity.X != projectile.velocity.X && (projectile.velocity.X < -3f || projectile.velocity.X > 3f)) ||
                (projectile.velocity.Y != projectile.velocity.Y && (projectile.velocity.Y < -3f || projectile.velocity.Y > 3f)))
            {
                Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
                Main.PlaySound(SoundID.NPCHit11, projectile.position);
            }
            if (projectile.velocity.X != projectile.velocity.X)
            {
                projectile.velocity.X = projectile.velocity.X * -0.5f;
            }
            if (projectile.velocity.Y != projectile.velocity.Y && projectile.velocity.Y > 1f)
            {
                projectile.velocity.Y = projectile.velocity.Y * -0.5f;
            }
            projectile.rotation += projectile.velocity.X * 0.05f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 5;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.NPCDeath15, projectile.position);
            for (int num621 = 0; num621 < 20; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 32, 0f, 0f, 100, default, 1.2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 30; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 85, 0f, 0f, 100, default, 1.7f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 85, 0f, 0f, 100, default, 1f);
                Main.dust[num624].velocity *= 2f;
            }
        }
    }
}

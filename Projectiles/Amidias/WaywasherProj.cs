using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class WaywasherProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Waywasher Blast");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.penetrate = 2;
            projectile.timeLeft = 300;
            projectile.magic = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 0.1f, 0.7f);
            for (int num105 = 0; num105 < 2; num105++)
            {
                float num99 = projectile.velocity.X / 3f * (float)num105;
                float num100 = projectile.velocity.Y / 3f * (float)num105;
                int num101 = 4;
                int num102 = Dust.NewDust(new Vector2(projectile.position.X + (float)num101, projectile.position.Y + (float)num101), projectile.width - num101 * 2, projectile.height - num101 * 2, 33, 0f, 0f, 0, new Color(0, 142, 255), 1.2f);
                Main.dust[num102].noGravity = true;
                Main.dust[num102].velocity *= 0.1f;
                Main.dust[num102].velocity += projectile.velocity * 0.1f;
                Dust expr_47FA_cp_0 = Main.dust[num102];
                expr_47FA_cp_0.position.X -= num99;
                Dust expr_4815_cp_0 = Main.dust[num102];
                expr_4815_cp_0.position.Y -= num100;
            }
            if (Main.rand.NextBool(5))
            {
                int num103 = 4;
                int num104 = Dust.NewDust(new Vector2(projectile.position.X + (float)num103, projectile.position.Y + (float)num103), projectile.width - num103 * 2, projectile.height - num103 * 2, 33, 0f, 0f, 0, new Color(0, 142, 255), 0.6f);
                Main.dust[num104].velocity *= 0.25f;
                Main.dust[num104].velocity += projectile.velocity * 0.5f;
            }
            if (projectile.ai[1] >= 20f)
            {
                projectile.velocity.Y = projectile.velocity.Y + 0.2f;
            }
            else
            {
                projectile.rotation += 0.3f * (float)projectile.direction;
            }
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 10);
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 33, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f, 0, new Color(0, 142, 255), 1f);
            }
        }
    }
}

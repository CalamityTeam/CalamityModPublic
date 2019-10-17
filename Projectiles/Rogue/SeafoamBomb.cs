using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles
{
    public class SeafoamBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seafoam Bomb");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 240;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.rotation += projectile.velocity.X * 0.1f;
            projectile.velocity.Y = projectile.velocity.Y + 0.15f;
            projectile.velocity.X = projectile.velocity.X * 0.99f;
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 128;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            Main.PlaySound(2, (int)projectile.Center.X, (int)projectile.Center.Y, 14);
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<SeafoamBubble>(), (int)((double)projectile.damage * 0.4), 0f, projectile.owner, 0f, 0f);
            for (int num625 = 0; num625 < 3; num625++)
            {
                float scaleFactor10 = 0.33f;
                if (num625 == 1)
                {
                    scaleFactor10 = 0.66f;
                }
                if (num625 == 2)
                {
                    scaleFactor10 = 1f;
                }
                int num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                Main.gore[num626].velocity *= scaleFactor10;
                Gore expr_13AB6_cp_0 = Main.gore[num626];
                expr_13AB6_cp_0.velocity.X += 1f;
                Gore expr_13AD6_cp_0 = Main.gore[num626];
                expr_13AD6_cp_0.velocity.Y += 1f;
                num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                Main.gore[num626].velocity *= scaleFactor10;
                Gore expr_13B79_cp_0 = Main.gore[num626];
                expr_13B79_cp_0.velocity.X -= 1f;
                Gore expr_13B99_cp_0 = Main.gore[num626];
                expr_13B99_cp_0.velocity.Y += 1f;
                num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                Main.gore[num626].velocity *= scaleFactor10;
                Gore expr_13C3C_cp_0 = Main.gore[num626];
                expr_13C3C_cp_0.velocity.X += 1f;
                Gore expr_13C5C_cp_0 = Main.gore[num626];
                expr_13C5C_cp_0.velocity.Y -= 1f;
            }
        }
    }
}

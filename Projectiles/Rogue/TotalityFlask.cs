using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class TotalityFlask : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/TotalityBreakers";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Totality Flask");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.aiStyle = 68;
            projectile.timeLeft = 180;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.Calamity().stealthStrike)
            {
                if (projectile.timeLeft % 10 == 0)
                {
                    if (projectile.owner == Main.myPlayer)
                    {
                        Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<TotalityTar>(), (int)(projectile.damage * 0.6), projectile.knockBack, projectile.owner, 0f, 0f);
                    }
                }
            }
            Vector2 spinningpoint = new Vector2(4f, -8f);
            float rotation = projectile.rotation;
            if (projectile.direction == -1)
                spinningpoint.X = -4f;
            Vector2 vector2 = spinningpoint.RotatedBy((double)rotation, new Vector2());
            for (int index1 = 0; index1 < 1; ++index1)
            {
                int index2 = Dust.NewDust(projectile.Center + vector2 - Vector2.One * 5f, 4, 4, 6, 0.0f, 0.0f, 0, new Color(), 1f);
                Main.dust[index2].scale = 1.5f;
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity = Main.dust[index2].velocity * 0.25f + Vector2.Normalize(vector2) * 1f;
                Main.dust[index2].velocity = Main.dust[index2].velocity.RotatedBy(-MathHelper.PiOver2 * (double)projectile.direction, new Vector2());
            }
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner != Main.myPlayer)
                return;

            //glass-pot break sound
            Main.PlaySound(SoundID.Shatter, (int) projectile.position.X, (int) projectile.position.Y, 1, 1f, 0f);

            int meltdown = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<TotalMeltdown>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
            Main.projectile[meltdown].Center = projectile.Center; //makes it centered because it's not without this

            Vector2 vector2 = new Vector2(20f, 20f);
            for (int d = 0; d < 5; ++d)
                Dust.NewDust(projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, 191, 0.0f, 0.0f, 0, Color.Red, 1f);
            for (int d = 0; d < 10; ++d)
            {
                int index2 = Dust.NewDust(projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, 31, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Dust dust = Main.dust[index2];
                dust.velocity *= 1.4f;
            }
            for (int d = 0; d < 20; ++d)
            {
                int index2 = Dust.NewDust(projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, DustID.Fire, 0.0f, 0.0f, 100, new Color(), 2.5f);
                Dust dust1 = Main.dust[index2];
                dust1.noGravity = true;
                dust1.velocity *= 5f;
                int index3 = Dust.NewDust(projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, DustID.Fire, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Dust dust2 = Main.dust[index3];
                dust2.velocity *= 3f;
            }
            int tarAmt = Main.rand.Next(2, 4);
            for (int t = 0; t < tarAmt; t++)
            {
                Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<TotalityTar>(), (int)(projectile.damage * 0.3), 0f, Main.myPlayer, 0f, 0f);
            }
        }
    }
}

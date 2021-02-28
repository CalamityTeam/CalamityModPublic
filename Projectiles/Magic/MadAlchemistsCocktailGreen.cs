using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class MadAlchemistsCocktailGreen : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mad Alchemist's Green Cocktail");
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
        }

        public override void AI()
        {
            projectile.rotation += Math.Abs(projectile.velocity.X) * 0.04f * (float)projectile.direction;
            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 90f)
            {
                projectile.velocity.Y = projectile.velocity.Y + 0.4f;
                projectile.velocity.X = projectile.velocity.X * 0.97f;
            }
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item107, projectile.position);
            Main.PlaySound(SoundID.Item88, projectile.position);
            Gore.NewGore(projectile.Center, -projectile.oldVelocity * 0.2f, 704, 1f);
            Gore.NewGore(projectile.Center, -projectile.oldVelocity * 0.2f, 705, 1f);
            for (int i = 0; i < 3; i++)
            {
                float x = projectile.position.X + (float)Main.rand.Next(-100, 100);
                float y = projectile.position.Y - (float)Main.rand.Next(500, 600);
                Vector2 vector = new Vector2(x, y);
                float num15 = projectile.position.X + (float)(projectile.width / 2) - vector.X;
                float num16 = projectile.position.Y + (float)(projectile.height / 2) - vector.Y;
                num15 += (float)Main.rand.Next(-100, 101);
                int num17 = 25;
                float num18 = (float)Math.Sqrt((double)(num15 * num15 + num16 * num16));
                num18 = (float)num17 / num18;
                num15 *= num18;
                num16 *= num18;

                float ai2 = num16 + projectile.position.Y;
                if (projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(x, y, num15, num16, ProjectileID.LunarFlare, projectile.damage / 2, 5f, projectile.owner, 0f, ai2);
            }
        }
    }
}

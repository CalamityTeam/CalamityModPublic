using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class MadAlchemistsCocktailBlue : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mad Alchemist's Cocktail Blue");
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
			FlaskAI();
        }

		public static void FlaskAI()
		{
            projectile.rotation += Math.Abs(projectile.velocity.X) * 0.04f * projectile.direction;
            if (projectile.ai[0]++ >= 90f)
            {
                projectile.velocity.Y += 0.4f;
                projectile.velocity.X *= 0.97f;
            }
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }
		}

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item107, projectile.position);
            Gore.NewGore(projectile.Center, -projectile.oldVelocity * 0.2f, 704, 1f);
            Gore.NewGore(projectile.Center, -projectile.oldVelocity * 0.2f, 705, 1f);
            int projAmt = Main.rand.Next(20, 31);
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < projAmt; i++)
                {
                    Vector2 velocity = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                    velocity.Normalize();
                    velocity *= Main.rand.Next(10, 201) * 0.01f;
                    Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<MadAlchemistsCocktailGasCloud>(), projectile.damage / 4, 0f, projectile.owner, 0f, Main.rand.Next(-45, 1));
                }
            }
        }
    }
}

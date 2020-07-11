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
            DisplayName.SetDefault("Mad Alchemist's Cocktail Green");
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
			MadAlchemistsCocktailBlue.FlaskAI();
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item107, projectile.position);
            Main.PlaySound(SoundID.Item88, projectile.position);
            Gore.NewGore(projectile.Center, -projectile.oldVelocity * 0.2f, 704, 1f);
            Gore.NewGore(projectile.Center, -projectile.oldVelocity * 0.2f, 705, 1f);
            for (int i = 0; i < 3; i++)
            {
                float x = projectile.Center.X + Main.rand.Next(-100, 100);
                float y = projectile.Center.Y - Main.rand.Next(500, 600);
                Vector2 source = new Vector2(x, y);
				Vector2 velocity = projectile.Center - source;
                velocity.X += Main.rand.Next(-100, 101);
                float speed = 25f;
                float targetDist = velocity.Length();
                targetDist = speed / targetDist;
                velocity.X *= targetDist;
                velocity.Y *= targetDist;

                float ai2 = velocity.Y + projectile.position.Y;
                if (projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(source, velocity, ProjectileID.LunarFlare, projectile.damage / 2, projectile.knockBack * 2f, projectile.owner, 0f, ai2);
            }
        }
    }
}

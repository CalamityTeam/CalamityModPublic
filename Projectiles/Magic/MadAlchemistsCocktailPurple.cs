using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class MadAlchemistsCocktailPurple : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mad Alchemist's Cocktail Purple");
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
			MadAlchemistsCocktailBlue.FlaskAI(projectile);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item107, projectile.position);
            Gore.NewGore(projectile.Center, -projectile.oldVelocity * 0.2f, 704, 1f);
            Gore.NewGore(projectile.Center, -projectile.oldVelocity * 0.2f, 705, 1f);
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 velocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    velocity.Normalize();
                    velocity *= (float)Main.rand.Next(10, 201) * 0.01f;
                    Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<MadAlchemistsCocktailShrapnel>(), projectile.damage / 3, 0f, projectile.owner, 0f, Main.rand.Next(-45, 1));
                }
            }
        }
    }
}

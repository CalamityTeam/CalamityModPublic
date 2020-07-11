using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class MadAlchemistsCocktailRed : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mad Alchemist's Cocktail Red");
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
            int height = 480;
            float dustScaleA = 3f;
            float dustScaleB = 2.4f;
            float dustScaleC = 3.8f;
			Vector2 dustDirection = (-MathHelper.PiOver2).ToRotationVector2();
			Vector2 dustVel = dustDirection * projectile.velocity.Length() * projectile.MaxUpdates;
            Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 20, 2f, 0f);
            Main.PlaySound(SoundID.Item107, projectile.position);
            Gore.NewGore(projectile.Center, -projectile.oldVelocity * 0.2f, 704, 1f);
            Gore.NewGore(projectile.Center, -projectile.oldVelocity * 0.2f, 705, 1f);
            projectile.position = projectile.Center;
            projectile.width = projectile.height = height;
            projectile.Center = projectile.position;
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.damage *= 3;
            projectile.Damage();
            for (int i = 0; i < 60; i++)
            {
                int red = Dust.NewDust(projectile.position, projectile.width, projectile.height, 174, 0f, 0f, 200, default, dustScaleA);
                Dust dust = Main.dust[red];
                dust.position = projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * Main.rand.NextFloat() * (float)projectile.width / 2f;
                dust.noGravity = true;
                dust.velocity *= 8f;
                dust.velocity += dustVel * Main.rand.NextFloat();
                red = Dust.NewDust(projectile.position, projectile.width, projectile.height, 174, 0f, 0f, 100, default, dustScaleB);
                dust.position = projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * Main.rand.NextFloat() * (float)projectile.width / 2f;
                dust.velocity *= 6f;
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.color = Color.Crimson * 0.5f;
                dust.velocity += dustVel * Main.rand.NextFloat();
                red = Dust.NewDust(projectile.position, projectile.width, projectile.height, 174, 0f, 0f, 100, default, dustScaleB);
                dust.position = projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * Main.rand.NextFloat() * (float)projectile.width / 2f;
                dust.velocity *= 4f;
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.color = Color.Crimson * 0.5f;
                dust.velocity += dustVel * Main.rand.NextFloat();
                red = Dust.NewDust(projectile.position, projectile.width, projectile.height, 174, 0f, 0f, 100, default, dustScaleB);
                dust.position = projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * Main.rand.NextFloat() * (float)projectile.width / 2f;
                dust.velocity *= 2f;
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.color = Color.Crimson * 0.5f;
                dust.velocity += dustVel * Main.rand.NextFloat();
            }
            for (int j = 0; j < 30; j++)
            {
                int crimson = Dust.NewDust(projectile.position, projectile.width, projectile.height, 174, 0f, 0f, 0, default, dustScaleC);
                Dust dust = Main.dust[crimson];
                dust.position = projectile.Center + Vector2.UnitX.RotatedByRandom(Math.PI).RotatedBy((double)projectile.velocity.ToRotation(), default) * (float)projectile.width / 3f;
                dust.noGravity = true;
                dust.velocity *= 0.5f;
                dust.velocity += dustVel * (0.6f + 0.6f * Main.rand.NextFloat());
            }
        }
    }
}

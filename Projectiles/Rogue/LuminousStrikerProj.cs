using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class LuminousStrikerProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/LuminousStriker";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Luminous Striker");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 4;
            projectile.localNPCHitCooldown = 13;
            projectile.usesLocalNPCImmunity = true;
            projectile.timeLeft = 120;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0f && projectile.Calamity().stealthStrike)
            {
                projectile.timeLeft = 600;
                projectile.ai[0] = 1f;
            }

            if (Main.rand.NextBool(4))
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 176, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);

            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver4;

            if (projectile.timeLeft % 4 == 0)
            {
                if (projectile.owner == Main.myPlayer)
                {
                    if (projectile.Calamity().stealthStrike)
                    {
                        Projectile.NewProjectile(projectile.Center.X + Main.rand.NextFloat(-15f, 15f), projectile.Center.Y + Main.rand.NextFloat(-15f, 15f), projectile.velocity.X, projectile.velocity.Y, ModContent.ProjectileType<LuminousShard>(), (int)(projectile.damage * 0.25), projectile.knockBack * 0.25f, projectile.owner);
                    }
                    else
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0f, -2f, ModContent.ProjectileType<LuminousShard>(), (int)(projectile.damage * 0.25), projectile.knockBack * 0.25f, projectile.owner);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 7; i++)
                {
                    Vector2 speed = new Vector2(Main.rand.Next(-50, 51), Main.rand.Next(-50, 51));
                    while (speed.X == 0f && speed.Y == 0f)
                    {
                        speed = new Vector2(Main.rand.Next(-50, 51), Main.rand.Next(-50, 51));
                    }
                    speed.Normalize();
                    speed *= Main.rand.Next(30, 61) * 0.1f * 2f;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, speed.X, speed.Y, ModContent.ProjectileType<LuminousShard>(), (int)(projectile.damage * 0.25), projectile.knockBack * 0.25f, projectile.owner);
                }
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 7; i++)
                {
                    Vector2 speed = new Vector2(Main.rand.Next(-50, 51), Main.rand.Next(-50, 51));
                    while (speed.X == 0f && speed.Y == 0f)
                    {
                        speed = new Vector2(Main.rand.Next(-50, 51), Main.rand.Next(-50, 51));
                    }
                    speed.Normalize();
                    speed *= Main.rand.Next(30, 61) * 0.1f * 2f;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, speed.X, speed.Y, ModContent.ProjectileType<LuminousShard>(), (int)(projectile.damage * 0.25), projectile.knockBack * 0.25f, projectile.owner);
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i <= 10; i++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 176, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}

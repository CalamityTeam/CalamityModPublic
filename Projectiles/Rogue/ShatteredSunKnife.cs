using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ShatteredSunKnife : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ShatteredSun";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shattered Sun");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 56;
            projectile.height = 56;
            projectile.friendly = true;
            projectile.penetrate = 4;
            projectile.timeLeft = 300;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 15;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);
            projectile.ai[1] += 1f;
            if (projectile.ai[1] < 5f)
            {
                projectile.alpha -= 50;
            }
            if (projectile.ai[1] == 5f)
            {
                projectile.alpha = 0;
                projectile.tileCollide = false;
            }

            if (projectile.ai[1] == 20f)
            {
                int numProj = 5;
                float rotation = MathHelper.ToRadians(10);
                if (projectile.owner == Main.myPlayer)
                {
                    int spread = 6;
                    for (int i = 0; i < numProj; i++)
                    {
                        Vector2 perturbedspeed = new Vector2(projectile.velocity.X, projectile.velocity.Y + Main.rand.Next(-3, 4)).RotatedBy(MathHelper.ToRadians(spread));
                        int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, perturbedspeed.X * 0.2f, perturbedspeed.Y * 0.2f, ModContent.ProjectileType<ShatteredSunScorchedBlade>(), (int)((double)projectile.damage * 0.75), 1f, projectile.owner, 0f, 0f);
                        Main.projectile[proj].Calamity().stealthStrike = projectile.Calamity().stealthStrike;
                        spread -= Main.rand.Next(2, 6);
                    }
                    Main.PlaySound(SoundID.Item27, projectile.position);
                    projectile.active = false;
                    for (int num621 = 0; num621 < 8; num621++)
                    {
                        int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 127, 0f, 0f, 100, default, 2f);
                        Main.dust[num622].velocity *= 3f;
                        if (Main.rand.NextBool(2))
                        {
                            //Main.dust[num622].scale = 0.5f;
                            Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                        }
                    }
                    for (int num623 = 0; num623 < 16; num623++)
                    {
                        int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 127, 0f, 0f, 100, default, 3f);
                        Main.dust[num624].noGravity = true;
                        Main.dust[num624].velocity *= 5f;
                        num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 127, 0f, 0f, 100, default, 2f);
                        Main.dust[num624].velocity *= 2f;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<ShatteredExplosion>(), (int) ((double) damage * 0.15), projectile.knockBack, projectile.owner, 0f, 0f);
            Main.PlaySound(SoundID.Item14, projectile.position);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<ShatteredExplosion>(), (int)((double)damage * 0.15), projectile.knockBack, projectile.owner, 0f, 0f);
            Main.PlaySound(SoundID.Item14, projectile.position);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<ShatteredExplosion>(), (int)((double)projectile.damage * 0.15), projectile.knockBack, projectile.owner, 0f, 0f);
            Main.PlaySound(SoundID.Item14, projectile.position);
            return true;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item27, projectile.position);
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 246, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f, 0, default, 1f);
            }
        }
    }
}

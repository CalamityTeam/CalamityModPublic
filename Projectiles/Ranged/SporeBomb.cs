using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class SporeBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bomb");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.extraUpdates = 1;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.ranged = true;
            projectile.light = 0.2f;
        }

        public override void AI()
        {
            projectile.alpha -= 2;
            if (projectile.localAI[0] == 0f)
            {
                projectile.scale += 0.05f;
                if (projectile.scale > 1.2f)
                {
                    projectile.localAI[0] = 1f;
                }
            }
            else
            {
                projectile.scale -= 0.05f;
                if (projectile.scale < 0.8f)
                {
                    projectile.localAI[0] = 0f;
                }
            }
            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 20f && projectile.ai[0] < 40f)
            {
                projectile.velocity.Y = projectile.velocity.Y + 0.3f;
                projectile.velocity.X = projectile.velocity.X * 0.98f;
            }
            else if (projectile.ai[0] >= 40f && projectile.ai[0] < 60f)
            {
                projectile.velocity.Y = projectile.velocity.Y - 0.3f;
                projectile.velocity.X = projectile.velocity.X * 1.02f;
            }
            else if (projectile.ai[0] >= 60f)
            {
                projectile.ai[0] = 0f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(Main.DiscoR, 203, 103, projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.position);
            for (int d = 0; d < 25; d++)
            {
                int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, 157, 0f, 0f, 0, new Color(Main.DiscoR, 203, 103), 1f);
                Main.dust[index].noGravity = true;
                Main.dust[index].velocity *= 1.5f;
                Main.dust[index].scale = 1.5f;
            }
            int sporeAmt = Main.rand.Next(3, 7);
            if (projectile.owner == Main.myPlayer)
            {
                for (int s = 0; s < sporeAmt; s++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    int proj = Projectile.NewProjectile(projectile.Center, velocity, ProjectileID.SporeGas + Main.rand.Next(3), (int)(projectile.damage * 0.25), 0f, projectile.owner);
                    if (proj.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[proj].Calamity().forceRanged = true;
                        Main.projectile[proj].usesLocalNPCImmunity = true;
                        Main.projectile[proj].localNPCHitCooldown = 30;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}

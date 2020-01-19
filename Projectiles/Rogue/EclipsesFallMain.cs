using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class EclipsesFallMain : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eclipse's Fall");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.extraUpdates = 1;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (Main.rand.Next(8) == 0)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 138, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 0.785f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            for (int n = 0; n < Main.rand.Next(3, 6); n++) //3 to 5 spears
            {
                float x = target.position.X + (float)Main.rand.Next(-400, 400);
                float y = target.position.Y - (float)Main.rand.Next(500, 800);
                Vector2 vector = new Vector2(x, y);
                float num13 = target.position.X + (float)(target.width / 2) - vector.X;
                float num14 = target.position.Y + (float)(target.height / 2) - vector.Y;
                num13 += (float)Main.rand.Next(-100, 101);
                int num15 = 29;
                float num16 = (float)Math.Sqrt((double)(num13 * num13 + num14 * num14));
                num16 = (float)num15 / num16;
                num13 *= num16;
                num14 *= num16;
                Projectile.NewProjectile(x, y, num13, num14, ModContent.ProjectileType<EclipsesSmol>(), (int)((double)projectile.damage * 0.1 * Main.rand.Next(4, 7)), (int)((double)projectile.knockBack * 0.1 * Main.rand.Next(7, 10)), projectile.owner, 0f, 0f);
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            for (int n = 0; n < Main.rand.Next(3, 6); n++) //3 to 5 spears
            {
                float x = target.position.X + (float)Main.rand.Next(-400, 400);
                float y = target.position.Y - (float)Main.rand.Next(500, 800);
                Vector2 vector = new Vector2(x, y);
                float num13 = target.position.X + (float)(target.width / 2) - vector.X;
                float num14 = target.position.Y + (float)(target.height / 2) - vector.Y;
                num13 += (float)Main.rand.Next(-100, 101);
                int num15 = 29;
                float num16 = (float)Math.Sqrt((double)(num13 * num13 + num14 * num14));
                num16 = (float)num15 / num16;
                num13 *= num16;
                num14 *= num16;
                Projectile.NewProjectile(x, y, num13, num14, ModContent.ProjectileType<EclipsesSmol>(), (int)((double)projectile.damage * 0.1 * Main.rand.Next(4, 7)), (int)((double)projectile.knockBack * 0.1 * Main.rand.Next(7, 10)), projectile.owner, 0f, 0f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }
    }
}

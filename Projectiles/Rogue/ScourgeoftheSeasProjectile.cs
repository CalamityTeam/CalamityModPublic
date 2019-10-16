using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class ScourgeoftheSeasProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Moist Scourge");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.aiStyle = 113;
            aiType = 598;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 1200;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (Main.rand.Next(5) == 0)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 85, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 0.785f;
            projectile.velocity.X *= 1.015f;
            projectile.velocity.Y *= 1.015f;
            if (projectile.velocity.X > 16f)
            {
                projectile.velocity.X = 16f;
            }
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 600);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 14);
            for (int num621 = 0; num621 < 8; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 85, 0f, 0f, 100, default, 1f);
                Main.dust[num622].velocity *= 1f;
            }
            if (projectile.owner == Main.myPlayer)
            {
                int num320 = Main.rand.Next(2, 6);
                int num3;
                for (int num321 = 0; num321 < num320; num321 = num3 + 1)
                {
                    Vector2 vector15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    vector15.Normalize();
                    vector15 *= (float)Main.rand.Next(10, 201) * 0.01f;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, vector15.X, vector15.Y, ModContent.ProjectileType<ScourgeVenomCloud>(), (int)((double)projectile.damage * 0.25), 1f, projectile.owner, 0f, (float)Main.rand.Next(-45, 1));
                    num3 = num321;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }
    }
}

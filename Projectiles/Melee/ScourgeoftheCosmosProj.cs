using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class ScourgeoftheCosmosProj : ModProjectile
    {
        private int bounce = 3;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Scourge");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 36;
            projectile.alpha = 255;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (projectile.alpha <= 200)
            {
                int num3;
                for (int num20 = 0; num20 < 2; num20 = num3 + 1)
                {
                    int dustType = Main.rand.NextBool(3) ? 56 : 242;
                    float num21 = projectile.velocity.X / 4f * num20;
                    float num22 = projectile.velocity.Y / 4f * num20;
                    int num23 = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 0, default, 1f);
                    Main.dust[num23].position.X = projectile.Center.X - num21;
                    Main.dust[num23].position.Y = projectile.Center.Y - num22;
                    Dust dust = Main.dust[num23];
                    dust.velocity *= 0f;
                    Main.dust[num23].scale = 0.7f;
                    num3 = num20;
                }
            }
            projectile.alpha -= 50;
            if (projectile.alpha < 0)
                projectile.alpha = 0;
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 0.785f;
            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 180f)
            {
                projectile.velocity.Y = projectile.velocity.Y + 0.4f;
                projectile.velocity.X = projectile.velocity.X * 0.97f;
            }
            if (projectile.velocity.Y > 16f)
                projectile.velocity.Y = 16f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bounce--;
            if (bounce <= 0)
                projectile.Kill();
            else
            {
                Main.PlaySound(SoundID.NPCHit4, projectile.position);
                if (projectile.velocity.X != oldVelocity.X)
                    projectile.velocity.X = -oldVelocity.X;
                if (projectile.velocity.Y != oldVelocity.Y)
                    projectile.velocity.Y = -oldVelocity.Y;
                if (projectile.owner == Main.myPlayer)
                {
                    int num626 = 1;
                    if (Main.rand.NextBool(10))
                        num626++;
                    int num3;
                    for (int num627 = 0; num627 < num626; num627 = num3 + 1)
                    {
                        float num628 = Main.rand.Next(-35, 36) * 0.02f;
                        float num629 = Main.rand.Next(-35, 36) * 0.02f;
                        num628 *= 10f;
                        num629 *= 10f;
                        Projectile.NewProjectile(projectile.position.X, projectile.position.Y, num628, num629, ModContent.ProjectileType<ScourgeoftheCosmosMini>(), (int)(projectile.damage * 0.7), projectile.knockBack * 0.35f, Main.myPlayer);
                        num3 = num627;
                    }
                }
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.NPCHit4, projectile.position);
            int num3;
            for (int num622 = 0; num622 < 10; num622 = num3 + 1)
            {
                int dustType = Main.rand.NextBool(3) ? 56 : 242;
                int num623 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 0, default, 1f);
                Dust dust = Main.dust[num623];
                dust.scale *= 1.1f;
                Main.dust[num623].noGravity = true;
                num3 = num622;
            }
            for (int num624 = 0; num624 < 15; num624 = num3 + 1)
            {
                int dustType = Main.rand.NextBool(3) ? 56 : 242;
                int num625 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 0, default, 1f);
                Dust dust = Main.dust[num625];
                dust.velocity *= 2.5f;
                dust = Main.dust[num625];
                dust.scale *= 0.8f;
                Main.dust[num625].noGravity = true;
                num3 = num624;
            }
            if (projectile.owner == Main.myPlayer)
            {
                int num626 = 3;
                if (Main.rand.NextBool(10))
                    num626++;
                for (int num627 = 0; num627 < num626; num627 = num3 + 1)
                {
                    float num628 = Main.rand.Next(-35, 36) * 0.02f;
                    float num629 = Main.rand.Next(-35, 36) * 0.02f;
                    num628 *= 10f;
                    num629 *= 10f;
                    Projectile.NewProjectile(projectile.position.X, projectile.position.Y, num628, num629, ModContent.ProjectileType<ScourgeoftheCosmosMini>(), (int)(projectile.damage * 0.7), projectile.knockBack * 0.35f, Main.myPlayer);
                    num3 = num627;
                }
            }
        }
    }
}

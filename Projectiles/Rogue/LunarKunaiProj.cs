using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Projectiles.Rogue
{
    public class LunarKunaiProj : ModProjectile
    {
        bool lunarEnhance = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kunai");
            Main.projFrames[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            CalamityPlayer modPlayer = Main.player[projectile.owner].Calamity();
            projectile.ai[0] += 1f;
            if(projectile.ai[0] == 1f && modPlayer.StealthStrikeAvailable())
                lunarEnhance = true;
            else if (projectile.ai[0] >= 50f)
                lunarEnhance = true;

            if (lunarEnhance)
                projectile.frame = 1;
            else
                projectile.frame = 0;

            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
            CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, lunarEnhance ? 300f : 150f, lunarEnhance ? 12f : 8f, 20f);
            if (Main.rand.Next(6) == 0 && lunarEnhance)
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 229, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
        }

        public override void Kill(int timeLeft)
        {
            if (lunarEnhance)
            {
                Main.PlaySound(SoundID.Item14, projectile.Center);
                projectile.position = projectile.Center;
                projectile.width = projectile.height = 28;
                projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
                projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
                projectile.damage /= 4;
                projectile.usesLocalNPCImmunity = true;
                projectile.localNPCHitCooldown = 10;
                for (int num194 = 0; num194 < 10; num194++)
                {
                    int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 229, 0f, 0f, 0, default, 1.5f);
                    Main.dust[num195].noGravity = true;
                    Main.dust[num195].velocity *= 3f;
                    num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 229, 0f, 0f, 100, default, 1f);
                    Main.dust[num195].velocity *= 2f;
                    Main.dust[num195].noGravity = true;
                }
                projectile.Damage();
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    int num304 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 265, 0f, 0f, 100, default, 1f);
                    Main.dust[num304].noGravity = true;
                    Main.dust[num304].velocity *= 1.2f;
                    Main.dust[num304].velocity -= projectile.oldVelocity * 0.3f;
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

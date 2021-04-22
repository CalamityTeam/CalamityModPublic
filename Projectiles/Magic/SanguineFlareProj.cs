using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class SanguineFlareProj : ModProjectile
    {
        private int x;
        private double speed = 10;
        private float startSpeedY = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flare");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;
            projectile.alpha = 255;
            projectile.magic = true;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                projectile.velocity.X = projectile.velocity.X + (Main.player[projectile.owner].velocity.X * 0.5f);
                startSpeedY = projectile.velocity.Y + (Main.player[projectile.owner].velocity.Y * 0.5f);
                projectile.velocity.Y = startSpeedY;
            }
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] >= 180f)
            {
                x++;
                speed += 0.1;
                projectile.velocity.Y = startSpeedY + (float)(speed * Math.Sin(x / 4));
            }
            projectile.rotation += projectile.velocity.Y * 0.02f;
            projectile.alpha -= 5;
            if (projectile.alpha < 30)
            {
                projectile.alpha = 30;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.position);
            projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
            projectile.width = 10;
            projectile.height = 10;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num621 = 0; num621 < 3; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 1.2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 6; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 1.7f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 1f);
                Main.dust[num624].velocity *= 2f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
            if (!target.canGhostHeal || Main.player[projectile.owner].moonLeech)
            {
                return;
            }
            Player player = Main.player[projectile.owner];
            player.statLife += 1;
            player.HealEffect(1);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class EmpyreanKnife : ModProjectile
    {
        private int bounce = 3;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Empyrean Knife");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 75f)
            {
                projectile.alpha += 10;
                projectile.damage = (int)(projectile.damage * 0.95);
                projectile.knockBack = projectile.knockBack * 0.95f;
                if (projectile.alpha >= 255)
                    projectile.active = false;
            }
            if (projectile.ai[0] < 75f)
                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;
            else
            {
                projectile.rotation += 0.5f;
            }
            CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 150f, 12f, 20f);
            if (Main.rand.NextBool(6))
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 58, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bounce--;
            if (bounce <= 0)
                projectile.Kill();
            else
            {
                if (projectile.velocity.X != oldVelocity.X)
                    projectile.velocity.X = -oldVelocity.X;
                if (projectile.velocity.Y != oldVelocity.Y)
                    projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int num303 = 0; num303 < 3; num303++)
            {
                int num304 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 58, 0f, 0f, 100, default, 0.8f);
                Main.dust[num304].noGravity = true;
                Main.dust[num304].velocity *= 1.2f;
                Main.dust[num304].velocity -= projectile.oldVelocity * 0.3f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            float healAmt = damage * 0.005f;
            if ((int)healAmt == 0)
                return;

            if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                return;

            if (healAmt > CalamityMod.lifeStealCap)
                healAmt = CalamityMod.lifeStealCap;

            CalamityGlobalProjectile.SpawnLifeStealProjectile(projectile, Main.player[projectile.owner], healAmt, ProjectileID.VampireHeal, 1200f, 3f);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            float healAmt = damage * 0.005f;
            if ((int)healAmt == 0)
                return;

            if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                return;

            if (healAmt > CalamityMod.lifeStealCap)
                healAmt = CalamityMod.lifeStealCap;

            CalamityGlobalProjectile.SpawnLifeStealProjectile(projectile, Main.player[projectile.owner], healAmt, ProjectileID.VampireHeal, 1200f, 3f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}

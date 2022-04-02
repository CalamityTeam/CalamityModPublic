using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Healing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class IllustriousKnife : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Illustrious Knife");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 50;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 240f)
            {
                projectile.alpha += 4;
                projectile.damage = (int)(projectile.damage * 0.95);
                projectile.knockBack = (int)(projectile.knockBack * 0.95);
            }
            if (projectile.ai[0] < 240f)
                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 1.57f;
            else
            {
                projectile.rotation += 0.5f;
            }
            CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 300f, 12f, 20f);
            if (Main.rand.NextBool(6))
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 20, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
        }

        public override void Kill(int timeLeft)
        {
            for (int num303 = 0; num303 < 3; num303++)
            {
                int num304 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 20, 0f, 0f, 100, default, 0.8f);
                Main.dust[num304].noGravity = true;
                Main.dust[num304].velocity *= 1.2f;
                Main.dust[num304].velocity -= projectile.oldVelocity * 0.3f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);

            float healAmt = damage * 0.015f;
            if ((int)healAmt == 0)
                return;

            if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                return;

            if (healAmt > CalamityMod.lifeStealCap)
                healAmt = CalamityMod.lifeStealCap;

            CalamityGlobalProjectile.SpawnLifeStealProjectile(projectile, Main.player[projectile.owner], healAmt, ModContent.ProjectileType<RoyalHeal>(), 1200f, 3f);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);

            float healAmt = damage * 0.015f;
            if ((int)healAmt == 0)
                return;

            if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                return;

            if (healAmt > CalamityMod.lifeStealCap)
                healAmt = CalamityMod.lifeStealCap;

            CalamityGlobalProjectile.SpawnLifeStealProjectile(projectile, Main.player[projectile.owner], healAmt, ModContent.ProjectileType<RoyalHeal>(), 1200f, 3f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}

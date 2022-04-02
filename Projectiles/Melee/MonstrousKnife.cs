using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class MonstrousKnife : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Monstrous Knife");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.aiStyle = 2;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 30f)
            {
                projectile.alpha += 10;
                if (projectile.damage > 1)
                    projectile.damage = (int)(projectile.damage * 0.9);
                projectile.knockBack = projectile.knockBack * 0.9f;
                if (projectile.alpha >= 255)
                    projectile.Kill();
            }
            if (projectile.ai[0] < 30f)
                projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + MathHelper.PiOver2;
        }

        public override void Kill(int timeLeft)
        {
            for (int dustIndex = 0; dustIndex < 3; ++dustIndex)
            {
                int redDust = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 182, 0f, 0f, 100, new Color(), 0.8f);
                Dust dust = Main.dust[redDust];
                dust.noGravity = true;
                dust.velocity *= 1.2f;
                dust.velocity -= projectile.oldVelocity * 0.3f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.myPlayer != projectile.owner)
                return;

            if (target.lifeMax <= 5 || Main.player[projectile.owner].moonLeech)
                return;

            if (Main.player[projectile.owner].lifeSteal <= 0f)
                return;

            float healAmt = damage * Main.rand.NextFloat(0.075f, 0.9f);
            if (healAmt < 1f)
                healAmt = 1f;

            if (healAmt > CalamityMod.lifeStealCap)
                healAmt = CalamityMod.lifeStealCap;

            if (Main.rand.NextBool(3))
                CalamityGlobalProjectile.SpawnLifeStealProjectile(projectile, Main.player[projectile.owner], healAmt, ProjectileID.VampireHeal, 1200f, 3f);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (Main.myPlayer != projectile.owner)
                return;

            if (Main.player[projectile.owner].moonLeech)
                return;

            if (Main.player[projectile.owner].lifeSteal <= 0f)
                return;

            float healAmt = damage * Main.rand.NextFloat(0.075f, 0.9f);
            if (healAmt < 1f)
                healAmt = 1f;

            if (healAmt > CalamityMod.lifeStealCap)
                healAmt = CalamityMod.lifeStealCap;

            if (Main.rand.NextBool(3))
                CalamityGlobalProjectile.SpawnLifeStealProjectile(projectile, Main.player[projectile.owner], healAmt, ProjectileID.VampireHeal, 1200f, 3f);
        }
    }
}

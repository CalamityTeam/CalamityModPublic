using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class ElementBallShiv : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shiv");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 120;
            projectile.aiStyle = 27;
        }

        public override void AI()
        {
            int rainbow = Dust.NewDust(projectile.position, projectile.width, projectile.height, 66, (float)(projectile.direction * 2), 0f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.3f);
            Main.dust[rainbow].noGravity = true;
            Main.dust[rainbow].velocity = Vector2.Zero;

            CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 300f, 12f, 20f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft > 115)
                return false;

            return true;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 4; k++)
            {
                int rainbow = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 66, (float)(projectile.direction * 2), 0f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
                Main.dust[rainbow].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            OnHitEffects(target.Center);
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 90);
            target.AddBuff(BuffID.Frostburn, 90);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 90);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            OnHitEffects(target.Center);
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 90);
            target.AddBuff(BuffID.Frostburn, 90);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 90);
        }

        private void OnHitEffects(Vector2 targetPos)
        {
            for (int x = 0; x < 4; x++)
            {
                if (projectile.owner == Main.myPlayer)
                {
                    CalamityUtils.ProjectileBarrage(projectile.Center, targetPos, x > 2, 800f, 800f, 0f, 800f, 1f, ModContent.ProjectileType<SHIV>(), projectile.damage, projectile.knockBack, projectile.owner, false, 50f);
                }
            }
        }
    }
}

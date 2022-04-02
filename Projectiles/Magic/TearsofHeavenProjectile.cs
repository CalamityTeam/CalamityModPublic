using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class TearsofHeavenProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tears of Heaven");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 3;
            projectile.alpha = 255;
            projectile.timeLeft = 240;
            projectile.light = 0.5f;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 25;
                if (projectile.alpha < 0)
                    projectile.alpha = 0;
            }

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 175, 0f, 0f, 100, default, 1f);
                Main.dust[num469].noGravity = true;
                int num96 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 174, 0f, 0f, 100, default, 1f);
                Main.dust[num96].noGravity = true;
                int num102 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 92, 0f, 0f, 100, default, 1f);
                Main.dust[num102].noGravity = true;
            }

            CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 250f, 8f, 20f);

            projectile.rotation += 0.3f * (float)projectile.direction;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 6;
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }
    }
}

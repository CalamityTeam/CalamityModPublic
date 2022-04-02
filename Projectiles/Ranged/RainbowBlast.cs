using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class RainbowBlast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blast");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.ranged = true;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            //Rotation
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

            Lighting.AddLight(projectile.Center, new Vector3(Main.DiscoR, Main.DiscoG, Main.DiscoB) * (1.5f / 255));

            projectile.localAI[0] ++;
            if (projectile.localAI[0] > 5f)
            {
                Vector2 dspeed = -projectile.velocity * 0.5f;
                int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 66, dspeed.X, dspeed.Y, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.1f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity = dspeed;
            }

            CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 200f, 12f, 20f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 90);
            target.AddBuff(BuffID.Frostburn, 90);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 90);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color color = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
            return color;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}

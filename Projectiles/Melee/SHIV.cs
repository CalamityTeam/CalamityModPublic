using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class SHIV : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shiv");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;
            projectile.aiStyle = 27;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (projectile.velocity.Length() < 25f && projectile.timeLeft % 2 == 0)
                projectile.velocity *= 1.04f;

            int rainbow = Dust.NewDust(projectile.position, projectile.width, projectile.height, 66, (float)(projectile.direction * 2), 0f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.3f);
            Main.dust[rainbow].noGravity = true;
            Main.dust[rainbow].velocity = Vector2.Zero;
        }

        public override void Kill(int timeLeft)
        {
            int rainbow = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 66, (float)(projectile.direction * 2), 0f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
            Main.dust[rainbow].noGravity = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 90);
            target.AddBuff(BuffID.Frostburn, 90);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 90);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft > 595)
                return false;

            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}

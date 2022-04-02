using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class StarnightBeam : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.aiStyle = 27;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 3;
            projectile.timeLeft = 600;
            aiType = ProjectileID.LightBeam;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.4f, 0f, 0.4f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft > 595)
                return false;

            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);
            int num3;
            for (int num795 = 4; num795 < 31; num795 = num3 + 1)
            {
                float num796 = projectile.oldVelocity.X * (30f / (float)num795);
                float num797 = projectile.oldVelocity.Y * (30f / (float)num795);
                int num798 = Dust.NewDust(new Vector2(projectile.oldPosition.X - num796, projectile.oldPosition.Y - num797), 8, 8, 73, projectile.oldVelocity.X, projectile.oldVelocity.Y, 100, default, 1.8f);
                Dust dust = Main.dust[num798];
                dust.noGravity = true;
                dust.velocity *= 0.5f;
                num798 = Dust.NewDust(new Vector2(projectile.oldPosition.X - num796, projectile.oldPosition.Y - num797), 8, 8, 73, projectile.oldVelocity.X, projectile.oldVelocity.Y, 100, default, 1.4f);
                dust.velocity *= 0.05f;
                num3 = num795;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 120);
        }
    }
}

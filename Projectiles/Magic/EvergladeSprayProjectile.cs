using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class EvergladeSprayProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spray");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 6;
            projectile.extraUpdates = 2;
            projectile.magic = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.01f / 255f, (255 - projectile.alpha) * 0.15f / 255f, (255 - projectile.alpha) * 0.05f / 255f);
            projectile.scale -= 0.002f;
            if (projectile.scale <= 0f)
            {
                projectile.Kill();
            }
            if (projectile.ai[0] <= 3f)
            {
                projectile.ai[0] += 1f;
                return;
            }
            projectile.velocity.Y = projectile.velocity.Y + 0.075f;
            for (int num151 = 0; num151 < 3; num151++)
            {
                float num152 = projectile.velocity.X / 3f * (float)num151;
                float num153 = projectile.velocity.Y / 3f * (float)num151;
                int num154 = 14;
                int num155 = Dust.NewDust(new Vector2(projectile.position.X + (float)num154, projectile.position.Y + (float)num154), projectile.width - num154 * 2, projectile.height - num154 * 2, 157, 0f, 0f, 100, default, 1f);
                Dust dust = Main.dust[num155];
                dust.noGravity = true;
                dust.velocity *= 0.1f;
                dust.velocity += projectile.velocity * 0.5f;
                dust.position.X -= num152;
                dust.position.Y -= num153;
            }
            if (Main.rand.NextBool(8))
            {
                int num156 = 16;
                int num157 = Dust.NewDust(new Vector2(projectile.position.X + (float)num156, projectile.position.Y + (float)num156), projectile.width - num156 * 2, projectile.height - num156 * 2, 157, 0f, 0f, 100, default, 0.5f);
                Main.dust[num157].velocity *= 0.25f;
                Main.dust[num157].velocity += projectile.velocity * 0.5f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 8;
            target.AddBuff(BuffID.Ichor, 600);
            target.AddBuff(BuffID.CursedInferno, 300);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class SirensSongNote : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Song");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 5;
            projectile.timeLeft = 420;
        }

        public override void AI()
        {
            if (projectile.velocity.Length() > 4f)
                projectile.velocity *= 0.985f;

            if (projectile.localAI[0] == 0f)
            {
                projectile.scale += 0.02f;
                if (projectile.scale >= 1.25f)
                    projectile.localAI[0] = 1f;
            }
            else if (projectile.localAI[0] == 1f)
            {
                projectile.scale -= 0.02f;
                if (projectile.scale <= 0.75f)
                    projectile.localAI[0] = 0f;
            }

            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                Main.harpNote = projectile.ai[0];
                Main.PlaySound(SoundID.Item26, projectile.position);
            }

            Lighting.AddLight(projectile.Center, 0f, 0f, 1.2f);

            if (projectile.velocity.X > 0f)
                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X);
            else
                projectile.rotation = (float)Math.Atan2(-projectile.velocity.Y, -projectile.velocity.X);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X != oldVelocity.X)
                projectile.velocity.X = -oldVelocity.X;

            if (projectile.velocity.Y != oldVelocity.Y)
                projectile.velocity.Y = -oldVelocity.Y;

            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft < 85)
            {
                byte b2 = (byte)(projectile.timeLeft * 3);
                byte a2 = (byte)(50f * (b2 / 255f));
                return new Color(b2, b2, b2, a2);
            }
            return new Color(255, 255, 255, 50);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 7;
            target.AddBuff(BuffID.Confused, 300);
        }
    }
}

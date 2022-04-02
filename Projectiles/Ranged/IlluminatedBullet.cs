using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class IlluminatedBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Illuminated Bullet");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.tileCollide = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.extraUpdates = 3;

            // Invisible for the first few frames
            projectile.alpha = 255;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(projectile.Center, 1f, 0.7f, 0f);

            // Projectile becomes visible after a few frames
            if (projectile.timeLeft == 298)
                projectile.alpha = 0;

            // Once projectile is visible, spawn trailing sparkles
            if (projectile.timeLeft <= 298 && Main.rand.NextBool(5))
            {
                int idx = Dust.NewDust(projectile.Center, 1, 1, 228, 0f, 0f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].noLight = true;
                Main.dust[idx].position = projectile.Center;
                Main.dust[idx].velocity = Vector2.Zero;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesFromEdge(projectile, 0, lightColor);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
        }

        // On impact, make impact dust and play a sound.
        public override void Kill(int timeLeft)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Item10, projectile.position);
        }
    }
}

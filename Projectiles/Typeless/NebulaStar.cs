using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class NebulaStar : BaseSporeSacProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 34;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 3600;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.owner == Main.myPlayer && !FadingOut)
            {
                Vector2 velocity = CalamityUtils.RandomVelocity(100f, 1f, 1f, 0.3f);
                Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<NebulaDust>(), projectile.damage, 0f, projectile.owner, 0f, 0f);
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Rectangle frame = new Rectangle(0, 0, Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Typeless/NebulaStarGlow"), projectile.Center - Main.screenPosition, frame, Color.White * ((255 - projectile.alpha) / 255f), projectile.rotation, projectile.Size / 2, 1f, SpriteEffects.None, 0f);
        }
    }
}

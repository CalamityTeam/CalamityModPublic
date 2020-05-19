using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class AuraRain : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aura Rain");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.aiStyle = 45;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = true;
            projectile.timeLeft = 300;
            projectile.alpha = 255;
            projectile.scale = 1.1f;
            projectile.magic = true;
            projectile.extraUpdates = 1;
            aiType = ProjectileID.RainFriendly;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) - MathHelper.PiOver2;
            Dust dust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 14, projectile.velocity.X, projectile.velocity.Y, 100, default, 1f)];
            dust.velocity = Vector2.Zero;
            dust.position -= projectile.velocity / 5f;
            dust.noGravity = true;
            dust.scale = 0.8f;
            dust.noLight = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}

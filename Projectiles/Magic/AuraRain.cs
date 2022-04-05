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
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = 45;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
            Projectile.scale = 1.1f;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 1;
            AIType = ProjectileID.RainFriendly;
        }

        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) - MathHelper.PiOver2;
            Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 14, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 1f)];
            dust.velocity = Vector2.Zero;
            dust.position -= Projectile.velocity / 5f;
            dust.noGravity = true;
            dust.scale = 0.8f;
            dust.noLight = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}

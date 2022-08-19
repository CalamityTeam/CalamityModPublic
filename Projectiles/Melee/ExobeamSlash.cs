using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using System;

namespace CalamityMod.Projectiles.Melee
{
    public class ExobeamSlash : ModProjectile
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Exobeam Slash");

        public override void SetDefaults()
        {
            Projectile.width = 512;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.Opacity = 1f;
            Projectile.timeLeft = 35;
            Projectile.MaxUpdates = 2;
            Projectile.scale = 0.75f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.MaxUpdates * 12;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Opacity = Projectile.timeLeft / 35f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Projectile.RotatingHitboxCollision(targetHitbox.TopLeft(), targetHitbox.Size());
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.ExoDebuffs();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.ExoDebuffs();
        }

        public override bool ShouldUpdatePosition() => true;

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(Color.Cyan, Color.Lime, Projectile.identity / 7f % 1f) * Projectile.Opacity;

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 33)
                return false;

            Main.spriteBatch.SetBlendState(BlendState.Additive);

            float progress = (33 - Projectile.timeLeft) / 33f;

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D bloomTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = texture.Size() * 0.5f;
            Vector2 scale = new Vector2(0.5f + 0.7f * (float)Math.Pow(progress, 0.45), 0.55f - 0.3f * (float)Math.Pow(progress, 0.4)) * Projectile.scale;


            Vector2 bloomScale = Projectile.Size / bloomTexture.Size() * new Vector2(1f, 2f);
            Vector2 bloomOrigin = bloomTexture.Size() * 0.5f;
            Main.spriteBatch.Draw(bloomTexture, drawPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, bloomOrigin, bloomScale, 0, 0f);
            Main.spriteBatch.Draw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, scale, 0, 0f);
            Main.spriteBatch.Draw(texture, drawPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, origin, scale * new Vector2(1f, 0.6f), 0, 0f);

            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}

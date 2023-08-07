using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Projectiles.Typeless
{
    public class SabatonSlam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public float scaleFromFall;
        public float damageScaleFromFall;

        public override void SetDefaults()
        {
            Projectile.width = 160;
            Projectile.height = 160;
            Projectile.friendly = true;
            Projectile.timeLeft = 60;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            // Image is 156x156
            scaleFromFall = (Projectile.ai[0] / 22) + 0.5f;
            damageScaleFromFall = Projectile.ai[0] / 40;
            Projectile.damage = (int)(300f * damageScaleFromFall + 300f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D lightTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/HollowCircleHardEdge").Value;

            Main.EntitySpriteDraw(lightTexture, Projectile.Center - Main.screenPosition, null, Color.White, 0f, lightTexture.Size() * 0.5f, scaleFromFall, SpriteEffects.None, 0);

            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, (scaleFromFall * 64f), targetHitbox);
    }
}

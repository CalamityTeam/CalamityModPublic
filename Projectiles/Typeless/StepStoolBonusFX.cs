using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class StepStoolBonusFX : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "Calamitymod/Projectiles/InvisibleProj";
        public float OpacityMultiplier = 1f;
        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;

            Projectile.timeLeft = 100;
            Projectile.alpha = 0;
        }

        public override void AI()
        {
            // Gravity!
            Projectile.velocity.Y += 0.15f;

            Projectile.rotation += 0.12f;
            Projectile.scale -= 0.0125f;
            OpacityMultiplier -= 0.025f;

            if (Projectile.timeLeft > 99)
                Projectile.alpha = 255;
            else
                Projectile.alpha = 0;
        }

        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Extra[102].Value;
            Vector2 origin = texture.Size() / 2f;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White * ((1f - (Projectile.alpha / 255f)) * OpacityMultiplier), Projectile.rotation + (Main.player[Projectile.owner].gravDir == -1 ? MathHelper.Pi : 0), origin, Projectile.scale * 1.25f, SpriteEffects.None);

            return false;
        }
    }
}

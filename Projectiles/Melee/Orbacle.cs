using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace CalamityMod.Projectiles.Melee
{
    public class Orbacle : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/ExtraTextures/TinyGreyscaleCircle";
        private static int Lifetime = 40;

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = Lifetime;

            Projectile.alpha = 80;

            // Auric orbs never hit the same enemy more than once.
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.SetBlendState(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = texture.Size() * 0.5f;
            Color color = new Color(83, 137, 230); // Auric Blue but slightly more blue. (#5389e6)
            Main.EntitySpriteDraw(texture, drawPosition, null, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
        public override void AI()
        {
            // Produces golden dust while in flight
            int dustType = Main.rand.NextBool(3) ? 244 : 246;
            float scale = 0.8f + Main.rand.NextFloat(0.6f);
            int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType);
            Main.dust[idx].noGravity = true;
            Main.dust[idx].velocity = Projectile.velocity / 3f;
            Main.dust[idx].scale = scale;

            Projectile.alpha += 4;
            Projectile.velocity *= 0.88f;
        }
    }
}

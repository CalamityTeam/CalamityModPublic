using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class WulfrumBolt : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 2;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, (255 - Projectile.alpha) * 0.1f / 255f, 0f);
            for (int num151 = 0; num151 < 3; num151++)
            {
                int num154 = 14;
                int num155 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width - num154 * 2, Projectile.height - num154 * 2, 61, 0f, 0f, 100, default, 3f);
                Main.dust[num155].noGravity = true;
                Main.dust[num155].noLight = true;
                Main.dust[num155].velocity *= 0.1f;
                Main.dust[num155].velocity += Projectile.velocity * 0.5f;
            }
            if (Main.rand.NextBool(8))
            {
                int num156 = 16;
                int num157 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width - num156 * 2, Projectile.height - num156 * 2, 61, 0f, 0f, 100, default, 2.25f);
                Main.dust[num157].velocity *= 0.25f;
                Main.dust[num157].noLight = true;
                Main.dust[num157].velocity += Projectile.velocity * 0.5f;
                return;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}

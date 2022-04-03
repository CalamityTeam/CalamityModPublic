using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class ClamorRifleProjSplit : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Ranged/ClamorRifleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Energy Bolt");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.scale = 0.9f;
            Projectile.timeLeft = 180;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 150 && target.CanBeChasedBy(Projectile);

        public override void AI()
        {
            Projectile.rotation += 0.15f;

            Lighting.AddLight(Projectile.Center, new Vector3(44, 191, 232) * (1.3f/255));

            for (int num151 = 0; num151 < 2; num151++)
            {
                int num154 = 14;
                int num155 = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.Center.Y), Projectile.width - num154 * 2, Projectile.height - num154 * 2, 68, 0f, 0f, 100, default, 1f);
                Main.dust[num155].noGravity = true;
                Main.dust[num155].velocity *= 0.1f;
                Main.dust[num155].velocity += Projectile.velocity * 0.5f;
            }

            if (Projectile.timeLeft < 150)
                CalamityGlobalProjectile.HomeInOnNPC(Projectile, !Projectile.tileCollide, 450f, 12f, 25f);
        }

        /* override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }*/
    }
}

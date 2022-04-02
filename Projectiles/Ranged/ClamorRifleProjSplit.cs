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
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.scale = 0.9f;
            projectile.timeLeft = 180;
            projectile.penetrate = 1;
            projectile.ranged = true;
        }

        public override bool? CanHitNPC(NPC target) => projectile.timeLeft < 150 && target.CanBeChasedBy(projectile);

        public override void AI()
        {
            projectile.rotation += 0.15f;

            Lighting.AddLight(projectile.Center, new Vector3(44, 191, 232) * (1.3f/255));

            for (int num151 = 0; num151 < 2; num151++)
            {
                int num154 = 14;
                int num155 = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y), projectile.width - num154 * 2, projectile.height - num154 * 2, 68, 0f, 0f, 100, default, 1f);
                Main.dust[num155].noGravity = true;
                Main.dust[num155].velocity *= 0.1f;
                Main.dust[num155].velocity += projectile.velocity * 0.5f;
            }

            if (projectile.timeLeft < 150)
                CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 450f, 12f, 25f);
        }

        /* override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }*/
    }
}

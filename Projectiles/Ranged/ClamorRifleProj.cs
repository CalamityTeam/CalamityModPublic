using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class ClamorRifleProj : ModProjectile
    {
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
            projectile.penetrate = 1;
            projectile.ranged = true;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            projectile.rotation += 0.15f;
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 3f)
            {
                Lighting.AddLight(projectile.Center, new Vector3(44, 191, 232) * (1.3f / 255));
                for (int num151 = 0; num151 < 2; num151++)
                {
                    int num154 = 14;
                    int num155 = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y), projectile.width - num154 * 2, projectile.height - num154 * 2, 68, 0f, 0f, 100, default, 1f);
                    Main.dust[num155].noGravity = true;
                    Main.dust[num155].velocity *= 0.1f;
                    Main.dust[num155].velocity += projectile.velocity * 0.5f;
                }
            }
            CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 150f, 12f, 25f);
        }

        /*public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }*/

        public override void Kill(int timeLeft)
        {
            int bulletAmt = 2;
            if (projectile.owner == Main.myPlayer)
            {
                for (int b = 0; b < bulletAmt; b++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<ClamorRifleProjSplit>(), (int)(projectile.damage * 0.45), 0f, projectile.owner, 0f, 0f);
                }
            }
            Main.PlaySound(SoundID.Item118, projectile.Center);
        }
    }
}

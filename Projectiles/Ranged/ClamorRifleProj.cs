using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
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
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            Projectile.rotation += 0.15f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 3f)
            {
                Lighting.AddLight(Projectile.Center, new Vector3(44, 191, 232) * (1.3f / 255));
                for (int num151 = 0; num151 < 2; num151++)
                {
                    int num154 = 14;
                    int num155 = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.Center.Y), Projectile.width - num154 * 2, Projectile.height - num154 * 2, 68, 0f, 0f, 100, default, 1f);
                    Main.dust[num155].noGravity = true;
                    Main.dust[num155].velocity *= 0.1f;
                    Main.dust[num155].velocity += Projectile.velocity * 0.5f;
                }
            }
            CalamityGlobalProjectile.HomeInOnNPC(Projectile, !Projectile.tileCollide, 150f, 12f, 25f);
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
            if (Projectile.owner == Main.myPlayer)
            {
                for (int b = 0; b < bulletAmt; b++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(Projectile.Center, velocity, ModContent.ProjectileType<ClamorRifleProjSplit>(), (int)(Projectile.damage * 0.45), 0f, Projectile.owner, 0f, 0f);
                }
            }
            SoundEngine.PlaySound(SoundID.Item118, Projectile.Center);
        }
    }
}

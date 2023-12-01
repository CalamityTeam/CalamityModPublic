using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class CosmicOrb : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetDefaults()
        {
            Projectile.extraUpdates = 0;
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.075f, 0.5f, 0.15f));

            Projectile.velocity *= 0.985f;
            Projectile.rotation += Projectile.velocity.X * 0.2f;

            if (Projectile.velocity.X > 0f)
            {
                Projectile.rotation += 0.08f;
            }
            else
            {
                Projectile.rotation -= 0.08f;
            }

            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] > 30f)
            {
                Projectile.alpha += 10;
                if (Projectile.alpha >= 255)
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                    return;
                }
            }

            CalamityUtils.MagnetSphereHitscan(Projectile, 300f, 6f, 8f, 5, ModContent.ProjectileType<CosmicBolt>());
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item54, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                int dustScale = (int)(10f * Projectile.scale);
                int d = Dust.NewDust(Projectile.Center - Vector2.One * (float)dustScale, dustScale * 2, dustScale * 2, 242, 0f, 0f, 0, default, 1f);
                Dust dust = Main.dust[d];
                Vector2 offset = Vector2.Normalize(dust.position - Projectile.Center);
                dust.position = Projectile.Center + offset * (float)dustScale * Projectile.scale;
                if (i < 30)
                {
                    dust.velocity = offset * dust.velocity.Length();
                }
                else
                {
                    dust.velocity = offset * Main.rand.NextFloat(4.5f, 9f);
                }
                dust.color = Main.hslToRgb(0.95f, 0.41f + Main.rand.NextFloat() * 0.2f, 0.93f);
                dust.color = Color.Lerp(dust.color, Color.White, 0.3f);
                dust.noGravity = true;
                dust.scale = 0.7f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200 - Projectile.alpha, 200 - Projectile.alpha, 200 - Projectile.alpha, 200 - Projectile.alpha);
        }
    }
}

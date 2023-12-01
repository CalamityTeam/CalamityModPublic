using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class FlareBoltProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 480;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 25;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.45f / 255f, (255 - Projectile.alpha) * 0.2f / 255f, (255 - Projectile.alpha) * 0.1f / 255f);
            for (int i = 0; i < 2; i++)
            {
                float shortXVel = Projectile.velocity.X / 3f * (float)i;
                float shortYVel = Projectile.velocity.Y / 3f * (float)i;
                int fourConst = 4;
                int fireDust = Dust.NewDust(new Vector2(Projectile.position.X + (float)fourConst, Projectile.position.Y + (float)fourConst), Projectile.width - fourConst * 2, Projectile.height - fourConst * 2, 174, 0f, 0f, 100, default, 1.2f);
                Dust dust = Main.dust[fireDust];
                dust.noGravity = true;
                dust.velocity *= 0.1f;
                dust.velocity += Projectile.velocity * 0.1f;
                dust.position.X -= shortXVel;
                dust.position.Y -= shortYVel;
            }
            if (Main.rand.NextBool(10))
            {
                int otherFourConst = 4;
                int fireDustSmol = Dust.NewDust(new Vector2(Projectile.position.X + (float)otherFourConst, Projectile.position.Y + (float)otherFourConst), Projectile.width - otherFourConst * 2, Projectile.height - otherFourConst * 2, 174, 0f, 0f, 100, default, 0.6f);
                Main.dust[fireDustSmol].velocity *= 0.25f;
                Main.dust[fireDustSmol].velocity += Projectile.velocity * 0.5f;
            }
            Projectile.rotation += 0.3f * (float)Projectile.direction;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
                SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 174, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }
    }
}

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class LuxorsGiftRanged : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 180;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < 85)
            {
                byte b2 = (byte)(Projectile.timeLeft * 3);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                return new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            return new Color(255, 255, 255, 100);
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.scale -= 0.01f;
                Projectile.alpha += 15;
                if (Projectile.alpha >= 250)
                {
                    Projectile.alpha = 255;
                    Projectile.localAI[0] = 1f;
                }
            }
            else if (Projectile.localAI[0] == 1f)
            {
                Projectile.scale += 0.01f;
                Projectile.alpha -= 15;
                if (Projectile.alpha <= 0)
                {
                    Projectile.alpha = 0;
                    Projectile.localAI[0] = 0f;
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.localAI[1] += 1f;
            if (Projectile.localAI[1] == 3f)
            {
                for (int l = 0; l < 12; l++)
                {
                    Vector2 offset = Vector2.UnitX * (float)-(float)Projectile.width / 2f;
                    offset += -Vector2.UnitY.RotatedBy((double)(l * MathHelper.Pi / 6f), default) * new Vector2(8f, 16f);
                    offset = offset.RotatedBy((double)(Projectile.rotation - MathHelper.PiOver2), default);
                    int electric = Dust.NewDust(Projectile.Center, 0, 0, 135, 0f, 0f, 160, default, 1f);
                    Main.dust[electric].scale = 1.1f;
                    Main.dust[electric].noGravity = true;
                    Main.dust[electric].position = Projectile.Center + offset;
                    Main.dust[electric].velocity = Projectile.velocity * 0.1f;
                    Main.dust[electric].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[electric].position) * 1.25f;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.ExpandHitboxBy(16);
            Projectile.maxPenetrate = Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item92, Projectile.position);
            int dustAmt = Main.rand.Next(10, 20);
            for (int d = 0; d < dustAmt; d++)
            {
                int electric = Dust.NewDust(Projectile.Center - Projectile.velocity / 2f, 0, 0, 135, 0f, 0f, 100, default, 2f);
                Main.dust[electric].velocity *= 2f;
                Main.dust[electric].noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }
    }
}

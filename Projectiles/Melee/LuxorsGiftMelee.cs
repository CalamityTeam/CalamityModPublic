using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class LuxorsGiftMelee : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 3;
            Projectile.alpha = 255;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
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
            Projectile.localAI[1] += 1f;
            if (Projectile.localAI[1] == 3f)
            {
                for (int l = 0; l < 12; l++)
                {
                    Vector2 dustRotate = Vector2.UnitX * (float)-(float)Projectile.width / 2f;
                    dustRotate += -Vector2.UnitY.RotatedBy((double)((float)l * 3.14159274f / 6f), default) * new Vector2(8f, 16f);
                    dustRotate = dustRotate.RotatedBy((double)(Projectile.rotation - 1.57079637f), default);
                    int spawnDust = Dust.NewDust(Projectile.Center, 0, 0, (int)CalamityDusts.Brimstone, 0f, 0f, 160, default, 1f);
                    Main.dust[spawnDust].scale = 1.1f;
                    Main.dust[spawnDust].noGravity = true;
                    Main.dust[spawnDust].position = Projectile.Center + dustRotate;
                    Main.dust[spawnDust].velocity = Projectile.velocity * 0.1f;
                    Main.dust[spawnDust].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[spawnDust].position) * 1.25f;
                }
            }
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

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int l = 0; l < 12; l++)
            {
                Vector2 dustRotate = Vector2.UnitX * (float)-(float)Projectile.width / 2f;
                dustRotate += -Vector2.UnitY.RotatedBy((double)((float)l * 3.14159274f / 6f), default) * new Vector2(8f, 16f);
                dustRotate = dustRotate.RotatedBy((double)(Projectile.rotation - 1.57079637f), default);
                int spawnDust = Dust.NewDust(Projectile.Center, 0, 0, (int)CalamityDusts.Brimstone, 0f, 0f, 160, default, 1f);
                Main.dust[spawnDust].scale = 1.1f;
                Main.dust[spawnDust].noGravity = true;
                Main.dust[spawnDust].position = Projectile.Center + dustRotate;
                Main.dust[spawnDust].velocity = Projectile.velocity * 0.1f;
                Main.dust[spawnDust].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[spawnDust].position) * 1.25f;
            }
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }
    }
}

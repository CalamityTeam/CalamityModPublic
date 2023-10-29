using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Magic
{
    public class IonBlast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 2;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 120;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + MathHelper.PiOver2;
            Projectile.velocity *= 1.015f;
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 3;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            Lighting.AddLight((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, 1f, 0f, 0.2f);
            float projTimer = 100f;
            float timerIncr = 3f;
            if (Projectile.ai[1] == 0f)
            {
                Projectile.localAI[0] += timerIncr;
                if (Projectile.localAI[0] > projTimer)
                {
                    Projectile.localAI[0] = projTimer;
                }
            }
            else
            {
                Projectile.localAI[0] -= timerIncr;
                if (Projectile.localAI[0] <= 0f)
                {
                    Projectile.Kill();
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 32;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);
            int randDustAmt = Main.rand.Next(15, 30);
            for (int i = 0; i < randDustAmt; i++)
            {
                int reddish = Dust.NewDust(Projectile.Center - Projectile.velocity / 2f, 0, 0, 130, 0f, 0f, 100, default, 1f);
                Main.dust[reddish].velocity *= 2f;
                Main.dust[reddish].noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}

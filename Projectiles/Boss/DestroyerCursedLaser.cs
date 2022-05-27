using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Boss
{
    public class DestroyerCursedLaser : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Laser");
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.scale = 1.8f;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, 0f, (255 - Projectile.alpha) * 0.75f / 255f, 0f);
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 125;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            if (Projectile.localAI[1] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item33, Projectile.position);
                Projectile.localAI[1] = 1f;
            }
            if (Projectile.velocity.Length() < 12f)
            {
                Projectile.velocity *= 1.0025f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.alpha < 200)
            {
                return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0);
            }
            return Color.Transparent;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, 120);
        }
    }
}

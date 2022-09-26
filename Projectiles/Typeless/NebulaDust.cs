using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class NebulaDust : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dust");
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.Opacity = 0f;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 3600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 3;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.X * 0.02f;

            if (Projectile.velocity.X < 0f)
                Projectile.rotation -= Math.Abs(Projectile.velocity.Y) * 0.02f;
            else
                Projectile.rotation += Math.Abs(Projectile.velocity.Y) * 0.02f;

            Projectile.velocity *= 0.98f;

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 60f)
            {
                if (Projectile.Opacity > 0f)
                {
                    Projectile.Opacity -= 0.02f;
                    if (Projectile.Opacity < 0f)
                        Projectile.Opacity = 0f;
                }
                else if (Projectile.owner == Main.myPlayer)
                    Projectile.Kill();
            }
            else if (Projectile.Opacity < 0.7f)
            {
                Projectile.Opacity += 0.1f;
                if (Projectile.Opacity > 0.7f)
                    Projectile.Opacity = 0.7f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor.R = (byte)(255 * Projectile.Opacity);
            lightColor.G = (byte)(255 * Projectile.Opacity);
            lightColor.B = (byte)(255 * Projectile.Opacity);
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}

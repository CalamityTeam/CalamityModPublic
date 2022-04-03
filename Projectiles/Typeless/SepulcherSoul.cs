using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class SepulcherSoul : ModProjectile
    {
        public ref float Time => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tormented Soul");
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.Opacity = 0f;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.frame = Main.rand.Next(Main.projFrames[Projectile.type]);
                Projectile.localAI[0] = 1f;
            }
            Vector2 idealVelocity = Vector2.Zero;
            idealVelocity.X = (float)(Math.Sin(Time / 27f + Projectile.identity * 1.1f) + (float)Math.Cos(Math.E * (Time / 27f + Projectile.identity * 1.1f))) * 4f;
            idealVelocity.Y = MathHelper.SmoothStep(-3f, -9f, (float)Math.Sin(Time / 23f + Projectile.identity * 1.1f) * 0.5f + 0.5f);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, idealVelocity, 0.075f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.Opacity = Utils.InverseLerp(0f, 15f, Time, true) * Utils.InverseLerp(0f, 25f, Projectile.timeLeft, true);
            Time++;
        }
    }
}

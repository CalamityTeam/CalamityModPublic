using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class SepulcherSoul : ModProjectile
    {
        public ref float Time => ref projectile.ai[0];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tormented Soul");
            Main.projFrames[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 16;
            projectile.Opacity = 0f;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 120;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                projectile.frame = Main.rand.Next(Main.projFrames[projectile.type]);
                projectile.localAI[0] = 1f;
            }
            Vector2 idealVelocity = Vector2.Zero;
            idealVelocity.X = (float)(Math.Sin(Time / 27f + projectile.identity * 1.1f) + (float)Math.Cos(Math.E * (Time / 27f + projectile.identity * 1.1f))) * 4f;
            idealVelocity.Y = MathHelper.SmoothStep(-3f, -9f, (float)Math.Sin(Time / 23f + projectile.identity * 1.1f) * 0.5f + 0.5f);
            projectile.velocity = Vector2.Lerp(projectile.velocity, idealVelocity, 0.075f);
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            projectile.Opacity = Utils.InverseLerp(0f, 15f, Time, true) * Utils.InverseLerp(0f, 25f, projectile.timeLeft, true);
            Time++;
        }
    }
}

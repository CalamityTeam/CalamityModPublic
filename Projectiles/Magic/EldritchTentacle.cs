using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class EldritchTentacle : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.MaxUpdates = 3;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            if (Projectile.velocity.X != Projectile.velocity.X)
            {
                if (Math.Abs(Projectile.velocity.X) < 1f)
                {
                    Projectile.velocity.X = -Projectile.velocity.X;
                }
                else
                {
                    Projectile.Kill();
                }
            }
            if (Projectile.velocity.Y != Projectile.velocity.Y)
            {
                if (Math.Abs(Projectile.velocity.Y) < 1f)
                {
                    Projectile.velocity.Y = -Projectile.velocity.Y;
                }
                else
                {
                    Projectile.Kill();
                }
            }
            Vector2 projCenter = Projectile.Center;
            Projectile.scale = 1f - Projectile.localAI[0];
            Projectile.width = (int)(20f * Projectile.scale);
            Projectile.height = Projectile.width;
            Projectile.position.X = projCenter.X - (float)(Projectile.width / 2);
            Projectile.position.Y = projCenter.Y - (float)(Projectile.height / 2);
            if ((double)Projectile.localAI[0] < 0.1)
            {
                Projectile.localAI[0] += 0.01f;
            }
            else
            {
                Projectile.localAI[0] += 0.025f;
            }
            if (Projectile.localAI[0] >= 0.95f)
            {
                Projectile.Kill();
            }
            Projectile.velocity.X = Projectile.velocity.X + Projectile.ai[0] * 1.5f;
            Projectile.velocity.Y = Projectile.velocity.Y + Projectile.ai[1] * 1.5f;
            if (Projectile.velocity.Length() > 16f)
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= 16f;
            }
            Projectile.ai[0] *= 1.05f;
            Projectile.ai[1] *= 1.05f;
            if (Projectile.scale < 1f)
            {
                int scaleLoopCheck = 0;
                while ((float)scaleLoopCheck < Projectile.scale * 10f)
                {
                    int eldritchRed = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 1.1f);
                    Main.dust[eldritchRed].position = (Main.dust[eldritchRed].position + Projectile.Center) / 2f;
                    Main.dust[eldritchRed].noGravity = true;
                    Main.dust[eldritchRed].velocity *= 0.1f;
                    Main.dust[eldritchRed].velocity -= Projectile.velocity * (1.3f - Projectile.scale);
                    Main.dust[eldritchRed].fadeIn = (float)(100 + Projectile.owner);
                    Main.dust[eldritchRed].scale += Projectile.scale * 0.75f;
                    scaleLoopCheck++;
                }
            }
        }
    }
}

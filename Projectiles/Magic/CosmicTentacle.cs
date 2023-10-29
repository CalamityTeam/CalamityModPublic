using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class CosmicTentacle : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.MaxUpdates = 3;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Projectile.localAI[1]++;
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
            if (Projectile.localAI[0] < 0.1f)
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
            if (Projectile.scale < 1f && Projectile.localAI[1] > 5f)
            {
                int scaleLoopCheck = 0;
                while ((float)scaleLoopCheck < Projectile.scale * 10f)
                {
                    int purpleDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 62, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 1.1f);
                    Main.dust[purpleDust].position = (Main.dust[purpleDust].position + Projectile.Center) / 2f;
                    Main.dust[purpleDust].noGravity = true;
                    Main.dust[purpleDust].velocity *= 0.1f;
                    Main.dust[purpleDust].velocity -= Projectile.velocity * (1.3f - Projectile.scale);
                    Main.dust[purpleDust].fadeIn = (float)(100 + Projectile.owner);
                    Main.dust[purpleDust].scale += Projectile.scale * 0.75f;
                    scaleLoopCheck++;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 5;
        }
    }
}

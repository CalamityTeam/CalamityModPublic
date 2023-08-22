using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class VoidTentacle : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.height = 160;
            Projectile.width = 160;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.MaxUpdates = 3;

            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.MaxUpdates * 4;
        }

        public override void AI()
        {
            // HOW CAN THIS CODE EVER RUN
            if (Projectile.velocity.X != Projectile.velocity.X)
            {
                if (Math.Abs(Projectile.velocity.X) < 1f)
                    Projectile.velocity.X = -Projectile.velocity.X;
                else
                    Projectile.Kill();
            }
            if (Projectile.velocity.Y != Projectile.velocity.Y)
            {
                if (Math.Abs(Projectile.velocity.Y) < 1f)
                    Projectile.velocity.Y = -Projectile.velocity.Y;
                else
                    Projectile.Kill();
            }

            Vector2 center10 = Projectile.Center;
            Projectile.scale = 1f - Projectile.localAI[0];
            Projectile.width = (int)(20f * Projectile.scale);
            Projectile.height = Projectile.width;
            Projectile.position.X = center10.X - (float)(Projectile.width / 2);
            Projectile.position.Y = center10.Y - (float)(Projectile.height / 2);
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
                int i = 0;
                while (i < Projectile.scale * 4f)
                {
                    int dustID = Main.rand.NextBool(5) ? 199 : 175;
                    int idx = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustID, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 1.1f);
                    Main.dust[idx].position = (Main.dust[idx].position + Projectile.Center) / 2f;
                    Main.dust[idx].noGravity = true;
                    Main.dust[idx].velocity *= 0.1f;
                    Main.dust[idx].velocity -= Projectile.velocity * (1.3f - Projectile.scale);
                    Main.dust[idx].fadeIn = (float)(100 + Projectile.owner);
                    Main.dust[idx].scale += Projectile.scale * 0.75f;
                    i++;
                }
            }
        }
    }
}

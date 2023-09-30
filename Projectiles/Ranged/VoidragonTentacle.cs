using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class VoidragonTentacle : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.MaxUpdates = 3;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 1;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
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
            Vector2 center10 = Projectile.Center;
            Projectile.scale = 1f - Projectile.localAI[0];
            Projectile.width = (int)(20f * Projectile.scale);
            Projectile.height = Projectile.width;
            Projectile.position.X = center10.X - (float)(Projectile.width / 2);
            Projectile.position.Y = center10.Y - (float)(Projectile.height / 2);
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
                int dustAmount = 0;
                while ((float)dustAmount < Projectile.scale * 10f)
                {
                    int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 62, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 1.3f);
                    Main.dust[dust].position = (Main.dust[dust].position + Projectile.Center) / 2f;
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.1f;
                    Main.dust[dust].velocity -= Projectile.velocity * (1.1f - Projectile.scale);
                    Main.dust[dust].fadeIn = (float)(125 + Projectile.owner);
                    Main.dust[dust].scale += Projectile.scale * 0.75f;
                    dustAmount++;
                }
            }
        }
    }
}

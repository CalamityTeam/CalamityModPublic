using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ShadecrystalProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.alpha = 50;
            Projectile.MaxUpdates = 3;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            if (Projectile.ai[2] < 255f)
            {
                Projectile.ai[2] += 3f;
                if (Projectile.ai[2] >= 255f)
                    Projectile.ai[2] = -255f;
            }

            Lighting.AddLight(Projectile.Center, 0.15f * Projectile.scale, 0f, 0.15f * Projectile.scale);

            Projectile.rotation += Projectile.velocity.X * 0.2f;

            if (Main.rand.NextBool(6))
            {
                int crystalDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleCrystalShard, 0f, 0f, Projectile.alpha, new Color(Math.Abs(Projectile.ai[2]), 0, 255, Projectile.alpha));
                Main.dust[crystalDust].noGravity = true;
                Main.dust[crystalDust].velocity = Projectile.velocity * 0.2f;
                Main.dust[crystalDust].scale = Projectile.scale;
            }

            float maxVelocity = 6f;
            if (Projectile.velocity.Length() < maxVelocity)
            {
                Projectile.velocity *= Projectile.ai[1];
                if (Projectile.velocity.Length() > maxVelocity)
                {
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= maxVelocity;
                }
            }
            else
                CalamityUtils.HomeInOnNPC(Projectile, false, 224f, maxVelocity * 1.25f, 30f);

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 520f)
            {
                Projectile.scale -= 0.01f;
                if (Projectile.scale <= 0.2f)
                {
                    Projectile.scale = 0.2f;
                    Projectile.Kill();
                }
                Projectile.width = (int)(6f * Projectile.scale);
                Projectile.height = (int)(6f * Projectile.scale);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;

            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 3; k++)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.PurpleCrystalShard, 0f, 0f, Projectile.alpha, new Color(Math.Abs(Projectile.ai[2]), 0, 255, Projectile.alpha));
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity = Projectile.oldVelocity * 0.5f;
                Main.dust[dust].scale = Projectile.scale;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Frostburn2, 120);

        public override Color? GetAlpha(Color lightColor) => new Color(Math.Abs(Projectile.ai[2]), 0, 255, Projectile.alpha);

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class RougeSlashSmall : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 3;
            Projectile.alpha = 255;
            Projectile.timeLeft = 300;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 3;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.75f, 0f, 0f);

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;

            if (Projectile.alpha > 0)
                Projectile.alpha -= 17;

            Projectile.localAI[1] += 1f;
            if (Projectile.localAI[1] == 12f)
            {
                Projectile.localAI[1] = 0f;
                for (int l = 0; l < 12; l++)
                {
                    Vector2 dustRotation = Vector2.UnitX * -Projectile.width / 2f;
                    dustRotation += -Vector2.UnitY.RotatedBy(l * MathHelper.Pi / 6f) * new Vector2(8f, 16f);
                    dustRotation = dustRotation.RotatedBy(Projectile.rotation - MathHelper.PiOver2);
                    int rougeDust = Dust.NewDust(Projectile.Center, 0, 0, 60, 0f, 0f, 160, default, 1f);
                    Main.dust[rougeDust].scale = 1.1f;
                    Main.dust[rougeDust].noGravity = true;
                    Main.dust[rougeDust].position = Projectile.Center + dustRotation;
                    Main.dust[rougeDust].velocity = Projectile.velocity * 0.1f;
                    Main.dust[rougeDust].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[rougeDust].position) * 1.25f;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Projectile.velocity *= 0.25f;

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}

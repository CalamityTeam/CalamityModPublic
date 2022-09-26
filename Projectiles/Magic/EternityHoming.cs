using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class EternityHoming : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eternity");
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            NPC target = Projectile.Center.ClosestNPCAt(3000f);
            if (target != null)
                Projectile.velocity = (Projectile.velocity * 7f + Projectile.SafeDirectionTo(target.Center) * 10f) / 8f;

            Projectile.ai[0] += 0.18f;
            float angle = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            float pulse = (float)Math.Sin(Projectile.ai[0]);
            float radius = 4f;
            Vector2 offset = angle.ToRotationVector2() * pulse * radius;

            Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, 264, Vector2.Zero);
            dust.color = Eternity.BlueColor;
            dust.scale = 1.4f;
            dust.noLight = true;
            dust.noGravity = true;

            dust = Dust.NewDustPerfect(Projectile.Center - offset, 264, Vector2.Zero);
            dust.color = Eternity.PinkColor;
            dust.scale = 1.4f;
            dust.noLight = true;
            dust.noGravity = true;
        }
    }
}

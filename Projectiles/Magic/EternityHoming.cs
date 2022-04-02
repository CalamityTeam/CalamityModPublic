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
            projectile.width = 2;
            projectile.height = 2;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 120;
            projectile.alpha = 255;
            projectile.magic = true;
        }

        public override void AI()
        {
            NPC target = projectile.Center.ClosestNPCAt(3000f);
            if (target != null)
                projectile.velocity = (projectile.velocity * 7f + projectile.SafeDirectionTo(target.Center) * 10f) / 8f;

            projectile.ai[0] += 0.18f;
            float angle = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            float pulse = (float)Math.Sin(projectile.ai[0]);
            float radius = 4f;
            Vector2 offset = angle.ToRotationVector2() * pulse * radius;

            Dust dust = Dust.NewDustPerfect(projectile.Center + offset, 264, Vector2.Zero);
            dust.color = Eternity.BlueColor;
            dust.scale = 1.4f;
            dust.noLight = true;
            dust.noGravity = true;

            dust = Dust.NewDustPerfect(projectile.Center - offset, 264, Vector2.Zero);
            dust.color = Eternity.PinkColor;
            dust.scale = 1.4f;
            dust.noLight = true;
            dust.noGravity = true;
        }
    }
}

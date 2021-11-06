using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class CondemnationArrowHoming : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Ranged/CondemnationArrow";
        public ref float Time => ref projectile.ai[0];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Condemning Echo");
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 9;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 45;
            projectile.scale = 0.5f;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.ranged = true;
            projectile.arrow = true;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, Color.Violet.ToVector3());
            projectile.Opacity = Utils.InverseLerp(0f, 20f, Time, true) * Utils.InverseLerp(0f, 20f, projectile.timeLeft, true);

            NPC potentialTarget = projectile.Center.ClosestNPCAt(1500f, false);
            if (potentialTarget != null)
                projectile.velocity = (projectile.velocity * 29f + projectile.SafeDirectionTo(potentialTarget.Center) * 21f) / 30f;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Time++;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color c1 = new Color(226, 40, 40, 0);
            Color c2 = new Color(205, 0, 194, 0);
            return Color.Lerp(c1, c2, (float)Math.Cos(projectile.identity * 1.41f + Main.GlobalTime * 8f) * 0.5f + 0.5f) * projectile.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            // Release a burst of magic dust on death.
            if (Main.dedServ)
                return;

            for (int i = 0; i < 10; i++)
            {
                Dust fire = Dust.NewDustPerfect(projectile.Center, 267);
                fire.velocity = Main.rand.NextVector2Circular(2f, 2f);
                fire.color = Color.Lerp(Color.Red, Color.Purple, Main.rand.NextFloat());
                fire.scale = Main.rand.NextFloat(1f, 1.1f);
                fire.noGravity = true;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => projectile.RotatingHitboxCollision(targetHitbox.TopLeft(), targetHitbox.Size());
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class CondemnationArrow : ModProjectile
    {
        public ref float Time => ref projectile.ai[0];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Condemnation Arrow");
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 9;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 90;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.arrow = true;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, Color.Violet.ToVector3());
            projectile.Opacity = Utils.InverseLerp(0f, 20f, Time, true);
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Time++;

            // Release side arrows every so often.
            if (Main.netMode != NetmodeID.MultiplayerClient && Time % 90f == 89f)
            {
                for (int i = -1; i <= 1; i += 2)
                {
                    Vector2 shootVelocity = projectile.velocity.RotatedBy(i * 0.036f);
                    Projectile.NewProjectile(projectile.Center, shootVelocity, ModContent.ProjectileType<CondemnationArrowHoming>(), projectile.damage / 2, projectile.knockBack, projectile.owner);
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color c1 = new Color(226, 40, 40, 0);
            Color c2 = new Color(205, 0, 194, 0);
            Color fadeColor = Color.Lerp(c1, c2, (float)Math.Cos(projectile.identity * 1.41f + Main.GlobalTime * 8f) * 0.5f + 0.5f);
            return Color.Lerp(lightColor, fadeColor, 0.5f) * projectile.Opacity;
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

            for (int i = 0; i < 30; i++)
            {
                Dust fire = Dust.NewDustPerfect(projectile.Center, 130);
                fire.velocity = projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(0.8f) * new Vector2(4f, 1.25f) * Main.rand.NextFloat(0.9f, 1f);
                fire.velocity = fire.velocity.RotatedBy(projectile.rotation - MathHelper.PiOver2);
                fire.velocity += projectile.velocity * 0.7f;

                fire.noGravity = true;
                fire.color = Color.Lerp(Color.White, Color.Purple, Main.rand.NextFloat());
                fire.scale = Main.rand.NextFloat(1f, 1.1f);

                fire = Dust.CloneDust(fire);
                fire.velocity = Main.rand.NextVector2Circular(3f, 3f);
                fire.velocity += projectile.velocity * 0.6f;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => projectile.RotatingHitboxCollision(targetHitbox.TopLeft(), targetHitbox.Size());
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class PrismComet : ModProjectile
    {
        public ref float Time => ref projectile.ai[0];
        public override string Texture => "CalamityMod/Projectiles/Melee/Exocomet";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Comet");
            Main.projFrames[projectile.type] = 5;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.scale = 0.86f;
            projectile.width = projectile.height = 12;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.alpha = 50;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, Color.LightSeaGreen.ToVector3());
            projectile.frameCounter++;
            if (projectile.frameCounter % 6 == 5)
                projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];

            projectile.alpha = Utils.Clamp(projectile.alpha - 25, 30, 255);

            if (projectile.alpha < 40)
                ReleaseIdleDust();

            // Home onto any targets after a short amount of time.
            if (Time >= 25f)
            {
                NPC potentialTarget = projectile.Center.ClosestNPCAt(1050f);
                if (potentialTarget != null)
                    projectile.velocity = (projectile.velocity * 12f + projectile.SafeDirectionTo(potentialTarget.Center) * 19f) / 13f;
            }
            else
                projectile.velocity *= 1.05f;

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Time++;
        }
        public void ReleaseIdleDust()
        {
            if (Main.dedServ)
                return;

            Dust prismEnergy = Dust.NewDustDirect(projectile.position - projectile.velocity * 4f, 8, 8, 107, projectile.oldVelocity.X, projectile.oldVelocity.Y, 100, new Color(0, 255, 255), 0.5f);
            prismEnergy.velocity *= -0.25f;
            prismEnergy.velocity -= projectile.velocity * 0.3f;

            prismEnergy = Dust.NewDustDirect(projectile.position - projectile.velocity * 4f, 8, 8, 107, projectile.oldVelocity.X, projectile.oldVelocity.Y, 100, new Color(0, 255, 255), 0.5f);
            prismEnergy.velocity *= -0.25f;
            prismEnergy.position -= projectile.velocity * 0.5f;
            prismEnergy.velocity -= projectile.velocity * 0.3f;
        }

        public override bool CanDamage() => Time >= 20f;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color color = Color.Honeydew * projectile.Opacity;
            color.A = 0;
            return color;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Zombie, (int)projectile.position.X, (int)projectile.position.Y, 103, 1f, 0f);
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 80);
            projectile.Damage();

            if (Main.myPlayer != projectile.owner)
                return;

            Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<PrismExplosionSmall>(), projectile.damage, 0f, projectile.owner);
        }
    }
}

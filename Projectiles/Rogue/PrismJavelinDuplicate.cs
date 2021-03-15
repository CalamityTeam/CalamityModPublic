using CalamityMod.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class PrismJavelinDuplicate : ModProjectile
    {
        public ref float Time => ref projectile.ai[0];
        public ref float Lifetime => ref projectile.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prism Javelin");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.scale = 0.66f;
            projectile.width = projectile.height = (int)(124f * projectile.scale);
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 16;
            projectile.timeLeft = 900;
            projectile.extraUpdates = 1;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            // Accelerate until at a certain speed.
            if (projectile.velocity.Length() < 20f)
                projectile.velocity *= 1.02f;

            projectile.Opacity = CalamityUtils.Convert01To010(Time / Lifetime) * 0.6f;
            if (Time >= Lifetime)
                projectile.Kill();

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Time++;
        }

        public override bool CanDamage() => projectile.alpha < 180;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (Time <= 1f)
                return false;

            Color drawColor = CalamityUtils.MulticolorLerp((Time / 35f + projectile.identity / 4f) % 1f, CalamityGlobalItem.ExoPalette);
            drawColor.A = 0;
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, drawColor, ProjectileID.Sets.TrailingMode[projectile.type]);
            return false;
        }
    }
}

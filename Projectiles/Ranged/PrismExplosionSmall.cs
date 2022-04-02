using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class PrismExplosionSmall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosion");
            Main.projFrames[projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 130;
            projectile.friendly = true;
            projectile.ignoreWater = false;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 150;
            projectile.extraUpdates = 2;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 11;
            projectile.scale = 0.5f;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, Color.White.ToVector3() * 1.15f);
            projectile.frameCounter++;
            if (projectile.frameCounter % 8 == 7)
                projectile.frame++;

            if (projectile.frame >= Main.projFrames[projectile.type])
                projectile.Kill();
            projectile.scale *= 1.0115f;
            projectile.Opacity = Utils.InverseLerp(5f, 36f, projectile.timeLeft, true);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Texture2D lightTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/PhotovisceratorLight");
            Rectangle frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            Vector2 drawPosition = projectile.Center - Main.screenPosition;
            Vector2 origin = frame.Size() * 0.5f;

            spriteBatch.Draw(texture, drawPosition, frame, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);
            return false;
        }
    }
}

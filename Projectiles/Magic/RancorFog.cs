using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class RancorFog : ModProjectile
    {
        public ref float LightPower => ref projectile.ai[0];
        public override void SetStaticDefaults() => DisplayName.SetDefault("Fog");

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 184;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.timeLeft = 210;
            projectile.hide = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            // Decide scale and initial rotation on the first frame this projectile exists.
            if (projectile.localAI[0] == 0f)
            {
                projectile.scale = Main.rand.NextFloat(1f, 1.7f);
                projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                projectile.localAI[0] = 1f;
            }

            // Calculate light power. This checks below the position of the fog to check if this fog is underground.
            // Without this, it may render over the fullblack that the game renders for obscured tiles.
            float lightPowerBelow = Lighting.GetColor((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16 + 6).ToVector3().Length() / (float)Math.Sqrt(3D);
            LightPower = MathHelper.Lerp(LightPower, lightPowerBelow, 0.15f);
            projectile.Opacity = Utils.InverseLerp(210f, 195f, projectile.timeLeft, true) * Utils.InverseLerp(0f, 90f, projectile.timeLeft, true);
            projectile.rotation += projectile.velocity.X * 0.004f;
            projectile.velocity *= 0.985f;
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindNPCsAndTiles.Add(index);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.SetBlendState(BlendState.Additive);

            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 origin = texture.Size() * 0.5f;
            Vector2 drawPosition = projectile.Center - Main.screenPosition;
            float opacity = Utils.InverseLerp(0f, 0.08f, LightPower, true) * projectile.Opacity * 0.5f;
            Color drawColor = new Color(236, 0, 68) * opacity;
            Vector2 scale = projectile.Size / texture.Size() * projectile.scale;
            spriteBatch.Draw(texture, drawPosition, null, drawColor, projectile.rotation, origin, scale, SpriteEffects.None, 0f);

            spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}

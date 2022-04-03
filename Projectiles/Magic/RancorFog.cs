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
        public ref float LightPower => ref Projectile.ai[0];
        public override void SetStaticDefaults() => DisplayName.SetDefault("Fog");

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 184;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 210;
            Projectile.hide = true;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            // Decide scale and initial rotation on the first frame this projectile exists.
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.scale = Main.rand.NextFloat(1f, 1.7f);
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                Projectile.localAI[0] = 1f;
            }

            // Calculate light power. This checks below the position of the fog to check if this fog is underground.
            // Without this, it may render over the fullblack that the game renders for obscured tiles.
            float lightPowerBelow = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16 + 6).ToVector3().Length() / (float)Math.Sqrt(3D);
            LightPower = MathHelper.Lerp(LightPower, lightPowerBelow, 0.15f);
            Projectile.Opacity = Utils.InverseLerp(210f, 195f, Projectile.timeLeft, true) * Utils.InverseLerp(0f, 90f, Projectile.timeLeft, true);
            Projectile.rotation += Projectile.velocity.X * 0.004f;
            Projectile.velocity *= 0.985f;
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindNPCsAndTiles.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            spriteBatch.SetBlendState(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = texture.Size() * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float opacity = Utils.InverseLerp(0f, 0.08f, LightPower, true) * Projectile.Opacity * 0.5f;
            Color drawColor = new Color(236, 0, 68) * opacity;
            Vector2 scale = Projectile.Size / texture.Size() * Projectile.scale;
            spriteBatch.Draw(texture, drawPosition, null, drawColor, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);

            spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}

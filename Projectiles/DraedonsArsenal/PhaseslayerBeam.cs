using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class PhaseslayerBeam : ModProjectile
    {
        public const int Lifetime = 180;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Light Blade");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 64;
            projectile.height = 120;
            projectile.scale = 0.5f;
            projectile.Size *= projectile.scale;
            
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.timeLeft = Lifetime;
            projectile.localNPCHitCooldown = 6;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            NPC potentialTarget = projectile.Center.ClosestNPCAt(1000f);
            if (potentialTarget != null && projectile.Distance(potentialTarget.Center) > 40f && projectile.timeLeft > Lifetime - 60)
                projectile.velocity = (projectile.velocity * 7f + projectile.SafeDirectionTo(potentialTarget.Center, -Vector2.UnitY) * 24f) / 8f;

            projectile.frameCounter++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D bladeTexture = ModContent.GetTexture(Texture);
            MiscShaderData distortationShader = GameShaders.Misc["CalamityMod:LightDistortion"];

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Rectangle frame = bladeTexture.Frame(1, 5, 0, projectile.frameCounter / 4 % 5);
            var drawData2 = new DrawData(bladeTexture, projectile.Center - Main.screenPosition, frame, Color.White, projectile.rotation, bladeTexture.Size() / new Vector2(1f, 5f) * 0.5f, projectile.scale * 1.2f, SpriteEffects.None, 0);
            distortationShader.Apply(drawData2);
            drawData2.Draw(spriteBatch);

            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}

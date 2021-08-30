using CalamityMod.DataStructures;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.BaseProjectiles
{
    public abstract class BaseMassiveExplosionProjectile : ModProjectile
    {
        public ScreenShakeSpot ScreenShakeSpot;
        public ref float CurrentRadius => ref projectile.ai[0];
        public ref float MaxRadius => ref projectile.ai[1];
        public virtual bool UsesScreenshake { get; } = false;
        public virtual float GetScreenshakePower(float pulseCompletionRatio) => 0f;

        public abstract int Lifetime { get; }
        public abstract Color GetCurrentExplosionColor(float pulseCompletionRatio);

        // Use the invisible projectile by default. This can be overridden in child types if desired.
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        // Declare this projectile as needing to use the UUID system if it uses screen shake spots, due to a need for said spots to reference their parent projectile.
        public sealed override void SetStaticDefaults()
        {
            SafeSetStaticDefaults();
            if (UsesScreenshake)
                ProjectileID.Sets.NeedsUUID[projectile.type] = true;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(ScreenShakeSpot.ScreenShakePower);
            writer.WriteVector2(ScreenShakeSpot.Position);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            ScreenShakeSpot = new ScreenShakeSpot(reader.ReadInt32(), reader.ReadVector2());
        }

        public override void AI()
        {
            if (UsesScreenshake)
            {
                if (projectile.localAI[0] == 0f)
                {
                    ScreenShakeSpot = new ScreenShakeSpot(0, projectile.Center);
                    projectile.localAI[0] = 1f;
                }
                ScreenShakeSpot.ScreenShakePower = GetScreenshakePower(projectile.timeLeft / (float)Lifetime);
                ScreenShakeSpot.Position = projectile.Center;
                CalamityWorld.ScreenShakeSpots[Projectile.GetByUUID(projectile.owner, projectile.whoAmI)] = ScreenShakeSpot;
            }

            // Expand outward.
            CurrentRadius = MathHelper.Lerp(CurrentRadius, MaxRadius, 0.25f);
            projectile.scale = MathHelper.Lerp(1.2f, 5f, Utils.InverseLerp(Lifetime, 0f, projectile.timeLeft, true));

            // Adjust the hitbox.
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, (int)(CurrentRadius * projectile.scale), (int)(CurrentRadius * projectile.scale));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            float pulseCompletionRatio = Utils.InverseLerp(Lifetime, 0f, projectile.timeLeft, true);
            Vector2 scale = new Vector2(1.5f, 1f);
            Vector2 drawPosition = projectile.Center - Main.screenPosition + projectile.Size * scale * 0.5f;
            Rectangle drawArea = new Rectangle(0, 0, projectile.width, projectile.height);
            Color fadeoutColor = new Color(new Vector4(1f - (float)Math.Sqrt(pulseCompletionRatio))) * projectile.Opacity * 0.7f;
            DrawData drawData = new DrawData(ModContent.GetTexture("Terraria/Misc/Perlin"), drawPosition, drawArea, fadeoutColor, projectile.rotation, projectile.Size, scale, SpriteEffects.None, 0);

            GameShaders.Misc["ForceField"].UseColor(GetCurrentExplosionColor(pulseCompletionRatio));
            GameShaders.Misc["ForceField"].Apply(drawData);
            drawData.Draw(spriteBatch);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            // Remove the explosion associated with this projectile's UUID.
            if (UsesScreenshake)
                CalamityWorld.ScreenShakeSpots.Remove(Projectile.GetByUUID(projectile.owner, projectile.whoAmI));
            SafeKill(timeLeft);
        }

        public virtual void SafeSetStaticDefaults() { }
        
        public virtual void SafeKill(int timeLeft) { }
    }
}

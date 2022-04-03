using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class DoGP1EndPortal : ModProjectile
    {
        public ref float TimeCountdown => ref Projectile.ai[1];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Portal");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 120;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60000;
            Projectile.hide = true;
        }

        public override void AI()
        {
            if (TimeCountdown > 0f)
            {
                if (TimeCountdown > 120f)
                    Projectile.scale = MathHelper.Clamp(Projectile.scale + 0.02f, 0f, 1f);
                if (TimeCountdown < 55f)
                    Projectile.scale = MathHelper.Clamp(Projectile.scale - 0.02f, 0f, 1f);
                TimeCountdown--;
            }
            else
                Projectile.scale = Utils.InverseLerp(60000f, 59945f, Projectile.timeLeft, true) * Utils.InverseLerp(60f, 115f, CalamityWorld.DoGSecondStageCountdown, true);

            if ((CalamityWorld.DoGSecondStageCountdown < 60f && TimeCountdown == 0f) || NPCs.CalamityGlobalNPC.DoGHead == -1 || TimeCountdown == 1f)
                Projectile.Kill();
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsOverWiresUI.Add(index);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.EnterShaderRegion();

            Texture2D noiseTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/VoronoiShapes");
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = noiseTexture.Size() * 0.5f;
            GameShaders.Misc["CalamityMod:DoGPortal"].UseOpacity(Projectile.scale);
            GameShaders.Misc["CalamityMod:DoGPortal"].UseColor(Color.Cyan);
            GameShaders.Misc["CalamityMod:DoGPortal"].UseSecondaryColor(Color.Fuchsia);
            GameShaders.Misc["CalamityMod:DoGPortal"].Apply();

            spriteBatch.Draw(noiseTexture, drawPosition, null, Color.White, 0f, origin, 3.5f, SpriteEffects.None, 0f);
            spriteBatch.ExitShaderRegion();

            return false;
        }
    }
}

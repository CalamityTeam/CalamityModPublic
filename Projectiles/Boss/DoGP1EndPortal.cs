using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class DoGP1EndPortal : ModProjectile
    {
        public ref float TimeCountdown => ref projectile.ai[1];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Portal");
        }

        public override void SetDefaults()
        {
			projectile.width = projectile.height = 74;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 60000;
		}

        public override void AI()
        {
            projectile.scale = Utils.InverseLerp(60000f, 59945f, projectile.timeLeft, true) * Utils.InverseLerp(60f, 115f, CalamityWorld.DoGSecondStageCountdown, true);
            if (TimeCountdown > 0f)
            {
                if (TimeCountdown > 120f)
                    projectile.scale = MathHelper.Clamp(projectile.scale + 0.02f, 0f, 1f);
                if (TimeCountdown < 55f)
                    projectile.scale = MathHelper.Clamp(projectile.scale - 0.02f, 0f, 1f);
                TimeCountdown--;
            }

            if (CalamityWorld.DoGSecondStageCountdown < 60f || NPCs.CalamityGlobalNPC.DoGHead == -1 || TimeCountdown == 1f)
                projectile.Kill();
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server && Filters.Scene["CalamityMod:DoGPortal"].IsActive())
                Filters.Scene["CalamityMod:DoGPortal"].Deactivate();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            // Manage a screen shader that acts as the projectile.
            if (Filters.Scene["CalamityMod:DoGPortal"].IsActive())
                Filters.Scene["CalamityMod:DoGPortal"].Deactivate();

            Filters.Scene.Activate("CalamityMod:DoGPortal", projectile.Center).GetShader().UseTargetPosition(projectile.Center);
            Filters.Scene["CalamityMod:DoGPortal"].GetShader().UseImage("Images/Misc/Perlin");
            Filters.Scene["CalamityMod:DoGPortal"].GetShader().UseColor(Color.Cyan);
            Filters.Scene["CalamityMod:DoGPortal"].GetShader().UseSecondaryColor(Color.Fuchsia);
            Filters.Scene["CalamityMod:DoGPortal"].GetShader().UseImage(ModContent.GetTexture("CalamityMod/ExtraTextures/VoronoiShapes"));
            Filters.Scene["CalamityMod:DoGPortal"].GetShader().UseProgress(projectile.scale);

            return false;
		}
	}
}

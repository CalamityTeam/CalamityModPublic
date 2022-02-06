using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class AresDeathBeamTelegraph : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public float TelegraphDelay
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }

        public NPC ThingToAttachTo => Main.npc.IndexInRange((int)projectile.ai[1]) ? Main.npc[(int)projectile.ai[1]] : null;

        public Vector2 OldVelocity;
		public const float deathrayTelegraphDuration_Normal = 150f;
		public const float deathrayTelegraphDuration_Expert = 120f;
		public const float deathrayTelegraphDuration_Rev = 105f;
		public const float deathrayTelegraphDuration_Death = 90f;
		public const float deathrayTelegraphDuration_Malice = 60f;
		public const float TelegraphFadeTime = 15f;
        public const float TelegraphWidth = 2400f;

        public override void SetStaticDefaults()
        {
            // Telegraph for Ares' eight-pointed-star laser beams
            DisplayName.SetDefault("Exo Overload Telegraph");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.penetrate = -1;

			// Difficulty modes
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

			projectile.timeLeft = (int)(malice ? deathrayTelegraphDuration_Malice : death ? deathrayTelegraphDuration_Death :
				revenge ? deathrayTelegraphDuration_Rev : expertMode ? deathrayTelegraphDuration_Expert : deathrayTelegraphDuration_Normal);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(OldVelocity);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            OldVelocity = reader.ReadVector2();
        }

        public override void AI()
        {
            // Determine the relative opacities for each player based on their distance.
            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;
                projectile.netUpdate = true;
            }

            // Die if the thing to attach to disappears.
            if (ThingToAttachTo is null || !ThingToAttachTo.active)
            {
                projectile.Kill();
                return;
            }

            // Set start of telegraph to the npc center.
            projectile.Center = ThingToAttachTo.Center + new Vector2(-1f, 23f) + (Vector2.Normalize(OldVelocity) * 17f);

            // Be sure to save the velocity the projectile started with. It will be set again when the telegraph is over.
            if (OldVelocity == Vector2.Zero)
            {
                OldVelocity = projectile.velocity;
                projectile.velocity = Vector2.Zero;
                projectile.netUpdate = true;
                projectile.rotation = OldVelocity.ToRotation() + MathHelper.PiOver2;
            }

            TelegraphDelay++;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			// Difficulty modes
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

			float TelegraphTotalTime = malice ? deathrayTelegraphDuration_Malice : death ? deathrayTelegraphDuration_Death :
				revenge ? deathrayTelegraphDuration_Rev : expertMode ? deathrayTelegraphDuration_Expert : deathrayTelegraphDuration_Normal;

			if (TelegraphDelay >= TelegraphTotalTime)
                return true;

            Texture2D laserTelegraph = ModContent.GetTexture("CalamityMod/ExtraTextures/LaserWallTelegraphBeam");
            float yScale = 2f;
            if (TelegraphDelay < TelegraphFadeTime)
                yScale = MathHelper.Lerp(0f, 2f, TelegraphDelay / 15f);
            if (TelegraphDelay > TelegraphTotalTime - TelegraphFadeTime)
                yScale = MathHelper.Lerp(2f, 0f, (TelegraphDelay - (TelegraphTotalTime - TelegraphFadeTime)) / 15f);

            Vector2 scaleInner = new Vector2(TelegraphWidth / laserTelegraph.Width, yScale);
            Vector2 origin = laserTelegraph.Size() * new Vector2(0f, 0.5f);
            Vector2 scaleOuter = scaleInner * new Vector2(1f, 1.6f);

            Color colorOuter = Color.Lerp(Color.Cyan, Color.LightGreen, TelegraphDelay / TelegraphTotalTime * 2f % 1f); // Iterate through light green and cyan once and then flash.
            Color colorInner = Color.Lerp(colorOuter, Color.White, 0.75f);

            colorOuter *= 0.7f;
            colorInner *= 0.7f;

            spriteBatch.Draw(laserTelegraph, projectile.Center - Main.screenPosition, null, colorInner, OldVelocity.ToRotation(), origin, scaleInner, SpriteEffects.None, 0f);
            spriteBatch.Draw(laserTelegraph, projectile.Center - Main.screenPosition, null, colorOuter, OldVelocity.ToRotation(), origin, scaleOuter, SpriteEffects.None, 0f);

            return false;
        }
    }
}

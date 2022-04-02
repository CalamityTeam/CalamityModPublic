using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
	public class ArtemisChargeTelegraph : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public NPC ThingToAttachTo => Main.npc.IndexInRange((int)projectile.ai[1]) ? Main.npc[(int)projectile.ai[1]] : null;

        public Vector2 OldVelocity;
        public PrimitiveTrail TelegraphDrawer = null;
        public const float TelegraphWidth = 2000f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Artemis Charge Telegraph");
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
            projectile.timeLeft = 45;
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
            projectile.Center = ThingToAttachTo.Center + projectile.velocity * 50f;

            // Determine opacity
            projectile.Opacity = Utils.InverseLerp(0f, 8f, projectile.timeLeft, true);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, projectile.alpha);
        }

        public Color TelegraphPrimitiveColor(float completionRatio)
        {
            float colorInterpolant = (completionRatio * 1.2f + Main.GlobalTime * 0.26f) % 1f;
            float opacity = MathHelper.Lerp(0.2f, 0.425f, projectile.Opacity) * Utils.InverseLerp(30f, 24f, projectile.timeLeft, true);
            return CalamityUtils.MulticolorLerp(colorInterpolant, Color.Orange, Color.Red, Color.Crimson, Color.Red) * opacity;
        }

        public float TelegraphPrimitiveWidth(float completionRatio)
        {
            // Used to determine the degree to which the ends of the telegraph should smoothen away.
            float endSmoothenFactor = Utils.InverseLerp(1f, 0.995f, completionRatio, true);
            return endSmoothenFactor * projectile.Opacity * 22f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (TelegraphDrawer is null)
                TelegraphDrawer = new PrimitiveTrail(TelegraphPrimitiveWidth, TelegraphPrimitiveColor, specialShader: GameShaders.Misc["CalamityMod:Flame"]);

            GameShaders.Misc["CalamityMod:Flame"].UseImage("Images/Misc/Perlin");
            GameShaders.Misc["CalamityMod:Flame"].UseSaturation(0.28f);
            Vector2[] drawPositions = new Vector2[]
            {
                projectile.Center,
                projectile.Center + projectile.velocity.SafeNormalize(Vector2.UnitY) * TelegraphWidth
            };

            TelegraphDrawer.Draw(drawPositions, projectile.Size * 0.5f - Main.screenPosition, 87);
            return false;
        }
    }
}

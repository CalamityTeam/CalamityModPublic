using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class ApolloChargeTelegraph : ModProjectile
    {
        public Vector2[] ChargePositions = new Vector2[1];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public NPC ThingToAttachTo => Main.npc.IndexInRange((int)projectile.ai[1]) ? Main.npc[(int)projectile.ai[1]] : null;

        public PrimitiveTrail TelegraphDrawer = null;
        public const float TelegraphFadeTime = 15f;
        public const float TelegraphWidth = 943.39811f; // a squared plus b squared equals c squared, dumbass

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Apollo Charge Telegraph");
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

            projectile.timeLeft = malice ? 30 : death ? 40 : revenge ? 45 : expertMode ? 50 : 60;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ChargePositions.Length);
            for (int i = 0; i < ChargePositions.Length; i++)
                writer.WriteVector2(ChargePositions[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            ChargePositions = new Vector2[reader.ReadInt32()];
            for (int i = 0; i < ChargePositions.Length; i++)
                ChargePositions[i] = reader.ReadVector2();
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

            // Difficulty modes
            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

            // Determine opacity
            float telegraphTotalTime = malice ? 30f : death ? 40f : revenge ? 45f : expertMode ? 50f : 60f;
            projectile.Opacity = Utils.InverseLerp(0f, 6f, projectile.timeLeft, true) * Utils.InverseLerp(telegraphTotalTime, telegraphTotalTime - 6f, projectile.timeLeft, true);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, projectile.alpha);
        }

        public Color TelegraphPrimitiveColor(float completionRatio)
        {
            float opacity = MathHelper.Lerp(0.38f, 1.2f, projectile.Opacity);
            opacity *= CalamityUtils.Convert01To010(completionRatio);
            opacity *= MathHelper.Lerp(0.9f, 0.2f, projectile.ai[0] / (ChargePositions.Length - 1f));
            if (completionRatio > 0.95f)
                opacity = 0.0000001f;
            return Color.Green * opacity;
        }

        public float TelegraphPrimitiveWidth(float completionRatio)
        {
            return projectile.Opacity * 15f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (TelegraphDrawer is null)
                TelegraphDrawer = new PrimitiveTrail(TelegraphPrimitiveWidth, TelegraphPrimitiveColor, PrimitiveTrail.RigidPointRetreivalFunction, GameShaders.Misc["CalamityMod:Flame"]);

            GameShaders.Misc["CalamityMod:Flame"].UseImage("Images/Misc/Perlin");
            GameShaders.Misc["CalamityMod:Flame"].UseSaturation(0.41f);

            for (int i = ChargePositions.Length - 2; i >= 0; i--)
            {
                Vector2[] positions = new Vector2[2]
                {
                    ChargePositions[i],
                    ChargePositions[i + 1]
                };

                // Stand-in variable used to differentiate between the beams.
                // It is not used anywhere else.
                projectile.ai[0] = i;

                TelegraphDrawer.Draw(positions, projectile.Size * 0.5f - Main.screenPosition, 55);
            }
            return false;
        }
    }
}

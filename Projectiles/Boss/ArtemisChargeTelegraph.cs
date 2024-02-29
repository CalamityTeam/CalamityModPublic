using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class ArtemisChargeTelegraph : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public NPC ThingToAttachTo => Main.npc.IndexInRange((int)Projectile.ai[1]) ? Main.npc[(int)Projectile.ai[1]] : null;

        public Vector2 OldVelocity;
        public const float TelegraphWidth = 2000f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 45;
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
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                Projectile.netUpdate = true;
            }

            // Die if the thing to attach to disappears.
            if (ThingToAttachTo is null || !ThingToAttachTo.active)
            {
                Projectile.Kill();
                return;
            }

            // Set start of telegraph to the npc center.
            Projectile.Center = ThingToAttachTo.Center + Projectile.velocity * 50f;

            // Determine opacity
            Projectile.Opacity = Utils.GetLerpValue(0f, 8f, Projectile.timeLeft, true);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, Projectile.alpha);
        }

        public Color TelegraphPrimitiveColor(float completionRatio)
        {
            float colorInterpolant = (completionRatio * 1.2f + Main.GlobalTimeWrappedHourly * 0.26f) % 1f;
            float opacity = MathHelper.Lerp(0.2f, 0.425f, Projectile.Opacity) * Utils.GetLerpValue(30f, 24f, Projectile.timeLeft, true);
            return CalamityUtils.MulticolorLerp(colorInterpolant, Color.Orange, Color.Red, Color.Crimson, Color.Red) * opacity;
        }

        public float TelegraphPrimitiveWidth(float completionRatio)
        {
            // Used to determine the degree to which the ends of the telegraph should smoothen away.
            float endSmoothenFactor = Utils.GetLerpValue(1f, 0.995f, completionRatio, true);
            return endSmoothenFactor * Projectile.Opacity * 22f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            GameShaders.Misc["CalamityMod:Flame"].UseImage1("Images/Misc/Perlin");
            GameShaders.Misc["CalamityMod:Flame"].UseSaturation(0.28f);
            Vector2[] drawPositions = new Vector2[]
            {
                Projectile.Center,
                Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitY) * TelegraphWidth
            };
            PrimitiveSet.Prepare(drawPositions, new(TelegraphPrimitiveWidth, TelegraphPrimitiveColor, (_) => Projectile.Size * 0.5f, shader: GameShaders.Misc["CalamityMod:Flame"]), 87);
            return false;
        }
    }
}

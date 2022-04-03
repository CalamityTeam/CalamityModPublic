using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class ThanatosBeamTelegraph : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public NPC ThingToAttachTo => Main.npc.IndexInRange((int)Projectile.ai[0]) ? Main.npc[(int)Projectile.ai[0]] : null;
        public float ConvergenceRatio => MathHelper.SmoothStep(0f, 1f, Utils.InverseLerp(25f, 120f, Time, true));
        public ref float StartingRotationalOffset => ref Projectile.ai[1];
        public ref float ConvergenceAngle => ref Projectile.localAI[0];
        public ref float Time => ref Projectile.localAI[1];
        public const int Lifetime = 180;
        public const float TelegraphWidth = 3600f;
        public const float BeamPosOffset = 16f;
        public override void SetStaticDefaults() => DisplayName.SetDefault("Gamma Disintegration Beam Telegraph");

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ConvergenceAngle);
            writer.Write(Time);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            ConvergenceAngle = reader.ReadSingle();
            Time = reader.ReadSingle();
        }

        public override void AI()
        {
            // Die if the thing to attach to disappears.
            if (ThingToAttachTo is null || !ThingToAttachTo.active || ThingToAttachTo.Calamity().newAI[0] != 2f)
            {
                Projectile.Kill();
                return;
            }

            // The direction of the host NPC.
            Vector2 hostNPCDirection = Vector2.Normalize(ThingToAttachTo.velocity);

            // Offset to move the beam forward so that it starts inside the NPC's mouth.
            float beamStartForwardsOffset = -8f;

            // Set the starting location of the beam to the center of the NPC.
            Projectile.Center = ThingToAttachTo.Center;
            // Add a fixed offset to align with the NPC's spritesheet (?)
            Projectile.position += hostNPCDirection * BeamPosOffset + new Vector2(0f, -ThingToAttachTo.gfxOffY);
            // Add the forwards offset, measured in pixels.
            Projectile.position += hostNPCDirection * beamStartForwardsOffset;

            Projectile.rotation = StartingRotationalOffset.AngleLerp(ConvergenceAngle, ConvergenceRatio) + ThingToAttachTo.velocity.ToRotation();

            Time++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D laserTelegraph = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/LaserWallTelegraphBeam");

            float verticalScale = Utils.InverseLerp(0f, 20f, Time, true) * Utils.InverseLerp(0f, 16f, Projectile.timeLeft, true) * 4f;

            Vector2 origin = laserTelegraph.Size() * new Vector2(0f, 0.5f);
            Vector2 scaleInner = new Vector2(TelegraphWidth / laserTelegraph.Width, verticalScale);
            Vector2 scaleOuter = scaleInner * new Vector2(1f, 2.2f);

            Color colorOuter = Color.Lerp(Color.Red, Color.Crimson, Time / Lifetime * 2f % 1f); // Iterate through crimson and red twice and then flash.
            colorOuter = Color.Lerp(colorOuter, Color.White, Utils.InverseLerp(40f, 0f, Projectile.timeLeft, true) * 0.8f);
            Color colorInner = Color.Lerp(colorOuter, Color.White, 0.5f);

            colorInner *= 0.85f;
            colorOuter *= 0.7f;

            spriteBatch.Draw(laserTelegraph, Projectile.Center - Main.screenPosition, null, colorOuter, Projectile.rotation, origin, scaleOuter, SpriteEffects.None, 0f);
            spriteBatch.Draw(laserTelegraph, Projectile.Center - Main.screenPosition, null, colorInner, Projectile.rotation, origin, scaleInner, SpriteEffects.None, 0f);
            return false;
        }
    }
}

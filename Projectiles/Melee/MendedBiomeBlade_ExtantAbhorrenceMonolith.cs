using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static CalamityMod.CalamityUtils;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class ExtantAbhorrenceMonolith : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/MendedBiomeBlade_ExtantAbhorrenceMonolith";
        public Player Owner => Main.player[Projectile.owner];
        public float Timer => (100f - Projectile.timeLeft) / 100f;
        public ref float Variant => ref Projectile.ai[0]; //Yes
        public ref float Size => ref Projectile.ai[1]; //Yes
        public float WaitTimer; //How long till it appears fr. GOtta remember to sync that one
        public const float BaseWidth = 90f;
        public const float BaseHeight = 420f;

        public CurveSegment StaySmall = new CurveSegment(EasingType.Linear, 0f, 0.2f, 0f);
        public CurveSegment GoBig = new CurveSegment(EasingType.SineOut, 0.25f, 0.2f, 1f);
        public CurveSegment GoNormal = new CurveSegment(EasingType.CircIn, 0.4f, 1.2f, -0.2f);
        public CurveSegment StayNormal = new CurveSegment(EasingType.Linear, 0.5f, 1f, 0f);

        internal float Width() => PiecewiseAnimation(Timer, new CurveSegment[] { StaySmall, GoBig, GoNormal, StayNormal }) * BaseWidth * Size;

        public CurveSegment Anticipate = new CurveSegment(EasingType.CircIn, 0f, 0f, 0.15f);
        public CurveSegment Overextend = new CurveSegment(EasingType.SineOut, 0.2f, 0.15f, 1f);
        public CurveSegment Unextend = new CurveSegment(EasingType.CircIn, 0.25f, 1.15f, -0.15f);
        public CurveSegment Hold = new CurveSegment(EasingType.ExpOut, 0.70f, 1f, -0.1f);
        internal float Height() => PiecewiseAnimation(Timer, new CurveSegment[] { Anticipate, Overextend, Unextend, Hold }) * BaseHeight * Size;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abhorrent Monolith");
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 70;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 102;
            Projectile.hide = true;
        }

        public override bool CanDamage() => WaitTimer <= 0;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + ((Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * Height()), Width(), ref collisionPoint);
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindNPCsAndTiles.Add(index);
        }

        public override void AI()
        {
            if (WaitTimer > 0)
            {
                Projectile.timeLeft = 102;
                WaitTimer--;
            }

            if (Projectile.timeLeft == 100)
            {
                SurfaceUp();
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                Projectile.velocity = Vector2.Zero;

                if (Size >= 1) //Big monoliths create sparkles even before sprouting up
                {

                    for (int i = 0; i < 20; i++)
                    {
                        Particle Sparkle = new CritSpark(Projectile.Center, Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(7.5f, 20f), Color.White, Main.rand.NextBool() ? Color.MediumTurquoise : Color.DarkOrange, 0.1f + Main.rand.NextFloat(0f, 1.5f), 20 + Main.rand.Next(30), 1, 3f);
                        GeneralParticleHandler.SpawnParticle(Sparkle);
                    }
                }
            }

            if (Projectile.timeLeft == 80)
            {
                Vector2 particleDirection = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch, Projectile.Center);

                for (int i = 0; i < 8; i++)
                {
                    Vector2 hitPositionDisplace = particleDirection.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(0f, 10f);
                    Vector2 flyDirection = particleDirection.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)) * Main.rand.NextFloat(5f, 15f);
                    Particle smoke = new SmallSmokeParticle(Projectile.Center + hitPositionDisplace, flyDirection, Color.Lerp(Color.DarkOrange, Color.MediumTurquoise, Main.rand.NextFloat()), new Color(130, 130, 130), Main.rand.NextFloat(2.8f, 3.6f) * Size, 165 - Main.rand.Next(30), 0.1f);
                    GeneralParticleHandler.SpawnParticle(smoke);
                }
                for (int i = 0; i < 6; i++)
                {
                    Vector2 hitPositionDisplace = particleDirection.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(10f, 30f);
                    Vector2 flyDirection = particleDirection.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4));

                    Particle Rock = new StoneDebrisParticle(Projectile.Center + hitPositionDisplace * 3, flyDirection * Main.rand.NextFloat(3f, 6f), Color.Lerp(Color.DarkSlateBlue, Color.LightSlateGray, Main.rand.NextFloat()), (1f + Main.rand.NextFloat(0f, 2.4f)) * Size, 30 + Main.rand.Next(50), 0.1f);
                    GeneralParticleHandler.SpawnParticle(Rock);
                }
            }
        }

        //Go up to the "surface" so you're not stuck in the middle of the ground like a complete moron.
        public void SurfaceUp()
        {
            for (float i = 0; i < 40; i += 0.5f)
            {
                Vector2 positionToCheck = Projectile.Center + Projectile.velocity * i;
                if (!Main.tile[(int)(positionToCheck.X / 16), (int)(positionToCheck.Y / 16)].IsTileSolid())
                {
                    Projectile.Center = Projectile.Center + Projectile.velocity * i;
                    return;
                }
            }
            Projectile.Center = Projectile.Center + Projectile.velocity * 40f;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Projectile.numHits > 0)
                damage = (int)(damage * TrueBiomeBlade.AstralAttunement_MonolithDamageFalloff);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            if (WaitTimer > 0)
                return false;

            Texture2D tex = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade_ExtantAbhorrenceMonolith");

            float drawAngle = Projectile.rotation;
            Rectangle frame = new Rectangle(0 + (int)Variant * 94, 0, 94, 420);

            Vector2 drawScale = new Vector2(Width() / BaseWidth, Height() / BaseHeight);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition - (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * 26f;
            Vector2 drawOrigin = new Vector2(frame.Width / 2f, frame.Height);

            float opacity = MathHelper.Clamp(1f - ((Timer - 0.85f) / 0.15f), 0f, 1f);

            spriteBatch.Draw(tex, drawPosition, frame, lightColor * opacity, drawAngle, drawOrigin, drawScale, 0f, 0f);

            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            if (WaitTimer > 0)
                return;

            Texture2D tex = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade_ExtantAbhorrenceMonolith_Glow");

            float drawAngle = Projectile.rotation;
            Rectangle frame = new Rectangle(0 + (int)Variant * 94, 0, 94, 420);

            Vector2 drawScale = new Vector2(Width() / BaseWidth, Height() / BaseHeight);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition - (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * 26f;
            Vector2 drawOrigin = new Vector2(frame.Width / 2f, frame.Height);

            float opacity = MathHelper.Clamp(1f - ((Timer - 0.85f) / 0.15f), 0f, 1f);

            spriteBatch.Draw(tex, drawPosition, frame, Color.White * opacity, drawAngle, drawOrigin, drawScale, 0f, 0f);
        }


        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(WaitTimer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            WaitTimer = reader.ReadSingle();
        }

    }
}

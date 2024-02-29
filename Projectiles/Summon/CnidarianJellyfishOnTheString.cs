using CalamityMod.DataStructures;
using CalamityMod.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.CalamityUtils;
using static Terraria.ModLoader.ModContent;
using CalamityMod.Graphics.Primitives;

namespace CalamityMod.Projectiles.Summon
{
    public class CnidarianJellyfishOnTheString : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public const int SegmentCount = 10;
        public const float SegmentDistance = 20;
        public static int FadeoutTime = 20;
        public static int ElectrifyTimer = 180;
        public static float ZapDamageMultiplier = 0.5f;

        //Sounds
        public static readonly SoundStyle ZapSound = SoundID.Item94 with { Volume = SoundID.Item94.Volume * 0.5f };
        public static readonly SoundStyle SlapSound = new("CalamityMod/Sounds/Custom/WetSlap", 4);

        public List<VerletSimulatedSegment> Segments;
        public Player Owner => Main.player[Projectile.owner];
        public ref float Initialized => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public Vector2 CnidarianPos => Segments[SegmentCount - 1].position;
        public float TotalChainLength => (SegmentCount - 1) * SegmentCount;

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.scale = 1.15f;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public void SetOrigin(Vector2 position)
        {
            Projectile.Center = position;
            if ((Projectile.Center - Owner.Center).Length() > 380f * Owner.whipRangeMultiplier)
                Projectile.Center = Owner.Center + (Projectile.Center - Owner.Center).SafeNormalize(Vector2.One) * 380f * Owner.whipRangeMultiplier;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvAABBCollision(targetHitbox.TopLeft(), targetHitbox.Size(), CnidarianPos - Projectile.Hitbox.Size() / 2f, Projectile.Hitbox.Size());
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public void Initialize()
        {
            //Initialize the segments
            SetOrigin(Owner.Calamity().mouseWorld);

            Segments = new List<VerletSimulatedSegment>(SegmentCount);
            for (int i = 0; i < SegmentCount; i++)
            {
                VerletSimulatedSegment segment = new VerletSimulatedSegment(Projectile.Center + Vector2.UnitY * SegmentDistance * i);
                Segments.Add(segment);
            }

            Segments[0].locked = true;

            int j = 0;
            foreach (VerletSimulatedSegment segment in Segments)
            {
                CritSpark particle = new CritSpark(segment.position, Vector2.UnitY * (-1f * j / (float)SegmentCount), Color.White, Color.Cyan, 1f, 10);
                GeneralParticleHandler.SpawnParticle(particle);
                j++;
            }


            Initialized = 1f;
            return;
        }

        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;

            if (Initialized == 0f)
                Initialize();

            if (Owner.channel)
                Projectile.timeLeft = FadeoutTime;

            SetOrigin(Projectile.Center.MoveTowards(Owner.Calamity().mouseWorld, 10f));

            SimulateSegments();

            Electrify(3, 300f);


            if ((Projectile.Center - Owner.Center).Length() > 3200f) //200 blocks
                Projectile.Kill();

            Timer++;
        }

        public void Electrify(int maxTargets, float targettingDistance)
        {
            //The volt bunnys gores could be nice to look at, but may be too much for this. Maybe for a future ghost bell upgrade?

            float timeAfterZap = MathHelper.Clamp(20 - (Timer - 20f) % ElectrifyTimer, 0, 20);
            float postZapTime = 1 - timeAfterZap / 20f;

            Lighting.AddLight(CnidarianPos, Color.DeepSkyBlue.ToVector3() * (1 - postZapTime));

            if (Timer % ElectrifyTimer == ElectrifyTimer - 1)
            {

                SoundEngine.PlaySound(ZapSound, CnidarianPos);
                int maxDust = 2 + Main.rand.Next(3);
                for (int i = 0; i < maxDust; i++)
                {
                    Dust.NewDustDirect(Projectile.Center, 0, 0, 226, -3f + Main.rand.NextFloat(0, 6f), -5f, Scale: Main.rand.NextFloat(0.2f, 1f));

                    Dust.NewDustDirect(CnidarianPos, 0, 0, 226, -4f + Main.rand.NextFloat(0, 8f), -3f, Scale: Main.rand.NextFloat(0.2f, 1f));
                }

                int[] targetArray = new int[maxTargets];
                int targetsAquired = 0;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (targetsAquired == maxTargets)
                        break;

                    if (Main.npc[i].CanBeChasedBy(Projectile))
                    {
                        if ((CnidarianPos - Main.npc[i].Center).Length() < targettingDistance)
                        {
                            targetArray[targetsAquired] = i;
                            targetsAquired++;
                        }
                    }
                }

                // If there is anything to actually shoot at, pick targets at random and fire.
                if (targetsAquired > 0)
                {
                    Vector2 velocity;

                    for (int i = 0; i < targetsAquired; i++)
                    {
                        velocity = (Main.npc[targetArray[i]].Center - CnidarianPos).SafeNormalize(Vector2.Zero) * 10f;
                        
                        for (int j = 0; j < 3; j++)
                        {
                            Color bloomColor = Main.rand.NextBool() ? (Main.rand.NextBool() ? Color.Gold : Color.Cyan) : Color.SpringGreen;
                            ElectricSpark spark = new ElectricSpark(CnidarianPos, velocity.RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(0.2f, 1.3f), Color.Gold, bloomColor, 0.5f + Main.rand.NextFloat(0.5f), 30, bloomScale: 2) ;
                            GeneralParticleHandler.SpawnParticle(spark);
                        }

                        if (Projectile.owner == Main.myPlayer)
                        {
                            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), CnidarianPos, velocity, ProjectileType<CnidarianSpark>(), (int)(Projectile.damage * ZapDamageMultiplier), Projectile.knockBack, Projectile.owner, targetArray[i], 0f);
                        }
                    }
                }
            }
        }

        public void SimulateSegments()
        {
            // TODO -- Ozzatron put this here to stop multiplayer errors.
            if (Segments is null)
            {
                Segments = new List<VerletSimulatedSegment>(SegmentCount);
                for (int i = 0; i < SegmentCount; ++i)
                    Segments[i] = new VerletSimulatedSegment(Projectile.Center, false);
            }

            Segments[0].oldPosition = Segments[0].position;
            Segments[0].position = Projectile.Center;

            Segments = VerletSimulatedSegment.SimpleSimulation(Segments, SegmentDistance);

            Projectile.netUpdate = true;
            Projectile.netSpam = 0;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Play a wet slap sound if you hit an enemy fast enough. Also make the players minions target the slapped npc.
            float centrifugalForce = Math.Clamp((Segments[SegmentCount - 1].position - Segments[SegmentCount - 1].oldPosition).Length() * 2f, 0f, 130f) / 130f;
            if (centrifugalForce > 0.2f)
            {
                SoundEngine.PlaySound(SlapSound with { Volume = SlapSound.Volume * centrifugalForce + 0.8f }, target.position);
                Owner.MinionAttackTargetNPC = target.whoAmI;
            }

        }

        //Squish animation keys
        public CurveSegment anticipation = new CurveSegment(EasingType.PolyInOut, 0f, 1f, 0.35f, 3);
        public CurveSegment contraction = new CurveSegment(EasingType.PolyOut, 0.5f, 1.35f, -0.85f, 5);
        public CurveSegment retract = new CurveSegment(EasingType.SineInOut, 0.7f, 0.5f, 0.5f);
        internal float StretchRatio() => PiecewiseAnimation(MathHelper.Clamp((Timer + 45) % ElectrifyTimer, 0, 80) / 80f, new CurveSegment[] { anticipation, contraction, retract });

        public float PrimWidthFunction(float completionRatio)
        {
            return 1.6f;
        }

        public Color PrimColorFunction(float completionRatio)
        {
            float timeAfterZap = MathHelper.Clamp( 20 - (Timer - 5 - completionRatio * 12f) % ElectrifyTimer, 0, 20);
            float postZapTime = 1 - timeAfterZap / 20f;

            Color startingColor = Color.Lerp(Color.Cyan, Color.Maroon, (float)Math.Pow(postZapTime, 2f)) * (Projectile.timeLeft / (float)FadeoutTime);
            Color endColor = Color.DarkCyan * 0f;
            return Color.Lerp(endColor, startingColor, (float)Math.Pow(completionRatio, 1.5D)) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2[] segmentPositions = Segments.Select(x => x.position).ToArray();

            PrimitiveSet.Prepare(segmentPositions, new(PrimWidthFunction, PrimColorFunction), 66);

            Texture2D tex = Request<Texture2D>(Texture).Value;

            Vector2 squish = new Vector2(2 - StretchRatio(), StretchRatio());

            float centrifugalForce = Math.Clamp((Segments[SegmentCount - 1].position - Segments[SegmentCount - 1].oldPosition).Length() * 2f - 10f, 0f, 130f) / 150f;
            Vector2 centrifugalSquish = new Vector2(1 - centrifugalForce * 0.66f, 1 + centrifugalForce * 2.2f);
            squish *= centrifugalSquish;

            float rotation = (Segments[SegmentCount - 1].position - Segments[SegmentCount - 2].position).ToRotation() - MathHelper.PiOver2;
            lightColor = Lighting.GetColor((int)CnidarianPos.X / 16, (int)CnidarianPos.Y / 16);


            Main.EntitySpriteDraw(tex, Segments[SegmentCount - 1].position - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * (Projectile.timeLeft / (float)FadeoutTime), rotation, tex.Size() / 2f, Projectile.scale * squish, SpriteEffects.None, 0);

            //Add a blue glowing overlay quickly following a zap
            float timeAfterZap = MathHelper.Clamp(20 - (Timer - 15f) % ElectrifyTimer, 0, 20);
            float postZapTime = 1 - timeAfterZap / 20f;

            if (postZapTime < 1)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                Main.EntitySpriteDraw(tex, Segments[SegmentCount - 1].position - Main.screenPosition, null, Color.DeepSkyBlue * (1 - postZapTime), rotation, tex.Size() / 2f, (Projectile.scale + postZapTime * 1.4f) * squish, 0f, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(Segments[SegmentCount - 1].position);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Vector2 sentPos = reader.ReadVector2();
            if (Segments is not null)
            {
                try
                {
                    Segments[SegmentCount - 1].position = sentPos;
                }
                catch (Exception) {
                    CalamityMod.Instance.Logger.Warn("IbanPlay Victide Cnidarian Position Netcode failed safely");
                }
            }
        }
    }
}

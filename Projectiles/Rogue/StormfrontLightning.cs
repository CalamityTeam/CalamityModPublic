using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

//Yes, this is Heavenly Gale's lightning code but modified - Shade

namespace CalamityMod.Projectiles.Rogue
{
    public class StormfrontLightning : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";

        private int noTileHitCounter = 81; //Using other projectile's methods to not collide until a certain time has passed, allowing use inside caves

        public bool HasPlayedSound;

        public const int Lifetime = 45;

        // Technically not a ratio, and more of a seed, but it is used in a 0-2pi squash
        // later in the code to get an arbitrary unit vector (which is then checked).
        public ref float BaseTurnAngleRatio => ref Projectile.ai[1];
        public ref float AccumulatedXMovementSpeeds => ref Projectile.localAI[0];
        public ref float BranchingIteration => ref Projectile.localAI[1];
        public ref float InitialVelocityAngle => ref Projectile.ai[0];
        public override string Texture => "CalamityMod/Projectiles/LightningProj";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;
        }

        public override void SetDefaults()
        {
            Projectile.width = 35;
            Projectile.height = 35;
            Projectile.alpha = 255;
            Projectile.penetrate =4;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.MaxUpdates = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = Projectile.MaxUpdates * Lifetime;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(AccumulatedXMovementSpeeds);
            writer.Write(BranchingIteration);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            AccumulatedXMovementSpeeds = reader.ReadSingle();
            BranchingIteration = reader.ReadSingle();
        }

        public override void AI()
        {
            //Pass through tiles until certain time has passed
            noTileHitCounter -= 1;
            if (noTileHitCounter == 0)
            {
                Projectile.tileCollide = true;
            }

            //dust sparks are now a feature
            if (Main.rand.NextBool(10))
            {
                int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 226, 0f, 0f, 100, new Color(Main.rand.Next(20, 100), 204, 250), 1f);
                Main.dust[d].scale += (float)Main.rand.Next(50) * 0.01f;
                Main.dust[d].noGravity = true;
                Main.dust[d].position = Projectile.Center;
            }

            // FrameCounter in this context is really just an arbitrary timer
            // which allows random turning to occur.
            Projectile.frameCounter++;
            Projectile.oldPos[1] = Projectile.oldPos[0];

            // Adjust opacity and scale.
            float adjustedTimeLife = Projectile.timeLeft / Projectile.MaxUpdates;
            Projectile.Opacity = Utils.GetLerpValue(0f, 9f, adjustedTimeLife, true) * Utils.GetLerpValue(Lifetime, Lifetime - 3f, adjustedTimeLife, true);
            Projectile.scale = Projectile.Opacity;

            // Play a strike sound on the first frame.
            if (!HasPlayedSound)
            {
                SoundEngine.PlaySound(CommonCalamitySounds.LightningSound with { Volume = 0.5f }, Main.player[Projectile.owner].Center);
                HasPlayedSound = true;
            }

            Lighting.AddLight(Projectile.Center, new Color(Main.rand.Next(20, 100), 204, 250).ToVector3());
            if (Projectile.frameCounter >= Projectile.extraUpdates * 2)
            {
                Projectile.frameCounter = 0;
                float originalSpeed = MathHelper.Min(20f, Projectile.velocity.Length());
                UnifiedRandom unifiedRandom = new((int)BaseTurnAngleRatio);
                int turnTries = 0;
                Vector2 newBaseDirection = -Vector2.UnitY;
                Vector2 potentialBaseDirection;

                do
                {
                    BaseTurnAngleRatio = unifiedRandom.Next() % 100;
                    potentialBaseDirection = (BaseTurnAngleRatio / 100f * MathHelper.TwoPi).ToRotationVector2();

                    // Ensure that the new potential direction base is always moving downwards (this is supposed to be somewhat similar to a -UnitY + RotatedBy).
                    potentialBaseDirection.Y = -Math.Abs(potentialBaseDirection.Y);

                    bool canChangeLightningDirection = true;

                    // Potential directions with very little Y speed should not be considered, because this
                    // consequentially means that the X speed would be quite large.
                    if (potentialBaseDirection.Y > -0.2f)
                        canChangeLightningDirection = false;

                    //No very fast X speeds either, its gotta hit at least consistently y'know
                    if (potentialBaseDirection.X < -0.2f || potentialBaseDirection.X > 0.2f)
                        canChangeLightningDirection = false;

                    // If the above checks were all passed, redefine the base direction of the lightning.
                    if (canChangeLightningDirection)
                        newBaseDirection = potentialBaseDirection;

                    turnTries++;
                }
                while (turnTries < 20);

                // Rotation and speed ajustment
                if (Projectile.velocity != Vector2.Zero)
                {
                    //AccumulatedXMovementSpeeds += newBaseDirection.X * (Projectile.extraUpdates + 1) * originalSpeed;
                    Projectile.velocity = newBaseDirection.RotatedBy(InitialVelocityAngle + MathHelper.PiOver2) * originalSpeed;
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                }
            }
        }

        public float PrimitiveWidthFunction(float completionRatio) => CalamityUtils.Convert01To010(completionRatio) * Projectile.scale * Projectile.width;

        public Color PrimitiveColorFunction(float completionRatio)
        {
            //Why did you have to overcomplicate it Dom, I could have just fucking used "lightningColor = new Color(Main.rand.Next(20,100), 204, 250);" and shove it in PreDraw - Shade
            float colorInterpolant = (float)Math.Sin(Projectile.identity / 3f + completionRatio * 20f + Main.GlobalTimeWrappedHourly * 1.1f) * 0.5f + 0.5f;
            Color color = CalamityUtils.MulticolorLerp(colorInterpolant, new Color(Main.rand.Next(20, 100), 204, 250), new Color(Main.rand.Next(20, 100), 204, 250));
            return color;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item93, Projectile.position);
            target.AddBuff(BuffID.Electrified, 150);
            Sparks();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            SoundEngine.PlaySound(SoundID.Item93, Projectile.position);
            target.AddBuff(BuffID.Electrified, 150);
            Sparks();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Just gonna use Dom's work on Heavenly Gale, tried to do a new one myself but I barely understand the .fx files and how they work, might revist it again eventually - Shade
            GameShaders.Misc["CalamityMod:HeavenlyGaleLightningArc"].UseImage1("Images/Misc/Perlin");
            GameShaders.Misc["CalamityMod:HeavenlyGaleLightningArc"].Apply();

            PrimitiveSet.Prepare(Projectile.oldPos, new(PrimitiveWidthFunction, PrimitiveColorFunction, (_) => Projectile.Size * 0.3f, false,
                shader: GameShaders.Misc["CalamityMod:HeavenlyGaleLightningArc"]), 10);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit53, Projectile.Center);
            Sparks();
        }
        public void Sparks()
        {
            if (Projectile.owner == Main.myPlayer)
            {
                for (int index = 0; index < 4; index++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(-100f, 10f, 200f, 0.01f);
                    //Visual sparks on death
                    Vector2 sparkS = new Vector2(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-10f, 0f));
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, sparkS, ModContent.ProjectileType<Stormfrontspark>(), 0, 0f, Projectile.owner);
                }
            }
        }
    }


}

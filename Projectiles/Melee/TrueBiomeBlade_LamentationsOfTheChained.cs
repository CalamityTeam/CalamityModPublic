using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using CalamityMod.Particles;
using CalamityMod.DataStructures;
using CalamityMod.Items.Weapons.Melee;
using Terraria.Audio;
using CalamityMod.Sounds;

namespace CalamityMod.Projectiles.Melee
{
    public class LamentationsOfTheChained : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        private NPC[] excludedTargets = new NPC[4];

        public override string Texture => "CalamityMod/Projectiles/Melee/TrueBiomeBlade_LamentationsOfTheChained";
        public Player Owner => Main.player[Projectile.owner];
        public ref float ChainSwapTimer => ref Projectile.ai[0];
        public ref float SnapCoyoteTime => ref Projectile.ai[1];
        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;

        const float MaxTangleReach = 400f; //How long can tangling vines from crits be

        //Doing your mom. Be warned that the X value of these vectors represents the angle of the whip while the Y value represents its RNG seed
        public Vector2 whip1;
        public Vector2 whip2;
        public Vector2 whip3;

        public Particle smear;
        public Particle smear2;

        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 80;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = OmegaBiomeBlade.FlailBladeAttunement_LocalIFrames;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //Cuz it intializes the hooks as pointing flat to the right of the player and we dont want that.
            if (ChainSwapTimer < Math.Max(OmegaBiomeBlade.FlailBladeAttunement_FlailTime, OmegaBiomeBlade.FlailBladeAttunement_LocalIFrames))
                return false;

            GenerateCurve(whip1.Y, whip1.X.ToRotationVector2(), out Vector2 _, out Vector2 _, out Vector2 _, out Vector2 control13, (ChainSwapTimer % OmegaBiomeBlade.FlailBladeAttunement_FlailTime) / (float)OmegaBiomeBlade.FlailBladeAttunement_FlailTime, 1);
            GenerateCurve(whip2.Y, whip2.X.ToRotationVector2(), out Vector2 _, out Vector2 _, out Vector2 _, out Vector2 control23, ((ChainSwapTimer - OmegaBiomeBlade.FlailBladeAttunement_FlailTime / 3f) % OmegaBiomeBlade.FlailBladeAttunement_FlailTime) / (float)OmegaBiomeBlade.FlailBladeAttunement_FlailTime, -1);
            GenerateCurve(whip3.Y, whip3.X.ToRotationVector2(), out Vector2 _, out Vector2 _, out Vector2 _, out Vector2 control33, ((ChainSwapTimer - OmegaBiomeBlade.FlailBladeAttunement_FlailTime * 2f / 3f) % OmegaBiomeBlade.FlailBladeAttunement_FlailTime) / (float)OmegaBiomeBlade.FlailBladeAttunement_FlailTime, -1);

            if (Collision.CheckAABBvAABBCollision(targetHitbox.TopLeft(), targetHitbox.Size(), control13 - Vector2.One * 25f, Vector2.One * 50f) ||
                Collision.CheckAABBvAABBCollision(targetHitbox.TopLeft(), targetHitbox.Size(), control23 - Vector2.One * 25f, Vector2.One * 50f) ||
                Collision.CheckAABBvAABBCollision(targetHitbox.TopLeft(), targetHitbox.Size(), control33 - Vector2.One * 25f, Vector2.One * 50f)
                )
                return true;

            //What you wanted me to calculate all 3 curves? Have straight line collisions instead
            float CollisionPoint = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.MountedCenter, control13, 20f, ref CollisionPoint) ||
                Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.MountedCenter, control13, 20f, ref CollisionPoint) ||
                Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.MountedCenter, control13, 20f, ref CollisionPoint)
                )
                return true;

            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            GenerateCurve(whip1.Y, whip1.X.ToRotationVector2(), out _, out _, out _, out Vector2 control13, (ChainSwapTimer % OmegaBiomeBlade.FlailBladeAttunement_FlailTime) / (float)OmegaBiomeBlade.FlailBladeAttunement_FlailTime, 1);
            GenerateCurve(whip2.Y, whip2.X.ToRotationVector2(), out _, out _, out _, out Vector2 control23, ((ChainSwapTimer - OmegaBiomeBlade.FlailBladeAttunement_FlailTime / 3f) % OmegaBiomeBlade.FlailBladeAttunement_FlailTime) / (float)OmegaBiomeBlade.FlailBladeAttunement_FlailTime, -1);
            GenerateCurve(whip3.Y, whip3.X.ToRotationVector2(), out _, out _, out _, out Vector2 control33, ((ChainSwapTimer - OmegaBiomeBlade.FlailBladeAttunement_FlailTime * 2f / 3f) % OmegaBiomeBlade.FlailBladeAttunement_FlailTime) / (float)OmegaBiomeBlade.FlailBladeAttunement_FlailTime, -1);
            //If you hit the enemy with the blade of the whip, guarantee a crit
            if (Collision.CheckAABBvAABBCollision(target.Hitbox.TopLeft(), target.Hitbox.Size(), control13 - Vector2.One * 25f, Vector2.One * 50f) ||
                Collision.CheckAABBvAABBCollision(target.Hitbox.TopLeft(), target.Hitbox.Size(), control23 - Vector2.One * 25f, Vector2.One * 50f) ||
                Collision.CheckAABBvAABBCollision(target.Hitbox.TopLeft(), target.Hitbox.Size(), control33 - Vector2.One * 25f, Vector2.One * 50f)
                )
            {

                if (Owner.HeldItem.ModItem is OmegaBiomeBlade sword && Main.rand.NextFloat() <= OmegaBiomeBlade.FlailBladeAttunement_BladeProc)
                    sword.OnHitProc = true;


                modifiers.SetCrit();
                for (int i = 0; i < 2; i++)
                {
                    Vector2 sparkSpeed = Owner.DirectionTo(target.Center).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)) * 9f;
                    Particle Spark = new CritSpark(target.Center, sparkSpeed, Color.White, Color.Turquoise, 1f + Main.rand.NextFloat(0, 1f), 30, 0.4f, 1f);
                    GeneralParticleHandler.SpawnParticle(Spark);
                }

                Vector2 sliceDirection = Main.rand.NextVector2CircularEdge(50f, 100f);
                Particle SliceLine = new LineVFX(target.Center - sliceDirection, sliceDirection * 2f, 0.2f, Color.PaleTurquoise * 0.6f)
                {
                    Lifetime = 6
                };
                GeneralParticleHandler.SpawnParticle(SliceLine);
            }

            else
            {
                if (Owner.HeldItem.ModItem is OmegaBiomeBlade sword && Main.rand.NextFloat() <= OmegaBiomeBlade.FlailBladeAttunement_ChainProc)
                    sword.OnHitProc = true;

                modifiers.SourceDamage *= OmegaBiomeBlade.FlailBladeAttunement_ChainDamageReduction; //If the enemy is hit with the chain of the whip, the damage gets reduced
                modifiers.DisableCrit(); //For once, we also block crits completely from the chain
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit)
            {
                SoundEngine.PlaySound(CommonCalamitySounds.SwiftSliceSound, Projectile.Center);
                excludedTargets[0] = target;
                for (int i = 0; i < 3; i++)
                {
                    NPC potentialTarget = TargetNext(target.Center, i);
                    if (potentialTarget == null)
                        break;
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ProjectileType<GhastlyChain>(), (int)(damageDone * OmegaBiomeBlade.FlailBladeAttunement_GhostChainDamageReduction), 0, Owner.whoAmI, target.whoAmI, potentialTarget.whoAmI);
                    if (proj.ModProjectile is GhastlyChain chain)
                        chain.Gravity = Main.rand.NextFloat(30f, 50f);
                }
                Array.Clear(excludedTargets, 0, 3);
            }
        }

        public NPC TargetNext(Vector2 hitFrom, int index)
        {
            float longestReach = MaxTangleReach;
            NPC target = null;
            for (int i = 0; i < Main.maxNPCs; ++i)
            {
                NPC npc = Main.npc[i];
                if (!excludedTargets.Contains(npc) && npc.CanBeChasedBy() && !npc.friendly && !npc.townNPC)
                {
                    float distance = Vector2.Distance(hitFrom, npc.Center);
                    if (distance < longestReach)
                    {
                        longestReach = distance;
                        target = npc;
                    }
                }
            }
            if (index < 3)
                excludedTargets[index + 1] = target;
            return target;
        }

        public override void AI()
        {
            if (!OwnerCanShoot)
            {
                Projectile.Kill();
                return;
            }

            Projectile.velocity = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.Zero);
            Projectile.velocity.Normalize();
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center = Owner.Center + (Projectile.velocity * 60);

            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = Projectile.whoAmI;
            Owner.ChangeDir(Math.Sign(Projectile.velocity.X));
            Owner.itemRotation = Projectile.rotation;
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= 3.14f;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Projectile.timeLeft = 2;

            if (ChainSwapTimer % OmegaBiomeBlade.FlailBladeAttunement_FlailTime == 1)
            {
                SoundEngine.PlaySound(SoundID.DD2_OgreSpit, Projectile.Center);

                Vector2 smearPos = Owner.Center + whip1.X.ToRotationVector2() * OmegaBiomeBlade.FlailBladeAttunement_Reach * Main.rand.NextFloat(0.7f, 1.1f);
                Vector2 squish = new(Main.rand.NextFloat(3f, 4f), Main.rand.NextFloat(0.5f, 1f));

                if (smear == null)
                {
                    smear = new SemiCircularSmearVFX(smearPos, Color.PowderBlue * 0.5f, whip1.X + MathHelper.Pi, Projectile.scale * 1.5f, squish)
                    {
                        Lifetime = 2
                    };
                    GeneralParticleHandler.SpawnParticle(smear);
                }
                //Update the variables of the smear
                else
                {
                    smear.Rotation = whip1.X + MathHelper.Pi; //Add some v ariation?
                    smear.Position = smearPos;
                    (smear as SemiCircularSmearVFX).Squish = squish;
                }

                smearPos = Owner.Center + whip1.X.ToRotationVector2() * OmegaBiomeBlade.FlailBladeAttunement_Reach * Main.rand.NextFloat(0.8f, 1.3f);
                squish = new Vector2(Main.rand.NextFloat(2f, 3f), Main.rand.NextFloat(0.9f, 1.4f));

                if (smear2 == null)
                {
                    smear2 = new SemiCircularSmearVFX(smearPos, Color.PowderBlue * 0.5f, whip2.X + MathHelper.Pi, Projectile.scale * 1.5f, squish)
                    {
                        Lifetime = 2
                    };
                    GeneralParticleHandler.SpawnParticle(smear2);
                }
                //Update the variables of the smear
                else
                {
                    smear2.Rotation = whip2.X + MathHelper.Pi; //Add some v ariation?
                    smear2.Position = smearPos;
                    (smear2 as SemiCircularSmearVFX).Squish = squish;
                }
            }

            if (smear != null)
            {
                smear.Rotation = smear.Rotation.AngleTowards(Projectile.velocity.ToRotation(), 0.01f);
                smear.Time = 0;
                (smear as SemiCircularSmearVFX).Squish.Y *= 0.985f;
                (smear as SemiCircularSmearVFX).Squish.X *= 1.01f;
            }
            if (smear2 != null)
            {
                smear2.Rotation = smear2.Rotation.AngleTowards(Projectile.velocity.ToRotation(), 0.01f);
                smear2.Time = 0;
                (smear2 as SemiCircularSmearVFX).Squish.Y *= 0.985f;
                (smear2 as SemiCircularSmearVFX).Squish.X *= 1.01f;
            }

            ChainSwapTimer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D handle = Request<Texture2D>("CalamityMod/Items/Weapons/Melee/OmegaBiomeBlade").Value;
            Texture2D blade = Request<Texture2D>("CalamityMod/Projectiles/Melee/TrueBiomeBlade_LamentationsOfTheChained").Value;

            CalculateChains(out Vector2[] chainPositions1, out Vector2[] chainPositions2, out Vector2[] chainPositions3);

            //float drawRotation = (projBottom - chainPositions[chainPositions.Length - 2]).ToRotation() +  MathHelper.PiOver4; //Face away from the last point of the bezier curve

            Vector2 drawPos = Projectile.Center - Projectile.velocity * 55f;
            Vector2 drawOrigin = new(0f, handle.Height);
            float drawRotation = Projectile.rotation + MathHelper.PiOver4;

            //lightColor = Lighting.GetColor((int)(projectile.Center.X / 16f), (int)(projectile.Center.Y / 16f));

            Main.EntitySpriteDraw(handle, drawPos - Main.screenPosition, null, lightColor, drawRotation, drawOrigin, Projectile.scale, 0f, 0);


            //Turn on additive blending
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            //Draw the tip of the flails here
            Texture2D flailBlade = Request<Texture2D>("CalamityMod/Projectiles/Melee/TrueBiomeBlade_LamentationsOfTheChainedFlail").Value;
            Rectangle bladeFrame = new(0, 0, 24, 48);
            Vector2 flailOrigin = new(bladeFrame.Width / 2, bladeFrame.Height); //Draw from center bottom of texture

            float flailRotation = (chainPositions1[^2] - chainPositions1[^3]).ToRotation() + MathHelper.PiOver2;
            Main.EntitySpriteDraw(flailBlade, chainPositions1[^2] - Main.screenPosition, bladeFrame, Color.White, flailRotation, flailOrigin, 1, SpriteEffects.None, 0);

            flailRotation = (chainPositions2[^2] - chainPositions2[chainPositions1.Length - 3]).ToRotation() + MathHelper.PiOver2;
            Main.EntitySpriteDraw(flailBlade, chainPositions2[^2] - Main.screenPosition, bladeFrame, Color.White, flailRotation, flailOrigin, 1, SpriteEffects.None, 0);

            flailRotation = (chainPositions3[^2] - chainPositions3[^3]).ToRotation() + MathHelper.PiOver2;
            Main.EntitySpriteDraw(flailBlade, chainPositions3[^2] - Main.screenPosition, bladeFrame, Color.White, flailRotation, flailOrigin, 1, SpriteEffects.None, 0);

            drawOrigin = new Vector2(0f, blade.Height);
            Main.EntitySpriteDraw(blade, drawPos - Main.screenPosition, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.9f, drawRotation, drawOrigin, Projectile.scale, 0f, 0);
            //Back to normal
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        //TLDR : https://media.discordapp.net/attachments/659100646397575208/933521303443501136/placeholder.gif
        private void GenerateCurve(float seed, Vector2 direction, out Vector2 control0, out Vector2 control1, out Vector2 control2, out Vector2 control3, float angleShift = 0f, float necessaryOrientation = 1f)
        {
            float randomNumber = 0.5f + ((float)Math.Sin(seed * 17.07947) + (float)Math.Sin(seed * 0.2f * 25.13274)) * 0.25f; //Ty dom for the awesome pseudo rng
            //Get new random numbers based on the same seed
            float seed1 = 0.5f + ((float)Math.Sin(randomNumber * 100f * 17.07947) + (float)Math.Sin(randomNumber * 100f * 0.2f * 25.13274)) * 0.25f;
            float seed2 = 0.5f + ((float)Math.Sin(seed1 * 50f * 17.07947) + (float)Math.Sin(seed1 * 50f * 0.2f * 25.13274)) * 0.25f;
            float seed3 = 0.5f + ((float)Math.Sin(seed2 * 17.07947) + (float)Math.Sin(seed2 * 0.2f * 25.13274)) * 0.25f;


            if ((necessaryOrientation == -1 && randomNumber >= 0.5) || (necessaryOrientation == 1 && randomNumber < 0.5))
                randomNumber = 1 - randomNumber;

            randomNumber += angleShift * 0.1f * necessaryOrientation;


            float flip = randomNumber >= 0.5 ? 1f : -1f;


            control0 = Owner.MountedCenter + direction.RotatedBy(MathHelper.ToRadians(MathHelper.Lerp(MathHelper.PiOver4 * 0.3f * flip, MathHelper.PiOver4 * flip, randomNumber))) * MathHelper.Lerp(50f, 130f, (float)Math.Sin(randomNumber * MathHelper.Pi));

            float easedShift = angleShift == 1 ? 1 : 1 - (float)Math.Pow(2, -10 * angleShift);
            float Reach = OmegaBiomeBlade.FlailBladeAttunement_Reach * (0.75f + 0.75f * seed2 - 0.05f * easedShift);
            control3 = Owner.MountedCenter + (direction * Reach).RotatedBy(MathHelper.Lerp(-0.01f * flip, 0.01f * flip, easedShift)); // The last point of the curve gets put straight at the front

            //Over the double fucking bezier rainbow
            Vector2 point1 = control3 + direction.RotatedBy(MathHelper.PiOver2) * 50;
            Vector2 point2 = control3 + direction.RotatedBy(MathHelper.PiOver2) * 50 + direction * 250;
            Vector2 point3 = control3 + direction.RotatedBy(-MathHelper.PiOver2) * 50 + direction * 250;
            Vector2 point4 = control3 + direction.RotatedBy(-MathHelper.PiOver2) * 50;
            BezierCurve curve = new(new Vector2[] { point1, point2, point3, point4 });
            control3 = curve.Evaluate(randomNumber);

            Vector2 directionFromHead = direction.RotatedBy(MathHelper.ToRadians(MathHelper.Lerp(0, 160f * flip, (float)Math.Sin(randomNumber * MathHelper.Pi)))) * MathHelper.Lerp(130f, 200f, MathHelper.Clamp((float)Math.Sin(randomNumber * MathHelper.Pi) - 0.5f, 0f, 1f) * 2f);
            control2 = control3 + directionFromHead;

            Vector2 directionFromSecondToLastPoint = Utils.SafeNormalize(directionFromHead.RotatedBy(MathHelper.Pi - MathHelper.ToRadians(MathHelper.Lerp(80f * flip, 110f * flip, (float)Math.Sin(randomNumber * MathHelper.Pi)))), Vector2.Zero) * MathHelper.Lerp(120f, 280f, (float)Math.Sin(randomNumber * MathHelper.Pi));
            control1 = control2 + directionFromSecondToLastPoint;

            control3 += Vector2.UnitX.RotatedBy(MathHelper.TwoPi * seed1) * seed3 * 30f;
            control2 += Vector2.UnitX.RotatedBy(MathHelper.TwoPi * seed2) * seed1 * 30f;
            control1 += Vector2.UnitX.RotatedBy(MathHelper.TwoPi * seed3) * seed2 * 30f;
        }

        private void CalculateChains(out Vector2[] chainPositions1, out Vector2[] chainPositions2, out Vector2[] chainPositions3)
        {

            if (ChainSwapTimer % 6 == 0 || whip1.Y == 0)
            {
                whip1.X = Projectile.velocity.RotatedBy(Main.rand.NextFloat(MathHelper.PiOver4 / 16f, -MathHelper.PiOver4 / 16f)).ToRotation(); //X is the orientation
                whip1.Y = Main.rand.NextFloat(0.2f, 100f);//Y is the "seed"

                ChainSwapTimer++;
            }

            if ((ChainSwapTimer - 2) % 6 == 0 || whip2.Y == 0)
            {
                whip2.X = Projectile.velocity.RotatedBy(Main.rand.NextFloat(MathHelper.Pi / 16f, -MathHelper.Pi / 16f)).ToRotation(); //X is the orientation
                whip2.Y = Main.rand.NextFloat(0.2f, 100f);//Y is the "seed"

                ChainSwapTimer++;
            }

            if ((ChainSwapTimer - 4) % 6 == 0 || whip3.Y == 0)
            {
                whip3.X = Projectile.velocity.RotatedBy(Main.rand.NextFloat(MathHelper.Pi / 16f, -MathHelper.Pi / 16f)).ToRotation(); //X is the orientation
                whip3.Y = Main.rand.NextFloat(0.2f, 100f);//Y is the "seed"

                ChainSwapTimer++;
            }

            GenerateCurve(whip1.Y, whip1.X.ToRotationVector2(), out Vector2 control0, out Vector2 control1, out Vector2 control2, out Vector2 control3, (ChainSwapTimer % OmegaBiomeBlade.FlailBladeAttunement_FlailTime) / (float)OmegaBiomeBlade.FlailBladeAttunement_FlailTime, 1);
            DrawChain(out chainPositions1, control0, control1, control2, control3);

            GenerateCurve(whip2.Y, whip2.X.ToRotationVector2(), out control0, out control1, out control2, out control3, ((ChainSwapTimer - OmegaBiomeBlade.FlailBladeAttunement_FlailTime / 3f) % OmegaBiomeBlade.FlailBladeAttunement_FlailTime) / (float)OmegaBiomeBlade.FlailBladeAttunement_FlailTime, -1);
            DrawChain(out chainPositions2, control0, control1, control2, control3);

            GenerateCurve(whip3.Y, whip3.X.ToRotationVector2(), out control0, out control1, out control2, out control3, ((ChainSwapTimer - OmegaBiomeBlade.FlailBladeAttunement_FlailTime * 2f / 3f) % OmegaBiomeBlade.FlailBladeAttunement_FlailTime) / (float)OmegaBiomeBlade.FlailBladeAttunement_FlailTime, -1);
            DrawChain(out chainPositions3, control0, control1, control2, control3);
        }

        private void DrawChain(out Vector2[] chainPositions, Vector2 control0, Vector2 control1, Vector2 control2, Vector2 control3)
        {

            Texture2D tex = Request<Texture2D>("CalamityMod/Projectiles/Melee/TrueBiomeBlade_LamentationsOfTheChainedFlail").Value;
            Rectangle chainFrame = new(0, 70, 24, 14);
            Rectangle guardFrame = new(0, 50, 24, 18);

            //First chain
            BezierCurve curve = new(new Vector2[] { Owner.MountedCenter, control0, control1, control2, control3 });
            int numPoints = 40;
            chainPositions = curve.GetPoints(numPoints).ToArray();

            //Draw each chain segment bar the very first one
            for (int i = 1; i < numPoints; i++)
            {
                Vector2 position = chainPositions[i];

                float rotation = (chainPositions[i] - chainPositions[i - 1]).ToRotation() + MathHelper.PiOver2; //Calculate rotation based on direction from last point
                Color chainLightColor = Lighting.GetColor((int)position.X / 16, (int)position.Y / 16); //Lighting of the position of the chain segment

                if (i < numPoints - 1)
                {
                    float yScale = Vector2.Distance(chainPositions[i], chainPositions[i - 1]) / chainFrame.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points

                    float segmentProgress = i < 30f ? i / 30f : 1 - (i - 30f) / 10f;
                    float xScale = 1f + 0.75f * (float)Math.Sin(segmentProgress * MathHelper.PiOver2);

                    Vector2 scale = new(xScale, yScale); //Remember to make it get thicker and thinner based on whatever
                    Vector2 origin = new(chainFrame.Width / 2, chainFrame.Height); //Draw from center bottom of texture

                    float chainOpacity = MathHelper.Clamp((i / (float)numPoints) * 3f, 0f, 1f);

                    Main.EntitySpriteDraw(tex, chainPositions[i] - Main.screenPosition, chainFrame, chainLightColor * chainOpacity, rotation, origin, scale, SpriteEffects.None, 0);
                }
                else
                {
                    Vector2 origin = new(guardFrame.Width / 2, guardFrame.Height); //Draw from center bottom of texture
                    Main.EntitySpriteDraw(tex, chainPositions[i] - Main.screenPosition, guardFrame, chainLightColor, rotation, origin, 1, SpriteEffects.None, 0);

                    if ((ChainSwapTimer % OmegaBiomeBlade.FlailBladeAttunement_FlailTime) == 1 && Main.rand.Next(3) == 0f)
                    {
                        Particle Flake = new SnowflakeSparkle(chainPositions[i], Vector2.Zero, Color.PaleTurquoise, Color.MediumTurquoise, 1f + Main.rand.NextFloat(0, 1f), 30, 0.4f, 0.2f);
                        GeneralParticleHandler.SpawnParticle(Flake);
                    }
                }
            }
        }


        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(whip1);
            writer.WriteVector2(whip2);
            writer.WriteVector2(whip3);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            whip1 = reader.ReadVector2();
            whip2 = reader.ReadVector2();
            whip3 = reader.ReadVector2();
        }
    }
}

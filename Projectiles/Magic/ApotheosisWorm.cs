using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ApotheosisWorm : ModProjectile
    {
        internal class Segment
        {
            // Doing a typical byte seems to cause the thing to flicker when doing super fast fades.
            // This is most likely due to under/overflows.
            internal short Alpha;
            internal float Rotation;
            internal Vector2 Center;
            internal void WriteTo(BinaryWriter writer)
            {
                writer.Write(Alpha);
                writer.Write(Rotation);
                writer.WritePackedVector2(Center);
            }

            internal void ReadFrom(BinaryReader reader)
            {
                Alpha = reader.ReadInt16();
                Rotation = reader.ReadSingle();
                Center = reader.ReadPackedVector2();
            }

            internal Segment(byte alpha, float rotation, Vector2 center)
            {
                Alpha = alpha;
                Rotation = rotation;
                Center = center;
            }
        }

        internal Vector2 PortalPosition;
        internal Segment[] Segments = new Segment[80];

        internal Player Owner => Main.player[projectile.owner];
        internal ref float Time => ref projectile.ai[0];
        internal ref float FlyAcceleration => ref projectile.ai[1];
        internal ref float JawRotation => ref projectile.localAI[1];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Devourer of Gods");
        }

        public override void SetDefaults()
        {
            projectile.width = 108;
            projectile.height = 108;
            projectile.alpha = 255;
            projectile.timeLeft = 300;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 9;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.netImportant = true;
            projectile.light = 1.5f;
        }

        // Be VERY careful with the projectile's netcode, and use syncs very sparingly.
        // Syncing will use a lot more bytes than usual.

        public override void SendExtraAI(BinaryWriter writer)
        {
            if (Segments is null || Segments[0] is null)
                InitializeSegments();

            for (int i = 0; i < Segments.Length; i++)
            {
                Segments[i].WriteTo(writer);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            if (Segments is null || Segments[0] is null)
                InitializeSegments();

            for (int i = 0; i < Segments.Length; i++)
            {
                Segments[i].ReadFrom(reader);
            }
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // Initialization.
            if (projectile.localAI[0] == 0f)
            {
                PortalPosition = projectile.Center + projectile.velocity * 7f;
                if (Main.myPlayer == projectile.owner)
                    InitializeSegments();
                projectile.localAI[0] = 1f;
            }

            // Head fade-in effects.
            if (projectile.timeLeft <= 75)
                projectile.alpha = Utils.Clamp(projectile.alpha + 40, 0, 255);
            else if (Time > 15f)
                projectile.alpha = Utils.Clamp(projectile.alpha - 40, 0, 255);

            // Update all segments of the devourer every frame.
            for (int i = 0; i < Segments.Length; i++)
                UpdateSegment(i);

            Time++;

            // Reset the jaw's rotation slowly over time.
            JawRotation = MathHelper.Lerp(JawRotation, 0f, 0.08f);

            NPC potentialTarget = projectile.Center.ClosestNPCAt(4200f);
            if (potentialTarget != null)
                AttackTarget(potentialTarget);
        }

        internal void InitializeSegments()
        {
            Vector2 directionToMouse = (Main.MouseWorld - projectile.Center).SafeNormalize(Vector2.UnitX * Owner.direction);
            projectile.rotation = directionToMouse.ToRotation() + MathHelper.PiOver2;
            for (int i = 0; i < Segments.Length; i++)
            {
                Segments[i] = new Segment(255, projectile.rotation, projectile.Center + directionToMouse / Segments.Length * i * 900f);
            }
            projectile.netUpdate = true;
        }

        internal void UpdateSegment(int segmentIndex)
        {
            float aheadSegmentRotation = segmentIndex > 0 ? Segments[segmentIndex - 1].Rotation : projectile.rotation;
            Vector2 aheadSegmentCenter = segmentIndex > 0 ? Segments[segmentIndex - 1].Center : projectile.Center;
            Vector2 offsetToAheadSegment = aheadSegmentCenter - Segments[segmentIndex].Center;

            // Individual segment fade effects depending on time.
            if (projectile.timeLeft <= 60)
            {
                int invisibleSegmentIndex = (int)MathHelper.Lerp(0f, Segments.Length, 1f - MathHelper.Clamp(projectile.timeLeft / 60f * 1.4f, 0f, 1f));
                if (segmentIndex < invisibleSegmentIndex)
                {
                    Segments[segmentIndex].Alpha += 31;
                    if (Segments[segmentIndex].Alpha > 255)
                        Segments[segmentIndex].Alpha = 255;
                }
            }

            if (Time <= 90f)
            {
                int visibleSegmentIndex = (int)MathHelper.Lerp(0f, Segments.Length, Utils.InverseLerp(15f, 70f, Time, true));
                if (segmentIndex < visibleSegmentIndex)
                {
                    Segments[segmentIndex].Alpha -= 17;
                    if (Segments[segmentIndex].Alpha < 0)
                        Segments[segmentIndex].Alpha = 0;
                }
            }

            // This variant of segment attachment incorporates rotation.
            // Given the fact that all segments will execute this code is succession, the
            // result across the entire worm will exponentially decay over each segment,
            // allowing for smooth rotations. This code is what the stardust dragon and mechworm use for their segmenting.
            if (aheadSegmentRotation != Segments[segmentIndex].Rotation)
            {
                float offsetAngle = MathHelper.WrapAngle(aheadSegmentRotation - Segments[segmentIndex].Rotation);
                offsetToAheadSegment = offsetToAheadSegment.RotatedBy(offsetAngle * 0.075f);
            }
            Segments[segmentIndex].Rotation = offsetToAheadSegment.ToRotation() + MathHelper.PiOver2;
            if (offsetToAheadSegment != Vector2.Zero)
                Segments[segmentIndex].Center = aheadSegmentCenter - offsetToAheadSegment.SafeNormalize(Vector2.Zero) * 70f;
        }

        internal void AttackTarget(NPC target)
        {
            if (Time >= 40f && projectile.alpha == 0 && Main.rand.NextBool(8))
            {
                if (Main.myPlayer == projectile.owner)
                {
                    Segment segmentToShootFrom = Segments[Main.rand.Next(Segments.Length)];
                    Vector2 shootVelocity = (target.Center - segmentToShootFrom.Center).SafeNormalize(Vector2.UnitY).RotatedByRandom(0.25f) * 18f;
                    Projectile.NewProjectile(segmentToShootFrom.Center, shootVelocity, ModContent.ProjectileType<ApotheosisEnergy>(), projectile.damage / 2, 0f, projectile.owner);
                }

                if (Main.rand.NextBool(10))
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LargeMechGaussRifle"), projectile.Center);
            }

            float idealFlyAcceleration = 0.18f;

            Vector2 destination = target.Center;
            float distanceFromDestination = projectile.Distance(destination);

            // Get a swerve effect if somewhat far from the target.
            if (projectile.Distance(destination) > 725f)
            {
                destination += (Time % 30f / 30f * MathHelper.TwoPi).ToRotationVector2() * 145f;
                distanceFromDestination = projectile.Distance(destination);
                idealFlyAcceleration *= 2.5f;
            }

            // Charge if the target is far away.
            if (distanceFromDestination > 1500f && Time > 45f)
                idealFlyAcceleration = MathHelper.Min(6f, FlyAcceleration + 1f);

            FlyAcceleration = MathHelper.Lerp(FlyAcceleration, idealFlyAcceleration, 0.3f);

            float directionToTargetOrthogonality = Vector2.Dot(projectile.velocity.SafeNormalize(Vector2.Zero), projectile.SafeDirectionTo(destination));

            // Fly towards the target if it's far.
            if (distanceFromDestination > 320f)
            {
                float speed = projectile.velocity.Length();
                if (speed < 23f)
                    speed += 0.08f;

                if (speed > 32f)
                    speed -= 0.08f;

                // Go faster if the line of sight is aiming closely at the target.
                if (directionToTargetOrthogonality < 0.85f && directionToTargetOrthogonality > 0.5f)
                    speed += 6f;

                // And go slower otherwise so that the devourer can angle towards the target more accurately.
                if (directionToTargetOrthogonality < 0.5f && directionToTargetOrthogonality > -0.7f)
                    speed -= 10f;

                speed = MathHelper.Clamp(speed, 16f, 34f);

                projectile.velocity = projectile.velocity.ToRotation().AngleTowards(projectile.AngleTo(destination), FlyAcceleration).ToRotationVector2() * speed;
            }

            // Open jaws when near the target, and snap when really close.
            if (distanceFromDestination < 200f)
                JawRotation = MathHelper.Lerp(JawRotation, MathHelper.ToRadians(-34f), 0.4f);
            else if (distanceFromDestination < 480f)
                JawRotation = MathHelper.Lerp(JawRotation, MathHelper.ToRadians(15f), 0.24f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300, true);
            target.AddBuff(ModContent.BuffType<DemonFlames>(), 300, true);
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 300, true);
            target.AddBuff(ModContent.BuffType<ExoFreeze>(), 30, true);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D headTexture = Main.projectileTexture[projectile.type];
            Vector2 drawPosition = projectile.Center - Main.screenPosition;
            Vector2 headTextureOrigin = Main.projectileTexture[projectile.type].Size() * 0.5f;
            drawPosition -= headTexture.Size() * projectile.scale * 0.5f;
            drawPosition += headTextureOrigin * projectile.scale + new Vector2(0f, 4f + projectile.gfxOffY);

            Texture2D jawTexture = ModContent.GetTexture("CalamityMod/Projectiles/Magic/ApotheosisJaw");
            Vector2 jawOrigin = jawTexture.Size() * 0.5f;

            // Segment drawing.
            Texture2D bodyTexture = ModContent.GetTexture("CalamityMod/NPCs/DevourerofGods/DevourerofGodsBodyS");
            Texture2D tailTexture = ModContent.GetTexture("CalamityMod/NPCs/DevourerofGods/DevourerofGodsTailS");

            // Not white to differentiate itself slightly from regular DoG when fighting Boss Rush.
            Color baseColor = Color.Lerp(Color.White, Color.Fuchsia, 0.15f);

            for (int i = 0; i < Segments.Length; i++)
            {
                bool isTail = i == Segments.Length - 1;
                Texture2D segmentTexture = isTail ? tailTexture : bodyTexture;
                Vector2 segmentOrigin = segmentTexture.Size() * 0.5f;
                spriteBatch.Draw(segmentTexture, Segments[i].Center - Main.screenPosition, null, baseColor * ((255 - Segments[i].Alpha) / 255f), Segments[i].Rotation, segmentOrigin, projectile.scale, SpriteEffects.None, 0f);
            }

            // Jaw drawing.
            for (int i = -1; i <= 1; i += 2)
            {
                float jawBaseOffset = 42f;
                SpriteEffects jawSpriteEffect = SpriteEffects.None;
                if (i == 1)
                    jawSpriteEffect |= SpriteEffects.FlipHorizontally;

                // X/Y offsets for the jaws depending on the angle of the jaw.
                // Has potential to act weird at unexpected angles, but that should never happen naturally.
                Vector2 jawPosition = drawPosition;
                jawPosition += Vector2.UnitX.RotatedBy(projectile.rotation + JawRotation * i) * i * (jawBaseOffset + (float)Math.Sin(JawRotation) * 24f);
                jawPosition -= Vector2.UnitY.RotatedBy(projectile.rotation) * (38f + (float)Math.Sin(JawRotation) * 30f);

                spriteBatch.Draw(jawTexture, jawPosition, null, baseColor * projectile.Opacity, projectile.rotation + JawRotation * i, jawOrigin, projectile.scale, jawSpriteEffect, 0f);
            }

            // Portal drawing at the start of the devourer's lifetime.
            // This is extremely similar to the portal created when DoG teleports.
            if (Time < 60f)
            {
                float currentFade = Utils.InverseLerp(0f, 8f, Time, true) * Utils.InverseLerp(60f, 52f, Time, true);
                currentFade *= (1f + 0.2f * (float)Math.Cos(Main.GlobalTime % 30f * MathHelper.Pi * 3f)) * 0.8f;

                Texture2D texture = ModContent.GetTexture("CalamityMod/Projectiles/StarProj");
                Vector2 drawPos = PortalPosition - Main.screenPosition;
                baseColor = new Color(150, 100, 255, 255) * projectile.Opacity;
                baseColor *= 0.5f;
                baseColor.A = 0;
                Color colorA = baseColor;
                Color colorB = baseColor * 0.5f;
                colorA *= currentFade;
                colorB *= currentFade;
                Vector2 origin = texture.Size() / 2f;
                Vector2 scale = new Vector2(4f, 10f) * projectile.Opacity * currentFade;

                spriteBatch.Draw(texture, drawPos, null, colorA, MathHelper.PiOver2, origin, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, drawPos, null, colorA, 0f, origin, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, drawPos, null, colorB, MathHelper.PiOver2, origin, scale * 0.8f, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, drawPos, null, colorB, 0f, origin, scale * 0.8f, SpriteEffects.None, 0);

                spriteBatch.Draw(texture, drawPos, null, colorA, MathHelper.PiOver2 + Main.GlobalTime * 3f * 0.25f, origin, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, drawPos, null, colorA, Main.GlobalTime * 3f * 0.25f, origin, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, drawPos, null, colorB, MathHelper.PiOver2 + Main.GlobalTime * 3f * 0.5f, origin, scale * 0.8f, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, drawPos, null, colorB, Main.GlobalTime * 3f * 0.5f, origin, scale * 0.8f, SpriteEffects.None, 0);

                spriteBatch.Draw(texture, drawPos, null, colorA, MathHelper.PiOver4, origin, scale * 0.6f, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, drawPos, null, colorA, MathHelper.PiOver4 * 3f, origin, scale * 0.6f, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, drawPos, null, colorB, MathHelper.PiOver4, origin, scale * 0.4f, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, drawPos, null, colorB, MathHelper.PiOver4 * 3f, origin, scale * 0.4f, SpriteEffects.None, 0);

                spriteBatch.Draw(texture, drawPos, null, colorA, MathHelper.PiOver4 + Main.GlobalTime * 3f * 0.75f, origin, scale * 0.6f, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, drawPos, null, colorA, MathHelper.PiOver4 * 3f + Main.GlobalTime * 3f * 0.75f, origin, scale * 0.6f, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, drawPos, null, colorB, MathHelper.PiOver4 + Main.GlobalTime * 3f, origin, scale * 0.4f, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, drawPos, null, colorB, MathHelper.PiOver4 * 3f + Main.GlobalTime * 3f, origin, scale * 0.4f, SpriteEffects.None, 0);

            }

            // Head drawing.
            spriteBatch.Draw(headTexture, drawPosition, null, baseColor * projectile.Opacity, projectile.rotation, headTextureOrigin, projectile.scale, spriteEffects, 0f);
            return false;
        }

        // Manual collision detection that incorporates segments.
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Collision.CheckAABBvAABBCollision(projectile.position, projHitbox.Size(), targetHitbox.TopLeft(), projHitbox.Size()))
                return true;

            for (int i = 0; i < Segments.Length; i++)
            {
                if (Collision.CheckAABBvAABBCollision(Segments[i].Center - projHitbox.Size() * 0.5f, projHitbox.Size(), targetHitbox.TopLeft(), projHitbox.Size()))
                    return true;
            }
            return false;
        }
    }
}

using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using CalamityMod.Graphics.Primitives;

namespace CalamityMod.Projectiles.Melee
{
    public class TaintedBladeSlasher : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public ref float SwordItemID => ref Projectile.ai[1];
        public ref float VerticalOffset => ref Projectile.localAI[0];
        public ref float Time => ref Projectile.localAI[1];
        public float AttackCompletionRatio
        {
            get
            {
                float completionRatio = 1f - Owner.itemAnimation / (float)Owner.itemAnimationMax;
                if (float.IsNaN(completionRatio) || float.IsInfinity(completionRatio))
                    completionRatio = 0f;
                return completionRatio;
            }
        }
        public Player Owner => Main.player[Projectile.owner];
        public int Variant => (int)Projectile.ai[0] % 2;

        public const float ForearmLength = 80f;
        public const float ArmLength = 108f;

        public Rectangle BladeFrame
        {
            get
            {
                Texture2D bladeTexture = TextureAssets.Item[(int)SwordItemID].Value;
                Rectangle bladeFrame = bladeTexture.Frame(1, 1, 0, 0);
                bool hasMultipleFrames = Main.itemAnimations[(int)SwordItemID] != null;
                if (hasMultipleFrames)
                    bladeFrame = Main.itemAnimations[(int)SwordItemID].GetFrame(bladeTexture);
                return bladeFrame;
            }
        }

        public Vector2 BackArmAimPosition
        {
            get
            {
                Vector2 baseAimPosition = Owner.Top + new Vector2(-Owner.direction * (ForearmLength + Math.Abs(VerticalOffset) * 0.55f), -27f);
                if (Variant == 1)
                    baseAimPosition.Y += 30f;
                if (Owner.itemAnimation == 0)
                    return baseAimPosition;

                Vector2 endSwingPosition = Owner.Center + Vector2.UnitX * Owner.direction * 510f;
                return Vector2.SmoothStep(baseAimPosition, endSwingPosition, Utils.GetLerpValue(0f, 0.67f, AttackCompletionRatio, true) * Utils.GetLerpValue(1f, 0.67f, AttackCompletionRatio, true));
            }
        }
        public Vector2 IdleMoveOffset
        {
            get
            {
                Vector2 offset = Owner.itemAnimation == 0 ? (Time / 27f).ToRotationVector2() * new Vector2(5f, 4f) : Vector2.Zero;
                if (Variant == 1)
                    offset += Vector2.UnitX * Owner.direction * (1f - AttackCompletionRatio) * 40f;
                return offset;
            }
        }
        public Vector2 FrontArmEnd
        {
            get
            {
                float backArmRotation = Owner.AngleTo(BackArmAimPosition);
                Vector2 fromArmDrawPosition = Owner.Center + backArmRotation.ToRotationVector2() * ForearmLength;
                Vector2 backOffset = (Projectile.Center - fromArmDrawPosition).SafeNormalize(Vector2.Zero) * ArmLength + IdleMoveOffset;
                return fromArmDrawPosition + backOffset;
            }
        }
        public Vector2 BladeOffsetDirection
        {
            get
            {
                float backArmRotation = Owner.AngleTo(BackArmAimPosition);
                Vector2 fromArmDrawPosition = Owner.Center + backArmRotation.ToRotationVector2() * ForearmLength;
                Vector2 backOffset = (Projectile.Center - fromArmDrawPosition).SafeNormalize(Vector2.Zero) * ArmLength + IdleMoveOffset;
                float bladeRotation = backOffset.ToRotation() - MathHelper.PiOver4 + MathHelper.Pi;
                return -(bladeRotation + MathHelper.PiOver4 + BladeRotationOffset).ToRotationVector2();
            }
        }
        public Vector2 BladeCenterPosition
        {
            get
            {
                float offsetFactor = BladeFrame.Height / 4f + 31f;
                return FrontArmEnd + BladeOffsetDirection * offsetFactor;
            }
        }
        public float BladeRotationOffset => MathHelper.Lerp(MathHelper.PiOver2, 0f, Utils.GetLerpValue(0f, 0.17f, 1f - AttackCompletionRatio, true)) * -Owner.direction;
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 70;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = TrueMeleeDamageClass.Instance;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90000;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.noEnchantmentVisuals = true;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(VerticalOffset);

        public override void ReceiveExtraAI(BinaryReader reader) => VerticalOffset = reader.ReadSingle();

        public override void AI()
        {
            CalamityPlayer.EnchantHeldItemEffects(Owner, Owner.Calamity(), Owner.ActiveItem());
            if (!Owner.Calamity().bladeArmEnchant || Owner.ActiveItem().type != SwordItemID || Owner.CCed || !Owner.active || Owner.dead)
            {
                Projectile.Kill();
                return;
            }

            if (Owner.itemAnimationMax == 0)
                Owner.itemAnimationMax = (int)(Owner.ActiveItem().useAnimation * Owner.GetAttackSpeed<MeleeDamageClass>());

            float swingOffsetAngle = MathHelper.SmoothStep(-1.87f, 3.79f, AttackCompletionRatio);

            // Offset a little bit angularly if the secondary variant to prevent complete overlap.
            if (Variant == 1)
                swingOffsetAngle -= 0.7f;
            swingOffsetAngle *= Owner.direction;
            swingOffsetAngle = MathHelper.Lerp(0f, swingOffsetAngle, Utils.GetLerpValue(0f, 0.16f, AttackCompletionRatio, true));

            if (Owner.itemAnimation == 0)
                swingOffsetAngle = 0f;

            Vector2 destination = Owner.Center;

            if (Owner.itemAnimation == 0)
            {
                destination.X += Owner.direction * (VerticalOffset * 0.6f + 180f);
                destination.Y += 330f + VerticalOffset;
                if (Variant == 1)
                    destination.Y += 540f;

                // The blade can be kept in a static location when not being swung.
                // This can result in insane damage coming from "poking" enemies.
                // To accomodate for this, the hit countdown is increased when not attacking.
                Projectile.localNPCHitCooldown = 24;
            }
            else
            {
                // Swing around in an arc as needed.
                // One arm extends a little bit further than the other.
                destination -= Vector2.UnitY.RotatedBy(swingOffsetAngle) * 600f;

                // Make a fire/swing sound.
                if (Owner.itemAnimation == 21)
                    SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot, Owner.Center);
                Projectile.localNPCHitCooldown = 3;
            }

            if (AttackCompletionRatio < 0.16f)
                Projectile.oldPos = new Vector2[Projectile.oldPos.Length];

            // Very, very quickly approach the swing destination while attacking.
            // Also gain a temporary extra update.
            if (Owner.itemAnimation > 0)
            {
                Projectile.Center = Vector2.Lerp(Projectile.Center, destination, 0.19f);
                Projectile.extraUpdates = 1;
            }

            // Idly move towards the destination.
            if (Projectile.Center != destination)
                Projectile.Center += (destination - Projectile.Center).SafeNormalize(Vector2.Zero) * MathHelper.Min(Projectile.Distance(destination), 12f + Owner.velocity.Length());

            // Ensure that the position is never too far from the destination.
            if (!Projectile.WithinRange(destination, 300f))
                Projectile.Center = destination;

            Time++;
        }

        internal float PrimitiveWidthFunction(float completionRatio) => BladeFrame.Height * 0.47f;

        internal Color PrimitiveColorFunction(float completionRatio)
        {
            float opacity = Utils.GetLerpValue(0.8f, 0.52f, completionRatio, true) * Utils.GetLerpValue(1f, 0.81f, AttackCompletionRatio, true);
            Color startingColor = Color.Lerp(Color.Red, Color.DarkRed, 0.4f);
            Color endingColor = Color.Lerp(Color.DarkRed, Color.Purple, 0.77f);
            return Color.Lerp(startingColor, endingColor, (float)Math.Pow(completionRatio, 0.37f)) * opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D forearmTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/TaintedForearm").Value;
            Texture2D armTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/TaintedArm").Value;
            Texture2D handTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/TaintedHand").Value;

            // Reflect the 1st hand variant horizontally.
            SpriteEffects handDirection = SpriteEffects.None;
            if (Variant == (Owner.direction == -1).ToInt())
            {
                handTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/TaintedHand2").Value;
                handDirection = SpriteEffects.FlipHorizontally;
            }
            Texture2D bladeTexture = TextureAssets.Item[(int)SwordItemID].Value;

            // Draw the arms.
            float backArmRotation = Owner.AngleTo(BackArmAimPosition);
            Main.EntitySpriteDraw(forearmTexture, Owner.Center - Main.screenPosition, null, Color.White, backArmRotation - MathHelper.PiOver2, Vector2.UnitX * forearmTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            Vector2 frontArmDrawPosition = Owner.Center + backArmRotation.ToRotationVector2() * ForearmLength + IdleMoveOffset;
            float frontArmRotation = Projectile.AngleFrom(frontArmDrawPosition);
            Main.EntitySpriteDraw(armTexture, frontArmDrawPosition - Main.screenPosition, null, Color.White, frontArmRotation - MathHelper.PiOver2, Vector2.UnitX * armTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            float handRotation = frontArmRotation + MathHelper.PiOver2 + MathHelper.Pi;
            float bladeRotation = handRotation - MathHelper.PiOver4 + MathHelper.Pi + BladeRotationOffset;
            if (Owner.direction == -1)
                bladeRotation += MathHelper.PiOver2;

            // Draw the trail behind the blade.
            if (Owner.itemAnimation > 0)
            {
                GameShaders.Misc["CalamityMod:FadingSolidTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/BladeTrailUVMap"));
                GameShaders.Misc["CalamityMod:FadingSolidTrail"].Shader.Parameters["shouldFlip"].SetValue((float)(Owner.direction == -1).ToInt());

                Vector2 bottom = BladeCenterPosition - BladeOffsetDirection * BladeFrame.Height * 0.5f - Main.screenPosition;
                Vector2 top = BladeCenterPosition + BladeOffsetDirection * BladeFrame.Height * 0.5f - Main.screenPosition;
                Vector2 offsetToBlade = (top - bottom).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * 5f;

                Vector2[] drawPoints = new Vector2[Projectile.oldPos.Length];
                Vector2 perpendicularDirection = BladeOffsetDirection.SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.PiOver2);
                for (int i = 1; i < drawPoints.Length; i++)
                {
                    if (Projectile.oldPos[i] == Vector2.Zero)
                        continue;

                    drawPoints[i] = Projectile.Center + perpendicularDirection.RotatedBy(i * -0.014f * Owner.direction) * i * -Owner.direction * 6f;
                }

                var leftVertexPosition = BladeCenterPosition - BladeOffsetDirection * BladeFrame.Height * 0.5f + offsetToBlade - Main.screenPosition;
                var rightVertexPosition = BladeCenterPosition + BladeOffsetDirection * BladeFrame.Height * 0.5f + offsetToBlade - Main.screenPosition;

                // Swap the ends if needed so that the trail faces the right direction.
                if (Owner.direction == -1)
                    Utils.Swap(ref leftVertexPosition, ref rightVertexPosition);

                PrimitiveSet.Prepare(drawPoints, new(PrimitiveWidthFunction, PrimitiveColorFunction, (_) => BladeCenterPosition - Projectile.position,
                    shader: GameShaders.Misc["CalamityMod:FadingSolidTrail"], initialVertexPositionsOverride: (leftVertexPosition, rightVertexPosition)), 67);
            }

            // Draw the blade.
            Vector2 bladeDrawPosition = BladeCenterPosition - Main.screenPosition;
            SpriteEffects bladeDirection = Owner.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(bladeTexture, bladeDrawPosition, BladeFrame, Color.White, bladeRotation, BladeFrame.Size() * 0.5f, Projectile.scale, bladeDirection, 0);

            // Draw the hand.
            Main.EntitySpriteDraw(handTexture, FrontArmEnd - Main.screenPosition, null, Projectile.GetAlpha(Color.White), handRotation, handTexture.Size() * 0.5f, Projectile.scale, handDirection, 0);
            return false;
        }

        // Register collision at the blade's position.
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = BladeCenterPosition - BladeOffsetDirection * BladeFrame.Height * 0.5f;
            Vector2 end = BladeCenterPosition + BladeOffsetDirection * BladeFrame.Height * 0.5f;
            float width = 60f;
            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ItemLoader.OnHitNPC(Owner.ActiveItem(), Owner, target, hit, damageDone);
            NPCLoader.OnHitByItem(target, Owner, Owner.ActiveItem(), hit, damageDone);
            PlayerLoader.OnHitNPC(Owner, target, hit, damageDone);
        }
    }
}

using System;
using System.IO;
using CalamityMod.Buffs.Summon;
using CalamityMod.InverseKinematics;
using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon.SmallAresArms
{
    public abstract class ExoskeletonCannon : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public int ShootTimer;

        public int HoverOffsetIndex => (int)Projectile.ai[0];

        public bool TargetingSomething => Projectile.ai[1] == 1f;

        public Player Owner => Main.player[Projectile.owner];

        public LimbCollection Limbs = new(new CyclicCoordinateDescentUpdateRule(0.27f, MathHelper.PiOver2), 70f, 82f);

        public static readonly Vector2[] HoverOffsetTable = new Vector2[]
        {
            new(300f, 96f),
            new(-300f, 96f),
            new(190f, -102f),
            new(-190f, -102f)
        };

        public static readonly float[] RotationalClampTable = new float[]
        {
            0.23f,
            MathHelper.Pi - 0.23f,
            0.2f,
            MathHelper.Pi - 0.2f
        };

        public virtual bool UsesSuperpredictiveness => false;

        public virtual Vector2 DrawOffset => new((OwnerRestingOffset.X > 0f).ToDirectionInt() * 6f, -6f);

        public virtual Vector2 ConnectOffset => HoverOffsetIndex >= 2 ? new((OwnerRestingOffset.X > 0f).ToDirectionInt() * 14f, -30f) : Vector2.Zero;

        public abstract int ShootRate { get; }

        public abstract float ShootSpeed { get; }

        public abstract Vector2 OwnerRestingOffset { get; }

        public abstract void ClampFirstLimbRotation(ref double limbRotation);

        public abstract void ShootAtTarget(NPC target, Vector2 shootDirection);

        public override void SetDefaults()
        {
            ShootTimer = Main.rand?.Next(ShootRate) ?? 0;
            Projectile.width = 94;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.minion = true;
            Projectile.minionSlots = AresExoskeleton.MinionSlotsPerCannon;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 900000;
            Projectile.scale = 1f;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.rotation);
            writer.Write(Limbs?.Limbs?.Length ?? 0);
            for (int i = 0; i < Limbs.Limbs.Length; i++)
                writer.Write(Limbs[i].Rotation);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.rotation = reader.ReadSingle();

            //limb count has passed from SendExtraAI
            //therefore we should read this for packet aligns sake
            _ = reader.ReadInt32();

            int limbCount = Limbs.Limbs.Length;
            for (int i = 0; i < limbCount; i++)
            {
                Limbs[i].Rotation = reader.ReadDouble();
                if (i >= 1)
                    Limbs[i].ConnectPoint = Limbs[i - 1].EndPoint;
            }
        }

        public override void AI()
        {
            // Update limbs.
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 connectPosition = Main.LocalPlayer.Center + ConnectOffset;
                connectPosition.X += (OwnerRestingOffset.X > 0f).ToDirectionInt() * Projectile.scale * 20f;
                Vector2 endPosition = Owner.Center + OwnerRestingOffset;
                endPosition += (Main.MouseWorld - endPosition) * 0.075f;

                ClampFirstLimbRotation(ref Limbs[0].Rotation);
                Limbs.Update(connectPosition, endPosition);

                Projectile.netSpam = 0;
                Projectile.netUpdate = true;
            }

            Projectile.ai[1] = 0f;
            Projectile.velocity = Vector2.Zero;
            Projectile.Center = Limbs.EndPoint;

            // Handle buffs.
            Owner.AddBuff(ModContent.BuffType<ExoskeletonCannons>(), 3600);
            if (Owner.dead)
                Owner.Calamity().AresCannons = false;
            if (Owner.Calamity().AresCannons)
                Projectile.timeLeft = 2;

            // Look at the mouse if not targetting anything.
            // If something is being targeted, look at them instead.
            float idealRotation = Main.myPlayer != Projectile.owner ? Projectile.rotation : Projectile.AngleTo(Main.MouseWorld);
            NPC potentialTarget = Projectile.Center.ClosestNPCAt(AresExoskeleton.TargetingDistance);
            if (potentialTarget != null)
            {
                Projectile.ai[1] = 1f;

                idealRotation = Projectile.AngleTo(potentialTarget.Center);
                if (UsesSuperpredictiveness)
                    idealRotation = CalamityUtils.CalculatePredictiveAimToTarget(Projectile.Center, potentialTarget, ShootSpeed).ToRotation();

                ShootTimer++;
                if (ShootTimer >= ShootRate)
                {
                    ShootAtTarget(potentialTarget, idealRotation.ToRotationVector2());
                    ShootTimer = 0;
                }
            }

            Projectile.rotation = Projectile.rotation.AngleLerp(idealRotation, 0.15f);
            Projectile.spriteDirection = (Math.Cos(Projectile.rotation) > 0f).ToDirectionInt();
        }

        public void DefaultDrawCannon(Texture2D glowmask)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = texture.Frame(2, Main.projFrames[Type], TargetingSomething.ToInt(), Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rotation = Projectile.rotation;
            if (Projectile.spriteDirection == -1)
                rotation += MathHelper.Pi;

            DrawLimbs();

            Color lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), rotation, origin, Projectile.scale, direction, 0);
            Main.EntitySpriteDraw(glowmask, drawPosition, frame, Projectile.GetAlpha(Color.White), rotation, origin, Projectile.scale, direction, 0);
        }

        public void DrawLimbs()
        {
            // Draw the arms. Third and onward arms use the otherwise unused old Ares arm segment texture.
            int frame = (int)(Main.GlobalTimeWrappedHourly * 8.1f) % 9;
            for (int i = 0; i < Limbs.Limbs.Length; i++)
            {
                float scale = Projectile.scale;
                float rotation = (float)Limbs[i].Rotation;
                Vector2 segmentOriginFactor = new(0f, 0.5f);
                SpriteEffects segmentDirection = SpriteEffects.FlipHorizontally;
                Texture2D segmentTexture;
                Texture2D glowmaskTexture;

                if (i == 0)
                {
                    segmentTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/SmallAresArms/ArmPart1").Value;
                    glowmaskTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/SmallAresArms/ArmPart1Glowmask").Value;
                }
                else
                {
                    segmentTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/SmallAresArms/ArmPart2").Value;
                    glowmaskTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/SmallAresArms/ArmPart2Glowmask").Value;
                }

                Rectangle segmentFrame = segmentTexture.Frame(1, 9, 0, frame);
                if (i <= 1)
                {
                    segmentDirection = OwnerRestingOffset.X < 0f ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    if (OwnerRestingOffset.X < 0f)
                    {
                        segmentOriginFactor.X = 1f;
                        rotation += MathHelper.Pi;
                    }
                }
                else
                    scale *= 0.67f;

                Color segmentColor = Lighting.GetColor(Limbs[i].ConnectPoint.ToTileCoordinates());
                Vector2 segmentDrawPosition = Limbs[i].ConnectPoint - Main.screenPosition;
                Main.spriteBatch.DrawLineBetter(Limbs[i].ConnectPoint, Limbs[i].EndPoint, Color.Cyan, 3f);
                Main.EntitySpriteDraw(segmentTexture, segmentDrawPosition, segmentFrame, Projectile.GetAlpha(segmentColor), rotation, segmentFrame.Size() * segmentOriginFactor, scale, segmentDirection, 0);
                if (glowmaskTexture != null)
                    Main.EntitySpriteDraw(glowmaskTexture, segmentDrawPosition, segmentFrame, Projectile.GetAlpha(Color.White), rotation, segmentFrame.Size() * segmentOriginFactor, scale, segmentDirection, 0);
            }

            // Draw the shoulders on top of everything else.
            Vector2 shoulderPosition = Limbs.ConnectPoint + DrawOffset * Projectile.scale;
            Color shoulderColor = Lighting.GetColor(Limbs.ConnectPoint.ToTileCoordinates());
            Texture2D shoulderTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/SmallAresArms/ArmTopShoulder").Value;
            SpriteEffects shoulderDirection = OwnerRestingOffset.X < 0f ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Rectangle shoulderFrame = shoulderTexture.Frame(1, 9, 0, frame);
            Main.EntitySpriteDraw(shoulderTexture, shoulderPosition - Main.screenPosition, shoulderFrame, Projectile.GetAlpha(shoulderColor), 0f, shoulderFrame.Size() * 0.5f, Projectile.scale, shoulderDirection, 0);
        }
    }
}

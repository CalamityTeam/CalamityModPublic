using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System.Collections.Generic;
using CalamityMod.DataStructures;
using System.Linq;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Projectiles.Melee
{
    public class ExobladeProj : BaseIdleHoldoutProjectile
    {
        public enum SwingState
        {
            Default,
            Swinging,
            BonkDash
        }

        public int Direction = 1;
        
        public PrimitiveTrail SlashDrawer = null;

        public SwingState CurrentState;

        public ref float ChargeTime => ref Projectile.ai[0];

        public ref float GeneralTime => ref Projectile.ai[1];

        public ref float VerticalStretchFactor => ref Projectile.localAI[0];

        public ref float SwingDirection => ref Projectile.localAI[1];

        public ref bool RMBChannel => ref (Owner.HeldItem.ModItem as Exoblade).RMBchannel;

        public bool LMBUse => Owner.altFunctionUse != 2 && !RMBChannel && Owner.itemAnimation > 0 && Main.mouseLeft;

        public const float StartingSwingRotation = -0.72f;
        public const float EndingSwingRotation = StartingSwingRotation + MathHelper.TwoPi - 0.33f;
        
        // Starts at 0.125 and reels back.
        public static CurveSegment Anticipation => new(EasingType.PolyInOut, 0f, 0.27f, -0.27f);

        public static CurveSegment SlashWait => new(EasingType.Linear, 0.37f, 0f, 0f);

        // After reeling back, a powerful slash happens.
        public static CurveSegment Slash => new(EasingType.PolyOut, 0.51f, 0f, 1f);

        public static CurveSegment HoldBladeInPlace => new(EasingType.Linear, 0.7f, 1f, 0f);

        public override string Texture => "CalamityMod/Items/Weapons/Melee/Exoblade";

        public override int AssociatedItemID => ModContent.ItemType<Exoblade>();

        public override int IntendedProjectileType => ModContent.ProjectileType<ExobladeProj>();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exoblade");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 120;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 98;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90000;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.MaxUpdates = 3;
            Projectile.noEnchantments = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Direction);
            writer.Write((int)CurrentState);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Direction = reader.ReadInt32();
            CurrentState = (SwingState)reader.ReadInt32();
        }

        public override void SafeAI()
        {
            // Handle right click behaviors.
            if (Main.myPlayer == Projectile.owner)
                Owner.Calamity().rightClickListener = true;

            if (!Owner.Calamity().mouseRight)
                RMBChannel = false;
            else
                RMBChannel = true;

            // Initialize the animation max time if necessary.
            if (Owner.itemAnimationMax == 0)
                Owner.itemAnimationMax = (int)(Owner.ActiveItem().useAnimation * Owner.GetAttackSpeed<MeleeDamageClass>());

            // Decide the current phase state of the blade.
            DecideCurrentState();

            Projectile.scale = 0f;
            Projectile.Opacity = 0f;
            switch (CurrentState)
            {
                case SwingState.Swinging:
                    DoBehavior_Swinging();
                    break;
                default:
                    Direction = Owner.direction;
                    Projectile.oldRot = new float[Projectile.oldRot.Length];
                    break;
            }

            // Glue the sword to its owner.
            Projectile.Center = Owner.RotatedRelativePoint(Owner.Center, true) - Projectile.velocity;
            Owner.heldProj = Projectile.whoAmI;

            // Decide the arm rotation for the owner.
            float armRotation = Projectile.rotation + MathHelper.Pi;
            if (Direction == -1)
                armRotation += MathHelper.PiOver2;

            if (CurrentState == SwingState.Default)
                armRotation = 0f;
            Owner.SetCompositeArmFront(Math.Abs(armRotation) > 0.01f, Player.CompositeArmStretchAmount.Full, Owner.compositeFrontArm.rotation.AngleLerp(armRotation, 0.2f));

            GeneralTime++;
        }

        public void DoBehavior_Swinging()
        {
            // Decide the swing direction.
            if (Owner.itemAnimation == Owner.itemAnimationMax)
                SwingDirection = 1f;

            float exactSwingCompletion = 1f - Owner.itemAnimation / (float)Owner.itemAnimationMax;
            float directionalSwingCompletion = exactSwingCompletion;
            
            directionalSwingCompletion = MathHelper.Clamp(directionalSwingCompletion, 0f, 1f);

            // TODO -- Make this better.
            // Hi Iban! Good luck :)
            float swingCompletion = PiecewiseAnimation(directionalSwingCompletion, Anticipation, SlashWait, Slash, HoldBladeInPlace);

            // Clear the rotation cache after enough time has passed.
            if (directionalSwingCompletion < 0.45f)
                Projectile.oldRot = new float[Projectile.oldRot.Length];

            // Decide the swing direction.
            Projectile.rotation = MathHelper.Lerp(StartingSwingRotation, EndingSwingRotation, swingCompletion);
            Projectile.rotation = Projectile.rotation.AngleLerp(MathHelper.Lerp(StartingSwingRotation, EndingSwingRotation, 0.27f), Utils.GetLerpValue(0.8f, 1f, directionalSwingCompletion, true));
            Projectile.rotation = Projectile.rotation * Direction - MathHelper.PiOver4;
            if (Direction == -1)
                Projectile.rotation -= MathHelper.PiOver2;

            VerticalStretchFactor = Utils.Remap(swingCompletion, 0f, 0.5f, 0f, 0.4f);

            // Decide the scale of the sword.
            Projectile.scale = 1f;
            if (!LMBUse)
                Projectile.scale = Utils.GetLerpValue(0f, 0.2f, exactSwingCompletion, true) * Utils.GetLerpValue(1f, 0.8f, exactSwingCompletion, true);

            // Create a bunch of homing beams.
            int beamShootRate = Projectile.MaxUpdates * 2;
            if (Main.myPlayer == Projectile.owner && Projectile.timeLeft % beamShootRate == 0 && swingCompletion > 0.2f && swingCompletion < 0.9f)
            {
                int boltDamage = Projectile.damage / 2;
                Vector2 boltVelocity = (Projectile.rotation + MathHelper.PiOver4).ToRotationVector2();
                boltVelocity = Vector2.Lerp(boltVelocity, Vector2.UnitX * Direction, 0.8f).SafeNormalize(Vector2.UnitY);
                boltVelocity *= 9f;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + boltVelocity * 5f, boltVelocity, ModContent.ProjectileType<Exobeam>(), boltDamage, Projectile.knockBack / 3f, Projectile.owner);
            }

            Projectile.Opacity = Projectile.scale;
        }

        public void DecideCurrentState()
        {
            // Reset the state of the weapon if the player stops using it. This cannot reset a dash.
            if (Owner.itemAnimation <= 0 && CurrentState != SwingState.BonkDash)
            {
                Owner.itemAnimation = 0;
                CurrentState = SwingState.Default;
            }

            // Switch to the swing state as necessary. This cannot happen while channeling or dashing.
            if (CurrentState == 0f && !RMBChannel && Owner.itemAnimation > 0)
                CurrentState = SwingState.Swinging;
            
            // Switch to the dash charge state. This can only be done while in the default state.
            if (CurrentState == 0f && Owner.itemAnimation > 0)
            {
                Owner.fallStart = (int)(Owner.position.Y / 16f);
                if (CurrentState != SwingState.BonkDash)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, Owner.Center);
                    CurrentState = SwingState.BonkDash;
                    Projectile.netUpdate = true;
                }
            }
        }

        public float SlashWidthFunction(float completionRatio) => Projectile.scale * 43.5f;

        public Color SlashColorFunction(float completionRatio) => Color.Lime * Utils.GetLerpValue(0.9f, 0.4f, completionRatio, true) * Projectile.Opacity;

        public IEnumerable<Vector2> GenerateSlashPoints()
        {
            List<Vector2> result = new();

            for (int i = 0; i < Owner.itemAnimationMax; i++)
            {
                if (Projectile.oldRot[i] == 0f)
                    continue;

                if (i >= 1 && Projectile.oldRot[i] == Projectile.oldRot[i - 1])
                    continue;

                float oldRotation = Projectile.oldRot[i] - MathHelper.PiOver4 + (SwingDirection == -1 ? -0.18f : 0.04f);
                if (Direction == -1)
                    oldRotation += MathHelper.PiOver2 - 0.2f;

                result.Add(oldRotation.ToRotationVector2() * (SlashWidthFunction(0f) + 10f));
            }

            if (result.Count < 4 || result.First() == result.Last())
                result.Clear();

            return result;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.Opacity <= 0f)
                return false;

            // Initialize the trail drawer.
            SlashDrawer ??= new(SlashWidthFunction, SlashColorFunction, null, GameShaders.Misc["CalamityMod:ExobladeSlash"]);

            // Draw the zany slash effect.
            Main.spriteBatch.EnterShaderRegion();
            GameShaders.Misc["CalamityMod:ExobladeSlash"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/VoronoiShapes"));
            GameShaders.Misc["CalamityMod:ExobladeSlash"].UseColor(new Color(105, 240, 220));
            GameShaders.Misc["CalamityMod:ExobladeSlash"].UseSecondaryColor(new Color(57, 46, 115));
            GameShaders.Misc["CalamityMod:ExobladeSlash"].Shader.Parameters["fireColor"].SetValue(new Color(242, 112, 72).ToVector3());

            // What the heck? XOR? In MY exoblade code?????
            GameShaders.Misc["CalamityMod:ExobladeSlash"].Shader.Parameters["flipped"].SetValue((SwingDirection == 1) ^ (Direction == -1));
            GameShaders.Misc["CalamityMod:ExobladeSlash"].Apply();

            if (CurrentState == SwingState.Swinging)
                SlashDrawer.Draw(GenerateSlashPoints(), Projectile.Center - Main.screenPosition, 94);

            Main.spriteBatch.EnterShaderRegion();

            var texture = ModContent.Request<Texture2D>(Texture).Value;
            CalculatePerspectiveMatricies(out Matrix viewMatrix, out Matrix projectionMatrix);
            GameShaders.Misc["CalamityMod:LinearTransformation"].UseColor(Main.hslToRgb(0.95f, 0.85f, 0.5f));
            GameShaders.Misc["CalamityMod:LinearTransformation"].UseOpacity(0f);
            GameShaders.Misc["CalamityMod:LinearTransformation"].Shader.Parameters["uWorldViewProjection"].SetValue(viewMatrix * projectionMatrix);
            GameShaders.Misc["CalamityMod:LinearTransformation"].Shader.Parameters["localMatrix"].SetValue(new Matrix()
            {
                M11 = 1f,
                M12 = 0f,
                M21 = 0f,
                M22 = 1f + VerticalStretchFactor * 0.4f,
            });
            GameShaders.Misc["CalamityMod:LinearTransformation"].Apply();

            float rotation = Projectile.rotation;
            Vector2 origin = Projectile.Size * new Vector2(0.5f, 1f);
            Vector2 drawPosition = Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.scale * 32f - Main.screenPosition;
            if (Direction == -1)
                rotation += MathHelper.Pi;

            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(Color.White), rotation, origin, Projectile.scale, Direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            Main.spriteBatch.ExitShaderRegion();

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            ItemLoader.OnHitNPC(Owner.ActiveItem(), Owner, target, damage, knockback, crit);
            NPCLoader.OnHitByItem(target, Owner, Owner.ActiveItem(), damage, knockback, crit);
            PlayerLoader.OnHitNPC(Owner, Owner.ActiveItem(), target, damage, knockback, crit);
        }
        
        public override bool? CanDamage() => CurrentState != SwingState.Default;

        public override void Kill(int timeLeft) => Owner.fullRotation = 0f;
    }
}

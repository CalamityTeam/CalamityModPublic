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
using CalamityMod.Sounds;
using CalamityMod.Items.Weapons.Typeless;
using CalamityMod.Items.Weapons.DraedonsArsenal;

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

        public PrimitiveTrail PierceAfterimageDrawer = null;

        public SwingState CurrentState;

        public ref float ChargeTime => ref Projectile.ai[0];

        public ref float GeneralTime => ref Projectile.ai[1];

        public ref float EnergyFormInterpolant => ref Projectile.localAI[0];

        public ref float SwingDirection => ref Projectile.localAI[1];

        public ref bool RMBChannel => ref (Owner.HeldItem.ModItem as Exoblade).RMBchannel;

        public bool LMBUse => Owner.altFunctionUse != 2 && !RMBChannel && Owner.itemAnimation > 0 && Main.mouseLeft;

        public const float StartingSwingRotation = -0.72f;
        public const float EndingSwingRotation = StartingSwingRotation + MathHelper.TwoPi - 0.33f;
        
        // Starts at 0.27 and reels back.
        public static CurveSegment Anticipation => new(EasingType.PolyOut, 0f, 0.27f, -0.27f, 2);

        public static CurveSegment SlashWait => new(EasingType.Linear, 0.37f, 0f, 0f);

        // After reeling back, a powerful slash happens.
        public static CurveSegment Slash => new(EasingType.PolyOut, 0.51f, 0f, 1f);

        // Manual easing code handles this, due to a 0-1 clamp within the piecewise curve function generator.
        public static CurveSegment HoldBladeInPlace => new(EasingType.PolyOut, 0.7f, 1f, 0.27f, 5);

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
            EnergyFormInterpolant = 0f;
            switch (CurrentState)
            {
                case SwingState.Swinging:
                    DoBehavior_Swinging();
                    break;
                case SwingState.BonkDash:
                    DoBehavior_BonkDash();
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
            float armRotation = Projectile.rotation + MathHelper.Pi + MathHelper.PiOver2;

            if (CurrentState == SwingState.Default)
                armRotation = 0f;
            Owner.SetCompositeArmFront(Math.Abs(armRotation) > 0.01f, Player.CompositeArmStretchAmount.Full, Owner.compositeFrontArm.rotation.AngleLerp(armRotation, 0.2f));

            GeneralTime++;
        }

        public void DoBehavior_Swinging()
        {
            // Decide the swing direction.
            if (Owner.itemAnimation == Owner.itemAnimationMax)
            {
                Direction = Owner.direction;
                SwingDirection = 1f;
            }

            if (Owner.itemAnimation == (int)(Owner.itemAnimationMax * 0.6))
                SoundEngine.PlaySound(CommonCalamitySounds.MeatySlashSound, Projectile.Center);

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
            Projectile.rotation = MathHelper.Lerp(StartingSwingRotation, EndingSwingRotation, swingCompletion) * Direction - MathHelper.PiOver4;
            if (Direction == -1)
                Projectile.rotation -= MathHelper.PiOver2;

            // Decide the scale of the sword.
            Projectile.scale = 1f;
            if (!LMBUse)
                Projectile.scale = Utils.GetLerpValue(0f, 0.2f, exactSwingCompletion, true) * Utils.GetLerpValue(1f, 0.9f, exactSwingCompletion, true);

            // Create a bunch of homing beams.
            int beamShootRate = Projectile.MaxUpdates * 2;
            if (Main.myPlayer == Projectile.owner && Projectile.timeLeft % beamShootRate == 0 && swingCompletion > 0.3f && swingCompletion < 0.9f)
            {
                int boltDamage = (int)(Projectile.damage * Exoblade.NotTrueMeleeDamagePenalty);
                Vector2 boltVelocity = (Projectile.rotation + MathHelper.PiOver4).ToRotationVector2();
                boltVelocity = Vector2.Lerp(boltVelocity, Vector2.UnitX * Direction, 0.8f).SafeNormalize(Vector2.UnitY);
                boltVelocity *= 9f;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + boltVelocity * 5f, boltVelocity, ModContent.ProjectileType<Exobeam>(), boltDamage, Projectile.knockBack / 3f, Projectile.owner);
            }

            Projectile.Opacity = Projectile.scale;
        }

        public void DoBehavior_BonkDash()
        {
            Owner.mount?.Dismount(Owner);

            // Do the dash.
            if (Owner.itemAnimation > Owner.itemAnimationMax * 0.6f)
            {
                if (Owner.itemAnimation == (int)(Owner.itemAnimationMax * 0.6f) + 1)
                    SoundEngine.PlaySound(CommonCalamitySounds.ELRFireSound, Projectile.Center);

                Projectile.oldPos = new Vector2[Projectile.oldPos.Length];
                if (Main.myPlayer == Projectile.owner)
                {
                    Direction = Owner.direction;
                    Projectile.velocity = Projectile.SafeDirectionTo(Main.MouseWorld);
                    Owner.ChangeDir((Projectile.velocity.X > 0f).ToDirectionInt());
                    Projectile.netSpam = 0;
                    Projectile.netUpdate = true;
                }
            }
            else
                Owner.velocity = Projectile.velocity * Exoblade.LungeSpeed * 1.6f;

            if (Owner.itemAnimation == 1)
            {
                Owner.itemAnimation = 0;
                DecideCurrentState();
            }

            // Decide the scale and opacty of the sword.
            Projectile.scale = 1f;
            Projectile.Opacity = 1f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 * Direction;

            float exactSwingCompletion = 1f - Owner.itemAnimation / (float)Owner.itemAnimationMax;
            EnergyFormInterpolant = Utils.GetLerpValue(0f, 0.32f, exactSwingCompletion, true) * Utils.GetLerpValue(1f, 0.85f, exactSwingCompletion, true);
        }

        public void DecideCurrentState()
        {
            // Reset the state of the weapon if the player stops using it.
            if (Owner.itemAnimation <= 0)
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
                    CurrentState = SwingState.BonkDash;
                    Projectile.netUpdate = true;
                }
            }
        }

        public float SlashWidthFunction(float completionRatio) => Projectile.scale * 43.5f;

        public Color SlashColorFunction(float completionRatio) => Color.Lime * Utils.GetLerpValue(0.9f, 0.4f, completionRatio, true) * Projectile.Opacity;

        public float PierceWidthFunction(float completionRatio) => Utils.GetLerpValue(0f, 0.2f, completionRatio, true) * Projectile.scale * 50f;

        public Color PierceColorFunction(float completionRatio) => Color.Lime * EnergyFormInterpolant * Projectile.Opacity;

        public IEnumerable<Vector2> GenerateSlashPoints()
        {
            List<Vector2> result = new();

            for (int i = 0; i < Owner.itemAnimationMax * 0.45; i++)
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

        // TODO -- Split draw logic into distinct methods.
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.Opacity <= 0f)
                return false;

            // Initialize the primitives drawers.
            SlashDrawer ??= new(SlashWidthFunction, SlashColorFunction, null, GameShaders.Misc["CalamityMod:ExobladeSlash"]);
            PierceAfterimageDrawer ??= new(PierceWidthFunction, PierceColorFunction, null, GameShaders.Misc["CalamityMod:ExobladePierce"]);

            float rotation = Projectile.rotation;
            Vector2 origin = Projectile.Size * new Vector2(0.5f, 1f);
            Vector2 drawPosition = Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.scale * 32f - Main.screenPosition;
            if (Direction == -1)
                rotation += MathHelper.Pi;
            var texture = ModContent.Request<Texture2D>(Texture).Value;
            Color bladeColor = Color.White;
            bladeColor = Color.Lerp(bladeColor, Color.Lerp(Color.Cyan, Color.Lime, 0.5f) with { A = 0 }, EnergyFormInterpolant);
            bladeColor *= Projectile.Opacity;
            Main.EntitySpriteDraw(texture, drawPosition, null, bladeColor, rotation, origin, Projectile.scale, Direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

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
                SlashDrawer.Draw(GenerateSlashPoints(), Projectile.Center - Main.screenPosition, 95);

            Main.spriteBatch.EnterShaderRegion();

            Vector2 trailOffset = (Projectile.rotation - Direction * MathHelper.PiOver4).ToRotationVector2() * 98f + Projectile.Size * 0.5f - Main.screenPosition;
            GameShaders.Misc["CalamityMod:ExobladePierce"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/EternityStreak"));
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseImage2("Images/Extra_189");
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseColor(Color.Cyan);
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseSecondaryColor(Color.Lime);
            GameShaders.Misc["CalamityMod:ExobladePierce"].Apply();
            if (CurrentState == SwingState.BonkDash)
                PierceAfterimageDrawer.Draw(Projectile.oldPos.Take(31), trailOffset, 53);
            
            Main.spriteBatch.EnterShaderRegion();

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            ItemLoader.OnHitNPC(Owner.ActiveItem(), Owner, target, damage, knockback, crit);
            NPCLoader.OnHitByItem(target, Owner, Owner.ActiveItem(), damage, knockback, crit);
            PlayerLoader.OnHitNPC(Owner, Owner.ActiveItem(), target, damage, knockback, crit);

            if (CurrentState == SwingState.BonkDash && Owner.itemAnimation < Owner.itemAnimationMax * 0.6f)
            {
                Owner.itemAnimation = 0;
                Owner.velocity = Owner.SafeDirectionTo(target.Center) * -11.5f;

                SoundEngine.PlaySound(PlasmaGrenade.ExplosionSound, target.Center);
                SoundEngine.PlaySound(YanmeisKnife.HitSound, target.Center);
                if (Main.myPlayer == Projectile.owner)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        int slash = Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Projectile.velocity * 0.1f, ModContent.ProjectileType<ExobeamSlashCreator>(), Projectile.damage * 2, 0f, Projectile.owner, target.whoAmI);
                        if (Main.projectile.IndexInRange(slash))
                            Main.projectile[slash].timeLeft -= i * 4;
                    }
                }

                DecideCurrentState();
            }
        }
        
        public override bool? CanDamage() => CurrentState != SwingState.Default;

        public override void Kill(int timeLeft) => Owner.fullRotation = 0f;
    }
}

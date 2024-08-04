using System;
using System.IO;
using CalamityMod.Effects;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class BladecrestOathswordProj : BaseIdleHoldoutProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<BladecrestOathsword>();
        public enum SwingState : byte
        {
            Default,
            Swinging
        }

        public int Direction = 1;
        public SwingState CurrentState;
        public ref float SwingDirection => ref Projectile.ai[0];
        public ref float BladeHorizontalFactor => ref Projectile.ai[1];
        public ref float PostSwingRepositionDelay => ref Projectile.localAI[0];
        public ref float ChargePower => ref Projectile.localAI[1];

        public override string Texture => "CalamityMod/Items/Weapons/Melee/BladecrestOathsword";
        public override int AssociatedItemID => ModContent.ItemType<BladecrestOathsword>();
        public override int IntendedProjectileType => ModContent.ProjectileType<BladecrestOathswordProj>();
        public override bool? CanDamage() => CurrentState != 0; //Could also disable the damage during the channel state,

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 56;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90000;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.noEnchantmentVisuals = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Direction);
            writer.Write(PostSwingRepositionDelay);
            writer.Write((byte)CurrentState);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Direction = reader.ReadInt32();
            PostSwingRepositionDelay = reader.ReadSingle();
            CurrentState = (SwingState)reader.ReadByte();
        }

        public override void SafeAI()
        {
            // Initialize the animation max time if necessary.
            if (Owner.itemAnimationMax == 0)
                Owner.itemAnimationMax = (int)(Owner.ActiveItem().useAnimation * Owner.GetAttackSpeed<MeleeDamageClass>());

            // Decide the current phase state of the blade.
            DecideCurrentState();

            Direction = Owner.direction;

            //Set the owner's arm to rotate with the blade
            if (Owner.itemAnimation > 0)
            {
                float armPointingDirection = Projectile.rotation + MathHelper.ToRadians(240);
                Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armPointingDirection);
            }

            // Glue the sword to its owner.
            Projectile.Opacity = 1f;
            Projectile.position = Owner.RotatedRelativePoint(Owner.MountedCenter, true) - Projectile.Size * 0.5f + Vector2.UnitY * Owner.gfxOffY;

            float swingSpeedInterpolant = 0.27f;
            float swingCompletion = 1f - Owner.itemAnimation / (float)Owner.itemAnimationMax;
            float unchangedSwingCompletion = swingCompletion;
            if (SwingDirection == -1f)
                swingCompletion = 1f - swingCompletion;

            // Calculate the direction of the blade.
            Vector3 aimDirection3D = Vector3.Transform(Vector3.UnitX, Matrix.CreateRotationZ(MathHelper.PiOver2 * -Direction));

            if ((swingCompletion > 0f && swingCompletion < 1f) || PostSwingRepositionDelay > 0f)
            {
                swingSpeedInterpolant = MathHelper.Lerp(swingSpeedInterpolant, 1f, Utils.GetLerpValue(0f, 0.2f, swingCompletion, true));

                float horizontalAngle = MathHelper.Lerp(-1.2f, 2.6f, (float)Math.Pow(MathHelper.SmoothStep(0f, 1f, swingCompletion), 3D)) * Direction;
                Matrix offsetLinearTransformation = Matrix.CreateRotationZ(horizontalAngle);

                aimDirection3D = Vector3.Transform(Vector3.UnitX, offsetLinearTransformation);
            }

            // Disable the holdout effect. This was decided after indeciciveness with what would be best.
            else
                Projectile.Opacity = 0f;

            // Determine the horizontal stretch offset of the blade. This is used in matrix math below to create 2.5D visuals.
            BladeHorizontalFactor = MathHelper.Lerp(1f, 1.25f, (aimDirection3D.X * 0.5f + 0.5f) * Utils.GetLerpValue(1f, 0.8f, unchangedSwingCompletion, true));

            float baseRotation = new Vector2(aimDirection3D.X, aimDirection3D.Y).ToRotation();

            // Hold the blade upwards if not firing.
            if (CurrentState == SwingState.Swinging)
                PostSwingRepositionDelay = 10f;
            else if (PostSwingRepositionDelay > 0f)
                PostSwingRepositionDelay--;

            float idealRotation = baseRotation;

            Owner.itemRotation = 0f;
            Owner.heldProj = Projectile.whoAmI;

            idealRotation += MathHelper.PiOver4;
            if (Direction == -1)
                idealRotation += MathHelper.Pi;

            // Define rotation.
            Projectile.rotation = Projectile.rotation.AngleTowards(idealRotation, swingSpeedInterpolant * 0.45f).AngleLerp(idealRotation, swingSpeedInterpolant * 0.2f);

            // Offset the blade so that the handle is attached to the owner's hand.
            Vector2 bladeOffset = (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * Projectile.width * 0.65f;
            bladeOffset += Vector2.UnitY * Owner.gfxOffY;
            Projectile.position += bladeOffset;

            // Create demon magic dust along the blade when swinging, as well as demon blood scythes.
            if (PostSwingRepositionDelay >= 10f && CurrentState == SwingState.Swinging)
            {
                for (int i = 0; i < 2; i++)
                {
                    Dust shadowflame = Dust.NewDustPerfect(Projectile.Center + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * Projectile.width * Main.rand.NextFloat(0.4f), 27);
                    shadowflame.velocity = (Projectile.oldRot[0] - Projectile.oldRot[1]).ToRotationVector2() * Main.rand.NextFloat(4f, 10f);
                    shadowflame.velocity += (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 7.5f;
                    shadowflame.scale = Main.rand.NextFloat(1f, 1.5f);
                    shadowflame.fadeIn = 0.5f;
                    shadowflame.noGravity = true;
                }

                if (Main.myPlayer == Projectile.owner && Owner.itemAnimation % 5 == 3 && Owner.itemAnimation < Owner.itemAnimationMax - 3)
                {
                    Vector2 bloodScytheShootVelocity = (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2();
                    bloodScytheShootVelocity.Y *= 0.04f;
                    bloodScytheShootVelocity = bloodScytheShootVelocity.SafeNormalize(Vector2.UnitY) * 50f;
                    Vector2 bloodScytheSpawnPosition = Projectile.Center + bloodScytheShootVelocity.SafeNormalize(Vector2.UnitY) * 35f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), bloodScytheSpawnPosition, bloodScytheShootVelocity, ModContent.ProjectileType<BloodScythe>(), Projectile.damage, Projectile.knockBack * 0.4f, Projectile.owner);
                }
            }
        }

        public void DecideCurrentState()
        {
            // Switch to the swing state as necessary.
            if (CurrentState == 0f && Owner.itemAnimation > 0 && PostSwingRepositionDelay <= 0f)
            {
                if (SwingDirection == 0f)
                    SwingDirection = 1f;
                else
                    SwingDirection *= -1f;

                CurrentState = SwingState.Swinging;
            }

            // Reset the state of the weapon if the player stops using it.
            else if (Owner.itemAnimation <= 0)
            {
                Owner.itemAnimation = 0;
                Projectile.oldPos = new Vector2[Projectile.oldPos.Length];

                CurrentState = SwingState.Default;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.EnterShaderRegion();

            CalamityUtils.CalculatePerspectiveMatricies(out Matrix viewMatrix, out Matrix projectionMatrix);

            GameShaders.Misc["CalamityMod:LinearTransformation"].UseColor(Main.hslToRgb(0.95f, 0.85f, 0.5f));
            GameShaders.Misc["CalamityMod:LinearTransformation"].UseOpacity(0f);
            GameShaders.Misc["CalamityMod:LinearTransformation"].Shader.Parameters["uWorldViewProjection"].SetValue(viewMatrix * projectionMatrix);
            GameShaders.Misc["CalamityMod:LinearTransformation"].Shader.Parameters["localMatrix"].SetValue(new Matrix()
            {
                M11 = BladeHorizontalFactor,
                M12 = 0f,
                M21 = 0f,
                M22 = 1f,
            });
            GameShaders.Misc["CalamityMod:LinearTransformation"].Apply();

            CalamityUtils.DrawAfterimagesCentered(Projectile, 2, lightColor);

            Main.spriteBatch.ExitShaderRegion();

            CalamityUtils.CalculatePerspectiveMatricies(out var view, out var proj);
            CalamityShaders.PrimitiveClearShader.Parameters["uWorldViewProjection"].SetValue(view * proj);
            CalamityShaders.PrimitiveClearShader.CurrentTechnique.Passes[0].Apply();
            Filters.Scene["CalamityMod:PrimitiveClearShader"].GetShader().Shader.CurrentTechnique.Passes[0].Apply();

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ItemLoader.OnHitNPC(Owner.ActiveItem(), Owner, target, hit, damageDone);
            NPCLoader.OnHitByItem(target, Owner, Owner.ActiveItem(), hit, damageDone);
            PlayerLoader.OnHitNPC(Owner, target, hit, damageDone);
        }
    }
}

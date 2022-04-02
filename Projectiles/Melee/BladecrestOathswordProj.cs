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

namespace CalamityMod.Projectiles.Melee
{
	public class BladecrestOathswordProj : BaseIdleHoldoutProjectile
    {
        public enum SwingState
        {
            Default,
            Swinging
        }

        public int Direction = 1;
        public SwingState CurrentState;
        public ref float SwingDirection => ref projectile.ai[0];
        public ref float BladeHorizontalFactor => ref projectile.ai[1];
        public ref float PostSwingRepositionDelay => ref projectile.localAI[0];
        public ref float ChargePower => ref projectile.localAI[1];

        public override string Texture => "CalamityMod/Items/Weapons/Melee/BladecrestOathsword";
        public override int AssociatedItemID => ModContent.ItemType<BladecrestOathsword>();
        public override int IntendedProjectileType => ModContent.ProjectileType<BladecrestOathswordProj>();
        public override bool CanDamage() => CurrentState != 0; //Could also disable the damage during the channel state,

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bladecrest Oathsword");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 56;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 90000;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 12;
            projectile.noEnchantments = true;
            projectile.Calamity().trueMelee = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Direction);
            writer.Write(PostSwingRepositionDelay);
            writer.Write((int)CurrentState);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Direction = reader.ReadInt32();
            PostSwingRepositionDelay = reader.ReadSingle();
            CurrentState = (SwingState)reader.ReadInt32();
        }

        public override void SafeAI()
        {
            // Initialize the animation max time if necessary.
            if (Owner.itemAnimationMax == 0)
                Owner.itemAnimationMax = (int)(Owner.ActiveItem().useAnimation * Owner.meleeSpeed);

            // Decide the current phase state of the blade.
            DecideCurrentState();

            Direction = Owner.direction;

            // Glue the sword to its owner.
            projectile.Opacity = 1f;
            projectile.position = Owner.RotatedRelativePoint(Owner.MountedCenter, true) - projectile.Size / 2f + Vector2.UnitY * Owner.gfxOffY;

            float swingSpeedInterpolant = 0.27f;
            float swingCompletion = 1f - Owner.itemAnimation / (float)Owner.itemAnimationMax;
            float unchangedSwingCompletion = swingCompletion;
            if (SwingDirection == -1f)
                swingCompletion = 1f - swingCompletion;

            // Calculate the direction of the blade.
            Vector3 aimDirection3D = Vector3.Transform(Vector3.UnitX, Matrix.CreateRotationZ(MathHelper.PiOver2 * -Direction));

            if ((swingCompletion > 0f && swingCompletion < 1f) || PostSwingRepositionDelay > 0f)
            {
                swingSpeedInterpolant = MathHelper.Lerp(swingSpeedInterpolant, 1f, Utils.InverseLerp(0f, 0.2f, swingCompletion, true));

                float horizontalAngle = MathHelper.Lerp(-1.2f, 2.6f, (float)Math.Pow(MathHelper.SmoothStep(0f, 1f, swingCompletion), 3D)) * Direction;
                Matrix offsetLinearTransformation = Matrix.CreateRotationZ(horizontalAngle);

                aimDirection3D = Vector3.Transform(Vector3.UnitX, offsetLinearTransformation);
            }

            // Fuck this shit.
            // The code is kept around if we want the holdout effect but I'm honestly not going to continue Criticism Whack-a-Mole with this frankly arbitrary visual.
            else
                projectile.Opacity = 0f;

            // Determine the horizontal stretch offset of the blade. This is used in matrix math below to create 2.5D visuals.
            BladeHorizontalFactor = MathHelper.Lerp(1f, 1.5f, (aimDirection3D.X * 0.5f + 0.5f) * Utils.InverseLerp(1f, 0.8f, unchangedSwingCompletion, true));

            float baseRotation = new Vector2(aimDirection3D.X, aimDirection3D.Y).ToRotation();

            // Hold the blade upwards if not firing.
            if (CurrentState == SwingState.Swinging)
                PostSwingRepositionDelay = 10f;
            else if (PostSwingRepositionDelay > 0f)
                PostSwingRepositionDelay--;

            float idealRotation = baseRotation;

            Owner.itemRotation = 0f;
            Owner.heldProj = projectile.whoAmI;

            idealRotation += MathHelper.PiOver4;
            if (Direction == -1)
                idealRotation += MathHelper.Pi;

            // Define rotation.
            projectile.rotation = projectile.rotation.AngleTowards(idealRotation, swingSpeedInterpolant * 0.45f).AngleLerp(idealRotation, swingSpeedInterpolant * 0.2f);

            // Offset the blade so that the handle is attached to the owner's hand.
            float horizontalBladeOffset = MathHelper.Lerp(-4f, 10f, Utils.InverseLerp(1f, 0.72f, unchangedSwingCompletion, true) * Utils.InverseLerp(0f, 0.28f, unchangedSwingCompletion, true));
            Vector2 bladeOffset = (projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * projectile.width * 0.5f;
            bladeOffset += new Vector2(Direction * horizontalBladeOffset, 2f).RotatedBy(Owner.fullRotation) + Vector2.UnitY * Owner.gfxOffY;
            projectile.position += bladeOffset;

            // Create demon magic dust along the blade when swinging, as well as demon blood scythes.
            if (PostSwingRepositionDelay >= 10f && CurrentState == SwingState.Swinging)
            {
                for (int i = 0; i < 2; i++)
                {
                    Dust shadowflame = Dust.NewDustPerfect(projectile.Center + (projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * projectile.width * Main.rand.NextFloat(0.4f), 27);
                    shadowflame.velocity = (projectile.oldRot[0] - projectile.oldRot[1]).ToRotationVector2() * Main.rand.NextFloat(4f, 10f);
                    shadowflame.velocity += (projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 7.5f;
                    shadowflame.scale = Main.rand.NextFloat(1f, 1.5f);
                    shadowflame.fadeIn = 0.5f;
                    shadowflame.noGravity = true;
                }

                if (Main.myPlayer == projectile.owner && Owner.itemAnimation % 4 == 3 && Owner.itemAnimation < Owner.itemAnimationMax - 3)
                {
                    Vector2 bloodScytheShootVelocity = (projectile.rotation - MathHelper.PiOver4).ToRotationVector2();
                    bloodScytheShootVelocity.Y *= 0.04f;
                    bloodScytheShootVelocity = bloodScytheShootVelocity.SafeNormalize(Vector2.UnitY) * 50f;
                    Vector2 bloodScytheSpawnPosition = projectile.Center + bloodScytheShootVelocity.SafeNormalize(Vector2.UnitY) * 50f;
                    Projectile.NewProjectile(bloodScytheSpawnPosition, bloodScytheShootVelocity, ModContent.ProjectileType<BloodScythe>(), projectile.damage, projectile.knockBack * 0.4f, projectile.owner);
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
                projectile.oldPos = new Vector2[projectile.oldPos.Length];

                CurrentState = SwingState.Default;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.EnterShaderRegion();

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

            CalamityUtils.DrawAfterimagesCentered(projectile, 2, lightColor);
            spriteBatch.ExitShaderRegion();

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            ItemLoader.OnHitNPC(Owner.ActiveItem(), Owner, target, damage, knockback, crit);
            NPCLoader.OnHitByItem(target, Owner, Owner.ActiveItem(), damage, knockback, crit);
            PlayerHooks.OnHitNPC(Owner, Owner.ActiveItem(), target, damage, knockback, crit);
        }
    }
}

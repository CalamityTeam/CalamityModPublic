using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Melee;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.Graphics.Shaders;
using Terraria.Audio;
using static CalamityMod.CalamityUtils;
using System.Linq;
using CalamityMod.Graphics.Primitives;

namespace CalamityMod.Projectiles.Melee
{
    public class TerratomereHoldoutProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";

        public Player Owner => Main.player[Projectile.owner];

        public int Direction => Projectile.velocity.X.DirectionalSign();

        public float SwingCompletion => MathHelper.Clamp(Time / Terratomere.SwingTime, 0f, 1f);

        public float SwingCompletionAtStartOfTrail
        {
            get
            {
                float swingCompletion = SwingCompletion - Terratomere.TrailOffsetCompletionRatio;

                // Ensure that the trail does not attempt to "start" in the anticipation state, as the trail only exists after the charge begins.
                return MathHelper.Clamp(swingCompletion, SwingCompletionRatio, 1f);
            }
        }

        public float SwordRotation
        {
            get
            {
                float swordRotation = InitialRotation + GetSwingOffsetAngle(SwingCompletion) * Projectile.spriteDirection + MathHelper.PiOver4;
                if (Projectile.spriteDirection == -1)
                    swordRotation += MathHelper.PiOver2;
                return swordRotation;
            }
        }

        public Vector2 SwordDirection => SwordRotation.ToRotationVector2() * Direction;

        public ref float Time => ref Projectile.ai[0];

        public ref float InitialRotation => ref Projectile.ai[1];

        // Easings for things such as rotation.
        public static float SwingCompletionRatio => 0.37f;

        public static float RecoveryCompletionRatio => 0.84f;

        // Brief delay before the animations begin, with the blade simply being held upright for a time.
        public static CurveSegment AnticipationWait => new(EasingType.PolyOut, 0f, -1.67f, 0f);

        // Period of time where the blade reels back in anticipation of a swing.
        public static CurveSegment Anticipation => new(EasingType.PolyOut, 0.14f, AnticipationWait.EndingHeight, -1.05f, 2);

        // A short, powerful swing that rapidly approaches it destination.
        public static CurveSegment Swing => new(EasingType.PolyIn, SwingCompletionRatio, Anticipation.EndingHeight, 4.43f, 5);

        // Period of time after the swing where the blade reels back further before it disappears.
        public static CurveSegment Recovery => new(EasingType.PolyOut, RecoveryCompletionRatio, Swing.EndingHeight, 0.97f, 3);

        public static float GetSwingOffsetAngle(float completion) => PiecewiseAnimation(completion, AnticipationWait, Anticipation, Swing, Recovery);

        public override string Texture => "CalamityMod/Items/Weapons/Melee/Terratomere";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 100;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 66;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.timeLeft = Terratomere.SwingTime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.MaxUpdates = 2;
            Projectile.localNPCHitCooldown = Projectile.MaxUpdates * 7;
            Projectile.noEnchantmentVisuals = true;
        }

        #region AI and Behaviors

        public override void AI()
        {
            // Initialize the initial rotation if necessary.
            if (InitialRotation == 0f)
            {
                InitialRotation = Projectile.velocity.ToRotation();
                Projectile.netUpdate = true;
            }

            // Perform squish effects.
            Projectile.scale = Utils.GetLerpValue(0f, 0.13f, SwingCompletion, true) * Utils.GetLerpValue(1f, 0.87f, SwingCompletion, true) * 0.7f + 0.3f;

            AdjustPlayerValues();
            StickToOwner();
            CreateProjectiles();
            if (SwingCompletion > SwingCompletionRatio + 0.2f && SwingCompletion < RecoveryCompletionRatio)
                CreateSlashSparkleDust();

            // Determine rotation.
            Projectile.rotation = SwordRotation;
            Time++;
        }

        public void AdjustPlayerValues()
        {
            Projectile.spriteDirection = Projectile.direction = Direction;
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = (Projectile.direction * Projectile.velocity).ToRotation();

            // Decide the arm rotation for the owner.
            float armRotation = SwordRotation - Direction * 1.67f;
            Owner.SetCompositeArmFront(Math.Abs(armRotation) > 0.01f, Player.CompositeArmStretchAmount.Full, armRotation);
        }

        public void StickToOwner()
        {
            // Glue the sword to its owner. This applies a handful of offsets to make the blade look like it's roughly inside of the owner's hand.
            Projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter, true) + SwordDirection * new Vector2(7f, 16f) * Projectile.scale;
            Projectile.Center -= Projectile.velocity.SafeNormalize(Vector2.UnitY) * new Vector2(66f, 54f + Projectile.scale * 8f);

            // Set the owner's held projectile to this and register a false item time calculation.
            Owner.heldProj = Projectile.whoAmI;
            Owner.SetDummyItemTime(2);

            // Make the owner turn in the direction of the blade.
            Owner.ChangeDir(Direction);
        }

        public void CreateProjectiles()
        {
            // Create the slash.
            if (Time == (int)(Terratomere.SwingTime * (SwingCompletionRatio + 0.15f)))
                SoundEngine.PlaySound(Terratomere.SwingSound, Projectile.Center);
            
            if (Main.myPlayer == Projectile.owner && Time == (int)(Terratomere.SwingTime * (SwingCompletionRatio + 0.34f)))
            {
                Vector2 bigSlashVelocity = Projectile.SafeDirectionTo(Main.MouseWorld) * Owner.ActiveItem().shootSpeed;
                if (bigSlashVelocity.AngleBetween(InitialRotation.ToRotationVector2()) > 1.456f)
                    bigSlashVelocity = InitialRotation.ToRotationVector2() * bigSlashVelocity.Length();

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - bigSlashVelocity * 0.4f, bigSlashVelocity, ModContent.ProjectileType<TerratomereBigSlash>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            // Create a terra blade-like beam when the slash terminates.
            if (Main.myPlayer == Projectile.owner && Time == (int)(Terratomere.SwingTime * RecoveryCompletionRatio) + 5f)
            {
                Vector2 bigSlashVelocity = InitialRotation.ToRotationVector2() * Owner.ActiveItem().shootSpeed / 6f;
                Vector2 bigSlashSpawnPosition = Projectile.Center + bigSlashVelocity.SafeNormalize(Vector2.UnitY) * 64f;

                int slash = Projectile.NewProjectile(Projectile.GetSource_FromThis(), bigSlashSpawnPosition, bigSlashVelocity, ModContent.ProjectileType<TerratomereBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                if (Main.projectile.IndexInRange(slash))
                {
                    Main.projectile[slash].ai[0] = (Direction == 1f).ToInt();
                    Main.projectile[slash].ModProjectile<TerratomereBeam>().ControlPoints = GenerateSlashPoints().ToArray();
                }
            }
        }

        public void CreateSlashSparkleDust()
        {
            Vector2 initialDirection = InitialRotation.ToRotationVector2();
            Vector2 bladeEnd = Projectile.Center + (GetSwingOffsetAngle(SwingCompletion) * Direction + InitialRotation).ToRotationVector2() * Main.rand.NextFloat(8f, 66f) + initialDirection * 76f;

            int dustID = Main.rand.NextBool() ? 267 : 264;
            Dust magic = Dust.NewDustPerfect(bladeEnd, dustID, Vector2.Zero);
            magic.color = Color.Lerp(Terratomere.TerraColor1, Terratomere.TerraColor2, Main.rand.NextFloat());
            magic.color = Color.Lerp(magic.color, Color.Yellow, (float)Math.Pow(Main.rand.NextFloat(), 1.63));
            magic.fadeIn = Main.rand.NextFloat(1f, 2f);
            magic.scale = 0.4f;
            magic.velocity = initialDirection * Main.rand.NextFloat(0.5f, 15f);
            magic.noLight = true;
            magic.noGravity = true;
        }
        #endregion AI and Behaviors

        #region Drawing

        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;

        public override bool PreDraw(ref Color lightColor)
        {
            // Draw the slash effect.
            DrawSlash();

            // Draw the blade.
            DrawBlade(lightColor);

            return false;
        }

        public float SlashWidthFunction(float completionRatio) => Projectile.scale * 22f;

        public Color SlashColorFunction(float completionRatio) => Color.Lime * Utils.GetLerpValue(0.9f, 0.4f, completionRatio, true) * Projectile.Opacity;

        public IEnumerable<Vector2> GenerateSlashPoints()
        {
            for (int i = 0; i < 20; i++)
            {
                float progress = MathHelper.Lerp(SwingCompletion, SwingCompletionAtStartOfTrail, i / 20f);
                float reelBackAngle = Math.Abs(Projectile.oldRot[0] - Projectile.oldRot[1]) * 0.8f;
                if (SwingCompletion > RecoveryCompletionRatio)
                    reelBackAngle = 0.21f;

                float offsetAngle = (GetSwingOffsetAngle(progress) - reelBackAngle) * Direction + InitialRotation;
                yield return offsetAngle.ToRotationVector2() * Projectile.scale * 54f;
            }
        }

        public void DrawSlash()
        {
            // Draw the slash effect.
            Main.spriteBatch.EnterShaderRegion();

            // Prepare shader parameters. This relies on the same shader as the Exoblade, albeit with a less contrasted palette.
            PrepareSlashShader(Direction == 1);

            if (SwingCompletionAtStartOfTrail > SwingCompletionRatio)
                PrimitiveSet.Prepare(GenerateSlashPoints(), new(SlashWidthFunction, SlashColorFunction, (_) => Projectile.Center, shader: GameShaders.Misc["CalamityMod:ExobladeSlash"]), 95);

            Main.spriteBatch.ExitShaderRegion();
        }

        public void DrawBlade(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = texture.Size() * Vector2.UnitY;
            if (Projectile.spriteDirection == -1)
                origin.X += texture.Width;

            SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, direction, 0f);
        }

        public static void PrepareSlashShader(bool flipped)
        {
            GameShaders.Misc["CalamityMod:ExobladeSlash"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/VoronoiShapes"));
            GameShaders.Misc["CalamityMod:ExobladeSlash"].UseColor(Terratomere.TerraColor1);
            GameShaders.Misc["CalamityMod:ExobladeSlash"].UseSecondaryColor(Terratomere.TerraColor2);
            GameShaders.Misc["CalamityMod:ExobladeSlash"].Shader.Parameters["fireColor"].SetValue(Terratomere.TerraColor1.ToVector3());
            GameShaders.Misc["CalamityMod:ExobladeSlash"].Shader.Parameters["flipped"].SetValue(flipped);
            GameShaders.Misc["CalamityMod:ExobladeSlash"].Apply();
        }
        #endregion Drawing

        #region Hit Effects and Collision

        public void OnHitHealEffect()
        {
            if (Owner.moonLeech)
                return;
            
            Owner.statLife += Terratomere.TrueMeleeHitHeal;
            Owner.HealEffect(Terratomere.TrueMeleeHitHeal);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            Vector2 direction = (InitialRotation + GetSwingOffsetAngle(SwingCompletion)).ToRotationVector2() * new Vector2(Projectile.spriteDirection, 1f);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + direction * Projectile.height * Projectile.scale, Projectile.width * 0.25f, ref _);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), Terratomere.TrueMeleeGlacialStateTime);
            if (target.canGhostHeal)
                OnHitHealEffect();

            // Create a slash creator on top of the hit target.
            int slashCreatorID = ModContent.ProjectileType<TerratomereSlashCreator>();
            if (Owner.ownedProjectileCounts[slashCreatorID] < 4)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, slashCreatorID, Projectile.damage, Projectile.knockBack, Projectile.owner, target.whoAmI, Main.rand.NextFloat(MathHelper.TwoPi));
                Owner.ownedProjectileCounts[slashCreatorID]++;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), Terratomere.TrueMeleeGlacialStateTime);
            OnHitHealEffect();
        }
        #endregion Hit Effects and Collision
    }
}

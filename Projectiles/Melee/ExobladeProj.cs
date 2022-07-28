using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Typeless;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Sounds;
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
using System.Linq;
using static CalamityMod.CalamityUtils;
using Terraria.Graphics.Effects;
using CalamityMod.Dusts;

namespace CalamityMod.Projectiles.Melee
{
    public class ExobladeProj : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];

        public enum SwingState
        {
            Swinging,
            BonkDash
        }

        public PrimitiveTrail SlashDrawer = null;

        public PrimitiveTrail PierceAfterimageDrawer = null;

        public int SwingTime
        {
            get 
            {
                float itemUseTime = Owner.ActiveItem().useTime;

                return 78;
                return (int)(58 / (float)(itemUseTime > 0 ? itemUseTime : 1));
            }

        }
            
        public float Timer => SwingTime - Projectile.timeLeft;
        public float Progression => Timer / (float)SwingTime;

        public SwingState State
        {
            get => (SwingState)Projectile.ai[0];
            
            set
            {
                Projectile.ai[0] = (int)value;
            }
        }

        public bool PerformingPowerfulSlash
        {
            get => Projectile.ai[1] > 0;
            set => Projectile.ai[1] = value ? 1 : 0;
        }



        public ref float PostBonkCountdown => ref Projectile.ai[1];
        public ref float EnergyFormInterpolant => ref Projectile.localAI[0];
        public ref float SquishFactor => ref Projectile.localAI[1];



        public ref bool RMBChannel => ref (Owner.HeldItem.ModItem as Exoblade).RMBchannel;

        public bool LMBUse => Owner.altFunctionUse != 2 && !RMBChannel && Owner.itemAnimation > 0 && Main.mouseLeft;

        public const float StartingSwingRotation = -0.72f;
        public const float EndingSwingRotation = StartingSwingRotation + MathHelper.TwoPi - 0.33f;
        
        // Starts ahead and reels back.
        public static CurveSegment Anticipation => new(EasingType.PolyOut, 0f, Exoblade.AnticipationOffsetRatio, -Exoblade.AnticipationOffsetRatio, 2);

        public static CurveSegment SlashWait => new(EasingType.Linear, 0.37f, 0f, 0f);

        // After reeling back, a powerful slash happens.
        public static CurveSegment Slash => new(EasingType.PolyOut, 0.51f, 0f, 1f);

        // Manual easing code handles this, due to a 0-1 clamp within the piecewise curve function generator.
        public static CurveSegment HoldBladeInPlace => new(EasingType.PolyOut, 0.7f, 1f, Exoblade.AnticipationOffsetRatio, 5);


        #region A lot of angles
        public int Direction => Math.Sign(Projectile.velocity.X) <= 0 ? -1 : 1;
        public float BaseRotation => Projectile.velocity.ToRotation(); //The rotation of the swing's "main" diretion
        public Vector2 SquishVector => new Vector2(1f + (1 - SquishFactor) * 0.6f, SquishFactor); //The vector for the swords squish


        public static float MaxSwingAngle = MathHelper.PiOver2 * 1.8f;
        public CurveSegment SlowStart = new (PolyOutEasing, 0f, -1f, 0.3f, 2);
        public CurveSegment SwingFast = new (PolyInEasing, 0.27f, -0.7f, 1.6f, 4);
        public CurveSegment EndSwing = new (PolyOutEasing, 0.85f, 0.9f, 0.1f, 2);
        public float SwingAngleShiftAtProgress(float progress) => MaxSwingAngle * PiecewiseAnimation(progress, new CurveSegment[] { SlowStart, SwingFast, EndSwing });
        public float SwordRotationAtProgress(float progress) => BaseRotation + SwingAngleShiftAtProgress(progress) * Direction;
        public float SquishAtProgress(float progress) => MathHelper.Lerp(SquishVector.X, SquishVector.Y, (float)Math.Abs(Math.Sin(SwingAngleShiftAtProgress(progress))));
        public Vector2 DirectionAtProgress(float progress) => SwordRotationAtProgress(progress).ToRotationVector2() * SquishAtProgress(progress);



        public float SwingAngleShift => SwingAngleShiftAtProgress(Progression); //The current displacement from the "straigth forward" of swords angle
        public float SwordRotation => SwordRotationAtProgress(Progression); //The current rotation of the sword itself
        public float CurrentSquish => SquishAtProgress(Progression); //How squished is the sword based on its current rotation
        public Vector2 SwordDirection => DirectionAtProgress(Progression); //A "unit" vector that keeps the stretch of the sword in mind
        #endregion

        #region Ok so prims
        public float TrailEndProgression //What's the progression of the "end point" of the trail
        {
            get
            {
                float endProgression;
                if (Progression < 0.75f)
                    endProgression = Progression - 0.5f + 0.1f * (Progression / 0.75f);

                else
                    endProgression = Progression - 0.4f * (1 - (Progression - 0.75f) / 0.75f);

                return Math.Clamp(endProgression, 0, 1);
            }
        }

        public float RealProgressionAtTrailCompletion(float completion) => MathHelper.Lerp(Progression, TrailEndProgression, completion); //Gives the "progression" in the swing of the trail at the specified completion

        //Direction at progress except goes a bit harder at the end if the squish is strong to avoid the prim trail cutting off weirdly.
        public Vector2 DirectionAtProgressScuffed(float progress)
        {
            if (SquishFactor > 0.7f || progress < 0.5f)
                return DirectionAtProgress(progress);

            float angleShift = SwingAngleShiftAtProgress(progress);
            angleShift += MathHelper.PiOver4 * 0.3f * (float)Math.Pow((progress - 0.5f) / 0.5f, 0.5f) * ((0.7f - SquishFactor) / 0.2f);

            return (BaseRotation + angleShift * Direction).ToRotationVector2() * SquishAtProgress(progress);
        }
        #endregion


        public float RiskOfDust
        {
            get
            {
                if (Progression > 0.85f)
                    return 0;

                if (Progression < 0.4f)
                    return (float)Math.Pow(Progression / 0.3f, 2) * 0.2f;

                if (Progression < 0.5f)
                    return 0.2f + 0.7f * (Progression - 0.4f) / 0.1f;

                return 0.9f;
            }
        }


        public override string Texture => "CalamityMod/Items/Weapons/Melee/ExobladeSquare";

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
            Projectile.timeLeft = 500;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.MaxUpdates = 3;
            Projectile.localNPCHitCooldown = Projectile.MaxUpdates * 8;
            Projectile.noEnchantments = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(PerformingPowerfulSlash);
            writer.Write(SquishFactor);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            PerformingPowerfulSlash = reader.ReadBoolean();
            SquishFactor = reader.ReadSingle();
        }


        public override bool ShouldUpdatePosition() => State == SwingState.BonkDash;



        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            Vector2 start = Projectile.Center;
            Vector2 end = start + SwordDirection * 140f * Projectile.scale;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, Projectile.scale * 30f, ref _);
        }



        public void InitializationEffects(bool startInitialization)
        {
            Projectile.velocity = Owner.MountedCenter.DirectionTo(Owner.Calamity().mouseWorld);
            SquishFactor = Main.rand.NextFloat(0.67f, 1f);
            Projectile.timeLeft = SwingTime;

            if (startInitialization)
                Projectile.scale = 0.02f;

            PerformingPowerfulSlash = false;

            Projectile.netUpdate = true;
        }

        public override void AI()
        {
            if (Projectile.timeLeft >= 500 || (Projectile.timeLeft == 1 && Owner.channel))
                InitializationEffects(Projectile.timeLeft >= 500);

            switch (State)
            {
                case SwingState.Swinging:
                    DoBehavior_Swinging();
                    break;
                case SwingState.BonkDash:
                    DoBehavior_BonkDash();
                    break;
            }

            // Glue the sword to its owner.
            Projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Owner.heldProj = Projectile.whoAmI;
            Owner.SetDummyItemTime(2);
            Owner.direction = Direction;

            // Decide the arm rotation for the owner.
            float armRotation = SwordRotation - MathHelper.PiOver2;
            Owner.SetCompositeArmFront(Math.Abs(armRotation) > 0.01f, Player.CompositeArmStretchAmount.Full, armRotation);

            // Handle timers.
            if (PostBonkCountdown > 0)
                PostBonkCountdown--;
        }

        public void DoBehavior_Swinging()
        {
            if (Projectile.timeLeft == SwingTime / 5)
                SoundEngine.PlaySound(Exoblade.SwingSound, Projectile.Center);

            float exactSwingCompletion = 1f - Owner.itemAnimation / (float)Owner.itemAnimationMax;
            float directionalSwingCompletion = exactSwingCompletion;            
            directionalSwingCompletion = MathHelper.Clamp(directionalSwingCompletion, 0f, 1f);
            float swingCompletion = PiecewiseAnimation(directionalSwingCompletion, Anticipation, SlashWait, Slash, HoldBladeInPlace);

            // Clear the rotation cache after enough time has passed.
            if (directionalSwingCompletion < 0.45f)
                Projectile.oldRot = new float[Projectile.oldRot.Length];

            // Decide the scale of the sword.
            
            if (Projectile.scale < 1)
                Projectile.scale = MathHelper.Lerp(Projectile.scale, 1f, 0.08f);

            //Make the sword get smaller near the end of the slash
            if (!Owner.channel && Progression > 0.7f)
                Projectile.scale = 0.5f + 0.5f * (float)Math.Pow(1 - (Progression - 0.7f) / 0.3f, 0.5);

            // If performing a powerful slash it is made significantly bigger. This effect looks slightly goofy but there is no
            // way in hell you're going to consistently hit SCal without it.
            if (PerformingPowerfulSlash)
            {
                Projectile.scale *= Exoblade.BigSlashUpscaleFactor;
                EnergyFormInterpolant = 1f;
            }


            if (Main.rand.NextFloat() * 5f < RiskOfDust)
            {
                Dust auricDust = Dust.NewDustPerfect(Owner.MountedCenter + SwordDirection * 140 * Projectile.scale * (float)Math.Pow(Main.rand.NextFloat(0.5f, 1f), 0.5f), ModContent.DustType<AuricBarDust>(), SwordDirection.RotatedBy(-MathHelper.PiOver2 * Direction) * 2f);
                auricDust.noGravity = true;
                auricDust.alpha = 10;
                auricDust.scale = 0.5f;
            }

            if (Main.rand.NextFloat() < RiskOfDust)
            {
                Color dustColor = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.9f);
                Dust must = Dust.NewDustPerfect(Owner.MountedCenter + SwordDirection * 140 * Projectile.scale * (float)Math.Pow(Main.rand.NextFloat(0.2f, 1f), 0.5f), 267, SwordDirection.RotatedBy(MathHelper.PiOver2 * Direction) * 2.6f, 0, dustColor);

                must.scale = 0.3f;
                must.fadeIn = Main.rand.NextFloat() * 1.2f;
                must.noGravity = true;
            }


            // Create a bunch of homing beams.
            int beamShootRate = Projectile.MaxUpdates * 2;
            if (Main.myPlayer == Projectile.owner && Projectile.timeLeft % beamShootRate == 0 && swingCompletion > 0.3f && swingCompletion < 0.9f)
            {
                int boltDamage = (int)(Projectile.damage * Exoblade.NotTrueMeleeDamagePenalty);
                Vector2 boltVelocity = (Projectile.rotation + MathHelper.PiOver4).ToRotationVector2();
                boltVelocity = Vector2.Lerp(boltVelocity, Vector2.UnitX * Direction, 0.8f).SafeNormalize(Vector2.UnitY);
                boltVelocity *= Owner.ActiveItem().shootSpeed;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + boltVelocity * 5f, boltVelocity, ModContent.ProjectileType<Exobeam>(), boltDamage, Projectile.knockBack / 3f, Projectile.owner);
            }
        }

        public void DoBehavior_BonkDash()
        {
            Owner.mount?.Dismount(Owner);
            Owner.RemoveAllGrapplingHooks();

            if (Owner.itemAnimation > Owner.itemAnimationMax * Exoblade.PercentageOfAnimationSpentLunging)
            {
                // Play a charge sound right before the dash.
                if (Owner.itemAnimation == (int)(Owner.itemAnimationMax * Exoblade.PercentageOfAnimationSpentLunging) + 1)
                    SoundEngine.PlaySound(CommonCalamitySounds.ELRFireSound, Projectile.Center);

                // Look at the mouse.
                Projectile.oldPos = new Vector2[Projectile.oldPos.Length];
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.velocity = Projectile.SafeDirectionTo(Main.MouseWorld);
                    Owner.ChangeDir((Projectile.velocity.X > 0f).ToDirectionInt());
                    Projectile.netSpam = 0;
                    Projectile.netUpdate = true;
                }
            }

            // Do the dash.
            else
                Owner.velocity = Projectile.velocity * Exoblade.LungeSpeed;

            // Stop the dash on the last frame.
            if (Owner.itemAnimation == 1)
            {
                Owner.itemAnimation = 0;
            }

            // Decide the scale and opacty of the sword.
            Projectile.scale = 1f;
            Projectile.Opacity = 1f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 * Direction;

            // Use the energy form.
            float exactSwingCompletion = 1f - Owner.itemAnimation / (float)Owner.itemAnimationMax;
            EnergyFormInterpolant = Utils.GetLerpValue(0f, 0.32f, exactSwingCompletion, true) * Utils.GetLerpValue(1f, 0.85f, exactSwingCompletion, true);
        }



        public float SlashWidthFunction(float completionRatio) => SquishAtProgress(RealProgressionAtTrailCompletion(completionRatio)) * Projectile.scale * 36.5f;
        public Color SlashColorFunction(float completionRatio) => Color.Lime * Utils.GetLerpValue(0.9f, 0.4f, completionRatio, true) * Projectile.Opacity;

        public float PierceWidthFunction(float completionRatio) => Utils.GetLerpValue(0f, 0.2f, completionRatio, true) * Projectile.scale * 50f;

        public Color PierceColorFunction(float completionRatio) => Color.Lime * EnergyFormInterpolant * Projectile.Opacity;

        public IEnumerable<Vector2> GenerateSlashPoints()
        {
            List<Vector2> result = new();

            for (int i = 0; i < 90; i++)
            {
                float progress = MathHelper.Lerp(Progression, TrailEndProgression, i / 90f);

                result.Add(DirectionAtProgressScuffed(progress) * 110f * Projectile.scale);
            }

            return result;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.Opacity <= 0f)
                return false;

            // Initialize the primitives drawers.
            SlashDrawer ??= new(SlashWidthFunction, SlashColorFunction, null, GameShaders.Misc["CalamityMod:ExobladeSlash"]);
            PierceAfterimageDrawer ??= new(PierceWidthFunction, PierceColorFunction, null, GameShaders.Misc["CalamityMod:ExobladePierce"]);

            DrawSlash();
            DrawPierceTrail();
            DrawBlade();
            return false;
        }

        public void DrawSlash()
        {
            if (State != SwingState.Swinging || Progression < 0.45f)
                return;

            // Draw the zany slash effect.
            Main.spriteBatch.EnterShaderRegion();
            GameShaders.Misc["CalamityMod:ExobladeSlash"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/VoronoiShapes"));
            GameShaders.Misc["CalamityMod:ExobladeSlash"].UseColor(new Color(105, 240, 220));
            GameShaders.Misc["CalamityMod:ExobladeSlash"].UseSecondaryColor(new Color(57, 46, 115));
            GameShaders.Misc["CalamityMod:ExobladeSlash"].Shader.Parameters["fireColor"].SetValue(new Color(242, 112, 72).ToVector3());

            // What the heck? XORs? In MY exoblade code?????
            GameShaders.Misc["CalamityMod:ExobladeSlash"].Shader.Parameters["flipped"].SetValue(Direction == 1);
            GameShaders.Misc["CalamityMod:ExobladeSlash"].Apply();

            SlashDrawer.Draw(GenerateSlashPoints(), Projectile.Center - Main.screenPosition, 95);
        }

        public void DrawPierceTrail()
        {
            if (State != SwingState.BonkDash)
                return;

            Main.spriteBatch.EnterShaderRegion();

            Vector2 trailOffset = (Projectile.rotation - Direction * MathHelper.PiOver4).ToRotationVector2() * 98f + Projectile.Size * 0.5f - Main.screenPosition;
            GameShaders.Misc["CalamityMod:ExobladePierce"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/EternityStreak"));
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseImage2("Images/Extra_189");
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseColor(Color.Cyan);
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseSecondaryColor(Color.Lime);
            GameShaders.Misc["CalamityMod:ExobladePierce"].Apply();
            PierceAfterimageDrawer.Draw(Projectile.oldPos.Take(31), trailOffset, 53);
        }

        public void DrawBlade()
        {
            Main.spriteBatch.ExitShaderRegion();
            float rotation = Projectile.rotation;
            Vector2 origin = Projectile.Size * new Vector2(0.5f, 1f);
            Vector2 drawPosition = Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.scale * 32f - Main.screenPosition;
            if (Direction == -1)
                rotation += MathHelper.Pi;
            
            var texture = ModContent.Request<Texture2D>(Texture).Value;
            Color bladeColor = Color.White;
            bladeColor = Color.Lerp(bladeColor, Color.Lerp(Color.Cyan, Color.Lime, 0.5f) with { A = 60 }, EnergyFormInterpolant);
            bladeColor *= Projectile.Opacity;
            SpriteEffects direction = Direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            //Main.spriteBatch.Draw(texture, drawPosition, null, bladeColor, rotation, origin, Projectile.scale, direction, 0);

            /*
            // Draw a glow effect if in the energy form.
            if (EnergyFormInterpolant > 0f)
            {
                for (int i = 0; i < 6; i++)
                {
                    Vector2 drawOffset = (MathHelper.TwoPi * i / 6f).ToRotationVector2() * EnergyFormInterpolant * Projectile.scale * 4f;
                    Main.spriteBatch.Draw(texture, drawPosition + drawOffset, null, bladeColor with { A = 0 } * 0.1f, rotation, origin, Projectile.scale, direction, 0);
                }
            }
            */

            Effect swingFX = Filters.Scene["SwingSprite"].GetShader().Shader;
            swingFX.Parameters["rotation"].SetValue(SwingAngleShift + MathHelper.PiOver4 + (Direction == -1 ? MathHelper.Pi : 0f));
            swingFX.Parameters["pommelToOriginPercent"].SetValue(0.1f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, swingFX, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Owner.MountedCenter - Main.screenPosition, null, Color.White, BaseRotation, texture.Size() / 2f, SquishVector * 3f * Projectile.scale, direction, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            ItemLoader.OnHitNPC(Owner.ActiveItem(), Owner, target, damage, knockback, crit);
            NPCLoader.OnHitByItem(target, Owner, Owner.ActiveItem(), damage, knockback, crit);
            PlayerLoader.OnHitNPC(Owner, Owner.ActiveItem(), target, damage, knockback, crit);

            if (State == SwingState.BonkDash && Owner.itemAnimation < Owner.itemAnimationMax * 0.6f)
            {
                Owner.itemAnimation = 0;
                Owner.velocity = Owner.SafeDirectionTo(target.Center) * -Exoblade.ReboundSpeed;
                PostBonkCountdown = Exoblade.OpportunityForBigSlash;
                Projectile.netUpdate = true;

                SoundEngine.PlaySound(PlasmaGrenade.ExplosionSound, target.Center);
                SoundEngine.PlaySound(YanmeisKnife.HitSound, target.Center);
                if (Main.myPlayer == Projectile.owner)
                {
                    int lungeHitDamage = (int)(Projectile.damage * Exoblade.LungeDamageFactor);
                    for (int i = 0; i < 5; i++)
                    {
                        int slash = Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Projectile.velocity * 0.1f, ModContent.ProjectileType<ExobeamSlashCreator>(), lungeHitDamage, 0f, Projectile.owner, target.whoAmI);
                        if (Main.projectile.IndexInRange(slash))
                            Main.projectile[slash].timeLeft -= i * 4;
                    }
                }

                // Freeze the target briefly, to allow the player to more easily perform a powerful slash.
                target.AddBuff(ModContent.BuffType<ExoFreeze>(), 60);
            }

            if (State == SwingState.Swinging && PerformingPowerfulSlash && Owner.ownedProjectileCounts[ModContent.ProjectileType<Exoboom>()] < 1)
            {
                SoundEngine.PlaySound(TeslaCannon.FireSound, Projectile.Center);
                if (Main.myPlayer == Projectile.owner)
                {
                    int explosionDamage = (int)(Projectile.damage * Exoblade.ExplosionDamageFactor);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<Exoboom>(), explosionDamage, 0f, Projectile.owner);
                }
            }
        }

        public override void Kill(int timeLeft) => Owner.fullRotation = 0f;
    }
}

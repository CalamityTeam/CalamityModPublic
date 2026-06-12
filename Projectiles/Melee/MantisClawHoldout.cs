using System;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class MantisClawHoldout : BaseCustomUseStyleProjectile, ILocalizedModType
    {
        bool AnimationCooldown = false;

        float JetDamageMultiplier => 6.5f;
        int SlashSpeed => 6;

        int BlastChargeUses => 3;
        float SlashTimer = 0;

        public override Vector2 SpriteOrigin => new(0, 20);

        public override int AssignedItemID => ModContent.ItemType<MantisClaws>();
        public override bool IgnoreAutoScale => true;

        public float ClawOpenness = MathHelper.ToRadians(80);
        public float BubbleSize = 0f;
        public int m2KillTimer = 0;
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Items/Weapons/Melee/MantisClaws";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Melee;
        }
        public override void ResetStyle()
        {
            if (Projectile.scale < 0.8f && m2KillTimer <= 0)
                Projectile.active = false;
            if (AnimationCooldown)
            {
                if (NumberOfAnimations % 5 <= BlastChargeUses)
                {
                    ClawOpenness = MathHelper.Lerp(ClawOpenness, MathHelper.ToRadians(80f), 0.15f);

                    BubbleSize *= 0.85f;

                    if (BubbleSize < 0.05f)
                    {
                        if (Projectile.scale > 0 && m2KillTimer <= 10)
                            Projectile.scale -= 0.1f;
                        if (Projectile.scale < 0.1f && m2KillTimer <= 0)
                            Projectile.active = false;
                        else
                        {
                            Owner.altFunctionUse = 2;
                            m2KillTimer--;
                        }
                    }
                }
                else
                {
                    Clamp();
                }
                Offset = new Vector2(MathHelper.Lerp(0, 12, BubbleSize / 2), 0).RotatedBy(Projectile.rotation);
            }
        }

        public void Clamp()
        {
            if (BubbleSize != 0f)
            {
                Owner.Calamity().mouseWorldListener = true;
                m2KillTimer = 20;

                // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center + new Vector2(20, 0).RotatedBy(Projectile.rotation), 
                    Owner.DirectionTo(Owner.Calamity().mouseWorld) * 30, ModContent.ProjectileType<MantisClawJet>(), (int)(Projectile.damage * JetDamageMultiplier), 7, Owner.whoAmI, 0f, 40f);

                for (int i = 0; i < 9; i++)
                {
                    WaterFlavoredParticle waterFlavored = new WaterFlavoredParticle(Owner.Center, new Vector2(Main.rand.NextFloat(15, 25), 0).RotatedBy(Projectile.rotation + MathHelper.ToRadians(Main.rand.NextFloat(-45, 45))), true, 40, Main.rand.NextFloat(0.5f, 1.2f), MantisClawJet.WaterColor);
                    waterFlavored.AffectedByLight = true;
                    GeneralParticleHandler.SpawnParticle(waterFlavored);
                }

                SoundEngine.PlaySound((Main.rand.NextBool(2) ? SoundID.Item85 : SoundID.Item86).WithPitchOffset(-0.5f), Owner.Center);
                SoundEngine.PlaySound(SoundID.NPCDeath14.WithPitchOffset(1f), Owner.Center);

                Owner.SetScreenshake(3f);

                GeneralParticleHandler.SpawnParticle(new MantisPunch(Owner.Center + new Vector2(26, 0).RotatedBy(Projectile.rotation), Projectile.rotation));
            }

            ClawOpenness = MathHelper.ToRadians(80);
            BubbleSize = 0f;
        }

        public override void UseStyle()
        {
            if (Owner.altFunctionUse != 2)
            {
                if (AnimationCooldown)
                {
                    if (!Owner.Calamity().mouseRight)
                    {
                        if (Owner.itemAnimation > 0)
                        {
                            Projectile.Kill();
                        }
                        DrawUnconditionally = true;
                        AnimationCooldown = true;
                        Owner.itemAnimation = 0;
                        Owner.itemTime = 0;
                    }
                }

                Projectile.scale = 1f;

                DrawUnconditionally = false;

                Owner.Calamity().mouseWorldListener = true;

                { 
                    if (SlashTimer % (int)(SlashSpeed / Owner.GetAttackSpeed<MeleeDamageClass>()) == 0)
                    {
                        SoundStyle SlashStyle = new SoundStyle("CalamityMod/Sounds/Item/MantisSwipe", 2);
                        SlashStyle.PitchVariance = 0.3f;
                        SlashStyle.Volume = 0.7f;

                        SoundEngine.PlaySound(SlashStyle.WithPitchOffset(0.2f), Owner.Center);

                        Owner.Calamity().mouseWorldListener = true;

                        // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                        Projectile slash = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center, Owner.DirectionTo(Owner.Calamity().mouseWorld) * 6f, ModContent.ProjectileType<MantisClawSlash>(), Projectile.damage, 2f, Owner.whoAmI);
                        slash.rotation = Owner.AngleTo(Owner.Calamity().mouseWorld) + MathHelper.ToRadians(Main.rand.NextFloat(-25, 25));
                        slash.localAI[0] = Owner.GetMeleeScale();
                    }

                    SlashTimer++;
                }

                Owner.direction = Math.Sign(Owner.Calamity().mouseWorld.X - Owner.Center.X);
                Projectile.rotation = Owner.AngleTo(Owner.Calamity().mouseWorld);

                Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], MathHelper.ToRadians(135f * (SlashTimer % (SlashSpeed * 2) < SlashSpeed ? 1 : -1)), 0.2f);

                ArmRotationOffset = MathHelper.ToRadians(-90f) - (Projectile.ai[2]) * Owner.direction;
                ArmRotationOffsetBack = MathHelper.ToRadians(-90f) + (Projectile.ai[2]) * Owner.direction;
            }
            else
            {
                Projectile.scale = MathHelper.Lerp(Projectile.scale, 1f, 0.15f);
                Projectile.scale = MathHelper.Clamp(Projectile.scale, 0f, 1f);
                if (Owner.Calamity().mouseRight)
                {
                    Owner.itemAnimation = 3;

                    float chargeFactor = (float)(NumberOfAnimations % 5f) / 3f;

                    DrawUnconditionally = true;

                    ArmRotationOffset = MathHelper.ToRadians(-90f) - (ClawOpenness / 5) * Owner.direction;
                    ArmRotationOffsetBack = MathHelper.ToRadians(-90f) + (ClawOpenness / 5) * Owner.direction;
                    if (NumberOfAnimations % 5 > BlastChargeUses)
                    {
                        Clamp();
                    }
                    else
                    {
                        Owner.Calamity().mouseWorldListener = true;

                        Owner.direction = Math.Sign(Owner.Calamity().mouseWorld.X - Owner.Center.X);
                        Projectile.rotation = Owner.AngleTo(Owner.Calamity().mouseWorld);

                        Projectile.ai[0]++;

                        if (AnimationProgress == 1)
                        {
                            if (NumberOfAnimations % 5 == 0)
                            {
                                ClawOpenness = MathHelper.ToRadians(80);
                                BubbleSize = 0f;
                            }

                            SoundEngine.PlaySound((Main.rand.NextBool(2) ? SoundID.Item85 : SoundID.Item86).WithPitchOffset((float)NumberOfAnimations % 5 / (float)BlastChargeUses), Owner.Center);

                            Projectile.ai[1] = MathHelper.Lerp(0.5f, 2f, chargeFactor);
                            Projectile.ai[2] = MathHelper.Lerp(MathHelper.ToRadians(100f), MathHelper.ToRadians(180f), chargeFactor);
                        }

                        BubbleSize = MathHelper.Lerp(BubbleSize, Projectile.ai[1], 0.2f);
                        ClawOpenness = MathHelper.Lerp(ClawOpenness, Projectile.ai[2], 0.2f);
                    }

                    Offset = new Vector2(MathHelper.Lerp(0, 12, BubbleSize / 2), 0).RotatedBy(Projectile.rotation);
                }
                else
                {
                    if (Owner.controlUseItem)
                    {
                        Projectile.Kill();
                    }
                    DrawUnconditionally = true;
                    AnimationCooldown = true;
                    Owner.itemAnimation = 0;
                    Owner.itemTime = 0;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            {
                FlipAsSword = false;
                float rot = 0f;
                if (Owner.altFunctionUse != 2 && !AnimationCooldown)
                {
                    rot = Projectile.ai[2];
                    if (Owner.direction == -1)
                    {
                        FlipAsSword = true;

                        RotationOffset = MathHelper.ToRadians(-90f);

                        Owner.compositeFrontArm.rotation -= RotationOffset;
                        Owner.compositeBackArm.rotation -= RotationOffset;
                    }
                }
                else
                {
                    RotationOffset = 0f;
                }

                // Bubble texture drawing

                Asset<Texture2D> Bubble = ModContent.Request<Texture2D>("CalamityMod/Particles/Bubble");

                Main.EntitySpriteDraw(Bubble.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY) + new Vector2(20, 0).RotatedBy(Projectile.rotation + RotationOffset), Bubble.Frame(), lightColor, 0f, Bubble.Size() / 2, BubbleSize, SpriteEffects.None);

                // Only draw the projectile if the projectile's owner is currently using the item this projectile is attached to.
                if (Owner.itemAnimation > 0 || DrawUnconditionally)
                {
                    Asset<Texture2D> tex = ModContent.Request<Texture2D>(Texture);

                    float r = FlipAsSword ? 0f : MathHelper.ToRadians(90);

                    Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY), tex.Frame(1, FrameCount, 0, Frame), lightColor, Projectile.rotation - ClawOpenness + rot + RotationOffset + r, FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
                }
                if (Owner.itemAnimation > 0 || DrawUnconditionally)
                {

                    if (Owner.altFunctionUse == 2 || AnimationCooldown)
                    {
                        Asset<Texture2D> tex = ModContent.Request<Texture2D>(Texture);

                        float r = FlipAsSword ? 0f : MathHelper.ToRadians(90);

                        Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY), tex.Frame(1, FrameCount, 0, Frame), lightColor, Projectile.rotation + ClawOpenness + RotationOffset + r, !FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.None : SpriteEffects.FlipHorizontally));
                    }
                    else
                    {
                        Asset<Texture2D> tex = ModContent.Request<Texture2D>(Texture);

                        float r = FlipAsSword ? 0f : MathHelper.ToRadians(90);

                        Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY), tex.Frame(1, FrameCount, 0, Frame), lightColor, Projectile.rotation - ClawOpenness - rot + RotationOffset + r, FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
                    }
                }
            }

            return false;
        }

        // The holdout cannot deal damage.
        public override bool? CanDamage() => false;

        public override void WhenSpawned() => Projectile.scale = 0f;
    }
}

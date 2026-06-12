using System;
using CalamityMod.DataStructures;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class TruePureClarity : BaseCustomUseStyleProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override int AssignedItemID => ModContent.ItemType<TrueBiomeBlade>();
        public override string Texture => "CalamityMod/Items/Weapons/Melee/TrueBiomeBlade";

        public override float HitboxOutset => 65f;
        public override Vector2 HitboxSize => new Vector2(100, 100);
        public override float HitboxRotationOffset => MathHelper.ToRadians(-45);
        public override Vector2 SpriteOrigin => new(-2, 70);

        public ref float State => ref Projectile.ai[0]; // 0 - Swinging. 1 - Thrusting.
        public ref float SwingDir => ref Projectile.ai[1];
        public Vector2 mousePos;
        public Vector2 aimPos;
        public bool doSwing = true;
        public bool postSwing = false;
        public int useAnimation;
        public static readonly SoundStyle FullChargeSound = new("CalamityMod/Sounds/Item/MagicRockSound");

        public Color outlineColorGreen => (Main.player[Projectile.owner].HeldItem.ModItem as TrueBiomeBlade).mainAttunement.tooltipColor;
        public Color outlineColorBlue => new Color(110, 216, 255);
        public const int dashTime = 30;
        public float dashRotation;
        public Vector2 lastDisplacement;
        public Vector2 dashDirection = Vector2.Zero;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.scale = 1.25f;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void WhenSpawned()
        {
            SwingDir = 1f;
            mousePos = Owner.Calamity().mouseWorld;
            aimPos = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
            useAnimation = Owner.itemAnimationMax;

            Owner.direction = (mousePos.X < Owner.Center.X) ? -1 : 1;
            FlipAsSword = Owner.direction == -1 ? true : false;
        }

        public override void UseStyle()
        {
            if (Owner.HeldItem.ModItem is not TrueBiomeBlade || (Owner.HeldItem.ModItem as TrueBiomeBlade).mainAttunement != AttunementSystem.FindOrNull(AttunementID.TrueDefault))
            {
                Projectile.Kill();
                return;
            }

            // If the player stops using the weapon after hitting with it three times, switch to lunge.
            if (Owner.CantUseHoldout())
            {
                if (State == 0f && Projectile.numHits > 2)
                {
                    State = 1f;
                    SoundEngine.PlaySound(SoundID.Item120 with { Volume = SoundID.Item120.Volume * 0.5f }, Projectile.Center);

                    AnimationProgress = 0;
                    Projectile.rotation = (Owner.Calamity().mouseWorld - Owner.Center).SafeNormalize(Vector2.UnitX).ToRotation();
                    Projectile.timeLeft = dashTime;
                    lastDisplacement = Projectile.Center - Owner.Center;
                    Projectile.ForceNetUpdate();
                }
            }

            // Normal swing behavior.
            if (State == 0f)
            {
                dashDirection = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.Zero).SafeNormalize(Vector2.UnitX);

                if (CanHit || postSwing)
                    mousePos = Owner.Center - aimPos;
                else
                    mousePos = Owner.Calamity().mouseWorld;

                // Reset before a new swing.
                if (!doSwing)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                        Projectile.localNPCImmunity[i] = 0;

                    mousePos = Owner.Calamity().mouseWorld;
                    aimPos = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                    CanHit = false;
                    Owner.direction = (mousePos.X < Owner.Center.X) ? -1 : 1;
                    FlipAsSword = Owner.direction == -1 ? true : false;
                    doSwing = true;
                }
                else
                {
                    if (!CanHit && !postSwing)
                        Owner.direction = (mousePos.X < Owner.Center.X) ? -1 : 1;
                    else
                        Owner.direction = ((Owner.Center - aimPos).X < Owner.Center.X) ? -1 : 1;

                    Projectile.rotation = Projectile.rotation.AngleLerp(Owner.AngleTo(mousePos) + MathHelper.ToRadians(65f), 0.1f);

                    if (AnimationProgress < (useAnimation / 3))
                    {
                        // Swing wind-up. Should not deal damage.
                        aimPos = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                        CanHit = false;
                        postSwing = false;
                        if (AnimationProgress == 0)
                        {
                            doSwing = false;
                            SwingDir = -SwingDir;
                        }
                        RotationOffset = MathHelper.Lerp(RotationOffset, MathHelper.ToRadians(120f * SwingDir * Owner.direction), 0.2f);
                    }
                    else
                    {
                        float swingTime = AnimationProgress - (useAnimation / 3);
                        float swingTimeMax = useAnimation - (useAnimation / 3);

                        if (swingTime > (int)(swingTimeMax * 0.2f) && swingTime < (int)(swingTimeMax * 0.85f))
                        {
                            CanHit = true;

                            // Particle effects on swing.
                            Vector2 particleVel = new Vector2(0, 10 * -SwingDir * Owner.direction).RotatedBy(FinalRotation - MathHelper.PiOver4);
                            Vector2 particlePos = Owner.Center + new Vector2(Main.rand.Next(0, 80), 0).RotatedBy(FinalRotation - MathHelper.PiOver4);
                            Color particleColor = Main.rand.NextBool() ? outlineColorBlue : outlineColorGreen;
                            bool empowered = Projectile.numHits > 2;
                            if (Main.rand.NextBool())
                            {
                                GenericBloom bloom = new(particlePos, particleVel, particleColor, empowered ? 0.12f : 0.08f, 20);
                                GeneralParticleHandler.SpawnParticle(bloom);
                            }
                            else
                            {
                                GenericSparkle sparkle = new(particlePos, particleVel, particleColor, particleColor, empowered ? 0.78f : 0.55f, 20);
                                GeneralParticleHandler.SpawnParticle(sparkle);
                            }
                        }
                        else
                            CanHit = false;

                        // Fire projectiles.
                        if (swingTime == (int)(swingTimeMax * 0.4f))
                        {
                            SoundEngine.PlaySound(SoundID.Item43 with { Volume = 0.65f }, Projectile.Center);
                            Vector2 projVel = -aimPos.SafeNormalize(Vector2.UnitX) * 12f;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, projVel, ModContent.ProjectileType<TruePurityProjection>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                        }

                        RotationOffset = MathHelper.Lerp(RotationOffset,
                            MathHelper.ToRadians(MathHelper.Lerp(150f * SwingDir * Owner.direction, 120f * -SwingDir * Owner.direction, CalamityUtils.ExpInOutEasing(swingTime / swingTimeMax, 1))),
                            0.2f);

                        // Extra swing control.
                        if (swingTime >= swingTimeMax)
                            doSwing = false;
                        if (swingTime < (int)(swingTimeMax * 0.7f))
                            postSwing = true;
                    }
                }

                // Make the player's arms rotate.
                ArmRotationOffset = MathHelper.ToRadians(-140f);
                ArmRotationOffsetBack = MathHelper.ToRadians(-140f);
            }
            else
            {
                // These are set to ensure the lunge looks and works properly.
                CanHit = true;
                Owner.direction = Owner.velocity.X > 0f ? 1 : -1;
                FlipAsSword = Owner.direction == -1 ? true : false;
                RotationOffset = MathHelper.PiOver4 * Owner.direction;
                Owner.itemAnimation = 2;
                Owner.fallStart = (int)(Owner.position.Y / 16f);
                Owner.Calamity().LungingDown = true;

                // Fancy particle dash effects.
                Vector2 waterVel = Owner.velocity.RotatedBy(MathHelper.PiOver2 * (Main.rand.NextBool() ? 1 : -1)).RotatedByRandom(MathHelper.Pi / 16f).SafeNormalize(Vector2.UnitX) * 5f;
                WaterFoamParticle water = new(Owner.Center, waterVel, 20, 0.45f, outlineColorBlue);
                GeneralParticleHandler.SpawnParticle(water);

                // Immediately cancel if the lunge hits a tile.
                Vector2 collisionCheckPos = Owner.Center + (dashDirection * 120 * Projectile.scale) - Vector2.One * 5f;
                if (Collision.SolidCollision(collisionCheckPos, 10, 10))
                {
                    Projectile.timeLeft = 0;
                    Owner.itemAnimation = 0;
                    Owner.Calamity().LungingDown = false;
                    Projectile.active = false;
                    Projectile.ForceNetUpdate();
                }

                Owner.velocity = dashDirection * 30f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Burst of particles to signify that the lunge is ready.
            // Due to how numHits works, this has to be 1 less than our target.
            if (Projectile.numHits == 2)
            {
                SoundEngine.PlaySound(FullChargeSound, Owner.Center);

                for (int i = 0; i < 10; i++)
                {
                    Vector2 particleVel = Vector2.UnitX.RotatedBy(MathHelper.TwoPi * (i / 10f)) * Main.rand.NextFloat(3.5f, 4f);
                    MediumMistParticle readySmoke = new(Owner.Center, particleVel, outlineColorGreen, Color.Brown, 1f, 192f);
                    GeneralParticleHandler.SpawnParticle(readySmoke);
                }
            }

            if (State == 1f)
            {
                // Biome Blade's Pure Clarity lunge gives iframes when striking enemies in a similar manner to a ram dash.
                // This is a fixed and intentionally very low number of iframes, and is not boosted by Cross Necklace.
                Owner.GiveUniversalIFrames(TrueBiomeBlade.DefaultAttunement_LungeIFrames);

                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.type == ModContent.ProjectileType<PurityProjectionSigil>() && proj.owner == Owner.whoAmI)
                    {
                        // Reset the time left on the sigil and set the new target.
                        proj.ai[0] = target.whoAmI;
                        proj.timeLeft = TrueBiomeBlade.DefaultAttunement_SigilTime;
                        return;
                    }
                }
                // If no sigil exists, spawn one.
                var source = Owner.GetSource_ItemUse(Owner.HeldItem);
                Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<PurityProjectionSigil>(), 0, 0, Owner.whoAmI, target.whoAmI);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (State == 1f)
                modifiers.SourceDamage *= TrueBiomeBlade.DefaultAttunement_LungeDamageMult;
        }

        public override void OnKill(int timeLeft)
        {
            // Cut the velocity short if dashing.
            if (State == 1f)
                Owner.velocity *= 0.33f;

            Owner.itemAnimation = 0;
            Owner.Calamity().LungingDown = false;

            Projectile.active = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Owner.itemAnimation > 0)
            {
                Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
                float r = FlipAsSword ? (State == 1f ? MathHelper.Pi : MathHelper.PiOver2) : 0f;

                // Draw extra outline sprites if you've hit once, to signify that you can lunge.
                if (Projectile.numHits > 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Color outlineColor = Color.Lerp(outlineColorGreen, outlineColorBlue, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1.5f)) * MathHelper.Clamp(Projectile.numHits / 3f, 0f, 1f);
                        Texture2D outline = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/MendedBiomeBlade_TruePureClarityGhost").Value;
                        Vector2 rotationalDrawOffset = (MathHelper.TwoPi * i / 7f + Main.GlobalTimeWrappedHourly * 17f).ToRotationVector2();
                        rotationalDrawOffset *= MathHelper.Lerp(3.25f, 6f, (float)Math.Cos(Main.GlobalTimeWrappedHourly * 4f) * 0.5f + 1.5f);
                        Main.EntitySpriteDraw(outline, Projectile.Center - Main.screenPosition + rotationalDrawOffset + new Vector2(0, Owner.gfxOffY), null, outlineColor, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(tex.Width - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
                    }
                }

                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY), tex.Frame(1, FrameCount, 0, Frame), lightColor, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(tex.Width - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
            }
            return false;
        }

        public override void ResetStyle()
        {
        }
    }
}

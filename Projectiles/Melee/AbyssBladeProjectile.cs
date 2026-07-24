using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Projectiles.Melee
{
    public class AbyssBladeProjectile : ModProjectile, ILocalizedModType
    {
        private static Asset<Texture2D> _cachedTexCircularSmearSmokey;
        private static Asset<Texture2D> _cachedTexSemiCircularSmearSwipe;

        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Items/Weapons/Melee/AbyssBlade";

        public int Time = 9000;
        public int ChargeupTime = 25;
        public int Lifetime = 300;
        public int startDamage;
        public bool setDamage = false;
        public int dustType1 = 104;
        public int dustType2 = 29;
        public bool spinMode = true;
        public Vector2 NPCDestination = new Vector2(0, 0);
        public float OverallProgress => 1 - Projectile.timeLeft / (float)Lifetime;
        public float ThrowProgress => 1 - Projectile.timeLeft / (float)(Lifetime);
        public float ChargeProgress => 1 - (Projectile.timeLeft - Lifetime) / (float)(ChargeupTime);

        public Player Owner => Main.player[Projectile.owner];
        public SlotId SpinSoundSlot;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime + ChargeupTime;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override bool ShouldUpdatePosition()
        {
            return ChargeProgress >= 1;
        }

        public override bool? CanDamage()
        {
            //We don't want the anticipation to deal damage.
            if (ChargeProgress < 1)
                return false;

            return base.CanDamage();
        }

        //Swing animation keys
        public CurveSegment pullback = new CurveSegment(EasingType.PolyOut, 0f, 0f, MathHelper.PiOver4 * -1.2f, 2);
        public CurveSegment throwout = new CurveSegment(EasingType.PolyOut, 0.7f, MathHelper.PiOver4 * -1.2f, MathHelper.PiOver4 * 1.2f + MathHelper.PiOver2, 3);
        internal float ArmAnticipationMovement() => PiecewiseAnimation(ChargeProgress, new CurveSegment[] { pullback, throwout });

        public override void AI()
        {
            if (SoundEngine.TryGetActiveSound(SpinSoundSlot, out var SpinSound) && SpinSound.IsPlaying)
                SpinSound.Position = Projectile.Center;

            float playerDist = Vector2.Distance(Owner.Center, Projectile.Center);

            Time++;
            Projectile.spriteDirection = Projectile.direction;
            Vector3 Light = new Vector3(0.070f, 0.070f, 0.250f);
            Lighting.AddLight(Projectile.Center, Light * 3);

            //Anticipation animation. Make the player look like theyre holding the depth crusher
            if (ChargeProgress < 1)
            {
                Owner.ChangeDir(MathF.Sign(Main.MouseWorld.X - Owner.Center.X));

                float armRotation = ArmAnticipationMovement() * Owner.direction;

                Owner.heldProj = Projectile.whoAmI;
                Projectile.spriteDirection = Owner.direction;
                Projectile.direction = Owner.direction;

                Projectile.Center = Owner.MountedCenter + Vector2.UnitY.RotatedBy(armRotation * Owner.gravDir) * -55f * Owner.gravDir;
                Projectile.rotation = (-MathHelper.PiOver4 * Projectile.direction + armRotation) * Owner.gravDir;

                Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, MathHelper.Pi + armRotation);

                return;
            }

            //Play the throw sound when the throw ACTUALLY BEGINS.
            //Additionally, make the projectile collide and set its speed and velocity
            if (Projectile.timeLeft == Lifetime)
            {
                Projectile.netUpdate = true;
                SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
                Projectile.Center = Owner.MountedCenter + (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.UnitX * Owner.direction) * 12;

                // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                Projectile.velocity = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.UnitX * Owner.direction) * 20;
                startDamage = Projectile.damage;
                Projectile.spriteDirection = Projectile.direction;
                SpinSoundSlot = SoundEngine.PlaySound(AbyssBlade.SpinSound, Projectile.Center);
                Time = 0;
            }

            if (Projectile.velocity.X > 0)
                Projectile.direction = 1;
            else
                Projectile.direction = -1;

            if (spinMode)
            {
                Projectile.rotation += (0.9f * (MathF.Abs(Projectile.velocity.Y) * 0.03f + 0.85f)) * Projectile.direction;
                Projectile.spriteDirection = Projectile.direction;

                if (Projectile.velocity.Y < 25)
                    Projectile.velocity.Y += 0.42f;

                if (Projectile.velocity.Y > 0)
                    Projectile.velocity.X *= 0.975f;

                for (int i = 0; i < 2; i++)
                {
                    Vector2 dustPos = Projectile.Center + (i * MathHelper.Pi + Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * 40f;
                    Dust dust = Dust.NewDustPerfect(dustPos, Main.rand.NextBool(3) ? dustType1 : dustType2, (i * MathHelper.Pi + Projectile.rotation * Math.Sign(Projectile.velocity.X)).ToRotationVector2() * 3f);
                    dust.noGravity = true;
                    dust.scale = 1.8f;
                }

                if (Collision.SolidCollision(Projectile.Center, 10, 10) && Time >= 2)
                {
                    Projectile.extraUpdates = 2;
                    Projectile.rotation = 0;
                    spinMode = false;
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/CeramicImpact", 2) { Volume = 0.65f, PitchVariance = 0.3f }, Projectile.Center);
                    for (int i = 0; i < 3; i++)
                    {
                        GenericSparkle sparker = new GenericSparkle(Projectile.Center, Vector2.Zero, Color.DodgerBlue, Color.MediumBlue, Main.rand.NextFloat(2.5f, 2.9f) - i * 0.55f, 14, Main.rand.NextFloat(-0.01f, 0.01f), 2.5f);
                        GeneralParticleHandler.SpawnParticle(sparker);
                    }
                    Projectile.ResetLocalNPCHitImmunity();
                    Projectile.penetrate = 1;
                    Projectile.damage = (int)(startDamage * 2); // Launched blade deals 200% damage
                    Time = 0;

                    bool foundTarget = false;
                    NPC target = Owner.ClampedMouseWorld().ClosestNPCAt(1000);
                    if (target != null)
                    {
                        NPCDestination = target.Center + target.velocity * 5f;
                        foundTarget = true;
                    }
                    else
                        foundTarget = false;

                    if (!foundTarget)
                    {
                        // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                        Projectile.velocity = (Owner.Calamity().mouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX * Projectile.direction) * 25;
                    }
                    else
                    {
                        Projectile.velocity = (NPCDestination - Projectile.Center).SafeNormalize(Vector2.UnitX * Projectile.direction) * 25;
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, ((Projectile.velocity).SafeNormalize(Vector2.UnitX * Projectile.direction) * 5).RotatedByRandom(0.75f) * Main.rand.NextFloat(1.4f, 2.2f), ModContent.ProjectileType<AbyssBladeSplitProjectile>(), (int)(startDamage * 0.3), Projectile.knockBack / 4, Projectile.owner);
                    }
                }
            }
            else
            {
                SpinSound?.Stop();

                Projectile.rotation = (Projectile.velocity.ToRotation() + MathHelper.PiOver4 * (Projectile.direction == 1 ? 1 : 3));

                if (Time > 9)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 dustPos = Projectile.Center - Projectile.velocity * 3 + Main.rand.NextVector2Circular(15, 15);
                        Dust dust = Dust.NewDustPerfect(dustPos, Main.rand.NextBool(3) ? dustType2 : dustType1);
                        dust.noGravity = true;
                        dust.scale = Main.rand.NextFloat(1.3f, 1.6f);
                        dust.velocity = -Projectile.velocity * Main.rand.NextFloat(0.05f, 0.7f);
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 180);
            SoundStyle HitSound = new("CalamityMod/Sounds/Custom/AbyssGravelMine2") { Volume = 0.7f, PitchVariance = 0.3f };
            if (!spinMode)
            {
                SoundEngine.PlaySound(HitSound, Projectile.Center);
                for (int i = 0; i < 30; i++)
                {
                    Vector2 dustPos = Projectile.Center;
                    Dust dust = Dust.NewDustPerfect(dustPos, Main.rand.NextBool(3) ? dustType1 : dustType2);
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(1.1f, 1.8f);
                    dust.velocity = new Vector2(3, 3).RotatedByRandom(100) * Main.rand.NextFloat(0.1f, 1.7f);
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(SpinSoundSlot, out var SpinSound))
                SpinSound?.Stop();

            for (int i = 0; i < 40; i++)
            {
                float dustMulti = Main.rand.NextFloat(0.3f, 1.5f);
                Vector2 dustPos = Projectile.Center;
                Dust dust = Dust.NewDustPerfect(dustPos, Main.rand.NextBool(3) ? dustType1 : dustType2);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(1.6f, 2.5f) - dustMulti;
                dust.velocity = new Vector2(5, 5).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 1f) * dustMulti;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> p = (_cachedTexCircularSmearSmokey ??= ModContent.Request<Texture2D>("CalamityMod/Particles/CircularSmearSmokey"));
            Asset<Texture2D> p2 = (_cachedTexSemiCircularSmearSwipe ??= ModContent.Request<Texture2D>("CalamityMod/Particles/SemiCircularSmearSwipe"));
            Vector2 generalDrawPos = Projectile.Center - Main.screenPosition;
            if (spinMode && Time < 9000)
            {
                Main.EntitySpriteDraw(p2.Value, generalDrawPos, null, Color.Blue with { A = 0 } * 0.55f, Projectile.rotation * Main.rand.NextFloat(1.6f, 1.7f), p2.Size() * 0.5f, 1.1f * Main.rand.NextFloat(0.8f, 1.15f), SpriteEffects.None);
                Main.EntitySpriteDraw(p.Value, generalDrawPos, null, Color.DodgerBlue with { A = 0 } * 0.75f, Projectile.rotation * Main.rand.NextFloat(1.2f, 1.3f), p.Size() * 0.5f, 0.85f, SpriteEffects.None);
            }
            return true;
        }
    }
}

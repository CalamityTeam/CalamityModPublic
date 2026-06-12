using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class BuzzkillHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<Buzzkill>();
        public override float RecoilResolveSpeed => 0.05f;
        public override float MaxOffsetLengthFromArm => 30f;
        public override float OffsetXUpwards => -10f;
        public override float OffsetXDownwards => 5f;
        public override float BaseOffsetY => -10f;
        public override float OffsetYDownwards => 10f;
        public override Vector2 GunTipPosition => Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.28f;

        public ref float Time => ref Projectile.ai[0];
        public const float ChargeupTime = 120f;
        public SlotId ChargeIdle;

        // Controls the saw visually disappearing from the holdout when it fires.
        public bool NoSawOnHoldout = false;

        public static Asset<Texture2D> Holdout;
        public static Asset<Texture2D> SmallSlash;
        public static Asset<Texture2D> LargeSlash;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void KillHoldoutLogic()
        {
            if (HeldItem.type != Owner.HeldItem.type)
            {
                Projectile.Kill();
                Projectile.netUpdate = true;
            }
        }

        public override void HoldoutAI()
        {
            Time++;
            float SawPower = MathHelper.Clamp(Time / ChargeupTime, 0f, 1f);

            if (SoundEngine.TryGetActiveSound(ChargeIdle, out var Idle) && Idle.IsPlaying)
                Idle.Position = GunTipPosition;

            if (Owner.CantUseHoldout())
            {
                if (Projectile.ai[1] < 1f)
                {
                    KeepRefreshingLifetime = false;
                    Idle?.Stop();

                    Projectile.ai[1] = 1f;
                    Projectile.timeLeft = Owner.HeldItem.useAnimation;
                    SoundStyle ShootSound = new("CalamityMod/Sounds/Item/SawShot", 2) { PitchVariance = 0.1f, Volume = 0.4f + SawPower * 0.5f };
                    SoundEngine.PlaySound(ShootSound, GunTipPosition);

                    int sawLevel = (SawPower >= 1f).ToInt() + (SawPower >= 0.25f).ToInt();
                    if (Main.myPlayer == Projectile.owner)
                    {
                        float sawDamageMult = MathHelper.Lerp(1f, 5f, SawPower);
                        int sawPierce = (int)MathHelper.Lerp(2f, 6f, SawPower);

                        Projectile buzzsaw = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), GunTipPosition, Projectile.velocity.SafeNormalize(Vector2.UnitY) * Owner.HeldItem.shootSpeed, ModContent.ProjectileType<BuzzkillSaw>(), (int)(Projectile.damage * sawDamageMult), (int)(Projectile.knockBack * (sawDamageMult / 2)), Main.myPlayer, sawLevel);
                        buzzsaw.penetrate = sawPierce;
                        buzzsaw.rotation = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                    }

                    NoSawOnHoldout = true;
                    OffsetLengthFromArm -= 4f + 12f * SawPower;

                    int sparkPairCount = 3 + 2 * sawLevel;
                    for (int s = 0; s < sparkPairCount; s++)
                    {
                        float velocityMult = Main.rand.NextFloat(5f, 8f) + Main.rand.NextFloat(4f, 7f) * sawLevel;
                        float scale = Main.rand.NextFloat(0.6f, 0.8f) + Main.rand.NextFloat(0.3f, 0.5f) * sawLevel;

                        Vector2 sparkVelocity = Projectile.velocity.RotatedByRandom(MathHelper.PiOver4) * velocityMult;
                        Particle weaponShootSparks = new AltLineParticle(GunTipPosition, sparkVelocity, false, 40, scale, new Color(250, 250, 107));
                        GeneralParticleHandler.SpawnParticle(weaponShootSparks);

                        // re-randomize rotation for the alternate particle
                        sparkVelocity = Projectile.velocity.RotatedByRandom(MathHelper.PiOver4) * velocityMult;
                        Particle weaponShootSparks2 = new AltSparkParticle(GunTipPosition, sparkVelocity, false, 40, scale, new Color(250, 250, 107));
                        GeneralParticleHandler.SpawnParticle(weaponShootSparks2);
                    }
                }
            }

            if (NoSawOnHoldout)
            {
                Projectile.frame = 4;
                return;
            }
            else
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 3)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame > 3)
                        Projectile.frame = 1;
                }
            }

            if (Time > 30f)
            {
                if (Time % 3 == 0)
                {
                    Vector2 sparkVel = Main.rand.NextVector2CircularEdge(1f, 1f);
                    sparkVel.SafeNormalize(Vector2.Zero);
                    sparkVel *= Main.rand.NextFloat(3f, 4.5f) + (SawPower * 4);

                    Particle buzzsawSparks = new AltLineParticle(GunTipPosition, sparkVel, false, 10, Utils.GetLerpValue(0.05f, 0.65f, SawPower, true), new Color(250, 250, 107));
                    GeneralParticleHandler.SpawnParticle(buzzsawSparks);
                }
            }

            if (Time < ChargeupTime)
            {
                if (Time == 30f)
                    ChargeIdle = SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/BuzzsawCharge") { Volume = 0.3f }, GunTipPosition);

                if (Time > 30f && Projectile.frame == 0)
                    Projectile.frame = 1;
            }
            else
            {
                if ((Time + 240) % 360 == 0)
                    ChargeIdle = SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/BuzzsawIdle"), GunTipPosition);

                if (Time % 3 == 0)
                {
                    Vector2 smokeVelocity = Vector2.UnitY * Main.rand.NextFloat(-7f, -12f);
                    smokeVelocity = smokeVelocity.RotatedByRandom(MathHelper.Pi / 8f);
                    Particle fullChargeSmoke = new HeavySmokeParticle(GunTipPosition + Main.rand.NextVector2CircularEdge(3f, 3f), smokeVelocity, Color.Gray, 30, 0.65f, 0.5f, Main.rand.NextFloat(-0.2f, 0.2f), true);
                    GeneralParticleHandler.SpawnParticle(fullChargeSmoke);
                }
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            ExtraBackArmRotation = MathHelper.ToRadians(15f);
        }

        // Failsafe because apparently the sound doesn't stop sometimes
        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(ChargeIdle, out var Idle))
                Idle?.Stop();
        }

        // The holdout can deal damage; you're literally spinning up a buzzsaw at the end, after all.
        public override bool? CanDamage() => !NoSawOnHoldout && Time % Projectile.localNPCHitCooldown == 0;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HeavyBleeding>(), 240);
            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/SwiftSlice") { Volume = 0.7f }, GunTipPosition);

            if (damageDone > 2 && !target.Calamity().IsArmored())
                Projectile.damage = (int)(Projectile.damage * 0.8f);

            int SawLevel = (Time / ChargeupTime >= 1f).ToInt() + (Time / ChargeupTime >= 0.25f).ToInt();
            int bloodCount = 4 + 3 * SawLevel;
            for (int p = 0; p < bloodCount; p++)
            {
                float radius = Main.rand.NextFloat(6f, 10f) + Main.rand.NextFloat(4f, 10f) * SawLevel;
                Vector2 velocity = Main.rand.NextVector2CircularEdge(radius, radius);
                float scale = Main.rand.NextFloat(0.3f, 0.5f) + Main.rand.NextFloat(0.1f, 0.4f) * SawLevel;
                Particle hitSparks = new AltLineParticle(target.Center, velocity, false, 20, scale, new Color(112, 16, 16));
                GeneralParticleHandler.SpawnParticle(hitSparks);
            }
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox = new Rectangle((int)GunTipPosition.X - 19, (int)GunTipPosition.Y - 20, 38, 40);

            if (Time / ChargeupTime >= 1f)
                hitbox.Inflate(65, 65);
            else if (Time / ChargeupTime >= 0.25f)
                hitbox.Inflate(28, 28);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Holdout ??= ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/BuzzkillHoldout");
            Texture2D holdoutTexture = Holdout.Value;
            LargeSlash ??= ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/BuzzkillSawLargeSlash");
            Texture2D largeSlashTexture = LargeSlash.Value;
            SmallSlash ??= ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/BuzzkillSawSmallSlash");
            Texture2D smallSlashTexture = SmallSlash.Value;
            Color slashColor = new Color(200, 200, 200, 100);

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Rectangle frame = holdoutTexture.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Vector2 rotationPoint = frame.Size() * 0.5f;
            SpriteEffects flipSprite = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            if (!NoSawOnHoldout)
            {
                float shake = Utils.Remap(Time, 0f, ChargeupTime, 0f, 3f);
                drawPosition += Main.rand.NextVector2Circular(shake, shake);
            }

            Main.EntitySpriteDraw(holdoutTexture, drawPosition, frame, Projectile.GetAlpha(lightColor), drawRotation, rotationPoint, Projectile.scale, flipSprite);

            if (Time > 30f && !NoSawOnHoldout)
            {
                if (Time / ChargeupTime >= 1f)
                    Main.EntitySpriteDraw(largeSlashTexture, GunTipPosition - Main.screenPosition, null, slashColor, Time * -MathHelper.ToRadians(42f), largeSlashTexture.Size() * 0.5f, 1f, SpriteEffects.None);

                if (Time / ChargeupTime >= 0.25f)
                    Main.EntitySpriteDraw(smallSlashTexture, GunTipPosition - Main.screenPosition, null, slashColor, Time * MathHelper.ToRadians(42f), smallSlashTexture.Size() * 0.5f, 1f, SpriteEffects.None);

                if (!CalamityClientConfig.Instance.Afterimages)
                    return false;
                
                // Special afterimage drawing for the slashes only
                for (int i = 1; i < 3; i++)
                {
                    float intensity = MathHelper.Lerp(0.05f, 0.25f, 1f - i / 3f);

                    if (Time / ChargeupTime >= 1f)
                        Main.EntitySpriteDraw(largeSlashTexture, GunTipPosition - Main.screenPosition, null, slashColor * intensity, (Time - i) * -MathHelper.ToRadians(42f), largeSlashTexture.Size() * 0.5f, 1f, SpriteEffects.None);

                    if (Time / ChargeupTime >= 0.25f)
                        Main.EntitySpriteDraw(smallSlashTexture, GunTipPosition - Main.screenPosition, null, slashColor * intensity, (Time - i) * MathHelper.ToRadians(42f), smallSlashTexture.Size() * 0.5f, 1f, SpriteEffects.None);
                }
            }

            return false;
        }
    }
}

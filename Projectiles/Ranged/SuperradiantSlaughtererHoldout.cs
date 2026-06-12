using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Cooldowns;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class SuperradiantSlaughtererHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<SuperradiantSlaughterer>();
        public override float RecoilResolveSpeed => 0.05f;
        public override float MaxOffsetLengthFromArm => 36f;
        public override float BaseOffsetY => -5f;
        public override float OffsetYUpwards => -5f;
        public override float OffsetYDownwards => 5f;
        public override Vector2 GunTipPosition => Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.25f;

        public ref float Time => ref Projectile.ai[0];
        public const float ChargeupTime = 120f;
        public SlotId ChargeIdle;

        // Controls the saw visually disappearing from the holdout when it fires.
        public bool NoSawOnHoldout = false;

        public Particle SmallSlashSmear;
        public Particle LargeSlashSmear;
        public static Asset<Texture2D> Holdout;
        public static Asset<Texture2D> HoldoutGlow;
        public static Asset<Texture2D> MiniSaw;
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
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void KillHoldoutLogic()
        {
            if (HeldItem.type != Owner.HeldItem.type || Owner.dead || !Owner.active)
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

            // Handle the right-click dash (only works when not used from left-click)
            if (Owner.Calamity().mouseRight && !Owner.HasCooldown(SuperradiantSawBoost.ID))
            {
                if (Projectile.ai[1] >= 2f)
                {
                    Owner.AddCooldown(SuperradiantSawBoost.ID, SuperradiantSlaughterer.DashCooldown);
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/MeatySlash"), GunTipPosition);

                    // Throws a lingering saw at the cursor that deals 2x damage (since the holdout already deals 1.5x)
                    if (Main.myPlayer == Projectile.owner)
                    {
                        float clampedMouseDist = MathHelper.Clamp(Vector2.Distance(GunTipPosition, Owner.Calamity().mouseWorld), 0f, 960f);
                        float adjustedMouseDist = clampedMouseDist / 21f;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, Projectile.velocity.SafeNormalize(Vector2.UnitY) * adjustedMouseDist, ModContent.ProjectileType<SuperradiantSawLingering>(), (int)(Projectile.damage * 4f / 3f), Projectile.knockBack, Projectile.owner);
                    }

                    // End animation early
                    NoSawOnHoldout = true;
                    OffsetLengthFromArm -= 16f;
                    Projectile.timeLeft = Owner.HeldItem.useAnimation;
                    KeepRefreshingLifetime = false;
                    Idle?.Stop();

                    // If moving, make particle effects when the dash activates
                    if (Owner.velocity != Vector2.Zero)
                    {
                        int particleAmt = 7;
                        for (int c = 0; c < particleAmt; c++)
                        {
                            Color sparkColor = Color.Lerp(new Color(122, 240, 58), new Color(32, 186, 171), c / (particleAmt - 1));
                            Particle spark = new CritSpark(Owner.Center, Owner.velocity.RotatedByRandom(MathHelper.ToRadians(13f)) * Main.rand.NextFloat(-2.1f, -4.5f), Color.White, sparkColor, 2f, 45, 2.25f, 2f);
                            GeneralParticleHandler.SpawnParticle(spark);
                        }
                        for (int e = 0; e < particleAmt * 2; e++)
                        {
                            Color sparkColor2 = Color.Lerp(new Color(122, 240, 58), new Color(32, 186, 171), e / (particleAmt - 1));
                            Particle spark2 = new NanoParticle(Owner.Center, Owner.velocity.RotatedByRandom(MathHelper.ToRadians(-MathHelper.PiOver4)) * Main.rand.NextFloat(2.5f, 4.5f), sparkColor2, 1f, 45, Main.rand.NextBool(3));
                            GeneralParticleHandler.SpawnParticle(spark2);
                        }
                    }
                }
            }
            else if (Owner.CantUseHoldout() && Projectile.ai[1] < 1f)
            {
                KeepRefreshingLifetime = false;
                Idle?.Stop();

                Projectile.ai[1] = 1f;
                Projectile.timeLeft = Owner.HeldItem.useAnimation;
                SoundStyle ShootSound = new("CalamityMod/Sounds/Item/SawShot", 2) { PitchVariance = 0.1f, Volume = 0.4f + SawPower * 0.5f };
                SoundEngine.PlaySound(ShootSound, GunTipPosition);

                float sawDamageMult = MathHelper.Lerp(1f, 5f, SawPower) / 1.5f; // The damage must be divided by 1.5 to offset the holdout having 1.5x base damage.
                int sawPierce = (int)MathHelper.Lerp(2f, 7f, SawPower);
                int sawLevel = (SawPower >= 1f).ToInt() + (SawPower >= 0.25f).ToInt();

                // ai[0] determines which slashes are drawn. ai[1] is the saw's timer variable. ai[2] stores the saw's pierce.
                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, Projectile.velocity.SafeNormalize(Vector2.UnitY) * SuperradiantSlaughterer.ShootSpeed, ModContent.ProjectileType<SuperradiantSaw>(), (int)(Projectile.damage * sawDamageMult), Projectile.knockBack, Projectile.owner, sawLevel, 0f, sawPierce);

                NoSawOnHoldout = true;
                OffsetLengthFromArm -= 4f + 12f * SawPower;

                int sparkPairCount = 3 + 2 * sawLevel;
                for (int s = 0; s < sparkPairCount; s++)
                {
                    float velocityMult = Main.rand.NextFloat(5f, 8f) + Main.rand.NextFloat(4f, 7f) * sawLevel;
                    float scale = Main.rand.NextFloat(0.6f, 0.8f) + Main.rand.NextFloat(0.3f, 0.5f) * sawLevel;
                    Color color = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.8f);

                    Vector2 sparkVelocity = Projectile.velocity.RotatedByRandom(MathHelper.PiOver4) * velocityMult;
                    Particle weaponShootSparks = new AltLineParticle(GunTipPosition, sparkVelocity, false, 40, scale, color);
                    GeneralParticleHandler.SpawnParticle(weaponShootSparks);

                    // re-randomize rotation for the alternate particle
                    sparkVelocity = Projectile.velocity.RotatedByRandom(MathHelper.PiOver4) * velocityMult;
                    Particle weaponShootSparks2 = new AltSparkParticle(GunTipPosition, sparkVelocity, false, 40, scale, color);
                    GeneralParticleHandler.SpawnParticle(weaponShootSparks2);
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
                        Projectile.frame = 0;
                }
            }

            // Smear particles which follow the path of the slashes
            if (SawPower >= 1f)
            {
                if (LargeSlashSmear == null)
                {
                    LargeSlashSmear = new CircularSmearVFX(GunTipPosition, Color.Black, Time * -MathHelper.ToRadians(42f), 1.35f);
                    GeneralParticleHandler.SpawnParticle(LargeSlashSmear);
                }
                else
                {
                    LargeSlashSmear.Rotation = Time * -MathHelper.ToRadians(42f);
                    LargeSlashSmear.Time = 0;
                    LargeSlashSmear.Position = GunTipPosition;
                    LargeSlashSmear.Scale = 1.35f;
                    LargeSlashSmear.Color = Main.hslToRgb(0.5f + 0.5f * MathF.Sin(Main.GlobalTimeWrappedHourly * 5f), 1f, 0.6f) * 0.8f;
                }
            }
            if (SawPower >= 0.25f)
            {
                if (SmallSlashSmear == null)
                {
                    SmallSlashSmear = new CircularSmearVFX(GunTipPosition, Color.Black, Time * MathHelper.ToRadians(42f), 0.8f);
                    GeneralParticleHandler.SpawnParticle(SmallSlashSmear);
                }
                else
                {
                    SmallSlashSmear.Rotation = Time * MathHelper.ToRadians(42f);
                    SmallSlashSmear.Time = 0;
                    SmallSlashSmear.Position = GunTipPosition;
                    SmallSlashSmear.Scale = 0.8f;
                    SmallSlashSmear.Color = Main.hslToRgb(0.5f + 0.5f * MathF.Cos(Main.GlobalTimeWrappedHourly * 5f), 1f, 0.6f) * 0.6f;
                }
            }

            if (Time < ChargeupTime)
            {
                if (Time == 30f && !NoSawOnHoldout)
                    ChargeIdle = SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/BuzzsawCharge") { Volume = 0.3f }, GunTipPosition);
            }
            else
            {
                if ((Time + 240) % 360 == 0)
                    ChargeIdle = SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/BuzzsawIdle"), GunTipPosition);

                if (Time % 3 == 0)
                {
                    Vector2 smokeVelocity = Vector2.UnitY * Main.rand.NextFloat(-7f, -12f);
                    smokeVelocity = smokeVelocity.RotatedByRandom(MathHelper.Pi / 8f);
                    Color smokeColor = Main.rand.NextBool() ? Main.DiscoColor : Color.Gray;
                    Particle fullChargeSmoke = new HeavySmokeParticle(GunTipPosition + Main.rand.NextVector2CircularEdge(3f, 3f), smokeVelocity, smokeColor, 30, 0.65f, 0.5f, Main.rand.NextFloat(-0.2f, 0.2f), true);
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

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox = new Rectangle((int)GunTipPosition.X - 23, (int)GunTipPosition.Y - 23, 46, 46);

            if (Time / ChargeupTime >= 1f)
                hitbox.Inflate(72, 72);
            else if (Time / ChargeupTime >= 0.25f)
                hitbox.Inflate(32, 32);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Laceration>(), 300);
            target.AddBuff(ModContent.BuffType<ElementalMix>(), 150);
            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/SwiftSlice") { Volume = 0.7f }, GunTipPosition);

            if (damageDone > 2 && !target.Calamity().IsArmored())
                Projectile.damage = (int)(Projectile.damage * 0.8f);

            // EPIC AND COOL RAINBOW PARTICLES WOOOO
            int SawLevel = (Time / ChargeupTime >= 1f).ToInt() + (Time / ChargeupTime >= 0.25f).ToInt();
            int onHitSparkAmount = 4 + 4 * SawLevel;
            for (int s = 0; s < onHitSparkAmount; s++)
            {
                Vector2 sparkVel = Main.rand.NextVector2CircularEdge(1f, 1f) * (Main.rand.NextFloat(6f, 10f) + 5f * SawLevel);
                float sparkSize = 0.4f + Main.rand.NextFloat(0.3f, 0.6f) * SawLevel;
                Color sparkColor = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.8f);

                Particle sparked = new AltLineParticle(target.Center, sparkVel, false, 20, sparkSize, sparkColor);
                GeneralParticleHandler.SpawnParticle(sparked);
            }
            for (int sq = 0; sq < 5; sq++)
            {
                Vector2 squareVel = Main.rand.NextVector2CircularEdge(1f, 1f)* (Main.rand.NextFloat(6f, 10f) + 5f * SawLevel);
                float squareSize = 1.6f + Main.rand.NextFloat(1f, 1.6f) * SawLevel;
                Color squareColor = Main.hslToRgb(Main.rand.NextFloat(), 0.6f, 0.8f);

                Particle squared = new SquareParticle(target.Center, squareVel, true, 20, squareSize, squareColor);
                GeneralParticleHandler.SpawnParticle(squared);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Holdout ??= ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/SuperradiantSlaughtererHoldout");
            Texture2D holdoutTexture = Holdout.Value;
            LargeSlash ??= ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/SuperradiantSawLargeSlash");
            Texture2D largeSlashTexture = LargeSlash.Value;
            SmallSlash ??= ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/SuperradiantSawSmallSlash");
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

            // Mini saw drawn under the gun
            MiniSaw ??= ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/SuperradiantSlaughtererHoldoutMiniSaw");
            Texture2D mini = MiniSaw.Value;
            Vector2 verticalOffset = Vector2.UnitY.RotatedBy(Projectile.rotation);
            if (Math.Cos(Projectile.rotation) < 0f)
                verticalOffset *= -1f;
            Vector2 miniSawPosition = drawPosition - Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.125f + verticalOffset * 6f;
            Main.EntitySpriteDraw(mini, miniSawPosition, null, Color.White, Time * MathHelper.ToRadians(24f), mini.Size() * 0.5f, Projectile.scale, flipSprite);

            Main.EntitySpriteDraw(holdoutTexture, drawPosition, frame, Projectile.GetAlpha(lightColor), drawRotation, rotationPoint, Projectile.scale, flipSprite);

            // Glowmask
            HoldoutGlow ??= ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/SuperradiantSlaughtererHoldoutGlow");
            Texture2D glow = HoldoutGlow.Value;
            Main.EntitySpriteDraw(glow, drawPosition, frame, Color.White, drawRotation, rotationPoint, Projectile.scale, flipSprite);

            if (NoSawOnHoldout)
                return false;

            if (Time > 30f)
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

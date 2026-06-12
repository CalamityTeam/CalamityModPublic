using System;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Enums;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Systems.Graphic.PixelationSystem;
using Microsoft.Build.Construction;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class DryadsTearMain : ModProjectile, ILocalizedModType, IPixelatedPrimitiveRenderer
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public ref float time => ref Projectile.localAI[1];
        public int turnDirection = 0; // Direction it will rotate
        public int turnChange = 0; // Adds variance to multiple aspects of the bullet, including when it will change its turn direction
        public float turnIntensity = 0; // Adds a bit more variance, this time to velocity
        public float damageMult = 1; // Multiplier that updates instantly, used for most of the gameplay effects
        public float visualMult = 0.75f;// Multiplier that updates over time, used for most of the visual effects
        public Vector2 dropletPosition; // The position of the orbiting teardrop
        public Vector2 lastDropletPosition; // The previous position of the orbiting teardroplet
        public Vector2 dropletVel; // The velocity of the teardrop
        public float orbitSine = 0; // A sine wave multiplier for the orbiting arc
        public static int startingUpdates = 5; // The number of extra updates the projectile starts with
        public bool dropletAttack = false; // If the bullet has begun sending it's droplet to attack
        public float dropletAttackLerp = 1; // A lerp variable used for timing the droplet's attack
        public int frameDelay = 0; // A few frames of delay after landing on the enemy before explosing, which improves the visuals
        public bool detachEffects = true; // Used for the moment when the droplet detatched from the main bullet
        public bool isMaxPower => damageMult >= 1; // If the bullet is at maximum power
        public float finalEffectMult => Projectile.scale * visualMult; // Used for many but not all visual scaling effects
        public NPC targeted; // The target being attacked by the droplet
        public ref float trueLifetime => ref Projectile.ai[2]; // The lifetime of the projectile, used over vanilla lifetime because it can increment by decimals

        public int effectSpeedMod = 0; // Used to add variance to effects when firing rapidly
        public int fadeLifetime => (int)(35 + effectSpeedMod * Math.Max(1 - damageMult, 0)); // The time in which the projectile fades out and can no longer hit anything
        public int extendedLifetime => (int)(85 * Math.Min(damageMult, 1) + effectSpeedMod * Math.Max(1 - damageMult, 0)); // The time after lifetime reaches zero where the droplet does its attack
        public bool fading => trueLifetime <= fadeLifetime; // If the projectile is fading away

        public float explosionDamageMult = 1.4f;
        public float maxDamageMult = 1.65f;
        public static Asset<Texture2D> ShineTexture { get; private set; }
        public static Asset<Texture2D> BloomTexture { get; private set; }
        public override void Load()
        {
            if (Main.dedServ)
                return;

            ShineTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/GlowSpark");
            BloomTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");
        }
        public Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 35;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.Bullet;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 300;
            Projectile.ArmorPenetration = 18;
            Projectile.extraUpdates = startingUpdates;
        }

        public override void AI()
        {
            if (dropletAttack && targeted != null && (targeted.life <= 0 || !targeted.active || !targeted.CanBeChasedBy(Projectile))) // If the chosen target becomes invalid, target a near enemy instead
                targeted = dropletPosition.ClosestNPCAt(1200); if (targeted == null) dropletAttack = false; // If that fails, cancel the teardrop effect
            Projectile.timeLeft++; // Keep regular lifetime from going down, since it uses it's own lifetime timer
            float mainBulletFadeTime = -extendedLifetime * 0.3f;
            Projectile.scale = 1 - MathF.Pow(Utils.GetLerpValue(fadeLifetime, mainBulletFadeTime, trueLifetime, true), 1.3f); // fade out the bullet scale when lifetime is at its end
            
            if (turnDirection == 0)
            {
                trueLifetime = fadeLifetime * 1.5f;
                turnDirection = Main.rand.NextBool() ? -1 : 1;
                turnChange = Main.rand.Next(30, 190 + 1);
                turnIntensity = Main.rand.NextFloat(0.2f, 1.6f);
                effectSpeedMod = Main.rand.Next(5, 35);
            }
            if (time % turnChange == 0 && !fading)
                turnDirection *= -1;

            visualMult = MathHelper.Lerp(visualMult, damageMult, 0.1f / Projectile.MaxUpdates);

            float speed = 8 * MathF.Pow(visualMult, 0.3f) * Math.Max(MathF.Pow(Utils.GetLerpValue(0, fadeLifetime, trueLifetime, true), 2), 0.15f);
            bool chanceForDust = Main.rand.NextBool((int)Math.Clamp((fading ? 8 : 5) / finalEffectMult, 3, 60));
            if (time > 12 && chanceForDust && finalEffectMult > 0.1f) // Some dust effects
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 5 + Main.rand.NextVector2CircularEdge(9, 9) * finalEffectMult, ModContent.DustType<SquashDustHollowPixelated>(), -Projectile.velocity * Main.rand.NextFloat(0.1f, 0.4f));
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.85f, 1.0f) * finalEffectMult;
                dust.color = Color.Aquamarine;
                dust.noLight = true;
                dust.noLightEmittence = true;
                dust.fadeIn = 1;
            }
            
            if (time > 0) // Reduce damage and visual intensity for every projectile of this type from the same player that are relativley close to each other
            {
                float damageReduction = 1;
                int ownedProj = Owner.ownedProjectileCounts[Projectile.type];
                for (int x = 0; x < Main.maxProjectiles; x++)
                {
                    Projectile projectile = Main.projectile[x];
                    if (projectile.owner == Projectile.owner && projectile.active && projectile.type == Projectile.type && projectile != Projectile)
                        damageReduction++;
                }
                float newDamageMult = MathF.Pow(1 / damageReduction, 0.25f);
                if (newDamageMult < damageMult && !fading)
                    damageMult = newDamageMult;
            }

            if (isMaxPower)
                damageMult = maxDamageMult;

            
            float movement = Main.rand.NextFloat(0.009f, 0.0115f) * speed;
            if (damageMult < 1)
                Projectile.velocity += Vector2.Lerp(Projectile.velocity.RotatedBy(MathHelper.PiOver2 * turnDirection).SafeNormalize(Vector2.UnitX) * movement * turnIntensity, Projectile.velocity, MathF.Pow(Math.Min(damageMult + 0.15f, 1), 3.3f));
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * speed;
            
            float startDropAttackTime = 0;
            if (dropletAttack && trueLifetime <= startDropAttackTime)
            {
                if (detachEffects)
                {
                    Projectile.Opacity = 0;
                    float scale = 0.7f * damageMult;
                    SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact with { Volume = 0.9f * damageMult, Pitch = 0.3f }, Projectile.Center);
                    for (int i = 0; i <= 2; i++)
                    {
                        CustomColorChangeSpark detach = new CustomColorChangeSpark(dropletPosition, Vector2.UnitY * 0.1f, "CalamityMod/Particles/BloomCircle", false, 15, 0.45f * scale, Color.Lime, Color.Turquoise, new Vector2(1f, 1f), true, true, colorFadeSpeed: 5, shrinkSpeed: -0.25f, extraRotation: MathHelper.PiOver2);
                        GeneralParticleHandler.SpawnParticle(detach, true);
                    }
                    for (int i = -1; i <= 1; i += 2)
                    {
                        CustomColorChangeSpark detachWave = new CustomColorChangeSpark(dropletPosition, Vector2.UnitX * 1.5f * i * scale, "CalamityMod/Particles/ArchSmear", false, 15, 0.35f * scale, Color.Turquoise, Color.Lime, new Vector2(0.8f, 1.1f), colorFadeSpeed: 5, shrinkSpeed: 0.45f);
                        GeneralParticleHandler.SpawnParticle(detachWave, true);
                    }
                    for (int i = -2; i <= 2; i += 1) // 4
                    {
                        float angle = MathHelper.PiOver4 / 2;
                        Vector2 vel = Vector2.UnitX.RotatedBy(Math.Abs(i) == 1 ? angle : -angle) * 2.2f * Math.Sign(i) * scale;
                        for (int t = -1; t <= 1; t++) // 3
                        {
                            Dust dust = Dust.NewDustPerfect(dropletPosition, ModContent.DustType<SquashDustPixelated>(), vel.RotatedBy(0.25f * t) * (2.5f - Math.Abs(t)));
                            dust.noGravity = true;
                            dust.scale = Main.rand.NextFloat(1.45f, 1.55f) * scale;
                            dust.color = t == 0 ? Color.Aquamarine : Color.Lime;
                            dust.noLight = true;
                            dust.noLightEmittence = true;
                            dust.fadeIn = 1 * scale;
                            dust.customData = new Vector2(0.6f, 3.5f);
                        }
                        if (i == -1)
                            i++;
                    }
                    detachEffects = false;
                }
                dropletAttackLerp = (1 - MathF.Pow(Utils.GetLerpValue(startDropAttackTime, -extendedLifetime, trueLifetime, true), 4)) * damageMult;
                float cappedReverseDamageMult = Math.Max(1 - damageMult, 0);
                Vector2 Xoffset = Vector2.UnitX * (targeted.width * 0.5f * turnIntensity) * cappedReverseDamageMult * turnDirection;
                Vector2 goalPosition = targeted.Center + Xoffset - (Vector2.UnitY * (targeted.height / 2 + (220 + turnChange * (0.25f + cappedReverseDamageMult)))) * dropletAttackLerp;
                Vector2 goalVel = (goalPosition - dropletPosition) / ((1 + 6 * dropletAttackLerp) * Projectile.MaxUpdates);
                dropletVel = goalVel;
                lastDropletPosition = dropletPosition;
                dropletPosition += dropletVel;
            }
            else
            {
                float sineTimer = time + turnChange;
                float sine = MathF.Sin((sineTimer / 0.5f) / MathHelper.Pi / startingUpdates);
                float sine2 = MathF.Sin((sineTimer / 1f) / MathHelper.Pi / startingUpdates);
                orbitSine = MathHelper.Lerp(Math.Abs(sine2), 0.1f, 1 - Math.Abs(sine2));
                float xOrbit = 35f * finalEffectMult;
                float yOrbit = 25f * finalEffectMult;
                if (finalEffectMult > 0.35f)
                    dropletPosition = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2) * xOrbit * sine + Projectile.velocity.SafeNormalize(Vector2.UnitX) * yOrbit * orbitSine;
                else
                    dropletPosition += (dropletVel * 3f + (Projectile.velocity.SafeNormalize(Vector2.UnitX) + dropletPosition.DirectionFrom(Projectile.Center)) * 0.75f) * finalEffectMult;

            }

            if (lastDropletPosition == Vector2.Zero) lastDropletPosition = dropletPosition;
            bool lowLifetime = trueLifetime <= mainBulletFadeTime;
            float dropletScaling = (dropletAttack ? (1 + (1 - dropletAttackLerp)) * damageMult : finalEffectMult);
            if (time > 1 * Projectile.MaxUpdates && (!lowLifetime || dropletAttack))
            {
                if (!lowLifetime)
                {
                    CustomColorChangeSpark trail = new CustomColorChangeSpark(Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.UnitX) * (20 - Math.Min(9f / finalEffectMult, 20)), -Projectile.velocity.SafeNormalize(Vector2.UnitX) * 0.1f, "CalamityMod/Particles/BloomCircle", false,
                        (int)(18 * finalEffectMult), 0.2f * finalEffectMult, Color.Turquoise, Color.Lime, new Vector2(0.25f, 2.25f * Utils.Remap(speed, 7, 0.5f, 1, 0.1f)), noShrink: true, colorFadeSpeed: 5);
                    GeneralParticleHandler.SpawnParticle(trail, true);
                }

                bool falling = trueLifetime < -extendedLifetime * 0.65f;
                if (frameDelay < 2)
                {
                    float dropLerp = MathF.Pow(Utils.GetLerpValue(startDropAttackTime, -extendedLifetime, trueLifetime, true), 2.5f);
                    if (lastDropletPosition != dropletPosition)
                        dropletVel = lastDropletPosition.DirectionTo(dropletPosition);
                    float speedMult = falling ? dropLerp * 0.5f : Utils.Remap(speed, 7, 0.5f, 1, 0.1f);
                    float colorMult = falling ? 1 - dropLerp * 0.35f : 1;
                    CustomColorChangeSpark dropTrail = new CustomColorChangeSpark(dropletPosition - dropletVel * 16.5f * speedMult * dropletScaling, falling ? dropletVel * 3 : dropletVel * 0.1f, "CalamityMod/Particles/BloomCircle", false, 
                        falling ? (int)(10 * (1 - dropLerp * 0.6f)) : (int)(6 + 2 * dropletScaling), 0.135f * dropletScaling * (1 + dropLerp * 0.3f), Color.Turquoise * colorMult, falling ? Color.Lime * colorMult : Color.Teal * colorMult, new Vector2(0.15f * (falling ? 0.5f + dropLerp * 3 : 1) * damageMult, (Math.Max(0.15f * lastDropletPosition.Distance(dropletPosition), 0.25f) * (1 - dropLerp * 0.2f))), noShrink: true, colorFadeSpeed: 5, glowOpacity: colorMult, shrinkSpeed: falling ? 1.5f - dropLerp : 0);
                    GeneralParticleHandler.SpawnParticle(dropTrail, true);

                    if (Main.rand.NextBool(3))
                    {
                        Dust dust = Dust.NewDustPerfect(dropletPosition + Main.rand.NextVector2Circular(5 + 20 * (1 - dropletAttackLerp), 5 + 20 * (1 - dropletAttackLerp)), ModContent.DustType<SquashDustPixelated>());
                        dust.noGravity = true;
                        dust.scale = Main.rand.NextFloat(1.15f, 1.55f) * 0.5f * visualMult;
                        dust.velocity = dropletVel.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.3f, 0.9f);
                        dust.color = Main.rand.NextBool() ? Color.Aquamarine : Color.Teal;
                        dust.noLight = true;
                        dust.noLightEmittence = true;
                        dust.fadeIn = Math.Max((Main.rand.NextFloat(-0.2f, -0.4f) - (Main.rand.NextBool(5) ? 0.4f : 0)) * visualMult, -0.85f);
                        dust.customData = new Vector2(1, 1);
                    }
                }
            }

            Lighting.AddLight(Projectile.Center, Color.LimeGreen.ToVector3() * 0.4f * finalEffectMult);
            Lighting.AddLight(dropletPosition, Color.Turquoise.ToVector3() * 0.25f * dropletScaling);

            lastDropletPosition = dropletPosition;
            if (trueLifetime <= 0 && (!dropletAttack || (frameDelay > 3 && trueLifetime <= -extendedLifetime)))
            {
                Splash();
                Projectile.Kill();
                return;
            }
            if (Projectile.FinalExtraUpdate())
                trueLifetime -= 1 / MathF.Pow(damageMult, 0.3f);
            if (trueLifetime <= -extendedLifetime && Projectile.FinalExtraUpdate())
                frameDelay++;
            time++;
        }
        public void Splash()
        {
            if (dropletAttack)
            {
                SoundStyle drop1 = new("CalamityMod/Sounds/Item/MittHit");
                SoundEngine.PlaySound(drop1 with { Volume = 0.8f * damageMult, Pitch = Main.rand.NextFloat(0.6f, 0.7f) }, dropletPosition);
                SoundStyle drop2 = new("CalamityMod/Sounds/Item/WaterSplash", 2);
                SoundEngine.PlaySound(drop2 with { Volume = 0.7f * damageMult, Pitch = Main.rand.NextFloat(1.4f, 1.5f) }, dropletPosition);
                if (isMaxPower)
                {
                    SoundStyle drop3 = new("CalamityMod/Sounds/Item/HolyFireBulletExplosion");
                    SoundEngine.PlaySound(drop3 with { Volume = 0.8f, Pitch = Main.rand.NextFloat(-0.5f, -0.6f) }, dropletPosition);
                }

                float scale = (isMaxPower ? 2f : 1.5f) * damageMult;
                bool reduceVisuals = damageMult < 0.5f;
                for (int i = 0; i <= (reduceVisuals ? 1 : 2); i++)
                {
                    CustomColorChangeSpark bloomHit = new CustomColorChangeSpark(dropletPosition, Vector2.UnitY * 0.1f, "CalamityMod/Particles/BloomCircle", false, 10, 0.45f * scale, Color.Lime, Color.Turquoise, new Vector2(1f, 2f), true, true, colorFadeSpeed: 5, shrinkSpeed: 0.45f, extraRotation: MathHelper.PiOver2);
                    GeneralParticleHandler.SpawnParticle(bloomHit, true);
                }
                for (int i = -1; i <= 1; i += 2)
                {
                    CustomColorChangeSpark wave = new CustomColorChangeSpark(dropletPosition, Vector2.UnitX * 7.5f * i * scale, "CalamityMod/Particles/ArchSmear", false, 12, 0.35f * scale, Color.Turquoise, Color.Lime, new Vector2(1f, 1.1f), colorFadeSpeed: 5, shrinkSpeed: 0.45f);
                    GeneralParticleHandler.SpawnParticle(wave, true);
                    CustomColorChangeSpark wave2 = new CustomColorChangeSpark(dropletPosition, Vector2.UnitX * 10.5f * i * scale, "CalamityMod/Particles/ArchSmear", false, 8, 0.35f * scale, Color.Lime, Color.Turquoise, new Vector2(0.8f, 1.1f), colorFadeSpeed: 5, shrinkSpeed: 0.55f);
                    GeneralParticleHandler.SpawnParticle(wave2, true);
                }
                for (int i = -2; i <= 2; i += 1) // 4
                {
                    float angle = MathHelper.PiOver4 / 2.5f;
                    Vector2 vel = Vector2.UnitX.RotatedBy(Math.Abs(i) == 1 ? angle : -angle) * 2.2f * Math.Sign(i) * scale;
                    for (int t = (reduceVisuals ? 0 : -1); t <= (reduceVisuals ? 0 : 1); t++) // 3
                    {
                        Dust dust = Dust.NewDustPerfect(dropletPosition, ModContent.DustType<SquashDustPixelated>(), vel.RotatedBy(0.25f * t) * (2.5f - Math.Abs(t)) * scale);
                        dust.noGravity = true;
                        dust.scale = Main.rand.NextFloat(1.25f, 1.35f) * scale;
                        dust.color = t == 0 ? Color.Aquamarine : Color.Lime;
                        dust.noLight = true;
                        dust.noLightEmittence = true;
                        dust.fadeIn = 1.4f * scale;
                        dust.customData = new Vector2(0.6f, 3.5f);
                    }
                    if (i == -1)
                        i++;
                }
                int dusts = reduceVisuals ? 4 : 10;
                for (int i = -dusts; i <= dusts; i++)
                {
                    Dust dust = Dust.NewDustPerfect(dropletPosition + Main.rand.NextVector2CircularEdge(10 - Math.Abs(i), 10 - Math.Abs(i)), ModContent.DustType<SquashDustPixelated>(), Vector2.UnitX * 2f * i * scale);
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(0.85f, 0.95f) * scale;
                    dust.color = Main.rand.NextBool() ? Color.Aquamarine : Color.Lime;
                    dust.noLight = true;
                    dust.noLightEmittence = true;
                    dust.fadeIn = 0.45f * scale;
                    dust.customData = new Vector2(0.8f, 1.3f);
                    if (i == -1)
                        i++;
                }
                if (targeted != null)
                {
                    float blastSize = 75 * scale;
                    float minMultiplier = 0.15f;
                    int hitsToMinMult = (int)(3 + 5 * damageMult);
                    int knockback = (int)(7 * damageMult * (isMaxPower ? -1 : 1)); // Give heavy knockback at full power
                    int damage = (int)(Projectile.damage * explosionDamageMult * damageMult);
                    Projectile blast = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), targeted.Center, Vector2.Zero, ModContent.ProjectileType<BasicBurst>(), damage, knockback, Owner.whoAmI, blastSize, minMultiplier, hitsToMinMult);
                    blast.timeLeft = 8;
                    blast.DamageType = DamageClass.Ranged;
                    blast.ArmorPenetration = Projectile.ArmorPenetration;
                }
                if (damageMult >= 0.75f)
                    Owner.SetScreenshake(3f * damageMult);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Projectile.extraUpdates = 2;
            int chanceReduction = (int)(2 / damageMult);
            if (isMaxPower)
            {
                SoundStyle perfectHit = new("CalamityMod/Sounds/Item/LightMetal");
                for (int i = 0; i < 3; i++)
                    SoundEngine.PlaySound(perfectHit with { Volume = 0.85f, Pitch = Main.rand.NextFloat(-1.2f, -1.3f) + 0.6f * i, MaxInstances = 3 }, dropletPosition);
            }
            if (damageMult >= 0.75f)
            {
                float angle = Main.rand.NextFloat(0, MathHelper.TwoPi);
                for (int i = -6; i <= 6; i++) // 12
                {
                    float variance = Main.rand.NextFloat(-0.5f, 0.5f);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SquashDustPixelated>(), Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(variance * 0.3f).RotatedByRandom(0.1f) * (Main.rand.NextFloat(5, 6) - Math.Abs(variance * 9)) * 5 * damageMult);
                    dust.noGravity = true;
                    dust.scale = (Main.rand.NextFloat(2.5f, 2.75f) - Math.Abs(variance)) * damageMult;
                    dust.color = Main.rand.NextBool() ? Color.Aquamarine : Color.Lime;
                    dust.noLight = true;
                    dust.noLightEmittence = true;
                    dust.fadeIn = 1.25f * damageMult;
                    dust.customData = new Vector2(0.3f, 1.7f);
                    if (i == -1)
                        i++;
                }
                float spining = Main.rand.NextFloat(-0.15f, 0.15f);
                for (int i = 0; i < 2; i++)
                {
                    CustomColorChangeSpark hit = new CustomColorChangeSpark(Projectile.Center, Vector2.UnitX.RotatedBy(angle + MathHelper.PiOver2 * i) * 0.01f, "CalamityMod/Particles/FullStar", false, 9, 3.55f * damageMult, Color.Lime, Color.Turquoise, new Vector2(1f, 1f), true, true, colorFadeSpeed: 5, shrinkSpeed: 1f, extraRotation: MathHelper.PiOver2, spin: spining);
                    GeneralParticleHandler.SpawnParticle(hit, true);
                }
            }
            dropletAttack = true;
            targeted = target;
            trueLifetime = fadeLifetime;

            modifiers.SourceDamage *= damageMult;
        }
        public override bool? CanHitNPC(NPC target) => ((time > 1) ? null : false && Projectile.numHits == 0);
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float hitboxSize = 20 * Projectile.scale * damageMult;
            return CalamityUtils.CircularHitboxCollision(Projectile.Center, hitboxSize, targetHitbox);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPosition = dropletPosition - Main.screenPosition;

            float dropletScaling = (dropletAttack ? (1 + (1 - dropletAttackLerp) * 0.7f) * damageMult : finalEffectMult);
            PixelationManager.AddPixelatedDrawer((_) =>
            {
                float dropLerp = MathF.Pow(Utils.GetLerpValue(0, -extendedLifetime, trueLifetime, true), 2.5f); ;
                int draws = 3;
                for (int i = 0; i < draws; i++)
                {
                    float circlingScale = ((i == 0 ? 0.15f : 0.3f) - orbitSine * 0.1f);
                    Main.EntitySpriteDraw(BloomTexture.Value, drawPosition, null, i == 0 ? Color.White * 0.8f : Color.Turquoise, 0, BloomTexture.Value.Size() * 0.5f, dropletScaling * (circlingScale) * (0.8f + dropLerp * 0.2f), SpriteEffects.None);
                }

                float fallTime = -extendedLifetime * 0.65f;
                float shineMult = damageMult * MathF.Pow((trueLifetime > fallTime ? Utils.GetLerpValue(0, fallTime, trueLifetime, true) : Utils.GetLerpValue(-extendedLifetime, fallTime, trueLifetime, true)), 2);
                for (int i = -1; i <= 1; i++)
                {
                    if (i == 0) i++;
                    float rot = MathHelper.PiOver2 * (i > 0 ? -1 : 1);
                    for (int t = 0; t < 2; t++)
                        Main.EntitySpriteDraw(ShineTexture.Value, drawPosition, null, t == 0 ? Color.Teal : Color.White * 0.65f, rot, ShineTexture.Value.Size() * 0.5f, new Vector2((t == 0 ? 3 : 1) * 0.02f, (t == 0 ? 0.6f : 1) * (0.2f + shineMult * 0.4f)) * shineMult * 0.2f, SpriteEffects.None);
                }
                
            }, ((orbitSine > 0.5f || trueLifetime <= 0) ? Enums.GeneralDrawLayer.AfterProjectiles : Enums.GeneralDrawLayer.BeforeProjectiles), BlendState.Additive);
            return false;
        }

        public float WidthFunction(float completion, Vector2 _)
        {
            float width;
            float sine = MathF.Sin(Main.GlobalTimeWrappedHourly * 9 - completion * 6);
            float newSine = 0.7f - sine * 0.1f;
            width = finalEffectMult * 25 * newSine * (completion < 0.4f ? 1 - MathF.Pow(Utils.GetLerpValue(0.4f, 0f, completion, true), 3) : Utils.GetLerpValue(1f, 0.4f, completion, true)) * (float)Math.Sin(Math.Acos(1 - Utils.GetLerpValue(0f, 0.08f, completion, true)));

            return width;
        }

        public Color ColorFunction(float completion, Vector2 _)
        {
            Color clr = Color.Lerp(Color.Lime, Color.Turquoise, completion + 0.3f);
            Color tipColor = Color.Lerp(clr, Color.Transparent, Utils.GetLerpValue(0.8f, 1f, completion, true));
            return Color.Lerp(clr, tipColor, completion);
        }

        public float CoreWidthFunction(float completion, Vector2 _)
        {
            float width;

            float sine = MathF.Sin(Main.GlobalTimeWrappedHourly * 9 - completion * 6);
            float newSine = 0.7f - sine * 0.2f;
            width = finalEffectMult * 20 * newSine * (completion < 0.4f ? 1 - MathF.Pow(Utils.GetLerpValue(0.4f, 0f, completion, true), 3) : Utils.GetLerpValue(1f, 0.4f, completion, true)) * (float)Math.Sin(Math.Acos(1 - Utils.GetLerpValue(0f, 0.08f, completion, true)));

            return width;
        }

        public Color CoreColorFunction(float completion, Vector2 _)
        {
            Color tipColor = Color.Lerp(Color.Lerp(Color.White, Color.Turquoise, completion + 0.65f), Color.Transparent, Utils.GetLerpValue(0.8f, 1f, completion, true));
            return Color.Lerp(Color.White, tipColor, completion);
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch, GeneralDrawLayer layer)
        {
            // Render the main trail
            string shader = "CalamityMod:TrailStreak";
            Vector2[] length = Projectile.oldPos.Take((int)(Math.Max(40 * visualMult, 14))).ToArray();
            GameShaders.Misc[shader].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/Particles/BloomRing"));
            PrimitiveRenderer.RenderTrail(length, new(WidthFunction, ColorFunction, (_, _) => Projectile.Size * 0.5f, true, true, GameShaders.Misc[shader]), length.Length * 2);

            // Render a smaller, pure white trail in the same position
            Vector2[] length2 = Projectile.oldPos.Take((int)(Math.Max(18 * visualMult, 6))).ToArray();
            GameShaders.Misc[shader].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/SylvestaffStreak"));
            PrimitiveRenderer.RenderTrail(length2, new(CoreWidthFunction, CoreColorFunction, (_, _) => Projectile.Size * 0.5f, true, true, GameShaders.Misc[shader]), length2.Length * 2);
        }
        public override void OnKill(int timeLeft)
        {
            
        }
    }
}

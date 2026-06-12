using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Systems.Graphic.PixelationSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ElephantKillerThrown : ModProjectile, ILocalizedModType
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public new string LocalizationCategory => "Projectiles.Rogue";
        public Player Owner => Main.player[Projectile.owner];
        public ref float time => ref Projectile.ai[0];
        public ref float rightClicking => ref Projectile.ai[1];
        public int initialDirection = 0;
        public float throwSpeed = 0; // The speed at which the gun is thrown and returns
        public int animationTimeMax = 0;
        public int returnTime = 0;
        public float holdoutOffset = 0; // Placement adjustments used for "recoil"
        public bool tryShooting = true; // Used in the holding mode, is used in inverse for stealth strikes
        public bool soundToss = true;
        public bool soundCatch = true;
        public bool resetIframes = true;

        public bool spinFxOn = false; // Fades in spin effects if on, otherwise fades them out
        public float spinFx = 0;
        public bool doingFireAnimation = false;
        public bool hasFiredBulletEver = false;
        public int firingEffectDisableTimer = 0; // Timer to track the firing animation and disable it when completed
        public int disableTimerMax = 4;
        public bool resetAll = false;
        public bool shouldDie = false;
        public Vector2 aimedTargetPos;

        public float recoilRotation = 0; // Rotation adjustments used for "recoil"
        public int recoilLifetime = 0;
        public int recoilLifetimeMax = 30;

        public float shineProgress = 0;
        public float shineOpacity = 0;
        public bool shineSound = true;

        public float rumble = 0; // Shake effect for stealth strikes and thrown impacts
        public int hitStop = 0; // A few frames where the projectile pauses on impact
        public int tileHits = 0;
        public bool setReturnTime = false;

        public List<(NPC, float, bool)> hitNPCs = new List<(NPC, float, bool)>();
        public static Asset<Texture2D> Gun { get; private set; }
        public static Asset<Texture2D> GunFlash { get; private set; }
        public static Asset<Texture2D> Smear { get; private set; }
        public static Asset<Texture2D> Bloom { get; private set; }
        public static Asset<Texture2D> BloomLine { get; private set; }
        public static Asset<Texture2D> Shine { get; private set; }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            Gun = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Rogue/ElephantKiller");
            GunFlash = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/ElephantKillerFlash");
            Smear = ModContent.Request<Texture2D>("CalamityMod/Particles/CircularSmearLarge");
            Bloom = ModContent.Request<Texture2D>("CalamityMod/Particles/BrightFlash");
            BloomLine = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomLineFade");
            Shine = ModContent.Request<Texture2D>("CalamityMod/Particles/FullStar");
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public enum gunState
        {
            NotThrown,
            Thrown,
            Returning,
            Shooting,
            Rebounding,
            Stealth
        }
        public gunState mode;
        public override bool ShouldUpdatePosition()
        {
            return !(mode == gunState.NotThrown || mode == gunState.Stealth || doingFireAnimation || hitStop > 0);
        }
        public override bool? CanDamage()
        {
            //We don't want the anticipation to deal damage.
            if (mode == gunState.NotThrown || mode == gunState.Stealth || doingFireAnimation || hitStop > 0)
                return false;

            return null;
        }
        public void Thrown(Vector2 toMouse, int returnTime, int animationTimeMax)
        {
            if (mode == gunState.Thrown || mode == gunState.Returning)
            {
                if (time < animationTimeMax * 1.65f)
                {
                    Owner.ChangeDir(MathF.Sign(toMouse.X));
                    Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, MathHelper.Pi * 1.5f + toMouse.ToRotation());
                }
                if (!doingFireAnimation)
                    Projectile.rotation += 0.55f * initialDirection;
                
                if (time > returnTime && mode != gunState.Returning)
                {
                    mode = gunState.Returning;
                }
            }

            if (mode == gunState.Returning)
            {
                if (setReturnTime)
                { time = returnTime; setReturnTime = false; }
                if (time > returnTime * 1.2f && resetIframes)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                        Projectile.localNPCImmunity[i] = 0;
                    resetIframes = false;
                    Projectile.tileCollide = false;
                }
                // Aim back at the player
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.Center.DirectionTo(Owner.Center) * throwSpeed, 0.03f * (1 + 2 * Utils.GetLerpValue(returnTime, returnTime * 2, time)));

                bool afterHit = (Projectile.numHits > 0 || tileHits > 0);

                if (afterHit && time > returnTime && !hasFiredBulletEver)
                    Projectile.velocity *= 0.93f;

                if (afterHit && time > returnTime * 1.4f && (!hasFiredBulletEver || (hasFiredBulletEver && doingFireAnimation)))
                {
                    NPC attemptTarget = !hasFiredBulletEver ? Projectile.Center.ClosestNPCAt(1000, true) : null;
                    if (aimedTargetPos == Vector2.Zero && attemptTarget != null)
                        aimedTargetPos = attemptTarget.Center;

                    Vector2 shootVel = aimedTargetPos == Vector2.Zero ? Projectile.rotation.ToRotationVector2() : Projectile.Center.DirectionTo(aimedTargetPos);
                    if (aimedTargetPos != Vector2.Zero)
                    {
                        if (!hasFiredBulletEver)
                        {
                            Projectile.extraUpdates = 0;
                            spinFxOn = false;
                            disableTimerMax = (int)(disableTimerMax * 1.5f);
                            FireShot(shootVel);
                        }
                        spinFx = 0;
                        Projectile.rotation = shootVel.ToRotation();
                        Projectile.Center += -shootVel * holdoutOffset * 2.3f;
                        Projectile.velocity = -shootVel * 20;
                        time--;
                    }
                }
                else
                {
                    if (Projectile.extraUpdates == 0)
                    {
                        spinFxOn = true;
                        Projectile.extraUpdates = 1;
                    }
                }

                if ((Projectile.Center - Owner.MountedCenter).Length() < 24f && time > returnTime * 1.5f)
                {
                    Projectile.Kill();
                }
            }
        }
        public void Held(Vector2 toMouse, int returnTime, int animationTimeMax)
        {
            if (mode == gunState.NotThrown || mode == gunState.Stealth) // The throw animation
            {
                float throwAnimLerp = Utils.GetLerpValue(0, animationTimeMax, time);
                float dudFireEndPoint = 0.4f; float throwStartPoint = 0.5f; float throwCutPoint = 0.75f; float maxRotation = -MathHelper.PiOver2; float startRotation = MathHelper.PiOver2;

                Owner.ChangeDir(MathF.Sign(toMouse.X));
                initialDirection = MathF.Sign(toMouse.X);
                Owner.heldProj = Projectile.whoAmI;
                if (recoilLifetime < recoilLifetimeMax - 7)
                    Owner.itemTime = Owner.itemAnimation = 5;
                if (mode == gunState.Stealth)
                {
                    Owner.itemTime = Owner.itemAnimation = 5;
                    if (!tryShooting)
                        rumble = MathHelper.Lerp(rumble, 11, 0.08f);
                }
                
                // The attempted fire at the start of the use animation
                if (throwAnimLerp >= dudFireEndPoint / 2 && ((mode != gunState.Stealth && tryShooting) || (mode == gunState.Stealth && !tryShooting)))
                {
                    float roundedStealth = MathF.Round(Owner.Calamity().rogueStealth, 2);
                    float stealthAmount = MathF.Round(Owner.Calamity().rogueStealthMax * ElephantKiller.stealthCostToShoot, 2);
                    bool fires = mode == gunState.Stealth || (Owner.Calamity().rogueStealthMax > 0 && rightClicking == 1 && roundedStealth >= stealthAmount);
                    if (fires)
                    {
                        if (mode == gunState.Stealth)
                        {
                            Owner.Calamity().ConsumeStealthByAttacking();
                            recoilLifetime++;
                        }
                        else
                        {
                            Owner.Calamity().rogueStealth -= MathF.Round(stealthAmount, 2);
                        }
                        CheckStealth();
                        FireShot(toMouse, true, ElephantKiller.stealthShotDamageMult, mode == gunState.Stealth);
                    }
                    else
                    {
                        holdoutOffset = 8;
                    }
                    SoundEngine.PlaySound(ElephantKiller.ShotFail with { Volume = 0.8f, Pitch = Main.rand.NextFloat(-0.1f, 0.1f) }, Projectile.Center);
                    tryShooting = mode == gunState.Stealth;
                }
                if (throwAnimLerp >= dudFireEndPoint)
                {
                    if (recoilLifetime < recoilLifetimeMax && mode == gunState.Stealth && recoilLifetime > 0)
                        time--;
                    else if (hasFiredBulletEver)
                    {
                        if (mode == gunState.Stealth)
                        { Projectile.Kill(); return; }
                        if (Owner.Calamity().mouseRight)
                        { Reset(); return; }
                        else
                            shouldDie = true;
                    }
                }

                if (throwAnimLerp >= throwStartPoint && soundToss)
                {
                    SoundEngine.PlaySound(ElephantKiller.Throw with { Volume = 0.9f, Pitch = Main.rand.NextFloat(-0.3f, -0.2f) }, Projectile.Center);
                    spinFx = 0.5f;
                    spinFxOn = true;
                    soundToss = false;
                }
                if (throwAnimLerp >= throwCutPoint && soundCatch)
                {
                    soundCatch = false;
                    spinFxOn = false;
                    SoundEngine.PlaySound(ElephantKiller.Catch with { Volume = 0.9f, Pitch = Main.rand.NextFloat(-0.1f, 0.1f) }, Projectile.Center);
                }

                float prepThrowLerp = Utils.Remap(throwAnimLerp, dudFireEndPoint, throwStartPoint, 0, 1);
                float aim = MathHelper.PiOver2 + MathHelper.PiOver4 * (prepThrowLerp < 0.5f ? (1 - MathF.Pow(Utils.GetLerpValue(0.5f, 0, prepThrowLerp, true), 1f)) : (1 - MathF.Pow(Utils.GetLerpValue(0.5f, 1, prepThrowLerp, true), 2.5f)));
                float pullbackLerp = CalamityUtils.EaseInOutExp(Utils.Remap(throwAnimLerp, throwStartPoint, throwCutPoint, 0, 1), 2, 4); float pullback = MathHelper.Lerp(startRotation, maxRotation, pullbackLerp);
                float throwForward = MathHelper.Lerp(startRotation, maxRotation, 1 - MathF.Pow(Utils.Remap(throwAnimLerp, throwCutPoint, 1, 0, 1), mode == gunState.Stealth ? 5 : 8.5f));

                float baseRotation = (recoilLifetime > 0 ? MathHelper.PiOver2 : (throwAnimLerp > throwStartPoint ? throwAnimLerp > throwCutPoint ? throwForward : pullback : aim)) + recoilRotation;
                float finalRotation = toMouse.ToRotation() + baseRotation * Owner.direction + (Owner.direction == -1 ? MathHelper.Pi : 0);
                float armRotation = finalRotation;

                float jumpLerp = Utils.Remap(throwAnimLerp, throwStartPoint, throwCutPoint, 0, 1);
                float jumpMult = (jumpLerp < 0.5f ? (1 - MathF.Pow(Utils.GetLerpValue(0.5f, 0, jumpLerp, true), 6)) : (1 - MathF.Pow(Utils.GetLerpValue(0.5f, 1, jumpLerp, true), 6)));
                if (jumpMult == 0)
                    spinFx = 0;
                Vector2 jump = Vector2.UnitY * -90 * (jumpMult);
                Projectile.Center = jump + Owner.MountedCenter + (Vector2.UnitY.RotatedBy(armRotation * Owner.gravDir) * (-31f + holdoutOffset) * Owner.gravDir + Vector2.UnitY.RotatedBy(armRotation + MathHelper.PiOver2 * initialDirection) * 3.5f) * Projectile.scale;

                float rotMult = 1.25f;
                if (throwAnimLerp <= throwCutPoint && throwAnimLerp >= throwStartPoint)
                    Projectile.rotation -= 0.85f * initialDirection * (jumpMult);
                else
                    Projectile.rotation = (toMouse.ToRotation() + -MathHelper.PiOver2 + ((baseRotation - MathHelper.PiOver2) * rotMult + MathHelper.PiOver2) * Owner.direction + (Owner.direction == -1 ? MathHelper.Pi : 0)) * Owner.gravDir;

                Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, MathHelper.Pi + armRotation);

                if (mode == gunState.Stealth)
                {
                    float shineLerp = Utils.Remap(throwAnimLerp, 0, dudFireEndPoint, 0, 1);
                    shineProgress = CalamityUtils.EaseInOutExp(shineLerp, 6, 6);
                    shineOpacity = soundToss ? (1 - MathF.Pow(shineLerp, 3)) : 0;
                    if (shineProgress > 0 && shineSound)
                    {
                        SoundEngine.PlaySound(ElephantKiller.Shine with { Volume = 0.6f, Pitch = 0f }, Projectile.Center);
                        shineSound = false;
                    }
                }

                if (throwAnimLerp > 1)
                {
                    if (mode == gunState.Stealth)
                    {
                        time = 0;
                        tryShooting = false;
                        return;
                    }
                    mode = gunState.Thrown;
                    Projectile.Center = Owner.MountedCenter;
                    Projectile.extraUpdates = 1;
                    spinFxOn = true;
                    Projectile.tileCollide = true;
                    Projectile.velocity = toMouse * throwSpeed;
                    SoundEngine.PlaySound(ElephantKiller.Throw with { Volume = 0.9f, Pitch = Main.rand.NextFloat(-0.1f, 0.1f) }, Projectile.Center);
                }
            }
        }
        public void Reset()
        {
            resetAll = true;
            for (int i = 0; i < Main.maxNPCs; i++)
                Projectile.localNPCImmunity[i] = 0;

            mode = gunState.NotThrown;
            Projectile.extraUpdates = 0;
            Projectile.numHits = 0;
            doingFireAnimation = false;
            hasFiredBulletEver = false;
            firingEffectDisableTimer = 0;
            throwSpeed = 0;
            holdoutOffset = 0;
            tryShooting = true;
            soundToss = true;
            resetIframes = true;
            Owner.itemTime = Owner.itemAnimation = 0;
            time = 0;
        }
        public override void AI()
        {
            if (hitStop > 0)
            { 
                Projectile.timeLeft++; hitStop--; 
                if (hitStop == 0) { Projectile.extraUpdates = 2; }
                Projectile.rotation += 0.75f * initialDirection; 
                return; 
            
            }

            if (Owner.Calamity().mouseRight || rightClicking == 1)
                rightClicking = 1;
            else
                rightClicking = 0;
            if (time == 0) // On spawn
            {
                if (!Owner.Calamity().heldElephantKillerLastFrame)
                    Owner.Calamity().rogueStealth = 0;

                resetAll = false;
                bool canStealth = Owner.Calamity().StealthStrikeAvailable();
                if (canStealth)
                    Projectile.Calamity().stealthStrike = true;
                mode = (canStealth && rightClicking == 0) ? gunState.Stealth : gunState.NotThrown;
                throwSpeed = Owner.HeldItem.shootSpeed * Owner.Calamity().rogueVelocity;
                animationTimeMax = (int)((Owner.HeldItem.useAnimation * 2) / Owner.GetTotalAttackSpeed<RogueDamageClass>());
                returnTime = 30 + animationTimeMax;
            }
            if (doingFireAnimation && firingEffectDisableTimer < disableTimerMax)
            {
                firingEffectDisableTimer++;
                spinFx = 0;
                Lighting.AddLight(Projectile.Center, Color.Khaki.ToVector3() * 0.9f);
            }
            if (firingEffectDisableTimer >= disableTimerMax)
            {
                doingFireAnimation = false;
                if (mode == gunState.Stealth)
                    recoilRotation = 0.001f;
                else
                    recoilRotation = -MathHelper.PiOver4 / 2;
                firingEffectDisableTimer = 0;
            }

            Vector2 toMouse = Owner.Center.DirectionTo(Owner.Calamity().mouseWorld);

            Held(toMouse, returnTime, animationTimeMax);

            Thrown(toMouse, returnTime, animationTimeMax);

            if (resetAll)
                return;

            if (Projectile.soundDelay <= 0 && spinFx > 0.3f) // Spin sounds
            {
                SoundEngine.PlaySound(ElephantKiller.Woosh with { Volume = 0.3f, MaxInstances = -1, Pitch = Main.rand.NextFloat(-0.1f, 0.1f) }, Projectile.Center);
                Projectile.soundDelay = 4 * Projectile.MaxUpdates;
            }

            holdoutOffset = MathHelper.Lerp(holdoutOffset, 0, 0.185f);
            if (recoilRotation != 0)
                recoilRotation = MathHelper.Lerp(recoilRotation, mode == gunState.Stealth ? (-MathHelper.PiOver4 * (1.3f + 0.03f * recoilLifetime)) : 0, 0.245f);
            if (!doingFireAnimation)
                spinFx = MathHelper.Lerp(spinFx, spinFxOn ? 1 : 0, 0.28f);
            Projectile.scale = MathHelper.Lerp(Projectile.scale, shouldDie ? 0 : 1, 0.28f);
            if (recoilLifetime > 0)
                rumble = MathHelper.Lerp(rumble, 0, 0.06f);
            if (shouldDie)
            {
                if (Projectile.scale <= 0.05f)
                { Projectile.Kill(); return; }
                time--;
            }
            if (tileHits >= 3)
                Projectile.tileCollide = false;

            Projectile.timeLeft++;
            if (recoilLifetime > 0)
                recoilLifetime++;
            time++;
        }
        public void FireShot(Vector2 velocity, bool recoil = false, float damageMult = 1, bool elephant = false)
        {
            holdoutOffset = 15;
            Owner.SetScreenshake(3.5f + damageMult);
            doingFireAnimation = true;
            hasFiredBulletEver = true;

            SoundEngine.PlaySound(ElephantKiller.Shot with { Volume = 0.9f, Pitch = Main.rand.NextFloat(-0.1f, 0.1f) - (elephant ? 0.6f : 0), MaxInstances = 3 }, Projectile.Center);
            if (elephant)
                SoundEngine.PlaySound(ElephantKiller.Shot with { Volume = 0.9f, Pitch = 0.4f, MaxInstances = 3 }, Projectile.Center);
            if (recoil)
                Owner.velocity -= velocity * 2.25f * damageMult * (elephant ? 1.5f : 1);

            if (elephant)
            {
                for (int x = 0; x < Main.maxProjectiles; x++)
                {
                    Projectile projectile = Main.projectile[x];
                    if (projectile.owner == Projectile.owner && projectile.active && projectile.type == ModContent.ProjectileType<ElephantKillerElephant>())
                    {
                        projectile.timeLeft = 90;
                    }
                }
                Projectile.NewProjectileDirect(Owner.GetSource_FromThis(), Projectile.Center, velocity * Owner.HeldItem.shootSpeed * 2, ModContent.ProjectileType<ElephantKillerElephant>(), 0, 0, Owner.whoAmI);
            }
            else
            {
                float elephantDistance = 0;
                bool elephantBoosted = false;
                Vector2 redirectPoint = Vector2.Zero;
                Vector2 redirectVelocity = Vector2.Zero;

                // Check if you shot an elephant
                for (int x = 0; x < Main.maxProjectiles; x++)
                {
                    Projectile projectile = Main.projectile[x];
                    if (projectile.active && projectile.type == ModContent.ProjectileType<ElephantKillerElephant>())
                    {
                        float _ = float.NaN;
                        bool elephantCollide = Collision.CheckAABBvLineCollision(projectile.Hitbox.TopLeft(), projectile.Hitbox.Size(), Projectile.Center, Projectile.Center + velocity * 2500, 10 * Projectile.scale, ref _);
                        if (Projectile.Hitbox.Intersects(projectile.Hitbox))
                        {
                            elephantCollide = true;
                            _ = Projectile.width;
                        }
                        if (elephantCollide)
                        {
                            elephantDistance = _;
                            Vector2 hitPoint = Projectile.Center + velocity * _;
                            elephantBoosted = true;
                            Vector2 adjustedHitPoint = hitPoint + velocity * 15;
                            projectile.ai[1] = adjustedHitPoint.X;
                            projectile.ai[2] = adjustedHitPoint.Y;
                            projectile.localAI[1] = 0.5f + damageMult;

                            redirectPoint = hitPoint;
                            NPC redirectTarget = hitPoint.ClosestNPCAt(2500, true);
                            redirectVelocity = redirectTarget == null ? velocity : hitPoint.DirectionTo(redirectTarget.Center);

                            projectile.localAI[0] = redirectVelocity.ToRotation();

                            makeHitEffects(velocity, 2, hitPoint, projectile.whoAmI, false, 1, redirectVelocity.ToRotation());
                        }
                    }
                }

                for (int y = 0; y < (elephantBoosted ? 2 : 1); y++)
                {
                    bool redirected = y == 1;
                    Vector2 lineCheckVelocity = (!redirected ? velocity : redirectVelocity);
                    Vector2 startPoint = (!redirected ? Projectile.Center : redirectPoint);
                    Vector2 endPoint = (elephantBoosted && !redirected) ? redirectPoint : (startPoint + lineCheckVelocity * 2500);

                    for (int index = 0; index < Main.npc.Length; index++)
                    {
                        NPC target = Main.npc[index];
                        if (!target.dontTakeDamage && target.active && target.life > 0 && !target.townNPC)
                        {
                            float _ = float.NaN;
                            bool collides = Collision.CheckAABBvLineCollision(target.Hitbox.TopLeft(), target.Hitbox.Size(), startPoint, endPoint, 10 * Projectile.scale, ref _);
                            if (Projectile.Hitbox.Intersects(target.Hitbox))
                            {
                                collides = true;
                                _ = Projectile.width;
                            }
                            if (collides)
                                hitNPCs.Add((target, ((!redirected ? 0 : elephantDistance) + _), redirected));
                        }
                    }
                }
                bool resetHits = false;
                hitNPCs = hitNPCs.OrderBy(x => x.Item2).ToList(); // Order hit NPCs by distance to hit them in order
                for (int index = 0; index < hitNPCs.Count(); index++)
                {
                    NPC target = hitNPCs.ElementAt(index).Item1;
                    float distance = hitNPCs.ElementAt(index).Item2;
                    bool isAfterElephantHit = hitNPCs.ElementAt(index).Item3;

                    if (isAfterElephantHit && !resetHits) // Getting the redirect resets num hits
                    { resetHits = true; Projectile.numHits = 0; }

                    float minMult = 0.1f;
                    int hitsToMinMult = 5;
                    float damageFalloff = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
                    float adjustedDamageMult = damageMult * damageFalloff;

                    Vector2 vel = (isAfterElephantHit ? redirectVelocity : velocity);

                    Vector2 hitPoint = (isAfterElephantHit ? redirectPoint : Projectile.Center) + vel * (distance - (isAfterElephantHit ? elephantDistance : 0)); // maybe here?

                    int damage = (int)((Projectile.damage / 2) * (isAfterElephantHit ? ElephantKiller.elephantBoostedShotDamageMult * adjustedDamageMult : adjustedDamageMult)); // Bullets deal half of gun damage, but always crit
                    Projectile bullet = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<Gunshot>(), damage, 0, Owner.whoAmI, target.whoAmI);
                    bullet.DamageType = RogueDamageClass.Instance;
                    bullet.Calamity().stealthStrike = isAfterElephantHit; // Turn bullets shot through the Elephant into stealth strikes

                    makeHitEffects(vel, adjustedDamageMult, hitPoint, elephantBoosted: isAfterElephantHit, effectMult: damageFalloff);
                    
                    Projectile.numHits++;
                }
                hitNPCs.Clear();
            }
        }
        public void makeHitEffects(Vector2 velocity, float damageMult, Vector2 hitPoint, int shotElephantID = -50, bool elephantBoosted = false, float effectMult = 1, float exitAngle = -1000)
        {
            bool shotElephant = shotElephantID != -50;
            bool blue = (shotElephant || !ChildSafety.Disabled);
            Color flashColor = (elephantBoosted || shotElephant) ? Color.CornflowerBlue : Color.Khaki;
            if (elephantBoosted)
            {
                velocity *= 1.3f;
                Owner.SetScreenshake(6f);

                for (int i = 0; i < (damageMult == ElephantKiller.stealthShotDamageMult ? 2 : 1); i++)
                    SoundEngine.PlaySound(ElephantKiller.BoostedShotHit with { Volume = 0.2f + 0.8f * effectMult, Pitch = Main.rand.NextFloat(-0.1f, 0.1f) - 0.5f * (1 - effectMult), MaxInstances = -1 }, hitPoint);
            }
            for (int y = 0; y < ((shotElephant || elephantBoosted) ? 2 : 1); y++)
            {
                if (y == 1 && shotElephant)
                {
                    velocity = exitAngle.ToRotationVector2();
                    if (!elephantBoosted)
                    {
                        float lastCollisionDistance = float.NaN;
                        Projectile elephant = Main.projectile[shotElephantID];
                        Vector2 lineStartPoint = hitPoint + velocity.SafeNormalize(Vector2.UnitX) * 600;
                        bool collides = Collision.CheckAABBvLineCollision(elephant.Hitbox.TopLeft(), elephant.Hitbox.Size(), lineStartPoint, hitPoint, 10 * Projectile.scale, ref lastCollisionDistance);

                        hitPoint = lineStartPoint - velocity.SafeNormalize(Vector2.UnitX) * lastCollisionDistance;
                    }
                    velocity *= -1;
                }
                Vector2 displayHitPoint = hitPoint;
                if (y == 0)
                {
                    Particle bloom = new CustomSpark(displayHitPoint - velocity * 8, Vector2.Zero, "CalamityMod/Particles/BrightFlash", false, Main.rand.Next(7, 8 + 1), Main.rand.NextFloat(0.3f, 0.35f) * 1.5f * damageMult, flashColor, new Vector2(1f, 1f), true, true, glowOpacity: 0.7f, colorFadeSpeed: 10, extraRotation: Main.rand.NextFloat(0, MathHelper.TwoPi));
                    GeneralParticleHandler.SpawnParticle(bloom, true);
                }
                if (elephantBoosted && y == 0)
                {
                    Vector2 spawnPoint = displayHitPoint - velocity.SafeNormalize(Vector2.UnitX) * 16;
                    float spins = Main.rand.NextFloat(0.02f, 0.06f) * (Main.rand.NextBool() ? 1 : -1);
                    for (int i = 0; i <= 1; i++) // twice
                    {
                        Particle bloom2 = new CustomSpark(spawnPoint, Vector2.Zero, "CalamityMod/Particles/HalfStar", false, 10, 4f * damageMult, flashColor, new Vector2(1.5f, 0.4f), true, true, glowOpacity: 0.7f, colorFadeSpeed: 10, shrinkSpeed: 1.5f, extraRotation: MathHelper.PiOver2 * i, spin: spins);
                        GeneralParticleHandler.SpawnParticle(bloom2, true);
                    }
                    float scale = 0.01f;
                    Particle pulse = new CustomPulse(spawnPoint, Vector2.Zero, flashColor, "CalamityMod/Particles/HigResThinCircle", Vector2.One, 0, damageMult * scale, 4f * damageMult * scale, 35);
                    GeneralParticleHandler.SpawnParticle(pulse, true);
                    Particle pulse2 = new CustomPulse(spawnPoint, Vector2.Zero, Color.SkyBlue, "CalamityMod/Particles/HigResThinCircle", Vector2.One, 0, damageMult * scale, 6f * damageMult * scale, 22);
                    GeneralParticleHandler.SpawnParticle(pulse2, true);
                }
                
                if ((!shotElephant && y == 0) || (y == 1 && !elephantBoosted))
                {
                    for (int i = 0; i < (int)(12 * damageMult); i++)
                    {
                        Vector2 bloodVel = !shotElephant ? velocity : -velocity;
                        Color clr = blue ? (Main.rand.NextBool(3) ? Color.SkyBlue : Color.CornflowerBlue) : (Main.rand.NextBool(3) ? Color.DarkRed : Color.Maroon);

                        int dustType = blue ? ModContent.DustType<SquashDustPixelated>() : DustID.Blood;
                        bool spread = Main.rand.NextBool();
                        Vector2 dustVel = bloodVel.RotatedBy(spread ? ((Main.rand.NextBool() ? -1 : 1) * Main.rand.NextFloat(0.2f, 0.6f)) : 0) * Main.rand.NextFloat(8.5f, 18.5f) * damageMult * (spread ? 0.6f : 1);
                        Dust dust = Dust.NewDustPerfect(displayHitPoint + (spread? Vector2.Zero : Main.rand.NextVector2Circular(6, 6)) * damageMult, dustType, dustVel, Main.rand.Next(10, 120), default, Main.rand.NextFloat(0.9f, 1.4f) * damageMult);
                        dust.noGravity = true;
                        if (blue)
                        {
                            dust.alpha = 200;
                            dust.scale *= 0.75f;
                            dust.fadeIn = 2.5f;
                            dust.color = clr;
                            dust.noLight = true;
                            dust.noLightEmittence = true;
                        }
                        float speedMult = Main.rand.NextFloat(0, 1f);

                        Particle spark = new CustomSpark(displayHitPoint, bloodVel.RotateRandom((1 - speedMult) * 0.12f) * 10 * (speedMult + 0.25f) * damageMult, "CalamityMod/Particles/" + (Main.rand.NextBool() ? "FadeLine" : "GlowOrbParticle"), false, Main.rand.Next(7, 11 + 1), Main.rand.NextFloat(0.2f, 0.4f) * 2f * damageMult, clr, new Vector2(1.5f, 1f), blue, shrinkSpeed: 0.6f, colorFadeSpeed: 10, affectedByLight: !blue);
                        GeneralParticleHandler.SpawnParticle(spark, true);
                    }
                }
                
                int effects = 4;
                for (int i = -effects; i <= effects; i++)
                {
                    if (i == 0) i++;
                    bool glow = MathF.Abs(i) == 2;
                    Color clr = shotElephant ? (Main.rand.NextBool(3, 4) ? Color.DodgerBlue : Color.CornflowerBlue) : (Main.rand.NextBool(3, 4) ? new Color(114, 112, 116) : new Color(216, 216, 216));
                    Vector2 vel = -velocity.RotatedBy(MathHelper.PiOver2 * (elephantBoosted ? 0.1f : 0.4f) * (y == 1 ? 0.25f : 1) * Math.Sign(i)).RotatedByRandom(glow ? 0 : 0.35f) * Main.rand.NextFloat(5, 14);
                    Particle spark = new CustomSpark(displayHitPoint + vel * (glow ? 5f : 3f) * damageMult, vel * (y == 1 ? 1.5f : 1), "CalamityMod/Particles/" + (!glow ? "FadeLine" : "ForwardSmear"), false, (int)(Main.rand.Next(29, 35 + 1) * (glow ? 0.3f : 1)),
                        Main.rand.NextFloat(0.2f, 0.3f) * (glow ? 0.3f : 2f) * damageMult, glow ? flashColor : clr, new Vector2(glow ? 1.5f : 0.6f, 1f), glow || shotElephant, shrinkSpeed: glow ? 0.7f : 0.35f, colorFadeSpeed: 10);
                    GeneralParticleHandler.SpawnParticle(spark, true);
                }
            }
        }
        public void CheckStealth()
        {
            if (Owner.Calamity().rogueStealth > Owner.Calamity().rogueStealthMax)
                Owner.Calamity().rogueStealth = Owner.Calamity().rogueStealthMax;
            if (Owner.Calamity().rogueStealth < 0)
                Owner.Calamity().rogueStealth = 0;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.numHits == 0)
            {
                if (tileHits == 0)
                {
                    throwCollideEffects();
                    Projectile.extraUpdates = 0;
                    setReturnTime = true;
                    mode = gunState.Returning;
                    Projectile.velocity = new Vector2(-Math.Sign(Projectile.Center.DirectionTo(target.Center).X) * Projectile.velocity.Length() / 2, -18 * Math.Sign(Projectile.Center.DirectionTo(target.Center).Y));
                }
                Owner.Calamity().rogueStealth += MathF.Round(Owner.Calamity().rogueStealthMax * ElephantKiller.stealthGainOnThrowHit, 2); // Give stealth on hit
                CheckStealth();
            }
        }
        public void throwCollideEffects()
        {
            hitStop = Math.Max(6 - tileHits * 2, 0);
            if (hitStop >= 2)
            {
                Particle pulse = new CustomPulse(Projectile.Center, Vector2.Zero, new Color(114, 112, 116), "CalamityMod/Particles/HigResThinCircle", Vector2.One, 0, 0.04f, 0.05f, hitStop, false);
                GeneralParticleHandler.SpawnParticle(pulse, true);
            }

            SoundEngine.PlaySound(ElephantKiller.Hit with { Volume = 0.7f, Pitch = Main.rand.NextFloat(-0.1f, 0.1f) + tileHits * 0.1f, MaxInstances = -1 }, Projectile.Center);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            throwCollideEffects();
            tileHits++;
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float damageMult = MathHelper.Clamp(Utils.GetLerpValue(8, 1, Projectile.numHits), 0.4f, 1);
            modifiers.SourceDamage *= damageMult;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> tex = doingFireAnimation ? GunFlash : Gun;
            Asset<Texture2D> smear = Smear;
            Asset<Texture2D> flash = Bloom;
            Asset<Texture2D> flashTip = BloomLine;
            Asset<Texture2D> shineTip = Shine;

            Vector2 generalDrawPos = Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(rumble, rumble) + 
                ((mode == gunState.NotThrown || mode == gunState.Stealth) ? new Vector2(0, Owner.gfxOffY) : Vector2.Zero);
            PixelationManager.AddPixelatedDrawer((_) => // The spinnging smear when thrown
            {
                if (spinFx > 0.001f)
                {
                    float scale = 0.19f;
                    Color dark = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).MultiplyRGB(new Color(24, 20, 37));
                    Color light = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).MultiplyRGB(new Color(114, 112, 116));
                    Color shine = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).MultiplyRGB(new Color(216, 216, 216));
                    Main.EntitySpriteDraw(smear.Value, generalDrawPos, null, light * spinFx, Projectile.rotation * Main.rand.NextFloat(3.9f, 4.2f), smear.Size() * 0.5f, Projectile.scale * 0.55f * scale * Main.rand.NextFloat(0.95f, 1.05f), SpriteEffects.None);
                    Main.EntitySpriteDraw(smear.Value, generalDrawPos, null, shine * spinFx, Projectile.rotation * Main.rand.NextFloat(1.2f, 1.3f), smear.Size() * 0.5f, Projectile.scale * 0.45f * scale, SpriteEffects.None);
                    Main.EntitySpriteDraw(smear.Value, generalDrawPos, null, dark * spinFx, Projectile.rotation * Main.rand.NextFloat(0.7f, 0.8f), smear.Size() * 0.5f, Projectile.scale * 0.4f * scale * Main.rand.NextFloat(0.95f, 1.05f), SpriteEffects.None);
                }
            }, Enums.GeneralDrawLayer.BeforeProjectiles, BlendState.NonPremultiplied);

            Main.EntitySpriteDraw(tex.Value, generalDrawPos + Main.rand.NextVector2Circular(hitStop * 2, hitStop * 2), null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, initialDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0);
            
            if (doingFireAnimation) // The muzzle flash effect when firing
            {
                PixelationManager.AddPixelatedDrawer((_) =>
                {
                    float scale = (0.35f + firingEffectDisableTimer * 0.22f) * (mode == gunState.Stealth ? 2 : 1);
                    float flashFade = !doingFireAnimation ? 0 : MathF.Pow(Utils.GetLerpValue(disableTimerMax, 0, firingEffectDisableTimer, true), 3);
                    float flashMult = Utils.GetLerpValue(0, disableTimerMax, firingEffectDisableTimer, true) * 5;

                    Vector2 vel = Projectile.rotation.ToRotationVector2();
                    Vector2 gunTipPostion = generalDrawPos + vel * (30 + flashMult * 4.5f) - vel.RotatedBy(MathHelper.PiOver2 * initialDirection) * 5;
                    for (int i = 0; i < 2; i++)
                        Main.EntitySpriteDraw(flash.Value, gunTipPostion, null, Color.Khaki with { A = 0 } * 0.8f * Utils.GetLerpValue(disableTimerMax, 0, firingEffectDisableTimer, true), Projectile.rotation, flash.Size() * 0.5f, Projectile.scale * scale * (i == 0 ? 1 : 0.75f), SpriteEffects.None);
                    for (int i = -1; i <= 1; i++) // 3
                    {
                        Vector2 squash = new Vector2(0.9f * flashFade, 0.9f + 0.9f * (1 - flashFade) - Math.Abs(i) * 0.45f) * 0.15f;
                        Main.EntitySpriteDraw(flashTip.Value, gunTipPostion, null, Color.Khaki with { A = 0 } * (0.6f - 0.15f * flashMult), Projectile.rotation + MathHelper.PiOver2 + MathHelper.PiOver4 * i * flashFade, new Vector2(flashTip.Width() / 2, flashTip.Height()), squash * Projectile.scale * scale, SpriteEffects.None);
                    }
                }, Enums.GeneralDrawLayer.AfterProjectiles, default);
            }
            if (mode == gunState.Stealth) // The shine effect that occurs before firing an elephant
            {
                PixelationManager.AddPixelatedDrawer((_) =>
                {
                    float scale = 0.5f + 1.3f * shineProgress;

                    Vector2 vel = Projectile.rotation.ToRotationVector2();
                    Vector2 gunTipPostion = generalDrawPos + vel * (MathHelper.Lerp(-15, 30, shineProgress)) - vel.RotatedBy(MathHelper.PiOver2 * initialDirection) * 9;

                    for (int i = -1; i <= 1; i++)
                    {
                        Vector2 squash = new Vector2(1f - 0.7f * i * shineProgress, 1 + 0.7f * i * shineProgress) * scale;
                        Main.EntitySpriteDraw(shineTip.Value, gunTipPostion, null, (i == 0 ? Color.White : new Color(216, 216, 216)) with { A = 0 } * 0.8f * shineOpacity, Projectile.rotation + MathHelper.Pi * shineProgress, shineTip.Size() * 0.5f, squash * Projectile.scale * (i == 0 ? 0.5f : 1f), SpriteEffects.None);
                    }
                }, Enums.GeneralDrawLayer.AfterEverything, default);
            }
            
            return false;
        }
    }
}

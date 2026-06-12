using System;
using System.Collections.Generic;
using CalamityMod.Dusts;
using CalamityMod.Enums;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.NPCs;
using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.Particles;
using CalamityMod.Systems.Graphic.PixelationSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static Terraria.Player;

namespace CalamityMod.Projectiles.Melee
{
    [PierceResistException]
    public class GrandDadHoldout : ModProjectile, ILocalizedModType
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<GrandDad>();
        public override string Texture => "CalamityMod/Items/Weapons/Melee/GrandDad";

        public ref float attackTimer => ref Projectile.ai[0];
        public Player Owner => Main.player[Projectile.owner];
        public int fireRate => (int)((Owner.HeldItem.useAnimation * Owner.inverseMeleeSpeed * Projectile.MaxUpdates) / 2.75f);
        public int time = 0;
        public Vector2 toMouse = Vector2.Zero;
        public int cooldown => (int)(-Owner.HeldItem.useAnimation * Owner.inverseMeleeSpeed * Projectile.MaxUpdates);
        public float bladeRot = 0; // Rotation of the blade
        public int swingCount = 0; // Increases at the start of a swing
        public float bladefx = 0; // Visual effects of the blade
        // Checks to make sure sounds only happen once per swing
        public bool makeSound = true;
        public bool performSwing = false;
        public float enabledHitbox = 0; // Deals damage when above 0, ticks down quickly
        public float angledHold = 0; // Adds some angle based on swing direction
        public float angleArmHold = 0;
        public static int hitboxFramesMax = 11;
        public bool canReleaseHoldout = false;
        public int armoredHits = 0;

        public float startAngle = MathHelper.Pi * 0.9f;
        public float endAngle = -MathHelper.PiOver4 * 3 * 0.9f;
        public override bool? CanDamage() => enabledHitbox > 0 ? null : false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float outset = 40 * Projectile.scale;
            Vector2 hitboxPosition = Owner.Center + toMouse * outset;
            float hitboxSize = 155 * Projectile.scale;
            return CalamityUtils.CircularHitboxCollision(hitboxPosition, hitboxSize, targetHitbox);
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 124;
            Projectile.friendly = true;
            Projectile.DamageType = TrueMeleeDamageClass.Instance;
            Projectile.timeLeft = 5;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 8;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.noEnchantmentVisuals = true;
            Projectile.scale = 0;
        }
        public void OnSpawn()
        {
            if (time == 0)
            {
                Projectile.knockBack = 0;
                attackTimer = cooldown;
                swingCount = 1;
            }
        }
        public void ResetVariables() // After a swing, reset/adjust many stats
        {
            for (int i = 0; i < Main.maxNPCs; i++)
                Projectile.localNPCImmunity[i] = 0;
            Projectile.numHits = 0;
            swingCount++;
            makeSound = true;
            enabledHitbox = 0;
            canReleaseHoldout = false;
            armoredHits = 0;
            Projectile.ForceNetUpdate();
        }
        public void Positioning(Vector2 toMouse) // Hand and holdout positioning
        {
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemRotation = (Projectile.velocity * Owner.direction).ToRotation();

            angleArmHold = MathHelper.Lerp(angleArmHold, angledHold * MathF.Pow(Utils.GetLerpValue(cooldown, cooldown * 0.35f, attackTimer, true), 5), 0.12f / Projectile.MaxUpdates * Owner.meleeSpeed);
            float angleVariation = (float)Math.Sin(time * 0.04f / Projectile.MaxUpdates) * 0.05f;

            float outwardDistance = 5;
            Owner.ChangeDir(Math.Sign(toMouse.X));
            Owner.SetCompositeArmFront(true, CompositeArmStretchAmount.Full, (toMouse.ToRotation() + (bladeRot + angleArmHold + angleVariation) * Owner.direction + MathHelper.PiOver2 * -Owner.direction) + (Owner.direction == -1 ? MathHelper.Pi : 0));
            Vector2 handPos = Owner.GetFrontHandPosition(CompositeArmStretchAmount.None, Owner.compositeFrontArm.rotation) + (Owner.compositeFrontArm.rotation + MathHelper.PiOver2).ToRotationVector2() * outwardDistance;
            float armRotation = (Utils.DirectionTo(Owner.Center, handPos).ToRotation() - MathHelper.PiOver2) * Owner.gravDir + (Owner.gravDir == -1 ? MathHelper.Pi : 0f);
            Owner.SetCompositeArmBack(true, CompositeArmStretchAmount.Full, armRotation);

            Projectile.velocity = toMouse.RotatedBy((bladeRot + angledHold + angleVariation) * Owner.direction);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center = handPos;
        }
        public override void AI()
        {
            if (!Owner.CantUseHoldout(false))
                Projectile.timeLeft = 5;

            Projectile.scale = MathHelper.Lerp(Projectile.scale, Owner.GetMeleeScale(), 0.3f / Projectile.MaxUpdates);
            toMouse = Utils.DirectionTo(Owner.Center, Owner.ClampedMouseWorld());

            Positioning(toMouse);

            OnSpawn();

            bool fadeInFX = enabledHitbox > (int)(hitboxFramesMax / 3f);
            if (enabledHitbox > 0)
            {
                if (Projectile.FinalExtraUpdate())
                    enabledHitbox--;
            }
            if (fadeInFX)
                bladefx = MathHelper.Lerp(bladefx, 1, 0.25f / Projectile.MaxUpdates * Owner.meleeSpeed);
            else
                bladefx = MathHelper.Lerp(bladefx, 0, 0.15f / Projectile.MaxUpdates * Owner.meleeSpeed);

            if (canReleaseHoldout || time < 25)
            {
                if (Owner.HeldItem.type != ItemType<GrandDad>() || Main.mapFullscreen || Owner.mouseInterface || Owner.dead)
                {
                    Projectile.Kill();
                    return;
                }
            }
            else
                Owner.itemTime = Owner.itemAnimation = 5;

            
            float lerpSpeed = 0.1f / Projectile.MaxUpdates * Owner.meleeSpeed;
            float maxAngle = MathHelper.PiOver4 * 0.8f;
            if (swingCount % 2 == 0)
                angledHold = MathHelper.Lerp(angledHold, maxAngle, lerpSpeed);
            else
                angledHold = MathHelper.Lerp(angledHold, -maxAngle, lerpSpeed);
            

            #region Not Swinging
            if (attackTimer < 0) // When the sword isn't swinging
            {
                Timers();

                float lerp = Utils.GetLerpValue(cooldown, -1, attackTimer, true);

                if (attackTimer == 0) // Reset a few things in prep for the next swing
                    ResetVariables();
                else // Wind up animation in prep for the next swing
                {
                    int swingDir = (swingCount % 2 != 0 ? 1 : -1);
                    float lerp2 = Utils.GetLerpValue(cooldown, -1, attackTimer, true);
                    float start = -startAngle * swingDir;
                    float end = endAngle * swingDir;
                    bladeRot = MathHelper.Lerp(start, end, CalamityUtils.EaseInOutExp(lerp2, 2f, 2f));

                    if (lerp >= 0.85f)
                    {
                        if (Owner.controlUseItem || performSwing)
                            performSwing = true;
                        else
                        {
                            canReleaseHoldout = true;
                            attackTimer--;
                            lerp = 0.85f;
                        }
                    }
                }
                if (lerp > 0.85f)
                {
                    angledHold = MathHelper.Lerp(angledHold, maxAngle * 3.5f * (swingCount % 2 == 0 ? 1 : -1), lerpSpeed * 1.5f);
                }

                return;
            }
            #endregion

            if (attackTimer >= fireRate) // Once the swing is done, put it on cooldown
            {
                attackTimer = cooldown;
                performSwing = false;
            }
            else // Do the swing
            {
                #region Swinging
                int swingDir = (swingCount % 2 != 0 ? -1 : 1);
                float lerp = Utils.GetLerpValue(0, fireRate - 1, attackTimer, true);
                float start = endAngle * swingDir;
                float end = startAngle * swingDir;
                bladeRot = MathHelper.Lerp(start, end, CalamityUtils.EaseInOutExp(lerp, 11f, 11f));

                if (lerp > 0.3f && lerp < 0.8f && Main.rand.NextBool((int)(1 + 4 * Projectile.MaxUpdates * Math.Abs(Utils.GetLerpValue(0.5f, 1.2f, lerp)))))
                {
                    Vector2 dustPosition = Projectile.Center + Projectile.velocity * 170 * Projectile.scale;
                    Vector2 dustPosition2 = Projectile.Center + Projectile.velocity.RotatedByRandom(0.3f) * 170 * Projectile.scale;
                    Vector2 swingVel = Projectile.velocity.RotatedBy(MathHelper.PiOver2 * (swingCount % 2 == 0 ? 1 : -1) * Owner.direction);

                    Dust dust = Dust.NewDustPerfect(dustPosition, ModContent.DustType<VoidDustPixelated>(),
                        swingVel.RotatedByRandom(0.1f) * Main.rand.NextFloat(9.5f, 15.5f), 0, default, Main.rand.NextFloat(1.55f, 2.2f));
                    dust.noGravity = true;
                    dust.color = Main.rand.NextBool() ? Color.DodgerBlue : Color.Blue;
                    dust.customData = 7;

                    if (Main.rand.NextBool(3, 4))
                    {   
                        Vector2 vel = swingVel.RotatedByRandom(0.1f) * Main.rand.NextFloat(9.5f, 15.5f);
                        Dust dust2 = Dust.NewDustPerfect(dustPosition2, ModContent.DustType<SquashDustPixelated>(),
                            vel, 0, default, 2 * Main.rand.NextFloat(1.0f, 1.3f));
                        dust2.noGravity = true;
                        dust2.color = Main.rand.NextBool() ? Color.Gold : Color.Goldenrod;
                        dust2.customData = new Vector2(0.2f, 1.0f);
                        dust2.fadeIn = 0.2f;
                    }

                }
                if (lerp > 0.4f) // Partway through make the hitbox and sound
                {
                    if (makeSound)
                    {
                        enabledHitbox = hitboxFramesMax;
                        SoundStyle swing1 = new("CalamityMod/Sounds/Item/HeavySwing");
                        SoundEngine.PlaySound(swing1 with { Volume = 0.8f, Pitch = Main.rand.NextFloat(0.2f, 0.32f) }, Projectile.Center);
                        makeSound = false;
                    }
                }
                #endregion
            }
            Timers();
        }
        public void Timers()
        {
            time++;
            // Only increment the attack animation when on cooldown or when allowed to swing
            if ((attackTimer < fireRate && performSwing) || attackTimer < -1)
                attackTimer++;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // Dont increment numHits on armored targets or kills
            if (((target.life <= 0 && target.realLife == -1) || target.Calamity().IsArmored()))
                armoredHits++;

            float distanceToTiles = -100;
            bool canBeLaunched = false;
            bool lowEnoughToExecute = target.life <= Projectile.damage * 5;
            bool jared = target.type == ModContent.NPCType<PrimordialWyrmHead>();
            Vector2 toMouse = Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld);
            Vector2 launchVel = lowEnoughToExecute ? toMouse : Utils.DirectionTo(Owner.Center, target.Center);

            if (lowEnoughToExecute || jared)
            {
                distanceToTiles = CalamityUtils.DistanceToTileCollisionHit(target.Center, launchVel, 200) ?? -100;
                canBeLaunched = jared || ((distanceToTiles == -100 ? false : distanceToTiles < 200 ? true : false) && target.realLife == -1);
            }
            if (Projectile.numHits == 0)
            {
                Owner.SetScreenshake(6.5f);
                SoundStyle fire = new("CalamityMod/Sounds/NPCHit/ThanatosHitOpen1");
                SoundEngine.PlaySound(fire with { Volume = 0.75f, Pitch = -0.1f }, Projectile.Center);
                SoundStyle fire2 = new("CalamityMod/Sounds/Item/FinalDawnSlash");
                SoundEngine.PlaySound(fire2 with { Volume = 0.65f, Pitch = Main.rand.NextFloat(-0.2f, -0.3f) }, Projectile.Center);
            }
            int heal = Projectile.numHits == 0 ? 14 : 0;
            if (Projectile.numHits < 10)
            {
                Owner.Calamity().grandDadHealPool += heal;
                // Give partial Grand Dad healing to all nearby players
                for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
                {
                    Player player = Main.player[playerIndex];
                    float targetDist = player.Center.DistanceSQ(Owner.Center);

                    if (targetDist < 500f && player.team == Owner.team && player != Owner)
                    {
                        player.Calamity().grandDadHealPool += (int)(heal / 2);
                    }
                }
            }

            // Regular knockback
            float launchPower = 60;
            target.MoveNPC(toMouse, launchPower * (canBeLaunched ? 0.5f : 0.25f), true, Owner);
            if (canBeLaunched) // launches/executes enemies
            {
                if (jared)
                {
                    CombatText.NewText(target.Hitbox, Color.Aqua, CalamityUtils.GetTextValue("Misc.HecBoop"));
                    SoundStyle boop = new("CalamityMod/Sounds/Item/SnootBooped");
                    SoundEngine.PlaySound(boop with { Pitch = Main.rand.NextFloat(-0.15f, 0.15f) }, Projectile.Center);
                }

                target.Calamity().pacified = true;

                // Apply tile collison damage and forced velocity to launch the suckers
                // This always kills (unless it's Jared)
                int damage = (int)(Projectile.damage);
                target.GetGlobalNPC<CalamityTileCollisionHarmNPC>().ApplyCollisionDamage(target, Owner, (jared && !lowEnoughToExecute) ? 1 : damage, launchVel * launchPower, 5f, true, true);
            }

            if (Projectile.numHits < 3)
            {
                for (int i = 0; i < 2; i++)
                {
                    Particle spark = new CustomSpark(target.Center, launchVel * 65, "CalamityMod/Particles/GlowSpark2", false, (int)(16 - Projectile.numHits * 3), 0.145f - Projectile.numHits * 0.015f, Color.Black, new Vector2(2f, 1f), false, shrinkSpeed: 1f, colorFadeSpeed: 10);
                    GeneralParticleHandler.SpawnParticle(spark, true);
                    Particle spark2 = new CustomSpark(target.Center, launchVel * 65, "CalamityMod/Particles/GlowSpark", false, (int)(16 - Projectile.numHits * 3), 0.105f - Projectile.numHits * 0.015f, Color.Lerp(Color.Blue, Color.DodgerBlue, 0.65f), new Vector2(2f, 1f), true, false, shrinkSpeed: 1f, colorFadeSpeed: 10);
                    GeneralParticleHandler.SpawnParticle(spark2, true, GeneralDrawLayer.AfterEverything);
                }

                for (int i = 0; i < 25 - Projectile.numHits * 3; i++)
                {
                    float hitMult = 1 - 0.15f * Projectile.numHits;
                    float variance = Main.rand.NextFloat(-0.6f, 0.6f);
                    Dust dust = Dust.NewDustPerfect(target.Center, ModContent.DustType<VoidDustPixelated>(),
                        launchVel.RotatedBy(variance / 2).RotatedByRandom(MathF.Abs(variance) / 3) * Main.rand.NextFloat(30.0f, 33.0f) * hitMult * MathF.Pow((1 - MathF.Abs(variance)), 2), 0, default, 3 * hitMult * Main.rand.NextFloat(2.3f, 3.2f) - MathF.Abs(variance));
                    dust.noGravity = true;
                    dust.color = Main.rand.NextBool() ? Color.DodgerBlue : Color.Blue;
                    dust.customData = 3;
                    dust.fadeIn = 2.1f;

                    if (i % 2 == 0)
                    {
                        Vector2 vel = launchVel.RotatedBy(variance / 2).RotatedByRandom(MathF.Abs(variance) / 3) * Main.rand.NextFloat(20.0f, 23.0f) * hitMult * MathF.Pow((1 - MathF.Abs(variance)), 2);
                        Dust dust2 = Dust.NewDustPerfect(target.Center + vel * 13, ModContent.DustType<SquashDustPixelated>(),
                        vel, 0, default, 2 * hitMult * Main.rand.NextFloat(2.3f, 3.2f) - MathF.Abs(variance));
                        dust2.noGravity = true;
                        dust2.color = Main.rand.NextBool() ? Color.Gold : Color.Goldenrod;
                        dust2.customData = new Vector2(0.2f, 1.6f);
                        dust2.fadeIn = 2.5f;
                    }
                }
            }

            if (canBeLaunched)
            {
                modifiers.SourceDamage *= 0;
                modifiers.FinalDamage.Flat = 0.1f;
            }
            else
            {
                float minMult = 0.5f;
                int hitsToMinMult = 15;
                float damageMult = Utils.Remap(Projectile.numHits - armoredHits, 0, hitsToMinMult, 1, minMult, true);
                modifiers.SourceDamage *= damageMult;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (time < 3)
                return false;

            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Texture2D swoosh = ModContent.Request<Texture2D>("CalamityMod/Particles/CircularSmearLarge").Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY);
            Vector2 vel = Projectile.rotation.ToRotationVector2();
            Color drawColor = Projectile.GetAlpha(lightColor);
            float drawRotation = Projectile.rotation + ((Owner.direction == -1) ? MathHelper.Pi - MathHelper.PiOver4 : MathHelper.PiOver4);
            Vector2 rotationPoint = (Owner.direction == -1) ? new Vector2(texture.Width, texture.Height) : new Vector2(0, texture.Height);
            SpriteEffects flipSprite = (Owner.direction == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 handLatchingAdjustments = (swingCount % 2 == 0 ? new Vector2(-6, 0) : new Vector2(-2, 0));
            Vector2 placeAdjust = (drawRotation + MathHelper.PiOver2).ToRotationVector2() * 2.5f + (handLatchingAdjustments.RotatedBy(drawRotation) * Owner.direction);

            float lerp = 1 - bladefx;
            int swingCountFlip = (swingCount % 2 == 0 ? 1 : 3) + Owner.direction == 0 ? 2 : 0;
            Vector2 vel2 = Vector2.Lerp(Projectile.velocity.RotatedBy(MathHelper.PiOver2), Owner.Center.DirectionTo(Owner.ClampedMouseWorld()).RotatedBy(-MathHelper.PiOver2), 0f);
            float extraRot = MathHelper.PiOver4 * MathHelper.Lerp(3, 1.9f, lerp) * (swingCount % 2 == 0 ? (Owner.direction == -1 ? 1 : -1) : (Owner.direction == -1 ? -1 : 1));
            float swooshDrawRotation = vel2.ToRotation() + extraRot;
            SpriteEffects swooshFlipSprite = (swingCount % 2 == 0 ? (Owner.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None) : (Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally));
            if (bladefx > 0.02f)
            {
                PixelationManager.AddPixelatedDrawer((_) =>
                {
                    int draws = 15;
                    for (int i = 0; i < draws; i++)
                    {
                        Color clr = Color.Black;
                        Vector2 outlineOffset = (MathHelper.TwoPi / draws * i).ToRotationVector2() * 3.5f;
                        Main.EntitySpriteDraw(swoosh, drawPosition + Projectile.velocity.RotatedBy(extraRot) * 12.5f + outlineOffset, null, clr * MathF.Pow(bladefx, 3) * 0.4f, swooshDrawRotation, swoosh.Size() * 0.5f, Projectile.scale * 0.6f, swooshFlipSprite);
                        Main.EntitySpriteDraw(texture, drawPosition + placeAdjust + outlineOffset * 2 * bladefx, null, clr * 0.2f * bladefx, drawRotation, rotationPoint, Projectile.scale, flipSprite);
                    }
                }, Enums.GeneralDrawLayer.AfterProjectiles, BlendState.AlphaBlend);
                if (!CalamityClientConfig.Instance.Photosensitivity)
                {
                    PixelationManager.AddPixelatedDrawer((_) =>
                    {
                        Color clrCenter = Color.DodgerBlue;
                        Main.EntitySpriteDraw(swoosh, drawPosition + Projectile.velocity.RotatedBy(extraRot) * 12.5f, null, clrCenter * MathF.Pow(bladefx, 0.6f), swooshDrawRotation, swoosh.Size() * 0.5f, Projectile.scale * 0.6f, swooshFlipSprite);
                        Main.EntitySpriteDraw(texture, drawPosition + placeAdjust, null, clrCenter * bladefx, drawRotation, rotationPoint, Projectile.scale, flipSprite);
                    }, Enums.GeneralDrawLayer.AfterEverything, BlendState.Additive);

                }
            }
            
            Main.EntitySpriteDraw(texture, drawPosition + placeAdjust, null, drawColor, drawRotation, rotationPoint, Projectile.scale, flipSprite); // The holdout

            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
    }
}

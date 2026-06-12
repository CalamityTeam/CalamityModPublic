using System;
using System.Collections.Generic;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Particles;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using static Terraria.Player;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class PulseGrenadeProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override string Texture => "CalamityMod/Items/Weapons/DraedonsArsenal/PulseGrenade";

        public ref float time => ref Projectile.ai[0];
        public bool catching => Projectile.ai[1] == 5;
        public bool caught = false;
        public Player Owner => Main.player[Projectile.owner];
        public int tileHits = 0; // Keeps track of how many times the projectile has hit a tile.
        public bool flung = false; // If the projectile has entered the thrown state.
        public float UseTimer => Owner.HeldItem.useTime;
        public float fxScale = 0; // A scaling value for the effects
        public NPC targeted;

        public bool hasStoppedHolding = false; // If the player has let go of m1, thereby throwing the projectile.

        public Color col = Effects.ArsenalEffects.ArsenalPulseColor;
        public bool pullPin = true;
        public bool onSpawn = true;
        public int beepTimer = 0;
        public int beepRate = 1;

        public NPC lastHitTarget;
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = false;

            Projectile.timeLeft = 180;
            Projectile.DamageType = RogueDamageClass.Instance;
        }
        public override bool ShouldUpdatePosition() => flung;
        public override bool? CanDamage() => (flung ? null : false);
        public override void AI()
        {
            if (Owner.dead && !flung)
            {
                Projectile.Kill();
                return;
            }
            if (Owner.HeldItem.type != ModContent.ItemType<PulseGrenade>())
            {
                Projectile.Kill();
                return;
            }
            if (onSpawn)
            {
                if (catching)
                    caught = true;
                if (Owner.Calamity().StealthStrikeAvailable())
                {
                    Projectile.Calamity().stealthStrike = true;
                    time = 0;
                }
                Owner.Calamity().ConsumeStealthByAttacking();
                onSpawn = false;
            }

            if (flung)
                FlungState();
            else
                HeldState();

            if (catching)
                time--;
            else
                time++;

            if (Projectile.Opacity < 1)
                Projectile.Opacity += 0.03f;

            Lighting.AddLight(Projectile.Center, col.ToVector3() * 0.3f);
        }
        public void HeldState()
        {
            if (Projectile.Calamity().stealthStrike)
            {
                if (beepTimer < 40)
                {
                    if (Projectile.Opacity < 1)
                        Projectile.Opacity += 0.2f;
                    beepTimer += beepRate;
                }
                else
                {
                    SoundStyle pulse = new("CalamityMod/Sounds/Item/PulseSound");
                    SoundEngine.PlaySound(pulse with { Volume = 0.3f, Pitch = 0.1f + beepRate * 0.04f }, Projectile.Center);
                    Projectile.Opacity = 0;
                    beepTimer = 0;
                    beepRate += 2;
                }
                    
            }

            Projectile.velocity = Owner.velocity;
            float completion = time / (UseTimer * 0.7f * (Projectile.Calamity().stealthStrike ? 2 : 1)); // The completion of the throw animation.
            if (completion >= 1 && !catching) // The moment of being thrown.
            {
                time = -1;
                Projectile.Center = Owner.Center;
                Projectile.extraUpdates = (Projectile.Calamity().stealthStrike ? 22 : 5);
                Projectile.rotation += Main.rand.NextFloat(-4, 4);
                Vector2 velocity = Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld) * 9 * Owner.Calamity().rogueVelocity;
                Projectile.velocity = velocity;

                if (Projectile.Calamity().stealthStrike)
                {
                    SoundStyle hardThrow = new("CalamityMod/Sounds/Item/SwooshMid");
                    SoundEngine.PlaySound(hardThrow with { Volume = 0.9f, Pitch = 0.5f }, Projectile.Center);

                    Projectile.timeLeft = 240;
                    Particle pulse3 = new CustomSpark(Projectile.Center, Projectile.velocity * 1.5f, "CalamityMod/Particles/HighResHollowCircleHardEdgeAlt", false, 13, 0.08f, Effects.ArsenalEffects.ArsenalPulseColor, new Vector2(1.2f, 0.7f), shrinkSpeed: 0.1f);
                    GeneralParticleHandler.SpawnParticle(pulse3);

                    for (int i = 0; i <= 12; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, Effects.ArsenalEffects.ArsenalPulseDust);
                        dust.scale = Main.rand.NextFloat(1.2f, 1.9f);
                        dust.velocity = (Projectile.velocity.SafeNormalize(Vector2.UnitX)).RotateRandom(0.5f) * Main.rand.NextFloat(7f, 13f);
                        dust.noGravity = true;
                        dust.color = Effects.ArsenalEffects.ArsenalPulseColor;
                        dust.fadeIn = 1;
                    }
                    for (int i = 0; i <= 7; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SquashDust>());
                        dust.scale = Main.rand.NextFloat(1.2f, 1.9f);
                        dust.velocity = (Projectile.velocity.SafeNormalize(Vector2.UnitX)).RotateRandom(0.3f) * Main.rand.NextFloat(9f, 18f);
                        dust.noGravity = true;
                        dust.color = Effects.ArsenalEffects.ArsenalPulseColor;
                        dust.fadeIn = 0.3f;
                    }
                }

                SoundStyle toss = new("CalamityMod/Sounds/Item/LightThrow");
                SoundEngine.PlaySound(toss with { Volume = 1f, Pitch = 0 }, Projectile.Center);

                Projectile.tileCollide = true;
                flung = true;
            }
            else
            {
                fxScale = (float)Math.Pow(completion, 2);

                Owner.direction = Math.Sign(Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld).X);
                float grenadeRot = 0;
                float pinTime = 0.75f;
                Vector2 aimDirection = Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld);
                float dir = (Owner.direction == -1 ? MathHelper.ToRadians(180) : 0);
                // All the annoying to make rotation and placement code for the throw animation.
                if (completion >= 0.7f)
                {
                    if (pullPin && completion >= pinTime && !catching)
                    {
                        Vector2 vel = aimDirection * -6 - Vector2.UnitY * 3 + Owner.velocity;
                        Particle thePin = new CustomSpark(Projectile.Center, vel, "CalamityMod/Projectiles/DraedonsArsenal/PulseGrenadePin", true, 43, 1f, Color.White, Vector2.One, false, false, 0, false, true, noShrink: true, spin: 0.1f * Math.Sign(vel.X));
                        GeneralParticleHandler.SpawnParticle(thePin);
                        SoundStyle pin = new("CalamityMod/Sounds/Item/LightMetal");
                        SoundEngine.PlaySound(pin with { Volume = 0.6f, Pitch = Main.rand.NextFloat(-0.1f, 0.1f) }, Projectile.Center);
                        pullPin = false;
                    }
                    float completionLerp = (float)Math.Pow(Utils.GetLerpValue(0.7f, 1f, completion, true), catching ? 2 : 7);
                    grenadeRot = MathHelper.ToRadians(MathHelper.Lerp(-75, 130f, completionLerp) * Owner.direction);
                }
                else
                {
                    if (catching && !Main.mouseLeft)
                    {
                        Projectile.Kill();
                        return;
                    }
                    Projectile.ai[1] = 0;
                    
                    float completionLerp = (float)Math.Pow(Utils.GetLerpValue(0f, 0.7f, completion, true), 2);
                    grenadeRot = MathHelper.ToRadians(MathHelper.Lerp(120, -75f, completionLerp) * Owner.direction);
                }
                grenadeRot += aimDirection.ToRotation();
                Vector2 grenadePos = Owner.GetFrontHandPosition(CompositeArmStretchAmount.None, Owner.compositeFrontArm.rotation) + new Vector2(0, -18 * Owner.direction).RotatedBy(grenadeRot);
                float completionLerp2 = (float)Math.Pow(Utils.GetLerpValue(0f, 0.7f, completion, true), 2);
                float grenadeHalfRot = MathHelper.ToRadians(MathHelper.Lerp(120, -75f, completionLerp2) * dir);

                Projectile.Center = grenadePos;
                Projectile.rotation = (-aimDirection).ToRotation() + MathHelper.PiOver2 * Owner.direction + dir;

                float frontArmRot = aimDirection.ToRotation() - MathHelper.ToRadians(90);
                float backArmRot = grenadeRot - (Owner.direction == 1 ? MathHelper.ToRadians(180) : MathHelper.ToRadians(0));
                if (completion >= 0.4f && completion <= pinTime && !catching)
                {
                    float goalRot = MathHelper.Lerp(frontArmRot, backArmRot, (float)Math.Pow(Utils.GetLerpValue(caught ? 0.6f : 0.4f, pinTime, completion, true), (Projectile.Calamity().stealthStrike ? 2 : 1) * (caught ? 5 : 3)));
                    Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, goalRot); 
                }
                else
                    Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (completion > pinTime && !catching) ? MathHelper.Lerp(Owner.compositeFrontArm.rotation, frontArmRot, 0.07f) : frontArmRot);
                Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, backArmRot);
            }
        }
        public void FlungState()
        {
            if ((time / Projectile.extraUpdates) > Owner.HeldItem.useAnimation * 0.45f)
                Projectile.localAI[1] = 5; // Make sure you can throw another grenade, if any genade has this value at zero you cant throw another one.


            if (Projectile.Calamity().stealthStrike)
            {
                Particle fadeInfx = new CustomSpark(Projectile.Center, Projectile.velocity * 0.01f, "CalamityMod/Particles/BloomCircle", false, 23, 0.4f, col * 0.6f, new Vector2(0.8f, 1f), true, true, shrinkSpeed: 0.3f, glowOpacity: 0.35f);
                GeneralParticleHandler.SpawnParticle(fadeInfx);
            }
            else
            {
                if (targeted != null && time >= 30)
                {
                    Projectile.timeLeft++;
                    float homeSpeed = Utils.GetLerpValue(30, 180, time, true);
                    CalamityUtils.HomeInOnSelectedNPC(Projectile, targeted, true, 0.8f * homeSpeed, 8, 1f - 0.15f * homeSpeed, 0.99f, true);
                    if (targeted.life <= 0)
                        targeted = null;
                }
                else
                {
                    targeted = (Projectile.Center).ClosestNPCAt(500, false, true);
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            bool squash = Main.rand.NextBool();
            Dust dust = Dust.NewDustPerfect(Projectile.Center, squash ? ModContent.DustType<SquashDust>() : ModContent.DustType<SquashDustHollow>(), -Projectile.velocity.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(0.1f, 1f), 0, default, Main.rand.NextFloat(0.6f, 1.05f));
            dust.noGravity = true;
            dust.color = col;
            dust.noLightEmittence = true;
            if (squash)
            {
                dust.fadeIn = 1f;
                dust.scale *= 2;
            }
            else
                dust.velocity = dust.velocity.RotatedByRandom(0.2f);
            if (Projectile.Calamity().stealthStrike)
                dust.velocity = dust.velocity.RotatedByRandom(0.7f) * 12.5f;
            if (Projectile.timeLeft == 1)
                Explode();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundStyle sound = CommonCalamitySounds.VoidstoneMine with { Volume = 1 };
            SoundEngine.PlaySound(sound with { Volume = 0.3f, Pitch = -0.25f - 0.1f * tileHits, MaxInstances = 6 }, Projectile.Center);

            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }

            tileHits++;
            if (tileHits > 2)
                Explode();
            return false;
        }
        public void Explode()
        {
            Owner.SetScreenshake(3.5f * (Projectile.Calamity().stealthStrike ? 2 : 1));

            int spin = Main.rand.NextBool() ? -1 : 1;
            for (int i = 0; i < 8; i++)
            {
                Vector2 vel = (MathHelper.TwoPi * i / 8f).ToRotationVector2();
                Dust c = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SquashDustHollow>());
                c.velocity = vel * 15f * (i % 2 == 0 ? 0.6f : 1f);
                c.scale = (i % 2 == 0 ? 2.2f : 1.9f);
                c.noGravity = true;
                c.color = col;
                c.noLightEmittence = true;
                c.fadeIn = 0.3f;

                
                // Do NOT change the damage of these here, do it on the on hit
                Projectile orb = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, (vel * 7f).RotatedBy(MathHelper.PiOver4 / 2), ModContent.ProjectileType<PulseGrenadeOrb>(), Projectile.damage, 0f, Owner.whoAmI, 0, spin, i);
                if (Projectile.Calamity().stealthStrike)
                {
                    Projectile orb2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, (vel * 10f), ModContent.ProjectileType<PulseGrenadeOrb>(), Projectile.damage, 0f, Owner.whoAmI, 0, -spin, i + 1);
                }
            }

            if (Projectile.Calamity().stealthStrike)
            {
                SoundStyle pulseHard = new("CalamityMod/Sounds/Item/PulseSoundHeavy");
                SoundEngine.PlaySound(pulseHard with { Volume = 0.8f, Pitch = -0.6f, MaxInstances = 2 }, Projectile.Center);
            }
            SoundStyle pulse = new("CalamityMod/Sounds/Item/PulseSoundHeavy");
            SoundEngine.PlaySound(pulse with { Volume = 0.9f, Pitch = 0, MaxInstances = 2 }, Projectile.Center);


            Projectile.Kill();
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float minMult = 0.2f;
            int hitsToMinMult = 5;
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult;

            Explode();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 15, targetHitbox);
        public override bool PreDraw(ref Color lightColor)
        {
            float fade = 1;
            float reformMult = Utils.GetLerpValue(1, 0, Projectile.Opacity) * 3.5f;
            Texture2D tex = pullPin ? ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/DraedonsArsenal/PulseGrenade").Value : ModContent.Request<Texture2D>("CalamityMod/Projectiles/DraedonsArsenal/PulseGrenadeNoPin").Value;
            Vector2 baseDrawPos = Projectile.Center - Main.screenPosition + 
                ((!flung || catching) ? new Vector2(0, Owner.gfxOffY) : Vector2.Zero);
            for (int i = 0; i < 15; i++)
            {
                Color auraColor = col with { A = 0 } * 0.25f * fade * fxScale;
                Vector2 drawOffset = (MathHelper.TwoPi * i / 15f).ToRotationVector2() * 2 * Math.Max(fxScale, reformMult);
                Main.EntitySpriteDraw(tex, baseDrawPos + drawOffset, null, auraColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, (!flung ? Owner.direction : Projectile.direction) != 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            }
            Main.EntitySpriteDraw(tex, baseDrawPos, null, lightColor * fade * Projectile.Opacity, Projectile.rotation, tex.Size() / 2f, Projectile.scale, (!flung ? Owner.direction : Projectile.direction) != 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            Main.EntitySpriteDraw(ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/DraedonsArsenal/PulseGrenadeGlow").Value, baseDrawPos, null, Color.White * fade * Projectile.Opacity, Projectile.rotation, tex.Size() * 0.5f, 1f, (!flung ? Owner.direction : Projectile.direction) != 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }
    }
}

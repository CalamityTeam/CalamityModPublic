using System;
using System.Collections.Generic;
using CalamityMod.Dusts;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Ravager;
using CalamityMod.Particles;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    [PierceResistException]
    public class DoomsdayDeviceProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/DoomsdayDevice";
        public ref float time => ref Projectile.ai[0];
        public Player Owner => Main.player[Projectile.owner];
        public float rotSpeed = 0.05f; // How fast the projectile rotates.
        public int tileHits = 0; // Keeps track of if the projectile has hit a tile.
        public bool flung = false; // If the projectile has entered the thrown state.
        public float charge = 1; // How charged the projectile is, ranges from 1 - 4.
        public bool hasReachedFullCharge = false; // If the projectile is fully charged.
        public bool hasStoppedHolding = false; // If the player has let go of m1, thereby throwing the projectile.
        public int stealthPenaltyTimer = 0; // Timer to keep track of how long the player has held charge, if held for long enough stealth is drained.
        public bool doneHitting = false; // If the projectile has finished its normal throwing arc and is now in a falling state.
        public bool giveStealth = true; // If the projectile gives stealth on hit.

        public int maxStealthHits = 5; // How many bounces the stealth strike has. This does not include the inital and final hit, so be sure to take those into account if you adjust this.

        public Color mainColor = Color.White;
        public Color c1 = Color.Turquoise;
        public Color c2 = Color.Orchid;

        public NPC lastHitTarget;
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = false;

            Projectile.timeLeft = 3000;
            Projectile.DamageType = RogueDamageClass.Instance;
        }
        public override bool ShouldUpdatePosition() => flung;
        public override bool? CanDamage() => (flung && tileHits == 0 ? null : false);
        public override void AI()
        {
            if (Owner.dead && !flung)
            {
                Projectile.Kill();
                return;
            }
            // The main color shifting
            float rate = (Main.GlobalTimeWrappedHourly * 6);
            List<Color> eColors = new List<Color>()
            {
                c1,
                c2
            };
            int colorIndex = (int)(rate / 2 % eColors.Count);
            Color currentColor = eColors[colorIndex];
            Color nextColor = eColors[(colorIndex + 1) % eColors.Count];
            mainColor = Color.Lerp(currentColor, nextColor, rate % 2f > 1f ? 1f : rate % 1f);

            if (tileHits == 0) // Trail effects for charged throws
            {
                if (hasReachedFullCharge && !doneHitting)
                {
                    float squash = Utils.GetLerpValue(1, 3, Projectile.velocity.Length(), true);
                    Particle fadeInfx = new CustomSpark(Projectile.Center, Projectile.velocity * 0.01f, "CalamityMod/Particles/BloomCircle", false, 23, 0.4f, mainColor * 0.4f * squash, new Vector2(1 - 0.15f * squash, 1f), true, false, shrinkSpeed: 0.2f * squash);
                    GeneralParticleHandler.SpawnParticle(fadeInfx);
                }
            }

            if (flung)
            {
                if ((time / Projectile.extraUpdates) > Owner.HeldItem.useAnimation * 0.45f)
                    Projectile.localAI[1] = 5; // Make sure you can throw another grenade, if any genade has this value at zero you cant throw another one.

                if (!doneHitting)
                {
                    if (!Collision.SolidCollision(Projectile.Center, 5, 5) && tileHits == 0) // Since the arc of the throw is done by adjusting position and not velocity, extra checks are needed for tile collison.
                        Projectile.Center += new Vector2(0, Utils.Remap(time, 0, 120, -3, 3, false) * Projectile.ai[1]);
                    else
                    {
                        TileHit();
                    }
                    rotSpeed += 0.0002f;
                }
                else // When the grenade is falling to the floor after hitting an enemy.
                {
                    if (rotSpeed != 0)
                        rotSpeed *= 0.99f;
                    Projectile.velocity.Y += 0.05f;
                    charge *= 0.98f;
                    Projectile.Opacity = charge;
                }

                if (tileHits == 0) // Dust trail produced after being thrown.
                {
                    Projectile.rotation += Projectile.direction * rotSpeed;

                    bool smokey = Main.rand.NextBool(3);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + new Vector2(0, -10).RotatedBy(Projectile.rotation), smokey && Main.rand.NextBool() ? ModContent.DustType<VoidDust>() : ModContent.DustType<LightDust>(), Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(2f, 3f), 0, default, Main.rand.NextFloat(0.4f, 0.75f));
                    dust.noGravity = smokey;
                    dust.color = smokey ? Color.White : mainColor;
                    dust.noLight = true;
                    dust.alpha = 180;
                    dust.scale *= (smokey ? 2.2f : 1);
                    dust.velocity += (smokey ? Vector2.UnitY * -2 : Vector2.Zero);
                    dust.noLightEmittence = true;
                }
                else
                    Projectile.timeLeft = (int)(Projectile.timeLeft * 0.98f); // Decay lifetime if it has hit a tile.
            }
            else
            {
                Projectile.velocity = Owner.velocity;
                float completion = time / (Owner.HeldItem.useAnimation * 0.7f); // The completion of the throw animation.
                if (completion >= 1) // The moment of being thrown.
                {
                    time = -1;
                    Projectile.Center = Owner.Center;
                    Projectile.extraUpdates = hasReachedFullCharge ? 16 : 5;
                    Projectile.rotation += Main.rand.NextFloat(-4, 4);
                    Vector2 velocity = Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld);
                    float speedMult = (float)Math.Pow(Utils.GetLerpValue(1, 4, charge, true), 2) + 0.05f;
                    float arcValue = hasReachedFullCharge ? 0 : (float)Math.Pow(Utils.Remap(speedMult, 0, 1f, 1f, 0.8f), 4);
                    Projectile.ai[1] = arcValue;
                    Projectile.velocity = velocity * 9 * speedMult * Owner.Calamity().rogueVelocity;

                    SoundStyle w = new("CalamityMod/Sounds/Item/SwooshMid");
                    SoundEngine.PlaySound(w with { Volume = 1f, Pitch = 0.2f, MaxInstances = 6 }, Projectile.Center);
                    if (hasReachedFullCharge)
                    {
                        if (Owner.Calamity().StealthStrikeAvailable())
                        {
                            Projectile.Calamity().stealthStrike = true;
                            Owner.Calamity().ConsumeStealthByAttacking();
                        }
                        rotSpeed *= 2;
                        SoundStyle w2 = new("CalamityMod/Sounds/Item/SwooshMid");
                        SoundEngine.PlaySound(w2 with { Volume = 1f, Pitch = -0.4f, MaxInstances = 6 }, Projectile.Center);
                    }
                    else
                        Owner.Calamity().ConsumeStealthByAttacking();

                    

                    Projectile.tileCollide = true;
                    flung = true;
                }
                else
                {
                    if (Main.mouseLeft && !hasStoppedHolding) // The charging up.
                    {
                        if (completion >= 0.7f && completion <= 0.8f)
                        {
                            time--;
                            if (charge < 4)
                                charge += 0.1f;
                            else
                            {
                                // Player has 18 frames to let go after it has fully charged, otherwise it will start to drain stealth. Timing is key!
                                if (stealthPenaltyTimer == 18)
                                {
                                    SoundStyle w = new("CalamityMod/Sounds/Item/MeldSlice");
                                    SoundEngine.PlaySound(w with { Volume = 0.3f, Pitch = 0.4f }, Projectile.Center);
                                }
                                if (stealthPenaltyTimer >= 18)
                                {
                                    Owner.Calamity().rogueStealth -= Owner.Calamity().rogueStealthMax * 0.035f;
                                    CheckStealth();
                                }
                                else
                                    charge = 4;
                                stealthPenaltyTimer++;
                            }
                            if (charge >= 4 && !hasReachedFullCharge)
                            {
                                SoundStyle w = new("CalamityMod/Sounds/Item/MeldSlice");
                                SoundEngine.PlaySound(w with { Volume = 0.55f, Pitch = 1f }, Projectile.Center);

                                for (int i = 0; i < 12; i++)
                                {
                                    Dust c = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>());
                                    c.velocity = (MathHelper.TwoPi * i / 12f).ToRotationVector2().RotatedBy(Projectile.rotation) * 5.5f * (i % 2 == 0 ? 0.8f : 1f);
                                    c.scale = 0.7f * (i % 2 == 0 ? 2.2f : 1.8f);
                                    c.noGravity = true;
                                    c.color = i % 2 == 0 ? c1 : c2;
                                    c.noLightEmittence = true;
                                }

                                hasReachedFullCharge = true;
                            }
                        }
                    }
                    else
                        hasStoppedHolding = true;

                    Projectile.Opacity = (charge - 1) / 4; // This opacity if for the glowing effects of the projectile, not the projectile itself.
                    Owner.direction = Math.Sign(Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld).X);
                    float grenadeRot = 0;
                    // All the annoying to make rotation and placement code for the throw animation.
                    if (completion >= 0.7f)
                    {
                        float completionLerp = (float)Math.Pow(Utils.GetLerpValue(0.7f, 1f, completion, true), 7);
                        grenadeRot = MathHelper.ToRadians(MathHelper.Lerp(-75, 130f, completionLerp) * Owner.direction);
                    }
                    else
                    {
                        float completionLerp = (float)Math.Pow(Utils.GetLerpValue(0f, 0.7f, completion, true), 2);
                        grenadeRot = MathHelper.ToRadians(MathHelper.Lerp(120, -75f, completionLerp) * Owner.direction);
                    }
                    grenadeRot += Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld).ToRotation();
                    Vector2 grenadePos = Owner.MountedCenter + new Vector2(0, -24 * Owner.direction).RotatedBy(grenadeRot);
                    float completionLerp2 = (float)Math.Pow(Utils.GetLerpValue(0f, 0.7f, completion, true), 2);
                    float grenadeHalfRot = MathHelper.ToRadians(MathHelper.Lerp(120, -75f, completionLerp2) * Owner.direction);

                    Projectile.Center = grenadePos;
                    Projectile.rotation = grenadeRot - MathHelper.ToRadians(25 * grenadeHalfRot);

                    Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld).ToRotation() - MathHelper.ToRadians(90));
                    Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, grenadeRot - (Owner.direction == 1 ? MathHelper.ToRadians(180) : MathHelper.ToRadians(0)));
                }
            }
            time++;
            Lighting.AddLight(Projectile.Center, mainColor.ToVector3() * (0.3f + charge * 0.1f));
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            TileHit();
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float finalHitMult = 1; // If it is the last hit of a stealth strike, this is increased.

            if (Projectile.Calamity().stealthStrike && !doneHitting) // Stealth strike bounce code
            {
                NPC chosenTarget = null;
                float distance = 2500;
                for (int index = 0; index < Main.npc.Length; index++) // look for a target that isnt one it has already hit in the last two hits.
                {
                    NPC searchedTarget = Main.npc[index];
                    if (searchedTarget.CanBeChasedBy(null, false))
                    {
                        float extraDistance = (searchedTarget.width / 2) + (searchedTarget.height / 2);

                        bool canHit = true;
                        if (extraDistance < distance)
                            canHit = Collision.CanHit(Projectile.Center, 1, 1, Main.npc[index].Center, 1, 1);

                        if (Vector2.Distance(Projectile.Center, searchedTarget.Center) < distance && (lastHitTarget != null ? searchedTarget != lastHitTarget : true) && searchedTarget != target && searchedTarget.active && searchedTarget.life > 0 && canHit)
                        {
                            distance = Vector2.Distance(Projectile.Center, searchedTarget.Center);
                            chosenTarget = searchedTarget;
                        }
                    }
                }
                if (chosenTarget == null) // If no target was found, check again but accept last hit targets as viable.
                {
                    if (lastHitTarget != null)
                        Projectile.localNPCImmunity[lastHitTarget.whoAmI] = 0;

                    for (int index = 0; index < Main.npc.Length; index++)
                    {
                        NPC searchedTarget = Main.npc[index];
                        if (searchedTarget.CanBeChasedBy(null, false))
                        {
                            float extraDistance = (searchedTarget.width / 2) + (searchedTarget.height / 2);

                            bool canHit = true;
                            if (extraDistance < distance)
                                canHit = Collision.CanHit(Projectile.Center, 1, 1, Main.npc[index].Center, 1, 1);

                            if (Vector2.Distance(Projectile.Center, searchedTarget.Center) < distance && searchedTarget != target && searchedTarget.active && searchedTarget.life > 0 && canHit)
                            {
                                distance = Vector2.Distance(Projectile.Center, searchedTarget.Center);
                                chosenTarget = searchedTarget;
                            }
                        }
                    }
                }

                if (chosenTarget != null && Projectile.numHits <= maxStealthHits) // If a valid target is found and it can still bounce, redirect the projectile.
                {
                    if (lastHitTarget != null)
                        Projectile.localNPCImmunity[lastHitTarget.whoAmI] = 0;
                    lastHitTarget = target;
                    Projectile.velocity = Utils.DirectionTo(Projectile.Center, chosenTarget.Center) * 9;

                    SoundStyle sound = new("CalamityMod/Sounds/Item/DoomsdayDeviceImpact");
                    SoundEngine.PlaySound(sound with { Volume = 0.9f, Pitch = 0.1f + Projectile.numHits * 0.15f, MaxInstances = 6 }, Projectile.Center);

                    for (int i = 0; i <= 6; i++)
                    {
                        float variance = Main.rand.NextFloat(-0.6f, 0.6f);
                        int dustStyle = ModContent.DustType<LightDust>();
                        Dust dust2 = Dust.NewDustPerfect(Projectile.Center, dustStyle, Projectile.velocity);
                        dust2.scale = Main.rand.NextFloat(1.5f, 1.7f) - Math.Abs(variance);
                        dust2.velocity = (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 5 * charge).RotatedBy(variance) * Main.rand.NextFloat(0.3f, 1f) * (1 - Math.Abs(variance)) * finalHitMult;
                        dust2.noGravity = true;
                        dust2.color = Main.rand.NextBool() ? c1 : c2;
                    }

                    Particle pulse = new CustomSpark(Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.UnitX) * 3, "CalamityMod/Particles/HollowCircleSoftEdge", false, 14, 0.2f, c1 * 0.85f, new Vector2(3f, 1f), extraRotation: MathHelper.ToRadians(90), shrinkSpeed: 0.9f);
                    GeneralParticleHandler.SpawnParticle(pulse);
                    Particle pulse2 = new CustomSpark(Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.UnitX) * 2.5f, "CalamityMod/Particles/HollowCircleSoftEdge", false, 14, 0.175f * finalHitMult, c2 * 0.7f, new Vector2(2.5f, 2f), extraRotation: MathHelper.ToRadians(90), shrinkSpeed: 0.9f);
                    GeneralParticleHandler.SpawnParticle(pulse2);

                    Owner.SetScreenshake(3f);
                }
                else // If there's no targets other than the currently hit one or the projectile is out of bounces, deal the final stronger blow.
                {
                    finalHitMult = 2;
                    SoundStyle sound = new("CalamityMod/Sounds/Item/HeliumFlashCoreImpact");
                    SoundEngine.PlaySound(sound with { Volume = 0.7f, Pitch = 0.2f }, Projectile.Center);
                    Projectile.numHits = maxStealthHits;
                }
                    
            }

            // Do the damage calculation now. It's not at the bottom of this section because of some variables that are changed below.
            float minMult = 0.2f;
            int hitsToMinMult = maxStealthHits + 2;
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true) * (doneHitting ? 0.3f : 1);
            float finalDamageMult = (charge / 4) * (hasReachedFullCharge ? 1.5f : 1) * damageMult * Utils.Remap(finalHitMult, 1, 2, 1, 5);
            modifiers.SourceDamage *= finalDamageMult;

            if ((!doneHitting && !Projectile.Calamity().stealthStrike) || (!doneHitting && finalHitMult > 1)) // Regular hits and the final hit from stealth strikes.
            {
                Vector2 launchDir = Utils.DirectionTo(Projectile.Center, target.Center);
                float launchPower = ((hasReachedFullCharge ? 9 : 0) + charge * 1.5f) * finalHitMult;

                target.MoveNPC(launchDir, launchPower, true, Owner);

                float extraPitch = (Owner.Calamity().rogueStealthMax > 0 ? (0.25f * (Owner.Calamity().rogueStealth / Owner.Calamity().rogueStealthMax)) : 0);
                
                if (!Projectile.Calamity().LocketClone && finalHitMult == 1 && giveStealth) // Locket clones and stealth strikes don't create stealth.
                {
                    Owner.Calamity().rogueStealth += Owner.Calamity().rogueStealthMax * 0.3f;
                    CheckStealth();
                    giveStealth = false;
                }

                Owner.SetScreenshake(charge * (hasReachedFullCharge ? 1.2f : 0.8f) * finalHitMult);

                for (int i = 0; i <= 12 * finalHitMult; i++)
                {
                    float variance = Main.rand.NextFloat(-0.6f, 0.6f);
                    int dustStyle = ModContent.DustType<LightDust>();
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center, dustStyle, launchDir * 5);
                    dust2.scale = Main.rand.NextFloat(1.5f, 1.7f) - Math.Abs(variance);
                    dust2.velocity = (launchDir * 5 * charge).RotatedBy(variance) * Main.rand.NextFloat(0.3f, 1f) * (1 - Math.Abs(variance)) * finalHitMult;
                    dust2.noGravity = true;
                    dust2.color = Main.rand.NextBool() ? c1 : c2;
                }

                if (hasReachedFullCharge)
                {
                    SoundStyle soundExtra = new("CalamityMod/Sounds/Item/DoomsdayDeviceImpact");
                    SoundEngine.PlaySound(soundExtra with { Volume = 1f, Pitch = 0.35f + extraPitch, MaxInstances = 6 }, Projectile.Center);

                    Particle pulse = new CustomSpark(Projectile.Center, -launchDir * 3, "CalamityMod/Particles/HollowCircleSoftEdge", false, 14, 0.4f * finalHitMult, c1 * 0.85f, new Vector2(3f, 1f), extraRotation: MathHelper.ToRadians(90), shrinkSpeed: 0.9f);
                    GeneralParticleHandler.SpawnParticle(pulse);
                    Particle pulse2 = new CustomSpark(Projectile.Center, -launchDir * 2.5f, "CalamityMod/Particles/HollowCircleSoftEdge", false, 14, 0.35f * finalHitMult, c2 * 0.7f, new Vector2(2.5f, 2f), extraRotation: MathHelper.ToRadians(90), shrinkSpeed: 0.9f);
                    GeneralParticleHandler.SpawnParticle(pulse2);
                }
                SoundStyle sound = new("CalamityMod/Sounds/Item/DoomsdayDeviceImpact");
                SoundEngine.PlaySound(sound with { Volume = hasReachedFullCharge ? 1 : 0.7f, Pitch = 0.1f + extraPitch, MaxInstances = 6 }, Projectile.Center);

                doneHitting = true;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            bool onKill = ((target.life <= 0 && target.realLife == -1) || target.lifeMax == 1);

            if (onKill && Projectile.numHits < maxStealthHits && hasReachedFullCharge) // If an enemy is killed by the grenade impact, don't increase the hit counter and allow it to pierce them.
            {
                Projectile.numHits--;
                doneHitting = false;
            }
            else if (doneHitting && Projectile.extraUpdates != 4) // The fall to ground after hit effect.
            {
                Projectile.velocity = Vector2.Lerp(Utils.DirectionFrom(Projectile.Center, target.Center) * 4, Vector2.UnitY * -2, 0.75f).RotatedByRandom(0.1f);
                Projectile.extraUpdates = 4;
            }
        }
        public void TileHit()
        {
            Projectile.velocity = Vector2.Zero;
            rotSpeed = 0;
            Projectile.extraUpdates = 3;
            hasReachedFullCharge = false;
            if (tileHits == 0)
            {
                SoundStyle sound = CommonCalamitySounds.VoidstoneMine with { Volume = 1 };
                SoundEngine.PlaySound(sound with { Volume = 0.3f, Pitch = -0.55f, MaxInstances = 6 }, Projectile.Center);
                SoundStyle sound2 = RockPillar.HitSound;
                SoundEngine.PlaySound(sound2 with { Volume = 0.4f, Pitch = -0.2f, MaxInstances = 6 }, Projectile.Center);
            }
            tileHits++;

        }
        public void CheckStealth()
        {
            if (Owner.Calamity().rogueStealth > Owner.Calamity().rogueStealthMax)
                Owner.Calamity().rogueStealth = Owner.Calamity().rogueStealthMax;
            if (Owner.Calamity().rogueStealth < 0)
                Owner.Calamity().rogueStealth = 0;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 30, targetHitbox);
        public override bool PreDraw(ref Color lightColor)
        {
            Color glowColor = Color.Lerp(mainColor, Color.Red, Utils.GetLerpValue(60, 18, (stealthPenaltyTimer >= 18 ? stealthPenaltyTimer : 60)));
            float fade = Utils.GetLerpValue(0, 300, Projectile.timeLeft, true);
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Vector2 baseDrawPos = Projectile.Center - Main.screenPosition + (!flung ? new Vector2(0, Owner.gfxOffY) : Vector2.Zero);
            for (int i = 0; i < 25; i++)
            {
                Color auraColor = glowColor with { A = 0 } * 0.35f * fade;
                Vector2 drawOffset = (MathHelper.TwoPi * i / 25f).ToRotationVector2() * (hasReachedFullCharge ? 7 : 3f) * Projectile.Opacity;
                Main.EntitySpriteDraw(tex, baseDrawPos + drawOffset + Main.rand.NextVector2Circular(4, 4), null, auraColor * Projectile.Opacity, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, (!flung ? Owner.direction : Projectile.direction) != 1 ? SpriteEffects.FlipVertically : SpriteEffects.None);
            }
            Main.EntitySpriteDraw(tex, baseDrawPos, null, lightColor * fade, Projectile.rotation, tex.Size() / 2f, Projectile.scale, (!flung ? Owner.direction : Projectile.direction) != 1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0);

            Main.EntitySpriteDraw(ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Rogue/DoomsdayDeviceGlow2").Value, baseDrawPos, null, glowColor * Projectile.Opacity * fade, Projectile.rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Rogue/DoomsdayDeviceGlow2").Value.Size() * 0.5f, 1f, (!flung ? Owner.direction : Projectile.direction) != 1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0);
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
    }
}

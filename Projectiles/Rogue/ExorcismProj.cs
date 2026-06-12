using System;
using System.Collections.Generic;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ExorcismProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Exorcism";

        public ref float time => ref Projectile.ai[0];
        public Player Owner => Main.player[Projectile.owner];

        public bool flung = false; // If the projectile has entered the thrown state.
        public bool hitTiles => Projectile.ai[1] == -5; // In post hit tiles state
        public bool inSky => Projectile.ai[1] == 5; // After being thrown, waiting to come down state
        public bool falling => Projectile.ai[1] == 10; // Falling from the sky state
        public int fallTime = 55; // Time spend in the sky
        public NPC targeted; // Target impaled into
        public Vector2 impaleDist; // Distance from impaled target's center
        public int hitRate = 60; // How fast the cross burns with holy light
        public float randpitch = 0; // Some random pitch for the looping sound
        public bool stealth => Projectile.Calamity().stealthStrike;
        public Vector2 storedStealthVel; // stored velocity for the boomerang turn around
        public Vector2 crossCenter => Projectile.Center + (Vector2.UnitY * -11).RotatedBy(Projectile.rotation); // The center of the cross

        public Color mainColor = Color.Gold;
        public SlotId AudSlot;
        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = false;

            Projectile.timeLeft = 600;
            Projectile.DamageType = RogueDamageClass.Instance;
        }
        public override bool ShouldUpdatePosition() => flung && !inSky && !hitTiles && (impaleDist == Vector2.Zero || stealth);
        public override bool? CanDamage() => ((flung && !inSky && !hitTiles && impaleDist == Vector2.Zero) ? null : false);
        public override void AI()
        {
            if (Owner.dead && !flung)
            {
                Projectile.Kill();
                return;
            }
            if (stealth && Projectile.ai[2] != 5) // Multiplayer Syncing?
            {
                Projectile.ai[2] = 5;
                Projectile.ForceNetUpdate();
            }
            if (Projectile.ai[2] == 5)
                Projectile.Calamity().stealthStrike = true;

            if (!flung && time == 0)
                randpitch = Main.rand.NextFloat(-0.1f, 0.1f);
            if (flung)
            {
                if (hitTiles)
                {
                    holySound();
                    if (Projectile.timeLeft > 300)
                        Projectile.timeLeft = 300;
                }
                else if (inSky)
                {
                    Projectile.Center = new Vector2(Owner.ClampedMouseWorld().X, Owner.Center.Y) + new Vector2(0, -700);
                    Projectile.Opacity = 0;
                    Projectile.extraUpdates = 0;
                    Projectile.timeLeft++;
                    fallTime--;
                    if (fallTime <= 0)
                    {
                        for (int i = 0; i < Main.maxNPCs; i++)
                            Projectile.localNPCImmunity[i] = 0;
                        Projectile.Opacity = 1;
                        Projectile.velocity = Utils.DirectionTo(Projectile.Center, Owner.Calamity().mouseWorld) * 7;
                        Projectile.numHits = 0;
                        Projectile.extraUpdates = 25;
                        Projectile.ai[1] = 10;
                    }
                    if (fallTime == 10)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            SoundStyle sound = new("CalamityMod/Sounds/Item/MissileNearing");
                            SoundEngine.PlaySound(sound with { Volume = 0.5f, Pitch = 0.8f, MaxInstances = 2 }, Projectile.Center);
                        }
                    }
                }
                else if (falling)
                {
                    if (Projectile.numHits == 0)
                    {
                        Projectile.timeLeft++;
                        if (Projectile.Center.Y + 80 > Owner.Calamity().mouseWorld.Y && Collision.SolidCollision(Projectile.Center, 20, 20))
                        {
                            Projectile.netUpdate = true;
                            Projectile.extraUpdates = 3;
                            Projectile.ai[1] = -5;
                            SoundStyle sound = new("CalamityMod/Sounds/NPCHit/ExoHit2");
                            SoundEngine.PlaySound(sound with { Volume = 0.6f, Pitch = Main.rand.NextFloat(0f, 0.1f) }, Projectile.Center);
                            SoundStyle sound2 = new("CalamityMod/Sounds/NPCHit/RavagerRockPillarHit", 3);
                            SoundEngine.PlaySound(sound2 with { Volume = 0.7f, Pitch = -0.3f }, Projectile.Center);
                        }

                        Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

                        float squash = Utils.GetLerpValue(1, 3, Projectile.velocity.Length(), true);
                        Particle trail = new CustomSpark(Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 35, Projectile.velocity * 0.01f, "CalamityMod/Particles/BloomCircle", false, 43, 0.4f, mainColor * 0.4f * squash, new Vector2(1 - 0.15f * squash, 1f), true, false, shrinkSpeed: 0.07f * squash);
                        GeneralParticleHandler.SpawnParticle(trail);
                    }
                    else
                    {
                        Projectile.extraUpdates = 3;
                        Projectile.Center = targeted.Center + impaleDist;
                        if (stealth)
                            Projectile.rotation += (0.08f - hitRate * 0.001f) * Projectile.direction * 0.3f;

                        holySound();

                        if (time >= hitRate)
                        {
                            impaleDist.Y *= 0.93f;
                            Projectile strike = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), targeted.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), (int)(Projectile.damage * Utils.Remap(hitRate, 50, 5, 0.05f, 0.1f)), 0f, Owner.whoAmI, targeted.whoAmI, 1f);
                            strike.ArmorPenetration = 30;
                            strike.DamageType = RogueDamageClass.Instance;
                            if (hitRate > 10)
                                hitRate -= 10;
                            time = 0;
                        }
                    }
                }
                else // State before it enters the sky
                {
                    if (stealth)
                    {
                        if (Projectile.timeLeft > 240)
                        {
                            // Boomerang noises
                            if (Projectile.soundDelay == 0)
                            {
                                Projectile.soundDelay = 5 * Projectile.MaxUpdates;
                                SoundStyle sound = new("CalamityMod/Sounds/Item/SwooshMid");
                                SoundEngine.PlaySound(sound with { MaxInstances = -1, Volume = 0.5f, Pitch = -0.2f }, Projectile.Center);
                            }
                            Projectile.rotation += 0.07f * Projectile.direction;
                        }

                        if (time > 80)
                        {
                            if (Projectile.velocity.Length() < storedStealthVel.Length())
                                Projectile.velocity += Utils.DirectionTo(Projectile.Center, Owner.Center) * 0.1f;
                            Projectile.velocity *= 0.99f;
                            if (time == 180) // Allow it to hit on the return trip
                            {
                                for (int i = 0; i < Main.maxNPCs; i++)
                                    Projectile.localNPCImmunity[i] = 0;
                            }
                            if (Projectile.timeLeft <= 240)
                            {
                                impaleDist = Vector2.One; // This is so the projectile stops dealing damage
                                Projectile.extraUpdates = 3;
                                float endLerp = Utils.GetLerpValue(240, 90, Projectile.timeLeft, true);
                                Vector2 endPos = Vector2.Lerp((Owner.Center + Vector2.UnitY * (-50 - 90 * endLerp)), Owner.ClampedMouseWorld(), (float)Math.Pow(endLerp, 5));
                                Projectile.velocity = (endPos - Projectile.Center) / 25;
                                Projectile.rotation = Utils.AngleLerp(Projectile.rotation, 0, endLerp);
                            }
                        }
                        else
                            storedStealthVel = -Projectile.velocity * 1.1f;

                        if (Projectile.timeLeft > 240)
                        {
                            int numParts = 2;
                            for (int i = 0; i < numParts; i++)
                            {
                                float fade = (Utils.GetLerpValue(5, 2, Projectile.velocity.Length(), true) * 3 + 1);

                                float rot = Projectile.rotation + (MathHelper.TwoPi * i / numParts);
                                Vector2 vel = (Utils.MoveTowards(-Projectile.velocity, new Vector2(0, -130).RotatedBy(rot).RotatedBy(-1.3f * Projectile.direction), (Utils.GetLerpValue(5, 2, Projectile.velocity.Length(), true))));

                                if (time % 5 == 0)
                                {
                                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center + new Vector2(0, -70).RotatedBy(rot), Main.rand.NextBool(4) ? 278 : ModContent.DustType<LightDust>());
                                    dust2.noGravity = (dust2.type == 278 ? false : true);
                                    dust2.scale = (dust2.type == 278 ? 0.95f : 1.2f) * 0.6f;
                                    dust2.color = Color.Red;
                                    dust2.velocity = (vel * 2).RotatedByRandom(0.4f) * fade;
                                }
                                if (time % 2 == 0)
                                {
                                    Dust dust = Dust.NewDustPerfect(Projectile.Center + new Vector2(0, -70).RotatedBy(rot), Main.rand.NextBool(4) ? 278 : ModContent.DustType<LightDust>());
                                    dust.noGravity = (dust.type == 278 ? false : true);
                                    dust.scale = (dust.type == 278 ? 0.75f : 0.9f) * 0.6f;
                                    dust.color = Main.rand.NextBool(4) ? Color.Khaki : Color.Goldenrod;
                                    dust.velocity = (vel * 2).RotatedByRandom(0.4f) * fade;
                                }
                            }
                        }

                        holySound();
                    }
                    else
                    {
                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                        if (time >= 120)
                            Projectile.ai[1] = 5;
                        if (time % 6 == 0 && time > 12)
                        {
                            Particle trail = new CustomSpark(Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 35 + Main.rand.NextVector2Circular(10, 10), -Projectile.velocity * 0.01f, "CalamityMod/Particles/BloomCircle", false, 13, 0.2f, mainColor * 0.6f, new Vector2(0.6f, 1f), true, false, shrinkSpeed: 0.6f);
                            GeneralParticleHandler.SpawnParticle(trail);
                        }
                    }
                }
            }
            else
            {
                Vector2 projToSky = Utils.DirectionTo(Projectile.Center, new Vector2(MathHelper.Lerp(Owner.ClampedMouseWorld().X, Owner.Center.X, 0.35f), Owner.Center.Y) + new Vector2(0, -500));
                Projectile.velocity = projToSky;
                float completion = time / (Owner.HeldItem.useAnimation * 0.7f); // The completion of the throw animation.
                if (completion >= 1) // The moment of being thrown.
                {
                    Projectile.Center = Owner.Center;
                    Projectile.extraUpdates = stealth ? 9 : 6;
                    if (stealth)
                        Projectile.timeLeft = 560;
                    Vector2 velocity = Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld);
                    float speed = 9;
                    Projectile.velocity = (stealth ? velocity : projToSky) * speed;
                    time = -1;
                    SoundStyle w = new("CalamityMod/Sounds/Item/SwooshMid");
                    for (int i = 0; i < 2; i++)
                        SoundEngine.PlaySound(w with { Volume = 1f, Pitch = -0.4f + 0.2f * i, MaxInstances = 6 }, Projectile.Center);

                    flung = true;
                }
                else
                {
                    Owner.direction = Math.Sign(Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld).X);
                    float crossRot = 0;
                    // All the annoying to make rotation and placement code for the throw animation.
                    if (completion >= 0.7f)
                    {
                        float completionLerp = (float)Math.Pow(Utils.GetLerpValue(0.7f, 1f, completion, true), 7);
                        crossRot = MathHelper.ToRadians(MathHelper.Lerp(-45, 130f, completionLerp) * Owner.direction);
                    }
                    else
                    {
                        float completionLerp = (float)Math.Pow(Utils.GetLerpValue(0f, 0.7f, completion, true), 2);
                        crossRot = MathHelper.ToRadians(MathHelper.Lerp(120, -45f, completionLerp) * Owner.direction);
                    }
                    crossRot += Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld).ToRotation();
                    Vector2 crossPos = Owner.MountedCenter + new Vector2(0, -24 * Owner.direction).RotatedBy(crossRot);
                    float completionLerp2 = (float)Math.Pow(Utils.GetLerpValue(0f, 0.7f, completion, true), 2);

                    Projectile.Center = crossPos;
                    if (stealth)
                        Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.Pow(completion, 3) * 12 * Owner.direction;
                    else
                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

                    Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld).ToRotation() - MathHelper.ToRadians(90));
                    Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, crossRot - (Owner.direction == 1 ? MathHelper.ToRadians(180) : MathHelper.ToRadians(0)));
                }
            }
            time++;
            if (!inSky)
                Lighting.AddLight(Projectile.Center, Color.Gold.ToVector3() * 0.5f);
            if (targeted != null && ((targeted.life <= 0 && targeted.realLife == -1) || targeted.lifeMax == 1))
                    Projectile.Kill();
        }
        public void holySound()
        {
            if (SoundEngine.TryGetActiveSound(AudSlot, out var holy) && holy.IsPlaying)
            {
                holy.Position = Projectile.Center;
                holy.Pitch = Utils.Remap(Projectile.timeLeft, 300, 0, -0.4f + randpitch, (stealth ? -0.15f : -0.3f) + randpitch);
                holy.Volume = Utils.Remap(Projectile.timeLeft, 300, 0, 0f, 0.5f) * 100;
            }
            else if (Projectile.timeLeft > 1)
            {
                SoundStyle choir = new("CalamityMod/Sounds/Item/HolyLoop");
                AudSlot = SoundEngine.PlaySound(choir with { Volume = 0.01f, Pitch = 0 }, Projectile.Center);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (stealth)
            {
                float minMult = 0.35f;
                int hitsToMinMult = 10;
                float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
                modifiers.SourceDamage *= damageMult / 2;
            }
            else if (!falling || Projectile.numHits != 0)
                modifiers.SourceDamage *= 0.1f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            bool onKill = ((target.life <= 0 && target.realLife == -1) || target.lifeMax == 1);
            if (!onKill && falling)
            {
                if (!stealth)
                {
                    time = 0;
                    SoundStyle sound = new("CalamityMod/Sounds/NPCHit/ExoHit4");
                    SoundEngine.PlaySound(sound with { Volume = 0.7f, Pitch = Main.rand.NextFloat(0.15f, 0.25f) }, Projectile.Center);
                    SoundStyle sound2 = new("CalamityMod/Sounds/NPCHit/ExoHit3");
                    SoundEngine.PlaySound(sound2 with { Volume = 0.5f, Pitch = Main.rand.NextFloat(-0.1f, 0f) }, Projectile.Center);
                    Owner.SetScreenshake(5.5f);
                    Projectile.timeLeft = 300;
                    targeted = target;
                    impaleDist = (Projectile.Center - targeted.Center);
                }
            }
            else
                Projectile.numHits--;
        }
        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(AudSlot, out var holy))
            {
                holy?.Stop();
            }
            if (stealth)
            {
                if (Main.zenithWorld)
                {
                    // All is cleansed by The Lord
                    for (int index = 0; index < Main.npc.Length; index++)
                    { NPC searchedTarget = Main.npc[index]; searchedTarget.active = false; }
                    for (int x = 0; x < Main.maxProjectiles; x++)
                    { Projectile projectile = Main.projectile[x]; if (projectile.active && projectile.type != ModContent.ProjectileType<ExorcismShockwave>() && projectile.type != ModContent.ProjectileType<ExorcismProj>()) { projectile.active = false; } }
                    for (int x = 0; x < Main.maxItems; x++)
                    { Item item = Main.item[x]; if (item.active) { item.active = false; } }
                    for (int x = 0; x < Main.maxDust; x++)
                    { Dust dust = Main.dust[x]; if (dust.active) { dust.active = false; } }
                    for (int x = 0; x < Main.maxGore; x++)
                    { Gore gore = Main.gore[x]; if (gore.active) { gore.active = false; } }

                    SoundStyle gong = new("CalamityMod/Sounds/Custom/GFB/Jesus");
                    for (int i = 0; i < 2; i++)
                        SoundEngine.PlaySound(gong with { Volume = 1f, MaxInstances = 2 }, Projectile.Center);
                }
                else
                {
                    Owner.SetScreenshake(7f);
                    SoundStyle soundBurst = new("CalamityMod/Sounds/Item/HolyBurst");
                    for (int i = 0; i < 3; i++)
                        SoundEngine.PlaySound(soundBurst with { Volume = 0.8f, Pitch = 0.2f * i, MaxInstances = 3 }, Projectile.Center);
                    SoundStyle soundExplosion = new("CalamityMod/Sounds/Item/HolyColliderProjectileHit");
                    SoundEngine.PlaySound(soundExplosion with { Volume = 0.65f, Pitch = 0.6f }, Projectile.Center);

                    for (int i = 0; i < 4; i++)
                    {
                        for (int y = 0; y < 24; y++)
                        {
                            bool red = Main.rand.NextBool(5);
                            float variance = Main.rand.NextFloat(0.7f, 1);
                            float placementVariance = 35;
                            Vector2 fxVel = Vector2.UnitY.RotatedBy(MathHelper.PiOver2 * i) * Main.rand.NextFloat(y * 1.2f, y * 1.5f) * (i == 0 ? 1.7f : 1) * variance;
                            Vector2 fxPos = crossCenter + Main.rand.NextVector2CircularEdge(placementVariance - variance * placementVariance, placementVariance - variance * placementVariance);
                            Dust dust = Dust.NewDustPerfect(fxPos, ModContent.DustType<LightDust>(), fxVel * 1.7f, 0, default, Main.rand.NextFloat(1.2f, 1.6f));
                            dust.noGravity = true;
                            dust.scale = (red ? 1.4f : 1.2f) * Main.rand.NextFloat(0.7f, 0.9f);
                            dust.color = red ? Color.Red : mainColor;
                            if (y % 2 == 0)
                            {
                                Particle spark = new GlowSparkParticle(fxPos, fxVel, false, 24, 0.065f, Main.rand.NextBool() ? Color.Khaki : mainColor, new Vector2(0.8f, 0.3f), true, false, 0.5f);
                                GeneralParticleHandler.SpawnParticle(spark);
                            }
                        }
                        Vector2 fxVel2 = Vector2.UnitY.RotatedBy(MathHelper.PiOver2 * i) * (i == 0 ? 1.7f : 1);
                        Particle bigCross = new CustomSpark(crossCenter + fxVel2 * 145, fxVel2, "CalamityMod/Particles/BloomLineFade", false, 15, 0.15f, mainColor, new Vector2(1.8f, 1.2f), true, true, 0, false, false, 0.8f, 0.8f, 0.8f);
                        GeneralParticleHandler.SpawnParticle(bigCross);
                    }
                }
                Projectile crossBurst = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), crossCenter, Vector2.Zero, ModContent.ProjectileType<ExorcismShockwave>(), (int)(Projectile.damage * 1.5f), 0f, Owner.whoAmI);
                crossBurst.Calamity().stealthStrike = true;
            }
            else
            {
                SoundStyle soundBurst = new("CalamityMod/Sounds/Item/HolyBurst");
                SoundEngine.PlaySound(soundBurst with { Volume = 1f, Pitch = 0 }, Projectile.Center);
                Projectile crossBurst = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), crossCenter, Vector2.Zero, ModContent.ProjectileType<ExorcismShockwave>(), (int)(Projectile.damage * 0.35f), 0f, Owner.whoAmI);
                for (int i = 0; i < 4; i++)
                {
                    for (int y = 0; y < 14; y++)
                    {
                        bool red = Main.rand.NextBool(5);
                        float variance = Main.rand.NextFloat(0.7f, 1);
                        float placementVariance = 27;
                        Vector2 fxVel = Vector2.UnitY.RotatedBy(MathHelper.PiOver2 * i) * Main.rand.NextFloat(y * 0.8f, y) * (i == 0 ? 1.7f : 1) * variance;
                        Vector2 fxPos = crossCenter + Main.rand.NextVector2CircularEdge(placementVariance - variance * placementVariance, placementVariance - variance * placementVariance);
                        Dust dust = Dust.NewDustPerfect(fxPos, ModContent.DustType<LightDust>(), fxVel * 1.5f, 0, default, Main.rand.NextFloat(1.2f, 1.6f));
                        dust.noGravity = true;
                        dust.scale = (red ? 1.2f : 1) * Main.rand.NextFloat(0.7f, 0.9f);
                        dust.color = red ? Color.Red : mainColor;
                        if (y % 2 == 0)
                        {
                            Particle spark = new GlowSparkParticle(fxPos, fxVel, false, 18, 0.05f, Main.rand.NextBool() ? Color.Khaki : mainColor, new Vector2(0.8f, 0.3f), true, false, 0.6f);
                            GeneralParticleHandler.SpawnParticle(spark);
                        }
                    }
                }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (stealth)
                return CalamityUtils.CircularHitboxCollision(Projectile.Center, 120, targetHitbox);
            else
                return base.Colliding(projHitbox, targetHitbox);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float glowUp = (1 + Utils.GetLerpValue(250, 0, Projectile.timeLeft, true));
            float throwCompletion = flung ? 1 : (float)Math.Pow(Math.Min(time / (Owner.HeldItem.useAnimation * 0.7f), 1), 5);
            Color glowColor = Color.Lerp(mainColor, Color.Khaki, glowUp - 1);
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Asset<Texture2D> glowBlade = ModContent.Request<Texture2D>("CalamityMod/Particles/GlowBlade");
            Asset<Texture2D> glowBottom = ModContent.Request<Texture2D>("CalamityMod/Particles/SquareRotated");
            Asset<Texture2D> woosh = ModContent.Request<Texture2D>("CalamityMod/Particles/VerticalSmearLarge");

            Vector2 drawAdjust = -Main.screenPosition + (!flung ? new Vector2(0, Owner.gfxOffY) : Vector2.Zero);

            if (hitTiles || falling || stealth)
            {
                for (int i = 0; i < 25; i++)
                {
                    Vector2 drawPos = Projectile.Center;
                    Color auraColor = glowColor with { A = 0 } * 0.05f * glowUp;
                    Vector2 drawOffset = (MathHelper.TwoPi * i / 25f).ToRotationVector2() * glowUp * 2;
                    Main.EntitySpriteDraw(tex, drawPos + drawAdjust + drawOffset + Main.rand.NextVector2Circular(6, 6) * (1 - glowUp), null, auraColor with { A = 0 } * Projectile.Opacity, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
                }
            }
            if (stealth)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 fxVel = Vector2.UnitY.RotatedBy(MathHelper.PiOver2 * i).RotatedBy(Projectile.rotation) * (i == 0 ? 1.2f : 1) * 80 * throwCompletion;
                    Vector2 fxPos = crossCenter + fxVel;

                    if (i == 0 && Projectile.timeLeft > 240)
                    {
                        Main.EntitySpriteDraw(woosh.Value, Projectile.Center + drawAdjust, null, Color.Khaki with { A = 0 } * 0.1f, fxVel.ToRotation() + Projectile.rotation + Main.GlobalTimeWrappedHourly * Projectile.direction * 27, woosh.Size() / 2f, Projectile.scale * 0.45f * throwCompletion, SpriteEffects.None, 0);
                        Main.EntitySpriteDraw(woosh.Value, Projectile.Center + drawAdjust, null, mainColor with { A = 0 } * 0.15f, fxVel.ToRotation() - 1.3f * Projectile.direction + Projectile.rotation + Main.GlobalTimeWrappedHourly * Projectile.direction * 27, woosh.Size() / 2f, Projectile.scale * 0.41f * throwCompletion, SpriteEffects.None, 0);
                    }

                    for (int y = 0; y < 2; y++)
                        Main.EntitySpriteDraw(glowBlade.Value, fxPos + drawAdjust, null, (y != 0 ? Color.White : mainColor) with { A = 0 } * (0.3f * glowUp), fxVel.ToRotation() + MathHelper.PiOver2, glowBlade.Size() / 2f, new Vector2(0.4f * (y != 0 ? 0.65f : 1f), 1f * throwCompletion * glowUp * (i == 0 ? 1.5f : 1)) * Projectile.scale * 0.04f, SpriteEffects.None, 0);
                    float softGlowUp = (float)Math.Pow(glowUp, 0.15f);
                    for (int y = 0; y < 3; y++)
                        Main.EntitySpriteDraw(glowBottom.Value, fxPos + drawAdjust - fxVel * 0.55f, null, (y != 0 ? Color.White : mainColor) with { A = 0 } * (0.1f * glowUp), fxVel.ToRotation() + MathHelper.PiOver2, glowBottom.Size() / 2f, new Vector2(1f * (y != 0 ? 0.65f : 1f) * softGlowUp, 1.7f * throwCompletion * softGlowUp * (i == 0 ? 1.5f : 1)) * Projectile.scale * (y == 2 ? 0.3f : 0.25f), SpriteEffects.None, 0);
                }
            }
            
            Main.EntitySpriteDraw(tex, Projectile.Center + drawAdjust, null, Color.Lerp(Color.White with { A = 0 }, lightColor, (2 - glowUp)) * (inSky ? 0 : 1), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
    }
}

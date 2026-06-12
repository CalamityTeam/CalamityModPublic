using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class TriactisHammerProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Items/Weapons/Melee/TriactisTruePaladinianMageHammerofMight";

        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/Item/PwnagehammerSound") { Volume = 0.6f };
        public static readonly SoundStyle HitSoundGFB = new("CalamityMod/Sounds/Item/CalamityBell");
        public static readonly SoundStyle WindUpSound = new("CalamityMod/Sounds/Item/GalaxySmasherClone") { Volume = 0.9f };
        public static readonly SoundStyle SmashSound = new("CalamityMod/Sounds/Item/GalaxySmasherSmash") { Volume = 0.7f };
        public static readonly SoundStyle SmashSoundGFB = new("CalamityMod/Sounds/Item/TF2PanHit");

        public static float ExplosionDamageKBMult = 2f;
        public static float SuperHammerDamageMult = 3f;
        public static float SmashHomingRange = 800f; // 50 tiles
        public static float WindUpTime = 216f; // 1.2 seconds
        public static float ConvergeTime = 18f; // 0.1 seconds

        public ref float AirTime => ref Projectile.ai[0];
        public ref float HammerState => ref Projectile.ai[1]; // R, G, B, Center
        public ref float SmashTarget => ref Projectile.ai[2];
        public Player Owner => Main.player[Projectile.owner];
        public float OrbitRadius;

        public static Asset<Texture2D> EchoHammer;
        public override void Load() => EchoHammer = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/TriactisHammerEcho");

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 168;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 3;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            // Smashing hammers
            if (HammerState > 0f)
            {
                // Central spot (invisible)
                if (HammerState == 4f)
                {
                    // Target should already be set -- if not, retarget
                    NPC target = Main.npc[(int)SmashTarget];
                    if (target is null || target.life <= 0 || !target.active || target.dontTakeDamage || target.immortal)
                    {
                        target = Projectile.Center.ClosestNPCAt(SmashHomingRange, bossPriority: true);
                        if (target != null)
                            SmashTarget = target.whoAmI;
                    }                        

                    // Strongly locks onto the target center
                    // If there is no target, it would just rest there
                    if (target != null)
                        Projectile.Center = target.Center;

                    // Explode when told to
                    if (AirTime == -1f)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TriactisHammerExplosion>(), (int)(Projectile.damage * ExplosionDamageKBMult), Projectile.knockBack * ExplosionDamageKBMult, Projectile.owner);
                        SoundEngine.PlaySound(Main.zenithWorld ? SmashSoundGFB : SmashSound, Projectile.Center);
                        Projectile.Kill();
                        return;
                    }
                }
                // Orbital RGB hammers
                else
                {
                    // Need the central spot to exist or they explode immediately
                    float rotation = Main.GlobalTimeWrappedHourly * 2f + MathHelper.ToRadians(120f) * HammerState;
                    Color currentColor = TriactisHammerFlare.GetColor(HammerState);
                    Projectile target = Main.projectile[(int)SmashTarget];
                    if (target is null || !target.active)
                    {
                        Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, currentColor, Vector2.One, 0f, 0.2f, 4f, 20);
                        GeneralParticleHandler.SpawnParticle(pulse);
                        Projectile.Kill();
                        return;
                    }

                    // Initialize orbit radius
                    if (AirTime == 0f)
                        OrbitRadius = MathHelper.Clamp(Vector2.Distance(Projectile.Center, target.Center), 64f, 400f);

                    // Move outwards for a bit
                    AirTime++;
                    if (AirTime < WindUpTime)
                    {
                        OrbitRadius = MathHelper.Lerp(OrbitRadius, 800f, 0.04f);
                        Projectile.Center = target.Center + Vector2.UnitX.RotatedBy(rotation) * OrbitRadius;
                        Projectile.rotation = Projectile.AngleFrom(target.Center) + MathHelper.PiOver4;                        
                        // Scales up to full size after a short portion of the animation time
                        Projectile.scale = Utils.GetLerpValue(0f, WindUpTime * 0.1f, AirTime, true);

                        if (AirTime == 1f)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                Vector2 velocity = Vector2.UnitX.RotatedBy(rotation).RotatedByRandom(MathHelper.ToRadians(36f)) * Main.rand.NextFloat(10f, 30f);
                                float scale = Main.rand.NextFloat(0.8f, 1.5f);
                                Particle sparkle = new CritSpark(Projectile.Center, velocity, Color.White, currentColor, 1f * scale, 24, 0.1f, 2.5f * scale);
                                GeneralParticleHandler.SpawnParticle(sparkle);
                            }
                        }

                        if (OrbitRadius > 792f)
                        {
                            Particle streak = new ManaDrainStreak(Owner, Main.rand.NextFloat(0.6f, 1f), Main.rand.NextVector2Unit() * Main.rand.NextFloat(160f, 320f), 0f, currentColor, Projectile.GetAlpha(Color.White), Main.rand.Next(10, 20), Projectile.Center);
                            GeneralParticleHandler.SpawnParticle(streak);
                            if (AirTime % 18f == 0f)
                            {
                                Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Projectile.GetAlpha(Color.White), Vector2.One, 0f, 0.2f, 2.5f, 12);
                                GeneralParticleHandler.SpawnParticle(pulse);
                            }
                        }
                    }
                    // Then converge at the middle where the central spot is
                    else
                    {
                        OrbitRadius = MathHelper.Lerp(800f, 0f, (AirTime - WindUpTime) / ConvergeTime);
                        Projectile.Center = target.Center + Vector2.UnitX.RotatedBy(rotation) * OrbitRadius;
                        Projectile.rotation = Projectile.AngleTo(target.Center) + MathHelper.PiOver4;

                        // Trigger the central spot to explode which then causes these hammers to explode too
                        if (Projectile.Hitbox.Intersects(target.Hitbox) || AirTime > WindUpTime + ConvergeTime)
                            target.ai[0] = -1f;
                    }
                }
            }
            // Normal hammers
            else if (HammerState == 0f)
            {
                AirTime++;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + Projectile.direction * MathHelper.ToRadians(2f) * AirTime;
                Projectile.velocity.X *= 0.99f;

                if (Projectile.velocity.Y < 20f)
                    Projectile.velocity.Y += 0.2f;

                // Dust trail
                Dust trail = Dust.NewDustPerfect(Projectile.Center, DustID.GoldFlame, Projectile.velocity * 0.3f);
                trail.position += Main.rand.NextVector2Unit() * Main.rand.NextFloat(0f, 64f);
                trail.noGravity = true;

                // Prevent the projectile from wreacking too much havoc off-screen
                if (Vector2.Distance(Projectile.Center, Owner.MountedCenter) >= 2000f)
                    Projectile.Kill();
            }
            // Returning hammers
            else
            {
                // Slows down a little
                if (AirTime < 30f)
                {
                    AirTime++;
                    Projectile.velocity *= 0.9f;
                    float idealRotation = Projectile.AngleTo(Owner.MountedCenter) + MathHelper.PiOver4;
                    Projectile.rotation = MathHelper.Lerp(Projectile.rotation, idealRotation, AirTime / 30f);
                }
                // Then return to the player
                else
                {
                    Projectile.rotation = Projectile.AngleTo(Owner.MountedCenter) + MathHelper.PiOver4;
                    Projectile.velocity = Projectile.SafeDirectionTo(Owner.MountedCenter) * 25f;
                    if (Projectile.Hitbox.Intersects(Owner.Hitbox) || Vector2.Distance(Projectile.Center, Owner.MountedCenter) >= 2000f)
                        Projectile.Kill();
                }

                // (Slightly less) dust trail
                if (!Main.rand.NextBool(4))
                {
                    Dust trail = Dust.NewDustPerfect(Projectile.Center, DustID.GoldFlame, Projectile.velocity * 0.2f);
                    trail.position += Main.rand.NextVector2Unit() * Main.rand.NextFloat(0f, 64f);
                    trail.noGravity = true;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            switch (HammerState)
            {
                case 1f:
                    return Color.Red;
                case 2f:
                    return new Color(132, 225, 26);
                case 3f:
                    return new Color(117, 170, 239);
            }
            return Color.White;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.numHits > 0 || HammerState > 0f)
                return;

            int FlareCount = Owner.ownedProjectileCounts[ModContent.ProjectileType<TriactisHammerFlare>()] + 1;

            if (Main.zenithWorld)
                SoundEngine.PlaySound(HitSoundGFB with { Pitch = FlareCount * 0.15f - 0.15f }, Projectile.Center);
            else
                SoundEngine.PlaySound(HitSound with { Pitch = FlareCount * 0.15f - 0.15f }, Projectile.Center);

            // Simple on-hit dust ring on all normal hits
            for (int i = 0; i < 32; i++)
            {
                Vector2 velocity = Vector2.UnitX.RotatedBy(MathHelper.TwoPi * i / 32f) * (8f + 2f * FlareCount) * (i % 2 == 0 ? 1.2f : 1f);
                Dust ring = Dust.NewDustPerfect(Projectile.Center, DustID.GoldFlame, velocity);
                ring.noGravity = true;
                ring.noLight = true;
                ring.scale = (i % 2 == 0 ? 1.6f : 2.4f) * (0.7f + 0.3f * FlareCount);
            }

            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.type == ModContent.ProjectileType<TriactisHammerFlare>() && p.owner == Owner.whoAmI)
                {
                    // Update target of orbit to the most recently hit one
                    // Create telporting particles if this happens to be a different target
                    if (p.ai[1] != target.whoAmI)
                    {
                        p.ai[1] = target.whoAmI;
                        for (int i = 0; i < 5; i++)
                        {
                            Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(6f, 10f);
                            Particle sparkle = new CritSpark(p.Center, velocity, Color.White, TriactisHammerFlare.GetColor(p.ai[0]), 1f, 24, 0.1f, 2.4f);
                            GeneralParticleHandler.SpawnParticle(sparkle);
                        }
                    }

                    // Set the flares up to transform themselves if ready
                    if (FlareCount > 3)
                    {
                        p.ai[1] = -2f;
                        p.ai[2] = Projectile.whoAmI;
                    }
                    p.netUpdate = true;
                }
            }

            // Change state and turn invisible
            if (FlareCount > 3)
            {
                Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Color.Lerp(Color.Gold, Color.White, 0.5f), Vector2.One, 0f, 0.2f, 2.5f, 24);
                GeneralParticleHandler.SpawnParticle(pulse);
                SoundEngine.PlaySound(WindUpSound, Projectile.Center);
                HammerState = 4f;
                SmashTarget = target.whoAmI;
                Projectile.velocity = Vector2.Zero;
                Projectile.ExpandHitboxBy(4);
                Projectile.netUpdate = true;
            }
            else
            {
                // Spawn flares and set to return to player
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<TriactisHammerFlare>(), (int)(Projectile.damage * SuperHammerDamageMult), 0f, Projectile.owner, FlareCount, target.whoAmI);
                HammerState = -1f;
                Projectile.netUpdate = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (HammerState == 4f)
                return false;

            if (HammerState > 0f)
            {
                Main.spriteBatch.EnterShaderRegion(BlendState.Additive);
                Texture2D echoTex = EchoHammer.Value;
                Main.EntitySpriteDraw(echoTex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, echoTex.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
                Main.spriteBatch.ExitShaderRegion();
                return false;
            }

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        // Echo Hammers can only hit when they converge
        public override bool? CanDamage()
        {
            if (HammerState == 4f)
                return false;

            if (HammerState > 0f && AirTime < WindUpTime)
                return false;
            return base.CanDamage();
        }
    }
}

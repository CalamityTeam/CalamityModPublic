using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Projectiles.Melee
{
    public class GalaxySmasherHammer : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Items/Weapons/Melee/GalaxySmasher";
        public static readonly SoundStyle UseSound = new("CalamityMod/Sounds/Item/PwnagehammerSound") { Volume = 0.35f };
        public static readonly SoundStyle RedHamSound = new("CalamityMod/Sounds/Item/GalaxySmasherClone") { Volume = 0.6f };
        public static readonly SoundStyle UseSoundFunny = new("CalamityMod/Sounds/Item/CalamityBell") { Volume = 1.5f };
        public ref int EmpoweredHammer => ref Main.player[Projectile.owner].Calamity().GalaxyHammer;
        public int returnhammer = 0;
        public float rotatehammer = 10f;
        public int PulseCooldown = 0;
        public float EchoHammerPrep = 0f;
        public float WaitTimer = 0f;
        public int InPulse = 0;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 14;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 62;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 120;
        }


        public override void AI()
        {
            //returnhammer determines if the hammer is slowing down after hitting an enemy, or homing in on the player.
            Player player = Main.player[Projectile.owner];
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
            Projectile.rotation += MathHelper.ToRadians(rotatehammer) * Projectile.direction;
            
            if (EmpoweredHammer >= 8)
                EmpoweredHammer = 0;

            if (returnhammer == 0)
            {
                Projectile.velocity.X *= 0.988f;
                Projectile.velocity.Y += 0.326f;
            }

            if (returnhammer == 1) //Hammer slows after the inital hit
            {
                if (EmpoweredHammer == 7)
                {
                    ++WaitTimer;
                    if (WaitTimer == 20f)
                    {
                        Projectile.velocity *= 0f;
                        EmpoweredHammer = 0;
                        returnhammer = 3;
                    }
                }
                else
                {
                    Projectile.velocity.Y *= 0.926f;
                    Projectile.velocity.X *= 0.811f;
                    if (Projectile.velocity.X > -1.05f && Projectile.velocity.X < 1.05f & Projectile.velocity.Y > -1.05f && Projectile.velocity.Y < 1.05f)
                        returnhammer = 2;
                }
            }

            if (returnhammer == 2) // Projectile returns to player
            {
                ++PulseCooldown;
                Projectile.extraUpdates = 2;
                float returnSpeed = StellarContempt.Speed;
                float acceleration = 1.1f;
                Player owner = Main.player[Projectile.owner];
                Vector2 playerCenter = owner.Center;
                float xDist = playerCenter.X - Projectile.Center.X;
                float yDist = playerCenter.Y - Projectile.Center.Y;
                float dist = (float)Math.Sqrt(xDist * xDist + yDist * yDist);
                dist = returnSpeed / dist;
                xDist *= dist;
                yDist *= dist;

                if (Projectile.velocity.X < xDist)
                {
                    Projectile.velocity.X = Projectile.velocity.X + acceleration;
                    if (Projectile.velocity.X < 0f && xDist > 0f)
                        Projectile.velocity.X += acceleration;
                }
                else if (Projectile.velocity.X > xDist)
                {
                    Projectile.velocity.X = Projectile.velocity.X - acceleration;
                    if (Projectile.velocity.X > 0f && xDist < 0f)
                        Projectile.velocity.X -= acceleration;
                }
                if (Projectile.velocity.Y < yDist)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + acceleration;
                    if (Projectile.velocity.Y < 0f && yDist > 0f)
                        Projectile.velocity.Y += acceleration;
                }
                else if (Projectile.velocity.Y > yDist)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - acceleration;
                    if (Projectile.velocity.Y > 0f && yDist < 0f)
                        Projectile.velocity.Y -= acceleration;
                }
                // Delete the projectile if it touches its owner, increase counter to the big hammer, and spawn a dustsplosion on the player that scales with how close they are to getting a big hammer.
                if (Main.myPlayer == Projectile.owner)
                {
                    if (Projectile.Hitbox.Intersects(player.Hitbox))
                    {
                        EmpoweredHammer++;
                        SoundEngine.PlaySound(SoundID.DD2_BetsysWrathShot with { Volume = 0.4f }, Projectile.Center);
                        for (int i = 0; i < 30; i++)
                        {
                            Dust fire = Dust.NewDustPerfect(Projectile.Center, 181);
                            fire.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(0.8f) * new Vector2(4f, 1.25f) * Main.rand.NextFloat(0.9f, 1f);
                            fire.velocity = fire.velocity.RotatedBy(Projectile.rotation - MathHelper.PiOver2);
                            fire.velocity += Projectile.velocity * (EmpoweredHammer * 0.04f);

                            fire.noGravity = true;
                            fire.scale = Main.rand.NextFloat(0.2f, 0.6f) * EmpoweredHammer;

                            fire = Dust.CloneDust(fire);
                            fire.velocity = Main.rand.NextVector2Circular(3f, 3f);
                            fire.velocity += Projectile.velocity * (EmpoweredHammer * 0.04f);
                        }
                        Projectile.Kill();
                    }
                }
            }
            if (returnhammer == 3) //Hammer prepares to spawn echo hammer, VibroIntensity
            {
                if (InPulse == 0)
                {
                    SoundEngine.PlaySound(RedHamSound, Projectile.Center);
                    Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Color.Aqua, new Vector2(1f, 1f), Main.rand.NextFloat(12f, 25f), 0.1f, 1.95f, 120);
                    GeneralParticleHandler.SpawnParticle(pulse);
                    InPulse = 1;
                }

                rotatehammer -= 0.25f;
                if (EchoHammerPrep >= 17f)
                {
                    if (InPulse == 1)
                    {
                        Particle pulse2 = new StaticPulseRing(Projectile.Center, Vector2.Zero, Color.Orchid, new Vector2(1f, 1f), 0f, 4.9f, 0f, 25);
                        GeneralParticleHandler.SpawnParticle(pulse2);
                        InPulse = 2;
                    }
                }

                if (EchoHammerPrep <= 24f)
                {
                    EchoHammerPrep += 0.2f;
                    Projectile.scale += 0.017f;
                    Particle streak = new ManaDrainStreak(player, Main.rand.NextFloat(0.7f, 0.9f), Main.rand.NextVector2CircularEdge(2f, 2f) * Main.rand.NextFloat(135f, 335f), Main.rand.NextFloat(20f, 20f), Color.Aqua, Color.Fuchsia, Main.rand.Next(10, 20), Projectile.Center);
                    GeneralParticleHandler.SpawnParticle(streak);
                }
                else
                {
                    int hammer = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<GalaxySmasherEcho>(), Projectile.damage * 10, Projectile.knockBack * 2.5f, Projectile.owner, 0f, Projectile.ai[1]);
                    Main.projectile[hammer].localAI[0] = Math.Sign(Projectile.velocity.X);
                    Main.projectile[hammer].netUpdate = true;
                    Projectile.Kill();
                }
            }

            //Spawn dust as the hammer travels.
            if (Main.rand.NextBool())
            {
                Vector2 offset = new Vector2(12, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(4, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Center.Y) + offset, 272, new Vector2(Projectile.velocity.X * 0.4f + velOffset.X, Projectile.velocity.Y * 0.4f + velOffset.Y), 100, default, 0.7f);
                dust.noGravity = true;
            }

            if (Main.rand.NextBool(6))
            {
                Vector2 offset = new Vector2(12, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(4, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Center.Y) + offset, 226, new Vector2(Projectile.velocity.X * 0.5f + velOffset.X, Projectile.velocity.Y * 0.5f + velOffset.Y), 100, default, 0.7f);
                dust.noGravity = true;
            }
        }
        // On hit play GONG and spawn dust.
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (returnhammer == 0)
            {
                if (Main.zenithWorld)
                    SoundEngine.PlaySound(UseSoundFunny with { Pitch = EmpoweredHammer * 0.05f - 0.05f }, Projectile.Center);

                else
                    SoundEngine.PlaySound(UseSound with { Pitch = EmpoweredHammer * 0.05f - 0.05f }, Projectile.Center);

                int FunSizeHamID = ModContent.ProjectileType<GalaxySmasherMini>();
                int FunSizeHamDamage = (int)(0.1f * Projectile.damage);
                float FunSizeHamKB = 0.2f;
                int NumOfFunSizeHams = EmpoweredHammer + 1;
                for (int i = 0; i < NumOfFunSizeHams; ++i)
                {
                    float startDist = Main.rand.NextFloat(160f, 190f);
                    Vector2 startDir = Main.rand.NextVector2Unit();
                    Vector2 startPoint = Projectile.Center + startDir * startDist;

                    float FunSizeHamSpeed = Main.rand.NextFloat(10f, 14f);
                    Vector2 velocity = startDir * -FunSizeHamSpeed;

                    if (Projectile.owner == Main.myPlayer)
                    {
                        int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), startPoint, velocity, FunSizeHamID, FunSizeHamDamage, FunSizeHamKB, Projectile.owner);
                        if (proj.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[proj].DamageType = DamageClass.MeleeNoSpeed;
                            Main.projectile[proj].tileCollide = false;
                            Main.projectile[proj].timeLeft = 30;
                            Main.projectile[proj].extraUpdates = 2;
                        }
                    }
                }

                returnhammer = 1;
            }
            if (PulseCooldown >= 15)
            { 
                Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Color.Violet, new Vector2(0.5f, 0.5f), Main.rand.NextFloat(12f, 25f), 0.2f, 1.4f + (EmpoweredHammer * 0.1f), 14);
                GeneralParticleHandler.SpawnParticle(pulse);
                PulseCooldown = 0;
            }   
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.22f }, Projectile.Center);
            Projectile.ai[1] = target.whoAmI;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Projectile.damage = (int)(Projectile.damage * 0.9f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 3);
            return false;
        }
    }
}

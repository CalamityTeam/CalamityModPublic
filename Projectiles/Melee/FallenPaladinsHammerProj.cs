using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class FallenPaladinsHammerProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Items/Weapons/Melee/FallenPaladinsHammer";
        public static readonly SoundStyle UseSound = new("CalamityMod/Sounds/Item/PwnagehammerSound") { Volume = 0.35f};
        public static readonly SoundStyle UseSoundFunny = new("CalamityMod/Sounds/Item/CalamityBell") { Volume = 1.5f};
        public static readonly SoundStyle RedHamSound = new("CalamityMod/Sounds/Item/FallenPaladinsHammerClone") { Volume = 0.6f };
        public ref int EmpoweredHammer => ref Main.player[Projectile.owner].Calamity().PHAThammer; 
        public int returnhammer = 0;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
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
            Projectile.rotation += MathHelper.ToRadians(22.5f) * Projectile.direction;

            if (returnhammer == 0)
            {
                Projectile.velocity.X *= 0.9711f;
                Projectile.velocity.Y += 0.426f;
            }
            
            if (returnhammer == 1)
                {
                    Projectile.velocity.Y *= 0.926f;
                    Projectile.velocity.X *= 0.811f;
                    if (Projectile.velocity.X > -1.05f && Projectile.velocity.X < 1.05f & Projectile.velocity.Y > -1.05f && Projectile.velocity.Y < 1.05f)
                
                    returnhammer = 2;
                }
                    
            if (returnhammer == 2)
                        {
                            float returnSpeed = FallenPaladinsHammer.Speed;
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
                                        if (EmpoweredHammer == 3)
                                            {
                                                SoundEngine.PlaySound(RedHamSound, Projectile.Center);
                                                for (int i = 0; i < 30; i++)
                                                {
                                                    Dust fire = Dust.NewDustPerfect(Projectile.Center, 218);
                                                    fire.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(0.8f) * new Vector2(4f, 1.25f) * Main.rand.NextFloat(0.9f, 1f);
                                                    fire.velocity = fire.velocity.RotatedBy(Projectile.rotation - MathHelper.PiOver2);
                                                    fire.velocity += Projectile.velocity * (EmpoweredHammer * 0.1f);

                                                    fire.noGravity = true;
                                                    fire.scale = Main.rand.NextFloat(0.5f, 1.5f) * EmpoweredHammer;

                                                    fire = Dust.CloneDust(fire);
                                                    fire.velocity = Main.rand.NextVector2Circular(3f, 3f);
                                                    fire.velocity += Projectile.velocity * (EmpoweredHammer * 0.1f);
                                                }

                                                int hammer = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<FallenPaladinsHammerEcho>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 0f, Projectile.ai[1]);
                                                Main.projectile[hammer].localAI[0] = Math.Sign(Projectile.velocity.X);
                                                Main.projectile[hammer].netUpdate = true;
                                                EmpoweredHammer = 0;
                                            }
                                        else
                                            {
                                                SoundEngine.PlaySound(SoundID.DD2_BetsysWrathShot with { Volume = 0.4f }, Projectile.Center);
                                                for (int i = 0; i < 30; i++)
                                                {
                                                    Dust fire = Dust.NewDustPerfect(Projectile.Center, 218);
                                                    fire.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(0.8f) * new Vector2(4f, 1.25f) * Main.rand.NextFloat(0.9f, 1f);
                                                    fire.velocity = fire.velocity.RotatedBy(Projectile.rotation - MathHelper.PiOver2);
                                                    fire.velocity += Projectile.velocity * (EmpoweredHammer * 0.1f);

                                                    fire.noGravity = true;
                                                    fire.scale = Main.rand.NextFloat(0.5f, 1.5f) * EmpoweredHammer;

                                                    fire = Dust.CloneDust(fire);
                                                    fire.velocity = Main.rand.NextVector2Circular(3f, 3f);
                                                    fire.velocity += Projectile.velocity * (EmpoweredHammer * 0.1f);
                                                }
                                            }
                                        
                                        Projectile.Kill();
                                    }
                                }
                        }
             
            //Spawn dust as the hammer travels.
            if (Main.rand.NextBool())
            {
                Vector2 offset = new Vector2(12, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(4, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Center.Y) + offset, DustID.RedTorch, new Vector2(Projectile.velocity.X * 0.2f + velOffset.X, Projectile.velocity.Y * 0.2f + velOffset.Y), 100, new Color(205, 38, 38), 2f);
                dust.noGravity = true;
            }

            if (Main.rand.NextBool(6))
            {
                Vector2 offset = new Vector2(12, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(4, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Center.Y) + offset, DustID.RedTorch, new Vector2(Projectile.velocity.X * 0.2f + velOffset.X, Projectile.velocity.Y * 0.2f + velOffset.Y), 100, new Color(205, 38, 38), 2f);
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
                        SoundEngine.PlaySound(UseSoundFunny with { Pitch = EmpoweredHammer * 0.2f - 0.4f }, Projectile.Center);

                    else
                        SoundEngine.PlaySound(UseSound with { Pitch = EmpoweredHammer * 0.2f - 0.4f }, Projectile.Center);

                returnhammer = 1;
                    }
                float numberOfDusts = 35f;
                float rotFactor = 360f / numberOfDusts;
                for (int i = 0; i < numberOfDusts; i++)
                {
                    float rot = MathHelper.ToRadians(i * rotFactor);
                    Vector2 offset = new Vector2(3.6f, 0).RotatedBy(rot * Main.rand.NextFloat(1.1f, 4.1f));
                    Vector2 velOffset = new Vector2(3f, 0).RotatedBy(rot * Main.rand.NextFloat(1.1f, 4.1f));
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, 90, new Vector2(velOffset.X, velOffset.Y));
                    dust.noGravity = true;
                    dust.velocity = velOffset;
                    dust.scale = Main.rand.NextFloat(1.5f, 3.2f);
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
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }
    }
}

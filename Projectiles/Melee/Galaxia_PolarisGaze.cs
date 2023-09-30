using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;
using CalamityMod.Sounds;

namespace CalamityMod.Projectiles.Melee
{
    public class PolarisGaze : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Items/Weapons/Melee/GalaxiaExtra"; //Red cuz close range yget the deal
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public ref float Shred => ref Projectile.ai[0]; //Charge, basically
        public ref float HitChargeCooldown => ref Projectile.ai[1];
        public ref float ChargeSoundCooldown => ref Projectile.localAI[0];
        public float Bounce(float x) => x <= 50 ? x / 50f : x <= 65 ? 1 + 0.15f * (float)Math.Sin((x - 50f) / 15f * MathHelper.Pi) : 1f;
        public float ShredRatio => MathHelper.Clamp(Shred / (maxShred * 0.5f), 0f, 1f);
        public Player Owner => Main.player[Projectile.owner];
        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;

        public const float maxShred = 650; //How much shred you get

        public Projectile Wheel;
        public bool Dashing;
        public Vector2 DashStart;

        public Particle[] Rings = new Particle[3];
        public Particle PolarStar;


        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 70;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = FourSeasonsGalaxia.PolarisAttunement_LocalIFrames;
        }

        public override bool? CanDamage()
        {
            return Shred >= FourSeasonsGalaxia.PolarisAttunement_LocalIFrames; //Prevent spam click abuse
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLength = 145 * Projectile.scale;
            float bladeWidth = 86 * Projectile.scale;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center, Owner.Center + (direction * bladeLength), bladeWidth, ref collisionPoint);
        }

        public override void AI()
        {
            if (!initialized) //Initialization.
            {
                SoundEngine.PlaySound(SoundID.Item90, Projectile.Center);
                initialized = true;

                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.type == ProjectileType<PolarisGazeStar>() && proj.owner == Owner.whoAmI)
                    {
                        if (CalamityUtils.AngleBetween(Owner.Center - Main.MouseWorld, Owner.Center - proj.Center) > MathHelper.PiOver4)
                        {
                            proj.Kill();
                            break;
                        }

                        Wheel = proj;
                        Dashing = true;
                        DashStart = Owner.Center;
                        Wheel.timeLeft = 60;
                        Owner.GiveIFrames(FourSeasonsGalaxia.PolarisAttunement_SlashIFrames);
                        break;
                    }
                }
            }

            if (!OwnerCanShoot)
            {
                Projectile.Kill();
                return;
            }


            #region sparkles and particles

            float bladeLength = 120 * Projectile.scale;
            for (int i = 0; i < 3; i++)
            {
                if (Rings[i] == null)
                {
                    Rings[i] = new ConstellationRingVFX(Owner.Center + (direction * (25 + bladeLength * 0.33f * i)), Color.DarkOrchid, direction.ToRotation(), Projectile.scale * 0.25f * i, new Vector2(0.5f, 1f), spinSpeed: 7, starAmount: 3 + i, important: true);
                    GeneralParticleHandler.SpawnParticle(Rings[i]);
                }
                else
                {
                    Rings[i].Time = 0;
                    Rings[i].Position = Owner.Center + Vector2.Lerp(Vector2.Zero, ((0.9f + 0.1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 10f - i * 0.5f)) * direction * (25 + bladeLength * 0.33f * (i + 1))), Bounce(MathHelper.Clamp(Shred - 30 * i, 0, maxShred)));
                    Rings[i].Rotation = direction.ToRotation();
                    Rings[i].Scale = Projectile.scale * 0.25f * (i + 1);
                    (Rings[i] as ConstellationRingVFX).Opacity = 0.5f + 0.5f * ShredRatio;
                }
            }

            if (PolarStar == null)
            {
                PolarStar = new GenericSparkle(Owner.Center + direction, Vector2.Zero, Color.White, Color.CornflowerBlue, Projectile.scale, 2, 0.05f, 5f, true);
                GeneralParticleHandler.SpawnParticle(PolarStar);
            }
            else
            {
                PolarStar.Time = 0;
                PolarStar.Position = Owner.Center + direction * 46 * Projectile.scale;
                PolarStar.Rotation += (1 + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 4f)) * 0.02f;
                PolarStar.Scale = Projectile.scale * 2f;
            }

            if (Main.rand.NextBool())
            {
                Vector2 smokeSpeed = direction.RotatedByRandom(MathHelper.PiOver4 * 0.3f) * Main.rand.NextFloat(10f, 30f) * (ShredRatio * 0.5f + 1f);
                Particle smoke = new HeavySmokeParticle(Projectile.Center, smokeSpeed + Owner.velocity, Color.Lerp(Color.Purple, Color.Indigo, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f)), 30, Main.rand.NextFloat(0.6f, 1.2f) * (ShredRatio * 0.5f + 1f), 0.8f, 0, false, 0, true);
                GeneralParticleHandler.SpawnParticle(smoke);

                if (Main.rand.Next(3) == 0)
                {
                    Particle smokeGlow = new HeavySmokeParticle(Projectile.Center, smokeSpeed + Owner.velocity, Main.hslToRgb(0.55f, 1, 0.5f + 0.2f * ShredRatio), 20, Main.rand.NextFloat(0.4f, 0.7f) * (ShredRatio * 0.5f + 1f), 0.8f, 0, true, 0.01f, true);
                    GeneralParticleHandler.SpawnParticle(smokeGlow);
                }
            }
            #endregion

            if (Shred >= maxShred)
                Shred = maxShred;
            if (Shred < 0)
                Shred = 0;

            //Manage position and rotation
            direction = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.Zero);
            direction.Normalize();
            Projectile.rotation = direction.ToRotation();
            Projectile.Center = Owner.Center + (direction * 60);

            //Scaling based on shred
            Projectile.localNPCHitCooldown = FourSeasonsGalaxia.PolarisAttunement_LocalIFrames - (int)(MathHelper.Lerp(0, FourSeasonsGalaxia.PolarisAttunement_LocalIFrames - FourSeasonsGalaxia.PolarisAttunement_LocalIFramesCharged, ShredRatio)); //Increase the hit frequency
            Projectile.scale = 1f + (ShredRatio * 1f); //SWAGGER


            if ((Wheel == null || !Wheel.active) && Dashing)
            {
                Dashing = false;
                Owner.velocity *= 0.1f; //Abrupt stop
                SoundEngine.PlaySound(CommonCalamitySounds.MeatySlashSound, Projectile.Center);

                if (Owner.whoAmI == Main.myPlayer)
                {
                    int blastDamage = Owner.CalcIntDamage<MeleeDamageClass>(FourSeasonsGalaxia.PolarisAttunement_SlashBoltsDamage);
                    for (int i = 0; i < 5; i++)
                    {
                        Projectile blast = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center, Main.rand.NextVector2CircularEdge(15, 15), ProjectileType<GalaxiaBolt>(), blastDamage, 0f, Owner.whoAmI, 0.55f, MathHelper.Pi * 0.02f);
                        {
                            blast.timeLeft = 100;
                        }
                    }


                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center - DashStart / 2f, Vector2.Zero, ProjectileType<PolarisGazeDash>(), (int)(Projectile.damage * FourSeasonsGalaxia.PolarisAttunement_SlashDamageBoost), 0, Owner.whoAmI);
                    if (proj.ModProjectile is PolarisGazeDash dash)
                    {
                        dash.DashStart = DashStart;
                        dash.DashEnd = Owner.Center;
                    }
                }

            }

            Owner.Calamity().LungingDown = false;

            if (Dashing)
            {
                Owner.Calamity().LungingDown = true;
                Owner.fallStart = (int)(Owner.position.Y / 16f);
                Owner.velocity = Owner.SafeDirectionTo(Wheel.Center, Vector2.Zero) * 60f;

                if (Owner.Distance(Wheel.Center) < 60f)
                    Wheel.active = false;
            }

            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = Projectile.whoAmI;
            Owner.direction = Math.Sign(direction.X);
            Owner.itemRotation = direction.ToRotation();
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= 3.14f;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            //Play a sound when the blade throw is available
            if (ShredRatio > 0.85 && Owner.whoAmI == Main.myPlayer)
            {
                if (ChargeSoundCooldown <= 0)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BookStaffCast with { Volume = SoundID.DD2_BookStaffCast.Volume * 2.5f });
                    ChargeSoundCooldown = 20;
                }
            }
            else
            {
                ChargeSoundCooldown--;
            }


            Shred += FourSeasonsGalaxia.PolarisAttunement_ShredChargeupGain;
            HitChargeCooldown--;
            Projectile.timeLeft = 2;
        }

        //Since the iframes vary, adjust the damage to be consistent no matter the iframes. The true scaling happens between the BaseDamage and the FulLChargeDamage
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float maxMultiplier = FourSeasonsGalaxia.PolarisAttunement_FullChargeDamage / (float)FourSeasonsGalaxia.PolarisAttunement_BaseDamage;
            float damageMultiplier = MathHelper.Lerp(1f, maxMultiplier, ShredRatio);
            //Adjust the damage to make it constant based on the local iframes
            float damageReduction = Projectile.localNPCHitCooldown / (float)FourSeasonsGalaxia.PolarisAttunement_LocalIFrames;

            modifiers.SourceDamage *= damageMultiplier * damageReduction;
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            float maxMultiplier = FourSeasonsGalaxia.PolarisAttunement_FullChargeDamage / (float)FourSeasonsGalaxia.PolarisAttunement_BaseDamage;
            float damageMultiplier = MathHelper.Lerp(1f, maxMultiplier, ShredRatio);
            //Adjust the damage to make it constant based on the local iframes
            float damageReduction = Projectile.localNPCHitCooldown / (float)FourSeasonsGalaxia.PolarisAttunement_LocalIFrames;

            modifiers.SourceDamage *= damageMultiplier * damageReduction;
        }



        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => ShredTarget();
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => ShredTarget();

        private void ShredTarget()
        {
            if (Main.myPlayer != Owner.whoAmI)
                return;

            Owner.fallStart = (int)(Owner.position.Y / 16f);
            // get lifted up
            if (HitChargeCooldown <= 0)
            {
                SoundEngine.PlaySound(SoundID.NPCHit30, Projectile.Center); //Sizzle
                Shred += 80; //Augment the shredspeed
                if (Owner.velocity.Y > 0)
                    Owner.velocity.Y = -2f; //Get "stuck" into the enemy partly
                Owner.GiveIFrames(FourSeasonsGalaxia.PolarisAttunement_ShredIFrames); // i framez.
                HitChargeCooldown = 20;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit43, Projectile.Center);
            if (ShredRatio > 0.85 && Owner.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, direction * 36f, ProjectileType<PolarisGazeStar>(), (int)(Projectile.damage * FourSeasonsGalaxia.PolarisAttunement_ShotDamageBoost), Projectile.knockBack, Owner.whoAmI, Shred);
            }
            if (Dashing)
            {
                Owner.velocity *= 0.1f; //Abrupt stop
            }

            Owner.Calamity().LungingDown = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D sword = Request<Texture2D>("CalamityMod/Items/Weapons/Melee/GalaxiaExtra").Value;

            float drawAngle = direction.ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;

            Vector2 drawOrigin = new Vector2(0f, sword.Height);
            Vector2 drawOffset = Owner.Center + direction * 10f - Main.screenPosition;

            Main.EntitySpriteDraw(sword, drawOffset, null, lightColor, drawRotation, drawOrigin, Projectile.scale, 0f, 0);

            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(initialized);
            writer.WriteVector2(direction);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            initialized = reader.ReadBoolean();
            direction = reader.ReadVector2();
        }
    }
}

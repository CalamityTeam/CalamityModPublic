using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.Sounds;
using System.CodeDom;

namespace CalamityMod.Projectiles.Rogue
{
    public class DynamicPursuerProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public bool ReturningToPlayer
        {
            get => Projectile.ai[0] == 1f;
            set => Projectile.ai[0] = value.ToInt();
        }

        public float Time
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public const float MaxTargetSearchDistance = 800;
        public float ElectricVelocityCharge = 0f;
        public float LaserVelocityCharge = 0f;
        public bool Ricochet = false;
        public NPC nextTarget = null;
        public int glowmaskFrame = 0;

        //Variables inherited from the weapon, they can be changed with DragonLens
        public float ReturnAcceleration = DynamicPursuer.ReturnAcceleration;
        public float ReturnMaxSpeed = DynamicPursuer.ReturnMaxSpeed;
        public float RicochetVelocityCap = DynamicPursuer.RicochetVelocityCap;
        public float ElectricityDmgMult = DynamicPursuer.ElectricityDmgMult;
        public float ElectricityCooldown = DynamicPursuer.ElectricityCooldown;
        public float RicochetShootingCooldown = DynamicPursuer.RicochetShootingCooldown;
        public float LaserDmgMult = DynamicPursuer.LaserDmgMult;
        public float LaserCooldown = DynamicPursuer.LaserCooldown;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 320; // 160
            Projectile.timeLeft = 600; //300 cuz extra updates
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.Blue.ToVector3());

            Player player = Main.player[Projectile.owner];

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 8)
            {
                glowmaskFrame++;
                Projectile.frameCounter = 0;
            }
            if (glowmaskFrame >= 9)
                glowmaskFrame = 0;

            Time++;
            if (!ReturningToPlayer)
            {
                if (Time >= 45f && !Ricochet)
                {
                    ReturningToPlayer = true;
                    Projectile.tileCollide = false;
                    Projectile.netUpdate = true;
                }
                else if (Ricochet)
                {
                    if (nextTarget != null)
                    {
                        Projectile.velocity = (float)Math.Pow(Math.E, Time / 175) * (nextTarget.Center - Projectile.Center).SafeNormalize(Vector2.One);

                        // Cap velocity to prevent projectile vomit and to see easier where its going
                        if (Projectile.velocity.X > RicochetVelocityCap)
                            Projectile.velocity.X = RicochetVelocityCap;
                        if (Projectile.velocity.X < -RicochetVelocityCap)
                            Projectile.velocity.X = -RicochetVelocityCap;
                        if (Projectile.velocity.Y > RicochetVelocityCap)
                            Projectile.velocity.Y = RicochetVelocityCap;
                        if (Projectile.velocity.Y < -RicochetVelocityCap)
                            Projectile.velocity.Y = -RicochetVelocityCap;
                    }
                    else
                    {
                        Ricochet = false;
                        ReturningToPlayer = true;
                        Projectile.tileCollide = false;
                        Projectile.netUpdate = true;
                    }

                    ElectricVelocityCharge += Projectile.velocity.Length();

                    if (ElectricVelocityCharge >= RicochetShootingCooldown)
                    {
                        ElectricVelocityCharge = 0f;
                        AttemptToFireElectricity((int)(Projectile.damage * ElectricityDmgMult));
                        AttemptToFireLasers((int)(Projectile.damage * LaserDmgMult));
                    }
                }
            }
            else
            {
                float distanceFromPlayer = Projectile.Distance(player.Center);
                if (distanceFromPlayer > 2800f)
                    Projectile.Kill();

                if (Projectile.Calamity().stealthStrike)
                    ReturnMaxSpeed = (float)Math.Pow(Math.E, Time / 125);
                else ReturnMaxSpeed = (float)Math.Pow(Math.E, Time / 150);

                Vector2 idealVelocity = (player.Center - Projectile.Center) / distanceFromPlayer * ReturnMaxSpeed;

                ReturnAcceleration = (float)Math.Pow(Math.E, Time / 300);
                Projectile.velocity.X += Math.Sign(idealVelocity.X - Projectile.velocity.X) * ReturnAcceleration;
                Projectile.velocity.Y += Math.Sign(idealVelocity.Y - Projectile.velocity.Y) * ReturnAcceleration;

                // Cap velocity to prevent projectile vomit and to see easier where its going
                if (Projectile.velocity.X > RicochetVelocityCap)
                    Projectile.velocity.X = RicochetVelocityCap;
                if (Projectile.velocity.X < -RicochetVelocityCap)
                    Projectile.velocity.X = -RicochetVelocityCap;
                if (Projectile.velocity.Y > RicochetVelocityCap)
                    Projectile.velocity.Y = RicochetVelocityCap;
                if (Projectile.velocity.Y < -RicochetVelocityCap)
                    Projectile.velocity.Y = -RicochetVelocityCap;

                ElectricVelocityCharge += Projectile.velocity.Length();
                LaserVelocityCharge += Projectile.velocity.Length();
                if (ElectricVelocityCharge >= ElectricityCooldown)
                {
                    ElectricVelocityCharge = 0f;
                    AttemptToFireElectricity((int)(Projectile.damage * ElectricityDmgMult));
                }

                if (Projectile.Calamity().stealthStrike && LaserVelocityCharge >= LaserCooldown)
                {
                    LaserVelocityCharge = 0f;
                    Projectile.velocity = Vector2.Zero;
                    AttemptToFireLasers((int)(Projectile.damage * LaserDmgMult));
                }

                if (Main.myPlayer == Projectile.owner)
                {
                    if (Projectile.Hitbox.Intersects(player.Hitbox))
                        Projectile.Kill();
                }
            }

            Projectile.rotation += 0.25f;
        }

        public void AttemptToFireElectricity(int damage)
        {
            if (Main.myPlayer != Projectile.owner)
                return;
            NPC potentialTarget = Projectile.Center.ClosestNPCAt(MaxTargetSearchDistance);
            Vector2 blueGem = (Vector2.UnitY * (-12)).RotatedBy(Projectile.rotation);
            if (potentialTarget != null)
            {
                Vector2 initialVelocity = Projectile.SafeDirectionTo(potentialTarget.Center) * 2f;
                if (Main.rand.NextBool())
                    initialVelocity = initialVelocity.RotatedByRandom(0.4f);
                float initialAngle = initialVelocity.ToRotation();
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + blueGem, Projectile.SafeDirectionTo(potentialTarget.Center) * 3f, ModContent.ProjectileType<DynamicPursuerElectricity>(), damage, Projectile.knockBack, Projectile.owner, initialAngle, Main.rand.Next(100));

            }
        }
        public void AttemptToFireLasers(int damage)
        {
            Vector2 direction1 = new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10));
            Vector2 direction2 = new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10));
            Vector2 redExtremity1 = new Vector2(-27, 7).RotatedBy(Projectile.rotation);
            Vector2 redExtremity2 = new Vector2(27, 7).RotatedBy(Projectile.rotation);
            SoundEngine.PlaySound(SoundID.Item12 with { Volume = SoundID.Item12.Volume * 0.4f }, Projectile.Center);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + redExtremity1, direction1 * 3f, ModContent.ProjectileType<DynamicPursuerLaser>(), damage, Projectile.knockBack, Projectile.owner);
            SoundEngine.PlaySound(SoundID.Item12 with { Volume = SoundID.Item12.Volume * 0.4f }, Projectile.Center);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + redExtremity2, direction2 * 3f, ModContent.ProjectileType<DynamicPursuerLaser>(), damage, Projectile.knockBack, Projectile.owner);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(CommonCalamitySounds.SwiftSliceSound, Projectile.position);
            if (((Projectile.Calamity().stealthStrike && Projectile.numHits == 4) || (!Projectile.Calamity().stealthStrike) && !ReturningToPlayer))
            {
                if ((Projectile.Calamity().stealthStrike && Projectile.numHits == 4) && !ReturningToPlayer)
                {
                    if (Main.myPlayer == Projectile.owner)
                        //TODO: Change explosion color somehow
                       Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PlasmaGrenadeSmallExplosion>(), Projectile.damage * 3/4, Projectile.knockBack * 2f, Projectile.owner);

                    {
                        for (int i = 0; i < 220; i++)
                        {
                            int type = Main.rand.NextBool() ? 261 : 107;
                            Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10f, 10f), type);
                            dust.scale = Main.rand.NextFloat(1.6f, 2.2f);
                            dust.velocity = Main.rand.NextVector2CircularEdge(75f, 75f);
                            dust.noGravity = true;
                            if (type == 261)
                            {
                                dust.velocity *= 1.5f;
                            }
                        }
                    }
                }
                ReturningToPlayer = true;
                Ricochet = false;
            }

            else
            {
                //Retarget
                Ricochet = true;
                NPC newTarget = null;
                float closestNPCDistance = 2800f;
                float targettingDistance = MaxTargetSearchDistance * 2f;


                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (i == target.whoAmI)
                        continue;

                    if (Main.npc[i].CanBeChasedBy(Projectile))
                    {
                        float potentialNewDistance = (Projectile.Center - Main.npc[i].Center).Length();
                        if (potentialNewDistance < targettingDistance && potentialNewDistance < closestNPCDistance)
                        {
                            closestNPCDistance = potentialNewDistance;
                            newTarget = Main.npc[i];
                            nextTarget = newTarget;
                            if (Projectile.timeLeft < 300)
                                Projectile.timeLeft = 300;
                        }
                    }
                }
                                
                if (newTarget == null)
                {
                    float potentialNewDistance = (Projectile.Center - Main.npc[target.whoAmI].Center).Length();
                    if (potentialNewDistance < targettingDistance && potentialNewDistance < closestNPCDistance)
                    {
                        closestNPCDistance = potentialNewDistance;
                        newTarget = Main.npc[target.whoAmI];
                        nextTarget = newTarget;
                        Projectile.timeLeft += 300; //Just in case a target it can ricochet to pops up
                    }
                    return;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            ReturningToPlayer = true;
            Projectile.tileCollide = false;
            Projectile.netUpdate = true;
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, ProjectileID.Sets.TrailCacheLength[Projectile.type]);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowmaskTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/DynamicPursuerProjectileGlowmask").Value;
            Rectangle glowmaskRectangle = glowmaskTexture.Frame(1, 9, 0, glowmaskFrame);
            Vector2 origin = glowmaskRectangle.Size()/2f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            SpriteEffects direction = SpriteEffects.None;
            
            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, direction, 0);
            Main.EntitySpriteDraw(glowmaskTexture, drawPosition, glowmaskRectangle, Color.White, Projectile.rotation, origin, Projectile.scale, direction, 0);

            return false;
        }
    }
}

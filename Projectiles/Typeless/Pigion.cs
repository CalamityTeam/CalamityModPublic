using System;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class Pigion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public Player Owner => Main.player[Projectile.owner];
        public CalamityPlayer moddedOwner => Owner.Calamity();
        public Color cl1 = Color.Gold;
        public Color cl2 = Color.Goldenrod;
        public int hopTimer = 0;
        public static int hopMax = 100;
        public bool hitGround = false;
        public bool hitWall = false;
        public Vector2 lastUnmodVel;
        public Vector2 mousePos;
        public int thrownTimer = 0; // Timer set when grabbed, fades out faster at low velocity, allowing gravity to take effect sooner
        public static int thrownTimerMax = 300;
        public static int lowestSpeed = 4;
        public static int highestSpeed = 20;
        public bool deadPig = false; // if Pigion hit a wall at high enough speed to explode
        public ref float time => ref Projectile.ai[0];
        public ref float minionNumber => ref Projectile.ai[1];
        public bool pigGrabbed => Projectile.ai[2] == 5;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.DamageType = AverageDamageClass.Instance;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
        }
        public void Frames()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6 * Projectile.MaxUpdates)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= 2)
                Projectile.frame = 0;
        }
        public override void AI()
        {
            mousePos = Owner.ClampedMouseWorld();
            if (time == 0)
            {
                time = Main.rand.Next(1, 800 + 1);
                Projectile.spriteDirection = Main.rand.NextBool() ? -1 : 1;
                lastUnmodVel = Projectile.velocity;
            }

            Frames();
            
            // Make an idle sound
            if (Main.rand.NextBool(280) && Projectile.soundDelay == 0)
            {
                SoundStyle oink = new("CalamityMod/Sounds/Item/Swine", 2);
                SoundEngine.PlaySound(oink with { Volume = 0.35f, Pitch = Main.rand.NextFloat(0.3f, 0.5f), MaxInstances = 5 }, Projectile.Center);
                Projectile.soundDelay = 180;
            }

            // Teleport back to player if too far away
            if (Projectile.Center.Distance(Owner.Center) > 1500)
            {
                Projectile.velocity = new Vector2(0, -16).RotatedByRandom(MathHelper.Pi / 2);
                Projectile.Center = Owner.Center;
            }

            // Check if being clicked and no other Pigions are being clicked
            if (Owner.controlUseItem && Projectile.Center.Distance(mousePos) <= 60 && !pigGrabbed && Projectile.ai[2] == 0)
                SetGrab(true);

            // Push away enemies it touches based on the Pigions velocity
            Vector2 toNPC = Vector2.Zero;
            NPC npc = Projectile.Center.ClosestNPCAt(300, false);
            if (npc != null && !pigGrabbed && Projectile.Hitbox.Intersects(npc.Hitbox))
            {
                toNPC = Projectile.Center.DirectionTo(npc.Center);
                npc.MoveNPC(Utils.DirectionTo(Projectile.Center, npc.Center), Utils.Remap(Projectile.velocity.Length(), lowestSpeed, highestSpeed * 1.5f, 4, 30), true);
            }

            float hitboxSizeMult = 0.45f;
            bool touchingWall = Collision.SolidCollision(Vector2.Lerp(Projectile.TopLeft, Projectile.Left, 0.15f) + Vector2.UnitX * Projectile.velocity.X, 1, (int)(Projectile.height * hitboxSizeMult)) || Collision.SolidCollision(Vector2.Lerp(Projectile.TopRight, Projectile.Right, 0.15f) + Vector2.UnitX * Projectile.velocity.X, 1, (int)(Projectile.height * hitboxSizeMult));
            if (touchingWall)
            {
                FlipDirection(-Projectile.spriteDirection);
                Projectile.velocity.X += Projectile.spriteDirection;
            }
            if (!pigGrabbed) // Check if hitting tiles or enemies, and if so rebound the Pigion or kill them
                Slam(touchingWall, hitboxSizeMult, toNPC);
            else
                thrownTimer = thrownTimerMax;
            lastUnmodVel = Projectile.velocity;

            Movement();

            thrownTimer -= 1 + (int)(20 * (Utils.GetLerpValue(highestSpeed, lowestSpeed * 2, Projectile.velocity.Length(), true)));
            if (thrownTimer < 0)
                thrownTimer = 0;
            hopTimer--;
            time++;
            Projectile.timeLeft++;
            if (moddedOwner.friendlyMinions < minionNumber || Owner.dead || deadPig)
            {
                Projectile.Kill();
                return;
            }
        }
        public void Slam(bool touchingWall, float hitboxSizeMult, Vector2 touchNPCVel)
        {
            bool onPlatform = false;
            float thrownMult = Utils.Remap(thrownTimer, 0, thrownTimerMax, 0.6f, 1f);
            if (Projectile.velocity.Y > 0) // If moving down, check for platforms
            {
                for (int i = 4; i < 8; i++)
                {
                    Point bottom = (Projectile.Bottom + Vector2.UnitY * i).ToTileCoordinates();
                    if (TileID.Sets.Platforms[CalamityUtils.ParanoidTileRetrieval(bottom.X, bottom.Y).TileType])
                    {
                        onPlatform = true;
                    }
                }
            }
            float velX = MathF.Abs(lastUnmodVel.X);
            float velY = MathF.Abs(lastUnmodVel.Y);
            bool hitNPC = touchNPCVel != Vector2.Zero;
            bool hitDownWall = (Collision.SolidCollision(Projectile.Bottom + Vector2.UnitY * Projectile.velocity.Y, (int)(Projectile.width * hitboxSizeMult), 1) || onPlatform);
            bool hitUpWall = Collision.SolidCollision(Projectile.Top + Vector2.UnitY * Projectile.velocity.Y, (int)(Projectile.width * hitboxSizeMult), 1);
            bool hitDown = velY > lowestSpeed && hitDownWall;
            bool hitUp = velY > lowestSpeed && hitUpWall;
            bool hitSide = velX > lowestSpeed && touchingWall;
            bool hardHit = Projectile.velocity.Length() >= highestSpeed * 1.5f; // The velocity a Pigion must be going to explode on impact
            float volumeMult = Utils.GetLerpValue(lowestSpeed - 2, highestSpeed, Projectile.velocity.Length(), true); // Sounds are quieter at low velocity

            bool hitAnything = false;
            if (hitDown || hitUp)
            {
                if (!hitGround)
                {
                    hitAnything = true;
                    if (hardHit)
                    {
                        deadPig = true;
                        return;
                    }
                    Projectile.velocity.Y = -lastUnmodVel.Y * thrownMult;
                    MakeDustAndSound(volumeMult);
                    hitGround = true;
                }
            }
            else
                hitGround = false;

            if (hitSide)
            {
                hitAnything = true;
                if (!hitWall)
                {
                    if (hardHit)
                    {
                        deadPig = true;
                        return;
                    }
                    Projectile.velocity.X = -lastUnmodVel.X * thrownMult;
                    MakeDustAndSound(volumeMult);
                    hitWall = true;
                }
            }
            else
                hitWall = false;

            if (hitNPC && Projectile.velocity.Length() >= lowestSpeed)
            {
                hitAnything = true;
                if (hardHit)
                {
                    deadPig = true;
                    return;
                }
                Vector2 npcBounceVel = -lastUnmodVel * thrownMult;
                Projectile.Center += npcBounceVel;
                Projectile.velocity = npcBounceVel;
                MakeDustAndSound(volumeMult);
            }

            if (hitAnything)
            {
                FlipDirection(MathF.Sign(Projectile.velocity.X));
            }
        }
        public void MakeDustAndSound(float volumeMult)
        {
            SoundStyle splat = new("CalamityMod/Sounds/NPCHit/PerfSmallHit", 3);
            SoundEngine.PlaySound(splat with { Volume = 0.8f * volumeMult, Pitch = Main.rand.NextFloat(0.3f, 0.4f) - volumeMult * 0.3f }, Projectile.Center);

            Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            float power = 0.2f + Utils.GetLerpValue(0, 40, Projectile.velocity.Length());
            for (int i = 0; i < (int)(12 * power); i++)
            {
                float variance = Main.rand.NextFloat(-0.8f, 0.8f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SquashDustPixelated>(),
                    vel.RotatedBy(variance).RotatedByRandom(MathF.Abs(variance) / 3) * Main.rand.NextFloat(15.0f, 18.0f) * MathF.Pow((1 - MathF.Abs(variance)), 2), 0, default, power * Main.rand.NextFloat(2.3f, 3.2f) - MathF.Abs(variance));
                dust.noGravity = Main.rand.NextBool(2, 3);
                dust.color = Main.rand.NextBool() ? cl1 : cl2;
                dust.customData = new Vector2(0.8f, 1.1f);
                dust.fadeIn = 3.5f * power;
            }
        }
        public void FlipDirection(int newDirection)
        {
            Projectile.spriteDirection = newDirection;
            time = 1;
        }
        public void SetGrab(bool isGrabbed) => Projectile.ai[2] = (isGrabbed ? 5 : 0);
        public void Movement()
        {
            // Flip movement direction every once in a while
            if (time % 900 == 0)
                FlipDirection(-Projectile.spriteDirection);

            // Slowing down over time
            if (thrownTimer <= thrownTimerMax / 2)
                Projectile.velocity.X *= 0.97f;

            // Move Pigion to mouse if grabbed, otherwise ungrab them if player isn't holding M1 anymore
            if (pigGrabbed)
            {
                FlipDirection(MathF.Sign(Projectile.Center.DirectionTo(mousePos).X));
                Vector2 goalVel = (mousePos - Projectile.Center) / (18);
                Projectile.velocity = goalVel;
            }
            if (!Owner.controlUseItem)
                SetGrab(false);

            // Falling
            if (Projectile.velocity.Y < 14 && thrownTimer <= thrownTimerMax / 2)
                Projectile.velocity.Y += 0.3f * Utils.GetLerpValue(100, 0, hopTimer, true);

            int walkRate = 90; // Walk along slowly
            if (!pigGrabbed && time % walkRate == 0)
            {
                Projectile.velocity.X += 1 * Projectile.spriteDirection;
            }

            // lil hops when near other Pigions
            for (int x = 0; x < Main.maxProjectiles; x++)
            {
                Projectile projectile = Main.projectile[x];
                bool validPig = projectile.active && projectile.type == Projectile.type && projectile != Projectile;
                float distance = Vector2.Distance(Projectile.Center, projectile.Center);
                if (validPig)
                {
                    if (pigGrabbed)
                        projectile.ai[2] = 1; // set all other Pigions to be counted as not grabable
                    if (distance <= 80 && distance != 0 && hopTimer <= 0 && thrownTimer == 0)
                    {
                        int directionOfJump = -MathF.Sign(Projectile.Center.DirectionTo(projectile.Center).X);
                        Projectile.velocity += new Vector2(Main.rand.NextFloat(-2.5f, -4) * directionOfJump, -2);
                        FlipDirection(-directionOfJump);
                        hopTimer = 100;
                    }
                }
            }

            // Rotate based on velocity and direction
            float goalRot = Utils.AngleLerp(0, Projectile.velocity.ToRotation() + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0), Utils.GetLerpValue(lowestSpeed, highestSpeed, Projectile.velocity.Length(), true));
            Projectile.rotation = goalRot;
        }
        public override void OnKill(int timeLeft)
        {
            if (deadPig) // Only spawn explosion if death is from collision
            {
                int pigNum = Owner.ownedProjectileCounts[Projectile.type];
                float blastSize = 100;
                float minMultiplier = 0.1f;
                int hitsToMinMult = 4;
                Projectile blast = Projectile.NewProjectileDirect(Owner.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BasicBurst>(), (int)(Projectile.damage*pigNum), -15, Owner.whoAmI, blastSize, minMultiplier, hitsToMinMult);
                blast.timeLeft = 5;
                blast.DamageType = AverageDamageClass.Instance;
                blast.CritChance = 100;
            }

            Owner.SetScreenshake(3);
            SoundStyle die = new("CalamityMod/Sounds/Item/PigionSqueal");
            SoundEngine.PlaySound(die with { Volume = 0.55f, Pitch = Main.rand.NextFloat(-0.2f, 0.4f), MaxInstances = 7 }, Projectile.Center);
            SoundStyle die2 = new("CalamityMod/Sounds/NPCKilled/PerfMediumDeath");
            SoundEngine.PlaySound(die2 with { Volume = 0.6f, Pitch = Main.rand.NextFloat(0.2f, 0.3f) }, Projectile.Center);
            for (int i = 0; i < 24; i++)
            {
                Vector2 dustVel = (Vector2.One * 6).RotatedByRandom(100);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SquashDustPixelated>(), dustVel * Main.rand.NextFloat(0.1f, 0.8f));
                dust.noGravity = Main.rand.NextBool(2, 3);
                dust.scale = Main.rand.NextFloat(0.5f, 0.8f);
                dust.color = Color.Red;
                dust.noLightEmittence = true;
                dust.fadeIn = -0.4f;

                if (i % 2 == 0)
                {
                    Particle pulse1 = new CustomSpark(Projectile.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1, 6), "CalamityMod/Particles/BloomCircle", false, Main.rand.Next(20, 35 + 1), Main.rand.NextFloat(0.2f, 0.35f), Color.Red * 0.55f, new Vector2(1f, 1f), noShrink: true);
                    GeneralParticleHandler.SpawnParticle(pulse1);
                    pulse1.Pixelate = true;
                }
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = pigGrabbed;
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame(1, 2, 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 squash = new Vector2(Utils.Remap(Projectile.velocity.Length(), lowestSpeed, highestSpeed, 1, 2), Utils.Remap(Projectile.velocity.Length(), lowestSpeed, highestSpeed, 1, 0.5f));
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, origin, squash * Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally :  SpriteEffects.None, 0);
            return false;
        }
        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;
    }
}

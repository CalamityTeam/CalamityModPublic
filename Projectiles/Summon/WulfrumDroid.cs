using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terraria.Audio;
using CalamityMod.Particles;

namespace CalamityMod.Projectiles.Summon
{
    public class WulfrumDroid : ModProjectile
    {
        public static readonly SoundStyle HelloSound = new("CalamityMod/Sounds/Custom/WulfrumDroidSpawnBeep") { PitchVariance = 0.4f };
        public static readonly SoundStyle PewSound = new("CalamityMod/Sounds/Custom/WulfrumDroidFire") { PitchVariance = 0.4f, Volume = 0.6f, MaxInstances = 0 };
        public static readonly SoundStyle RandomChirpSound = new("CalamityMod/Sounds/Custom/WulfrumDroidChirp", 4) { PitchVariance = 0.3f };
        public static readonly SoundStyle HurrySound = new("CalamityMod/Sounds/Custom/WulfrumDroidHurry", 2) { PitchVariance = 0.3f };
        public int NewSoundDelay
        {
            get
            {
                int minionCount = 1;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.owner == Owner.whoAmI && proj.type == Type && proj.whoAmI != Projectile.whoAmI)
                    {
                        minionCount++;
                    }
                }

                return (int)(Main.rand.Next(340, 1660) * minionCount);
            }
        }

        public float Initialized = 0f;
        public static float AggroRange = 450f;
        public static float ShootDelay = 110f; //This is the max delay, but it can b e shorter
        public enum BehaviorState { Aggressive, Idle, BuffOwner };
        public BehaviorState State
        {
            get => (BehaviorState)(int)Projectile.ai[0];

            set => Projectile.ai[0] = (float)value;
        }

        public ref float ShootTimer => ref Projectile.ai[1];
        public NPC Target
        {
            get
            {
                NPC target = null;

                if (Owner.HasMinionAttackTargetNPC)
                    target = CheckNPCTargetValidity(Main.npc[Owner.MinionAttackTargetNPC]);

                if (target != null)
                    return target;

                else
                {
                    for (int npcIndex = 0; npcIndex < Main.npc.Length; npcIndex++)
                    {
                        target = CheckNPCTargetValidity(Main.npc[npcIndex]);
                        if (target != null)
                            return target;
                    }
                }

                return null;
            }
        }

        public Player Owner => Main.player[Projectile.owner];


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Droid");
            Main.projFrames[Projectile.type] = 12;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 32;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Initialized = 0f;
        }

        //Returns the npc if targetable, returns null if not
        public NPC CheckNPCTargetValidity(NPC potentialTarget)
        {
            if (potentialTarget.CanBeChasedBy(this, false))
            {
                float targetDist = Vector2.Distance(potentialTarget.Center, Projectile.Center);

                if ((targetDist < AggroRange) && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, potentialTarget.position, potentialTarget.width, potentialTarget.height))
                {
                    return potentialTarget;
                }
            }

            return null;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            //Spawn dust
            if (Initialized == 0f)
            {
                int dustAmt = Main.rand.Next(10, 16);
                for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
                {
                    Vector2 direction = Main.rand.NextVector2CircularEdge(1f, 1f);

                    Dust wishyDust = Dust.NewDustPerfect(Projectile.Center + direction * Main.rand.NextFloat(1f, 8f), 229, Alpha : 100, Scale: Main.rand.NextFloat(1f, 1.4f));
                    wishyDust.noGravity = true;
                    wishyDust.noLight = true;
                    wishyDust.velocity = direction * Main.rand.NextFloat(2f, 4);
                }

                SoundEngine.PlaySound(HelloSound, Projectile.Center);
                Projectile.soundDelay = NewSoundDelay;

                Initialized ++;
            }


            //Do frame stuff i guess
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 8)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }


            //Buff stuff
            player.AddBuff(ModContent.BuffType<WulfrumDroidBuff>(), 3600);
            if (player.dead)
                modPlayer.wDroid = false;
            if (modPlayer.wDroid)
                Projectile.timeLeft = 2;
            Projectile.MinionAntiClump();

            
            //Lets only recalculate our target once per frame aight
            NPC targetCache = Target;
            float separationAnxietyDist = targetCache != null ? 1000f : 500f; //Have more lenience on how far away from the player should the drone return if theyre attacking an enemy

            if (Projectile.soundDelay > 0)
                Projectile.soundDelay--;
            else
            {
                SoundEngine.PlaySound(RandomChirpSound with { Volume = RandomChirpSound.Volume * Main.rand.NextFloat(0.5f, 1f) }, Projectile.Center);
                Projectile.soundDelay = NewSoundDelay;

                if (targetCache == null)
                {
                    Vector2 emoteDirection = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2 * 0.7f);

                    Particle emote = new WulfrumDroidEmote(Projectile.Center + emoteDirection * 10f, emoteDirection * Main.rand.NextFloat(3f, 5f), Main.rand.Next(30, 65), Main.rand.NextFloat(1.4f, 2f));
                    GeneralParticleHandler.SpawnParticle(emote);
                }
            }

            //Return to the player if too far away from them. Hurry up droids!
            if (Vector2.Distance(Owner.Center, Projectile.Center) > separationAnxietyDist)
            {
                State = BehaviorState.Idle;
                Projectile.netUpdate = true;

                //play the hurry sound once and never again until it is reset
                if (Projectile.soundDelay < 10000)
                    SoundEngine.PlaySound(HurrySound, Projectile.Center);

                Projectile.soundDelay = 10010;

                if (Main.rand.NextBool(7))
                {
                    Vector2 emoteDirection = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2 * 0.7f);
                    emoteDirection.X -= Math.Sign(Projectile.velocity.X) * 1f;

                    Particle emote = new WulfrumDroidSweatEmote(Projectile.Center + emoteDirection * 10f, emoteDirection * Main.rand.NextFloat(3f, 5f), Main.rand.Next(20, 35), Main.rand.NextFloat(1.4f, 2f));
                    GeneralParticleHandler.SpawnParticle(emote);
                }
            }

            else if (Projectile.soundDelay >= 10000)
                Projectile.soundDelay = NewSoundDelay;

            if (targetCache != null && State == BehaviorState.Aggressive)
            {
                Vector2 vectorToTarget = targetCache.Center - Projectile.Center;
                float distanceToTarget = vectorToTarget.Length();
                vectorToTarget = vectorToTarget.SafeNormalize(Vector2.Zero);

                //Accelerate towards target if far away
                if (distanceToTarget > 200f)
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, vectorToTarget * 6f, 1 / 41f);

                //"Float" the minion
                else if (Projectile.velocity.Y > -1f)
                    Projectile.velocity.Y -= 0.1f;

                //Small tweak that makes the minion hop away from the targets center if its too aligned with it vertically
                if (Math.Abs(Projectile.Center.X - targetCache.Center.X) < 10f)
                    Projectile.velocity.X += 4f * Math.Sign(Projectile.Center.X - targetCache.Center.X);
            }


            else
            {
                //Become passive if theres obstruction between it and the player
                if (!Collision.CanHitLine(Projectile.Center, 1, 1, player.Center, 1, 1))
                    State = BehaviorState.Idle;

                float returnSpeed = State == BehaviorState.Idle ? 15f : 6f;

                //Accelerate towards the player if too far & not fast enough
                Vector2 playerVec = player.Center - Projectile.Center + new Vector2(0f, -60f);
                float playerDist = playerVec.Length();
                if (playerDist > 200f && returnSpeed < 9f)
                    returnSpeed = 9f;


                if (playerDist < 100f && State == BehaviorState.Idle && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {
                    State = BehaviorState.Aggressive;
                    Projectile.netUpdate = true;
                }

                //Teleport to player if too far away.
                if (playerDist > 2000f)
                {
                    Projectile.Center = player.Center;
                    Projectile.netUpdate = true;
                }

                //Aim towards the top of the player
                else if (playerDist > 70f)
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, playerVec.SafeNormalize(Vector2.Zero) * returnSpeed, 1 / 21f);

                else
                {
                    //Get unstuck if your movespeed is zero
                    if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
                        Projectile.velocity = Main.rand.NextVector2CircularEdge(1f, 1f) * -0.15f;

                    //Accelerate more
                    Projectile.velocity *= 1.01f;
                }
            }

            Projectile.rotation = Projectile.velocity.X * 0.05f;
            Projectile.spriteDirection = Projectile.direction = Math.Sign(Projectile.velocity.X);

            //Increase the timer by a random number
            if (ShootTimer > 0f)
                ShootTimer += (float) Main.rand.Next(1, 4);

            if (ShootTimer > ShootDelay)
            {
                ShootTimer = 0f;
                Projectile.netUpdate = true;
            }

            if (ShootTimer > 0)
                return;

            ShootTimer++;

            if (targetCache == null)
                return;

            SoundEngine.PlaySound(PewSound, Projectile.Center);
            //Recoil
            Vector2 velocity = targetCache.Center - Projectile.Center;
            velocity.Normalize();
            velocity *= 10f;
            Projectile.velocity += velocity * -0.3f;


            if (Main.myPlayer == Projectile.owner)
            {
                int bolt = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<WulfrumBoltMinion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                Main.projectile[bolt].originalDamage = Projectile.originalDamage;
                Main.projectile[bolt].netUpdate = true;
                Projectile.netUpdate = true;
            }
        }

        public override bool? CanDamage() => false;
    }
}

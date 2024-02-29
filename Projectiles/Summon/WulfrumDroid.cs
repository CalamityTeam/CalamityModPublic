using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terraria.Audio;
using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Accessories;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.Graphics.Effects;
using static Terraria.ModLoader.ModContent;
using ReLogic.Content;
using System.IO;
using CalamityMod.Graphics.Primitives;

namespace CalamityMod.Projectiles.Summon
{
    public class WulfrumDroid : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public static readonly SoundStyle HelloSound = new("CalamityMod/Sounds/Custom/WulfrumDroidSpawnBeep") { PitchVariance = 0.4f };
        public static readonly SoundStyle PewSound = new("CalamityMod/Sounds/Custom/WulfrumDroidFire") { PitchVariance = 0.4f, Volume = 0.6f, MaxInstances = 0 };
        public static readonly SoundStyle RandomChirpSound = new("CalamityMod/Sounds/Custom/WulfrumDroidChirp", 4) { PitchVariance = 0.3f };
        public static readonly SoundStyle HurrySound = new("CalamityMod/Sounds/Custom/WulfrumDroidHurry", 2) { PitchVariance = 0.3f };
        public static readonly SoundStyle RepairSound = new("CalamityMod/Sounds/Custom/WulfrumDroidRepair") { Volume = 0.8f, PitchVariance = 0.3f, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew };
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

                return (int)(Main.rand.Next(340, 1460) * minionCount);
            }
        }

        internal Color PrimColorMult;
        public Player healedPlayer;

        public float Initialized = 0f;
        public static float AggroRange = 450f;
        public static float ShootDelay = 110f; //This is the max delay, but it can b e shorter
        public enum BehaviorState { Aggressive, Idle };
        public BehaviorState State
        {
            get => (BehaviorState)(int)Projectile.ai[0];

            set => Projectile.ai[0] = (float)value;
        }

        public ref float ShootTimer => ref Projectile.ai[1];

        public ref float AyeAyeCaptainCooldown => ref Projectile.localAI[0];
        public ref float NuzzleFlashTime => ref Projectile.localAI[1];

        public float BuffModeBuffer = 0f;

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

        public static Asset<Texture2D> SheenTex;


        public override void SetStaticDefaults()
        {
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
                ShootTimer = ShootDelay;

                Initialized ++;
            }

            if (Owner.Calamity().mouseRight && Owner.HeldItem.type == ModContent.ItemType<WulfrumController>())
            {
                if (BuffModeBuffer > 0)
                {
                    BuffModeBuffer--;
                    Projectile.netUpdate = true;
                }
            }

            else if (BuffModeBuffer < 15)
                BuffModeBuffer = 15;

            bool buffMode = BuffModeBuffer <= 0;

            //Do frame stuff i guess
            Projectile.frame = Projectile.frame % (Main.projFrames[Projectile.type] / 2);

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 8)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type] / 2)
            {
                Projectile.frame = 0;
            }

            if (buffMode)
                Projectile.frame += Main.projFrames[Projectile.type] / 2;


            //Buff stuff
            player.AddBuff(ModContent.BuffType<WulfrumDroidBuff>(), 3600);
            if (player.dead)
                modPlayer.wDroid = false;
            if (modPlayer.wDroid)
                Projectile.timeLeft = 2;
            Projectile.MinionAntiClump();


            //Lets only recalculate our target once per frame aight
            NPC targetCache = buffMode ? null : Target;
            float separationAnxietyDist = targetCache != null ? 1000f : 500f; //Have more lenience on how far away from the player should the drone return if theyre attacking an enemy

            if (AyeAyeCaptainCooldown > 0)
                AyeAyeCaptainCooldown--;
            else if (buffMode)
            {
                SoundEngine.PlaySound(RepairSound, Projectile.Center);
                AyeAyeCaptainCooldown = 100;
            }

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
            //If not hurrying up, reset the sound delay.
            else if (Projectile.soundDelay >= 10000)
                Projectile.soundDelay = NewSoundDelay;


            //Target the enemy
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
                if (!Collision.CanHitLine(Projectile.Center, 1, 1, Owner.Center, 1, 1))
                    State = BehaviorState.Idle;

                float returnSpeed = State == BehaviorState.Idle ? 15f : 6f;

                //Accelerate towards the player if too far & not fast enough
                Vector2 playerVec = Owner.Center - Projectile.Center - Vector2.UnitY * 60f;
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

                else if (buffMode)
                {
                    AyeAyeCaptainCooldown = 50f;

                    Player playerToBuff = Owner;
                    float mouseDistanceToOwner = (Owner.MountedCenter - Owner.Calamity().mouseWorld).Length();

                    if (Main.netMode == NetmodeID.MultiplayerClient && mouseDistanceToOwner > 120f)
                    {
                        for (int i = 0; i < Main.maxPlayers; i++)
                        {
                            if (Main.player[i].active && !Main.player[i].dead && (Main.player[i].team == Owner.team || Main.player[i].team == 0))
                            {
                                float mouseDistanceToPotentialTarget = (Main.player[i].MountedCenter - Owner.Calamity().mouseWorld).Length();
                                if (mouseDistanceToPotentialTarget < 120f && mouseDistanceToOwner > mouseDistanceToPotentialTarget)
                                {
                                    playerToBuff = Main.player[i];
                                    mouseDistanceToOwner = mouseDistanceToPotentialTarget;
                                }
                            }
                        }

                        if (playerToBuff != Owner && Main.rand.NextBool(4))
                        {
                            Vector2 direction = Main.rand.NextVector2CircularEdge(1f, 1f);

                            Dust wishyDust = Dust.NewDustPerfect(playerToBuff.Center + direction * Main.rand.NextFloat(4f, 9f), 229, Alpha: 100, Scale: Main.rand.NextFloat(1f, 1.4f));
                            wishyDust.noGravity = true;
                            wishyDust.noLight = true;
                            wishyDust.velocity = direction * Main.rand.NextFloat(2f, 4);

                        }
                    }

                    healedPlayer = playerToBuff;
                    float distanceToHealed = (healedPlayer.Center - Projectile.Center).Length();
                    Vector2 aimPosition = playerToBuff.MountedCenter - Vector2.UnitY.RotatedBy((float)Math.Sin(Main.GlobalTimeWrappedHourly + Projectile.whoAmI) * MathHelper.PiOver2 * 0.9f) * 60f - Vector2.UnitY * 20f;
                    float distanceToAim = (aimPosition - Projectile.Center).Length();

                    if (distanceToAim > 50)
                    {
                        float speed = MathHelper.Lerp(10f, 30f, Math.Clamp((distanceToAim - 110f) / 400f, 0f, 1f));
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, (aimPosition - Projectile.Center).SafeNormalize(Vector2.Zero) * speed, 0.05f);
                    }

                    else
                    {
                        Projectile.velocity *= 0.98f;
                        if (Projectile.velocity == Vector2.Zero)
                            Projectile.velocity = (aimPosition - Projectile.Center).SafeNormalize(Vector2.Zero) * 5f;
                    }

                    if (distanceToHealed < 200f)
                    {
                        //Heal player
                        playerToBuff.GetModPlayer<WulfrumControllerPlayer>().buffingDrones++;

                        ShootTimer--;
                        if (ShootTimer <= 0)
                        {
                            ShootTimer = ShootDelay;

                            // 1/3 chance to directly recharge the Rover Drive shield by 1 point
                            if (Main.rand.NextBool(3) && modPlayer.roverDrive && modPlayer.RoverDriveShieldDurability < RoverDrive.ShieldDurabilityMax)
                            {
                                CalamityPlayer buffedCalPlayer = playerToBuff.Calamity();
                                buffedCalPlayer.RoverDriveShieldDurability++;
                                if (buffedCalPlayer.cooldowns.TryGetValue(Cooldowns.WulfrumRoverDriveDurability.ID, out var cd))
                                    cd.timeLeft = buffedCalPlayer.RoverDriveShieldDurability;
                            }
                        }
                    }
                }

                else
                {
                    //Aim towards the top of the player
                    if (playerDist > 70f)
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
            }

            Projectile.rotation = Projectile.velocity.X * 0.05f;
            Projectile.spriteDirection = Projectile.direction = Math.Sign(Projectile.velocity.X);

            if (!buffMode)
                ChargeUpAndFire(targetCache);
        }

        public void ChargeUpAndFire(NPC targetCache)
        {
            //Decrease the timer by a random number
            ShootTimer -= Main.rand.Next(1, 4);


            if (ShootTimer <= 0)
            {
                ShootTimer = ShootDelay;
                Projectile.netUpdate = true;

                //Don't shoot if no target.
                if (targetCache == null)
                    return;

                NuzzleFlashTime = 20f;
                SoundEngine.PlaySound(PewSound, Projectile.Center);
                //Recoil
                Vector2 velocity = targetCache.Center - Projectile.Center;
                velocity.Normalize();
                velocity *= 10f;
                Projectile.velocity += velocity * -0.3f;


                if (Main.myPlayer == Projectile.owner)
                {
                    int bolt = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<WulfrumEnergyBurst>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Main.projectile[bolt].originalDamage = Projectile.originalDamage;
                    Main.projectile[bolt].netUpdate = true;
                    Projectile.netUpdate = true;
                }
            }
        }

        internal Color ColorFunction(float completionRatio)
        {
            float fadeOpacity = 0.4f + 0.4f * ((float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f + completionRatio * -12f) * 0.5f + 0.5f);

            fadeOpacity *= (1 - MathHelper.Clamp(((healedPlayer.Center - Projectile.Center).Length() - 170f) / 70f, 0f, 1f));

            return Color.CornflowerBlue.MultiplyRGB(PrimColorMult) * fadeOpacity;
        }

        internal float WidthFunction(float completionRatio)
        {
            return (3.4f + 4f * ((float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f + completionRatio * -12f) * 0.5f + 0.5f)) * 2f;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            if (healedPlayer == null)
                healedPlayer = Owner;

            if (BuffModeBuffer <= 0 && (healedPlayer.Center - Projectile.Center).Length() < 240f)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ZapTrail"));

                Vector2[] drawPos = new Vector2[] { Projectile.Center, Projectile.Center, healedPlayer.Center + (Projectile.Center - healedPlayer.Center) * 0.5f + Vector2.UnitY * 40f, healedPlayer.Center, healedPlayer.Center };

                CalamityUtils.DrawChromaticAberration(Vector2.UnitX, 1.8f, delegate (Vector2 offset, Color colorMod)
                {
                    PrimColorMult = colorMod;
                    PrimitiveSet.Prepare(drawPos, new(WidthFunction, ColorFunction, (_) => offset, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), 30);
                });

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            }
            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            if (NuzzleFlashTime > 0)
            {
                NuzzleFlashTime--;

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                float opacity = (float)Math.Pow(NuzzleFlashTime / 20f, 1.7f);

                if (SheenTex == null)
                    SheenTex = ModContent.Request<Texture2D>("CalamityMod/Particles/HalfStar");

                Texture2D shineTex = SheenTex.Value;
                Vector2 shineScale = new Vector2(1.2f, 2f - (opacity) * 1.33f);
                //float rotationBoost = opacity * MathHelper.PiOver2;

                Main.EntitySpriteDraw(shineTex, Projectile.Center - Main.screenPosition + Vector2.UnitY * 2f, null, Color.GreenYellow * opacity, MathHelper.PiOver2, shineTex.Size() / 2f, shineScale * Projectile.scale, SpriteEffects.None, 0);
                //Main.EntitySpriteDraw(shineTex, Projectile.Center - Main.screenPosition + Vector2.UnitY * 2f, null, Color.GreenYellow * opacity, rotationBoost + 0.3f, shineTex.Size() / 2f, shineScale * Projectile.scale, SpriteEffects.None, 0);


                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }

        public override bool? CanDamage() => false;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(BuffModeBuffer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            BuffModeBuffer = reader.ReadSingle();
        }
    }
}

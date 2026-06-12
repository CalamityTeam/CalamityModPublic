using System;
using System.Collections.Generic;
using System.IO;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Tools;
using CalamityMod.Particles;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.Utilities;
using Terraria.Utilities;

namespace CalamityMod.NPCs.NormalNPCs.HorribleHog
{
    public partial class HorribleHog : ModNPC
    {
        public enum BehaviorState
        {
            // Transitional behaviors.
            PiggyTransformation = -5,
            EngageAnimation = -4,
            LaughAtDeadPlayer = -3,
            DespawnAnimation = -2,
            DeathAnimation = -1,
            Idle,
            DigTowardsTarget,

            // Attacks.
            ChasePlayer,
            HogCharge,
            JumpAndDash,
            HorribleHoller,
            VomitBarrage,
        }

        private static Asset<Texture2D> HorribleHog_BalledUp;
        private static Asset<Texture2D> BloomCircle;
        private static Asset<Texture2D> ShineFlare;
        private static Asset<Texture2D> VortexTexture;
        private static Asset<Texture2D> VortexTextureSecondary;
        private static Asset<Texture2D> VortexDistortionTexture;

        private static SoundStyle HitSound = new("CalamityMod/Sounds/NPCHit/HorribleHogHit", 3);
        private static SoundStyle DeathSound = new("CalamityMod/Sounds/NPCKilled/HorribleHogDeath");
        private static SoundStyle CackleSound = new("CalamityMod/Sounds/Custom/HorribleHog/HorribleHogCackle");
        private static SoundStyle HiccupSound = new("CalamityMod/Sounds/Custom/HorribleHog/HorribleHogHiccup", 2);
        private static SoundStyle JumpSound = new("CalamityMod/Sounds/Custom/HorribleHog/HorribleHogJump", 2);
        private static SoundStyle GroundImpactSound = new("CalamityMod/Sounds/Custom/HorribleHog/HorribleHogGroundImpact", 2);
        private static SoundStyle VomitChargeUpSound = new("CalamityMod/Sounds/Custom/HorribleHog/HorribleHogVomitChargeUp", 2);
        private static SoundStyle VomitSound = new("CalamityMod/Sounds/Custom/HorribleHog/HorribleHogVomit", 2);
        private static SoundStyle IdleSound = new("CalamityMod/Sounds/Custom/HorribleHog/HorribleHogIdle", 2)
        {
            PitchVariance = 0.25f,
        };
        private static SoundStyle DiggingSlowSound = new("CalamityMod/Sounds/Custom/HorribleHog/HorribleHogDiggingSlow")
        {
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame,
            MaxInstances = 0
        };
        private static SoundStyle DiggingFastSound = new("CalamityMod/Sounds/Custom/HorribleHog/HorribleHogDiggingFast")
        {
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame,
            MaxInstances = 0
        };
        private static SoundStyle DevilsTongueLoopingSound = new("CalamityMod/Sounds/Custom/HorribleHog/HorribleHogNearbyLoop")
        {
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame,
            MaxInstances = 0
        };

        public Dictionary<BehaviorState, int> PreviousAttackCounters = [];

        public Dictionary<BehaviorState, float> AttackWeights = [];

        public bool SearchForTargetEveryFrame;

        public bool HasPlayedEngageAnimation;

        public bool HasPlayedDeathAnimation;

        public bool ReadyToPlayLaughingAnimation;

        public bool UseBalledSprite;

        public int FrameY;

        public float EyeGlintScale;

        public float AfterimageTrailOpacity;

        public float HorizontalShakeStrength;

        public float DevilsTongueVolumeMultiplier;

        public float TintStrength;

        public float SpriteRotation;

        public Vector2 LastPlayerPosition;

        public Vector2 DiggingEmergeSpot;

        public Vector2 SquashVector;

        public Vector2 SquashVectorTarget;

        public Color TintColor;

        public Color TintColorTarget;

        public SlotId DeathLaughSoundSlot;

        public SlotId DiggingSoundSlot;

        public SlotId DevilsTongueSlot;

        #region Static Behavior Properties
        public static int MaxAttacks_ChasePlayer => 2;
        public static int MaxAttacks_HogCharge => Main.expertMode ? 2 : 1;
        public static int MaxAttacks_HorribleHoller => 1;
        public static int MaxAttacks_JumpAndDash => Main.expertMode ? 3 : 2;
        public static int MaxAttacks_VomitBarrage => Main.expertMode ? 2 : 1;
        public static int MaxAttacksPerCycle => CalamityWorld.revenge ? 5 : Main.expertMode ? 4 : 3;
     
        public static int MaxTimeToStartDigging => 300;

        public static int MinFrame_Roar => 0;
        public static int MaxFrame_Roar => 9;
        public static int MinFrame_RoarFinish => 10;
        public static int MaxFrame_RoarFinish => 13;
        public static int MinFrame_Laughing => 8;
        public static int MaxFrame_Laughing => 9;
        public static int MinFrame_Walking => 15;
        public static int MaxFrame_Walking => 23;
        public static int JumpFrame => 24;
        public static int IdleFrame => 14;
        public static int BalledUpFrame => 19;
        public static int VomitFrame => 2;
        #endregion

        public ref float Timer => ref NPC.ai[0];

        public ref float AIState => ref NPC.ai[1];

        public ref float LocalAIState => ref NPC.ai[2];

        public ref float MainAttackCounter => ref NPC.ai[3];

        public ref float MiscAttackCounter => ref NPC.localAI[0];

        public ref float AltAttackVariant => ref NPC.localAI[1];

        public ref float DigTimer => ref NPC.localAI[2];

        public Vector2 VelocityBasedSquashNStretch
        {
            get
            {
                float stretch = Utils.Remap(NPC.velocity.Length(), 0f, 20f, 1f, 1.08f);
                Vector2 stretchedVector = new(1f * stretch, 1f - 1f * stretch * 0.3f);
                return stretchedVector;
            }
        }

        public bool PhaseTwo
        {
            get
            {
                if (CalamityWorld.death || Main.getGoodWorld)
                    return NPC.life <= NPC.lifeMax * 0.7f;

                if (CalamityWorld.revenge)
                    return NPC.life <= NPC.lifeMax * 0.6f;

                return Main.expertMode && NPC.life <= NPC.lifeMax * 0.5f;
            }
        }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                HorribleHog_BalledUp = ModContent.Request<Texture2D>("CalamityMod/NPCs/NormalNPCs/HorribleHog/HorribleHog_BalledUp");
                BloomCircle = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");
                ShineFlare = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/ShineFlare");
                VortexTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Neurons2");
                VortexTextureSecondary = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/EternityStreak");
                VortexDistortionTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Perlin");
            }
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25;
            NPCID.Sets.TrailCacheLength[Type] = 5;
            NPCID.Sets.TrailingMode[Type] = 0;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new();
            drawModifiers.Position.X += 12f;
            drawModifiers.PortraitPositionXOverride = 14f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = drawModifiers;
        }

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 28;
            NPC.damage = 20;
            NPC.defense = 14;
            NPC.lifeMax = 700;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 5f;
            NPC.rarity = 4;
            NPC.value = Item.buyPrice(0, 3, 0, 0);
            NPC.HitSound = HitSound;
            NPC.DeathSound = DeathSound;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.dontTakeDamage = false;

            Banner = NPC.type;
            BannerItem = ModContent.ItemType<HorribleHogBanner>();

            SquashVector = Vector2.One;
            SquashVectorTarget = Vector2.One;
            ResetAttackWeights();
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.HorribleHog")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((byte)PreviousAttackCounters.Count);
            foreach (var pair in PreviousAttackCounters)
            {
                writer.Write((byte)pair.Key);
                writer.Write((byte)pair.Value);
            }

            writer.Write((byte)AttackWeights.Count);
            foreach (var pair in AttackWeights)
            {
                writer.Write((byte)pair.Key);
                writer.Write((byte)pair.Value);
            }

            writer.WriteFlags(SearchForTargetEveryFrame, HasPlayedEngageAnimation, HasPlayedDeathAnimation, ReadyToPlayLaughingAnimation);
            writer.WritePackedWorldPosition(LastPlayerPosition);
            writer.WritePackedWorldPosition(DiggingEmergeSpot);

            for (int i = 0; i < 3; i++)
                writer.Write(NPC.localAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            int attackCountersLength = reader.ReadByte();
            for (int i = 0; i < attackCountersLength; i++)
                PreviousAttackCounters[(BehaviorState)reader.ReadByte()] = (int)reader.ReadByte();

            int attackWeightsLength = reader.ReadByte();
            for (int i = 0; i < attackWeightsLength; i++)
                AttackWeights[(BehaviorState)reader.ReadByte()] = (float)reader.ReadByte();

            reader.ReadFlags(out SearchForTargetEveryFrame, out HasPlayedEngageAnimation, out HasPlayedDeathAnimation, out ReadyToPlayLaughingAnimation);
            LastPlayerPosition = reader.ReadPackedWorldPosition();
            DiggingEmergeSpot = reader.ReadPackedWorldPosition();

            for (int i = 0; i < 3; i++)
                NPC.localAI[i] = reader.ReadSingle();
        }

        public override bool CheckDead()
        {
            if (!HasPlayedDeathAnimation)
            {
                NPC.velocity.X = 6f * -NPC.oldVelocity.X.DirectionalSign();
                NPC.velocity.Y = -10f;
                NPC.life = NPC.lifeMax;
                HasPlayedDeathAnimation = true;
                SwitchBehavior(specificAttack: BehaviorState.DeathAnimation);
                return false;
            }

            return true;
        }

        public override bool? CanFallThroughPlatforms()
        {
            // If Hog is not in any of the transitional behavior states, don't fall through platforms.
            bool inTransitionalBehaviorState = AIState <= (int)BehaviorState.Idle;
            if (inTransitionalBehaviorState)
                return false;

            if (NPC.HasValidTarget)
            {
                Player target = Main.player[NPC.target];
                return target.Center.Y - 8 > NPC.Center.Y;
            }

            return base.CanFallThroughPlatforms();
        }

        public override void AI()
        {
            if (!NPC.HasValidTarget || SearchForTargetEveryFrame)
                NPC.TargetClosest(false);

            if (NPC.direction == 0)
                NPC.direction = Main.rand.NextBool().ToDirectionInt();

            // Due to how much Death Mode increases Blood Moon spawn rates, this is kinda required in order for the fight to not be super overwhelming.
            if (CalamityWorld.death)
                NPC.npcSlots = 40f;

            Player target = Main.player[NPC.target];
            NPC.damage = NPC.defDamage;
            NPC.defense = NPC.defDefense;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.dontTakeDamage = false;
            NPC.chaseable = true;
            NPC.spriteDirection = NPC.direction;

            // Despawn immediately if it's morning.
            if (Main.dayTime && AIState != (int)BehaviorState.DespawnAnimation && AIState != (int)BehaviorState.DeathAnimation)
                SwitchBehavior(specificAttack: BehaviorState.DespawnAnimation);

            // If the current target dies, laugh at them.
            if (target.dead && ReadyToPlayLaughingAnimation && NPC.velocity.Y == 0f)
            {
                ReadyToPlayLaughingAnimation = false;
                SwitchBehavior(specificAttack: BehaviorState.LaughAtDeadPlayer);
            }

            // If the target is unable to be reached, count up to 5 seconds before switching behavior states and digging towards them.
            int tileRange = 64;
            Vector2 npcCanHitPosition = NPC.position - new Vector2(tileRange, tileRange);
            Vector2 targetCanHitPosition = target.position - new Vector2(tileRange, tileRange);
            bool currentlyDoingAnAttack = AIState > (int)BehaviorState.DigTowardsTarget;
            bool targetTooHigh = (NPC.Center.Y - target.Center.Y >= 160f && target.velocity.Y == 0f) || NPC.Center.Y - target.Center.Y >= 320f;
            bool targetTooFar = NPC.Distance(target.Center) >= 1280f;
            bool cantHitTarget = !Collision.CanHit(npcCanHitPosition, NPC.width + tileRange, NPC.height + tileRange, targetCanHitPosition, target.width + tileRange, target.height + tileRange);
            if (NPC.HasValidTarget && currentlyDoingAnAttack && (targetTooHigh || targetTooFar || cantHitTarget))
            {
                DigTimer++;
                if (DigTimer >= 300f && NPC.velocity.Y == 0f)
                    SwitchBehavior(specificAttack: BehaviorState.DigTowardsTarget);
            }
            else
            {
                if (DigTimer > 0f)
                    DigTimer--;
            }

            // Adjust volume correctly depending on the behavior state and play the nearby loop sound.
            float volumeTarget = AIState == (int)BehaviorState.Idle ? 1f : 0f;
            DevilsTongueVolumeMultiplier = MathHelper.Lerp(DevilsTongueVolumeMultiplier, volumeTarget, 0.075f);
            PlayNearbyLoopingSound();

            switch ((BehaviorState)AIState)
            {
                case BehaviorState.PiggyTransformation:
                    MainBehavior_PiggyTransformation();
                    break;

                case BehaviorState.EngageAnimation:
                    MainBehavior_EngageAnimation(target);
                    break;

                case BehaviorState.LaughAtDeadPlayer:
                    MainBehavior_LaughAtDeadPlayer();
                    break;

                case BehaviorState.DespawnAnimation:
                    MainBehavior_DespawnAnimation();
                    break;

                case BehaviorState.DeathAnimation:
                    MainBehavior_DeathAnimation();
                    break;

                case BehaviorState.Idle:
                    MainBehavior_Idle(target);
                    break;

                case BehaviorState.DigTowardsTarget:
                    MainBehavior_DigTowardsTarget(target);
                    break;

                case BehaviorState.ChasePlayer:
                    MainBehavior_ChasePlayer(target);
                    break;

                case BehaviorState.HogCharge:
                    MainBehavior_HogCharge(target);
                    break;

                case BehaviorState.JumpAndDash:
                    MainBehavior_JumpAndDash(target);
                    break;

                case BehaviorState.HorribleHoller:
                    MainBehavior_HorribleHoller(target);
                    break;

                case BehaviorState.VomitBarrage:
                    MainBehavior_VomitBarrage(target);
                    break;
            }

            SquashVector = Vector2.Lerp(SquashVector, SquashVectorTarget, 0.125f);
            EyeGlintScale = MathHelper.Lerp(EyeGlintScale, 0f, 0.125f);
            TintColor = Color.Lerp(TintColor, TintColorTarget, 0.125f);
            TintStrength = MathHelper.Lerp(TintStrength, 0, 0.125f);
            NPC.StepUpBlocks();
            Timer++;
        }

        public void SwitchBehavior(BehaviorState? attackToRecord = null, BehaviorState? specificAttack = null, params BehaviorState[] attacksToChooseFrom)
        {
            // Reset all the previous attack counters and weights in order to start a new cycle once the maximum amount of attacks overall has been reached.
            if (MainAttackCounter >= MaxAttacksPerCycle)
            {
                // Also search for the nearest target again once the attack cycle resets.
                NPC.TargetClosest(false);

                foreach (BehaviorState attack in PreviousAttackCounters.Keys)
                    PreviousAttackCounters[attack] = 0;
                ResetAttackWeights();
                MainAttackCounter = 0f;
            }

            if (attackToRecord.HasValue)
            {
                // Record the last attack to a dictionary and increment how many times it has been performed.
                if (!PreviousAttackCounters.ContainsKey(attackToRecord.Value))
                    PreviousAttackCounters[attackToRecord.Value] = 0;
                PreviousAttackCounters[attackToRecord.Value]++;

                // Increase the weight of other attacks and lower this one.
                AttackWeights[attackToRecord.Value] -= 0.33f;
                foreach (BehaviorState attack in AttackWeights.Keys)
                    AttackWeights[attack] += 0.33f;

                MainAttackCounter++;
            }

            // Default to returning to idling in the event there are no nearby targets after performing an attack.
            BehaviorState nextAttack = BehaviorState.Idle;

            // Switch to a specific behavior state if one is specified.
            // Otherwise, pick from the random attack array.
            if (specificAttack.HasValue)
            {
                nextAttack = specificAttack.Value;
            }
            else if (NPC.HasValidTarget && attacksToChooseFrom.Length > 0)
            { 
                WeightedRandom<BehaviorState> possibleAttacks = new();
                for (int i = 0; i < attacksToChooseFrom.Length; i++)
                {
                    // Don't add attacks to the attack pool if they've been performed their maximum amount of times.
                    if (PreviousAttackCounters.TryGetValue(attacksToChooseFrom[i], out var timesPerformed) && timesPerformed >= GetMaxAttackValue(attacksToChooseFrom[i]))
                        continue;

                    possibleAttacks.Add(attacksToChooseFrom[i], AttackWeights[attacksToChooseFrom[i]]);
                }

                // Pick a random attack.
                nextAttack = possibleAttacks;
            }

            // Reset so this once Hog has another player to target so it doesn't loop. 
            if (NPC.HasValidTarget)
                ReadyToPlayLaughingAnimation = true;

            // Switch and reset certain fields.
            AIState = (int)nextAttack;
            LocalAIState = 0f;
            Timer = 0f;
            MiscAttackCounter = 0f;
            AltAttackVariant = 0f;
            SpriteRotation = 0f;
            AfterimageTrailOpacity = 0f;
            SearchForTargetEveryFrame = false;
            UseBalledSprite = false;

            SetSquashVectors();
            KillChargeHitboxProjectile();

            NPC.netUpdate = true;
        }

        public void TransformIntoPiggy() => SwitchBehavior(specificAttack: BehaviorState.PiggyTransformation);

        public bool TryTransformingIntoPiggy()
        {
            if (AIState == (int)BehaviorState.PiggyTransformation || AIState == (int)BehaviorState.DespawnAnimation || AIState == (int)BehaviorState.DeathAnimation)
                return false;
            return true;
        }

        private int GetMaxAttackValue(BehaviorState attack)
        {
            int maxValue = attack switch
            {
                BehaviorState.ChasePlayer => MaxAttacks_ChasePlayer,
                BehaviorState.HogCharge => MaxAttacks_HogCharge,
                BehaviorState.JumpAndDash => MaxAttacks_JumpAndDash,
                BehaviorState.HorribleHoller => MaxAttacks_HorribleHoller,
                BehaviorState.VomitBarrage => MaxAttacks_VomitBarrage,
                _ => 1
            };

            // Every attack can be used one more time in phase two.
            if (PhaseTwo)
                maxValue += 1;

            return maxValue;
        }

        private void ResetAttackWeights()
        {
            AttackWeights[BehaviorState.ChasePlayer] = 1f;
            AttackWeights[BehaviorState.HogCharge] = 0.33f;
            AttackWeights[BehaviorState.JumpAndDash] = 1f;
            AttackWeights[BehaviorState.HorribleHoller] = 0.33f;
            AttackWeights[BehaviorState.VomitBarrage] = 0.66f;
        }

        private void GroundedMovement(Vector2 targetPosition, float maxSpeed, float maxAcceleration, float jumpHeight = 10f, float? slowdownDistance = null)
        {
            float distanceToPlayer = MathF.Abs(NPC.Center.X - targetPosition.X);
            if (targetPosition.X > NPC.Center.X)
            {
                if (slowdownDistance.HasValue)
                {
                    if (distanceToPlayer > slowdownDistance.Value)
                        NPC.velocity.X += maxAcceleration;
                    else
                        NPC.velocity.X -= maxAcceleration;
                }
                else
                {
                    NPC.velocity.X += maxAcceleration;
                }

            }
            else if (targetPosition.X < NPC.Center.X)
            {
                if (slowdownDistance.HasValue)
                {
                    if (distanceToPlayer > slowdownDistance.Value)
                        NPC.velocity.X -= maxAcceleration;
                    else
                        NPC.velocity.X += maxAcceleration;
                }
                else
                {
                    NPC.velocity.X -= maxAcceleration;
                }
            }

            if (NPC.velocity.Y == 0f)
            {
                if (NPC.collideX || IsNPCApproachingHole())
                    NPC.velocity.Y -= jumpHeight;
            }

            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -maxSpeed, maxSpeed);
        }

        private bool IsNPCApproachingHole()
        {
            int npcWidthInTiles = NPC.width / 16;
            int tileX = (int)(NPC.Center.X / 16f) - npcWidthInTiles;
            if (NPC.velocity.X > 0)
                tileX += npcWidthInTiles;

            int tileY = (int)((NPC.position.Y + NPC.height) / 16f);
            for (int y = tileY; y < tileY + 2; y++)
            {
                for (int x = tileX; x < tileX + npcWidthInTiles; x++)
                {
                    if (Main.tile[x, y].HasTile)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private Point FindSuitableGround(Point basePoint)
        {
            if (WorldGen.InWorld(basePoint.X, basePoint.Y))
            {
                // Tile is solid. Check to ensure the tile above is also isn't solid and move up if it is.
                if (WorldGen.ActiveAndWalkableTile(basePoint.X, basePoint.Y))
                {
                    while (WorldGen.ActiveAndWalkableTile(basePoint.X, basePoint.Y - 1) && basePoint.Y >= 1)
                        basePoint.Y--;
                }
                // Tile isn't solid. Check to ensure the tile under it is solid and move down if it isn't.
                else
                {
                    while (!WorldGen.ActiveAndWalkableTile(basePoint.X, basePoint.Y + 1) && basePoint.Y < Main.maxTilesY)
                        basePoint.Y++;
                }
            }

            return basePoint;
        }

        private void PlayNearbyLoopingSound()
        {
            if (DevilsTongueVolumeMultiplier < 0.05f)
            {
                if (SoundEngine.TryGetActiveSound(DevilsTongueSlot, out ActiveSound sound))
                    sound.Stop();
                return;
            }

            // "Devil's Tongue" looping sound.
            // Similar to Divine Swine; gets louder and lowers music volume based on proximity.
            if (!SoundEngine.TryGetActiveSound(DevilsTongueSlot, out _))
                DevilsTongueSlot = SoundEngine.PlaySound(DevilsTongueLoopingSound, NPC.Center, DevilsTongueLoopCallback);

            float idealVolume = Utils.Remap(NPC.Distance(Main.LocalPlayer.Center), 800f, 400f, 1f, 0.05f, true);
            Main.musicFade[Main.curMusic] = MathHelper.Lerp(1f, idealVolume, DevilsTongueVolumeMultiplier);
        }

        private bool DevilsTongueLoopCallback(ActiveSound soundInstance)
        {
            soundInstance.Position = NPC.Center;
            float idealVolume = Utils.Remap(NPC.Distance(Main.LocalPlayer.Center), 800f, 400f, 0.1f, 0.7f, true) * DevilsTongueVolumeMultiplier;
            soundInstance.Volume = idealVolume;
            return NPC.active && DevilsTongueVolumeMultiplier >= 0.05f;
        }

        private void DoJumpEffects(int dustCloudMin = 10, int dustCloudMax = 14, int dirtDustMin = 14, int dirtDustMax = 18)
        {
            // Do fart in a jar visuals when Hog does a jump mid-air.
            float tileCollisionDistance = CalamityUtils.DistanceToTileCollisionHit(NPC.Bottom, Vector2.UnitY, 10) ?? 9999f;
            if (tileCollisionDistance > 18f)
            {
                int fartCloudAmt = Main.rand.Next(dustCloudMin, dustCloudMax + 1);
                for (int i = 0; i < fartCloudAmt; i++)
                {
                    Vector2 spawnPosition = NPC.Bottom + Main.rand.NextVector2Circular(32f, 0f);
                    Vector2 velocity = Main.rand.NextVector2Circular(2f, 2f);
                    int goreType = Main.rand.Next(GoreID.FartCloud1, GoreID.FartCloud3 + 1);
                    Gore.NewGore(spawnPosition, velocity, goreType);
                }

                int fartDustAmt = Main.rand.Next(dirtDustMin, dirtDustMax + 1);
                for (int i = 0; i < fartDustAmt; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(2f, 2f);
                    Dust.NewDust(NPC.Bottom, 0, 0, DustID.FartInAJar, velocity.X, velocity.Y, Scale: Main.rand.NextFloat(0.8f, 1.2f));
                }

                SoundEngine.PlaySound(SoundID.Item16, NPC.Center);
            }
            else
            {
                int dustCloudAmt = Main.rand.Next(dustCloudMin, dustCloudMax + 1);
                for (int i = 0; i < dustCloudAmt; i++)
                {
                    Vector2 spawnPosition = FindSuitableGround((NPC.Bottom + Main.rand.NextVector2Circular(32f, 0f)).ToTileCoordinates()).ToWorldCoordinates();
                    Vector2 velocity = NPC.velocity * (Main.rand.NextFloat(0.1f, 0.2f) + i * 0.1f);
                    Color color = Color.Lerp(Color.SaddleBrown, Color.SandyBrown, Main.rand.NextFloat());
                    float rotationSpeed = Main.rand.NextFloat(0.01f, 0.03f) * Main.rand.NextBool().ToDirectionInt();

                    TimedSmokeParticle launchCloud = new(spawnPosition, velocity, color, color, Main.rand.NextFloat(0.4f, 0.6f), Main.rand.NextFloat(0.6f, 0.8f), Main.rand.Next(30, 45), rotationSpeed);
                    GeneralParticleHandler.SpawnParticle(launchCloud, true, Enums.GeneralDrawLayer.BeforeSolidTiles);
                }

                int dustAmt = Main.rand.Next(dirtDustMin, dirtDustMax + 1);
                for (int i = 0; i < dustAmt; i++)
                {
                    Vector2 velocity = NPC.velocity * (Main.rand.NextFloat(0.1f, 0.2f) + i * 0.025f);
                    Dust.NewDust(NPC.Bottom, 0, 0, DustID.Dirt, velocity.X, velocity.Y);
                }

                SoundEngine.PlaySound(JumpSound, NPC.Bottom);
            }
        }

        private void TrySpawningMist(int x, int y)
        {
            if (!WorldGen.InWorld(x, y, 10) || Main.tile[x, y] == null)
                return;

            Tile tile = Main.tile[x, y];
            if (!tile.HasTile || tile.Slope > 0 || tile.IsHalfBlock || !Main.tileSolid[Main.tile[x, y].TileType])
                return;

            tile = Main.tile[x, y - 1];
            if (tile.LiquidAmount > 0 && tile.Slope <= 0)
            {
                for (int i = tile.Y() - 1; i > tile.Y() - 50; i--)
                {
                    Tile liquidTile = Main.tile[x, i];
                    Tile liquidTileUnderOne = Main.tile[x, i + 1];
                    Tile liquidTileUnderTwo = Main.tile[x, i + 2];
                    if (liquidTile.LiquidAmount == 0 && !WorldGen.SolidTile(liquidTile) && !WorldGen.SolidTile(liquidTileUnderOne) && !WorldGen.SolidTile(liquidTileUnderTwo))
                    {
                        if (Main.rand.NextBool(240))
                        {
                            SpawnFloorMist(x, i + 1);
                            if (Main.rand.NextBool(3))
                                SpawnFloorMist(x, i + 2);
                        }
                        break;
                    }
                }
            }
            else
            {
                if (!WorldGen.SolidTile(tile) && Main.rand.NextBool(240))
                {
                    SpawnFloorMist(x, y + 1);
                    if (Main.rand.NextBool(3))
                        SpawnFloorMist(x, y);
                }
            }
            
        }

        private Rectangle GetTileWorkSpaceForMist()
        {
            Point point = NPC.Center.ToTileCoordinates();
            int width = 30;
            int height = 10;
            return new Rectangle(point.X - width / 2, point.Y - height / 2, width, height);
        }

        private void SpawnFloorMist(int x, int y)
        {
            int textureIndex = Main.rand.Next(GoreID.AmbientFloorCloud1, GoreID.AmbientFloorCloud4 + 1);

            Vector2 position = new Point(x, y - 1).ToWorldCoordinates();
            Vector2 velocity = Vector2.UnitX * Main.rand.NextFloat(0.2f, 0.4f) * Main.WindForVisuals;
            Color mistColor = Color.Lerp(new(30, 30, 30), Color.Crimson, Main.rand.NextBool(3) ? Main.rand.NextFloat(0.2f, 0.8f) : Main.rand.Next(2));
            Color color = Color.Lerp(Lighting.GetColor(new Point(x, y)), mistColor, 0.5f);

            float yOffset = 16f * Main.rand.NextFloat();
            position.Y -= yOffset;
            if (yOffset > 4f)
                textureIndex = GoreID.AmbientFloorCloud4;

            float scale = Main.rand.NextFloat(0.8f, 1.6f) + Main.rand.NextFloat() * 0.2f;
            float baseOpacity = Main.rand.NextFloat(0.8f, 1.2f) * Utils.Remap(Main.LocalPlayer.Distance(NPC.Center), 1200f, 600f, 0.3f, 1f, true);
            int lifetime = Main.rand.Next(480, 720);

            GraveyardMistParticle floorMist = new(position, velocity, color, scale, baseOpacity, lifetime, textureIndex);
            GeneralParticleHandler.SpawnParticle(floorMist);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (Main.bloodMoon && NPC.downedBoss1 && NPC.CountNPCS(Type) < 1)
            {
                float spawnChanceMultiplier = CalamityWorld.death ? 0.0075f : 0.025f;
                return SpawnCondition.OverworldNightMonster.Chance * spawnChanceMultiplier;
            }
            return 0f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            // Death effects spawned in MainBehavior_DeathAnimation.
            if (NPC.life <= 0)
                return;

            int dustAmt = Main.rand.Next(3, 7);
            if (hit.Crit)
                dustAmt += Main.rand.Next(2, 5);

            for (int i = 0; i < dustAmt; i++)
            {
                int dustType = Utils.SelectRandom(Main.rand, DustID.ToxicBubble, DustID.GreenBlood, DustID.Blood);
                Vector2 velocity = new Vector2(hit.HitDirection * Main.rand.NextFloat(1f, 2f), Main.rand.NextFloat(-2f, 2f));
                float scale = Main.rand.NextFloat(0.8f, 1.2f);
                if (hit.Crit)
                {
                    velocity *= Main.rand.NextFloat(1.25f, 1.75f);
                    scale += Main.rand.NextFloat(0.25f, 0.5f);
                }

                Dust.NewDust(NPC.position, NPC.width, NPC.height, dustType, velocity.X, velocity.Y, Scale: scale);
            }
        }

        public override void OnKill()
        {
            // Mark Horrible Hog as dead
            DownedBossSystem.downedHorribleHog = true;
            CalamityNetcode.SyncWorld();
        }  
        
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //Always drops Disgusting Meat, 50% chance to drop Money Trough, 25%/33% chance to drop Laudanum, 10% chance to drop Bloody Tear
            npcLoot.Add(ModContent.ItemType<DisgustingMeat>());
            npcLoot.Add(ModContent.ItemType<Laudanum>(), Main.expertMode ? 3 : 4);
            npcLoot.Add(ItemID.MoneyTrough, 2);
            npcLoot.Add(ItemID.BloodMoonStarter, 10);

            // 10-12 Blood Orbs Post-EoC to match Wandering Eye Fish and Zombie Merman as Blood Moon's faux minibosses.
            LeadingConditionRule postEoC = npcLoot.DefineConditionalDropSet(DropHelper.PostEoC());
            postEoC.Add(ModContent.ItemType<BloodOrb>(), 1, 10, 12);
        }
    }
}

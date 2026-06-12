using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CalamityMod.Balancing;
using CalamityMod.Balancing;
using CalamityMod.CalPlayer;
using CalamityMod.DataStructures;
using CalamityMod.Enums;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor;
using CalamityMod.NPCs;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Packets;
using CalamityMod.Systems.Collections;
using CalamityMod.Systems.Collections;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod
{
    #region Calamity Targeting Parameters Struct
    public struct CalamityTargetingParameters : IEquatable<CalamityTargetingParameters>
    {
        // Vanilla argument to TargetClosest. Defaults to true as it does in vanilla.
        // If true, the NPC will turn to face the target.
        public bool faceTarget = true;

        // Vanilla argument to TargetClosestUpgraded. That function is never used, but the flexibility is implemented here.
        // Allows targeting distance calculations to be measured from a different position than the NPC's center, if desired.
        public Vector2? targetingCenter = null;

        // Hard maximum range to search for targets. Players outside this physical Manhattan distance will always be ignored.
        //
        // Vanilla always uses infinity for this, leading to behavior like Queen Bee crossing the world to spawncamp you.
        // Calamity defaults to a very high but not infinite value.
        public float maxSearchRange = 9600f; // 600 tiles

        // Targeting preference enum.
        // Anyone = Target the "closest" player, no other considerations. Vanilla behavior.
        // PreferSame = Always pick the same player if they're within the search range, even if another player is closer or has more aggro.
        // ForceSwitch = Try to pick any other player but the current player, if possible. Similar to an "aggro drop" in MMOs.
        //
        // ForceSwitch intentionally does nothing in single player, because there is nobody to switch to.
        public NPCTargetType targetType = NPCTargetType.Anyone;

        // The ratio at which to consider aggro bonuses from player gear.
        // 1f is vanilla. Set to 0f to ignore aggro bonuses entirely.
        // Set to a negative value to make the NPC intentionally avoid tanks and preferentially go after other players.
        public float aggroRatio = 1f;

        // Whether or not players must have line of sight to the NPC to be considered valid targets.
        // This is always line of sight to the NPC itself, even if a different targeting center for range finding is specified.
        public bool requireLineOfSight = false;

        // If true, the targeting algorithm counts missing health as a gigantic boost to aggro.
        // This makes for a "merciless" or "bloodthirsty" NPC which is focused on killing the lowest health players.
        public bool finishThemOff = false;
        internal const float FinishThemOff_MaxAggroBoost = 4000f;

        // If true, this NPC ignores the Stardust armor set bonus "JoJo Tank Minion" (or "Algalon the Observer" according to the wiki).
        //
        // This is set to false by default, because that's vanilla behavior.
        // As Stardust armor is postgame in vanilla, no vanilla bosses ignore the Stardust Guardian.
        // It is highly recommended to set this to true for all bosses, or their aggro can be abusively manipulated.
        public bool ignoreTankMinions = false;

        // If true, this NPC ignores players who have less than zero net aggro and are not actively using items.
        //
        // This is set to true by default, because it's (undocumented) vanilla behavior.
        // Bosses will automatically attack stealthed players anyway -- you don't need to set this to false for that to occur.
        public bool ignoreStealthedPlayers = true;

        // If true, this targeting change forces a net update.
        // In vanilla, targeting updates cause net updates if direction changed or the target player changed,
        // but NEVER if the NPC has collideX or collideY set to true.
        //
        // Generally this doesn't need to be set to true, as bosses will never have collideX or collideY set to true.
        public bool forceNetUpdate = false;

        // Player indexes put into this list will not be considered for targetting.
        public HashSet<int> excludedPlayers = [];

        public CalamityTargetingParameters()
        {
        }

        // Quick defaults for recommended boss settings.
        public CalamityTargetingParameters(bool isBoss) => ignoreTankMinions = isBoss;

        // Using the default keyword on structs produces garbage. Please use the below instead, or define your own parameters.
        public static CalamityTargetingParameters Defaults => new();
        public static CalamityTargetingParameters BossDefaults => new(true);

        #region Equality Operators
        public static bool operator ==(CalamityTargetingParameters ctp1, CalamityTargetingParameters ctp2)
        {
            bool targetingCentersEqual = (ctp1.targetingCenter is null && ctp2.targetingCenter is null) || ctp1.targetingCenter == ctp2.targetingCenter;
            if (!targetingCentersEqual)
                return false;

            return ctp1.faceTarget == ctp2.faceTarget &&
                ctp1.maxSearchRange == ctp2.maxSearchRange &&
                ctp1.targetType == ctp2.targetType &&
                ctp1.aggroRatio == ctp2.aggroRatio &&
                ctp1.requireLineOfSight == ctp2.requireLineOfSight &&
                ctp1.finishThemOff == ctp2.finishThemOff &&
                ctp1.ignoreTankMinions == ctp2.ignoreTankMinions &&
                ctp1.ignoreStealthedPlayers == ctp2.ignoreStealthedPlayers &&
                ctp1.forceNetUpdate == ctp2.forceNetUpdate;
        }

        public static bool operator !=(CalamityTargetingParameters ctp1, CalamityTargetingParameters ctp2) => !(ctp1 == ctp2);

        public readonly bool Equals(CalamityTargetingParameters other) => this == other;

        public override readonly bool Equals([NotNullWhen(true)] object obj)
        {
            if (obj is not CalamityTargetingParameters)
                return false;

            return this == (CalamityTargetingParameters)obj;
        }

        // Visual Studio complains if this is not here. I do not know why.
        public override readonly int GetHashCode() => base.GetHashCode();
        #endregion
    }
    #endregion


    public static partial class CalamityUtils
    {
        public static T ModNPC<T>(this NPC npc) where T : ModNPC => npc.ModNPC as T;

        #region NPC Counting
        /// <summary>
        /// Efficiently counts the amount of existing enemies. May be used for multiple enemies.
        /// </summary>
        /// <param name="typesToCheck"></param>
        /// <returns></returns>
        public static int CountNPCsBetter(params int[] typesToCheck)
        {
            // Don't waste time if the type check list is empty for some reason.
            if (typesToCheck.Length <= 0)
                return 0;

            int count = 0;
            foreach (NPC n in Main.ActiveNPCs)
            {
                if (!typesToCheck.Contains(n.type))
                    continue;

                count++;
            }

            return count;
        }

        public static bool AnyBossNPCS(bool checkForMechs = false)
        {
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.IsABoss())
                {
                    // Added due to the new mech boss ore progression, return true if any mech is alive and checkForMechs is true, reduces mech boss projectile damage if true.
                    if (checkForMechs)
                        return npc.type == NPCID.TheDestroyer || npc.type == NPCID.SkeletronPrime || npc.type == NPCID.Spazmatism || npc.type == NPCID.Retinazer;
                    return true;
                }
            }
            return false;
        }
        #endregion

        /// <summary>
        /// Hides an NPC from the bestiary. This should be called in SetStaticDefaults.
        /// </summary>
        /// <param name="n"></param>
        public static void HideFromBestiary(this ModNPC n)
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(n.Type, value);
        }

        #region Stat Setting
        /// <summary>
        /// Allows you to set the lifeMax value of a NPC to different values based on the mode. Called instead of npc.lifeMax = X.
        /// </summary>
        /// <param name="npc">The NPC whose lifeMax value you are trying to set.</param>
        /// <param name="normal">The value lifeMax will be set to in normal mode, this value gets doubled automatically in Expert mode.</param>
        /// <param name="revengeance">The value lifeMax will be set to in Revegeneance mode.</param>
        /// <param name="bossRush">The value lifeMax will be set to during the Boss Rush.</param>
        public static void LifeMaxNERB(this NPC npc, int normal, int? revengeance = null, int? bossRush = null)
        {
            npc.lifeMax = normal;

            if (bossRush.HasValue && BossRushEvent.BossRushActive)
            {
                npc.lifeMax = bossRush.Value;
            }
            else if (revengeance.HasValue && CalamityWorld.revenge)
            {
                npc.lifeMax = revengeance.Value;
            }
        }

        /// <summary>
        /// Allows you to set the DR value of a NPC to different values based on the mode.
        /// </summary>
        /// <param name="npc">The NPC whose DR value you are trying to set.</param>
        /// <param name="normal">The value DR will be set to in normal mode.</param>
        /// <param name="revengeance">The value DR will be set to in Revegeneance mode.</param>
        /// <param name="bossRush">The value DR will be set to during the Boss Rush.</param>
        public static void DR_NERD(this NPC npc, float normal, float? revengeance = null, float? death = null, float? bossRush = null)
        {
            npc.Calamity().DR = normal;

            if (bossRush.HasValue && BossRushEvent.BossRushActive)
            {
                npc.Calamity().DR = bossRush.Value;
            }
            else if (revengeance.HasValue && CalamityWorld.revenge)
            {
                npc.Calamity().DR = CalamityWorld.death ? death.Value : revengeance.Value;
            }
        }
        #endregion

        #region NPC Threat Classification
        // This function controls the behavior of Proximity Rage.
        //
        // TODO -- In multiplayer, with more than one player, all enemies are listed as statue spawned.
        // This sounds like packet corruption or something, but it's impossible to know.
        // Even stranger, this bug only affects players who aren't player slot 0.
        // As such, statue enemies are currently allowed by default for Proximity Rage.
        // This is not the intent. Ideally, they would not count.
        //
        // TODO -- Use this function EVERYWHERE that target validity is checked, not just for Proximity Rage.
        // The easiest way to find locations this should be used is checks for whether something is statue spawned.
        public static bool IsAnEnemy(this NPC npc, bool allowStatues = true, bool checkDead = true, bool checkDamage = true)
        {
            // Null, inactive, town NPCs, and friendlies are right out.
            if (npc is null || (!npc.active && (!checkDead || npc.life > 0)) || npc.townNPC || npc.friendly)
                return false;

            // Unless allowed, statue spawns don't count for rage.
            if (!allowStatues && npc.SpawnedFromStatue)
                return false;

            // "Non-enemies" (e.g. butterflies or projectile enemies) with near zero max health,
            // or anything but the strongest enemies with no contact damage (e.g. Celestial Pillars, Providence)
            // do not generate rage.
            if (npc.lifeMax <= BalancingConstants.TinyHealthThreshold || ((npc.defDamage <= BalancingConstants.TinyDamageThreshold && checkDamage) && npc.lifeMax <= BalancingConstants.NoContactDamageHealthThreshold))
                return false;

            // Exclude NPCs that specified to not be counted as enemy
            // This includes: TargetDummy, SuperDummy by Default
            if (CalamityNPCSets.DontCountAsEnemy[npc.type])
                return false;

            // Anything else is considered a valid enemy target.
            return true;
        }

        // This function follows the behavior of Adrenaline.
        // Vanilla worm segments and Slime God slimes are specifically included.
        // Martian Saucers are specifically excluded.
        public static bool IsABoss(this NPC npc)
        {
            if (npc is null || !npc.active)
                return false;
            if (npc.boss && npc.type != NPCID.MartianSaucerCore)
                return true;
            if (npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsTail)
                return true;
            return npc.type == NPCType<EbonianPaladin>() || npc.type == NPCType<CrimulanPaladin>() ||
                npc.type == NPCType<SplitEbonianPaladin>() || npc.type == NPCType<SplitCrimulanPaladin>();
        }
        #endregion

        #region Net Synchronization
        /// <summary>
        /// Syncs <see cref="CalamityGlobalNPC.newAI"/>. This exists specifically for AIs manipulated in a global context, as <see cref="GlobalNPC"/> has no netUpdate related hooks.
        /// </summary>
        /// <param name="npc"></param>
        public static void SyncExtraAI(this NPC npc)
        {
            // Don't bother attempting to send packets in singleplayer.
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            SyncCalamityNPCAIArrayPacket.Send(npc);
        }

        /// <summary>
        /// Syncs <see cref="NPC.localAI"/>. This exists specifically for AIs manipulated in a global context, as <see cref="GlobalNPC"/> has no netUpdate related hooks.
        /// </summary>
        /// <param name="npc"></param>
        public static void SyncVanillaLocalAI(this NPC npc)
        {
            // Don't bother attempting to send packets in singleplayer.
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            SyncVanillaNPCLocalAIArrayPacket.Send(npc);
        }

        /// <summary>
        /// Helper method which both sets <see cref="NPC.netUpdate"/> and resets <see cref="NPC.netSpam"/> to 0 to prevent the net update from being blocked.
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="ignoreCurrentNetSpam">If set to false, the method will only reset <see cref="NPC.netSpam"/> if it is greater than or equal to 10.</param>
        public static void ForceNetUpdate(this NPC npc, bool ignoreCurrentNetSpam = true)
        {
            npc.netUpdate = true;
            if (npc.netSpam >= 10 || ignoreCurrentNetSpam)
                npc.netSpam = 0;
        }

        /// <summary>
        /// Syncs position and velocity from a client to the server. This is to be used in contexts where these things are reliant on client-side information, such as <see cref="Main.MouseWorld"/>.
        /// </summary>
        /// <param name="npc"></param>
        public static void SyncMotionToServer(this NPC npc)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                return;

            SyncNPCMotionDataToServerPacket.Send(npc);
        }

        public static void SyncNPCPosAndRotOnly(this NPC npc)
        {
            SyncNPCPosAndRotOnlyPacket.Send(npc);
        }
        #endregion

        #region Smooth Movement
        /// <summary>
        /// Smoother movement for NPCs
        /// </summary>
        /// <param name="npc">The NPC getting the movement change.</param>
        /// <param name="movementDistanceGateValue">The distance where the NPC should stop moving once it's close enough to its destination.</param>
        /// <param name="distanceFromDestination">How far the NPC is from its destination.</param>
        /// <param name="baseVelocity">How quickly the NPC moves towards its destination.</param>
        /// <param name="useSimpleFlyMovement">Whether the NPC should use SimpleFlyMovement to make the movement more affected by acceleration.</param>
        public static void SmoothMovement(NPC npc, float movementDistanceGateValue, Vector2 distanceFromDestination, float baseVelocity, float acceleration, bool useSimpleFlyMovement)
        {
            // Inverse lerp returns the percentage of progress between A and B
            float lerpValue = Utils.GetLerpValue(movementDistanceGateValue, 2400f, distanceFromDestination.Length(), true);

            // Min velocity
            float minVelocity = distanceFromDestination.Length();
            float minVelocityCap = baseVelocity;
            if (minVelocity > minVelocityCap)
                minVelocity = minVelocityCap;

            // Max velocity
            Vector2 maxVelocity = distanceFromDestination / 24f;
            float maxVelocityCap = minVelocityCap * 3f;
            if (maxVelocity.Length() > maxVelocityCap)
                maxVelocity = distanceFromDestination.SafeNormalize(Vector2.Zero) * maxVelocityCap;

            // Set the velocity
            Vector2 desiredVelocity = Vector2.Lerp(distanceFromDestination.SafeNormalize(Vector2.Zero) * minVelocity, maxVelocity, lerpValue);
            if (useSimpleFlyMovement)
                npc.SimpleFlyMovement(desiredVelocity, acceleration);
            else
                npc.velocity = desiredVelocity;
        }
        #endregion

        #region Calamity Style Targeting
        /// <summary>
        /// Replacement and extension for vanilla's NPC.TargetClosest. Has very flexible behavior.<br />
        /// Like vanilla's function, this function does not return any value, but makes its changes in-place.
        /// </summary>
        /// <param name="options">Struct to specify all options. Refer to struct definition in NPCUtils for details.</param>
        /// <returns>The targeted player ID.</returns>
        public static int CalamityTargeting(this NPC npc, CalamityTargetingParameters options)
        {
            // 05JUN2024: Ozzatron: Struct defaults are always memset to all-zero, giving garbage parameters.
            // If you actually call this function with the default keyword for the struct,
            // change the options on the spot to valid default / intended default parameters.
            if (options == default)
                options = new CalamityTargetingParameters();

            float distance = 0f;
            // float realDist = 0f; // Defined but not used by vanilla. Commented out here.
            bool anyTargetAvailable = false;
            int tankMinionProjectileID = -1;

            // The setup and initial loop is equivalent to vanilla NPC.TargetClosest, but optimized.
            foreach (Player p in Main.ActivePlayers)
            {
                bool playerDead = p.dead || p.ghost;
                if (playerDead)
                    continue;

                // ForceSwitch targeting. If the same player from last time is iterated over, just ignore them.
                // Player exclusion. If the player is to be excluded, do not consider them.
                bool sameTargetAsLastTime = p.whoAmI == npc.oldTarget;
                bool notSinglePlayer = Main.netMode != NetmodeID.SinglePlayer;
                if (notSinglePlayer && (options.excludedPlayers.Contains(p.whoAmI) || (options.targetType == NPCTargetType.ForceSwitch && sameTargetAsLastTime)))
                    continue;

                //
                // The below code is implemented in vanilla as a separate method. Here, it's inlined for efficiency.
                //

                Vector2 pCenter = p.Center;
                Vector2 targetCenter = options.targetingCenter ?? npc.Center;
                float manhattanDist = Math.Abs(targetCenter.X - pCenter.X) + Math.Abs(targetCenter.Y - pCenter.Y);

                // Hard cutoff range specified in options. If the player is further, completely ignore them.
                if (manhattanDist > options.maxSearchRange)
                    continue;

                // Line of sight requirement specified in options. Please don't use this without reducing the max search range.
                if (options.requireLineOfSight && !Collision.CanHit(npc.Center, 1, 1, pCenter, 1, 1))
                    continue;

                float aggroAdjustedDist = manhattanDist - options.aggroRatio * p.aggro;

                // Implementation of "Finish Them Off": Add enormous amounts of virtual aggro to low health players
                if (options.finishThemOff)
                {
                    float missingHPRatio = MathHelper.Clamp(1f - p.statLife / (float)p.statLifeMax2, 0f, 1f);
                    float bloodthirstAggro = MathHelper.Lerp(0f, CalamityTargetingParameters.FinishThemOff_MaxAggroBoost, missingHPRatio);
                    aggroAdjustedDist -= bloodthirstAggro;
                }

                bool aggroDisabled = p.npcTypeNoAggro[npc.type];
                if (aggroDisabled && npc.direction != 0)
                    aggroAdjustedDist += 1000f;

                bool cancelTargeting = false;

                // PreferSame targeting. If the same player from last time is a valid target, even if not the "best" target, pick it anyway.
                bool preferSameFound = options.targetType == NPCTargetType.PreferSame && sameTargetAsLastTime;

                // Standard targeting. If the adjusted distance is lower, or this is the first valid target, actually choose the new target.
                bool standardTargetingRequirementsMet = !anyTargetAvailable || aggroAdjustedDist < distance;

                // If either targeting method succeeded, then this target is being engaged.
                bool engageThisTarget = preferSameFound || standardTargetingRequirementsMet;
                if (engageThisTarget)
                {
                    anyTargetAvailable = true;
                    tankMinionProjectileID = -1; // Reset any Stardust Guardian aggro because a real player was found.
                    distance = aggroAdjustedDist;
                    npc.target = p.whoAmI;

                    // If PreferSame targeting is active, and the same player was found, cancel further iteration. They are being chosen above all others.
                    if (preferSameFound)
                        cancelTargeting = true;
                }

                // "Tank pet" accomodation, AKA the 1.4+ Stardust Guardian
                // Basically, if the player would be targeted, give a chance to instead target their tank minion
                //
                // This behavior is not documented on the vanilla wiki.
                if (p.tankPet >= 0 && !aggroDisabled && !options.ignoreTankMinions)
                {
                    Projectile tankMinion = Main.projectile[p.tankPet];
                    Vector2 tmCenter = tankMinion.Center;
                    float manhattanDistToTankMinion = Math.Abs(targetCenter.X - tmCenter.X) + Math.Abs(targetCenter.Y - tmCenter.Y);

                    // The Stardust Guardian is considered to have a 200 aggro bonus by default.
                    // In Calamity this is scaled by the aggro ratio specified in options.
                    manhattanDistToTankMinion -= options.aggroRatio * 200f;

                    // The Stardust Guardian only attracts the attention of NPCs within a very short distance
                    if (manhattanDistToTankMinion < distance && manhattanDistToTankMinion < 200f && Collision.CanHit(npc.Center, 1, 1, tmCenter, 1, 1))
                        tankMinionProjectileID = p.tankPet;
                }

                // If targeting has been short-circuited for any reason, cancel iteration over players.
                if (cancelTargeting)
                    break;
            }

            // If the NPC has been aggroed by a Stardust Guardian instead of an actual player, account for that
            if (tankMinionProjectileID >= 0)
            {
                Projectile tankMinion = Main.projectile[tankMinionProjectileID];
                npc.targetRect = tankMinion.Hitbox;

                // Always set direction to a nonzero value. This NPC has been engaged in combat.
                npc.direction = 1;
                if (tankMinion.Center.X < npc.Center.X)
                    npc.direction = -1;

                npc.directionY = 1;
                if (tankMinion.Center.Y < npc.Center.Y)
                    npc.directionY = -1;
            }

            // Standard player aggro occurs here
            else
            {
                bool shouldFaceTarget = options.faceTarget;

                // Sanitize targeted player index
                if (npc.target < 0 || npc.target >= Main.maxPlayers)
                    npc.target = 0;

                Player targetPlayer = Main.player[npc.target];
                npc.targetRect = targetPlayer.Hitbox;

                // Do not switch facing to look at dead players.
                if (targetPlayer.dead)
                    shouldFaceTarget = false;

                // If already engaged in combat, do not switch facing to look at players that ignore your aggro.
                if (targetPlayer.npcTypeNoAggro[npc.type] && npc.direction != 0)
                    shouldFaceTarget = false;

                if (shouldFaceTarget)
                {
                    bool oldTargetWasValid = npc.oldTarget >= 0 && npc.oldTarget < Main.maxPlayers;

                    bool targetIsLowAggroNotUsingItem = targetPlayer.itemAnimation == 0 && targetPlayer.aggro < 0;
                    bool willIgnoreStealthedPlayers = !npc.boss && options.ignoreStealthedPlayers;

                    // Regular NPCs (not bosses) will voluntarily ignore otherwise-valid player targets with less than zero aggro if they are not actively using an item.
                    // This ONLY WORKS if they already have another valid target, aka multiplayer.
                    // As such, having net less than zero aggro enables you to remain "stealthed" to regular enemies if you are not doing anything.
                    // This is undocumented vanilla behavior.
                    bool ignoreStealthedPlayer = willIgnoreStealthedPlayers && oldTargetWasValid && targetIsLowAggroNotUsingItem;
                    if (!ignoreStealthedPlayer)
                    {
                        // Always set direction to a nonzero value. This NPC has been engaged in combat.
                        npc.direction = 1;
                        if (targetPlayer.Center.X < npc.Center.X)
                            npc.direction = -1;

                        npc.directionY = 1;
                        if (targetPlayer.Center.Y < npc.Center.Y)
                            npc.directionY = -1;
                    }
                }
            }

            // Confused enemies always run in the exact wrong direction, horizontally at least.
            if (npc.confused)
                npc.direction *= -1;

            // Apply net updates.
            bool directionChange = npc.direction != npc.oldDirection || npc.directionY != npc.oldDirectionY;
            bool targetChange = npc.target != npc.oldTarget;
            bool shouldNetUpdate = (directionChange || targetChange) && !npc.collideX && !npc.collideY;
            if (shouldNetUpdate || options.forceNetUpdate)
                npc.netUpdate = true;

            return npc.target;
        }
        #endregion

        #region Minion Homing
        /// <summary>
        /// Detects nearby hostile NPCs from a given point
        /// </summary>
        /// <param name="origin">The position where we wish to check for nearby NPCs</param>
        /// <param name="maxDistanceToCheck">Maximum amount of pixels to check around the origin</param>
        /// <param name="ignoreTiles">Whether to ignore tiles when finding a target or not</param>
        /// <param name="bossPriority">Whether bosses should be prioritized in targetting or not</param>
        public static NPC ClosestNPCAt(this Vector2 origin, float maxDistanceToCheck, bool ignoreTiles = true, bool bossPriority = false)
        {
            NPC closestTarget = null;
            float distance = maxDistanceToCheck;
            if (bossPriority)
            {
                bool bossFound = false;
                for (int index = 0; index < Main.npc.Length; index++)
                {
                    // If we've found a valid boss target, ignore ALL targets which aren't bosses.
                    if (bossFound && !(Main.npc[index].boss || Main.npc[index].type == NPCID.WallofFleshEye))
                        continue;

                    if (Main.npc[index].CanBeChasedBy(null, false))
                    {
                        float extraDistance = (Main.npc[index].width / 2) + (Main.npc[index].height / 2);

                        bool canHit = true;
                        if (extraDistance < distance && !ignoreTiles)
                            canHit = Collision.CanHit(origin, 1, 1, Main.npc[index].Center, 1, 1);

                        if (Vector2.Distance(origin, Main.npc[index].Center) < distance && canHit)
                        {
                            if (Main.npc[index].boss || Main.npc[index].type == NPCID.WallofFleshEye)
                                bossFound = true;

                            distance = Vector2.Distance(origin, Main.npc[index].Center);
                            closestTarget = Main.npc[index];
                        }
                    }
                }
            }
            else
            {
                for (int index = 0; index < Main.npc.Length; index++)
                {
                    if (Main.npc[index].CanBeChasedBy(null, false))
                    {
                        float extraDistance = (Main.npc[index].width / 2) + (Main.npc[index].height / 2);

                        bool canHit = true;
                        if (extraDistance < distance && !ignoreTiles)
                            canHit = Collision.CanHit(origin, 1, 1, Main.npc[index].Center, 1, 1);

                        if (Vector2.Distance(origin, Main.npc[index].Center) < distance && canHit)
                        {
                            distance = Vector2.Distance(origin, Main.npc[index].Center);
                            closestTarget = Main.npc[index];
                        }
                    }
                }
            }
            return closestTarget;
        }

        /// <summary>
        /// Detects the hostile NPC that is closest angle-wise to the rotation vector
        /// </summary>
        /// <param name="origin">The position that will be used to find the rotation vector to NPCs</param>
        /// <param name="checkRotationVector">The rotation vector that the other rotation vectors to NPCs will be compared to</param>
        /// <param name="maxDistanceToCheck">Maximum amount of pixels to check around the origin</param>
        /// <param name="wantedHalfCone">When the angle between the rotation vector and the vector to the NPC is less than or equal to this, NPCs start getting ranked by distance. Set to 0 or less to ignore</param>
        /// <param name="ignoreTiles">Whether or not to ignore tiles when finding a target</param>
        /// <returns>The NPC that best fits the parameters. Null if no NPC is found</returns>
        public static NPC ClosestNPCToAngle(this Vector2 origin, Vector2 checkRotationVector, float maxDistanceToCheck, float wantedHalfCone = 0.125f, bool ignoreTiles = true)
        {
            NPC closestTarget = null;
            float distance = maxDistanceToCheck;
            float angle = MathHelper.Pi;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.CanBeChasedBy(null, false))
                    continue;

                float checkDist = origin.Distance(npc.Center);
                if (checkDist >= distance) // Immediately disqualify anything beyond the distance that must be beaten
                    continue;

                float angleBetween = checkRotationVector.AngleBetween(npc.Center - origin);
                if (angleBetween > angle) // Narrow down to the closest npc to the angle
                    continue;

                if (!ignoreTiles && !Collision.CanHit(origin, 1, 1, npc.Center, 1, 1)) // Tile LoS check if wanted
                    continue;

                if (angle <= wantedHalfCone)
                {
                    angle = wantedHalfCone;
                    distance = checkDist; // We are within the cone. Now npcs are further narrowed down by distance
                    closestTarget = npc;
                }
                else
                {
                    angle = angleBetween;
                    closestTarget = npc;
                }
            }

            return closestTarget;
        }

        /// <summary>
        /// Detects nearby hostile NPCs from a given point with minion support
        /// </summary>
        /// <param name="origin">The position where we wish to check for nearby NPCs</param>
        /// <param name="maxDistanceToCheck">Maximum amount of pixels to check around the origin</param>
        /// <param name="owner">Owner of the minion</param>
        /// <param name="ignoreTiles">Whether to ignore tiles when finding a target or not</param>
        public static NPC MinionHoming(this Vector2 origin, float maxDistanceToCheck, Player owner, bool ignoreTiles = true, bool checksRange = false)
        {
            if (owner is null || !owner.whoAmI.WithinBounds(Main.maxPlayers) || !owner.MinionAttackTargetNPC.WithinBounds(Main.maxNPCs))
                return ClosestNPCAt(origin, maxDistanceToCheck, ignoreTiles);
            NPC npc = Main.npc[owner.MinionAttackTargetNPC];
            bool canHit = true;
            if (!ignoreTiles)
                canHit = Collision.CanHit(origin, 1, 1, npc.Center, 1, 1);
            float extraDistance = (npc.width / 2) + (npc.height / 2);
            bool distCheck = Vector2.Distance(origin, npc.Center) < (maxDistanceToCheck + extraDistance) || !checksRange;
            if (owner.HasMinionAttackTargetNPC && canHit && distCheck)
            {
                return npc;
            }
            return ClosestNPCAt(origin, maxDistanceToCheck, ignoreTiles);
        }
        #endregion

        /// <summary>
        /// Check if an NPC is organic
        /// </summary>
        /// <param name="target">The NPC attacked.</param>
        /// <returns>Whether or not the NPC is organic.</returns>
        public static bool Organic(this NPC target)
        {
            if ((target.HitSound != SoundID.NPCHit4 && target.HitSound != SoundID.NPCHit41 && target.HitSound != SoundID.NPCHit2 &&
                target.HitSound != SoundID.NPCHit5 && target.HitSound != SoundID.NPCHit11 && target.HitSound != SoundID.NPCHit30 &&
                target.HitSound != SoundID.NPCHit34 && target.HitSound != SoundID.NPCHit36 && target.HitSound != SoundID.NPCHit42 &&
                target.HitSound != SoundID.NPCHit49 && target.HitSound != SoundID.NPCHit52 && target.HitSound != SoundID.NPCHit53 &&
                target.HitSound != SoundID.NPCHit54 && target.HitSound != null) || target.type == NPCType<Providence>() ||
                target.type == NPCType<ScornEater>() || target.type == NPCType<Yharon>())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if an NPC can be moved
        /// </summary>
        /// <param name="target">The NPC attacked.</param>
        /// <returns>Whether or not the NPC can be moved around.</returns>
        public static bool CanBeMoved(this NPC target)
        {
            bool whitelisted = CalamityHeavyKnockbackWhitelist.whitelistNPC.Contains(target.type);

            if ((target.knockBackResist != 0 || whitelisted) && target.IsAnEnemy(true, true, false))
                return true;
            return false;
        }
        /// <summary>
        /// Moves an NPC, usually used as custom knockback
        /// </summary>
        /// <param name="target">The NPC being moved.</param>
        /// <param name="ignoreKBImmune">Whether or not NPC's that normally have knockback immunity can be moved around.</param>
        public static void MoveNPC(this NPC target, Vector2 direction, float strength, bool heavyKnockback = false, Player attacker = null)
        {
            if (target.CanBeMoved())
            {
                Vector2 launchVel = direction.SafeNormalize(Vector2.UnitX) * strength;
                float playerKnockbackMult = 1; // Xyk 19MARCH2026: If we ever make knockback a real stat, it can go here. It's much too messy right now to work with, but at least the player is already supported.
                float knockbackMult = playerKnockbackMult * (heavyKnockback ? Math.Max(target.knockBackResist, 1) : target.knockBackResist); // Heavy knockback ignores knockback resist (but not knockback weakness)
                target.velocity = launchVel * knockbackMult;
                target.SyncMotionToServer();
            }
        }

        /// <summary>
        /// Allows grounded NPCs to step up blocks
        /// </summary>
        /// <param name="npc"></param>
        public static void StepUpBlocks(this NPC npc)
        {
            Vector2 position = npc.position;
            position.X += npc.velocity.X;
            int x = (int)((position.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 1)) * npc.direction) / 16f);
            int y = (int)((position.Y + (float)npc.height - 1f) / 16f);

            if ((float)(x * 16) >= position.X + (float)npc.width || (float)(x * 16 + 16) <= position.X)
                return;

            bool nextTileValid = Main.tile[x, y].HasUnactuatedTile && !Main.tile[x, y].TopSlope && !Main.tile[x, y - 1].TopSlope && Main.tileSolid[(int)Main.tile[x, y].TileType] && !Main.tileSolidTop[(int)Main.tile[x, y].TileType];
            bool aboveTileHalfBlock = Main.tile[x, y - 1].IsHalfBlock && Main.tile[x, y - 1].HasUnactuatedTile;
            bool aboveTileHasRoom = Main.tile[x, y - 1].IsHalfBlock && IsPassableTile(x, y - 4);
            bool aboveTileEmpty = !Main.tile[x, y - 1].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[x, y - 1].TileType] || Main.tileSolidTop[(int)Main.tile[x, y - 1].TileType] || aboveTileHasRoom;
            bool tile3AbovePassable = !Main.tile[x - npc.direction, y - 3].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[x - npc.direction, y - 3].TileType];

            if ((nextTileValid || aboveTileHalfBlock) && aboveTileEmpty && IsPassableTile(x, y - 2) && IsPassableTile(x, y - 3) && tile3AbovePassable)
            {
                float npcBottom = (float)(y * 16);
                if (Main.tile[x, y].IsHalfBlock)
                {
                    npcBottom += 8f;
                }
                if (Main.tile[x, y - 1].IsHalfBlock)
                {
                    npcBottom -= 8f;
                }
                if (npcBottom < position.Y + (float)npc.height)
                {
                    float percentageTileRisen = position.Y + (float)npc.height - npcBottom;
                    if (percentageTileRisen <= 16.1f)
                    {
                        npc.gfxOffY += npc.position.Y + (float)npc.height - npcBottom;
                        npc.position.Y = npcBottom - (float)npc.height;
                        if (percentageTileRisen < 9f)
                        {
                            npc.stepSpeed = 1f;
                        }
                        else
                        {
                            npc.stepSpeed = 2f;
                        }
                    }
                }
            }
        }

        public static bool IsPassableTile(int x, int y)
        {
            return (!Main.tile[x, y].HasUnactuatedTile ||
                !Main.tileSolid[(int)Main.tile[x, y].TileType] || Main.tileSolidTop[(int)Main.tile[x, y].TileType]);
        }


        public static void Inflict246DebuffsNPC(NPC target, int buff, float timeBase = 2f)
        {
            if (Main.rand.NextBool(4))
            {
                target.AddBuff(buff, SecondsToFrames(timeBase * 3f), false);
            }
            else if (Main.rand.NextBool())
            {
                target.AddBuff(buff, SecondsToFrames(timeBase * 2f), false);
            }
            else
            {
                target.AddBuff(buff, SecondsToFrames(timeBase), false);
            }
        }

        /// <summary>
        /// Spawns a <see cref="CombatText"/> indicating the amount of damage manually dealt to the NPC, such as from self-damage. Automatically syncs it in multiplayer.
        /// </summary>
        public static void DamageEffect(this NPC npc, int damageAmount)
        {
            Rectangle r = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
            Color textColor = new Color(255, 30, 100);
            if (Main.dedServ)
                NetMessage.SendData(MessageID.CombatTextInt, -1, -1, null, (int)textColor.PackedValue, r.Center.X, r.Center.Y, damageAmount);
            else
                CombatText.NewText(r, textColor, damageAmount);
        }

        public static void ProduceGoldCritterDust(this NPC npc)
        {
            npc.position += npc.netOffset;
            Color color = Lighting.GetColor((int)npc.Center.X / 16, (int)npc.Center.Y / 16);
            if (color.R > 20 || color.B > 20 || color.G > 20)
            {
                int colorVal = color.R;
                if (color.G > colorVal)
                {
                    colorVal = color.G;
                }
                if (color.B > colorVal)
                {
                    colorVal = color.B;
                }
                colorVal /= 30;
                if (Main.rand.Next(300) < colorVal)
                {
                    int golddust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.TintableDustLighted, 0f, 0f, 254, new Color(255, 255, 0), 0.5f);
                    Main.dust[golddust].velocity *= 0f;
                }
            }
            npc.position -= npc.netOffset;
        }

        /// <summary>
        /// Spawns gores. Automatically accounts for multiplayer.
        /// </summary>
        /// <param name="entity">The Entity in question. Usuaully used for NPCs but also applicable for other Entities.</param>
        /// <param name="suffix">The name of the gore.</param>
        /// <param name="variants">How many variants there are, defaults to 1.</param>
        /// <param name="startFrom">What number should the variant names start counting from? A default of -1 means the first gore has no number, while the rest start from 2 and increment by 1.</param>
        /// <param name="deathCheck">Check if the entity in question is a dead NPC. Defaults to true.</param>
        public static void SpawnGores(Entity entity, string suffix, int variants = 1, int startFrom = -1, bool deathCheck = true)
        {
            if (!Main.dedServ)
            {
                bool isADeadNPC = true;
                if (entity is NPC n && deathCheck)
                {
                    if (n.life > 0)
                        isADeadNPC = false;
                }
                if (isADeadNPC)
                {
                    for (int i = 0; i < variants; i++)
                    {
                        int? idx = startFrom == -1 ? null : startFrom;
                        if (i > 0)
                        {
                            idx = startFrom == -1 ? (i + 1) : (idx + i + 1);
                        }
                        Gore.NewGore(entity.GetSource_Death(), entity.position, entity.velocity, CalamityMod.Instance.Find<ModGore>(suffix + idx).Type, 1f);
                    }
                }
            }
        }

        public static NPCShop AddWithCustomValue(this NPCShop shop, int itemType, int customValue, params Condition[] conditions)
        {
            var item = new Item(itemType)
            {
                shopCustomPrice = customValue
            };
            return shop.Add(item, conditions);
        }

        public static NPCShop AddWithCustomValue<T>(this NPCShop shop, int customValue, params Condition[] conditions) where T : ModItem
        {
            return shop.AddWithCustomValue(ItemType<T>(), customValue, conditions);
        }

        public static void DrawBackglow(this NPC npc, Color backglowColor, float backglowArea, SpriteEffects spriteEffects, Rectangle frame, Vector2 screenPos, Texture2D overrideTexture = null)
        {
            Texture2D texture = overrideTexture is null ? TextureAssets.Npc[npc.type].Value : overrideTexture;
            Vector2 drawPosition = npc.Center - screenPos;
            Vector2 origin = frame.Size() * 0.5f;
            Color backAfterimageColor = backglowColor * npc.Opacity;
            for (int i = 0; i < 10; i++)
            {
                Vector2 drawOffset = (MathHelper.TwoPi * i / 10f).ToRotationVector2() * backglowArea;
                Main.spriteBatch.Draw(texture, drawPosition + drawOffset, frame, backAfterimageColor, npc.rotation, origin, npc.scale, spriteEffects, 0f);
            }
        }

        public static bool DrawAnimatedBestiaryWorm(SpriteBatch spriteBatch, NPC npc, Color drawColor, Texture2D headTexture, Texture2D bodyTexture, int segmentCount, int segmentSpacing, float rotationStrength, Vector2 baseOffset, int animationSpeed, float range, float headOffset = 0, float headSpeedOffset = 0, bool flip = false)
        {
            DrawAnimatedBestiaryWorm(spriteBatch, npc, drawColor, headTexture, [bodyTexture], segmentCount, segmentSpacing, rotationStrength, baseOffset, animationSpeed, range, headOffset, headSpeedOffset, flip);
            return false;
        }

        public static bool DrawAnimatedBestiaryWorm(SpriteBatch spriteBatch, NPC npc, Color drawColor, Texture2D headTexture, Texture2D bodyTexture, Texture2D bodyTextureAlt, int segmentCount, int segmentSpacing, float rotationStrength, Vector2 baseOffset, int animationSpeed, float range, float headOffset = 0, float headSpeedOffset = 0, bool flip = false)
        {
            DrawAnimatedBestiaryWorm(spriteBatch, npc, drawColor, headTexture, [bodyTexture, bodyTextureAlt], segmentCount, segmentSpacing, rotationStrength, baseOffset, animationSpeed, range, headOffset, headSpeedOffset, flip);
            return false;
        }

        /// <summary>
        /// Draws animated wiggly worms for the Bestiary
        /// </summary>
        /// <param name="spriteBatch">The PreDraw's SpriteBatch</param>
        /// <param name="npc">The NPC to draw</param>
        /// <param name="drawColor">The NPC's drawColor</param>
        /// <param name="headTexture">The worm's head texture</param>
        /// <param name="bodyTextures">The worm's body textures</param>
        /// <param name="segmentCount">The amount of segments that should be added</param>
        /// <param name="segmentSpacing">The spacing between segments</param>
        /// <param name="rotationStrength">How strongly the worm rotates. Higher values cause it to make sharper turns</param>
        /// <param name="baseOffset">Moves around the position of the worm</param>
        /// <param name="animationSpeed">How fast the worm moves</param>
        /// <param name="range">How far up and down the worm moves</param>
        /// <param name="headOffset">How far to bash (move) the head horizontally in case the automated math is too off or the worm's head extends past its neck joint</param>
        /// <param name="headSpeedOffset">Offsets the animation progression for the head. Meant to pair with headOffset</param>
        /// <param name="flip">If the sprites should be flipped. Used for worms viewed from the side like Wyverns</param>
        /// <returns></returns>
        public static bool DrawAnimatedBestiaryWorm(SpriteBatch spriteBatch, NPC npc, Color drawColor, Texture2D headTexture, Texture2D[] bodyTextures, int segmentCount, int segmentSpacing, float rotationStrength, Vector2 baseOffset, int animationSpeed, float range, float headOffset = 0, float headSpeedOffset = 0, bool flip = false)
        {
            npc.frame = headTexture.Frame();
            // Buffers the segment position and rotations
            float offset = -0.2f;
            float startX = baseOffset.X;
            float startY = baseOffset.Y;
            SpriteEffects fx = flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float wormTimer = npc.Calamity().bestiaryWormTimer;
            // Draw the body segments
            for (int i = segmentCount; i > 0; i--)
            {
                // The first segment is slightly closer to keep up with the head
                float bodyOffset = i == 1 ? i * segmentSpacing * 0.4f : i * segmentSpacing - segmentSpacing * 0.5f;

                // If there's only one texture passed in, use it for all segments
                // If two are passed in, alternate between them
                // If more are passed in, each iteration must correspond to a texture
                Texture2D toUse = bodyTextures.Length == 1 ? bodyTextures[0] : bodyTextures.Length == 2 ? (i % 2 == 0 ? bodyTextures[0] : bodyTextures[1]) : bodyTextures[i - 1];
                spriteBatch.Draw(toUse, npc.position + new Vector2(startX + bodyOffset, MathF.Sin((wormTimer + offset * i) * animationSpeed) * range + startY), toUse.Frame(1, 1, 0, 0), npc.GetAlpha(drawColor), npc.rotation - MathHelper.PiOver2 - MathF.Cos((wormTimer + offset * i) * animationSpeed) * MathHelper.PiOver4 * rotationStrength, toUse.Size() / 2, npc.scale, fx, 0f);
            }
            // Draw the head
            spriteBatch.Draw(headTexture, npc.position + new Vector2(startX + headOffset, MathF.Sin((wormTimer - headSpeedOffset) * animationSpeed) * range + startY), npc.frame, npc.GetAlpha(drawColor), npc.rotation - MathHelper.PiOver2 - MathF.Cos((wormTimer - headSpeedOffset) * animationSpeed) * MathHelper.PiOver4 * rotationStrength, new Vector2(headTexture.Width * 0.5f, headTexture.Height), npc.scale, fx, 0f);

            return false;
        }

        public static bool HasSight(this NPC npc, Vector2 target) => Collision.CanHit(npc.Center, 1, 1, target, 1, 1);


        /// <summary>
        /// Attempts to find a tile of a given type within a given radius
        /// </summary>
        /// <param name="npc">The NPC to center it on</param>
        /// <param name="tileType">The tile type to check for</param>
        /// <param name="radius">The radius of the check area in pixels</param>
        /// <param name="usesSunkenSeaValidity">Whether or not SunkenSeaTileValidity should be used</param>
        /// <returns>The position of the tile, or null if unsuccessful</returns>
        public static Vector2? NPCTileDetection(NPC npc, int tileType, float radius, bool usesSunkenSeaValidity = false)
        {
            Vector2? tileFoundPosition = null;
            int? tileIndexFound = null;
            for (int i = 0; i < 360 && tileFoundPosition == null; i += 15)
            {
                var points = GetIntersectingPointsInLine(npc.Center, npc.Center - Vector2.UnitY.RotatedBy(MathHelper.ToRadians(i)) * radius);
                for (int j = points.Count - 1; j >= 0; j--)
                {
                    if (Main.tile[points[j]].TileType == tileType)
                    {
                        if (usesSunkenSeaValidity)
                        {
                            tileIndexFound = j;
                        }
                        else
                        {
                            tileFoundPosition = points[j].ToWorldCoordinates();
                        }
                        break;
                    }
                }

                if (usesSunkenSeaValidity)
                {
                    if (tileIndexFound == null)
                        continue;

                    for (int k = tileIndexFound.Value; k >= 0; k--)
                    {
                        Vector2 worldPos = points[k].ToWorldCoordinates();
                        if (npc.HasSight(worldPos) && SunkenSeaNPC.SunkenSeaTileValidity(npc, points[k]))
                        {
                            tileFoundPosition = worldPos;
                            break;
                        }
                    }

                    tileIndexFound = null;
                }
            }



            if (tileFoundPosition.HasValue)
                return tileFoundPosition.Value;

            return null;
        }

        #region Boss Spawning
        /// <summary>
        /// Shortcut for the generic boss summon message.
        /// </summary>
        /// <param name="npcIndex">The whoAmI index of the summoned npc.</param>
        public static void BossAwakenMessage(int npcIndex)
        {
            // TODO -- this should use MiscUtils DisplayLocalizedText.
            string typeName = Main.npc[npcIndex].TypeName;
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(Language.GetTextValue("Announcement.HasAwoken", typeName), new Color(175, 75, 255));
            }
            else if (Main.dedServ)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", [Main.npc[npcIndex].GetTypeNetName()]), new Color(175, 75, 255));
            }
        }

        /// <summary>
        /// Summons a boss near a particular area depending on a specific spawn context.
        /// </summary>
        /// <param name="relativeSpawnPosition">The relative spawn position.</param>
        /// <param name="bossType">The NPC type ID of the boss to spawn.</param>
        /// <param name="spawnContext">The context in which the direct spawn position is decided.</param>
        /// <param name="ai0">The optional 1st ai parameter for the boss.</param>
        /// <param name="ai1">The optional 2nd ai parameter for the boss.</param>
        /// <param name="ai2">The optional 3rd ai parameter for the boss.</param>
        /// <param name="ai3">The optional 4th ai parameter for the boss.</param>
        public static NPC SpawnBossBetter(Vector2 relativeSpawnPosition, int bossType, BaseBossSpawnContext spawnContext = null, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, float ai3 = 0f)
        {
            // Don't spawn entities client-side.
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return null;

            // Fall back to an exact spawn position if nothing else is inputted.
            if (spawnContext is null)
                spawnContext = new ExactPositionBossSpawnContext();

            Vector2 spawnPosition = spawnContext.DetermineSpawnPosition(relativeSpawnPosition);
            int bossIndex = NPC.NewNPC(NPC.GetBossSpawnSource(Player.FindClosest(spawnPosition, 1, 1)), (int)spawnPosition.X, (int)spawnPosition.Y, bossType, 0, ai0, ai1, ai2, ai3);

            // Broadcast a spawn message to indicate the summoning of the boss if it was successfully spawned.
            if (Main.npc.IndexInRange(bossIndex))
            {
                BossAwakenMessage(bossIndex);
                return Main.npc[bossIndex];
            }
            else
                return null;
        }

        /// <summary>
        /// Spawn Boss Method for Using Spawn Items
        /// <para>NOTE: This method use vanilla's spawn position behaviour!</para>
        /// </summary>
        /// <param name="player">Player who used Item</param>
        /// <param name="npcType">Boss's NPCType to spawn</param>
        /// <param name="spawnSound">Sound to play when spawn, it play on used player's position</param>
        public static void SpawnBossUsingItem(Player player, int npcType, in SoundStyle? spawnSound = null)
        {
            SoundEngine.PlaySound(spawnSound, player.Center);

            if (player.whoAmI != Main.myPlayer)
                return;

            // NOTE: MP netcode can be simplified by directly spawn npc like SpawnBossOnPosUsingItem does
            // but leaving this as vanilla's standard now
            switch (Main.netMode)
            {
                // SP: Spawn Boss Immediately
                case NetmodeID.SinglePlayer:
                    NPC.SpawnOnPlayer(player.whoAmI, npcType);
                    break;

                // MP: Ask server to spawn one
                case NetmodeID.MultiplayerClient:
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, -1, -1, null, player.whoAmI, npcType);
                    break;
            }
        }

        /// <summary>
        /// Spawn Boss Method for Using Spawn Items
        /// <para>NOTE: This method use vanilla's spawn position behaviour!</para>
        /// </summary>
        /// <typeparam name="BossType">Boss's NPCType to spawn</typeparam>
        /// <param name="player">Player who used Item</param>
        /// <param name="spawnSound">Sound to play when spawn, it play on used player's position</param>
        public static void SpawnBossUsingItem<BossType>(Player player, in SoundStyle? spawnSound = null) where BossType : ModNPC
        {
            SpawnBossUsingItem(player, NPCType<BossType>(), spawnSound);
        }

        /// <summary>
        /// Spawn Boss Method for Using Spawn Items
        /// <para>NOTE: This method can override spawn position to anywhere you want</para>
        /// </summary>
        /// <param name="player">Player who used Item</param>
        /// <param name="npcType">Boss's NPCType to spawn</param>
        /// <param name="worldX">Spawn Position X</param>
        /// <param name="worldY">Spawn Position Y</param>
        /// <param name="spawnSound">Sound to play when spawn, it play on used player's position</param>
        /// <returns>Spawned NPC Instance, null on MultiplayerClient</returns>
        public static NPC SpawnBossOnPosUsingItem(Player player, int npcType, int worldX, int worldY, in SoundStyle? spawnSound = null)
        {
            SoundEngine.PlaySound(spawnSound, player.Center);

            // This will ensure the NPC is spawned only once on Item.UseItem
            if (player.whoAmI != Main.myPlayer)
                return null;

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                // SP: Spawn Boss Immediately
                int spawnedNPCIdx = NPC.NewNPC(new EntitySource_BossSpawn(target: player), worldX, worldY, npcType, Start: 1);

                // 200 is invalid Index
                if (spawnedNPCIdx >= 200)
                    return null;

                BossAwakenMessage(spawnedNPCIdx);
                NPC npc = Main.npc[spawnedNPCIdx];
                npc.timeLeft *= 20;
                return npc;
            }
            else
            {
                // MP CLIENT: send syncing net message instead
                SpawnBossOnPositionPacket.Send(worldX, worldY, npcType, player);
            }
            return null;
        }

        /// <summary>
        /// Spawn Boss Method for Using Spawn Items
        /// <para>NOTE: This method can override spawn position to anywhere you want</para>
        /// </summary>
        /// <typeparam name="BossType"></typeparam>
        /// <param name="player">Player who used Item</param>
        /// <param name="worldX">Spawn Position X</param>
        /// <param name="worldY">Spawn Position Y</param>
        /// <param name="spawnSound">Sound to play when spawn, it play on used player's position</param>
        /// <returns>Spawned NPC Instance, null on MultiplayerClient</returns>
        public static NPC SpawnBossOnPosUsingItem<BossType>(Player player, int worldX, int worldY, in SoundStyle? spawnSound = null) where BossType : ModNPC
        {
            return SpawnBossOnPosUsingItem(player, NPCType<BossType>(), worldX, worldY, spawnSound);
        }

        /// <summary>
        /// Spawns Old Duke on a player. Only works server side, and only works if the player owns a fishing bobber.<br />
        /// Old Duke will spawn above the fishing bobber if one is found.
        /// </summary>
        /// <param name="playerIndex">The index of the player who will spawn Old Duke.</param>
        internal static void SpawnOldDuke(int playerIndex)
        {
            if (!Main.dedServ)
                return;

            Player player = Main.player[playerIndex];
            if (!player.active || player.dead)
                return;

            Projectile projectile = null;
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.bobber && p.owner == playerIndex)
                {
                    projectile = p;
                    break;
                }
            }

            if (projectile is null)
                return;

            int oldDuke = NPC.NewNPC(NPC.GetBossSpawnSource(playerIndex), (int)projectile.Center.X, (int)projectile.Center.Y + 100, NPCType<OldDuke>());
            BossAwakenMessage(oldDuke);
        }
        #endregion
    }
}

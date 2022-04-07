using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.DataStructures;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;
using Terraria.Chat;
using Terraria.DataStructures;

namespace CalamityMod
{
    public static partial class CalamityUtils
    {
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
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (!typesToCheck.Contains(Main.npc[i].type) || !Main.npc[i].active)
                    continue;

                count++;
            }

            return count;
        }

        private static readonly FieldInfo waterSpeedField = typeof(NPC).GetField("waterMovementSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
        public static void RemoveWaterSlowness(this NPC npc)
        {
            waterSpeedField.SetValue(npc, 1f);
        }

        /// <summary>
        /// Syncs position and velocity from a client to the server. This is to be used in contexts where these things are reliant on client-side information, such as <see cref="Main.MouseWorld"/>.
        /// </summary>
        /// <param name="npc"></param>
        public static void SyncMotionToServer(this NPC npc)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                return;

            var netMessage = CalamityMod.Instance.GetPacket();
            netMessage.Write((byte)CalamityModMessageType.SyncNPCMotionDataToServer);
            netMessage.Write(npc.whoAmI);
            netMessage.WriteVector2(npc.Center);
            netMessage.WriteVector2(npc.velocity);
            netMessage.Send();
        }

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
        public static void DR_NERD(this NPC npc, float normal, float? revengeance = null, float? death = null, float? bossRush = null, bool? customDR = null)
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

            if (customDR.HasValue)
                npc.Calamity().customDR = true;
        }

        public static bool IsAnEnemy(this NPC npc)
        {
            // Null, inactive, town NPCs, friendlies, statue spawns, "non-enemies" (e.g. butterflies or projectile enemies),
            // or anything else with no contact damage (exception: Providence and Celestial Pillars) don't count for rage.
            if (npc is null || !npc.active || npc.townNPC || npc.friendly || npc.SpawnedFromStatue || npc.lifeMax <= 5 || (npc.damage <= 5 && npc.lifeMax <= 2000))
                return false;
            // Also explicitly exclude dummies and anything with a ridiculous health pool (dummies from Fargo's for example).
            if (npc.type == NPCID.TargetDummy || npc.type == NPCType<SuperDummyNPC>() || npc.lifeMax > 100000000)
                return false;
            // Finally, exclude boss spawners.
            if (npc.type == NPCType<LeviathanStart>() || npc.type == NPCType<HiveCyst>() || npc.type == NPCType<PerforatorCyst>())
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
            return npc.type == NPCType<SlimeGod>() || npc.type == NPCType<SlimeGodRun>() ||
                npc.type == NPCType<SlimeGodSplit>() || npc.type == NPCType<SlimeGodRunSplit>();
        }

        public static bool AnyBossNPCS(bool checkForMechs = false)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i] != null)
                {
                    NPC npc = Main.npc[i];
                    if (npc.IsABoss())
                    {
                        // Added due to the new mech boss ore progression, return true if any mech is alive and checkForMechs is true, reduces mech boss projectile damage if true.
                        if (checkForMechs)
                            return npc.type == NPCID.TheDestroyer || npc.type == NPCID.SkeletronPrime || npc.type == NPCID.Spazmatism || npc.type == NPCID.Retinazer;
                        return true;
                    }
                }
            }
            return FindFirstProjectile(ProjectileType<DeusRitualDrama>()) != -1;
        }

        /// <summary>
        /// Syncs <see cref="CalamityGlobalNPC.newAI"/>. This exists specifically for AIs manipulated in a global context, as <see cref="GlobalNPC"/> has no netUpdate related hooks.
        /// </summary>
        /// <param name="npc"></param>
        public static void SyncExtraAI(this NPC npc)
        {
            // Don't bother attempting to send packets in singleplayer.
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            ModPacket packet = CalamityMod.Instance.GetPacket();
            packet.Write((byte)CalamityModMessageType.SyncCalamityNPCAIArray);
            packet.Write((byte)npc.whoAmI);

            for (int i = 0; i < npc.Calamity().newAI.Length; i++)
                packet.Write(npc.Calamity().newAI[i]);

            packet.Send();
        }

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

                        if (Vector2.Distance(origin, Main.npc[index].Center) < (distance + extraDistance) && canHit)
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

                        if (Vector2.Distance(origin, Main.npc[index].Center) < (distance + extraDistance) && canHit)
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
                target.type == NPCType<ScornEater>())
            {
                return true;
            }
            return false;
        }

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
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", new object[] { Main.npc[npcIndex].GetTypeNetName() }), new Color(175, 75, 255));
            }
        }

        public static void Inflict246DebuffsNPC(NPC target, int buff, float timeBase = 2f)
        {
            if (Main.rand.NextBool(4))
            {
                target.AddBuff(buff, SecondsToFrames(timeBase * 3f), false);
            }
            else if (Main.rand.NextBool(2))
            {
                target.AddBuff(buff, SecondsToFrames(timeBase * 2f), false);
            }
            else
            {
                target.AddBuff(buff, SecondsToFrames(timeBase), false);
            }
        }

        /// Inflict typical exo weapon debuffs. Duration multiplier optional.
        /// </summary>
        /// <param name="target">The NPC attacked.</param>
        /// <param name="multiplier">Debuff time multiplier if needed.</param>
        /// <returns>Inflicts debuffs if they can.</returns>
        public static void ExoDebuffs(this NPC target, float multiplier = 1f)
        {
            target.AddBuff(BuffType<ExoFreeze>(), (int)(30 * multiplier));
            target.AddBuff(BuffType<HolyFlames>(), (int)(120 * multiplier));
            target.AddBuff(BuffID.Frostburn, (int)(150 * multiplier));
            target.AddBuff(BuffID.OnFire, (int)(180 * multiplier));
        }

        public static T ModNPC<T>(this NPC npc) where T : ModNPC => npc.ModNPC as T;

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
        /// Spawns Old Duke on a player. Only works server side, and only works if the player owns a fishing bobber.<br />
        /// Old Duke will spawn above the fishing bobber if one is found.
        /// </summary>
        /// <param name="playerIndex">The index of the player who will spawn Old Duke.</param>
        internal static void SpawnOldDuke(int playerIndex)
        {
            if (Main.netMode != NetmodeID.Server)
                return;

            Player player = Main.player[playerIndex];
            if (!player.active || player.dead)
                return;

            Projectile projectile = null;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                projectile = Main.projectile[i];
                if (Main.projectile[i].active && Main.projectile[i].bobber && Main.projectile[i].owner == playerIndex)
                {
                    projectile = Main.projectile[i];
                    break;
                }
            }

            if (projectile is null)
                return;

            int oldDuke = NPC.NewNPC(NPC.GetBossSpawnSource(playerIndex), (int)projectile.Center.X, (int)projectile.Center.Y + 100, NPCType<OldDuke>());
            BossAwakenMessage(oldDuke);
        }
    }
}

using System.Linq;
using System.Reflection;
using CalamityMod.Balancing;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.DataStructures;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

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
        public static bool IsAnEnemy(this NPC npc, bool allowStatues = true, bool checkDead = true)
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
            if (npc.lifeMax <= BalancingConstants.TinyHealthThreshold || (npc.damage <= BalancingConstants.TinyDamageThreshold && npc.lifeMax <= BalancingConstants.NoContactDamageHealthThreshold))
                return false;
            // Also explicitly exclude dummies and anything with a ridiculous health pool (dummies from Fargo's for example).
            if (npc.type == NPCID.TargetDummy || npc.type == NPCType<SuperDummyNPC>() || npc.lifeMax > BalancingConstants.UnreasonableHealthThreshold)
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
        /// Syncs <see cref="NPC.localAI"/>. This exists specifically for AIs manipulated in a global context, as <see cref="GlobalNPC"/> has no netUpdate related hooks.
        /// </summary>
        /// <param name="npc"></param>
        public static void SyncVanillaLocalAI(this NPC npc)
        {
            // Don't bother attempting to send packets in singleplayer.
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            ModPacket packet = CalamityMod.Instance.GetPacket();
            packet.Write((byte)CalamityModMessageType.SyncVanillaNPCLocalAIArray);
            packet.Write((byte)npc.whoAmI);

            for (int i = 0; i < NPC.maxAI; i++)
                packet.Write(npc.localAI[i]);

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
            else if (Main.rand.NextBool())
            {
                target.AddBuff(buff, SecondsToFrames(timeBase * 2f), false);
            }
            else
            {
                target.AddBuff(buff, SecondsToFrames(timeBase), false);
            }
        }

        public static T ModNPC<T>(this NPC npc) where T : ModNPC => npc.ModNPC as T;

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

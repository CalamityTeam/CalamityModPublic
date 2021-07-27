using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Items;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.Calamitas;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Events
{
    public class BossRushEvent
    {
        public enum TimeChangeContext
        {
            None,
            Day,
            Night
        }
        public struct Boss
        {
            public int EntityID;
            public int SpecialSpawnCountdown;
            public bool UsesSpecialSound;
            public TimeChangeContext ToChangeTimeTo;
            public OnSpawnContext SpawnContext;

            public delegate void OnSpawnContext(int type);

            public Boss(int id, TimeChangeContext toChangeTimeTo = TimeChangeContext.None, OnSpawnContext spawnContext = null, int specialSpawnCountdown = -1, bool usesSpecialSound = false)
            {
                // Default to a typical SpawnOnPlayer call for boss summoning if nothing else is inputted.
                if (spawnContext is null)
                {
                    spawnContext = (type) => NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
                }

                EntityID = id;
                SpecialSpawnCountdown = specialSpawnCountdown;
                UsesSpecialSound = usesSpecialSound;
                ToChangeTimeTo = toChangeTimeTo;
                SpawnContext = spawnContext;
            }
        }
        public static bool BossRushActive = false; // Whether Boss Rush is active or not.
        public static bool DeactivateStupidFuckingBullshit = false; // Force Boss Rush to inactive.
        public static int BossRushStage = 0; // Boss Rush Stage.
        public static int BossRushSpawnCountdown = 180; // Delay before another Boss Rush boss can spawn.
        public static List<Boss> Bosses = new List<Boss>();
        public static Dictionary<int, int[]> BossIDsAfterDeath = new Dictionary<int, int[]>();
        public static Dictionary<int, Action<NPC>> BossDeathEffects = new Dictionary<int, Action<NPC>>();
        public static readonly Color XerocTextColor = Color.LightCoral;
        public static int ClosestPlayerToWorldCenter => Player.FindClosest(new Vector2(Main.maxTilesX, Main.maxTilesY) * 16f * 0.5f, 1, 1);
        public static int CurrentlyFoughtBoss => Bosses[BossRushStage].EntityID;
        public static int NextBossToFight => Bosses[BossRushStage + 1].EntityID;

        #region Loading and Unloading
        public static void Load()
        {
            Bosses = new List<Boss>()
            {
                new Boss(NPCID.QueenBee),
                new Boss(NPCID.BrainofCthulhu),
                new Boss(NPCID.KingSlime),
                new Boss(NPCID.EyeofCthulhu, TimeChangeContext.Night),
                new Boss(NPCID.SkeletronPrime, TimeChangeContext.Night),
                new Boss(NPCID.Golem, TimeChangeContext.Day, type =>
                {
                    int shittyStatueBoss = NPC.NewNPC((int)(Main.player[ClosestPlayerToWorldCenter].position.X + Main.rand.Next(-100, 101)), (int)(Main.player[ClosestPlayerToWorldCenter].position.Y - 400f), type, 1);
                    Main.npc[shittyStatueBoss].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(shittyStatueBoss);
                }),
                new Boss(ModContent.NPCType<ProfanedGuardianBoss>(), TimeChangeContext.Day),
                new Boss(NPCID.EaterofWorldsHead),
                new Boss(ModContent.NPCType<AstrumAureus>(), TimeChangeContext.Night),
                new Boss(NPCID.TheDestroyer, TimeChangeContext.Night, specialSpawnCountdown: 300),
                new Boss(NPCID.Spazmatism, TimeChangeContext.Night, type =>
                {
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, NPCID.Spazmatism);
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, NPCID.Retinazer);
                }),
                new Boss(ModContent.NPCType<Bumblefuck>(), TimeChangeContext.Day),
                new Boss(NPCID.WallofFlesh, spawnContext: type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];
                    NPC.SpawnWOF(player.position);
                }),
                new Boss(ModContent.NPCType<HiveMind>()),
                new Boss(NPCID.SkeletronHead, TimeChangeContext.Night, type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];
                    int sans = NPC.NewNPC((int)(player.position.X + Main.rand.Next(-100, 101)), (int)(player.position.Y - 400f), type, 1);
                    Main.npc[sans].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(sans);
                }),
                new Boss(ModContent.NPCType<StormWeaverHead>(), TimeChangeContext.Day),
                new Boss(ModContent.NPCType<AquaticScourgeHead>()),
                new Boss(ModContent.NPCType<DesertScourgeHead>(), spawnContext: type =>
                {
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, ModContent.NPCType<DesertScourgeHead>());
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, ModContent.NPCType<DesertScourgeHeadSmall>());
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, ModContent.NPCType<DesertScourgeHeadSmall>());
                }),
                new Boss(NPCID.CultistBoss, spawnContext: type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];
                    int doctorLooneyTunes = NPC.NewNPC((int)player.Center.X, (int)player.Center.Y - 400, type, 1);
                    Main.npc[doctorLooneyTunes].direction = Main.npc[doctorLooneyTunes].spriteDirection = Math.Sign(player.Center.X - player.Center.X - 90f);
                    Main.npc[doctorLooneyTunes].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(doctorLooneyTunes);
                }),
                new Boss(ModContent.NPCType<CrabulonIdle>(), spawnContext: type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];

                    // Delete the pillars spawned by the cultist.
                    for (int doom = 0; doom < Main.maxNPCs; doom++)
                    {
                        bool isPillar = Main.npc[doom].type == NPCID.LunarTowerStardust || Main.npc[doom].type == NPCID.LunarTowerVortex || Main.npc[doom].type == NPCID.LunarTowerNebula || Main.npc[doom].type == NPCID.LunarTowerSolar;
                        if (Main.npc[doom].active && isPillar)
                        {
                            Main.npc[doom].active = false;
                            Main.npc[doom].netUpdate = true;
                        }
                    }
                    int thePefectOne = NPC.NewNPC((int)(player.position.X + Main.rand.Next(-100, 101)), (int)(player.position.Y - 400f), type, 1);
                    Main.npc[thePefectOne].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(thePefectOne);
                }, specialSpawnCountdown: 300),
                new Boss(NPCID.Plantera),
                new Boss(ModContent.NPCType<CeaselessVoid>()),
                new Boss(ModContent.NPCType<PerforatorHive>()),
                new Boss(ModContent.NPCType<Cryogen>()),
                new Boss(ModContent.NPCType<BrimstoneElemental>()),
                new Boss(ModContent.NPCType<Signus>(), specialSpawnCountdown: 360),
                new Boss(ModContent.NPCType<RavagerBody>(), spawnContext: type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];

                    Main.PlaySound(SoundID.Roar, player.position, 2);
                    int ravager = NPC.NewNPC((int)(player.position.X + Main.rand.Next(-100, 101)), (int)(player.position.Y - 400f), type, 1);
                    Main.npc[ravager].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(ravager);
                }, usesSpecialSound: true),
                new Boss(NPCID.DukeFishron, spawnContext: type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];
                    int dukeFishron = NPC.NewNPC((int)(player.position.X + Main.rand.Next(-100, 101)), (int)(player.position.Y - 400f), type, 1);
                    Main.npc[dukeFishron].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(dukeFishron);
                }),
                new Boss(NPCID.MoonLordCore, spawnContext: type =>
                {
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.BossRushTierThreeEndText2", XerocTextColor);
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
                }),
                new Boss(ModContent.NPCType<AstrumDeusHeadSpectral>(), TimeChangeContext.Night, type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];

                    Main.PlaySound(CalamityMod.Instance.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/AstrumDeusSpawn"), player.Center);
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
                }, usesSpecialSound: true),
                new Boss(ModContent.NPCType<Polterghast>(), TimeChangeContext.Day),
                new Boss(ModContent.NPCType<PlaguebringerGoliath>()),
                new Boss(ModContent.NPCType<Calamitas>(), TimeChangeContext.Night, specialSpawnCountdown: 420),
                new Boss(ModContent.NPCType<Siren>(), TimeChangeContext.Day),
                new Boss(ModContent.NPCType<OldDuke>(), spawnContext: type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];
                    int boomerDuke = NPC.NewNPC((int)(player.position.X + Main.rand.Next(-100, 101)), (int)(player.position.Y - 400f), type, 1);
                    Main.npc[boomerDuke].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(boomerDuke);
                }),
                new Boss(ModContent.NPCType<SlimeGodCore>()),
                new Boss(ModContent.NPCType<Providence>(), TimeChangeContext.Day, type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];

                    Main.PlaySound(CalamityMod.Instance.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ProvidenceSpawn"), player.Center);
                    int prov = NPC.NewNPC((int)(player.position.X + Main.rand.Next(-500, 501)), (int)(player.position.Y - 250f), type, 1);
                    Main.npc[prov].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(prov);
                }, usesSpecialSound: true),
                new Boss(ModContent.NPCType<SupremeCalamitas>(), spawnContext: type =>
                {
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.BossRushTierFourEndText2", XerocTextColor);
                    for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
                    {
                        if (Main.player[playerIndex].active)
                        {
                            Player player = Main.player[playerIndex];
                            if (player.FindBuffIndex(ModContent.BuffType<ExtremeGravity>()) > -1)
                                player.ClearBuff(ModContent.BuffType<ExtremeGravity>());
                        }
                    }
                    Main.PlaySound(CalamityMod.Instance.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SupremeCalamitasSpawn"), Main.player[ClosestPlayerToWorldCenter].Center);
                    CalamityUtils.SpawnBossBetter(Main.player[ClosestPlayerToWorldCenter].Top - new Vector2(42f, 84f), type);
                }),
                new Boss(ModContent.NPCType<Yharon>(), TimeChangeContext.Day),
                new Boss(ModContent.NPCType<DevourerofGodsHeadS>(), TimeChangeContext.Day, type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];

                    Main.PlaySound(CalamityMod.Instance.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/DevourerSpawn"), player.Center);
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
                }, usesSpecialSound: true)
            };

            BossIDsAfterDeath = new Dictionary<int, int[]>()
            {
                [ModContent.NPCType<HiveMind>()] = new int[] { ModContent.NPCType<HiveMindP2>() },
                [ModContent.NPCType<StormWeaverHead>()] = new int[] { ModContent.NPCType<StormWeaverHeadNaked>(), ModContent.NPCType<StormWeaverBodyNaked>(), ModContent.NPCType<StormWeaverTailNaked>() },
                [ModContent.NPCType<Calamitas>()] = new int[] { ModContent.NPCType<CalamitasRun3>() },
            };

            BossDeathEffects = new Dictionary<int, Action<NPC>>()
            {
                [NPCID.TheDestroyer] = npc =>
                {
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.BossRushTierOneEndText", XerocTextColor);
                },
                [NPCID.CultistBoss] = npc =>
                {
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.BossRushTierTwoEndText", XerocTextColor);
                },
                [NPCID.DukeFishron] = npc =>
                {
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.BossRushTierThreeEndText", XerocTextColor);
                },
                [ModContent.NPCType<Providence>()] = npc =>
                {
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.BossRushTierFourEndText", XerocTextColor);
                },
                [ModContent.NPCType<DevourerofGodsHeadS>()] = npc =>
                {
                    DropHelper.DropItem(npc, ModContent.ItemType<Rock>(), true);
                    BossRushStage = 0;
                    CalamityUtils.KillAllHostileProjectiles();
                    CalamityWorld.bossRushHostileProjKillCounter = 3;
                    BossRushActive = false;

                    CalamityNetcode.SyncWorld();
                    if (Main.netMode == NetmodeID.Server)
                    {
                        var netMessage = CalamityMod.Instance.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.BossRushStage);
                        netMessage.Write(BossRushStage);
                        netMessage.Send();
                    }

                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.BossRushTierFiveEndText", XerocTextColor);
                }
            };
        }

        public static void Unload()
        {
            Bosses = null;
            BossIDsAfterDeath = null;
            BossDeathEffects = null;
        }
        #endregion

        #region Updates
        internal static void MiscUpdateEffects()
        {
            if (!BossRushActive)
                return;

            // Prevent Moon Lord from spawning naturally
            if (NPC.MoonLordCountdown > 0)
                NPC.MoonLordCountdown = 0;

            // Handle projectile clearing.
            if (CalamityWorld.bossRushHostileProjKillCounter > 0)
            {
                CalamityWorld.bossRushHostileProjKillCounter--;
                if (CalamityWorld.bossRushHostileProjKillCounter == 1)
                    CalamityUtils.KillAllHostileProjectiles();

                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = CalamityMod.Instance.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.BRHostileProjKillSync);
                    netMessage.Write(CalamityWorld.bossRushHostileProjKillCounter);
                    netMessage.Send();
                }
            }
        }

        internal static void Update()
        {
            if (!BossRushActive)
            {
                BossRushSpawnCountdown = 180;
                if (BossRushStage != 0)
                {
                    BossRushStage = 0;
                    if (Main.netMode == NetmodeID.Server)
                    {
                        var netMessage = CalamityMod.Instance.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.BossRushStage);
                        netMessage.Write(BossRushStage);
                        netMessage.Send();
                    }
                }
                return;
            }

            MiscUpdateEffects();

            // Do boss rush countdown and shit if no boss is alive.
            if (!CalamityPlayer.areThereAnyDamnBosses)
            {
                if (BossRushSpawnCountdown > 0)
                    BossRushSpawnCountdown--;

                // Cooldown and boss spawn
                if (BossRushSpawnCountdown <= 0)
                {
                    // Cooldown before next boss spawns.
                    BossRushSpawnCountdown = 60;

                    // Increase cooldown post-Fishron.
                    if (BossRushStage >= Bosses.FindIndex(boss => boss.EntityID == NPCID.DukeFishron))
                        BossRushSpawnCountdown += 300;
                    
                    // Override the spawn countdown if specified.
                    if (BossRushStage < Bosses.Count - 1 && Bosses[BossRushStage + 1].SpecialSpawnCountdown != -1)
                        BossRushSpawnCountdown = Bosses[BossRushStage + 1].SpecialSpawnCountdown;

                    // Change time as necessary.
                    if (Bosses[BossRushStage].ToChangeTimeTo != TimeChangeContext.None)
                        CalamityWorld.ChangeTime(Bosses[BossRushStage].ToChangeTimeTo == TimeChangeContext.Day);
                    
                    // Play the typical boss roar sound.
                    if (!Bosses[BossRushStage].UsesSpecialSound)
                        Main.PlaySound(SoundID.Roar, Main.player[ClosestPlayerToWorldCenter].position, 0);

                    // And spawn the boss.
                    Bosses[BossRushStage].SpawnContext.Invoke(CurrentlyFoughtBoss);
                }
            }
        }

        #endregion

        #region On Boss Kill
        internal static void OnBossKill(NPC npc, Mod mod)
        {
            // Eater of Worlds splits in Boss Rush now, so you have to kill every single segment to progress.
            // Vanilla sets npc.boss to true for the last Eater of Worlds segment to die in NPC.checkDead.
            // This means we do not need to manually check for other segments ourselves.
            if (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail)
            {
                if (npc.boss)
                {
                    BossRushStage++;
                    CalamityUtils.KillAllHostileProjectiles();
                    CalamityWorld.bossRushHostileProjKillCounter = 3;
                }
            }

            // Anahita and Leviathan manually check for each other (this probably isn't necessary).
            else if (npc.type == ModContent.NPCType<Siren>() || npc.type == ModContent.NPCType<Leviathan>())
            {
                int bossType = (npc.type == ModContent.NPCType<Siren>()) ? ModContent.NPCType<Leviathan>() : ModContent.NPCType<Siren>();
                if (!NPC.AnyNPCs(bossType))
                {
                    BossRushStage++;
                    CalamityUtils.KillAllHostileProjectiles();
                    CalamityWorld.bossRushHostileProjKillCounter = 3;
                }
            }

            // Killing any split Deus head ends the fight instantly. You don't need to kill both.
            else if (npc.type == ModContent.NPCType<AstrumDeusHeadSpectral>() && npc.Calamity().newAI[0] != 0f)
            {
                BossRushStage++;
                CalamityUtils.KillAllHostileProjectiles();
                CalamityWorld.bossRushHostileProjKillCounter = 3;
            }

            // All Slime God entities must be killed to progress to the next stage.
            else if (npc.type == ModContent.NPCType<SlimeGodCore>() || npc.type == ModContent.NPCType<SlimeGodSplit>() || npc.type == ModContent.NPCType<SlimeGodRunSplit>())
            {
                if (npc.type == ModContent.NPCType<SlimeGodCore>() && !NPC.AnyNPCs(ModContent.NPCType<SlimeGodSplit>()) && !NPC.AnyNPCs(ModContent.NPCType<SlimeGodRunSplit>()) &&
                    !NPC.AnyNPCs(ModContent.NPCType<SlimeGod>()) && !NPC.AnyNPCs(ModContent.NPCType<SlimeGodRun>()))
                {
                    BossRushStage++;
                    CalamityUtils.KillAllHostileProjectiles();
                    CalamityWorld.bossRushHostileProjKillCounter = 3;
                }
                else if (npc.type == ModContent.NPCType<SlimeGodSplit>() && !NPC.AnyNPCs(ModContent.NPCType<SlimeGodCore>()) && !NPC.AnyNPCs(ModContent.NPCType<SlimeGodRunSplit>()) &&
                    NPC.CountNPCS(ModContent.NPCType<SlimeGodSplit>()) < 2 && !NPC.AnyNPCs(ModContent.NPCType<SlimeGodRun>()))
                {
                    BossRushStage++;
                    CalamityUtils.KillAllHostileProjectiles();
                    CalamityWorld.bossRushHostileProjKillCounter = 3;
                }
                else if (npc.type == ModContent.NPCType<SlimeGodRunSplit>() && !NPC.AnyNPCs(ModContent.NPCType<SlimeGodCore>()) && !NPC.AnyNPCs(ModContent.NPCType<SlimeGodSplit>()) &&
                    NPC.CountNPCS(ModContent.NPCType<SlimeGodRunSplit>()) < 2 && !NPC.AnyNPCs(ModContent.NPCType<SlimeGod>()))
                {
                    BossRushStage++;
                    CalamityUtils.KillAllHostileProjectiles();
                    CalamityWorld.bossRushHostileProjKillCounter = 3;
                }
            }

            // This is the generic form of "Are there any remaining NPCs on the boss list for this boss rush stage?" check.
            else if ((Bosses.Any(boss => boss.EntityID == npc.type) && !BossIDsAfterDeath.ContainsKey(npc.type)) ||
                     BossIDsAfterDeath.Values.Any(killList => killList.Contains(npc.type)))
            {
                BossRushStage++;
                CalamityUtils.KillAllHostileProjectiles();
                CalamityWorld.bossRushHostileProjKillCounter = 3;
                if (BossDeathEffects.ContainsKey(npc.type))
                    BossDeathEffects[npc.type].Invoke(npc);
            }

            // Sync the stage and progress of Boss Rush whenever a relevant boss dies.
            if (Main.netMode == NetmodeID.Server)
            {
                var netMessage = mod.GetPacket();
                netMessage.Write((byte)CalamityModMessageType.BossRushStage);
                netMessage.Write(BossRushStage);
                netMessage.Send();
                var netMessage2 = mod.GetPacket();
                netMessage2.Write((byte)CalamityModMessageType.BRHostileProjKillSync);
                netMessage2.Write(CalamityWorld.bossRushHostileProjKillCounter);
                netMessage2.Send();
            }
        }
        #endregion
    }
}

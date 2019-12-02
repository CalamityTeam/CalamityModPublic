using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.DifficultyItems;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.Calamitas;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.Tiles.Ores;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs
{
    public class CalamityGlobalNPCLoot : GlobalNPC
    {
        #region Instance Per Entity
        public override bool InstancePerEntity => false;
        public override bool CloneNewInstances => false;
        #endregion

        #region PreNPCLoot
        public override bool PreNPCLoot(NPC npc)
        {
            if (CalamityWorld.bossRushActive)
            {
                return BossRushLootCancel(npc, mod);
            }

            bool abyssLootCancel = AbyssLootCancel(npc, mod);
            if (abyssLootCancel)
            {
                return false;
            }

            if (CalamityWorld.revenge)
            {
                if (npc.type == NPCID.Probe || npc.type == NPCID.ServantofCthulhu || npc.type == NPCID.MoonLordCore)
                {
                    return false;
                }
            }

            // Determine whether this NPC is the second Twin killed in a fight, regardless of which Twin it is.
            bool lastTwinStanding = false;
            if (npc.type == NPCID.Retinazer)
            {
                lastTwinStanding = !NPC.AnyNPCs(NPCID.Spazmatism);
            }
            else if (npc.type == NPCID.Spazmatism)
            {
                lastTwinStanding = !NPC.AnyNPCs(NPCID.Retinazer);
            }

            // Mechanical Bosses' combined lore item
            bool mechLore = !NPC.downedMechBossAny && (lastTwinStanding || npc.type == NPCID.TheDestroyer || npc.type == NPCID.SkeletronPrime);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeMechs>(), true, mechLore);

            if (npc.type == NPCID.KingSlime)
            {
                // Drop a huge spray of Gel items
                int minGel = Main.expertMode ? 90 : 60;
                int maxGel = Main.expertMode ? 120 : 80;
                DropHelper.DropItemSpray(npc, ItemID.Gel, minGel, maxGel, 2);

                // King Slime doesn't have a lore item yet
                DropHelper.DropResidentEvilAmmo(npc, NPC.downedSlimeKing, 2, 0, 0);
            }
            else if (npc.type == NPCID.EyeofCthulhu)
            {
                DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeEyeofCthulhu>(), true, !NPC.downedBoss1);
                DropHelper.DropResidentEvilAmmo(npc, NPC.downedBoss1, 2, 0, 0);
            }
            else if ((npc.boss && (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail)) || npc.type == NPCID.BrainofCthulhu)
            {
                DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeCorruption>(), true, !WorldGen.crimson && !NPC.downedBoss2);
                DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeEaterofWorlds>(), true, !WorldGen.crimson && !NPC.downedBoss2);
                DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeCrimson>(), true, WorldGen.crimson && !NPC.downedBoss2);
                DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeBrainofCthulhu>(), true, WorldGen.crimson && !NPC.downedBoss2);
                DropHelper.DropResidentEvilAmmo(npc, NPC.downedBoss2, 2, 0, 0);
            }
            else if (npc.type == NPCID.QueenBee)
            {
                DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeQueenBee>(), true, !NPC.downedQueenBee);
                DropHelper.DropResidentEvilAmmo(npc, NPC.downedQueenBee, 2, 0, 0);
            }
            else if (npc.type == NPCID.SkeletronHead)
            {
                DropHelper.DropItemCondition(npc, ModContent.ItemType<ClothiersWrath>(), !Main.expertMode, DropHelper.RareVariantDropRateInt, 1, 1);
                DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeSkeletron>(), true, !NPC.downedBoss3);
                DropHelper.DropResidentEvilAmmo(npc, NPC.downedBoss3, 3, 1, 0);
            }
            else if (npc.type == NPCID.WallofFlesh)
            {
                DropHelper.DropItemCondition(npc, ModContent.ItemType<MLGRune>(), !Main.expertMode); // Demon Trophy
                DropHelper.DropItemCondition(npc, ModContent.ItemType<Meowthrower>(), !Main.expertMode, 5, 1, 1);
                DropHelper.DropItemCondition(npc, ModContent.ItemType<BlackHawkRemote>(), !Main.expertMode, 5, 1, 1);
                DropHelper.DropItemCondition(npc, ModContent.ItemType<BlastBarrel>(), !Main.expertMode, 5, 1, 1);
                DropHelper.DropItemCondition(npc, ModContent.ItemType<RogueEmblem>(), !Main.expertMode, 8, 1, 1);
                DropHelper.DropItemChance(npc, ModContent.ItemType<IbarakiBox>(), !Main.hardMode, Main.hardMode ? 0.1f : 1f); // 100% chance on first kill, 10% chance afterwards
                DropHelper.DropItemFromSetCondition(npc, !Main.expertMode, 5, ItemID.CorruptionKey, ItemID.CrimsonKey);

                DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeUnderworld>(), true, !Main.hardMode);
                DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeWallofFlesh>(), true, !Main.hardMode);
                DropHelper.DropResidentEvilAmmo(npc, Main.hardMode, 3, 1, 0);

                // First kill text (this is not a loot function)
                if (!Main.hardMode)
                {
                    string key2 = "Mods.CalamityMod.UglyBossText";
                    Color messageColor2 = Color.Aquamarine;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(Language.GetTextValue(key2), messageColor2);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key2), messageColor2);
                    }
                }
            }
            else if (lastTwinStanding)
            {
                DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeTwins>(), true, !NPC.downedMechBoss2);
                DropHelper.DropResidentEvilAmmo(npc, NPC.downedMechBoss2, 4, 2, 1);
            }
            else if (npc.type == NPCID.TheDestroyer)
            {
                DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeDestroyer>(), true, !NPC.downedMechBoss1);
                DropHelper.DropResidentEvilAmmo(npc, NPC.downedMechBoss1, 4, 2, 1);
            }
            else if (npc.type == NPCID.SkeletronPrime)
            {
                DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeSkeletronPrime>(), true, !NPC.downedMechBoss3);
                DropHelper.DropItemCondition(npc, ModContent.ItemType<GoldBurdenBreaker>(), true, npc.ai[1] == 2f && CalamityWorld.revenge);
                DropHelper.DropResidentEvilAmmo(npc, NPC.downedMechBoss3, 4, 2, 1);

                // If neither Prime nor Brimmy have been killed, show this text (not a loot function)
                string key = "Mods.CalamityMod.SteelSkullBossText";
                Color messageColor = Color.Crimson;
                if (!NPC.downedMechBoss3 && !CalamityWorld.downedBrimstoneElemental)
                {
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                }
            }
            else if (npc.type == NPCID.Plantera)
            {
                DropHelper.DropItemCondition(npc, ItemID.JungleKey, !Main.expertMode, 5, 1, 1);
                DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgePlantera>(), true, !NPC.downedPlantBoss);
                DropHelper.DropResidentEvilAmmo(npc, NPC.downedPlantBoss, 4, 2, 1);

                // Spawn Perennial Ore if Plantera has never been killed
                if (!NPC.downedPlantBoss)
                {
                    string key2 = "Mods.CalamityMod.PlantOreText";
                    Color messageColor2 = Color.GreenYellow;

                    WorldGenerationMethods.SpawnOre(ModContent.TileType<PerennialOre>(), 12E-05, .5f, .7f);
                    ;

                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(Language.GetTextValue(key2), messageColor2);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key2), messageColor2);
                    }
                }

                // Awaken the Abyss if neither Plantera or Calamitas has been killed
                if (!NPC.downedPlantBoss && !CalamityWorld.downedCalamitas)
                {
                    string key = "Mods.CalamityMod.PlantBossText";
                    Color messageColor = Color.RoyalBlue;

                    if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active)
                    {
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/WyrmScream"), (int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y);
                    }

                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                }
            }
            else if (npc.type == NPCID.Golem)
            {
                DropHelper.DropItemCondition(npc, ModContent.ItemType<EssenceofCinder>(), !Main.expertMode, 5, 10);
                DropHelper.DropItemCondition(npc, ItemID.Picksaw, true, !NPC.downedGolemBoss);
                DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeGolem>(), true, !NPC.downedGolemBoss);
                DropHelper.DropResidentEvilAmmo(npc, NPC.downedGolemBoss, 4, 2, 1);

                // If Golem has never been killed, send messages about PBG and Ravager
                if (!NPC.downedGolemBoss)
                {
                    string key = "Mods.CalamityMod.BabyBossText";
                    Color messageColor = Color.Lime;
                    string key2 = "Mods.CalamityMod.BabyBossText2";
                    Color messageColor2 = Color.Yellow;

                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                        Main.NewText(Language.GetTextValue(key2), messageColor2);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key2), messageColor2);
                    }
                }
            }
            else if (npc.type == NPCID.DD2Betsy && !CalamityWorld.downedBetsy)
            {
                DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedBetsy, 4, 2, 1);

                // Mark Betsy as dead (Vanilla does not keep track of her)
                CalamityWorld.downedBetsy = true;
                CalamityMod.UpdateServerBoolean();
            }
            else if (npc.type == NPCID.DukeFishron)
            {
                DropHelper.DropItemCondition(npc, ModContent.ItemType<DukesDecapitator>(), !Main.expertMode, 5, 1, 1);
                DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeDukeFishron>(), true, !NPC.downedFishron);
                DropHelper.DropResidentEvilAmmo(npc, NPC.downedPlantBoss, 4, 2, 1);
            }
            else if (npc.type == NPCID.CultistBoss)
            {
                DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeLunaticCultist>(), true, !NPC.downedAncientCultist);
                DropHelper.DropResidentEvilAmmo(npc, NPC.downedAncientCultist, 4, 2, 1);

                // Blood Moon lore item
                DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeBloodMoon>(), true, Main.bloodMoon);

                // Deus text (this is not a loot function)
                if (!NPC.downedAncientCultist)
                {
                    string key = "Mods.CalamityMod.DeusText";
                    Color messageColor = Color.Gold;

                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                }
            }
            else if (npc.type == NPCID.MoonLordCore)
            {
                DropHelper.DropItemCondition(npc, ModContent.ItemType<MLGRune2>(), true, !Main.expertMode);
                DropHelper.DropItemCondition(npc, ModContent.ItemType<UtensilPoker>(), !Main.expertMode, 9, 1, 1);
                DropHelper.DropItemCondition(npc, ModContent.ItemType<GrandDad>(), !Main.expertMode, DropHelper.RareVariantDropRateInt, 1, 1);
                DropHelper.DropItemCondition(npc, ModContent.ItemType<Infinity>(), !Main.expertMode, DropHelper.RareVariantDropRateInt, 1, 1);
                DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeMoonLord>(), true, !NPC.downedMoonlord);
                DropHelper.DropResidentEvilAmmo(npc, NPC.downedMoonlord, 5, 2, 1);

                string key = "Mods.CalamityMod.MoonBossText";
                Color messageColor = Color.Orange;
                string key2 = "Mods.CalamityMod.MoonBossText2";
                Color messageColor2 = Color.Violet;
                string key3 = "Mods.CalamityMod.MoonBossText3";
                Color messageColor3 = Color.Crimson;
                string key4 = "Mods.CalamityMod.ProfanedBossText2";
                Color messageColor4 = Color.Cyan;
                string key5 = "Mods.CalamityMod.FutureOreText";
                Color messageColor5 = Color.LightGray;

                // Spawn Exodium and send messages about Providence, Bloodstone, Phantoplasm, etc. if ML has not been killed yet
                if (!NPC.downedMoonlord)
                {
                    WorldGenerationMethods.SpawnOre(ModContent.TileType<ExodiumOre>(), 12E-05, .01f, .07f);

                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                        Main.NewText(Language.GetTextValue(key2), messageColor2);
                        Main.NewText(Language.GetTextValue(key3), messageColor3);
                        Main.NewText(Language.GetTextValue(key4), messageColor4);
                        Main.NewText(Language.GetTextValue(key5), messageColor5);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key2), messageColor2);
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key3), messageColor3);
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key4), messageColor4);
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key5), messageColor5);
                    }
                }
            }

            return true;
        }
        #endregion

        #region Boss Rush Loot Cancel
        private bool BossRushLootCancel(NPC npc, Mod mod)
        {
            if (npc.type == ModContent.NPCType<ProfanedGuardianBoss>())
            {
                CalamityWorld.bossRushStage = 7;
                CalamityUtils.KillAllHostileProjectiles();
            }
            else if (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail)
            {
                int count = 0;
                for (int j = 0; j < Main.maxNPCs; j++)
                {
                    if (Main.npc[j].active && (Main.npc[j].type == NPCID.EaterofWorldsHead || Main.npc[j].type == NPCID.EaterofWorldsBody || Main.npc[j].type == NPCID.EaterofWorldsTail))
                    {
                        count++;
                        break;
                    }
                }

                if (count < 4)
                {
                    CalamityWorld.bossRushStage = 8;
                    CalamityUtils.KillAllHostileProjectiles();
                }
            }
            else if (npc.type == ModContent.NPCType<AstrumAureus.AstrumAureus>())
            {
                CalamityWorld.bossRushStage = 9;
                CalamityUtils.KillAllHostileProjectiles();
            }
            else if (npc.type == ModContent.NPCType<Bumblefuck>())
            {
                CalamityWorld.bossRushStage = 12;
                CalamityUtils.KillAllHostileProjectiles();
            }
            else if (npc.type == ModContent.NPCType<HiveMindP2>())
            {
                CalamityWorld.bossRushStage = 14;
                CalamityUtils.KillAllHostileProjectiles();
            }
            else if (npc.type == ModContent.NPCType<StormWeaverHeadNaked>())
            {
                CalamityWorld.bossRushStage = 16;
                CalamityUtils.KillAllHostileProjectiles();
            }
            else if (npc.type == ModContent.NPCType<AquaticScourgeHead>())
            {
                CalamityWorld.bossRushStage = 17;
                CalamityUtils.KillAllHostileProjectiles();
            }
            else if (npc.type == ModContent.NPCType<DesertScourgeHead>())
            {
                CalamityWorld.bossRushStage = 18;
                CalamityUtils.KillAllHostileProjectiles();
            }
            else if (npc.type == ModContent.NPCType<CrabulonIdle>())
            {
                CalamityWorld.bossRushStage = 20;
                CalamityUtils.KillAllHostileProjectiles();
            }
            else if (npc.type == ModContent.NPCType<CeaselessVoid.CeaselessVoid>())
            {
                CalamityWorld.bossRushStage = 22;
                CalamityUtils.KillAllHostileProjectiles();
            }
            else if (npc.type == ModContent.NPCType<PerforatorHive>())
            {
                CalamityWorld.bossRushStage = 23;
                CalamityUtils.KillAllHostileProjectiles();
            }
            else if (npc.type == ModContent.NPCType<Cryogen.Cryogen>())
            {
                CalamityWorld.bossRushStage = 24;
                CalamityUtils.KillAllHostileProjectiles();
            }
            else if (npc.type == ModContent.NPCType<BrimstoneElemental.BrimstoneElemental>())
            {
                CalamityWorld.bossRushStage = 25;
                CalamityUtils.KillAllHostileProjectiles();
            }
            else if (npc.type == ModContent.NPCType<Signus.Signus>())
            {
                CalamityWorld.bossRushStage = 26;
                CalamityUtils.KillAllHostileProjectiles();
            }
            else if (npc.type == ModContent.NPCType<RavagerBody>())
            {
                CalamityWorld.bossRushStage = 27;
                CalamityUtils.KillAllHostileProjectiles();
            }
            else if (npc.type == ModContent.NPCType<AstrumDeusHeadSpectral>())
            {
                CalamityWorld.bossRushStage = 30;
                CalamityUtils.KillAllHostileProjectiles();
            }
            else if (npc.type == ModContent.NPCType<Polterghast.Polterghast>())
            {
                CalamityWorld.bossRushStage = 31;
                CalamityUtils.KillAllHostileProjectiles();
            }
            else if (npc.type == ModContent.NPCType<PlaguebringerGoliath.PlaguebringerGoliath>())
            {
                CalamityWorld.bossRushStage = 32;
                CalamityUtils.KillAllHostileProjectiles();
            }
            else if (npc.type == ModContent.NPCType<CalamitasRun3>())
            {
                CalamityWorld.bossRushStage = 33;
                CalamityUtils.KillAllHostileProjectiles();
            }
            else if (npc.type == ModContent.NPCType<Siren>() || npc.type == ModContent.NPCType<Leviathan.Leviathan>())
            {
                int bossType = (npc.type == ModContent.NPCType<Siren>()) ? ModContent.NPCType<Leviathan.Leviathan>() : ModContent.NPCType<Siren>();
                if (!NPC.AnyNPCs(bossType))
                {
                    CalamityWorld.bossRushStage = 34;
                    CalamityUtils.KillAllHostileProjectiles();
                }
            }
            else if (npc.type == ModContent.NPCType<SlimeGodCore>() || npc.type == ModContent.NPCType<SlimeGodSplit>() || npc.type == ModContent.NPCType<SlimeGodRunSplit>())
            {
                if (npc.type == ModContent.NPCType<SlimeGodCore>() && !NPC.AnyNPCs(ModContent.NPCType<SlimeGodSplit>()) && !NPC.AnyNPCs(ModContent.NPCType<SlimeGodRunSplit>()) &&
                    !NPC.AnyNPCs(ModContent.NPCType<SlimeGod.SlimeGod>()) && !NPC.AnyNPCs(ModContent.NPCType<SlimeGodRun>()))
                {
                    CalamityWorld.bossRushStage = 35;
                    CalamityUtils.KillAllHostileProjectiles();
                }
                else if (npc.type == ModContent.NPCType<SlimeGodSplit>() && !NPC.AnyNPCs(ModContent.NPCType<SlimeGodCore>()) && !NPC.AnyNPCs(ModContent.NPCType<SlimeGodRunSplit>()) &&
                    NPC.CountNPCS(ModContent.NPCType<SlimeGodSplit>()) < 2 && !NPC.AnyNPCs(ModContent.NPCType<SlimeGodRun>()))
                {
                    CalamityWorld.bossRushStage = 35;
                    CalamityUtils.KillAllHostileProjectiles();
                }
                else if (npc.type == ModContent.NPCType<SlimeGodRunSplit>() && !NPC.AnyNPCs(ModContent.NPCType<SlimeGodCore>()) && !NPC.AnyNPCs(ModContent.NPCType<SlimeGodSplit>()) &&
                    NPC.CountNPCS(ModContent.NPCType<SlimeGodRunSplit>()) < 2 && !NPC.AnyNPCs(ModContent.NPCType<SlimeGod.SlimeGod>()))
                {
                    CalamityWorld.bossRushStage = 35;
                    CalamityUtils.KillAllHostileProjectiles();
                }
            }
            else if (npc.type == ModContent.NPCType<Providence.Providence>())
            {
                CalamityWorld.bossRushStage = 36;
                CalamityUtils.KillAllHostileProjectiles();

                string key = "Mods.CalamityMod.BossRushTierFourEndText";
                Color messageColor = Color.LightCoral;
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                }
            }
            else if (npc.type == ModContent.NPCType<SupremeCalamitas.SupremeCalamitas>())
            {
                CalamityWorld.bossRushStage = 37;
                CalamityUtils.KillAllHostileProjectiles();
            }
            else if (npc.type == ModContent.NPCType<Yharon.Yharon>())
            {
                CalamityWorld.bossRushStage = 38;
                CalamityUtils.KillAllHostileProjectiles();
            }
            else if (npc.type == ModContent.NPCType<DevourerofGodsHeadS>())
            {
                DropHelper.DropItem(npc, ModContent.ItemType<Rock>(), true);
                CalamityWorld.bossRushStage = 0;
                CalamityUtils.KillAllHostileProjectiles();
                CalamityWorld.bossRushActive = false;

                CalamityMod.UpdateServerBoolean();
                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = mod.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.BossRushStage);
                    netMessage.Write(CalamityWorld.bossRushStage);
                    netMessage.Send();
                }

                string key = "Mods.CalamityMod.BossRushTierFiveEndText";
                Color messageColor = Color.LightCoral;
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                }

                return false;
            }

            switch (npc.type)
            {
                case NPCID.QueenBee:
                    CalamityWorld.bossRushStage = 1;
                    CalamityUtils.KillAllHostileProjectiles();
                    break;

                case NPCID.BrainofCthulhu:
                    CalamityWorld.bossRushStage = 2;
                    CalamityUtils.KillAllHostileProjectiles();
                    break;

                case NPCID.KingSlime:
                    CalamityWorld.bossRushStage = 3;
                    CalamityUtils.KillAllHostileProjectiles();
                    break;

                case NPCID.EyeofCthulhu:
                    CalamityWorld.bossRushStage = 4;
                    CalamityUtils.KillAllHostileProjectiles();
                    break;

                case NPCID.SkeletronPrime:
                    CalamityWorld.bossRushStage = 5;
                    CalamityUtils.KillAllHostileProjectiles();
                    break;

                case NPCID.Golem:
                    CalamityWorld.bossRushStage = 6;
                    CalamityUtils.KillAllHostileProjectiles();
                    break;

                case NPCID.TheDestroyer:
                    CalamityWorld.bossRushStage = 10;
                    CalamityUtils.KillAllHostileProjectiles();

                    string key = "Mods.CalamityMod.BossRushTierOneEndText";
                    Color messageColor = Color.LightCoral;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }

                    break;

                case NPCID.Spazmatism:
                    CalamityWorld.bossRushStage = 11;
                    CalamityUtils.KillAllHostileProjectiles();
                    break;

                case NPCID.Retinazer:
                    CalamityWorld.bossRushStage = 11;
                    CalamityUtils.KillAllHostileProjectiles();
                    break;

                case NPCID.WallofFlesh:
                    CalamityWorld.bossRushStage = 13;
                    CalamityUtils.KillAllHostileProjectiles();
                    break;

                case NPCID.SkeletronHead:
                    CalamityWorld.bossRushStage = 15;
                    CalamityUtils.KillAllHostileProjectiles();
                    break;

                case NPCID.CultistBoss:
                    CalamityWorld.bossRushStage = 19;
                    CalamityUtils.KillAllHostileProjectiles();

                    string key2 = "Mods.CalamityMod.BossRushTierTwoEndText";
                    Color messageColor2 = Color.LightCoral;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(Language.GetTextValue(key2), messageColor2);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key2), messageColor2);
                    }

                    break;

                case NPCID.Plantera:
                    CalamityWorld.bossRushStage = 21;
                    CalamityUtils.KillAllHostileProjectiles();
                    break;

                case NPCID.DukeFishron:
                    CalamityWorld.bossRushStage = 28;
                    CalamityUtils.KillAllHostileProjectiles();

                    string key3 = "Mods.CalamityMod.BossRushTierThreeEndText";
                    Color messageColor3 = Color.LightCoral;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(Language.GetTextValue(key3), messageColor3);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key3), messageColor3);
                    }

                    break;

                case NPCID.MoonLordCore:
                    CalamityWorld.bossRushStage = 29;
                    CalamityUtils.KillAllHostileProjectiles();
                    break;

                default:
                    break;
            }

            if (Main.netMode == NetmodeID.Server)
            {
                var netMessage = mod.GetPacket();
                netMessage.Write((byte)CalamityModMessageType.BossRushStage);
                netMessage.Write(CalamityWorld.bossRushStage);
                netMessage.Send();
            }

            return false;
        }
        #endregion

        #region Abyss Loot Cancel
        private bool AbyssLootCancel(NPC npc, Mod mod)
        {
            int x = Main.maxTilesX;
            int y = Main.maxTilesY;
            int genLimit = x / 2;
            int abyssChasmY = y - 250;
            int abyssChasmX = CalamityWorld.abyssSide ? genLimit - (genLimit - 135) : genLimit + (genLimit - 135);
            bool abyssPosX = false;
            bool abyssPosY = (double)(npc.position.Y / 16f) <= abyssChasmY;

            if (CalamityWorld.abyssSide)
            {
                if ((double)(npc.position.X / 16f) < abyssChasmX + 80)
                {
                    abyssPosX = true;
                }
            }
            else
            {
                if ((double)(npc.position.X / 16f) > abyssChasmX - 80)
                {
                    abyssPosX = true;
                }
            }

            bool hurtByAbyss = npc.wet && npc.damage > 0 && !npc.boss && !npc.friendly && !npc.dontTakeDamage &&
                (((npc.position.Y / 16f > (Main.rockLayer - Main.maxTilesY * 0.05)) &&
                abyssPosY && abyssPosX) || CalamityWorld.abyssTiles > 200) && !npc.buffImmune[ModContent.BuffType<CrushDepth>()];

            return hurtByAbyss;
        }
		#endregion

		#region NPCLoot
		public override void NPCLoot(NPC npc)
        {
            ResetAdrenaline(npc);

            // LATER -- keeping bosses alive lets draedon mayhem continue even after killing mechs
            // Reset Draedon Mayhem to false if no bosses are alive
            if (CalamityGlobalNPC.DraedonMayhem)
            {
                if (!CalamityPlayer.areThereAnyDamnBosses)
                {
                    CalamityGlobalNPC.DraedonMayhem = false;
                    CalamityMod.UpdateServerBoolean();
                }
            }

            if (CalamityWorld.defiled)
                DefiledLoot(npc);
            if (CalamityWorld.armageddon)
                ArmageddonLoot(npc);

            CheckBossSpawn(npc);
            ArmorSetLoot(npc);
            RareLoot(npc);
            RareVariants(npc);
            CommonLoot(npc);
            TownNPCLoot(npc);
            BossLoot(npc);
        }
        #endregion

        #region Defiled Loot
        private void DefiledLoot(NPC npc)
        {
            switch (npc.type)
            {
                case NPCID.Werewolf:
                    DropHelper.DropItemChance(npc, ItemID.MoonCharm, DropHelper.DefiledDropRateInt);
                    DropHelper.DropItemChance(npc, ItemID.AdhesiveBandage, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.AnglerFish:
                    DropHelper.DropItemChance(npc, ItemID.AdhesiveBandage, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.DesertBeast:
                    DropHelper.DropItemChance(npc, ItemID.AncientHorn, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.ArmoredSkeleton:
                    DropHelper.DropItemChance(npc, ItemID.BeamSword, DropHelper.DefiledDropRateInt);
                    DropHelper.DropItemChance(npc, ItemID.ArmorPolish, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.Clown:
                    DropHelper.DropItemChance(npc, ItemID.Bananarang, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.ToxicSludge:
                    DropHelper.DropItemChance(npc, ItemID.Bezoar, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.EyeofCthulhu:
                    DropHelper.DropItemChance(npc, ItemID.Binoculars, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.WanderingEye:
                    DropHelper.DropItemChance(npc, ItemID.BlackLens, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.CorruptSlime:
                    DropHelper.DropItemChance(npc, ItemID.Blindfold, DropHelper.DefiledDropRateInt);
                    break;

                // This is all the random skeletons in the hardmode dungeon
                case 269:
                case 270:
                case 271:
                case 272:
                case 273:
                case 274:
                case 275:
                case 276:
                case 277:
                case 278:
                case 279:
                case 280:
                    DropHelper.DropItemChance(npc, ItemID.Keybrand, DropHelper.DefiledDropRateInt);
                    DropHelper.DropItemChance(npc, ItemID.BoneFeather, DropHelper.DefiledDropRateInt);
                    DropHelper.DropItemChance(npc, ItemID.MagnetSphere, DropHelper.DefiledDropRateInt);
                    DropHelper.DropItemChance(npc, ItemID.WispinaBottle, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.UndeadMiner:
                    DropHelper.DropItemChance(npc, ItemID.BonePickaxe, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.ScutlixRider:
                    DropHelper.DropItemChance(npc, ItemID.BrainScrambler, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.Vampire:
                    DropHelper.DropItemChance(npc, ItemID.BrokenBatWing, DropHelper.DefiledDropRateInt);
                    DropHelper.DropItemChance(npc, ItemID.MoonStone, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.CaveBat:
                    DropHelper.DropItemChance(npc, ItemID.ChainKnife, DropHelper.DefiledDropRateInt);
                    DropHelper.DropItemChance(npc, ItemID.DepthMeter, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.DarkCaster:
                    DropHelper.DropItemChance(npc, ItemID.ClothierVoodooDoll, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.PirateCaptain:
                    DropHelper.DropItemChance(npc, ItemID.CoinGun, DropHelper.DefiledDropRateInt);
                    DropHelper.DropItemChance(npc, ItemID.DiscountCard, DropHelper.DefiledDropRateInt);
                    DropHelper.DropItemChance(npc, ItemID.Cutlass, DropHelper.DefiledDropRateInt);
                    DropHelper.DropItemChance(npc, ItemID.LuckyCoin, DropHelper.DefiledDropRateInt);
                    DropHelper.DropItemChance(npc, ItemID.PirateStaff, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.Reaper:
                    DropHelper.DropItemChance(npc, ItemID.DeathSickle, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.Demon:
                case NPCID.VoodooDemon:
                    DropHelper.DropItemChance(npc, ItemID.DemonScythe, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.DesertDjinn:
                    DropHelper.DropItemChance(npc, ItemID.DjinnLamp, DropHelper.DefiledDropRateInt);
                    DropHelper.DropItemChance(npc, ItemID.DjinnsCurse, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.Shark:
                    DropHelper.DropItemChance(npc, ItemID.DivingHelmet, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.Pixie:
                case NPCID.Wraith:
                case NPCID.Mummy:
                    DropHelper.DropItemChance(npc, ItemID.FastClock, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.RedDevil:
                    DropHelper.DropItemChance(npc, ItemID.FireFeather, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.IceElemental:
                case NPCID.IcyMerman:
                    DropHelper.DropItemChance(npc, ItemID.IceSickle, DropHelper.DefiledDropRateInt);
                    DropHelper.DropItemChance(npc, ItemID.FrostStaff, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.ArmoredViking:
                    DropHelper.DropItemChance(npc, ItemID.IceSickle, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.IceTortoise:
                    DropHelper.DropItemChance(npc, ItemID.IceSickle, DropHelper.DefiledDropRateInt);
                    DropHelper.DropItemChance(npc, ItemID.FrozenTurtleShell, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.Harpy:
                    DropHelper.DropItemCondition(npc, ItemID.GiantHarpyFeather, Main.hardMode, DropHelper.DefiledDropRateFloat);
                    break;

                case NPCID.QueenBee:
                    DropHelper.DropItemChance(npc, ItemID.HoneyedGoggles, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.Piranha:
                    DropHelper.DropItemChance(npc, ItemID.Hook, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.DiabolistRed:
                case NPCID.DiabolistWhite:
                    DropHelper.DropItemChance(npc, ItemID.InfernoFork, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.PinkJellyfish:
                    DropHelper.DropItemChance(npc, ItemID.JellyfishNecklace, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.Paladin:
                    DropHelper.DropItemChance(npc, ItemID.Kraken, DropHelper.DefiledDropRateInt);
                    DropHelper.DropItemChance(npc, ItemID.PaladinsHammer, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.SkeletonArcher:
                    DropHelper.DropItemChance(npc, ItemID.Marrow, DropHelper.DefiledDropRateInt);
                    DropHelper.DropItemChance(npc, ItemID.MagicQuiver, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.Lavabat:
                    DropHelper.DropItemChance(npc, ItemID.MagmaStone, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.WalkingAntlion:
                    DropHelper.DropItemChance(npc, ItemID.AntlionClaw, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.DarkMummy:
                    DropHelper.DropItemChance(npc, ItemID.Blindfold, DropHelper.DefiledDropRateInt);
                    DropHelper.DropItemChance(npc, ItemID.Megaphone, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.GreenJellyfish:
                    DropHelper.DropItemChance(npc, ItemID.Megaphone, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.CursedSkull:
                    DropHelper.DropItemChance(npc, ItemID.Nazar, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.FireImp:
                    DropHelper.DropItemChance(npc, ItemID.ObsidianRose, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.BlackRecluse:
                case NPCID.BlackRecluseWall:
                    DropHelper.DropItemChance(npc, ItemID.PoisonStaff, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.SkeletonSniper:
                    DropHelper.DropItemChance(npc, ItemID.RifleScope, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.ChaosElemental:
                    DropHelper.DropItemChance(npc, ItemID.RodofDiscord, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.Necromancer:
                case NPCID.NecromancerArmored:
                    DropHelper.DropItemChance(npc, ItemID.ShadowbeamStaff, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.SnowFlinx:
                    DropHelper.DropItemChance(npc, ItemID.SnowballLauncher, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.RaggedCaster:
                case NPCID.RaggedCasterOpenCoat:
                    DropHelper.DropItemChance(npc, ItemID.SpectreStaff, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.Plantera:
                    DropHelper.DropItemChance(npc, ItemID.TheAxe, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.GiantBat:
                    DropHelper.DropItemChance(npc, ItemID.TrifoldMap, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.AngryTrapper:
                    DropHelper.DropItemChance(npc, ItemID.Uzi, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.Corruptor:
                case NPCID.FloatyGross:
                    DropHelper.DropItemChance(npc, ItemID.Vitamins, DropHelper.DefiledDropRateInt);
                    break;

                case NPCID.GiantTortoise:
                    DropHelper.DropItemCondition(npc, ItemID.Yelets, NPC.downedMechBossAny, DropHelper.DefiledDropRateFloat);
                    break;

                default:
                    break;
            }

            // Every type of demon eye counts for Black Lenses
            if (CalamityMod.demonEyeList.Contains(npc.type))
            {
                DropHelper.DropItemChance(npc, ItemID.BlackLens, DropHelper.DefiledDropRateInt);
            }

            // Every type of Skeleton counts for the Bone Sword
            if (CalamityMod.skeletonList.Contains(npc.type))
            {
                DropHelper.DropItemChance(npc, ItemID.BoneSword, DropHelper.DefiledDropRateInt);
            }

            // Every type of Angry Bones counts for the Clothier Voodoo Doll
            if (CalamityMod.angryBonesList.Contains(npc.type))
            {
                DropHelper.DropItemChance(npc, ItemID.ClothierVoodooDoll, DropHelper.DefiledDropRateInt);
            }

            // Every type of hornet AND moss hornet can drop Bezoar
            if (CalamityMod.hornetList.Contains(npc.type) || CalamityMod.mossHornetList.Contains(npc.type))
            {
                DropHelper.DropItemChance(npc, ItemID.Bezoar, DropHelper.DefiledDropRateInt);
            }

            // Every type of moss hornet can drop Tattered Bee Wings
            if (CalamityMod.mossHornetList.Contains(npc.type))
            {
                DropHelper.DropItemChance(npc, ItemID.TatteredBeeWing, DropHelper.DefiledDropRateInt);
            }

            // Because all switch cases must be constant at compile time, modded NPC IDs (which can change) can't be included.
            if (npc.type == ModContent.NPCType<SunBat>())
            {
                DropHelper.DropItemChance(npc, ItemID.HelFire, DropHelper.DefiledDropRateInt);
            }
            else if (npc.type == ModContent.NPCType<Cryon>())
            {
                DropHelper.DropItemChance(npc, ItemID.Amarok, DropHelper.DefiledDropRateInt);
            }
        }
        #endregion

        #region Armageddon Loot
        private void ArmageddonLoot(NPC npc)
        {
            switch (npc.type)
            {
                case NPCID.KingSlime:
                case NPCID.EyeofCthulhu:
                case NPCID.EaterofWorldsHead:
                case NPCID.EaterofWorldsBody:
                case NPCID.EaterofWorldsTail:
                    if (npc.boss) // only drop from the 1 "boss" segment (redcode)
                    {
                        DropHelper.DropArmageddonBags(npc);
                    }

                    break;

                case NPCID.BrainofCthulhu:
                case NPCID.QueenBee:
                case NPCID.SkeletronHead:
                case NPCID.WallofFlesh:
                case NPCID.Retinazer: // only drop if spaz is already dead
                    if (!NPC.AnyNPCs(NPCID.Spazmatism))
                    {
                        DropHelper.DropArmageddonBags(npc);
                    }

                    break;

                case NPCID.Spazmatism: // only drop if ret is already dead
                    if (!NPC.AnyNPCs(NPCID.Retinazer))
                    {
                        DropHelper.DropArmageddonBags(npc);
                    }

                    break;

                case NPCID.TheDestroyer:
                case NPCID.SkeletronPrime:
                case NPCID.Plantera:
                case NPCID.Golem:
                case NPCID.DD2Betsy:
                case NPCID.DukeFishron:
                case NPCID.MoonLordCore:
                    DropHelper.DropArmageddonBags(npc);
                    break;

                default:
                    break;
            }
        }
        #endregion

        #region Reset Adrenaline
        private void ResetAdrenaline(NPC npc)
        {
            bool revenge = CalamityWorld.revenge;
            if (npc.boss && revenge)
            {
                if (npc.type != ModContent.NPCType<HiveMind.HiveMind>() && npc.type != ModContent.NPCType<Leviathan.Leviathan>() && npc.type != ModContent.NPCType<Siren>() &&
                    npc.type != ModContent.NPCType<StormWeaverHead>() && npc.type != ModContent.NPCType<StormWeaverBody>() &&
                    npc.type != ModContent.NPCType<StormWeaverTail>() && npc.type != ModContent.NPCType<DevourerofGodsHead>() &&
                    npc.type != ModContent.NPCType<DevourerofGodsBody>() && npc.type != ModContent.NPCType<DevourerofGodsTail>())
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active)
                        {
                            Main.player[Main.myPlayer].Calamity().adrenaline = 0;
                        }
                    }
                }
            }
        }
        #endregion

        #region Check Boss Spawn
        // not really drop code
        private void CheckBossSpawn(NPC npc)
        {
            if ((npc.type == ModContent.NPCType<PhantomSpirit>() || npc.type == ModContent.NPCType<PhantomSpiritS>() || npc.type == ModContent.NPCType<PhantomSpiritM>() ||
                npc.type == ModContent.NPCType<PhantomSpiritL>()) && !NPC.AnyNPCs(ModContent.NPCType<Polterghast.Polterghast>()) && !CalamityWorld.downedPolterghast)
            {
                CalamityMod.ghostKillCount++;
                if (CalamityMod.ghostKillCount == 10)
                {
                    string key = "Mods.CalamityMod.GhostBossText2";
                    Color messageColor = Color.Cyan;

                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                }
                else if (CalamityMod.ghostKillCount == 20)
                {
                    string key = "Mods.CalamityMod.GhostBossText3";
                    Color messageColor = Color.Cyan;

                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                }

                if (CalamityMod.ghostKillCount >= 30 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int lastPlayer = npc.lastInteraction;

                    if (!Main.player[lastPlayer].active || Main.player[lastPlayer].dead)
                    {
                        lastPlayer = npc.FindClosestPlayer();
                    }

                    if (lastPlayer >= 0)
                    {
                        NPC.SpawnOnPlayer(lastPlayer, ModContent.NPCType<Polterghast.Polterghast>());
                        CalamityMod.ghostKillCount = 0;
                    }
                }
            }

            if (NPC.downedPlantBoss && (npc.type == NPCID.SandShark || npc.type == NPCID.SandsharkHallow || npc.type == NPCID.SandsharkCorrupt || npc.type == NPCID.SandsharkCrimson || npc.type == ModContent.NPCType<FusionFeeder>()) && !NPC.AnyNPCs(ModContent.NPCType<GreatSandShark.GreatSandShark>()))
            {
                CalamityMod.sharkKillCount++;
                if (CalamityMod.sharkKillCount == 4)
                {
                    string key = "Mods.CalamityMod.SandSharkText";
                    Color messageColor = Color.Goldenrod;

                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                }
                else if (CalamityMod.sharkKillCount == 8)
                {
                    string key = "Mods.CalamityMod.SandSharkText2";
                    Color messageColor = Color.Goldenrod;

                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                }
                if (CalamityMod.sharkKillCount >= 10 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active)
                    {
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/MaulerRoar"),
                            (int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y);
                    }

                    int lastPlayer = npc.lastInteraction;

                    if (!Main.player[lastPlayer].active || Main.player[lastPlayer].dead)
                    {
                        lastPlayer = npc.FindClosestPlayer();
                    }

                    if (lastPlayer >= 0)
                    {
                        NPC.SpawnOnPlayer(lastPlayer, ModContent.NPCType<GreatSandShark.GreatSandShark>());
                        CalamityMod.sharkKillCount = -5;
                    }
                }
            }

            if (NPC.downedAncientCultist && !CalamityWorld.downedStarGod && npc.type == ModContent.NPCType<Atlas>() && !NPC.AnyNPCs(ModContent.NPCType<AstrumDeusHeadSpectral>()))
            {
                CalamityMod.astralKillCount++;
                if (CalamityMod.astralKillCount == 1)
                {
                    string key = "Mods.CalamityMod.DeusText2";
                    Color messageColor = Color.Gold;

                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                }
                else if (CalamityMod.astralKillCount == 2)
                {
                    string key = "Mods.CalamityMod.DeusText3";
                    Color messageColor = Color.Gold;

                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                }
                if (CalamityMod.astralKillCount >= 3 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int lastPlayer = npc.lastInteraction;

                    if (!Main.player[lastPlayer].active || Main.player[lastPlayer].dead)
                    {
                        lastPlayer = npc.FindClosestPlayer();
                    }

                    if (lastPlayer >= 0)
                    {
                        CalamityWorld.ChangeTime(false);

                        for (int x = 0; x < 10; x++)
                        {
                            NPC.SpawnOnPlayer(lastPlayer, ModContent.NPCType<AstrumDeusHead>());
                        }

                        NPC.SpawnOnPlayer(lastPlayer, ModContent.NPCType<AstrumDeusHeadSpectral>());
                        CalamityMod.astralKillCount = 0;
                    }
                }
            }
        }
        #endregion

        #region Armor Set Loot
        private void ArmorSetLoot(NPC npc)
        {
            // Tarragon armor set bonus: 20% chance to drop hearts from all valid enemies
            if (Main.player[(int)Player.FindClosest(npc.position, npc.width, npc.height)].Calamity().tarraSet)
            {
                if (!npc.SpawnedFromStatue && (npc.damage > 5 || npc.boss) && npc.lifeMax > 100)
                {
                    DropHelper.DropItemChance(npc, ItemID.Heart, 5);
                }
            }

            // Blood Orb drops: Valid enemy during a blood moon on the Surface
            if (!npc.SpawnedFromStatue && (npc.damage > 5 || npc.boss) && Main.bloodMoon && npc.HasPlayerTarget && npc.position.Y / 16D < Main.worldSurface)
            {
                if (Main.player[Player.FindClosest(npc.position, npc.width, npc.height)].Calamity().bloodflareSet)
                {
                    DropHelper.DropItemChance(npc, ModContent.ItemType<BloodOrb>(), 2); // 50% chance of 1 orb with Bloodflare
                }

                // 1/12 chance to get a Blood Orb with or without Bloodflare
                DropHelper.DropItemChance(npc, ModContent.ItemType<BloodOrb>(), 12);
            }
        }
        #endregion

        #region Rare Loot
        private void RareLoot(NPC npc)
        {
            switch (npc.type)
            {
                case NPCID.Drippler:
                    int eyeballDropRate = CalamityWorld.defiled ? DropHelper.DefiledDropRateInt : 300;
                    DropHelper.DropItemChance(npc, ModContent.ItemType<BouncingEyeball>(), eyeballDropRate, 1, 1);
                    break;

                case NPCID.PossessedArmor:
                    int amuletDropRate = CalamityWorld.defiled ? DropHelper.DefiledDropRateInt : Main.expertMode ? 150 : 200;
                    DropHelper.DropItemChance(npc, ModContent.ItemType<PsychoticAmulet>(), amuletDropRate, 1, 1);
                    break;

                case NPCID.SeaSnail:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<SeaShell>(), Main.expertMode ? 2 : 3);
                    break;

                case NPCID.GreekSkeleton:
                    int gladiatorDropRate = Main.expertMode ? 15 : 20;
                    DropHelper.DropItemChance(npc, ItemID.GladiatorHelmet, gladiatorDropRate);
                    DropHelper.DropItemChance(npc, ItemID.GladiatorBreastplate, gladiatorDropRate);
                    DropHelper.DropItemChance(npc, ItemID.GladiatorLeggings, gladiatorDropRate);
                    break;

                case NPCID.GiantTortoise:
                    int tortoiseDropRate = Main.expertMode ? 5 : 7;
                    float shellRoll = Main.rand.NextFloat();
                    bool fabledShell = shellRoll < 0.005f; // Exact 1/200 chance for rare regardless of difficulty
                    DropHelper.DropItemCondition(npc, ModContent.ItemType<GiantTortoiseShell>(), !fabledShell, tortoiseDropRate, 1, 1);
                    DropHelper.DropItemCondition(npc, ModContent.ItemType<FabledTortoiseShell>(), fabledShell, tortoiseDropRate, 1, 1);
                    break;

                case NPCID.GiantShelly:
                case NPCID.GiantShelly2:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<GiantShell>(), Main.expertMode ? 5 : 7);
                    break;

                case NPCID.AnomuraFungus:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<FungalCarapace>(), Main.expertMode ? 5 : 7);
                    break;

                case NPCID.Crawdad:
                case NPCID.Crawdad2:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<CrawCarapace>(), Main.expertMode ? 5 : 7);
                    break;

                case NPCID.GreenJellyfish:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<VitalJelly>(), Main.expertMode ? 5 : 7);
                    break;

                case NPCID.PinkJellyfish:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<LifeJelly>(), Main.expertMode ? 5 : 7);
                    break;

                case NPCID.BlueJellyfish:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<ManaJelly>(), Main.expertMode ? 5 : 7);
                    break;

                case NPCID.DarkCaster:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<AncientShiv>(), Main.expertMode ? 20 : 25);
                    break;

                case NPCID.BigMimicHallow:
                case NPCID.BigMimicCorruption:
                case NPCID.BigMimicCrimson:
                case NPCID.BigMimicJungle: // arguably unnecessary
                    DropHelper.DropItemChance(npc, ModContent.ItemType<CelestialClaymore>(), Main.expertMode ? 5 : 7);
                    break;

                case NPCID.Clinger:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<CursedDagger>(), Main.expertMode ? 20 : 25);
                    break;

                case NPCID.Shark:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<DepthBlade>(), Main.expertMode ? 10 : 15);
                    DropHelper.DropItemChance(npc, ItemID.SharkToothNecklace, Main.expertMode ? 20 : 30);
                    break;

                case NPCID.PresentMimic:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<HolidayHalberd>(), Main.expertMode ? 5 : 7);
                    break;

                case NPCID.IchorSticker:
                    int spearDropRate = Main.expertMode ? 20 : 25;
                    float spearRoll = Main.rand.NextFloat();
                    bool spearOfDestiny = spearRoll < 0.005f; // Exact 1/200 chance for rare regardless of difficulty
                    DropHelper.DropItemCondition(npc, ModContent.ItemType<IchorSpear>(), !spearOfDestiny, spearDropRate, 1, 1);
                    DropHelper.DropItemCondition(npc, ModContent.ItemType<SpearofDestiny>(), spearOfDestiny, spearDropRate, 1, 1);
                    break;

                case NPCID.Harpy:
                    int glazeDropRate = CalamityWorld.defiled ? 20 : Main.expertMode ? 60 : 80;
                    DropHelper.DropItemCondition(npc, ModContent.ItemType<SkyGlaze>(), NPC.downedBoss1, glazeDropRate, 1, 1);
                    break;

                case NPCID.Antlion:
                case NPCID.WalkingAntlion:
                case NPCID.FlyingAntlion:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<MandibleClaws>(), Main.expertMode ? 30 : 40);
                    DropHelper.DropItemChance(npc, ModContent.ItemType<MandibleBow>(), Main.expertMode ? 30 : 40);
                    break;

                case NPCID.TombCrawlerHead:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<BurntSienna>(), Main.expertMode ? 15 : 20);
                    break;

                case NPCID.DuneSplicerHead:
                    DropHelper.DropItemCondition(npc, ModContent.ItemType<Terracotta>(), NPC.downedPlantBoss, Main.expertMode ? 20 : 30, 1, 1);
                    break;

                case NPCID.MartianSaucerCore:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<NullificationRifle>(), Main.expertMode ? 5 : 7);
                    break;

                case NPCID.Demon:
                case NPCID.VoodooDemon:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<DemonicBoneAsh>(), Main.expertMode ? 2 : 3);
                    DropHelper.DropItemChance(npc, ModContent.ItemType<BladecrestOathsword>(), Main.expertMode ? 20 : 25);
                    break;

                case NPCID.BoneSerpentHead:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<DemonicBoneAsh>(), Main.expertMode ? 2 : 3);
                    DropHelper.DropItemChance(npc, ModContent.ItemType<OldLordOathsword>(), Main.expertMode ? 10 : 15);
                    break;

                case NPCID.Tim:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<PlasmaRod>(), Main.expertMode ? 2 : 3);
                    break;

                case NPCID.GoblinSorcerer:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<PlasmaRod>(), Main.expertMode ? 20 : 25);
                    break;

                case NPCID.PirateDeadeye:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<ProporsePistol>(), Main.expertMode ? 20 : 25);
                    break;

                case NPCID.PirateCrossbower:
                    int crossbowDropRate = Main.expertMode ? 20 : 25;
                    float arbalestRoll = Main.rand.NextFloat();
                    bool arbalest = arbalestRoll < 0.005f; // Exact 1/200 chance for rare regardless of difficulty
                    DropHelper.DropItemCondition(npc, ModContent.ItemType<RaidersGlory>(), !arbalest, crossbowDropRate, 1, 1);
                    DropHelper.DropItemCondition(npc, ModContent.ItemType<Arbalest>(), arbalest, crossbowDropRate, 1, 1);
                    break;

                case NPCID.GoblinSummoner:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<TheFirstShadowflame>(), Main.expertMode ? 5 : 7);
                    break;

                case NPCID.SandElemental:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<WifeinaBottle>(), Main.expertMode ? 5 : 7);
                    DropHelper.DropItemCondition(npc, ModContent.ItemType<WifeinaBottlewithBoobs>(), Main.expertMode, 20, 1, 1);
                    break;

                case NPCID.GoblinWarrior:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<Warblade>(), Main.expertMode ? 15 : 20);
                    break;

                case NPCID.MartianWalker:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<Wingman>(), Main.expertMode ? 5 : 7);
                    break;

                case NPCID.GiantCursedSkull:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<WrathoftheAncients>(), Main.expertMode ? 20 : 25);
                    DropHelper.DropItemCondition(npc, ModContent.ItemType<Keelhaul>(), CalamityWorld.downedLeviathan, 10, 1, 1);
                    break;

                case NPCID.Necromancer:
                case NPCID.NecromancerArmored:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<WrathoftheAncients>(), Main.expertMode ? 20 : 25);
                    break;

                default:
                    break;
            }

            // Every type of Moss Hornet counts for the Needler
            if (CalamityMod.mossHornetList.Contains(npc.type))
            {
                int needlerDropRate = Main.expertMode ? 20 : 25;
                DropHelper.DropItemChance(npc, ModContent.ItemType<Needler>(), needlerDropRate);
            }

            // Every type of Skeleton counts for the Waraxe and Ancient Bone Dust
            if (CalamityMod.skeletonList.Contains(npc.type))
            {
                DropHelper.DropItemCondition(npc, ModContent.ItemType<Waraxe>(), !Main.hardMode, Main.expertMode ? 15 : 20, 1, 1);
                DropHelper.DropItemChance(npc, ModContent.ItemType<AncientBoneDust>(), Main.expertMode ? 4 : 5);
            }
        }
        #endregion

        #region Rare Variants
        private void RareVariants(NPC npc)
        {
            switch (npc.type)
            {
                case NPCID.BloodZombie:
                    DropHelper.DropItemCondition(npc, ModContent.ItemType<Carnage>(), NPC.downedBoss3 && !npc.SpawnedFromStatue, 200, 1, 1);
                    break;

                case NPCID.TacticalSkeleton:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<TrueConferenceCall>(), 200);
                    break;

                case NPCID.DesertBeast:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<EvilSmasher>(), 200);
                    break;

                case NPCID.DungeonSpirit:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<PearlGod>(), 200);
                    break;

                case NPCID.RuneWizard:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<EyeofMagnus>(), 10);
                    break;

                case NPCID.Mimic:
                    DropHelper.DropItemCondition(npc, ModContent.ItemType<TheBee>(), !npc.SpawnedFromStatue, 100, 1, 1);
                    break;

                default:
                    break;
            }
        }
        #endregion

        #region Common Loot
        private void CommonLoot(NPC npc)
        {
            switch (npc.type)
            {
                case NPCID.Vulture:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<DesertFeather>(), 2, 1, Main.expertMode ? 2 : 1);
                    break;

                case NPCID.RedDevil:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<EssenceofChaos>(), Main.expertMode ? 1f : 0.5f);
                    break;

                case NPCID.WyvernHead:
                    DropHelper.DropItem(npc, ModContent.ItemType<EssenceofCinder>(), 1, Main.expertMode ? 2 : 1);
                    break;

                case NPCID.AngryNimbus:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<EssenceofCinder>(), Main.expertMode ? 2 : 3);
                    break;

                case NPCID.IcyMerman:
                case NPCID.IceTortoise:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<EssenceofEleum>(), Main.expertMode ? 2 : 3);
                    break;

                case NPCID.IceGolem:
                    DropHelper.DropItem(npc, ModContent.ItemType<EssenceofEleum>(), 1, 2);
                    break;

                case NPCID.Plantera:
                    DropHelper.DropItemCondition(npc, ModContent.ItemType<LivingShard>(), !Main.expertMode, 6, 9);
                    break;

                case NPCID.SolarSpearman: //Drakanian
                case NPCID.SolarSolenian: //Selenian
                case NPCID.SolarCorite:
                case NPCID.SolarSroller:
                case NPCID.SolarDrakomireRider:
                case NPCID.SolarDrakomire:
                case NPCID.SolarCrawltipedeHead:
                    DropHelper.DropItemChance(npc, ItemID.FragmentSolar, Main.expertMode ? 4 : 5);
                    break;

                case NPCID.VortexSoldier: //Vortexian
                case NPCID.VortexLarva: //Alien Larva
                case NPCID.VortexHornet: //Alien Hornet
                case NPCID.VortexHornetQueen: //Alien Queen
                case NPCID.VortexRifleman: //Storm Diver
                    DropHelper.DropItemChance(npc, ItemID.FragmentVortex, Main.expertMode ? 4 : 5);
                    break;

                case NPCID.NebulaBrain: //Nebula Floater
                case NPCID.NebulaSoldier: //Predictor
                case NPCID.NebulaHeadcrab: //Brain Suckler
                case NPCID.NebulaBeast: //Evolution Beast
                    DropHelper.DropItemChance(npc, ModContent.ItemType<MeldBlob>(), 4, Main.expertMode ? 2 : 1, Main.expertMode ? 3 : 2);
                    DropHelper.DropItemChance(npc, ItemID.FragmentNebula, Main.expertMode ? 4 : 5);
                    break;

                case NPCID.StardustSoldier: //Stargazer
                case NPCID.StardustSpiderBig: //Twinkle Popper
                case NPCID.StardustJellyfishBig: //Flow Invader
                case NPCID.StardustCellBig: //Star Cell
                case NPCID.StardustWormHead: //Milkyway Weaver
                    DropHelper.DropItemChance(npc, ItemID.FragmentStardust, Main.expertMode ? 4 : 5);
                    break;

                case NPCID.DungeonGuardian:
                    DropHelper.DropItemCondition(npc, ModContent.ItemType<GoldBurdenBreaker>(), Main.hardMode);
                    break;

                case NPCID.CultistBoss:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<StardustStaff>(), Main.expertMode ? 3 : 5);
                    DropHelper.DropItemChance(npc, ModContent.ItemType<ThornBlossom>(), DropHelper.RareVariantDropRateInt);
                    break;

                case NPCID.EyeofCthulhu:
                    DropHelper.DropItemCondition(npc, ModContent.ItemType<VictoryShard>(), !Main.expertMode, 2, 4);
                    DropHelper.DropItemCondition(npc, ModContent.ItemType<TeardropCleaver>(), !Main.expertMode, 5, 1, 1);
                    break;

                case NPCID.DevourerHead:
                case NPCID.SeekerHead:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<FetidEssence>(), Main.expertMode ? 2 : 3);
                    break;

                case NPCID.FaceMonster:
                case NPCID.Herpling:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<BloodlettingEssence>(), Main.expertMode ? 4 : 5);
                    break;

                case NPCID.ManEater:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<ManeaterBulb>(), Main.expertMode ? 2 : 3);
                    break;

                case NPCID.AngryTrapper:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<TrapperBulb>(), Main.expertMode ? 4 : 5);
                    break;

                case NPCID.MotherSlime:
                case NPCID.CorruptSlime:
                case NPCID.Crimslime:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<MurkySludge>(), Main.expertMode ? 3 : 4);
                    break;

                case NPCID.Moth:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<GypsyPowder>(), Main.expertMode ? 1f : 0.5f);
                    break;

                case NPCID.Derpling:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<BeetleJuice>(), Main.expertMode ? 4 : 5);
                    break;

                case NPCID.SpikedJungleSlime:
                case NPCID.Arapaima:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<MurkyPaste>(), Main.expertMode ? 4 : 5);
                    break;

                case NPCID.Reaper:
                case NPCID.Psycho:
                    DropHelper.DropItemCondition(npc, ModContent.ItemType<SolarVeil>(), (CalamityWorld.downedCalamitas || NPC.downedPlantBoss), Main.expertMode ? 0.75f : 0.5f, 1, 4);
                    DropHelper.DropItemCondition(npc, ModContent.ItemType<DarksunFragment>(), CalamityWorld.downedBuffedMothron, Main.expertMode ? 0.06f : 0.04f, 1, 1);
                    break;

				//other solar eclipse creatures
                case NPCID.Eyezor:
                case NPCID.Frankenstein:
                case NPCID.SwampThing:
                case NPCID.Vampire:
                case NPCID.VampireBat:
                case NPCID.CreatureFromTheDeep:
                case NPCID.Fritz:
                case NPCID.ThePossessed:
                case NPCID.Butcher:
                case NPCID.DeadlySphere:
                case NPCID.DrManFly:
                case NPCID.Nailhead:
                    DropHelper.DropItemCondition(npc, ModContent.ItemType<DarksunFragment>(), CalamityWorld.downedBuffedMothron, Main.expertMode ? 0.06f : 0.04f, 1, 1);
                    break;

                default:
                    break;
            }

            // All hardmode dungeon enemies drop Ectoblood
            if (CalamityMod.dungeonEnemyBuffList.Contains(npc.type))
            {
                DropHelper.DropItemChance(npc, ModContent.ItemType<Ectoblood>(), 2, 1, Main.expertMode ? 3 : 1);
            }
        }
        #endregion

        #region Town NPC Loot
        private void TownNPCLoot(NPC npc)
        {
            const float TrasherEatDistance = 48f;

            if (npc.type == NPCID.Angler)
            {
                bool fedToTrasher = false;
                for(int i = 0; i < Main.maxNPCs; ++i)
                {
                    NPC nearby = Main.npc[i];
                    if (!nearby.active || nearby.type != ModContent.NPCType<Trasher>())
                        continue;
                    if (npc.Distance(nearby.Center) < TrasherEatDistance)
                    {
                        fedToTrasher = true;
                        break;
                    }
                }

                if (fedToTrasher)
                    DropHelper.DropItemCondition(npc, ItemID.GoldenFishingRod, Main.hardMode);
                else
                    DropHelper.DropItemCondition(npc, ItemID.GoldenFishingRod, Main.hardMode, 12, 1, 1);
            }
        }
        #endregion

        #region Boss Loot
        private void BossLoot(NPC npc)
        {
            // Not really loot code, but NPCLoot is the only death hook
            if (npc.boss && !CalamityWorld.downedBossAny)
            {
                CalamityWorld.downedBossAny = true;
                CalamityMod.UpdateServerBoolean();
            }

            // Nightmare Fuel, Endothermic Energy and Darksun Fragments
            if (npc.type == NPCID.Pumpking)
            {
                DropHelper.DropItemCondition(npc, ModContent.ItemType<NightmareFuel>(), CalamityWorld.downedDoG, 10, 20);
            }
            else if (npc.type == NPCID.IceQueen)
            {
                DropHelper.DropItemCondition(npc, ModContent.ItemType<EndothermicEnergy>(), CalamityWorld.downedDoG, 20, 40);
            }
            else if (npc.type == NPCID.Mothron && CalamityWorld.buffedEclipse)
            {
                DropHelper.DropItem(npc, ModContent.ItemType<DarksunFragment>(), 10, 20);

                // Mark a buffed Mothron as killed (allowing access to Yharon P2)
                CalamityWorld.downedBuffedMothron = true;
                CalamityMod.UpdateServerBoolean();
            }
        }
        #endregion
    }
}

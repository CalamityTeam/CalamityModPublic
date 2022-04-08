using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.SulphurousSea;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Tiles.Ores;
using CalamityMod.World;
using CalamityMod.World.Planets;
using Microsoft.Xna.Framework;
using System.Threading;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs
{
    public partial class CalamityGlobalNPC : GlobalNPC
    {
        #region PreNPCLoot
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            // TODO - Make this work so that split worm segments don't drop loot.
            /*if (CalamityWorld.death && !SplittingWormLootBlockWrapper(npc, Mod))
                return false;*/

            // TODO - Make this work in the future.
            // Do not provide free hearts for certain boss NPCs in Rev+.
            /*if (CalamityWorld.revenge && CalamityLists.heartDropBlockList.Contains(npc.type))
                return false;*/

            // TODO - Make this work in the future.
            // Block loot from non-boss EoW segments and Creepers, their drops will drop from the Treasure Bag in Rev+.
            // This is to prevent heart drops until the boss is dead.
            /*if (CalamityWorld.revenge)
            {
                if (npc.boss && (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail))
                {
                    int[] EoWDrops = new int[]
                    {
                        ItemID.DemoniteOre,
                        ItemID.ShadowScale
                    };
                    DropHelper.BlockDrops(EoWDrops);
                }

                if (npc.type == NPCID.Creeper || (!npc.boss && (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail)))
                    return false;
            }*/

            //
            // Ozzatron 17FEB2021: A NOTE about PreNPCLoot vs NPCLoot
            // PreNPCLoot runs before the boss is marked as dead. This means it is required for lore items and Resident Evil ammo.
            // Because we already have clauses here for every boss, it is more convenient to drop everything here than it is.
            // to iterate through all the bosses twice.
            //

            // Determine whether this NPC is the second Twin killed in a fight, regardless of which Twin it is.
            bool lastTwinStanding = false;
            if (npc.type == NPCID.Retinazer)
                lastTwinStanding = !NPC.AnyNPCs(NPCID.Spazmatism);
            else if (npc.type == NPCID.Spazmatism)
                lastTwinStanding = !NPC.AnyNPCs(NPCID.Retinazer);

            // Mechanical Bosses' combined lore item
            bool mechLore = !NPC.downedMechBossAny && (lastTwinStanding || npc.type == NPCID.TheDestroyer || npc.type == NPCID.SkeletronPrime);

            switch (npc.type)
            {
                case NPCID.GreenJellyfish:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<VitalJelly>(), 7, 4));
                    break;

                case NPCID.Vulture:
                    npcLoot.Add(ModContent.ItemType<DesertFeather>(), 1, 1, 2);
                    break;

                case NPCID.RedDevil:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<DemonicBoneAsh>(), 3, 2));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<EssenceofChaos>(), 2, 1));
                    break;

                case NPCID.WyvernHead:
                    npcLoot.Add(ModContent.ItemType<EssenceofCinder>(), 1, 1, 2);
                    break;

                case NPCID.AngryNimbus:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<EssenceofCinder>(), 2, 1));
                    break;

                case NPCID.IcyMerman:
                case NPCID.IceTortoise:
                case NPCID.IceElemental:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<EssenceofEleum>(), 2, 1));
                    break;

                case NPCID.IceGolem:
                    npcLoot.Add(ModContent.ItemType<EssenceofEleum>(), 1, 1, 2);
                    break;

                case NPCID.SolarSpearman: //Drakanian
                case NPCID.SolarSolenian: //Selenian
                case NPCID.SolarCorite:
                case NPCID.SolarSroller:
                case NPCID.SolarDrakomireRider:
                case NPCID.SolarDrakomire:
                case NPCID.SolarCrawltipedeHead:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<MeldBlob>(), 5, 4));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.FragmentSolar, 5, 4));
                    break;

                case NPCID.VortexSoldier: //Vortexian
                case NPCID.VortexLarva: //Alien Larva
                case NPCID.VortexHornet: //Alien Hornet
                case NPCID.VortexHornetQueen: //Alien Queen
                case NPCID.VortexRifleman: //Storm Diver
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<MeldBlob>(), 5, 4));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.FragmentVortex, 5, 4));
                    break;

                case NPCID.NebulaBrain: //Nebula Floater
                case NPCID.NebulaSoldier: //Predictor
                case NPCID.NebulaHeadcrab: //Brain Suckler
                case NPCID.NebulaBeast: //Evolution Beast
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<MeldBlob>(), 5, 4));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.FragmentNebula, 5, 4));
                    break;

                case NPCID.StardustSoldier: //Stargazer
                case NPCID.StardustSpiderBig: //Twinkle Popper
                case NPCID.StardustJellyfishBig: //Flow Invader
                case NPCID.StardustCellBig: //Star Cell
                case NPCID.StardustWormHead: //Milkyway Weaver
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<MeldBlob>(), 5, 4));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.FragmentStardust, 5, 4));
                    break;

                case NPCID.AngryTrapper:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<TrapperBulb>(), 2, 1));
                    break;

                case NPCID.Derpling:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<BeetleJuice>(), 3, 2));
                    break;

                case NPCID.SpikedJungleSlime:
                case NPCID.Arapaima:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<MurkyPaste>(), 3, 2));
                    break;

                case NPCID.Reaper:
                case NPCID.Psycho:
                    npcLoot.AddIf(() => !DownedBossSystem.downedCalamitas || NPC.downedPlantBoss, ModContent.ItemType<SolarVeil>(), 2, 2, 4);
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<DarksunFragment>(), 2);
                    break;

                case NPCID.MartianOfficer:
                    npcLoot.Add(ModContent.ItemType<ShockGrenade>(), 3, 3, 8);
                    break;

                case NPCID.BrainScrambler:
                case NPCID.GrayGrunt:
                case NPCID.GigaZapper:
                case NPCID.MartianEngineer:
                case NPCID.RayGunner:
                case NPCID.ScutlixRider:
                    npcLoot.Add(ModContent.ItemType<ShockGrenade>(), 4, 1, 4);
                    break;

                case NPCID.Gastropod:
                    npcLoot.Add(ItemID.PinkGel, 1, 5, 10);
                    break;

                case NPCID.Drippler:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<BouncingEyeball>(), 40, 20));
                    break;

                case NPCID.FireImp:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<AshenStalactite>(), 10, 6));
                    break;

                case NPCID.PossessedArmor:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<PsychoticAmulet>(), 40, 20));
                    break;

                case NPCID.VampireBat:
                case NPCID.Vampire:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.BatHook, 40, 20));
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<DarksunFragment>(), 2);
                    break;

                case NPCID.SeaSnail:
                    npcLoot.Add(ModContent.ItemType<SeaShell>());
                    break;

                case NPCID.GreekSkeleton:
                    // Calamity increases the drop chance of the Gladiator's set because it's actually a set
                    float gladiatorDropRate = Main.expertMode ? 0.16f : 0.1f;
                    int[] gladiatorArmor = new int[]
                    {
                        ItemID.GladiatorHelmet,
                        ItemID.GladiatorBreastplate,
                        ItemID.GladiatorLeggings,
                    };
                    npcLoot.Add(ItemDropRule.OneFromOptions(6, gladiatorArmor));
                    DropHelper.BlockDrops(gladiatorArmor);
                    break;

                case NPCID.GiantTortoise:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<GiantTortoiseShell>(), 7, 4));
                    break;

                case NPCID.GiantShelly:
                case NPCID.GiantShelly2:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<GiantShell>(), 7, 4));
                    break;

                case NPCID.AnomuraFungus:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<FungalCarapace>(), 7, 4));
                    break;

                case NPCID.Crawdad:
                case NPCID.Crawdad2:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<CrawCarapace>(), 7, 4));
                    break;

                case NPCID.PinkJellyfish:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<LifeJelly>(), 7, 4));
                    break;

                case NPCID.BlueJellyfish:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<ManaJelly>(), 7, 4));
                    break;

                case NPCID.DarkCaster:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<AncientShiv>(), 25, 15));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<ShinobiBlade>(), 25, 15));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<StaffOfNecrosteocytes>(), 25, 15));
                    break;

                case NPCID.BigMimicHallow:
                case NPCID.BigMimicCorruption:
                case NPCID.BigMimicCrimson:
                case NPCID.BigMimicJungle: // arguably unnecessary
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<CelestialClaymore>(), 7, 4));
                    break;

                case NPCID.Clinger:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<CursedDagger>(), 25, 15));
                    break;

                case NPCID.Shark:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.SharkToothNecklace, 25, 15));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<JoyfulHeart>(), 25, 15));
                    break;

                case NPCID.IchorSticker:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<IchorSpear>(), 25, 15));
                    break;

                case NPCID.Harpy:
                    npcLoot.AddIf(() => NPC.downedBoss1, ModContent.ItemType<SkyGlaze>(), 30);
                    npcLoot.AddIf(() => Main.hardMode && !npc.SpawnedFromStatue, ModContent.ItemType<EssenceofCinder>(), 2);
                    break;

                case NPCID.Antlion:
                case NPCID.WalkingAntlion:
                case NPCID.FlyingAntlion:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<MandibleClaws>(), 50, 30));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<MandibleBow>(), 50, 30));
                    break;

                case NPCID.TombCrawlerHead:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<BurntSienna>(), 25, 15));
                    break;

                case NPCID.DuneSplicerHead:
                    npcLoot.AddIf(() => NPC.downedPlantBoss, ModContent.ItemType<Terracotta>(), 15);
                    break;

                case NPCID.MartianSaucerCore:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<NullificationRifle>(), 7, 4));
                    break;

                case NPCID.Demon:
                case NPCID.VoodooDemon:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<DemonicBoneAsh>(), 3, 2));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<BladecrestOathsword>(), 25, 15));
                    break;

                case NPCID.BoneSerpentHead:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<DemonicBoneAsh>(), 3, 2));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<OldLordOathsword>(), 12, 7));
                    break;

                case NPCID.Tim:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<PlasmaRod>(), 3, 2));
                    break;

                case NPCID.GoblinSorcerer:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<PlasmaRod>(), 25, 15));
                    break;

                case NPCID.PirateDeadeye:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<ProporsePistol>(), 25, 15));
                    break;

                case NPCID.PirateCrossbower:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<RaidersGlory>(), 25, 15));
                    break;

                case NPCID.GoblinSummoner:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<TheFirstShadowflame>(), 5, 3));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<BurningStrife>(), 5, 3));
                    break;

                case NPCID.SandElemental:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<WifeinaBottle>(), 5, 3));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<WifeinaBottlewithBoobs>(), 10, 6));
                    break;

                case NPCID.GoblinWarrior:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Warblade>(), 25, 15));
                    break;

                case NPCID.MartianWalker:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Wingman>(), 7, 4));
                    break;

                case NPCID.GiantCursedSkull:
                    npcLoot.AddIf(() => DownedBossSystem.downedLeviathan, ModContent.ItemType<Keelhaul>(), 15);
                    break;

                case NPCID.Necromancer:
                case NPCID.NecromancerArmored:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<WrathoftheAncients>(), 25, 15));
                    break;

                case NPCID.DeadlySphere:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<DefectiveSphere>(), 5, 3));
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<DarksunFragment>(), 2);
                    break;

                case NPCID.BloodJelly:
                case NPCID.FungoFish:
                    npcLoot.Add(ItemID.JellyfishNecklace, 100);
                    break;

                case NPCID.Goldfish:
                case NPCID.GoldfishWalker:
                    npcLoot.Add(ModContent.ItemType<PineapplePet>(), 500);
                    break;

                case NPCID.Nutcracker:
                case NPCID.NutcrackerSpinning:
                case NPCID.ElfCopter:
                case NPCID.Flocko:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<EndothermicEnergy>(), 2);
                    break;

                case NPCID.Krampus:
                case NPCID.Yeti:
                case NPCID.PresentMimic:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<EndothermicEnergy>(), 2, 1, 2);
                    break;

                case NPCID.Everscream:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<EndothermicEnergy>(), 1, 3, 5);
                    break;

                case NPCID.SantaNK1:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<EndothermicEnergy>(), 1, 5, 10);
                    break;

                case NPCID.IceQueen:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<EndothermicEnergy>(), 1, 10, 20);
                    break;

                case NPCID.Splinterling:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<NightmareFuel>(), 2);
                    break;

                case NPCID.Hellhound:
                case NPCID.Poltergeist:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<NightmareFuel>(), 2, 1, 2);
                    break;

                case NPCID.HeadlessHorseman:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<NightmareFuel>(), 1, 3, 5);
                    break;

                case NPCID.MourningWood:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<NightmareFuel>(), 1, 5, 10);
                    break;

                case NPCID.Pumpking:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<NightmareFuel>(), 1, 10, 20);
                    break;

                case NPCID.Frankenstein:
                case NPCID.SwampThing:
                case NPCID.Fritz:
                case NPCID.CreatureFromTheDeep:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<DarksunFragment>(), 10);
                    break;

                case NPCID.ThePossessed:
                case NPCID.Butcher:
                case NPCID.DrManFly:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<DarksunFragment>(), 2);
                    break;

                case NPCID.Eyezor:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<DarksunFragment>(), 1, 1, 2);
                    break;

                case NPCID.Nailhead:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<DarksunFragment>(), 1, 3, 5);
                    break;

                case NPCID.Mothron:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<DarksunFragment>(), 1, 20, 30);
                    break;

                case NPCID.Clothier:
                    npcLoot.AddIf(() => Main.hardMode, ModContent.ItemType<ClothiersWrath>());
                    break;

                case NPCID.KingSlime:
                    // TODO - Spray doesn't work at the moment and should be fixed in the future.
                    // Drop a huge spray of Gel items
                    // More gel is not dropped on Expert because he has more minions, which increases the amount of gel provided.
                    int minGel = 72;
                    int maxGel = 100;
                    //DropHelper.DropItemSpray(npc, ItemID.Gel, minGel, maxGel, 4);
                    npcLoot.Add(ItemID.Gel, 1, minGel, maxGel);
                    npcLoot.AddNormalOnly(ItemID.RoyalGel);
                    npcLoot.AddNormalOnly(ModContent.ItemType<CrownJewel>(), 10);
                    npcLoot.AddIf(() => !NPC.downedSlimeKing, ModContent.ItemType<KnowledgeKingSlime>());
                    break;

                case NPCID.EyeofCthulhu:
                    npcLoot.AddNormalOnly(ItemID.EoCShield);
                    npcLoot.AddNormalOnly(ModContent.ItemType<TeardropCleaver>(), 10);
                    npcLoot.AddNormalOnly(ModContent.ItemType<DeathstareRod>(), 4);
                    npcLoot.AddIf(() => !NPC.downedBoss1, ModContent.ItemType<KnowledgeEyeofCthulhu>());
                    break;

                case NPCID.EaterofWorldsHead:
                case NPCID.EaterofWorldsBody:
                case NPCID.EaterofWorldsTail:
                    npcLoot.AddIf(() => npc.boss && !Main.expertMode, ItemID.WormScarf);
                    npcLoot.AddIf(() => !WorldGen.crimson && !NPC.downedBoss2, ModContent.ItemType<KnowledgeCorruption>());
                    npcLoot.AddIf(() => !WorldGen.crimson && !NPC.downedBoss2, ModContent.ItemType<KnowledgeEaterofWorlds>());
                    npcLoot.AddIf(() => WorldGen.crimson && !NPC.downedBoss2, ModContent.ItemType<KnowledgeCrimson>());
                    npcLoot.AddIf(() => WorldGen.crimson && !NPC.downedBoss2, ModContent.ItemType<KnowledgeBrainofCthulhu>());
                    break;

                case NPCID.BrainofCthulhu:
                    npcLoot.AddNormalOnly(ItemID.BrainOfConfusion);
                    npcLoot.AddIf(() => !WorldGen.crimson && !NPC.downedBoss2, ModContent.ItemType<KnowledgeCorruption>());
                    npcLoot.AddIf(() => !WorldGen.crimson && !NPC.downedBoss2, ModContent.ItemType<KnowledgeEaterofWorlds>());
                    npcLoot.AddIf(() => WorldGen.crimson && !NPC.downedBoss2, ModContent.ItemType<KnowledgeCrimson>());
                    npcLoot.AddIf(() => WorldGen.crimson && !NPC.downedBoss2, ModContent.ItemType<KnowledgeBrainofCthulhu>());
                    break;

                case NPCID.QueenBee:
                    // Drop weapons Calamity style instead of mutually exclusive.
                    var normalOnly = npcLoot.DefineNormalOnlyDropSet();
                    {
                        int[] queenBeeWeapons = new int[]
                        {
                        ItemID.BeeKeeper,
                        ItemID.BeesKnees,
                        ItemID.BeeGun
                        };
                        // It's already 33.33% in vanilla normal, so we shouldn't lower the drop rate to 25%
                        normalOnly.Add(ItemDropRule.OneFromOptions(DropHelper.BagWeaponDropRateInt, queenBeeWeapons));
                        DropHelper.BlockDrops(queenBeeWeapons);
                    }
                    npcLoot.AddNormalOnly(ItemID.HiveBackpack);
                    npcLoot.AddNormalOnly(ModContent.ItemType<TheBee>(), 10);
                    npcLoot.AddNormalOnly(ItemID.Stinger, 1, 5, 10);
                    npcLoot.AddNormalOnly(ModContent.ItemType<HardenedHoneycomb>(), 1, 30, 50);
                    npcLoot.AddIf(() => !NPC.downedQueenBee, ModContent.ItemType<KnowledgeQueenBee>());
                    break;

                case NPCID.SkeletronHead:
                    npcLoot.Add(ItemID.Bone, 1, 70, 100);
                    npcLoot.AddNormalOnly(ItemID.BoneGlove);
                    // TODO - Item sprays currently don't work.
                    //DropHelper.DropItemSpray(npc, ItemID.Bone, 70, 100, 5);
                    npcLoot.AddIf(() => !NPC.downedBoss3, ModContent.ItemType<KnowledgeSkeletron>());
                    break;

                case NPCID.WallofFlesh:
                    var normalOnly2 = npcLoot.DefineNormalOnlyDropSet();
                    {
                        // Drop weapons Calamity style instead of mutually exclusive -- this includes Calamity weapons.
                        int[] wofWeapons = new int[]
                        {
                            ItemID.BreakerBlade,
                            ItemID.ClockworkAssaultRifle,
                            ModContent.ItemType<Meowthrower>(),
                            ItemID.LaserRifle,
                            ModContent.ItemType<BlackHawkRemote>(),
                            ModContent.ItemType<BlastBarrel>()
                        };
                        normalOnly2.Add(ItemDropRule.OneFromOptions(DropHelper.NormalWeaponDropRateInt, wofWeapons));
                        DropHelper.BlockDrops(wofWeapons);

                        // Drop emblems Calamity style instead of mutually exclusive -- this includes the Rogue Emblem.
                        int[] emblems = new int[]
                        {
                            ItemID.WarriorEmblem,
                            ItemID.RangerEmblem,
                            ItemID.SorcererEmblem,
                            ItemID.SummonerEmblem,
                            ModContent.ItemType<RogueEmblem>()
                        };
                        normalOnly2.Add(ItemDropRule.OneFromOptions(DropHelper.NormalWeaponDropRateInt, emblems));
                        DropHelper.BlockDrops(emblems);
                    }

                    npcLoot.AddNormalOnly(ModContent.ItemType<Carnage>(), 10);

                    // Drop Hermit's Box directly for EACH player, regardles of Expert or not. 100% chance on first kill, 10% chance afterwards.
                    npcLoot.AddIf(() => !Main.hardMode, ModContent.ItemType<IbarakiBox>());
                    npcLoot.AddIf(() => Main.hardMode, ModContent.ItemType<IbarakiBox>(), 10);
                    npcLoot.Add(ItemID.CorruptionKey, 3);
                    npcLoot.Add(ItemID.CrimsonKey, 3);
                    npcLoot.AddIf(() => !Main.hardMode, ModContent.ItemType<KnowledgeUnderworld>());
                    npcLoot.AddIf(() => !Main.hardMode, ModContent.ItemType<KnowledgeWallofFlesh>());
                    break;

                case NPCID.TheDestroyer:
                    // Only drop hallowed bars after all mechs are down
                    if ((!NPC.downedMechBoss1 || !NPC.downedMechBoss2 || !NPC.downedMechBoss3) && CalamityConfig.Instance.EarlyHardmodeProgressionRework)
                        DropHelper.BlockDrops(ItemID.HallowedBar);

                    npcLoot.AddNormalOnly(ItemID.MechanicalWagonPiece);
                    npcLoot.AddIf(() => !NPC.downedMechBoss1, ModContent.ItemType<KnowledgeDestroyer>());
                    npcLoot.AddIf(() => mechLore, ModContent.ItemType<KnowledgeMechs>());
                    break;

                case NPCID.Spazmatism:
                case NPCID.Retinazer:
                    // Only drop hallowed bars after all mechs are down
                    if ((!NPC.downedMechBoss1 || !NPC.downedMechBoss2 || !NPC.downedMechBoss3) && CalamityConfig.Instance.EarlyHardmodeProgressionRework && lastTwinStanding)
                        DropHelper.BlockDrops(ItemID.HallowedBar);

                    npcLoot.AddIf(() => !Main.expertMode && lastTwinStanding, ItemID.MechanicalWheelPiece);
                    npcLoot.AddIf(() => !Main.expertMode && lastTwinStanding, ModContent.ItemType<Arbalest>(), 10);
                    npcLoot.AddIf(() => !NPC.downedMechBoss2 && lastTwinStanding, ModContent.ItemType<KnowledgeTwins>());
                    npcLoot.AddIf(() => mechLore, ModContent.ItemType<KnowledgeMechs>());
                    break;

                case NPCID.SkeletronPrime:
                    // Only drop hallowed bars after all mechs are down
                    if ((!NPC.downedMechBoss1 || !NPC.downedMechBoss2 || !NPC.downedMechBoss3) && CalamityConfig.Instance.EarlyHardmodeProgressionRework)
                        DropHelper.BlockDrops(ItemID.HallowedBar);

                    npcLoot.AddNormalOnly(ItemID.MechanicalBatteryPiece);
                    npcLoot.AddIf(() => !NPC.downedMechBoss3, ModContent.ItemType<KnowledgeSkeletronPrime>());
                    npcLoot.AddIf(() => mechLore, ModContent.ItemType<KnowledgeMechs>());
                    break;

                case NPCID.Plantera:
                    // Drop weapons Calamity style instead of mutually exclusive.
                    var normalOnly3 = npcLoot.DefineNormalOnlyDropSet();
                    {
                        int[] planteraWeapons = new int[]
                        {
                            ItemID.FlowerPow,
                            ItemID.Seedler,
                            ItemID.GrenadeLauncher,
                            ItemID.VenusMagnum,
                            ItemID.LeafBlower,
                            ItemID.NettleBurst,
                            ItemID.WaspGun,
                            ModContent.ItemType<BloomStone>()
                        };
                        normalOnly3.Add(ItemDropRule.OneFromOptions(DropHelper.NormalWeaponDropRateInt, planteraWeapons));
                        DropHelper.BlockDrops(planteraWeapons);
                    }
                    npcLoot.AddNormalOnly(ItemID.SporeSac);
                    npcLoot.AddNormalOnly(ModContent.ItemType<BlossomFlux>(), 10);
                    npcLoot.AddNormalOnly(ModContent.ItemType<LivingShard>(), 1, 12, 18);
                    npcLoot.Add(ItemID.JungleKey, 3);
                    npcLoot.AddIf(() => !NPC.downedPlantBoss, ModContent.ItemType<KnowledgePlantera>());
                    break;

                case NPCID.Golem:
                    // Drop loot Calamity style instead of mutually exclusive.
                    var normalOnly4 = npcLoot.DefineNormalOnlyDropSet();
                    {
                        int[] golemItems = new int[]
                        {
                            ItemID.GolemFist,
                            ItemID.PossessedHatchet,
                            ItemID.Stynger,
                            ItemID.HeatRay,
                            ItemID.StaffofEarth,
                            ItemID.EyeoftheGolem,
                            ItemID.SunStone
                        };
                        normalOnly4.Add(ItemDropRule.OneFromOptions(DropHelper.NormalWeaponDropRateInt, golemItems));
                        DropHelper.BlockDrops(golemItems);
                    }

                    npcLoot.AddNormalOnly(ItemID.ShinyStone);
                    npcLoot.AddNormalOnly(ModContent.ItemType<AegisBlade>(), 10);

                    // If Golem has never been killed, provide a Picksaw to all players. This only applies in Normal Mode.
                    // The Golem Treasure Bag is guaranteed to provide a Picksaw if one is not yet in the inventory.
                    npcLoot.AddIf(() => !NPC.downedGolemBoss && !Main.expertMode, ItemID.Picksaw);

                    npcLoot.AddNormalOnly(ModContent.ItemType<EssenceofCinder>(), 1, 5, 10);
                    npcLoot.AddIf(() => !NPC.downedGolemBoss, ModContent.ItemType<KnowledgeGolem>());
                    break;

                case NPCID.DD2Betsy:
                    // Drop weapons Calamity style instead of mutually exclusive.
                    var normalOnly5 = npcLoot.DefineNormalOnlyDropSet();
                    {
                        int[] betsyWeapons = new int[]
                        {
                            ItemID.DD2SquireBetsySword, // Flying Dragon
                            ItemID.MonkStaffT3, // Sky Dragon's Fury
                            ItemID.DD2BetsyBow, // Aerial Bane
                            ItemID.ApprenticeStaffT3, // Betsy's Wrath
                        };
                        normalOnly5.Add(ItemDropRule.OneFromOptions(DropHelper.NormalWeaponDropRateInt, betsyWeapons));
                        DropHelper.BlockDrops(betsyWeapons);
                    }
                    break;

                case NPCID.DukeFishron:
                    // Drop weapons Calamity style instead of mutually exclusive -- this includes Calamity weapons.
                    var normalOnly6 = npcLoot.DefineNormalOnlyDropSet();
                    {
                        int[] dukeWeapons = new int[]
                        {
                            ItemID.Flairon,
                            ItemID.Tsunami,
                            ItemID.BubbleGun,
                            ItemID.RazorbladeTyphoon,
                            ItemID.TempestStaff,
                            ItemID.FishronWings,
                            ModContent.ItemType<DukesDecapitator>()
                        };
                        normalOnly6.Add(ItemDropRule.OneFromOptions(DropHelper.NormalWeaponDropRateInt, dukeWeapons));
                        DropHelper.BlockDrops(dukeWeapons);
                    }
                    npcLoot.AddNormalOnly(ItemID.ShrimpyTruffle);
                    npcLoot.AddNormalOnly(ModContent.ItemType<BrinyBaron>(), 10);
                    npcLoot.AddIf(() => !NPC.downedFishron, ModContent.ItemType<KnowledgeDukeFishron>());
                    break;

                case NPCID.CultistBoss:
                    npcLoot.AddIf(() => !NPC.downedAncientCultist, ModContent.ItemType<KnowledgeLunaticCultist>());
                    npcLoot.AddIf(() => Main.bloodMoon, ModContent.ItemType<KnowledgeBloodMoon>());
                    break;

                case NPCID.MoonLordCore:
                    // Drop weapons Calamity style instead of mutually exclusive -- this includes Calamity weapons.
                    var normalOnly7 = npcLoot.DefineNormalOnlyDropSet();
                    {
                        int[] moonLordWeapons = new int[]
                        {
                            ItemID.Meowmere,
                            ItemID.StarWrath,
                            ItemID.Terrarian,
                            ItemID.FireworksLauncher, // Celebration
                            ItemID.Celeb2, // Celebration Mk2
                            ItemID.SDMG,
                            ItemID.LastPrism,
                            ItemID.LunarFlareBook,
                            ItemID.MoonlordTurretStaff, // Lunar Portal Staff
                            ItemID.RainbowCrystalStaff,
                            ModContent.ItemType<UtensilPoker>(),
                        };
                        normalOnly7.Add(ItemDropRule.OneFromOptions(DropHelper.NormalWeaponDropRateInt, moonLordWeapons));
                        DropHelper.BlockDrops(moonLordWeapons);
                    }
                    npcLoot.AddNormalOnly(ItemID.GravityGlobe);
                    npcLoot.AddNormalOnly(ItemID.SuspiciousLookingTentacle);
                    npcLoot.AddNormalOnly(ModContent.ItemType<MLGRune2>());
                    npcLoot.AddIf(() => !NPC.downedMoonlord, ModContent.ItemType<KnowledgeMoonLord>());
                    break;

                default:
                    break;
            }

            // All hardmode dungeon enemies have a 20% chance to drop 1 ectoplasm
            if (CalamityLists.dungeonEnemyBuffList.Contains(npc.type))
                npcLoot.Add(ItemID.Ectoplasm, 5);

            // Every type of moss hornet can drop stingers
            if (CalamityLists.mossHornetList.Contains(npc.type))
                npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.Stinger, 2, 1));

            // Every type of Moss Hornet counts for the Needler
            if (CalamityLists.mossHornetList.Contains(npc.type))
                npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Needler>(), 25, 15));

            // Every type of Skeleton counts for the Waraxe and Ancient Bone Dust
            if (CalamityLists.skeletonList.Contains(npc.type))
            {
                npcLoot.AddIf(() => !Main.hardMode, ModContent.ItemType<Waraxe>(), 15);
                npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<AncientBoneDust>(), 5, 3));
            }

            if (npc.type == NPCID.RedDevil)
            {
                DropHelper.DropItemChance(npc, ItemID.FireFeather, 0.1f);
                DropHelper.BlockDrops(ItemID.FireFeather);
            }
            else if (npc.type == NPCID.Vampire || npc.type == NPCID.VampireBat)
            {
                DropHelper.DropItemChance(npc, ItemID.MoonStone, 0.15f);
                DropHelper.BlockDrops(ItemID.MoonStone);
            }
            else if (npc.type == NPCID.Werewolf)
            {
                DropHelper.DropItemChance(npc, ItemID.MoonCharm, 0.05f);
                DropHelper.BlockDrops(ItemID.MoonCharm);
            }
            else if (npc.type == NPCID.Mimic && !npc.SpawnedFromStatue)
            {
                float w = DropHelper.BagWeaponDropRateFloat;
                // Change to NPCID.IceMimic in 1.4 TMod
                if (npc.ai[3] == 4f)
                {
                    DropHelper.DropEntireWeightedSet(npc,
                        DropHelper.WeightStack(ItemID.Frostbrand, w),
                        DropHelper.WeightStack(ItemID.IceBow, w),
                        DropHelper.WeightStack(ItemID.FlowerofFrost, w),
                        DropHelper.WeightStack(ItemID.ToySled, 0.05f)
                    );
                }
                else
                {
                    DropHelper.DropEntireWeightedSet(npc,
                        DropHelper.WeightStack(ItemID.StarCloak, w),
                        DropHelper.WeightStack(ItemID.CrossNecklace, w),
                        DropHelper.WeightStack(ItemID.TitanGlove, w),
                        DropHelper.WeightStack(ItemID.DualHook, w),
                        DropHelper.WeightStack(ItemID.MagicDagger, w),
                        DropHelper.WeightStack(ItemID.PhilosophersStone, w)
                    );
                }

                int[] mimicDrops = new int[]
                {
                    ItemID.StarCloak,
                    ItemID.CrossNecklace,
                    ItemID.TitanGlove,
                    ItemID.DualHook,
                    ItemID.MagicDagger,
                    ItemID.PhilosophersStone,
                    ItemID.Frostbrand,
                    ItemID.IceBow,
                    ItemID.FlowerofFrost,
                    ItemID.ToySled
                };
                DropHelper.BlockDrops(mimicDrops);
            }
            else if (npc.type == NPCID.Moth)
            {
                DropHelper.DropItem(npc, ItemID.ButterflyDust);
                DropHelper.BlockDrops(ItemID.ButterflyDust);
            }
            else if (npc.type >= NPCID.RustyArmoredBonesAxe && npc.type <= NPCID.HellArmoredBonesSword)
            {
                DropHelper.DropItemChance(npc, ItemID.WispinaBottle, 0.005f);
                DropHelper.BlockDrops(ItemID.WispinaBottle);
            }
            else if (npc.type == NPCID.Paladin)
            {
                DropHelper.DropItemChance(npc, ItemID.PaladinsHammer, 0.15f);
                DropHelper.DropItemChance(npc, ItemID.PaladinsShield, 0.2f);

                int[] paladinDrops = new int[]
                {
                    ItemID.PaladinsHammer,
                    ItemID.PaladinsShield
                };
                DropHelper.BlockDrops(paladinDrops);
            }
            else if (npc.type == NPCID.BoneLee)
            {
                DropHelper.DropItemChance(npc, ItemID.BlackBelt, 0.25f);
                DropHelper.DropItemChance(npc, ItemID.Tabi, 0.25f);

                int[] boneLeeDrops = new int[]
                {
                    ItemID.BlackBelt,
                    ItemID.Tabi
                };
                DropHelper.BlockDrops(boneLeeDrops);
            }
        }
        #endregion

        public override void ModifyGlobalLoot(GlobalLoot globalLoot)
        {
            // Tarragon armor set bonus: 20% chance to drop hearts from all valid enemies
            if (Main.player[Player.FindClosest(npc.position, npc.width, npc.height)].Calamity().tarraSet)
            {
                if (!npc.SpawnedFromStatue && (npc.damage > 5 || npc.boss) && npc.lifeMax > 100)
                {
                    DropHelper.DropItemChance(npc, ItemID.Heart, 5);
                }
            }

            // Blood Orb drops: Valid enemy during a blood moon on the Surface
            if (!npc.SpawnedFromStatue && (npc.damage > 5 || npc.boss) && Main.bloodMoon && npc.HasPlayerTarget && npc.position.Y / 16D < Main.worldSurface)
            {
                if (Main.player[Player.FindClosest(npc.Center, npc.width, npc.height)].Calamity().bloodflareSet)
                {
                    DropHelper.DropItemChance(npc, ModContent.ItemType<BloodOrb>(), 2); // 50% chance of 1 orb with Bloodflare
                }

                // 20% chance to get a Blood Orb with or without Bloodflare
                DropHelper.DropItemChance(npc, ModContent.ItemType<BloodOrb>(), 5);
            }
        }

        public override void OnKill(NPC npc)
        {
            // No bosses drop loot in Boss Rush. Progress the event instead.
            if (BossRushEvent.BossRushActive)
                BossRushEvent.OnBossKill(npc, Mod);

            CheckBossSpawn(npc);

            if (CalamityWorld.rainingAcid)
                AcidRainEvent.OnEnemyKill(npc);

            // Determine whether this NPC is the second Twin killed in a fight, regardless of which Twin it is.
            bool lastTwinStanding = false;
            if (npc.type == NPCID.Retinazer)
                lastTwinStanding = !NPC.AnyNPCs(NPCID.Spazmatism);
            else if (npc.type == NPCID.Spazmatism)
                lastTwinStanding = !NPC.AnyNPCs(NPCID.Retinazer);

            if ((npc.boss && (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail)) || npc.type == NPCID.BrainofCthulhu)
            {
                SetNewShopVariable(new int[] { NPCID.Merchant, NPCID.ArmsDealer, NPCID.Dryad, NPCID.Demolitionist }, NPC.downedBoss2);
                SetNewBossJustDowned(npc);
            }

            switch (npc.type)
            {
                case NPCID.KingSlime:
                    SetNewShopVariable(new int[] { NPCID.Dryad }, NPC.downedSlimeKing);
                    SetNewBossJustDowned(npc);
                    break;

                case NPCID.EyeofCthulhu:
                    SetNewShopVariable(new int[] { NPCID.Merchant, NPCID.Dryad, NPCID.Demolitionist }, NPC.downedBoss1);
                    SetNewBossJustDowned(npc);
                    break;

                case NPCID.QueenBee:
                    SetNewShopVariable(new int[] { NPCID.ArmsDealer, NPCID.Dryad }, NPC.downedQueenBee);
                    SetNewBossJustDowned(npc);
                    break;

                case NPCID.SkeletronHead:
                    SetNewShopVariable(new int[] { NPCID.Merchant, NPCID.Dryad, NPCID.Demolitionist }, NPC.downedBoss3);
                    SetNewBossJustDowned(npc);
                    break;

                case NPCID.WallofFlesh:
                    SetNewShopVariable(new int[] { NPCID.Merchant, NPCID.ArmsDealer, NPCID.Dryad, NPCID.Painter, NPCID.WitchDoctor, NPCID.Stylist, NPCID.DyeTrader, NPCID.Demolitionist, NPCID.PartyGirl, NPCID.Clothier, NPCID.SkeletonMerchant, ModContent.NPCType<THIEF>() }, Main.hardMode);
                    SetNewBossJustDowned(npc);

                    if (!Main.hardMode)
                    {
                        // Increase altar count to allow natural mech boss spawning.
                        if (CalamityConfig.Instance.EarlyHardmodeProgressionRework)
                            WorldGen.altarCount++;

                        string key2 = "Mods.CalamityMod.UglyBossText";
                        Color messageColor2 = Color.Aquamarine;
                        CalamityUtils.DisplayLocalizedText(key2, messageColor2);

                        if (CalamityConfig.Instance.EarlyHardmodeProgressionRework)
                        {
                            string key3 = "Mods.CalamityMod.HardmodeOreTier1Text";
                            Color messageColor3 = new Color(50, 255, 130);
                            CalamityUtils.SpawnOre(TileID.Cobalt, 12E-05, 0.4f, 0.6f, 3, 8);
                            CalamityUtils.SpawnOre(TileID.Palladium, 12E-05, 0.4f, 0.6f, 3, 8);
                            CalamityUtils.DisplayLocalizedText(key3, messageColor3);
                        }
                    }
                    break;

                case NPCID.TheDestroyer:
                    SetNewShopVariable(new int[] { NPCID.DD2Bartender, NPCID.Stylist, NPCID.Truffle, ModContent.NPCType<THIEF>() }, NPC.downedMechBossAny);
                    SetNewShopVariable(new int[] { NPCID.Stylist, ModContent.NPCType<DILF>(), ModContent.NPCType<FAP>(), ModContent.NPCType<THIEF>() }, NPC.downedMechBoss1 || !NPC.downedMechBoss2 || !NPC.downedMechBoss3);
                    SetNewBossJustDowned(npc);

                    if (!NPC.downedMechBoss1 && CalamityConfig.Instance.EarlyHardmodeProgressionRework)
                        SpawnMechBossHardmodeOres();
                    break;

                case NPCID.Spazmatism:
                case NPCID.Retinazer:
                    SetNewShopVariable(new int[] { NPCID.DD2Bartender, NPCID.Stylist, NPCID.Truffle, ModContent.NPCType<THIEF>() }, NPC.downedMechBossAny);
                    SetNewShopVariable(new int[] { NPCID.Stylist, ModContent.NPCType<DILF>(), ModContent.NPCType<FAP>(), ModContent.NPCType<THIEF>() }, !NPC.downedMechBoss1 || NPC.downedMechBoss2 || !NPC.downedMechBoss3);
                    SetNewBossJustDowned(npc);

                    if (!NPC.downedMechBoss2 && CalamityConfig.Instance.EarlyHardmodeProgressionRework)
                        SpawnMechBossHardmodeOres();
                    break;

                case NPCID.SkeletronPrime:
                    SetNewShopVariable(new int[] { NPCID.DD2Bartender, NPCID.Stylist, NPCID.Truffle, ModContent.NPCType<THIEF>() }, NPC.downedMechBossAny);
                    SetNewShopVariable(new int[] { NPCID.Stylist, ModContent.NPCType<DILF>(), ModContent.NPCType<FAP>(), ModContent.NPCType<THIEF>() }, !NPC.downedMechBoss1 || !NPC.downedMechBoss2 || NPC.downedMechBoss3);
                    SetNewBossJustDowned(npc);

                    if (!NPC.downedMechBoss3 && CalamityConfig.Instance.EarlyHardmodeProgressionRework)
                        SpawnMechBossHardmodeOres();
                    break;

                case NPCID.Plantera:
                    SetNewShopVariable(new int[] { NPCID.WitchDoctor, NPCID.Truffle, ModContent.NPCType<FAP>(), ModContent.NPCType<THIEF>() }, NPC.downedPlantBoss);
                    SetNewBossJustDowned(npc);

                    // Spawn Perennial Ore if Plantera has never been killed
                    if (!NPC.downedPlantBoss)
                    {
                        string key = "Mods.CalamityMod.PlantOreText";
                        Color messageColor = Color.GreenYellow;
                        string key2 = "Mods.CalamityMod.SandSharkText3";
                        Color messageColor2 = Color.Goldenrod;

                        CalamityUtils.SpawnOre(ModContent.TileType<PerennialOre>(), 12E-05, 0.5f, 0.7f, 3, 8, TileID.Dirt, TileID.Stone);

                        CalamityUtils.DisplayLocalizedText(key, messageColor);
                        CalamityUtils.DisplayLocalizedText(key2, messageColor2);
                    }
                    break;

                case NPCID.Everscream:
                    SetNewShopVariable(new int[] { ModContent.NPCType<DILF>() }, NPC.downedChristmasTree || !NPC.downedChristmasSantank || !NPC.downedChristmasIceQueen);
                    break;

                case NPCID.SantaNK1:
                    SetNewShopVariable(new int[] { ModContent.NPCType<DILF>() }, !NPC.downedChristmasTree || NPC.downedChristmasSantank || !NPC.downedChristmasIceQueen);
                    break;

                case NPCID.IceQueen:
                    SetNewShopVariable(new int[] { ModContent.NPCType<DILF>() }, !NPC.downedChristmasTree || !NPC.downedChristmasSantank || NPC.downedChristmasIceQueen);
                    break;

                case NPCID.Golem:
                    SetNewShopVariable(new int[] { NPCID.ArmsDealer, NPCID.Cyborg, NPCID.Steampunker, NPCID.Wizard, NPCID.WitchDoctor, NPCID.DD2Bartender, ModContent.NPCType<FAP>(), ModContent.NPCType<THIEF>() }, NPC.downedGolemBoss);
                    SetNewBossJustDowned(npc);

                    // If Golem has never been killed, send a message about the Plague.
                    if (!NPC.downedGolemBoss)
                    {
                        string key3 = "Mods.CalamityMod.BabyBossText";
                        Color messageColor3 = Color.Lime;

                        CalamityUtils.DisplayLocalizedText(key3, messageColor3);
                    }
                    break;

                case NPCID.DD2Betsy:
                    // Mark Betsy as dead (Vanilla does not keep track of her)
                    DownedBossSystem.downedBetsy = true;
                    CalamityNetcode.SyncWorld();
                    break;

                case NPCID.DukeFishron:
                    SetNewBossJustDowned(npc);
                    break;

                case NPCID.CultistBoss:
                    SetNewBossJustDowned(npc);

                    // Deus text (this is not a loot function)
                    if (!NPC.downedAncientCultist)
                    {
                        string key4 = "Mods.CalamityMod.DeusText";
                        Color messageColor4 = Color.Gold;

                        CalamityUtils.DisplayLocalizedText(key4, messageColor4);
                    }
                    break;

                case NPCID.MoonLordCore:
                    SetNewShopVariable(new int[] { ModContent.NPCType<THIEF>() }, NPC.downedMoonlord);
                    SetNewBossJustDowned(npc);

                    string key5 = "Mods.CalamityMod.MoonBossText";
                    Color messageColor5 = Color.Orange;
                    string key6 = "Mods.CalamityMod.MoonBossText2";
                    Color messageColor6 = Color.Violet;
                    string key7 = "Mods.CalamityMod.ProfanedBossText2";
                    Color messageColor7 = Color.Cyan;
                    string key8 = "Mods.CalamityMod.FutureOreText";
                    Color messageColor8 = Color.LightGray;

                    if (!CalamityWorld.HasGeneratedLuminitePlanetoids)
                    {
                        // Generate luminite planetoids.
                        // This operation is done on a separate thread to lighten the load on servers so that they
                        // can focus on more critical operations asychronously and ideally avoid a time-out crash.
                        // Very few operations in Terraria utilize the pool, so it is highly unlikely that no threads will remain in it.
                        ThreadPool.QueueUserWorkItem(_ => LuminitePlanet.GenerateLuminitePlanetoids());

                        CalamityWorld.HasGeneratedLuminitePlanetoids = true;

                        // If the moon lord is already marked as dead, an associated world sync packet will not be sent automatically
                        // Send one manually.
                        if (NPC.downedMoonlord)
                            CalamityNetcode.SyncWorld();
                    }

                    // Spawn Exodium planetoids and send messages about Providence, Bloodstone, Phantoplasm, etc. if ML has not been killed yet
                    if (!NPC.downedMoonlord)
                    {
                        CalamityUtils.DisplayLocalizedText(key5, messageColor5);
                        CalamityUtils.DisplayLocalizedText(key6, messageColor6);
                        CalamityUtils.DisplayLocalizedText(key7, messageColor7);
                        CalamityUtils.DisplayLocalizedText(key8, messageColor8);
                    }
                    break;
            }
        }

        #region Spawn Mech Boss Hardmode Ores
        private void SpawnMechBossHardmodeOres()
        {
            if (!NPC.downedMechBossAny)
            {
                string key = "Mods.CalamityMod.HardmodeOreTier2Text";
                Color messageColor = new Color(50, 255, 130);
                CalamityUtils.SpawnOre(TileID.Mythril, 12E-05, 0.5f, 0.7f, 3, 8);
                CalamityUtils.SpawnOre(TileID.Orichalcum, 12E-05, 0.5f, 0.7f, 3, 8);
                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }
            else if ((!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1))
            {
                string key = "Mods.CalamityMod.HardmodeOreTier3Text";
                Color messageColor = new Color(50, 255, 130);
                CalamityUtils.SpawnOre(TileID.Adamantite, 12E-05, 0.6f, 0.8f, 3, 8);
                CalamityUtils.SpawnOre(TileID.Titanium, 12E-05, 0.6f, 0.8f, 3, 8);
                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }
            else
            {
                string key = "Mods.CalamityMod.HardmodeOreTier4Text";
                Color messageColor = new Color(50, 255, 130);
                CalamityUtils.SpawnOre(ModContent.TileType<HallowedOre>(), 12E-05, 0.45f, 0.8f, 3, 8, TileID.Pearlstone, TileID.HallowHardenedSand, TileID.HallowSandstone, TileID.HallowedIce);
                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }
        }
        #endregion

        #region Splitting Worm Loot
        internal static bool SplittingWormLootBlockWrapper(NPC npc, Mod mod)
        {
            if (!CalamityWorld.death)
                return true;

            switch (npc.type)
            {
                case NPCID.DiggerHead:
                case NPCID.DiggerBody:
                case NPCID.DiggerTail:
                    return SplittingWormLoot(npc, mod, 0);
                case NPCID.SeekerHead:
                case NPCID.SeekerBody:
                case NPCID.SeekerTail:
                    return SplittingWormLoot(npc, mod, 1);
                case NPCID.DuneSplicerHead:
                case NPCID.DuneSplicerBody:
                case NPCID.DuneSplicerTail:
                    return SplittingWormLoot(npc, mod, 2);
                default:
                    return true;
            }
        }

        internal static bool SplittingWormLoot(NPC npc, Mod mod, int wormType)
        {
            switch (wormType)
            {
                case 0: return CheckSegments(NPCID.DiggerHead, NPCID.DiggerBody, NPCID.DiggerTail);
                case 1: return CheckSegments(NPCID.SeekerHead, NPCID.SeekerBody, NPCID.SeekerTail);
                case 2: return CheckSegments(NPCID.DuneSplicerHead, NPCID.DuneSplicerBody, NPCID.DuneSplicerTail);
                default:
                    break;
            }

            bool CheckSegments(int head, int body, int tail)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (i != npc.whoAmI && Main.npc[i].active && (Main.npc[i].type == head || Main.npc[i].type == body || Main.npc[i].type == tail))
                    {
                        return false;
                    }
                }
                return true;
            }

            return true;
        }
        #endregion

        #region NPCLoot
        public override void NPCLoot(NPC npc)
        {
            TownNPCLoot(npc);
        }
        #endregion

        #region Check Boss Spawn
        // not really drop code
        private void CheckBossSpawn(NPC npc)
        {
            if ((npc.type == ModContent.NPCType<PhantomSpirit>() || npc.type == ModContent.NPCType<PhantomSpiritS>() || npc.type == ModContent.NPCType<PhantomSpiritM>() ||
                npc.type == ModContent.NPCType<PhantomSpiritL>()) && !NPC.AnyNPCs(ModContent.NPCType<Polterghast.Polterghast>()) && !DownedBossSystem.downedPolterghast)
            {
                CalamityMod.ghostKillCount++;
                if (CalamityMod.ghostKillCount == 10)
                {
                    string key = "Mods.CalamityMod.GhostBossText2";
                    Color messageColor = Color.Cyan;

                    CalamityUtils.DisplayLocalizedText(key, messageColor);
                }
                else if (CalamityMod.ghostKillCount == 20)
                {
                    string key = "Mods.CalamityMod.GhostBossText3";
                    Color messageColor = Color.Cyan;

                    CalamityUtils.DisplayLocalizedText(key, messageColor);
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

            if (NPC.downedPlantBoss && (npc.type == NPCID.SandShark || npc.type == NPCID.SandsharkHallow || npc.type == NPCID.SandsharkCorrupt || npc.type == NPCID.SandsharkCrimson) && !NPC.AnyNPCs(ModContent.NPCType<GreatSandShark.GreatSandShark>()))
            {
                CalamityMod.sharkKillCount++;
                if (CalamityMod.sharkKillCount == 4)
                {
                    string key = "Mods.CalamityMod.SandSharkText";
                    Color messageColor = Color.Goldenrod;

                    CalamityUtils.DisplayLocalizedText(key, messageColor);
                }
                else if (CalamityMod.sharkKillCount == 8)
                {
                    string key = "Mods.CalamityMod.SandSharkText2";
                    Color messageColor = Color.Goldenrod;

                    CalamityUtils.DisplayLocalizedText(key, messageColor);
                }
                if (CalamityMod.sharkKillCount >= 10 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active)
                    {
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/MaulerRoar"),
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
    }
}

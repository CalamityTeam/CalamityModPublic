using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.Deconstructors;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DraedonLabThings;
using CalamityMod.NPCs.GreatSandShark;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.SulphurousSea;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.Particles;
using CalamityMod.Walls.DraedonStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public sealed class TrustyOldRodEnemySystem : ModSystem
    {
        // Spawning for Trusty Old Rod NPCs, each biome has spawns of different rarity and sometimes style
        // Some spawns change with progression
        public static void SpawnTrustyOldRodNPC(Player owner, int bobberWhoAmI, int rarity = 1, bool Lava = false, bool Honey = false)
        {
            Projectile bobber = Main.projectile.FirstOrDefault(p => p.identity == bobberWhoAmI);
            bool common = rarity == 1;
            bool rare = rarity == 2;
            bool ultraRare = rarity == 3;
            bool hardmode = Main.hardMode;
            bool postML = NPC.downedMoonlord;
            int fallbackNPC = NPCID.Ghost; // If all biome checks fail, spawn a ghost

            bool night = !Main.dayTime;
            bool rain = Main.raining;
            bool underground = owner.ZoneDirtLayerHeight || owner.ZoneRockLayerHeight;
            // 1/3 chance to use preHM spawnpool in hardmode
            bool useHardmode = hardmode && Main.rand.NextBool(2, 3);

            // ReduceEnemyDrops is used for some reels that pull crazy numbers of enemies by making 1/3rd of them not drop loot
            List<(int npc, int spawnCount, bool reduceEnemyDrops)> npcIDsCommon = new List<(int npc, int spawnCount, bool reduceEnemyDrops)>();
            List<(int npc, int spawnCount, bool reduceEnemyDrops)> npcIDsRare = new List<(int npc, int spawnCount, bool reduceEnemyDrops)>();
            List<(int npc, int spawnCount, bool reduceEnemyDrops)> npcIDsUltraRare = new List<(int npc, int spawnCount, bool reduceEnemyDrops)>();

            Tile backWall = Framing.GetTileSafely((int)(owner.Center.X / 16), (int)(owner.Center.Y / 16));

            #region GetSpawns

            if (!Honey && !Lava)
            {
                if (Main.bloodMoon && !underground)
                {
                    // 5 Dripplers
                    if (common) { npcIDsCommon.Add((NPCID.Drippler, 5, false)); } // Common NPC/s
                    // 10 Blood Zombies
                    else if (rare) { npcIDsRare.Add((NPCID.BloodZombie, 10, false)); } // Rare NPC/s
                    // 1 Ravager
                    else if (ultraRare) { npcIDsUltraRare.Add((ModContent.NPCType<RavagerBody>(), 1, false)); } // Ultra Rare NPC/s
                }
                if (Main.eclipse && !underground) // Eclipse takes priority over surface spawns
                {
                    // 1/3 chance to use the pre Plant pool instead
                    bool postPlant = NPC.downedPlantBoss && Main.rand.NextBool(2, 3);
                    bool downedMechs = NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;
                    // 5 Creatures from the Eclipse, some tierlocked
                    int pickCommonEclipse = Main.rand.Next((downedMechs ? 0 : 1), (NPC.downedPlantBoss ? 7 : 3)) switch
                    {
                        0 => NPCID.Reaper, // Only available post 3 mechs

                        1 => NPCID.CreatureFromTheDeep,
                        2 => NPCID.ThePossessed,

                        3 => NPCID.DeadlySphere, // Only available post Plant
                        4 => NPCID.DrManFly,
                        5 => NPCID.Psycho,
                        _ => NPCID.Butcher
                    };

                    if (common) { npcIDsCommon.Add((pickCommonEclipse, 5, false)); } // Common NPC/s
                    // 8 Nailheads post Plant, otherwise 8 Vampires
                    else if (rare) { npcIDsRare.Add((postPlant ? NPCID.Nailhead : NPCID.Vampire, 8, false)); } // Rare NPC/s
                    // 5 Eyezors, 5 Mothrons post Plant
                    else if (ultraRare) { npcIDsUltraRare.Add((postPlant ? NPCID.Mothron : NPCID.Eyezor, 5, false)); } // Ultra Rare NPC/s
                }
                else if (owner.ZoneForest)
                {
                    bool wulfrum = !night && Main.rand.NextBool(3);
                    int tooManyZombies = Main.rand.Next(15) switch
                    {
                        0 => NPCID.Zombie,
                        1 => NPCID.BaldZombie,
                        2 => NPCID.PincushionZombie,
                        3 => NPCID.SlimedZombie,
                        4 => NPCID.SwampZombie,
                        5 => NPCID.TwiggyZombie,
                        6 => NPCID.FemaleZombie,
                        7 => NPCID.TorchZombie,
                        8 => NPCID.ArmedZombie,
                        9 => NPCID.ArmedZombiePincussion,
                        10 => NPCID.ArmedZombieSlimed,
                        11 => NPCID.ArmedZombieSwamp,
                        12 => NPCID.ArmedZombieTwiggy,
                        13 => NPCID.ArmedZombieCenx,
                        _ => NPCID.ArmedTorchZombie,
                    };

                    // All spawns are in groups of 2
                    // hardmode: Wraith or Wandering Eye, or Purple Slime if daytime
                    // nonHM pool: Zombie or Raincoat Zombie if raining, either Blue Slime or Green Slime if daytime or 1/3 chance for either Wulfrum Rover or Wulfrom Drone
                    int pickCommonForest = useHardmode ? (night ? Main.rand.NextBool() ? NPCID.Wraith : NPCID.WanderingEye : NPCID.PurpleSlime) :
                        (wulfrum ? Main.rand.NextBool() ? ModContent.NPCType<WulfrumDrone>() : ModContent.NPCType<WulfrumRover>() :
                        night ? rain ? NPCID.ZombieRaincoat : tooManyZombies :
                        Main.rand.NextBool() ? NPCID.BlueSlime : NPCID.GreenSlime);
                    // If hardmode and night, 1 Mimic.
                    // nonHM pool: 1/3 chance for 1 Wulfrum Amplifier, else if raining 1 Fearless Goldfish Warrior, otherwise 8 Zombies if night or 3 Clad Crabs if daytime
                    int pickRareForest = useHardmode && night ? NPCID.Mimic :
                        (wulfrum ? ModContent.NPCType<WulfrumAmplifier>() : rain ? ModContent.NPCType<FearlessGoldfishWarrior>() :
                        night ? tooManyZombies : ModContent.NPCType<CladCrab>());
                    int pickRareForestEnemyCount = useHardmode && night ? 1 :
                        (night && !rain && !wulfrum) ? 8 : wulfrum || rain ? 1 : 3;
                    // if hardmode, 20 Mimics at night or 20 Pinkys at daytime
                    // nonHM pool: Eye of Cthulhu if night or King Slime if daytime, 5 if hardmode and 15 if postML
                    int pickUltraRareForest = useHardmode ? night ? NPCID.Mimic : NPCID.Pinky :
                        (night ? NPCID.EyeofCthulhu : NPCID.KingSlime);

                    if (common) { npcIDsCommon.Add((pickCommonForest, 2, false)); } // Common NPC/s
                    else if (rare) { npcIDsRare.Add((pickRareForest, pickRareForestEnemyCount, false)); } // Rare NPC/s
                    else if (ultraRare) { npcIDsUltraRare.Add((pickUltraRareForest, useHardmode ? 20 : 1, useHardmode)); } // Ultra Rare NPC/s
                }
                if (owner.ZoneCorrupt)
                {
                    if (underground && hardmode) // if preHM, use the surface pool for underground
                    {
                        // 1 World Feeder
                        if (common) { npcIDsCommon.Add((NPCID.SeekerHead, 1, false)); } // Common NPC/s
                        // 3 Cursed Hammers
                        else if (rare) { npcIDsRare.Add((NPCID.CursedHammer, 3, false)); } // Rare NPC/s
                        // 20 Corrupt Mimics
                        else if (ultraRare) { npcIDsUltraRare.Add((NPCID.BigMimicCorruption, 20, true)); } // Ultra Rare NPC/s
                    }
                    else
                    {
                        // 1 Devourer or 3 Corruptors if hardmode
                        if (common) { npcIDsCommon.Add((useHardmode ? NPCID.Corruptor : NPCID.DevourerHead, useHardmode ? 3 : 1, false)); } // Common NPC/s
                        // 8 Eater of Souls or 8 Slimers if hardmode
                        else if (rare) { npcIDsRare.Add((useHardmode ? NPCID.Slimer : NPCID.EaterofSouls, 8, false)); } // Rare NPC/s
                        // 1 Hive Mind
                        else if (ultraRare) { npcIDsUltraRare.Add((ModContent.NPCType<HiveMind>(), 1, true)); } // Ultra Rare NPC/s
                    }
                }
                if (owner.ZoneCrimson)
                {
                    if (underground && hardmode) // if preHM, use the surface pool for underground
                    {
                        // 2 Ichor Stickers
                        if (common) { npcIDsCommon.Add((NPCID.IchorSticker, 2, false)); } // Common NPC/s
                        // 3 Crimson Axes
                        else if (rare) { npcIDsRare.Add((NPCID.CrimsonAxe, 3, false)); } // Rare NPC/s
                        // 20 Crimson Mimics
                        else if (ultraRare) { npcIDsUltraRare.Add((NPCID.BigMimicCrimson, 20, true)); } // Ultra Rare NPC/s
                    }
                    else
                    {
                        // 3 Crimeras or 3 Herplings if hardmode
                        if (common) { npcIDsCommon.Add((useHardmode ? NPCID.Herpling : NPCID.Crimera, 3, false)); } // Common NPC/s
                        // 8 Face Monsters or 8 Crimslimes if hardmode
                        else if (rare) { npcIDsRare.Add((useHardmode ? NPCID.Crimslime : NPCID.FaceMonster, 8, false)); } // Rare NPC/s
                        // 1 Perforator Hive
                        else if (ultraRare) { npcIDsUltraRare.Add((ModContent.NPCType<PerforatorHive>(), 1, true)); } // Ultra Rare NPC/s
                    }
                }
                if (owner.ZoneHallow)
                {
                    if (underground)
                    {
                        // 2 Chaos Elementals
                        if (common) { npcIDsCommon.Add((NPCID.ChaosElemental, 2, false)); } // Common NPC/s
                        // 3 Enchanted Swords
                        else if (rare) { npcIDsRare.Add((NPCID.EnchantedSword, 3, false)); } // Rare NPC/s
                        // 20 Hallowed Mimics
                        else if (ultraRare) { npcIDsUltraRare.Add((NPCID.BigMimicHallow, 20, true)); } // Ultra Rare NPC/s
                    }
                    else
                    {
                        bool profaned = postML && Main.rand.NextBool(4);
                        // 3 Pixies at daytime, 3 Gastropods at night
                        if (common) { npcIDsCommon.Add((profaned ? ModContent.NPCType<ImpiousImmolator>() : night ? NPCID.Gastropod : NPCID.Pixie, profaned ? 5 : 3, false)); } // Common NPC/s
                        // 3 Unicorns or 3 Rainbow Slimes if raining
                        else if (rare) { npcIDsRare.Add((profaned ? ModContent.NPCType<ScornEater>() : rain ? NPCID.RainbowSlime : NPCID.Unicorn, 3, false)); } // Rare NPC/s
                        // 1 Queen Slime or 2/3 for 1 Empress if post Plant
                        else if (ultraRare) { npcIDsUltraRare.Add((profaned ? ModContent.NPCType<Providence>() : NPC.downedPlantBoss && Main.rand.NextBool(2, 3) ? NPCID.HallowBoss : NPCID.QueenSlimeBoss, 1, false)); } // Ultra Rare NPC/s
                    }
                }
                if (owner.ZoneSnow)
                {
                    if (underground)
                    {
                        bool flinx = Main.rand.NextBool(3);
                        int pigron = owner.ZoneHallow ? NPCID.PigronHallow : owner.ZoneCrimson ? NPCID.PigronCrimson : owner.ZoneCorrupt ? NPCID.PigronCorruption : 0;
                        // 2 Undead Vikings, if hardmode 2 Ice Claspers or 2 Ice Tortoises. 2 Pigrons if also in Hallow/Corruption/Crimson
                        int pickRareUGSnow = (pigron != 0 && hardmode) ? pigron : (useHardmode ? (Main.rand.NextBool() ? ModContent.NPCType<IceClasper>() : NPCID.IceTortoise) : NPCID.UndeadViking);

                        // 3 Ice Bat or 1/3 for 2 Snow Flinx
                        if (common) { npcIDsCommon.Add((flinx ? NPCID.SnowFlinx : NPCID.IceBat, flinx ? 2 : 3, false)); } // Common NPC/s
                        else if (rare) { npcIDsRare.Add((pickRareUGSnow, 2, false)); } // Rare NPC/s
                        // 3 Ice Mimics if hardmode, otherwise 50 Ice bats
                        else if (ultraRare) { npcIDsUltraRare.Add((!hardmode ? NPCID.IceBat : NPCID.IceMimic, !hardmode ? 50 : 3, !hardmode)); } // Ultra Rare NPC/s
                    }
                    else
                    {
                        // If hardmode 2 Ice Elementals if night or 2 Aurora Spirits if day, otherwise 2 Frozen Zombies if night or 2 Ice Slimes if day
                        int pickCommonSnow = useHardmode ? (night ? NPCID.IceElemental : ModContent.NPCType<AuroraSpirit>()) : night ? NPCID.ZombieEskimo : NPCID.IceSlime;
                        // If hardmode and blizzard 1 Ice Golem, otherwise 3 Cryons if hardmode or 3 Rimehounds in preHM
                        int pickRareSnow = hardmode && rain ? NPCID.IceGolem : useHardmode ? ModContent.NPCType<Cryon>() : ModContent.NPCType<Rimehound>();

                        if (common) { npcIDsCommon.Add((pickCommonSnow, 2, false)); } // Common NPC/s
                        else if (rare) { npcIDsRare.Add((pickRareSnow, (hardmode && rain) ? 1 : 3, false)); } // Rare NPC/s
                        // 50 Snow Flinx preHM, 1 Cryogen in harmdode
                        else if (ultraRare) { npcIDsUltraRare.Add((useHardmode ? ModContent.NPCType<Cryogen>() : NPCID.SnowFlinx, useHardmode ? 1 : 50, (!postML && !useHardmode))); } // Ultra Rare NPC/s
                    }
                }
                if (owner.ZoneUndergroundDesert)
                {
                    bool golferCheck = !NPC.savedGolfer && !NPC.AnyNPCs(NPCID.GolferRescue);
                    int ghoul = owner.ZoneHallow ? NPCID.DesertGhoulHallow : owner.ZoneCrimson ? NPCID.DesertGhoulCrimson : owner.ZoneCorrupt ? NPCID.DesertGhoulCorruption : NPCID.DesertGhoul;

                    // 1/4 for 10 Antlion Larva, otherwise either 2 Antlion Swarmers, 2 Antlion Chargers, or 2 Stormlions
                    bool larva = Main.rand.NextBool(4);
                    int pickCommonUGDesert = larva ? NPCID.LarvaeAntlion : Main.rand.Next(3) switch
                    {
                        0 => NPCID.GiantWalkingAntlion,
                        1 => NPCID.GiantFlyingAntlion,
                        _ => ModContent.NPCType<Stormlion>()
                    };
                    // Golfer if he hasn't been saved, either 2 Ghouls (sub-biome dependant) or 2 Lamias (sub-biome dependant) if hardmode, otherwise Tomb Crawler
                    int pickRareUGDesert = golferCheck ? NPCID.GolferRescue : !useHardmode ? NPCID.TombCrawlerHead :
                        Main.rand.NextBool() ? ghoul : ((owner.ZoneCorrupt || owner.ZoneCrimson) ? NPCID.DesertLamiaDark : NPCID.DesertLamiaLight);

                    if (common) { npcIDsCommon.Add((pickCommonUGDesert, larva ? 10 : 2, false)); } // Common NPC/s
                    else if (rare) { npcIDsRare.Add((pickRareUGDesert, golferCheck || !useHardmode ? 1 : 2, false)); } // Rare NPC/s
                    // 1 Desert Scourge
                    else if (ultraRare) { npcIDsUltraRare.Add((ModContent.NPCType<DesertScourgeHead>(), 1, false)); } // Ultra Rare NPC/s
                }
                else if (owner.ZoneDesert)
                {
                    bool sandstorm = Sandstorm.Happening;
                    int mummy = owner.ZoneHallow ? NPCID.LightMummy : owner.ZoneCrimson ? NPCID.BloodMummy : owner.ZoneCorrupt ? NPCID.DarkMummy : NPCID.Mummy;
                    int sandShark = owner.Calamity().ZoneAstral ? ModContent.NPCType<FusionFeeder>() : owner.ZoneHallow ? NPCID.SandsharkHallow : owner.ZoneCrimson ? NPCID.SandsharkCrimson : owner.ZoneCorrupt ? NPCID.SandsharkCorrupt : NPCID.SandShark;
                    // 2 Sand Sharks if hardmode sandstorm (sub-biome dependant), otherwise 2 Angry Tumblers. 
                    // If no sandstorm 2 Mummys (sub-biome dependant) in hardmode, otherwise 2 Vultures (Hadarians if in Astral)
                    int pickCommonDesert = sandstorm ? (useHardmode ? sandShark : NPCID.Tumbleweed) :
                        useHardmode ? mummy : owner.Calamity().ZoneAstral ? ModContent.NPCType<Hadarian>() : NPCID.Vulture;
                    // Sand Elemental if hardmode sandstorm, otherwise 5 Angry Tumblers. If no sandstorm 5 Antlion Chargers or 5 Antlion Swarmers
                    int pickRareDesert = sandstorm ? (hardmode ? NPCID.SandElemental : NPCID.Tumbleweed) :
                        Main.rand.NextBool() ? NPCID.WalkingAntlion : NPCID.FlyingAntlion;

                    if (common) { npcIDsCommon.Add((pickCommonDesert, 2, false)); } // Common NPC/s
                    else if (rare) { npcIDsRare.Add((pickRareDesert, sandstorm && hardmode ? 1 : 5, false)); } // Rare NPC/s
                    // 3 Cnidrions or 3 Great Sand Sharks if post Plant
                    else if (ultraRare) { npcIDsUltraRare.Add((NPC.downedPlantBoss ? ModContent.NPCType<GreatSandShark>() : ModContent.NPCType<Cnidrion>(), 3, false)); } // Ultra Rare NPC/s
                }
                if (owner.ZoneBeach && !owner.Calamity().ZoneSulphur)
                {
                    // 2 Squid, Sea Snails, or Sharks. Can also be Mantis Shrimp post Plant
                    int pickRareOcean = Main.rand.Next(3 + (hardmode ? 1 : 0)) switch
                    {
                        0 => NPCID.Squid,
                        1 => NPCID.SeaSnail,
                        2 => NPCID.Shark,
                        _ => ModContent.NPCType<MantisShrimp>()
                    };
                    // 2 Pink Jellyfish or Crabs
                    if (common) { npcIDsCommon.Add((Main.rand.NextBool() ? NPCID.PinkJellyfish : NPCID.Crab, 2, false)); } // Common NPC/s
                    else if (rare) { npcIDsRare.Add((pickRareOcean, 2, false)); } // Rare NPC/s
                    // 1 Anahita
                    else if (ultraRare) { npcIDsUltraRare.Add((ModContent.NPCType<Anahita>(), 1, false)); } // Ultra Rare NPC/s
                }
                else if (owner.Calamity().ZoneSulphur)
                {
                    bool acidRain = AcidRainEvent.AcidRainEventIsOngoing;
                    bool tier3 = DownedBossSystem.downedPolterghast;
                    bool tier2 = tier3 || DownedBossSystem.downedAquaticScourge;
                    // In Acid Rain 1/3 for 3 Gamma Slimes if tier 3, else pulls from the tier 2 enemy list and then tier 1.
                    // If no Acid Rain, spawns 3 Trashers, Gnashers, or Aquatic Urchins
                    int pickCommonSulphor = Main.rand.Next(3) switch
                    {
                        0 => acidRain ? (tier3 ? ModContent.NPCType<GammaSlime>() : tier2 ? ModContent.NPCType<SulphurousSkater>() : ModContent.NPCType<NuclearToad>()) : ModContent.NPCType<Trasher>(),
                        1 => acidRain ? (tier2 ? ModContent.NPCType<FlakCrab>() : ModContent.NPCType<Skyfin>()) : ModContent.NPCType<Gnasher>(),
                        _ => acidRain ? (tier2 ? ModContent.NPCType<Orthocera>() : ModContent.NPCType<Radiator>()) : ModContent.NPCType<AquaticUrchin>(),
                    };
                    // 50% chance to use an abyss pool instead for rare pulls (when not acid rain)
                    bool abyssSpawns = Main.rand.NextBool();
                    // In Acid Rain if tier 3 pulls 1 Mauler or Nuclear Terror. If tier 2, pulls 1 Cragmaw Mire, or Acid Eel if tier 1 
                    // If no Acid Rain either Anthozoan Crab or Belching Coral if post Aquatic Scourge, otherwise either Viperfish or Giant Squid
                    int pickRareSulphor = acidRain ? (tier3 ? Main.rand.NextBool() ? ModContent.NPCType<Mauler>() : ModContent.NPCType<NuclearTerror>() :
                        tier2 ? ModContent.NPCType<CragmawMire>() : ModContent.NPCType<AcidEel>()) :
                        (tier2 ? Main.rand.NextBool() ? ModContent.NPCType<AnthozoanCrab>() : ModContent.NPCType<BelchingCoral>() : ModContent.NPCType<Toxicatfish>());
                    // Post Polter 1/3 chance for Reaper Shark or Eidolon Wyrm, else 1/2 chance for 3 Devil Fish or 3 Chaotic Puffer, else 3 Viperfish or 3 Cuttlefish
                    bool bigAbyssSpawn = Main.rand.NextBool(3);
                    int pickRareSulphorAbyss = tier3 && bigAbyssSpawn ? (Main.rand.NextBool() ? ModContent.NPCType<ReaperShark>() : ModContent.NPCType<EidolonWyrmHead>()) :
                        tier2 && Main.rand.NextBool() ? (Main.rand.NextBool() ? ModContent.NPCType<DevilFish>() : ModContent.NPCType<ChaoticPuffer>()) :
                        (Main.rand.NextBool() ? ModContent.NPCType<Viperfish>() : ModContent.NPCType<Cuttlefish>());

                    bool spawnSingular = (acidRain && (tier2 || tier3)) || (bigAbyssSpawn && abyssSpawns);
                    if (common) { npcIDsCommon.Add((pickCommonSulphor, 3, false)); } // Common NPC/s
                    else if (rare) { npcIDsRare.Add(((abyssSpawns && !acidRain) ? pickRareSulphorAbyss : pickRareSulphor, spawnSingular ? 1 : 3, false)); } // Rare NPC/s
                    // Primordial Wyrm
                    else if (ultraRare) { npcIDsUltraRare.Add((ModContent.NPCType<PrimordialWyrmHead>(), 1, false)); } // Ultra Rare NPC/s
                }
                if (owner.ZoneDungeon)
                {
                    bool postPlantDungeon = NPC.downedPlantBoss && Main.rand.NextBool(3, 4);
                    bool tileWall = backWall.WallType == WallID.PinkDungeonTileUnsafe || backWall.WallType == WallID.GreenDungeonTileUnsafe || backWall.WallType == WallID.BlueDungeonTileUnsafe;
                    bool slabWall = backWall.WallType == WallID.PinkDungeonSlabUnsafe || backWall.WallType == WallID.GreenDungeonSlabUnsafe || backWall.WallType == WallID.BlueDungeonSlabUnsafe;
                    bool brickWall = backWall.WallType == WallID.PinkDungeonUnsafe || backWall.WallType == WallID.GreenDungeonUnsafe || backWall.WallType == WallID.BlueDungeonUnsafe;
                    
                    int tooManyAngryBones = Main.rand.Next(4) switch
                    {
                        0 => NPCID.AngryBones,
                        1 => NPCID.AngryBonesBig,
                        2 => NPCID.AngryBonesBigMuscle,
                        _ => NPCID.AngryBonesBigHelmet,
                    };
                    int tooManyBlueBones = Main.rand.Next(4) switch
                    {
                        0 => NPCID.BlueArmoredBones,
                        1 => NPCID.BlueArmoredBonesMace,
                        2 => NPCID.BlueArmoredBonesNoPants,
                        _ => NPCID.BlueArmoredBonesSword,
                    };
                    int tooManyHellBones = Main.rand.Next(4) switch
                    {
                        0 => NPCID.HellArmoredBones,
                        1 => NPCID.HellArmoredBonesMace,
                        2 => NPCID.HellArmoredBonesSpikeShield,
                        _ => NPCID.HellArmoredBonesSword,
                    };
                    int tooManyRustyBones = Main.rand.Next(4) switch
                    {
                        0 => NPCID.RustyArmoredBonesAxe,
                        1 => NPCID.RustyArmoredBonesFlail,
                        2 => NPCID.RustyArmoredBonesSword,
                        _ => NPCID.RustyArmoredBonesSwordNoArmor,
                    };

                    // 3 of the basic "Bones" type enemy for the dungeon wall type. 3 Angry bones pre Plant (or if somehow another wall type)
                    int pickCommonDungeon = !postPlantDungeon ? tooManyAngryBones : 
                        brickWall ? tooManyBlueBones : tileWall ? tooManyHellBones : slabWall ? tooManyRustyBones : tooManyAngryBones;
                    // 2 Dark Casters or Cursed Skulls, if post Plant instead get either a 2 "Skeleton Mages" or 2 "Skeleton Rangers" based on background wall
                    bool getMage = Main.rand.NextBool();
                    int pickRareDungeon = !postPlantDungeon ? (Main.rand.NextBool() ? NPCID.DarkCaster : NPCID.CursedSkull) : !getMage ? (tileWall ? NPCID.TacticalSkeleton : slabWall ? NPCID.SkeletonSniper : brickWall ? NPCID.SkeletonCommando : tooManyAngryBones) :
                        (tileWall ? Main.rand.NextBool() ? NPCID.DiabolistWhite : NPCID.DiabolistRed : 
                        slabWall ? Main.rand.NextBool() ? NPCID.RaggedCaster : NPCID.RaggedCasterOpenCoat : 
                        brickWall ? Main.rand.NextBool() ? NPCID.Necromancer : NPCID.NecromancerArmored : tooManyAngryBones);
                    // 10 Dungeon Slimes pre Plant, 1 Cultist post Plant, 1 Polterghast postML
                    int pickUltraRareDungeon = postML ? ModContent.NPCType<Polterghast>() : postPlantDungeon ? NPCID.CultistBoss : NPCID.DungeonSlime;

                    if (common) { npcIDsCommon.Add((pickCommonDungeon, 3, false)); } // Common NPC/s
                    else if (rare) { npcIDsRare.Add((pickRareDungeon, 2, false)); } // Rare NPC/s
                    else if (ultraRare) { npcIDsUltraRare.Add((pickUltraRareDungeon, (postML || postPlantDungeon) ? 1 : 10, false)); } // Ultra Rare NPC/s
                }
                if (owner.ZoneSkyHeight)
                {
                    bool probe = NPC.downedGolemBoss && Main.rand.NextBool(10);
                    bool shuttle = hardmode && Main.rand.NextBool(4);
                    // 1/10 to be Martian Probe post Golem, otherwise if hardmode a 1/4 to be 3 Shockstorm Shuttles, otherwise 1 Wyvern or 1 Cloud Elemental if it's also raining. 7 Harpys if preHM
                    int pickRareSky = probe ? NPCID.MartianProbe : hardmode ? Main.raining ? ModContent.NPCType<CloudElemental>() : 
                        shuttle ? ModContent.NPCType<ShockstormShuttle>() : NPCID.WyvernHead : NPCID.Harpy;

                    // 1/4 to be 3 Harpys, otherwise 3 Sunskaters
                    if (common) { npcIDsCommon.Add((Main.rand.NextBool(4) ? NPCID.Harpy : ModContent.NPCType<Sunskater>(), 3, false)); } // Common NPC/s
                    else if (rare) { npcIDsRare.Add((pickRareSky, ((hardmode && !shuttle) || (hardmode && Main.raining) || probe) ? 1 : shuttle ? 3 : 7, false)); } // Rare NPC/s
                    // 100 Sunskaters
                    else if (ultraRare) { npcIDsUltraRare.Add((ModContent.NPCType<Sunskater>(), 100, true)); } // Ultra Rare NPC/s
                }
                if (backWall.WallType == WallID.SpiderUnsafe && underground) // Spider caves (Priority over Caverns and Mushroom and Granite/Marble)
                {
                    bool stylistCheck = !NPC.savedStylist && !NPC.AnyNPCs(NPCID.WebbedStylist);
                    // Astralachnea/Blood Crawlers always take the place of normal spiders if in Astral/Crimson
                    int pickASpider = owner.Calamity().ZoneAstral ? ModContent.NPCType<AstralachneaGround>() : owner.ZoneCrimson ? NPCID.BloodCrawler : useHardmode ? NPCID.BlackRecluse : NPCID.WallCreeper;
                    // 2 Wall Creepers/Black Recluses (HM)
                    if (common) { npcIDsCommon.Add((pickASpider, 2, false)); }// Common NPC/s
                    // Stylist if she hasn't been saved, otherwise 10 Wall Creepers/Black Recluses (HM)
                    else if (rare) { npcIDsRare.Add((stylistCheck ? NPCID.WebbedStylist : pickASpider, stylistCheck ? 1 : 10, false)); } // Rare NPC/s
                    // 100 Wall Creepers/Black Recluses (HM), always Black Recluses in hardmode
                    else if (ultraRare) { npcIDsUltraRare.Add((owner.Calamity().ZoneAstral ? ModContent.NPCType<AstralachneaGround>() : hardmode ? NPCID.BlackRecluse : NPCID.WallCreeper, 100, true)); } // Ultra Rare NPC/s
                }
                if (owner.ZoneGlowshroom) // (Priority over Caverns and Granite/Marble)
                {
                    // 3 Spore Bats
                    if (common) { npcIDsCommon.Add((NPCID.SporeBat, 3, false)); } // Common NPC/s
                    // 1/10 for 2 Truffle Worms, 9/10 for 2 Anomura Fungus
                    else if (rare) { npcIDsRare.Add(Main.rand.NextBool(10) ? (NPCID.TruffleWorm, 2, false) : (NPCID.AnomuraFungus, 2, false)); } // Rare NPC/s
                    // 1 Crabulon
                    else if (ultraRare) { npcIDsUltraRare.Add((ModContent.NPCType<Crabulon>(), 1, false)); } // Ultra Rare NPC/s
                }
                else if (owner.ZoneGranite || owner.ZoneMarble) // (Priority over Caverns)
                {
                    if (owner.ZoneGranite)
                    {
                        // 3 Granite Elementals
                        if (common) { npcIDsCommon.Add((NPCID.GraniteFlyer, 3, false)); } // Common NPC/s
                        // 4 Granite Golems
                        else if (rare) { npcIDsRare.Add((NPCID.GraniteGolem, 4, false)); } // Rare NPC/s
                        // 25 Diamond Crawlers
                        else if (ultraRare) { npcIDsUltraRare.Add((ModContent.NPCType<CrawlerDiamond>(), 25, true)); } // Ultra Rare NPC/s
                    }
                    if (owner.ZoneMarble)
                    {
                        // 2 Hoplites
                        if (common) { npcIDsCommon.Add((NPCID.GreekSkeleton, 2, false)); } // Common NPC/s
                        // 15 Hoplites, in hardmode 2 Medusas instead
                        else if (rare) { npcIDsRare.Add(useHardmode ? (NPCID.Medusa, 2, false) : (NPCID.GreekSkeleton, 15, true)); } // Rare NPC/s
                        // 25 Diamond Crawlers
                        else if (ultraRare) { npcIDsUltraRare.Add((ModContent.NPCType<CrawlerDiamond>(), 25, true)); } // Ultra Rare NPC/s
                    }
                }
                else if (owner.ZoneNormalCaverns)
                {
                    bool goblinCheck = NPC.downedGoblins && !NPC.savedGoblin && !NPC.AnyNPCs(NPCID.BoundGoblin);
                    bool wizardCheck = hardmode && !NPC.savedWizard && !NPC.AnyNPCs(NPCID.BoundWizard);
                    int tooManyLizards = Main.rand.Next(9) switch
                    {
                        0 => NPCID.Salamander,
                        1 => NPCID.Salamander2,
                        2 => NPCID.Salamander3,
                        3 => NPCID.Salamander4,
                        4 => NPCID.Salamander5,
                        5 => NPCID.Salamander6,
                        6 => NPCID.Salamander7,
                        7 => NPCID.Salamander8,
                        _ => NPCID.Salamander9
                    };
                    int tooManySkeletons = Main.rand.Next(8) switch
                    {
                        0 => NPCID.Skeleton,
                        1 => NPCID.HeadacheSkeleton,
                        2 => NPCID.MisassembledSkeleton,
                        3 => NPCID.PantlessSkeleton,
                        4 => NPCID.BoneThrowingSkeleton,
                        5 => NPCID.BoneThrowingSkeleton2,
                        6 => NPCID.BoneThrowingSkeleton3,
                        _ => NPCID.BoneThrowingSkeleton4
                    };
                    // 2/3 for 3 Cave Bats (3 Giant Bats in HM), otherwise a singular Shelly, Crawdad, Salamander, or Skeleton
                    bool bats = Main.rand.NextBool(2, 3);
                    int pickCommonCaverns = Main.rand.Next(4) switch
                    {
                        0 => Main.rand.NextBool() ? NPCID.GiantShelly : NPCID.GiantShelly2,
                        1 => tooManyLizards, // One of the 9 Salamander variants
                        2 => Main.rand.NextBool() ? NPCID.Crawdad : NPCID.Crawdad2,
                        _ => tooManySkeletons, // One of the many Skeletons (does not include the small/big variants, but does include bone throwing variants)
                    };
                    // Gobin if he hasn't been saved and the invasion has been cleared or Wizard if hardmode,
                    // otherwise an Undead Miner, Tim, or Nymph (Most often Undead Miner (70%), otherwise can be a Tim or Nymph (15% each))
                    int randNum = Main.rand.Next(1, 100 + 1);
                    int pickRareCaverns = (randNum > 30 ? NPCID.UndeadMiner : randNum <= 15 ? NPCID.Nymph : NPCID.Tim);
                    // 15 Mother Slimes, or if harmode either 35 Armored or Archer Skeletons, 2/3 chance postML for 35 Overloaded Soldiers
                    int pickUltraRareCaverns = postML && Main.rand.NextBool(2, 3) ? ModContent.NPCType<OverloadedSoldier>() : hardmode ? Main.rand.NextBool() ? NPCID.ArmoredSkeleton : NPCID.SkeletonArcher : NPCID.MotherSlime;

                    if (common) { npcIDsCommon.Add((bats ? (useHardmode ? NPCID.GiantBat : NPCID.CaveBat) : pickCommonCaverns, bats ? 3 : 1, false)); } // Common NPC/s
                    else if (rare) { npcIDsRare.Add((goblinCheck ? NPCID.BoundGoblin : wizardCheck ? NPCID.BoundWizard : pickRareCaverns, 1, false)); } // Rare NPC/s
                    else if (ultraRare) { npcIDsUltraRare.Add((pickUltraRareCaverns, (!hardmode && !postML) ? 15 : 35, true)); } // Ultra Rare NPC/s

                }
                else if (owner.ZoneNormalUnderground)
                {
                    // Giant Worm or Digger if hardmode
                    if (common) { npcIDsCommon.Add((useHardmode ? NPCID.DiggerHead : NPCID.GiantWormHead, 1, true)); } // Common NPC/s
                    // 1/2 for 7 Red Slimes, otherwise 7 Yellow Slimes, 7 Toxic Sludge instead in Hardmode
                    else if (rare) { npcIDsRare.Add((useHardmode ? NPCID.ToxicSludge : Main.rand.NextBool() ? NPCID.RedSlime : NPCID.YellowSlime, 7, false)); } // Rare NPC/s
                    // 1/3 for 50 Green Jellyfish, otherwise 50 Blue Jellyfish
                    else if (ultraRare) { npcIDsUltraRare.Add((Main.rand.NextBool(3) ? NPCID.GreenJellyfish : NPCID.BlueJellyfish, 50, true)); } // Ultra Rare NPC/s
                }
                if (owner.ZoneLihzhardTemple) // (Priority over Jungle)
                {
                    // 3 Lihzahrds
                    if (common) { npcIDsCommon.Add((NPCID.Lihzahrd, 3, false)); } // Common NPC/s
                    // 5 Flying Snakes or Bohldohrs
                    else if (rare) { npcIDsRare.Add((Main.rand.NextBool() ? NPCID.FlyingSnake : ModContent.NPCType<Bohldohr>(), 5, false)); } // Rare NPC/s
                    // 1 Golem
                    else if (ultraRare) { npcIDsUltraRare.Add((NPCID.Golem, 1, false)); } // Ultra Rare NPC/s
                }
                else if (owner.ZoneJungle)
                {
                    bool plagueSpawns = NPC.downedGolemBoss && Main.rand.NextBool(2, 3);
                    // Underground Jungle
                    if (underground)
                    {
                        int tooManyHornets = Main.rand.Next(6) switch
                        {
                            0 => NPCID.Hornet,
                            1 => NPCID.HornetFatty,
                            2 => NPCID.HornetHoney,
                            3 => NPCID.HornetLeafy,
                            4 => NPCID.HornetSpikey,
                            _ => NPCID.HornetStingy,
                        };
                        bool hornets = Main.rand.NextBool(4);
                        // 2 Giant Tortoises or Jungle Creeper if hardmode, 2 Jungle Slimes if not. Chance to be 2 Plagueshells if plague
                        int pickCommonUGJungle = plagueSpawns ? ModContent.NPCType<Plagueshell>() : useHardmode ? Main.rand.NextBool() ? NPCID.GiantTortoise : NPCID.JungleCreeper : NPCID.JungleSlime;
                        // 1/4 chance to be 22 Hornets (Moss Hornets if hardmode, chance for Plague Chargers if plague), otherwise a Moth in hardmode or 5 Spiked Jungle slimes if not 
                        int pickRareUGJungle = hornets ? (plagueSpawns ? ModContent.NPCType<PlagueCharger>() : useHardmode ? NPCID.MossHornet : tooManyHornets) : useHardmode ? NPCID.Moth : NPCID.SpikedJungleSlime;
                        // Plantera, or chance for PBG if plague
                        int pickUltraRareUGJungle = plagueSpawns ? ModContent.NPCType<PlaguebringerGoliath>() : NPCID.Plantera;
                        if (common) { npcIDsCommon.Add((pickCommonUGJungle, 2, false)); } // Common NPC/s
                        else if (rare) { npcIDsRare.Add((pickRareUGJungle, hornets ? 22 : useHardmode ? 1 : 5, hornets)); } // Rare NPC/s
                        else if (ultraRare) { npcIDsUltraRare.Add((pickUltraRareUGJungle, 1, false)); } // Ultra Rare NPC/s
                    }
                    else // Surface Jungle
                    {
                        // 3 Jungle Bats or Giant Flying Foxes if hardmode, sometimes Melters if plague
                        int pickCommonJungle = plagueSpawns ? ModContent.NPCType<Melter>() : useHardmode ? NPCID.GiantFlyingFox : NPCID.JungleBat;
                        // 10 Piranhas or Arapaimas if hardmode, 50% chance to be 3 Derplings instead or Virulings if plague
                        bool noFish = Main.rand.NextBool();
                        int pickRareJungle = plagueSpawns && noFish ? ModContent.NPCType<Viruling>() : useHardmode ? noFish ? NPCID.Derpling : NPCID.Arapaima : NPCID.Piranha;
                        // 50 Jungle Bats or Giant Flying Foxes if hardmode, or Draconic Swarmers if postML
                        int pickUltraRareJungle = postML ? ModContent.NPCType<WildBumblebirb>() : hardmode ? NPCID.GiantFlyingFox : NPCID.JungleBat;
                        if (common) { npcIDsCommon.Add((pickCommonJungle, 3, false)); } // Common NPC/s
                        else if (rare) { npcIDsRare.Add((pickRareJungle, useHardmode ? 3 : 10, false)); } // Rare NPC/s
                        else if (ultraRare) { npcIDsUltraRare.Add((pickUltraRareJungle, 50, true)); } // Ultra Rare NPC/s
                    }
                }
                if (owner.ZoneGraveyard)
                {
                    // 2 Maggot Zombies
                    if (common) { npcIDsCommon.Add((NPCID.MaggotZombie, 2, false)); } // Common NPC/s
                    // Either 3 Rotdogs or 3 Bucket Zombies
                    else if (rare) { npcIDsRare.Add((Main.rand.NextBool() ? ModContent.NPCType<Rotdog>() : ModContent.NPCType<BucketZombie>(), 3, false)); } // Rare NPC/s
                    // Either 2 Grooms or 2 Brides
                    else if (ultraRare) { npcIDsUltraRare.Add((Main.rand.NextBool() ? NPCID.TheBride : NPCID.TheGroom, 2, false)); } // Ultra Rare NPC/s
                }
                if (owner.Calamity().ZoneAstral)
                {
                    if (underground)
                    {
                        bool glomers = Main.rand.NextBool(4);
                        // 2 Stellar Culex or 1/4 for 15 Glomerlings
                        if (common) { npcIDsCommon.Add((glomers ? ModContent.NPCType<Glomerling>() : ModContent.NPCType<StellarCulex>(), glomers ? 15 : 2, false)); } // Common NPC/s
                        // Either 5 Astralachnea or 5 Astraglomerates
                        else if (rare) { npcIDsRare.Add((Main.rand.NextBool() ? ModContent.NPCType<AstralachneaGround>() : ModContent.NPCType<Astraglomerate>(), 5, false)); } // Rare NPC/s
                        // 1 Astrum Aureus
                        else if (ultraRare) { npcIDsUltraRare.Add((ModContent.NPCType<AstrumAureus>(), 1, false)); } // Ultra Rare NPC/s
                    }
                    else
                    {
                        // Either 2 Aries or 2 Nova
                        if (common) { npcIDsCommon.Add((Main.rand.NextBool() ? ModContent.NPCType<Aries>() : ModContent.NPCType<Nova>(), 2, false)); } // Common NPC/s
                        // Either 3 Atlas or 3 Mantis
                        else if (rare) { npcIDsRare.Add((Main.rand.NextBool() ? ModContent.NPCType<Atlas>() : ModContent.NPCType<Mantis>(), 3, false)); } // Rare NPC/s
                        // 1 Astrum Deus
                        else if (ultraRare) { npcIDsUltraRare.Add((ModContent.NPCType<AstrumDeusHead>(), 1, false)); } // Ultra Rare NPC/s
                    }
                }
                if (owner.Calamity().ZoneSunkenSea)
                {
                    bool clams = Main.rand.NextBool(3);
                    bool downedDS = DownedBossSystem.downedDesertScourge;
                    // 2 Ghost Bells
                    if (common) { npcIDsCommon.Add((ModContent.NPCType<GhostBell>(), 2, false)); } // Common NPC/s
                    // 1/3 for 8 Clams, otherwise 3 Prism Backs
                    else if (rare) { npcIDsRare.Add((clams ? ModContent.NPCType<Clam>() : ModContent.NPCType<PrismBack>(), clams ? 8 : 3, false)); } // Rare NPC/s
                    // postDS 3 Giant Clams or 10 postML, otherwise 50 Sea Floaties
                    else if (ultraRare) { npcIDsUltraRare.Add((downedDS ? ModContent.NPCType<GiantClam>() : ModContent.NPCType<SeaFloaty>(), downedDS ? postML ? 10 : 3 : 50, !downedDS)); } // Ultra Rare NPC/s
                }
                // Arsenal Labs
                if ((backWall.WallType == ModContent.WallType<HazardChevronWall>() ||
                    backWall.WallType == ModContent.WallType<LaboratoryPanelWall>() ||
                    backWall.WallType == ModContent.WallType<LaboratoryPlateBeam>() ||
                    backWall.WallType == ModContent.WallType<LaboratoryPlatePillar>() ||
                    backWall.WallType == ModContent.WallType<LaboratoryPlatingWall>()) &&
                    BiomeTileCounterSystem.ArsenalLabTiles > 150)
                {
                    // 5 Broken Nanodroids
                    if (common) { npcIDsCommon.Add((ModContent.NPCType<NanodroidDysfunctional>(), 5, false)); } // Common NPC/s
                    // 1 Androomba
                    else if (rare) { npcIDsRare.Add((ModContent.NPCType<Androomba>(), 1, false)); } // Rare NPC/s
                    // The Burrower
                    else if (ultraRare) { npcIDsUltraRare.Add((ModContent.NPCType<Burrower>(), 1, false)); } // Ultra Rare NPC/s
                }
            }
            else if (Honey)
            {
                // 15 Small Bees
                if (common) { npcIDsCommon.Add((NPCID.BeeSmall, 15, false)); } // Common NPC/s
                // 25 Bees
                else if (rare) { npcIDsRare.Add((NPCID.Bee, 25, false)); } // Rare NPC/s
                // 1 Queen Bee
                else if (ultraRare) { npcIDsUltraRare.Add((NPCID.QueenBee, 1, false)); } // Ultra Rare NPC/s
            }
            else if (Lava)
            {
                if (owner.Calamity().ZoneCalamity)
                {
                    if (common) { npcIDsCommon.Add((ModContent.NPCType<SoulSlurper>(), 2, false)); } // Common NPC/s
                    else if (rare) { npcIDsRare.Add((ModContent.NPCType<CalamityEye>(), 7, false)); } // Rare NPC/s
                    else if (ultraRare) { npcIDsUltraRare.Add((useHardmode ? ModContent.NPCType<RenegadeWarlock>() : ModContent.NPCType<DespairStone>(), 50, true)); } // Ultra Rare NPC/s
                }
                else
                {
                    // If not in hell, underworld enemies are replaced with some Heat Spirits (Common: 2, Rare: 10, UltraRare: 50)
                    bool notInHell = !owner.ZoneUnderworldHeight;
                    bool profaned = postML && Main.rand.NextBool(4);
                    int nonHellEnemy = ModContent.NPCType<HeatSpirit>();
                    bool snake = Main.rand.NextBool(4);
                    // 5 Hellbats or Lavabats if hardmode, 25% chance to be one Bone Serpent instead, or chance for Impious Immolator
                    int pickCommonHell = notInHell ? nonHellEnemy : profaned ? ModContent.NPCType<ImpiousImmolator>() : snake ? NPCID.BoneSerpentHead : (useHardmode ? NPCID.Lavabat : NPCID.Hellbat);
                    // 3 Demons or Red Devils if hardmode, or chance for Scorn Eater 
                    int pickRareHell = notInHell ? nonHellEnemy : profaned ? ModContent.NPCType<ScornEater>() : (useHardmode ? NPCID.RedDevil : NPCID.Demon);
                    // Wall of Flesh, or chance for Providence if postML
                    int pickUltraRareHell = notInHell ? nonHellEnemy : profaned ? ModContent.NPCType<Providence>() : NPCID.WallofFlesh;
                    if (common) { npcIDsCommon.Add((pickCommonHell, notInHell ? 2 : snake ? 1 : 5, false)); } // Common NPC/s
                    else if (rare) { npcIDsRare.Add((pickRareHell, notInHell ? 10 : 3, false)); } // Rare NPC/s
                    else if (ultraRare) { npcIDsUltraRare.Add((pickUltraRareHell, notInHell ? 50 : 1, notInHell)); } // Ultra Rare NPC/s
                }
            }
            #endregion

            #region NPC spawn
            // Spawn the npc/s
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (npcIDsCommon.Count + npcIDsRare.Count + npcIDsUltraRare.Count == 0)
                {
                    npcIDsCommon.Add((fallbackNPC, 1, false));
                    common = true;
                }
                // Get a random NPC type and count from the list of npcs within the rarity
                (int, int, bool) finalNPCstats = common ? npcIDsCommon.ElementAt(Main.rand.Next(0, npcIDsCommon.Count)) :
                    rare ? npcIDsRare.ElementAt(Main.rand.Next(0, npcIDsRare.Count)) :
                    npcIDsUltraRare.ElementAt(Main.rand.Next(0, npcIDsUltraRare.Count));
                // Spawn the actual NPC/s
                for (int i = 0; i < finalNPCstats.Item2; i++)
                {
                    float baseSpeed = 2.5f;
                    NPC catched = Main.npc[(int)NPC.NewNPC(owner.GetSource_FromThis(), (int)bobber.Center.X, (int)bobber.Center.Y, finalNPCstats.Item1)];
                    // If there's more than one, send in random directions, otherwise pull them upwards
                    catched.velocity = Vector2.UnitY * -baseSpeed + (finalNPCstats.Item2 == 1 ? Vector2.Zero : -Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * Main.rand.NextFloat(baseSpeed / 2, baseSpeed * 2 * MathF.Pow(finalNPCstats.Item2, 0.5f)));
                    catched.Calamity().preventDrops = (finalNPCstats.Item3 ? Main.rand.NextBool(3) : false);
                }
                
            }
            #endregion
        }

        public static void DoTrustyOldRodVFX(Player owner, int bobberWhoAmI, int rarity = 1, bool Lava = false, bool Honey = false)
        {
            Projectile bobber = Main.projectile.FirstOrDefault(p => p.identity == bobberWhoAmI);
            bool common = rarity == 1;
            bool rare = rarity == 2;
            bool ultraRare = rarity == 3;

            float scale = (common ? 1 : rare ? 2.5f : 6);
            for (int i = 0; i < (int)(12 * scale); i++)
            {
                Color liquidColor = Lava ? Color.OrangeRed : Honey ? Color.Gold : Color.DodgerBlue;
                if (i % 4 == 0)
                {
                    Particle sprayParticle = new CustomSpark(bobber.Center + Vector2.UnitX * Main.rand.NextFloat(-10, 10) * scale, -Vector2.UnitY.RotatedByRandom(0.2f * MathF.Pow(scale, 1)) * Main.rand.NextFloat(1, 7) * scale, "CalamityMod/Particles/BloomCircle", true, (int)(Main.rand.Next(18, 24) * 5 * scale), Main.rand.NextFloat(0.3f, 0.5f) * scale, Color.Lerp(Color.Gray, liquidColor, Main.rand.NextFloat(0.5f, 1f)) * 0.4f, new Vector2(0.8f, 1f), true, false, 0, false, false);
                    GeneralParticleHandler.SpawnParticle(sprayParticle);
                }

                Vector2 sprayPos = bobber.Center + Vector2.UnitX * Main.rand.NextFloat(-10, 10) * scale;
                Dust spray = Dust.NewDustPerfect(sprayPos, ModContent.DustType<SquashDustPixelated>());
                spray.noGravity = false;
                spray.scale = Main.rand.NextFloat(1.3f, 1.5f) * scale;
                spray.velocity = -Vector2.UnitY.RotatedByRandom(0.2f * MathF.Pow(scale, 1)) * Main.rand.NextFloat(5, 14) * scale;
                spray.color = Color.Lerp(Color.Gray, liquidColor, Main.rand.NextFloat(0.3f, 0.8f)) * 0.2f;
                spray.noLight = true;
                spray.noLightEmittence = true;
                spray.fadeIn = 2 + scale;
            }
            owner.SetScreenshake(2f * scale);
            SoundStyle epicFail = new("CalamityMod/Sounds/Item/WaterSplash", 2);
            SoundEngine.PlaySound(epicFail with { Volume = 0.4f + 0.1f * scale, Pitch = 0.2f - 0.1f * scale }, bobber.Center);
            bobber.velocity += -Vector2.UnitY * (common ? 10 : rare ? 20 : 45);
        }
    }
}

using System.Collections.Generic;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.GreatSandShark;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
using ReLogic.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Systems.Collections
{
    [ReinitializeDuringResizeArrays]
    public static class CalamityNPCSets
    {
        private static SetFactory Factory = NPCID.Sets.Factory;

        /// <summary>
        /// If <see langword="true"/> for an NPC type, makes this NPC be susceptible to <see cref="BuffID.Confused"/>.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] CalamityNPCNotImmuneToConfused = Factory.CreateNamedSet("CalamityNPCNotImmuneToConfused")
            .Description("Makes this NPC be susceptible to Confused.")
            .RegisterBoolSet(NPCType<AeroSlime>(), NPCType<AstralachneaGround>(), NPCType<AstralachneaWall>(), NPCType<BloomSlime>(), NPCType<Bohldohr>(), NPCType<CalamityEye>(),
                NPCType<CrimulanBlightSlime>(), NPCType<Cryon>(), NPCType<CryoSlime>(), NPCType<DespairStone>(), NPCType<EbonianBlightSlime>(), NPCType<FearlessGoldfishWarrior>(),
                NPCType<HeatSpirit>(), NPCType<MantisShrimp>(), NPCType<OverloadedSoldier>(), NPCType<PerennialSlime>(), NPCType<RenegadeWarlock>(), NPCType<Rimehound>(), NPCType<Rotdog>(),
                NPCType<Scryllar>(), NPCType<ScryllarRage>(), NPCType<StellarCulex>(), NPCType<Stormlion>(), NPCType<SuperDummyNPC>(), NPCType<WulfrumGyrator>(), NPCType<WulfrumRover>());

        /// <summary>
        /// If <see langword="true"/> for an NPC type, forces this NPC to draw Calamity's debuff display, even if it is not a boss.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] ForceDrawDebuffDisplay = Factory.CreateNamedSet("ForceDrawDebuffDisplay")
            .Description("Allows drawing Calamity's debuff display, even if not a boss.")
            .RegisterBoolSet(NPCID.TargetDummy, NPCID.WallofFleshEye, NPCType<SuperDummyNPC>());

        /// <summary>
        /// If <see langword="true"/> for an NPC type, then that NPC is a bound Town NPC.<br/>
        /// Used to make them immune to enemy damage.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] BoundTownNPC = Factory.CreateNamedSet("BoundTownNPC")
            .Description("Labels this NPC as a bound town NPC, to prevent them from taking hostile damage.")
            .RegisterBoolSet(NPCID.BoundGoblin, NPCID.BoundWizard, NPCID.BoundMechanic, NPCID.SleepingAngler, NPCID.BartenderUnconscious, NPCID.WebbedStylist, NPCID.GolferRescue);

        /// <summary>
        /// If <see langword="true"/> for an NPC type, then that NPC will not provide increased Rage generation despite being considered a boss.<br/>
        /// Primarily used by worm boss body and tail segments to prevent Rage being extraordinarily easy to get.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] BossSegmentThatDoesNotGenerateRageFaster = Factory.CreateNamedSet("BossSegmentThatDoesNotGenerateRageFaster")
            .Description("Prevent this boss NPC from generating Rage faster.")
            .RegisterBoolSet(NPCType<DesertScourgeBody>(), NPCType<DesertScourgeTail>(), NPCType<AquaticScourgeBody>(), NPCType<AquaticScourgeBodyAlt>(), NPCType<AquaticScourgeTail>(),
                NPCType<AstrumDeusBody>(), NPCType<AstrumDeusTail>(), NPCType<StormWeaverBody>(), NPCType<StormWeaverTail>(), NPCType<DevourerofGodsBody>(), NPCType<DevourerofGodsTail>(),
                NPCType<ThanatosBody1>(), NPCType<ThanatosBody2>(), NPCType<ThanatosTail>(), NPCType<AresLaserCannon>(), NPCType<AresTeslaCannon>(), NPCType<AresPlasmaFlamethrower>(), NPCType<AresGaussNuke>());

        /// <summary>
        /// If <see langword="true"/> for an NPC type, allows the NPC to scale its health based on the Boss Health Boost Percentage config option, even if it is not a boss.<br/>
        /// Also allows the NPC to receive health scaling from having multiple players in Expert+.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] ScalesHealthLikeBoss = Factory.CreateNamedSet("ScalesHealthLikeBoss")
            .Description("Allows scaling health from vanilla Expert+ multiplayer scaling and Boss Health Boost Percentage config, even if not a boss.")
            .RegisterBoolSet(NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail, NPCID.SkeletronHand, NPCID.WallofFleshEye, NPCID.TheDestroyerBody, NPCID.TheDestroyerTail,
                NPCID.PrimeCannon, NPCID.PrimeLaser, NPCID.PrimeVice, NPCID.PrimeSaw, NPCID.GolemHead, NPCID.GolemHeadFree, NPCID.GolemFistRight, NPCID.GolemFistLeft, NPCID.Sharkron,
                NPCID.Sharkron2, NPCID.MoonLordHead, NPCID.MoonLordHand, NPCType<DarkEnergy>(), NPCType<BrimstoneHeart>(), NPCType<SoulSeeker>(), NPCType<SoulSeekerSupreme>(),
                NPCType<Cataclysm>(), NPCType<SupremeCataclysm>(), NPCType<Catastrophe>(), NPCType<SupremeCatastrophe>(), NPCType<SepulcherHead>(), NPCType<SepulcherBody>(),
                NPCType<SepulcherTail>(), NPCType<SepulcherArm>(), NPCType<SepulcherBodyEnergyBall>(), NPCType<PrimordialWyrmBody>(), NPCType<PrimordialWyrmBodyAlt>(), NPCType<PrimordialWyrmHead>(),
                NPCType<PrimordialWyrmTail>(), NPCType<AquaticAberration>(), NPCType<AnahitasIceShield>(), NPCType<CryogenShield>(), NPCType<OldDukeToothBall>(), NPCType<SulphurousSharkron>(),
                NPCType<DraconicSwarmer>(), NPCType<AureusSpawn>(), NPCType<Brimling>(), NPCType<CrabShroom>(), NPCType<DankCreeper>(), NPCType<HiveBlob>(), NPCType<DarkHeart>(),
                NPCType<DesertNuisanceBody>(), NPCType<DesertNuisanceHead>(), NPCType<DesertNuisanceTail>(), NPCType<DesertNuisanceBodyYoung>(), NPCType<DesertNuisanceHeadYoung>(),
                NPCType<DesertNuisanceTailYoung>(), NPCType<PolterPhantom>(), NPCType<PhantomFuckYou>(), NPCType<KingSlimeJewelRuby>(), NPCType<PlanterasFreeTentacle>(),
                NPCType<PlagueHomingMissile>(), NPCType<PlagueMine>(), NPCType<ProfanedRocks>(), NPCType<ProvSpawnDefense>(), NPCType<ProvSpawnOffense>(), NPCType<ProvSpawnHealer>(),
                NPCType<RockPillar>(), NPCType<FlamePillar>(), NPCType<CosmicMine>(), NPCType<CosmicLantern>(), NPCType<ProfanedGuardianDefender>(), NPCType<ProfanedGuardianHealer>(),
                NPCType<CorruptSlimeSpawn>(), NPCType<CorruptSlimeSpawn2>(), NPCType<CrimsonSlimeSpawn>(), NPCType<CrimsonSlimeSpawn2>(), NPCType<PerforatorHeadLarge>(),
                NPCType<PerforatorBodyLarge>(), NPCType<PerforatorTailLarge>(), NPCType<PerforatorHeadMedium>(), NPCType<PerforatorBodyMedium>(), NPCType<PerforatorTailMedium>(),
                NPCType<PerforatorHeadSmall>(), NPCType<PerforatorBodySmall>(), NPCType<PerforatorTailSmall>(), NPCType<EbonianPaladin>(), NPCType<CrimulanPaladin>(),
                NPCType<SplitEbonianPaladin>(), NPCType<SplitCrimulanPaladin>(), NPCType<SlimeGodCore>(), NPCType<RavagerBody>(), NPCType<RavagerClawLeft>(), NPCType<RavagerClawRight>(),
                NPCType<RavagerLegLeft>(), NPCType<RavagerLegRight>(), NPCType<RavagerHead>());

        /// <summary>
        /// If <see langword="true"/> for an NPC type, then that NPC will have its contact damage set to 0.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] DealsZeroContactDamage = Factory.CreateNamedSet("DealsZeroContactDamage")
            .Description("Makes this NPC deal 0 contact damage.")
            .RegisterBoolSet(NPCID.AngryNimbus, NPCID.DarkCaster, NPCID.FireImp, NPCID.Tim, NPCID.DesertDjinn, NPCID.DiabolistRed, NPCID.DiabolistWhite,
                NPCID.Necromancer, NPCID.NecromancerArmored, NPCID.RaggedCaster, NPCID.RaggedCasterOpenCoat, NPCID.RuneWizard, NPCID.GoblinSorcerer, NPCID.GoblinSummoner, NPCID.NebulaBrain,
                NPCID.PirateShipCannon, NPCID.MartianSaucer, NPCID.MartianSaucerCannon, NPCID.MartianSaucerCore, NPCID.MartianSaucerTurret, NPCID.Probe, NPCID.CultistBoss, NPCID.GolemHead,
                NPCID.GolemHeadFree, NPCID.MoonLordFreeEye, NPCID.BloodSquid, NPCID.PlanterasHook, NPCID.MourningWood, NPCID.Pumpking, NPCID.Everscream, NPCID.IceQueen, NPCID.SantaNK1, NPCID.AncientDoom);

        /// <summary>
        /// If <see langword="true"/> for an NPC type, then that NPC will have its damage reduced by 25% if the world is in Hardmode.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] NerfDamageInHardmode = Factory.CreateNamedSet("NerfDamageInHardmode")
            .Description("Makes this NPC have its contact damage reduced by 25% if the world is in Hardmode.")
            .RegisterBoolSet(NPCID.AnglerFish, NPCID.AngryTrapper, NPCID.Arapaima, NPCID.BlackRecluse, NPCID.BlackRecluseWall, NPCID.BloodJelly, NPCID.FungoFish, NPCID.GreenJellyfish,
                NPCID.Clinger, NPCID.ArmoredSkeleton, NPCID.ArmoredViking, NPCID.Mummy, NPCID.DarkMummy, NPCID.LightMummy, NPCID.BloodFeeder, NPCID.DesertBeast, NPCID.ChaosElemental,
                NPCID.BloodMummy, NPCID.CorruptSlime, NPCID.Corruptor, NPCID.Crimslime, NPCID.CrimsonAxe, NPCID.CursedHammer, NPCID.Derpling, NPCID.Herpling, NPCID.DiggerHead,
                NPCID.DesertGhoul, NPCID.DesertGhoulCorruption, NPCID.DesertGhoulCrimson, NPCID.DesertGhoulHallow, NPCID.DuneSplicerHead, NPCID.EnchantedSword, NPCID.FloatyGross,
                NPCID.GiantBat, NPCID.GiantFlyingFox, NPCID.FungiSpore, NPCID.GiantTortoise, NPCID.IceTortoise, NPCID.HoppinJack, NPCID.Mimic, NPCID.IlluminantBat, NPCID.IlluminantSlime,
                NPCID.JungleCreeper, NPCID.JungleCreeperWall, NPCID.DesertLamiaDark, NPCID.DesertLamiaLight, NPCID.MossHornet, NPCID.Moth, NPCID.PigronCorruption, NPCID.PigronCrimson,
                NPCID.PigronHallow, NPCID.Pixie, NPCID.PossessedArmor, NPCID.RockGolem, NPCID.DesertScorpionWalk, NPCID.DesertScorpionWall, NPCID.Slimer, NPCID.ToxicSludge, NPCID.Unicorn,
                NPCID.WanderingEye, NPCID.Werewolf, NPCID.Wolf, NPCID.SeekerHead, NPCID.Wraith, NPCID.ChatteringTeethBomb, NPCID.IceGolem, NPCID.RainbowSlime, NPCID.SandShark,
                NPCID.SandsharkCorrupt, NPCID.SandsharkCrimson, NPCID.SandsharkHallow, NPCID.ShadowFlameApparition, NPCID.Parrot, NPCID.PirateCorsair, NPCID.PirateDeckhand, NPCID.PirateGhost,
                NPCID.BlueArmoredBonesMace, NPCID.BlueArmoredBonesSword, NPCID.BoneLee, NPCID.DungeonSpirit, NPCID.FlyingSnake, NPCID.HellArmoredBones, NPCID.HellArmoredBonesSpikeShield,
                NPCID.HellArmoredBonesSword, NPCID.MisterStabby, NPCID.Butcher, NPCID.CreatureFromTheDeep, NPCID.DeadlySphere, NPCID.Frankenstein, NPCID.Fritz, NPCID.Psycho, NPCID.Reaper,
                NPCID.SwampThing, NPCID.ThePossessed, NPCID.Vampire, NPCID.VampireBat, NPCID.HeadlessHorseman, NPCID.Hellhound, NPCID.Poltergeist, NPCID.Scarecrow1, NPCID.Scarecrow2,
                NPCID.Scarecrow3, NPCID.Scarecrow4, NPCID.Scarecrow5, NPCID.Scarecrow6, NPCID.Scarecrow7, NPCID.Scarecrow8, NPCID.Scarecrow9, NPCID.Scarecrow10, NPCID.Splinterling,
                NPCID.Flocko, NPCID.GingerbreadMan, NPCID.Krampus, NPCID.Nutcracker, NPCID.NutcrackerSpinning, NPCID.PresentMimic, NPCID.Yeti, NPCID.ZombieElf, NPCID.ZombieElfBeard,
                NPCID.ZombieElfGirl, NPCID.BloodEelHead, NPCID.GoblinShark, NPCID.EyeballFlyingFish, NPCID.ZombieMerman);

        /// <summary>
        /// If <see langword="true"/> for an NPC type, then that NPC is a post-Plantera Dungeon enemy. This causes two separate changes to the enemy.<br/>
        /// First, it grants them a 20% chance to drop Ectoplasm upon death.<br/>
        /// Second, it multiplies their max health by 2.5x and increases their damage by a flat 30 if Moon Lord has been defeated.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] IsBuffedDungeonEnemy = Factory.CreateNamedSet("IsBuffedDungeonEnemy")
            .Description("Makes this NPC directly drop Ectoplasm on death, and buffs its health and damage after defeating Moon Lord.")
            .RegisterBoolSet(NPCID.SkeletonSniper, NPCID.TacticalSkeleton, NPCID.SkeletonCommando, NPCID.Paladin, NPCID.GiantCursedSkull, NPCID.BoneLee, NPCID.DiabolistWhite,
                NPCID.DiabolistRed, NPCID.NecromancerArmored, NPCID.Necromancer, NPCID.RaggedCasterOpenCoat, NPCID.RaggedCaster, NPCID.HellArmoredBonesSword, NPCID.HellArmoredBonesMace,
                NPCID.HellArmoredBonesSpikeShield, NPCID.HellArmoredBones, NPCID.BlueArmoredBonesSword, NPCID.BlueArmoredBonesNoPants, NPCID.BlueArmoredBonesMace, NPCID.BlueArmoredBones,
                NPCID.RustyArmoredBonesSwordNoArmor, NPCID.RustyArmoredBonesSword, NPCID.RustyArmoredBonesFlail, NPCID.RustyArmoredBonesAxe);

        /// <summary>
        /// If <see langword="true"/> for an NPC type, then that NPC is spawned in the Pumpkin Moon.<br/>
        /// This will multiply their max health by 3.5x and increase their damage by a flat 30 if Devourer of Gods has been defeated.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] IsBuffedPumpkinMoonEnemy = Factory.CreateNamedSet("IsBuffedPumpkinMoonEnemy")
            .Description("Makes this Pumpkin Moon enemy have buffed health and damage after defeating Devourer of Gods.")
            .RegisterBoolSet(NPCID.Scarecrow1, NPCID.Scarecrow2, NPCID.Scarecrow3, NPCID.Scarecrow4, NPCID.Scarecrow5, NPCID.Scarecrow6, NPCID.Scarecrow7, NPCID.Scarecrow8, NPCID.Scarecrow9,
                NPCID.Scarecrow10, NPCID.HeadlessHorseman, NPCID.MourningWood, NPCID.Splinterling, NPCID.Pumpking, NPCID.PumpkingBlade, NPCID.Hellhound, NPCID.Poltergeist);

        /// <summary>
        /// If <see langword="true"/> for an NPC type, then that NPC is spawned in the Frost Moon.<br/>
        /// This will multiply their max health by 2.5x and increase their damage by a flat 30 if Devourer of Gods has been defeated.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] IsBuffedFrostMoonEnemy = Factory.CreateNamedSet("IsBuffedFrostMoonEnemy")
            .Description("Makes this Frost Moon enemy have buffed health and damage after defeating Devourer of Gods.")
            .RegisterBoolSet(NPCID.ZombieElf, NPCID.ZombieElfBeard, NPCID.ZombieElfGirl, NPCID.PresentMimic, NPCID.GingerbreadMan, NPCID.Yeti, NPCID.Everscream, NPCID.IceQueen,
                NPCID.SantaNK1, NPCID.ElfCopter, NPCID.Nutcracker, NPCID.NutcrackerSpinning, NPCID.ElfArcher, NPCID.Krampus, NPCID.Flocko);

        /// <summary>
        /// If <see langword="true"/> for an NPC type, then that NPC is spawned in the Solar Eclipse.<br/>
        /// This will multiply their max health by 5x and increase their damage by a flat 30 if Devourer of Gods has been defeated.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] IsBuffedSolarEclipseEnemy = Factory.CreateNamedSet("IsBuffedSolarEclipseEnemy")
            .Description("Makes this Solar Eclipse enemy have buffed health and damage after defeating Devourer of Gods.")
            .RegisterBoolSet(NPCID.Eyezor, NPCID.Reaper, NPCID.Frankenstein, NPCID.SwampThing, NPCID.Vampire, NPCID.VampireBat, NPCID.Butcher, NPCID.CreatureFromTheDeep, NPCID.Fritz,
                NPCID.Nailhead, NPCID.Psycho, NPCID.DeadlySphere, NPCID.DrManFly, NPCID.ThePossessed, NPCID.Mothron, NPCID.MothronEgg, NPCID.MothronSpawn);

        /// <summary>
        /// If <see langword="true"/> for an NPC type, then the NPC will not be affected by slowing debuffs.<br/>
        /// Also used to prevent weapon pulling effects and Anarchy Blade's ability to instantly kill enemies that are below 50% health.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] ImmuneToSlowsAndOtherSpecialEffects = Factory.CreateNamedSet("ImmuneToSlowsAndOtherSpecialEffects")
            .Description("Makes this NPC immune to slowing debuffs and specific weapon effects.")
            .RegisterBoolSet(NPCID.KingSlime, NPCType<KingSlimeJewelRuby>(), NPCID.EyeofCthulhu, NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail,
                NPCID.BrainofCthulhu, NPCID.Creeper, NPCID.QueenBee, NPCID.Deerclops, NPCID.SkeletronHead, NPCID.SkeletronHand, NPCID.WallofFlesh, NPCID.WallofFleshEye,
                NPCID.PirateShipCannon, NPCID.QueenSlimeBoss, NPCID.Probe, NPCID.Retinazer, NPCID.Spazmatism, NPCID.SkeletronPrime, NPCID.PrimeCannon, NPCID.PrimeSaw, NPCID.PrimeLaser,
                NPCID.PrimeVice, NPCID.Plantera, NPCID.PlanterasTentacle, NPCType<PlanterasFreeTentacle>(), NPCID.Everscream, NPCID.SantaNK1, NPCID.IceQueen, NPCID.MourningWood,
                NPCID.Pumpking, NPCID.Mothron, NPCID.Golem, NPCID.GolemHead, NPCID.GolemHeadFree, NPCID.GolemFistRight, NPCID.GolemFistLeft, NPCID.MartianSaucerCore, NPCID.MartianSaucerCannon,
                NPCID.MartianSaucerTurret, NPCID.DukeFishron, NPCID.Sharkron, NPCID.Sharkron2, NPCID.HallowBoss, NPCID.CultistBoss, NPCID.CultistDragonHead, NPCID.CultistDragonBody1,
                NPCID.CultistDragonBody2, NPCID.CultistDragonBody3, NPCID.CultistDragonBody4, NPCID.CultistDragonTail, NPCID.AncientCultistSquidhead, NPCID.MoonLordHead, NPCID.MoonLordHand,
                NPCID.MoonLordCore, NPCID.MoonLordFreeEye, NPCID.DD2WyvernT1, NPCID.DD2WyvernT2, NPCID.DD2WyvernT3, NPCID.DD2DarkMageT1, NPCID.DD2DarkMageT3, NPCID.DD2SkeletonT1,
                NPCID.DD2SkeletonT3, NPCID.DD2WitherBeastT2, NPCID.DD2WitherBeastT3, NPCID.DD2DrakinT2, NPCID.DD2DrakinT3, NPCID.DD2KoboldWalkerT2, NPCID.DD2KoboldWalkerT3, NPCID.DD2KoboldFlyerT2,
                NPCID.DD2KoboldFlyerT3, NPCID.DD2OgreT2, NPCID.DD2OgreT3, NPCID.DD2Betsy,
                NPCType<DesertNuisanceHead>(), NPCType<DesertNuisanceBody>(), NPCType<DesertNuisanceTail>(), NPCType<DesertNuisanceHeadYoung>(), NPCType<DesertNuisanceBodyYoung>(),
                NPCType<DesertNuisanceTailYoung>(), NPCType<GiantClam>(), NPCType<PerforatorHeadLarge>(), NPCType<PerforatorHeadMedium>(), NPCType<PerforatorHeadSmall>(),
                NPCType<PerforatorBodyLarge>(), NPCType<PerforatorBodyMedium>(), NPCType<PerforatorBodySmall>(), NPCType<PerforatorTailLarge>(), NPCType<PerforatorTailMedium>(),
                NPCType<PerforatorTailSmall>(), NPCType<EbonianPaladin>(), NPCType<CrimulanPaladin>(), NPCType<SplitEbonianPaladin>(), NPCType<SplitCrimulanPaladin>(), NPCType<EarthElemental>(),
                NPCType<CloudElemental>(), NPCType<CryogenShield>(), NPCType<AquaticScourgeHead>(), NPCType<AquaticScourgeBody>(), NPCType<AquaticScourgeBodyAlt>(), NPCType<AquaticScourgeTail>(),
                NPCType<CragmawMire>(), NPCType<Cataclysm>(), NPCType<Catastrophe>(), NPCType<SoulSeeker>(), NPCType<GreatSandShark>(), NPCType<AnahitasIceShield>(), NPCType<AureusSpawn>(),
                NPCType<PlaguebringerMiniboss>(), NPCType<PlagueHomingMissile>(), NPCType<PlagueMine>(), NPCType<RavagerClawLeft>(), NPCType<RavagerClawRight>(), NPCType<RavagerLegLeft>(),
                NPCType<RavagerLegRight>(), NPCType<RockPillar>(), NPCType<RavagerHead>(), NPCType<ProfanedGuardianDefender>(), NPCType<ProfanedGuardianHealer>(), NPCType<DraconicSwarmer>(),
                NPCType<ProvSpawnDefense>(), NPCType<ProvSpawnHealer>(), NPCType<ProvSpawnOffense>(), NPCType<BobbitWormHead>(), NPCType<Mauler>(), NPCType<ColossalSquid>(), NPCType<ReaperShark>(),
                NPCType<EidolonWyrmHead>(), NPCType<NuclearTerror>(), NPCType<OldDukeToothBall>(), NPCType<SulphurousSharkron>(), NPCType<SupremeCataclysm>(), NPCType<SupremeCatastrophe>(), NPCType<SoulSeekerSupreme>());

        /// <summary>
        /// If <see langword="true"/> for an NPC type, <see cref="ModNPC.CheckDead"/> or <see cref="GlobalNPC.CheckDead(NPC)"/> will be called on this NPC even if <see cref="NPC.realLife"/> is set.
        /// </summary>
        public static bool[] DoCheckDeadRegardlessRealLife = Factory.CreateNamedSet("DoCheckDeadRegardlessRealLife")
            .Description("Makes this NPC always call CheckDead, even if it sets realLife.")
            .RegisterBoolSet(NPCType<DevourerofGodsBody>(), NPCType<DevourerofGodsTail>());

        /// <summary>
        /// If <see langword="true"/> for an NPC type, <see cref="CalamityUtils.IsAnEnemy(NPC, bool, bool, bool)"/> will not count this NPC as an enemy.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] DontCountAsEnemy = Factory.CreateNamedSet("DontCountAsEnemy")
            .Description("Prevents Calamity's IsAnEnemy method from considering this NPC an enemy.")
            .RegisterBoolSet(NPCID.TargetDummy, NPCType<SuperDummyNPC>());

        /// <summary>
        /// The intended kill times of Calamity bosses, in frames
        /// </summary>
        public static int[] BossKillTimes = Factory.CreateNamedSet("BossKillTimes")
            .Description("The intended kill times of Calamity bosses, in frames")
            .RegisterIntSet(0,
                NPCID.KingSlime, 5400, // 1:30 (90 seconds)
                NPCID.EyeofCthulhu, 5400, // 1:30 (90 seconds)
                NPCID.EaterofWorldsHead, 7200, // 2:00 (120 seconds)
                NPCID.EaterofWorldsBody, 7200,
                NPCID.EaterofWorldsTail, 7200,
                NPCID.BrainofCthulhu, 7200, // 2:00 (120 seconds, total length of fight including Creepers phase)
                NPCID.Creeper, 1800, // 0:30 (30 seconds, length of Creepers phase)
                NPCID.Deerclops, 5400, // 1:30 (90 seconds)
                NPCID.QueenBee, 7200, // 2:00 (120 seconds)
                NPCID.SkeletronHead, 7200, // 2:00 (120 seconds)
                NPCID.WallofFlesh, 7200, // 2:00 (120 seconds)
                NPCID.WallofFleshEye, 7200,
                NPCID.QueenSlimeBoss, 7200, // 2:00 (120 seconds)
                NPCID.Spazmatism, 10800, // 3:00 (180 seconds)
                NPCID.Retinazer, 10800,
                NPCID.TheDestroyer, 10800, // 3:00 (180 seconds)
                NPCID.TheDestroyerBody, 10800,
                NPCID.TheDestroyerTail, 10800,
                NPCID.SkeletronPrime, 10800, // 3:00 (180 seconds)
                NPCID.Plantera, 10800, // 3:00 (180 seconds)
                NPCID.Golem, 9000, // 2:30 (150 seconds)
                NPCID.GolemHead, 3600, // 1:00 (60 seconds)
                NPCID.DukeFishron, 9000, // 2:30 (150 seconds)
                NPCID.HallowBoss, 10800, // 3:00 (180 seconds)
                NPCID.CultistBoss, 9000, // 2:30 (150 seconds)
                NPCID.MoonLordCore, 14400, // 4:00 (240 seconds)
                NPCID.MoonLordHand, 7200, // 2:00 (120 seconds)
                NPCID.MoonLordHead, 7200, // 2:00 (120 seconds)

                //
                // CALAMITY BOSSES
                //
                NPCType<DesertScourgeHead>(), 5400, // 1:30 (90 seconds)
                NPCType<DesertScourgeBody>(), 5400,
                NPCType<DesertScourgeTail>(), 5400,
                NPCType<Crabulon>(), 5400, // 1:30 (90 seconds)
                NPCType<HiveMind>(), 7200, // 2:00 (120 seconds)
                NPCType<PerforatorHive>(), 7200, // 2:00 (120 seconds)
                NPCType<SlimeGodCore>(), 9000, // 2:30 (150 seconds) -- total length of Slime God fight
                NPCType<EbonianPaladin>(), 4500, // 1:15 (75 seconds)
                NPCType<CrimulanPaladin>(), 4500, // 1:15 (75 seconds)
                NPCType<SplitEbonianPaladin>(), 4500, // 1:15 (75 seconds) -- split slimes should spawn at 1:15 and die at around 2:30
                NPCType<SplitCrimulanPaladin>(), 4500, // 1:15 (75 seconds)
                NPCType<Cryogen>(), 10800, // 3:00 (180 seconds)
                NPCType<AquaticScourgeHead>(), 9000, // 2:30 (150 seconds)
                NPCType<AquaticScourgeBody>(), 9000,
                NPCType<AquaticScourgeBodyAlt>(), 9000,
                NPCType<AquaticScourgeTail>(), 9000,
                NPCType<BrimstoneElemental>(), 10800, // 3:00 (180 seconds)
                NPCType<CalamitasClone>(), 10800, // 3:00 (180 seconds)
                NPCType<Anahita>(), 10800, // 3:00 (180 seconds)
                NPCType<Leviathan>(), 10800,
                NPCType<AstrumAureus>(), 10800, // 3:00 (180 seconds)
                NPCType<AstrumDeusHead>(), 7200, // 2:00 (120 seconds) -- first phase is 1:00
                NPCType<AstrumDeusBody>(), 7200,
                NPCType<AstrumDeusTail>(), 7200,
                NPCType<PlaguebringerGoliath>(), 10800, // 3:00 (180 seconds)
                NPCType<RavagerBody>(), 10800, // 3:00 (180 seconds)
                NPCType<ProfanedGuardianCommander>(), 7200, // 2:00 (120 seconds)
                NPCType<Dragonfolly>(), 7200, // 2:00 (120 seconds)
                NPCType<Providence>(), 14400, // 4:00 (240 seconds)
                NPCType<CeaselessVoid>(), 10800, // 3:00 (180 seconds)
                NPCType<DarkEnergy>(), 1200, // 0:20 (20 seconds)
                NPCType<StormWeaverHead>(), 8100, // 2:15 (135 seconds)
                NPCType<StormWeaverBody>(), 8100,
                NPCType<StormWeaverTail>(), 8100,
                NPCType<Signus>(), 7200, // 2:00 (120 seconds)
                NPCType<Polterghast>(), 10800, // 3:00 (180 seconds)
                NPCType<OldDuke>(), 10800, // 3:00 (180 seconds)
                NPCType<DevourerofGodsHead>(), 14400, // 4:00 (240 seconds)
                NPCType<DevourerofGodsBody>(), 14400, // DoG Phase 1 is 1:30, DoG Phase 2 is 2:30
                NPCType<DevourerofGodsTail>(), 14400,
                NPCType<Yharon>(), 14400, // 4:00 (240 seconds)
                NPCType<Apollo>(), 21600, // 6:00 (360 seconds)
                NPCType<Artemis>(), 21600,
                NPCType<AresBody>(), 21600, // 6:00 (360 seconds)
                NPCType<AresGaussNuke>(), 21600,
                NPCType<AresLaserCannon>(), 21600,
                NPCType<AresPlasmaFlamethrower>(), 21600,
                NPCType<AresTeslaCannon>(), 21600,
                NPCType<ThanatosHead>(), 21600, // 6:00 (360 seconds)
                NPCType<ThanatosBody1>(), 21600,
                NPCType<ThanatosBody2>(), 21600,
                NPCType<ThanatosTail>(), 21600,
                NPCType<SupremeCalamitas>(), 18000, // 5:00 (300 seconds)
                NPCType<PrimordialWyrmHead>(), 18000 // 5:00 (300 seconds)
             );

        /// <summary>
        /// Associates an NPC type with the base value of their max health in Boss Rush.<br/>
        /// If an NPC type is not a key in this dictionary, then it will use its standard max health value.
        /// </summary>
        public static Dictionary<int, int> BossRushHealth = new Dictionary<int, int>
        {
            // Tier 1
            { NPCID.KingSlime, 300000 }, // 30 seconds
            { NPCID.BlueSlime, 3600 },
            { NPCID.SlimeSpiked, 7200 },
            { NPCID.GreenSlime, 2700 },
            { NPCID.RedSlime, 5400 },
            { NPCID.PurpleSlime, 7200 },
            { NPCID.YellowSlime, 6300 },
            { NPCID.IceSlime, 4500 },
            { NPCID.UmbrellaSlime, 5400 },
            { NPCID.RainbowSlime, 30000 },
            { NPCID.Pinky, 15000 },
            { NPCType<KingSlimeJewelRuby>(), 21000 },
            { NPCID.EyeofCthulhu, 450000 }, // 30 seconds
            { NPCID.ServantofCthulhu, 6000 },
            { NPCID.EaterofWorldsHead, 15000 }, // 30 seconds + immunity timer at start
            { NPCID.EaterofWorldsBody, 15000 },
            { NPCID.EaterofWorldsTail, 15000 },
            { NPCID.BrainofCthulhu, 100000 }, // 30 seconds with creepers
            { NPCID.Creeper, 10000 },
            { NPCID.QueenBee, 315000 }, // 30 seconds
            { NPCID.Bee, 3000 },
            { NPCID.BeeSmall, 2000 },
            { NPCID.BigHornetHoney, 10000 },
            { NPCID.HornetHoney, 7500 },
            { NPCID.LittleHornetHoney, 5000 },
            { NPCID.Deerclops, 315000 }, // 30 seconds
            { NPCID.SkeletronHead, 150000 }, // 30 seconds
            { NPCID.SkeletronHand, 60000 },
            { NPCID.WallofFlesh, 450000 }, // 30 seconds
            { NPCID.WallofFleshEye, 450000 },
            { NPCID.TheHungry, 10000 },
            { NPCID.TheHungryII, 5000 },
            { NPCID.LeechHead, 5000 },
            { NPCID.LeechBody, 5000 },
            { NPCID.LeechTail, 5000 },
            // Tier 2
            { NPCID.QueenSlimeBoss, 200000 }, // 30 seconds
            { NPCID.QueenSlimeMinionBlue, 6000 },
            { NPCID.QueenSlimeMinionPink, 6000 },
            { NPCID.QueenSlimeMinionPurple, 5000 },
            { NPCID.Spazmatism, 150000 }, // 30 seconds
            { NPCID.Retinazer, 125000 },
            { NPCID.TheDestroyer, 600000 }, // 30 seconds + immunity timer at start
            { NPCID.TheDestroyerBody, 600000 },
            { NPCID.TheDestroyerTail, 600000 },
            { NPCID.Probe, 10000 },
            { NPCID.SkeletronPrime, 160000 }, // 30 seconds
            { NPCID.PrimeVice, 54000 },
            { NPCID.PrimeCannon, 45000 },
            { NPCID.PrimeSaw, 45000 },
            { NPCID.PrimeLaser, 38000 },
            { NPCID.Plantera, 160000 }, // 30 seconds
            { NPCID.PlanterasTentacle, 5000 },
            { NPCType<PlanterasFreeTentacle>(), 5000 },
            // Tier 3
            { NPCID.Golem, 100000 }, // 30 seconds
            { NPCID.GolemHead, 70000 },
            { NPCID.GolemFistLeft, 30000 },
            { NPCID.GolemFistRight, 30000 },
            { NPCID.HallowBoss, 200000 }, // 30 seconds
            { NPCID.DukeFishron, 290000 }, // 30 seconds
            { NPCID.CultistBoss, 220000 }, // 30 seconds
            { NPCID.CultistDragonHead, 60000 },
            { NPCID.CultistDragonBody1, 60000 },
            { NPCID.CultistDragonBody2, 60000 },
            { NPCID.CultistDragonBody3, 60000 },
            { NPCID.CultistDragonBody4, 60000 },
            { NPCID.CultistDragonTail, 60000 },
            { NPCID.AncientCultistSquidhead, 50000 },
            { NPCID.MoonLordCore, 160000 }, // 1 minute
            { NPCID.MoonLordHand, 45000 },
            { NPCID.MoonLordHead, 60000 },
            { NPCID.MoonLordLeechBlob, 800 }
        };

        /// <summary>
        /// Associates an NPC type with an ID number used for Calamity's Speedrun Timer. Used for drawing the correct map icon for the boss.<br/>
        /// If an NPC type is not a key in this dictionary, then it will not be displayed on the Speedrun Timer.
        /// </summary>
        public static Dictionary<int, int> BossSpeedrunTimerID = new Dictionary<int, int>
        {
            { NPCID.KingSlime, 1 },
            { NPCType<DesertScourgeHead>(), 2 },
            { NPCID.EyeofCthulhu, 3 },
            { NPCType<Crabulon>(), 4 },
            { NPCID.EaterofWorldsHead, 5 },
            { NPCID.EaterofWorldsBody, 5 },
            { NPCID.EaterofWorldsTail, 5 },
            { NPCID.BrainofCthulhu, 6 },
            { NPCType<HiveMind>(), 7 },
            { NPCType<PerforatorHive>(), 8 },
            { NPCID.QueenBee, 9 },
            { NPCID.SkeletronHead, 10 },
            { NPCType<SlimeGodCore>(), 11 },
            { NPCType<SplitEbonianPaladin>(), 11 },
            { NPCType<SplitCrimulanPaladin>(), 11 },
            { NPCID.WallofFlesh, 12 },
            { NPCType<Cryogen>(), 13 },
            { NPCID.Retinazer, 14 },
            { NPCID.Spazmatism, 14 },
            { NPCType<AquaticScourgeHead>(), 15 },
            { NPCID.TheDestroyer, 16 },
            { NPCType<BrimstoneElemental>(), 17 },
            { NPCID.SkeletronPrime, 18 },
            { NPCType<CalamitasClone>(), 19 },
            { NPCID.Plantera, 20 },
            { NPCType<Leviathan>(), 21 },
            { NPCType<Anahita>(), 21 },
            { NPCType<AstrumAureus>(), 22 },
            { NPCID.Golem, 23 },
            { NPCType<PlaguebringerGoliath>(), 24 },
            { NPCID.DukeFishron, 25 },
            { NPCType<RavagerBody>(), 26 },
            { NPCID.CultistBoss, 27 },
            { NPCType<AstrumDeusHead>(), 28 },
            { NPCID.MoonLordCore, 29 },
            { NPCType<ProfanedGuardianCommander>(), 30 },
            { NPCType<Dragonfolly>(), 31 },
            { NPCType<Providence>(), 32 },
            { NPCType<CeaselessVoid>(), 33 },
            { NPCType<StormWeaverHead>(), 34 },
            { NPCType<Signus>(), 35 },
            { NPCType<Polterghast>(), 36 },
            { NPCType<OldDuke>(), 37 },
            { NPCType<DevourerofGodsHead>(), 38 },
            { NPCType<Yharon>(), 39 },
            { NPCType<SupremeCalamitas>(), 40 },
            { NPCType<AresBody>(), 41 },
            { NPCType<ThanatosHead>(), 41 },
            { NPCType<Artemis>(), 41 },
            { NPCType<Apollo>(), 41 },
            { NPCID.QueenSlimeBoss, 42 },
            { NPCID.HallowBoss, 43 },
            { NPCID.Deerclops, 44 }
        };
    }
}

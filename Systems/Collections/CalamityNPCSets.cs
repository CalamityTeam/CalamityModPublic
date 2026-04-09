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
        public static SetFactory Factory = new SetFactory(NPCLoader.NPCCount, "CalamityMod/NPCID", Search);
        public static IdDictionary Search = IdDictionary.Create<NPCID, int>();

        /// <summary>
        /// If <see langword="true"/> for an NPC type, makes this NPC be susceptible to <see cref="BuffID.Confused"/>.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] CalamityNPCNotImmuneToConfused = Factory.CreateBoolSet(NPCType<AeroSlime>(), NPCType<AstralachneaGround>(), NPCType<AstralachneaWall>(), NPCType<BloomSlime>(),
                NPCType<Bohldohr>(), NPCType<CalamityEye>(), NPCType<CrimulanBlightSlime>(), NPCType<Cryon>(), NPCType<CryoSlime>(), NPCType<DespairStone>(), NPCType<EbonianBlightSlime>(),
                NPCType<FearlessGoldfishWarrior>(), NPCType<HeatSpirit>(), NPCType<MantisShrimp>(), NPCType<OverloadedSoldier>(), NPCType<PerennialSlime>(), NPCType<RenegadeWarlock>(),
                NPCType<Rimehound>(), NPCType<Rotdog>(), NPCType<Scryllar>(), NPCType<ScryllarRage>(), NPCType<StellarCulex>(), NPCType<Stormlion>(), NPCType<SuperDummyNPC>(),
                NPCType<WulfrumGyrator>(), NPCType<WulfrumRover>());

        /// <summary>
        /// If <see langword="true"/> for an NPC type, forces this NPC to draw Calamity's debuff display, even if it is not a boss.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] ForceDrawDebuffDisplay = Factory.CreateBoolSet(NPCID.TargetDummy, NPCID.WallofFleshEye, NPCType<SuperDummyNPC>());

        /// <summary>
        /// If <see langword="true"/> for an NPC type, then that NPC will not provide increased Rage generation despite being considered a boss.<br/>
        /// Primarily used by worm boss body and tail segments to prevent Rage being extraordinarily easy to get.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] BossSegmentThatDoesNotGenerateRageFaster = Factory.CreateBoolSet(NPCType<DesertScourgeBody>(), NPCType<DesertScourgeTail>(), NPCType<AquaticScourgeBody>(),
                NPCType<AquaticScourgeBodyAlt>(), NPCType<AquaticScourgeTail>(), NPCType<AstrumDeusBody>(), NPCType<AstrumDeusTail>(), NPCType<StormWeaverBody>(),
                NPCType<StormWeaverTail>(), NPCType<DevourerofGodsBody>(), NPCType<DevourerofGodsTail>(), NPCType<ThanatosBody1>(), NPCType<ThanatosBody2>(), NPCType<ThanatosTail>(),
                NPCType<AresLaserCannon>(), NPCType<AresTeslaCannon>(), NPCType<AresPlasmaFlamethrower>(), NPCType<AresGaussNuke>());

        /// <summary>
        /// If <see langword="true"/> for an NPC type, allows the NPC to scale its health based on the Boss Health Boost Percentage config option, even if it is not a boss.<br/>
        /// Also allows the NPC to receive health scaling from having multiple players in Expert+.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] ScalesHealthLikeBoss = Factory.CreateBoolSet(NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail, NPCID.SkeletronHand,
                NPCID.WallofFleshEye, NPCID.TheDestroyerBody, NPCID.TheDestroyerTail, NPCID.PrimeCannon, NPCID.PrimeLaser, NPCID.PrimeVice, NPCID.PrimeSaw, NPCID.GolemHead,
                NPCID.GolemHeadFree, NPCID.GolemFistRight, NPCID.GolemFistLeft, NPCID.Sharkron, NPCID.Sharkron2, NPCID.MoonLordHead, NPCID.MoonLordHand, NPCType<DarkEnergy>(),
                NPCType<BrimstoneHeart>(), NPCType<SoulSeeker>(), NPCType<SoulSeekerSupreme>(), NPCType<Cataclysm>(), NPCType<SupremeCataclysm>(), NPCType<Catastrophe>(),
                NPCType<SupremeCatastrophe>(), NPCType<SepulcherHead>(), NPCType<SepulcherBody>(), NPCType<SepulcherTail>(), NPCType<SepulcherArm>(), NPCType<SepulcherBodyEnergyBall>(),
                NPCType<PrimordialWyrmBody>(), NPCType<PrimordialWyrmBodyAlt>(), NPCType<PrimordialWyrmHead>(), NPCType<PrimordialWyrmTail>(), NPCType<AquaticAberration>(),
                NPCType<AnahitasIceShield>(), NPCType<CryogenShield>(), NPCType<OldDukeToothBall>(), NPCType<SulphurousSharkron>(), NPCType<DraconicSwarmer>(), NPCType<AureusSpawn>(),
                NPCType<Brimling>(), NPCType<CrabShroom>(), NPCType<DankCreeper>(),
                NPCType<HiveBlob>(), NPCType<DarkHeart>(), NPCType<DesertNuisanceBody>(), NPCType<DesertNuisanceHead>(), NPCType<DesertNuisanceTail>(),
                NPCType<DesertNuisanceBodyYoung>(), NPCType<DesertNuisanceHeadYoung>(), NPCType<DesertNuisanceTailYoung>(), NPCType<PolterPhantom>(), NPCType<PhantomFuckYou>(),
                NPCType<KingSlimeJewelRuby>(), NPCType<PlanterasFreeTentacle>(), NPCType<PlagueHomingMissile>(),
                NPCType<PlagueMine>(), NPCType<ProfanedRocks>(), NPCType<ProvSpawnDefense>(), NPCType<ProvSpawnOffense>(), NPCType<ProvSpawnHealer>(), NPCType<RockPillar>(),
                NPCType<FlamePillar>(), NPCType<CosmicMine>(), NPCType<CosmicLantern>(), NPCType<ProfanedGuardianDefender>(), NPCType<ProfanedGuardianHealer>(), NPCType<CorruptSlimeSpawn>(),
                NPCType<CorruptSlimeSpawn2>(), NPCType<CrimsonSlimeSpawn>(), NPCType<CrimsonSlimeSpawn2>(), NPCType<PerforatorHeadLarge>(), NPCType<PerforatorBodyLarge>(),
                NPCType<PerforatorTailLarge>(), NPCType<PerforatorHeadMedium>(), NPCType<PerforatorBodyMedium>(), NPCType<PerforatorTailMedium>(), NPCType<PerforatorHeadSmall>(),
                NPCType<PerforatorBodySmall>(), NPCType<PerforatorTailSmall>(), NPCType<EbonianPaladin>(), NPCType<CrimulanPaladin>(), NPCType<SplitEbonianPaladin>(),
                NPCType<SplitCrimulanPaladin>(), NPCType<SlimeGodCore>(), NPCType<RavagerBody>(), NPCType<RavagerClawLeft>(), NPCType<RavagerClawRight>(), NPCType<RavagerLegLeft>(),
                NPCType<RavagerLegRight>(), NPCType<RavagerHead>());

        /// <summary>
        /// If <see langword="true"/> for an NPC type, then that NPC will have its contact damage set to 0.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] DealsZeroContactDamage = Factory.CreateBoolSet(NPCID.Harpy, NPCID.Salamander, NPCID.Salamander2, NPCID.Salamander3, NPCID.Salamander4, NPCID.Salamander5,
                NPCID.Salamander6, NPCID.Salamander7, NPCID.Salamander8, NPCID.Salamander9, NPCID.GiantCursedSkull, NPCID.FungiBulb, NPCID.GiantFungiBulb, NPCID.IcyMerman,
                NPCID.AngryNimbus, NPCID.SandElemental, NPCID.DarkCaster, NPCID.FireImp, NPCID.Tim, NPCID.CultistArcherBlue, NPCID.DesertDjinn, NPCID.DiabolistRed, NPCID.DiabolistWhite,
                NPCID.Gastropod, NPCID.IceElemental, NPCID.IchorSticker, NPCID.Necromancer, NPCID.NecromancerArmored, NPCID.RaggedCaster, NPCID.RaggedCasterOpenCoat, NPCID.RuneWizard,
                NPCID.SkeletonArcher, NPCID.SkeletonCommando, NPCID.SkeletonSniper, NPCID.TacticalSkeleton, NPCID.Clown, NPCID.GoblinArcher, NPCID.GoblinSorcerer, NPCID.GoblinSummoner,
                NPCID.PirateCrossbower, NPCID.PirateDeadeye, NPCID.PirateCaptain, NPCID.SnowmanGangsta, NPCID.SnowBalla, NPCID.DrManFly, NPCID.Eyezor, NPCID.Nailhead, NPCID.BrainScrambler,
                NPCID.GigaZapper, NPCID.RayGunner, NPCID.ScutlixRider, NPCID.MartianWalker, NPCID.MartianTurret, NPCID.ElfCopter, NPCID.ElfArcher, NPCID.NebulaBrain, NPCID.NebulaSoldier,
                NPCID.StardustCellSmall, NPCID.StardustJellyfishBig, NPCID.StardustSoldier, NPCID.StardustSpiderBig, NPCID.VortexHornetQueen, NPCID.VortexRifleman, NPCID.VortexSoldier,
                NPCID.PirateShipCannon, NPCID.MartianSaucer, NPCID.MartianSaucerCannon, NPCID.MartianSaucerCore, NPCID.MartianSaucerTurret, NPCID.Probe, NPCID.CultistBoss, NPCID.GolemHead,
                NPCID.GolemHeadFree, NPCID.MoonLordFreeEye, NPCID.BloodSquid, NPCID.PlanterasHook, NPCID.Dandelion, NPCID.DD2DarkMageT1, NPCID.DD2DarkMageT3, NPCID.DD2OgreT2,
                NPCID.DD2OgreT3, NPCID.DD2GoblinBomberT1, NPCID.DD2GoblinBomberT2, NPCID.DD2GoblinBomberT3, NPCID.DD2JavelinstT1, NPCID.DD2JavelinstT2, NPCID.DD2JavelinstT3,
                NPCID.DD2KoboldWalkerT2, NPCID.DD2KoboldWalkerT3, NPCID.DD2DrakinT2, NPCID.DD2DrakinT3, NPCID.DD2KoboldFlyerT2, NPCID.DD2KoboldFlyerT3, NPCID.DD2WitherBeastT2,
                NPCID.DD2WitherBeastT3, NPCID.DD2LightningBugT3, NPCID.MourningWood, NPCID.Pumpking, NPCID.Everscream, NPCID.IceQueen, NPCID.SantaNK1, NPCID.AncientDoom);

        /// <summary>
        /// If <see langword="true"/> for an NPC type, then that NPC will have its damage reduced by 25% if the world is in Hardmode.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] NerfDamageInHardmode = Factory.CreateBoolSet(NPCID.AnglerFish, NPCID.AngryTrapper, NPCID.Arapaima, NPCID.BlackRecluse, NPCID.BlackRecluseWall,
                NPCID.BloodJelly, NPCID.FungoFish, NPCID.GreenJellyfish, NPCID.Clinger, NPCID.ArmoredSkeleton, NPCID.ArmoredViking, NPCID.Mummy, NPCID.DarkMummy, NPCID.LightMummy,
                NPCID.BloodFeeder, NPCID.DesertBeast, NPCID.ChaosElemental, NPCID.BloodMummy, NPCID.CorruptSlime, NPCID.Corruptor, NPCID.Crimslime, NPCID.CrimsonAxe, NPCID.CursedHammer,
                NPCID.Derpling, NPCID.Herpling, NPCID.DiggerHead, NPCID.DesertGhoul, NPCID.DesertGhoulCorruption, NPCID.DesertGhoulCrimson, NPCID.DesertGhoulHallow, NPCID.DuneSplicerHead,
                NPCID.EnchantedSword, NPCID.FloatyGross, NPCID.GiantBat, NPCID.GiantFlyingFox, NPCID.FungiSpore, NPCID.GiantTortoise, NPCID.IceTortoise, NPCID.HoppinJack, NPCID.Mimic,
                NPCID.IlluminantBat, NPCID.IlluminantSlime, NPCID.JungleCreeper, NPCID.JungleCreeperWall, NPCID.DesertLamiaDark, NPCID.DesertLamiaLight, NPCID.MossHornet, NPCID.Moth,
                NPCID.PigronCorruption, NPCID.PigronCrimson, NPCID.PigronHallow, NPCID.Pixie, NPCID.PossessedArmor, NPCID.RockGolem, NPCID.DesertScorpionWalk, NPCID.DesertScorpionWall,
                NPCID.Slimer, NPCID.ToxicSludge, NPCID.Unicorn, NPCID.WanderingEye, NPCID.Werewolf, NPCID.Wolf, NPCID.SeekerHead, NPCID.Wraith, NPCID.ChatteringTeethBomb, NPCID.IceGolem,
                NPCID.RainbowSlime, NPCID.SandShark, NPCID.SandsharkCorrupt, NPCID.SandsharkCrimson, NPCID.SandsharkHallow, NPCID.ShadowFlameApparition, NPCID.Parrot, NPCID.PirateCorsair,
                NPCID.PirateDeckhand, NPCID.PirateGhost, NPCID.BlueArmoredBonesMace, NPCID.BlueArmoredBonesSword, NPCID.BoneLee, NPCID.DungeonSpirit, NPCID.FlyingSnake, NPCID.HellArmoredBones,
                NPCID.HellArmoredBonesSpikeShield, NPCID.HellArmoredBonesSword, NPCID.MisterStabby, NPCID.Butcher, NPCID.CreatureFromTheDeep, NPCID.DeadlySphere, NPCID.Frankenstein,
                NPCID.Fritz, NPCID.Psycho, NPCID.Reaper, NPCID.SwampThing, NPCID.ThePossessed, NPCID.Vampire, NPCID.VampireBat, NPCID.HeadlessHorseman, NPCID.Hellhound, NPCID.Poltergeist,
                NPCID.Scarecrow1, NPCID.Scarecrow2, NPCID.Scarecrow3, NPCID.Scarecrow4, NPCID.Scarecrow5, NPCID.Scarecrow6, NPCID.Scarecrow7, NPCID.Scarecrow8, NPCID.Scarecrow9, NPCID.Scarecrow10,
                NPCID.Splinterling, NPCID.Flocko, NPCID.GingerbreadMan, NPCID.Krampus, NPCID.Nutcracker, NPCID.NutcrackerSpinning, NPCID.PresentMimic, NPCID.Yeti, NPCID.ZombieElf, NPCID.ZombieElfBeard,
                NPCID.ZombieElfGirl, NPCID.BloodEelHead, NPCID.GoblinShark, NPCID.EyeballFlyingFish, NPCID.ZombieMerman);

        /// <summary>
        /// If <see langword="true"/> for an NPC type, then that NPC is a post-Plantera Dungeon enemy. This causes two separate changes to the enemy.<br/>
        /// First, it grants them a 20% chance to drop Ectoplasm upon death.<br/>
        /// Second, it multiplies their max health by 2.5x and increases their damage by a flat 30 if Moon Lord has been defeated.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] IsBuffedDungeonEnemy = Factory.CreateBoolSet(NPCID.SkeletonSniper, NPCID.TacticalSkeleton, NPCID.SkeletonCommando, NPCID.Paladin, NPCID.GiantCursedSkull,
                NPCID.BoneLee, NPCID.DiabolistWhite, NPCID.DiabolistRed, NPCID.NecromancerArmored, NPCID.Necromancer, NPCID.RaggedCasterOpenCoat, NPCID.RaggedCaster,
                NPCID.HellArmoredBonesSword, NPCID.HellArmoredBonesMace, NPCID.HellArmoredBonesSpikeShield, NPCID.HellArmoredBones, NPCID.BlueArmoredBonesSword, NPCID.BlueArmoredBonesNoPants,
                NPCID.BlueArmoredBonesMace, NPCID.BlueArmoredBones, NPCID.RustyArmoredBonesSwordNoArmor, NPCID.RustyArmoredBonesSword, NPCID.RustyArmoredBonesFlail, NPCID.RustyArmoredBonesAxe);

        /// <summary>
        /// If <see langword="true"/> for an NPC type, then that NPC is spawned in the Pumpkin Moon.<br/>
        /// This will multiply their max health by 3.5x and increase their damage by a flat 30 if Devourer of Gods has been defeated.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] IsBuffedPumpkinMoonEnemy = Factory.CreateBoolSet(NPCID.Scarecrow1, NPCID.Scarecrow2, NPCID.Scarecrow3, NPCID.Scarecrow4, NPCID.Scarecrow5, NPCID.Scarecrow6,
                NPCID.Scarecrow7, NPCID.Scarecrow8, NPCID.Scarecrow9, NPCID.Scarecrow10, NPCID.HeadlessHorseman, NPCID.MourningWood, NPCID.Splinterling, NPCID.Pumpking,
                NPCID.PumpkingBlade, NPCID.Hellhound, NPCID.Poltergeist);

        /// <summary>
        /// If <see langword="true"/> for an NPC type, then that NPC is spawned in the Frost Moon.<br/>
        /// This will multiply their max health by 2.5x and increase their damage by a flat 30 if Devourer of Gods has been defeated.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] IsBuffedFrostMoonEnemy = Factory.CreateBoolSet(NPCID.ZombieElf, NPCID.ZombieElfBeard, NPCID.ZombieElfGirl, NPCID.PresentMimic, NPCID.GingerbreadMan,
                NPCID.Yeti, NPCID.Everscream, NPCID.IceQueen, NPCID.SantaNK1, NPCID.ElfCopter, NPCID.Nutcracker, NPCID.NutcrackerSpinning, NPCID.ElfArcher, NPCID.Krampus, NPCID.Flocko);

        /// <summary>
        /// If <see langword="true"/> for an NPC type, then that NPC is spawned in the Solar Eclipse.<br/>
        /// This will multiply their max health by 5x and increase their damage by a flat 30 if Devourer of Gods has been defeated.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] IsBuffedSolarEclipseEnemy = Factory.CreateBoolSet(NPCID.Eyezor, NPCID.Reaper, NPCID.Frankenstein, NPCID.SwampThing, NPCID.Vampire, NPCID.VampireBat,
                NPCID.Butcher, NPCID.CreatureFromTheDeep, NPCID.Fritz, NPCID.Nailhead, NPCID.Psycho, NPCID.DeadlySphere, NPCID.DrManFly, NPCID.ThePossessed, NPCID.Mothron,
                NPCID.MothronEgg, NPCID.MothronSpawn);

        /// <summary>
        /// If <see langword="true"/> for an NPC type, then the NPC will receive a 30 second global cooldown to slowing debuffs after being inflicted by one.<br/>
        /// Also used to prevent weapon pulling effects and Anarchy Blade's ability to instantly kill enemies that are below 50% health.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] ResistSlowingDebuffsAndOtherSpecialEffects = Factory.CreateBoolSet(NPCID.KingSlime, NPCType<KingSlimeJewelRuby>(),
                NPCID.EyeofCthulhu, NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail, NPCID.BrainofCthulhu, NPCID.Creeper,
                NPCID.QueenBee, NPCID.Deerclops, NPCID.SkeletronHead, NPCID.SkeletronHand, NPCID.WallofFlesh, NPCID.WallofFleshEye, NPCID.PirateShipCannon, NPCID.QueenSlimeBoss,
                NPCID.Probe, NPCID.Retinazer, NPCID.Spazmatism, NPCID.SkeletronPrime, NPCID.PrimeCannon, NPCID.PrimeSaw, NPCID.PrimeLaser, NPCID.PrimeVice, NPCID.Plantera,
                NPCID.PlanterasTentacle, NPCType<PlanterasFreeTentacle>(), NPCID.Everscream, NPCID.SantaNK1, NPCID.IceQueen, NPCID.MourningWood, NPCID.Pumpking, NPCID.Mothron,
                NPCID.Golem, NPCID.GolemHead, NPCID.GolemHeadFree, NPCID.GolemFistRight, NPCID.GolemFistLeft, NPCID.MartianSaucerCore, NPCID.MartianSaucerCannon, NPCID.MartianSaucerTurret,
                NPCID.DukeFishron, NPCID.Sharkron, NPCID.Sharkron2, NPCID.HallowBoss, NPCID.CultistBoss, NPCID.CultistDragonHead, NPCID.CultistDragonBody1, NPCID.CultistDragonBody2,
                NPCID.CultistDragonBody3, NPCID.CultistDragonBody4, NPCID.CultistDragonTail, NPCID.AncientCultistSquidhead, NPCID.MoonLordHead, NPCID.MoonLordHand, NPCID.MoonLordCore,
                NPCID.MoonLordFreeEye, NPCID.DD2WyvernT1, NPCID.DD2WyvernT2, NPCID.DD2WyvernT3, NPCID.DD2DarkMageT1, NPCID.DD2DarkMageT3, NPCID.DD2SkeletonT1, NPCID.DD2SkeletonT3,
                NPCID.DD2WitherBeastT2, NPCID.DD2WitherBeastT3, NPCID.DD2DrakinT2, NPCID.DD2DrakinT3, NPCID.DD2KoboldWalkerT2, NPCID.DD2KoboldWalkerT3, NPCID.DD2KoboldFlyerT2,
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
        public static bool[] DoCheckDeadRegardlessRealLife = Factory.CreateBoolSet(NPCType<DevourerofGodsBody>(), NPCType<DevourerofGodsTail>());

        /// <summary>
        /// If <see langword="true"/> for an NPC type, <see cref="CalamityUtils.IsAnEnemy(NPC, bool, bool, bool)"/> will not count this NPC as an enemy.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] DontCountAsEnemy = Factory.CreateBoolSet(NPCID.TargetDummy, NPCType<SuperDummyNPC>());

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
    }
}

using System.Collections.Generic;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using ReLogic.Reflection;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Systems.Collections
{
    [ReinitializeDuringResizeArrays]
    public static class CalamityNPCTypeSets
    {
        private static SetFactory Factory = NPCID.Sets.Factory;

        // Used for dropping Ancient Bone Dust, vanilla has a Skeleton NPCID set which has more unwanted enemies in it
        public static bool[] Skeleton = Factory.CreateNamedSet("Skeleton")
            .Description("Contains different IDs from vanilla's Skeleton set. Used for dropping Ancient Bone Dust.")
            .RegisterBoolSet(NPCID.Skeleton, NPCID.HeadacheSkeleton, NPCID.MisassembledSkeleton, NPCID.PantlessSkeleton, NPCID.BoneThrowingSkeleton, NPCID.BoneThrowingSkeleton2,
                NPCID.BoneThrowingSkeleton3, NPCID.BoneThrowingSkeleton4, NPCID.SkeletonTopHat, NPCID.SkeletonAstonaut, NPCID.SkeletonAlien, NPCID.ArmoredSkeleton, NPCID.SkeletonArcher,
                NPCID.GreekSkeleton, NPCID.SporeSkeleton);

        // Unused, though is more restrictive than the vanilla Zombie NPCID set
        public static bool[] Zombie = Factory.CreateNamedSet("Zombie")
            .Description("Contains different IDs from vanilla's Zombie set. Unused.")
            .RegisterBoolSet(NPCID.Zombie, NPCID.ArmedZombie, NPCID.BaldZombie, NPCID.PincushionZombie, NPCID.ArmedZombiePincussion, NPCID.SlimedZombie,
                NPCID.ArmedZombieSlimed, NPCID.SwampZombie, NPCID.ArmedZombieSwamp, NPCID.TwiggyZombie, NPCID.ArmedZombieTwiggy, NPCID.FemaleZombie, NPCID.ArmedZombieCenx,
                NPCID.ZombieRaincoat, NPCID.ZombieEskimo, NPCID.ArmedZombieEskimo, NPCID.MaggotZombie, NPCType<BucketZombie>());



        public static List<int> AquaticScourge = [ NPCType<AquaticScourgeHead>(), NPCType<AquaticScourgeBody>(), NPCType<AquaticScourgeBodyAlt>(), NPCType<AquaticScourgeTail>() ];

        public static List<int> Ares = [ NPCType<AresBody>(), NPCType<AresGaussNuke>(), NPCType<AresLaserCannon>(), NPCType<AresPlasmaFlamethrower>(), NPCType<AresTeslaCannon>() ];

        public static List<int> AstrumDeus = [ NPCType<AstrumDeusHead>(), NPCType<AstrumDeusBody>(), NPCType<AstrumDeusTail>() ];

        public static List<int> DesertScourge = [ NPCType<DesertScourgeHead>(), NPCType<DesertScourgeBody>(), NPCType<DesertScourgeTail>() ];

        public static List<int> Destroyer = [ NPCID.TheDestroyer, NPCID.TheDestroyerBody, NPCID.TheDestroyerTail ];

        public static List<int> DevourerOfGods = [ NPCType<DevourerofGodsHead>(), NPCType<DevourerofGodsBody>(), NPCType<DevourerofGodsTail>() ];

        public static List<int> EaterOfWorlds = [ NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail ];

        public static List<int> Perforators = [ NPCType<PerforatorHeadLarge>(), NPCType<PerforatorBodyLarge>(), NPCType<PerforatorTailLarge>(), NPCType<PerforatorHeadMedium>(),
                NPCType<PerforatorBodyMedium>(), NPCType<PerforatorTailMedium>(), NPCType<PerforatorHeadSmall>(), NPCType<PerforatorBodySmall>(), NPCType<PerforatorTailSmall>() ];

        public static List<int> Ravager = [ NPCType<RavagerBody>(), NPCType<RavagerClawLeft>(), NPCType<RavagerClawRight>(), NPCType<RavagerLegLeft>(), NPCType<RavagerLegRight>(), NPCType<RavagerHead>() ];

        public static List<int> SkeletronPrime = [ NPCID.SkeletronPrime, NPCID.PrimeCannon, NPCID.PrimeLaser, NPCID.PrimeSaw, NPCID.PrimeVice ];

        public static List<int> SlimeGod = [ NPCType<EbonianPaladin>(), NPCType<CrimulanPaladin>(), NPCType<SplitEbonianPaladin>(), NPCType<SplitCrimulanPaladin>(), NPCType<SlimeGodCore>() ];

        public static List<int> StormWeaver = [ NPCType<StormWeaverHead>(), NPCType<StormWeaverBody>(), NPCType<StormWeaverTail>() ];

        public static List<int> Thanatos = [ NPCType<ThanatosHead>(), NPCType<ThanatosBody1>(), NPCType<ThanatosBody2>(), NPCType<ThanatosTail>() ];
    }
}

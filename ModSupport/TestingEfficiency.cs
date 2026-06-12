using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.ModSupport
{
    //This uses its own class so the entire class can simply not load when TE is not enabled.
    public class TestingEfficiencySupport : ModSystem
    {
       
        internal static Mod testingEfficiency = null;
        public override bool IsLoadingEnabled(Mod mod)
        {
            testingEfficiency = null;
            return ModLoader.TryGetMod("TestingEfficiency", out testingEfficiency);
        }

        List<(string name, float tier, Func<bool> getter, Action<bool> setter, Func<Asset<Texture2D>> texture)> BossTogles = new()
        {
            ("Desert Scourge", 1.5f,
                () => DownedBossSystem.downedDesertScourge,
                x => DownedBossSystem.downedDesertScourge = x,
                () => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<DesertScourgeHead>()]]),

            ("Crabulon", 2.5f,
                () => DownedBossSystem.downedCrabulon,
                x => DownedBossSystem.downedCrabulon = x,
                () => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<Crabulon>()]]),

            ("Hive Mind / Perforators", 3.5f,
                () => DownedBossSystem.downedHiveMind || DownedBossSystem.downedPerforator,
                x => DownedBossSystem.downedHiveMind = DownedBossSystem.downedPerforator = x,
                () => WorldGen.crimson
                    ? TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<PerforatorHive>()]]
                    : TextureAssets.NpcHeadBoss[HiveMind.phase2IconIndex]),

            ("Slime God", 6.5f,
                () => DownedBossSystem.downedSlimeGod,
                x => DownedBossSystem.downedSlimeGod = x,
                () => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<SlimeGodCore>()]]),

            ("Cryogen", 8.5f,
                () => DownedBossSystem.downedCryogen,
                x => DownedBossSystem.downedCryogen = x,
                () => TextureAssets.NpcHeadBoss[Cryogen.cryoIconIndex]),

            ("Aquatic Scourge", 9.25f,
                () => DownedBossSystem.downedAquaticScourge,
                x => DownedBossSystem.downedAquaticScourge = x,
                () => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<AquaticScourgeHead>()]]),

            ("Brimstone Elemental", 10.25f,
                () => DownedBossSystem.downedBrimstoneElemental,
                x => DownedBossSystem.downedBrimstoneElemental = x,
                () => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<BrimstoneElemental>()]]),

            ("Calamitas Clone", 11.5f,
                () => DownedBossSystem.downedCalamitasClone,
                x => DownedBossSystem.downedCalamitasClone = x,
                () => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<CalamitasClone>()]]),

            ("Leviathan & Anahita", 12.25f,
                () => DownedBossSystem.downedLeviathan,
                x => DownedBossSystem.downedLeviathan = x,
                () => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<Leviathan>()]]),

            ("Astrum Aureus", 12.5f,
                () => DownedBossSystem.downedAstrumAureus,
                x => DownedBossSystem.downedAstrumAureus = x,
                () => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<AstrumAureus>()]]),

            ("Plaguebringer Goliath", 14.25f,
                () => DownedBossSystem.downedPlaguebringer,
                x => DownedBossSystem.downedPlaguebringer = x,
                () => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<PlaguebringerGoliath>()]]),

            ("Ravager", 15.25f,
                () => DownedBossSystem.downedRavager,
                x => DownedBossSystem.downedRavager = x,
                () => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<RavagerBody>()]]),

            ("Astrum Deus", 16.5f,
                () => DownedBossSystem.downedAstrumDeus,
                x => DownedBossSystem.downedAstrumDeus = x,
                () => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<AstrumDeusHead>()]]),

            ("Dragonfolly", 17.25f,
                () => DownedBossSystem.downedDragonfolly,
                x => DownedBossSystem.downedDragonfolly = x,
                () => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<Dragonfolly>()]]),

            ("Profaned Guardians", 17.5f,
                () => DownedBossSystem.downedGuardians,
                x => DownedBossSystem.downedGuardians = x,
                () => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<ProfanedGuardianHealer>()]]),

            ("Providence", 18f,
                () => DownedBossSystem.downedProvidence,
                x => DownedBossSystem.downedProvidence = x,
                () => TextureAssets.Item[ModContent.ItemType<ProfanedCore>()]),

            ("Ceaseless Void", 18.25f,
                () => DownedBossSystem.downedCeaselessVoid,
                x => DownedBossSystem.downedCeaselessVoid = x,
                () => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<CeaselessVoid>()]]),

            ("Storm Weaver", 18.5f,
                () => DownedBossSystem.downedStormWeaver,
                x => DownedBossSystem.downedStormWeaver = x,
                () => TextureAssets.NpcHeadBoss[StormWeaverHead.normalIconIndex]),

            ("Signus", 18.75f,
                () => DownedBossSystem.downedSignus,
                x => DownedBossSystem.downedSignus = x,
                () => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<Signus>()]]),

            ("Polterghast", 19f,
                () => DownedBossSystem.downedPolterghast,
                x => DownedBossSystem.downedPolterghast = x,
                () => TextureAssets.NpcHeadBoss[Polterghast.phase1IconIndex]),

            ("Old Duke", 19.5f,
                () => DownedBossSystem.downedBoomerDuke,
                x => DownedBossSystem.downedBoomerDuke = x,
                () => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<OldDuke>()]]),

            ("Devourer of Gods", 20f,
                () => DownedBossSystem.downedDoG,
                x => DownedBossSystem.downedDoG = x,
                () => TextureAssets.NpcHeadBoss[DevourerofGodsHead.phase1IconIndex]),

            ("Yharon", 21f,
                () => DownedBossSystem.downedYharon,
                x => DownedBossSystem.downedYharon = x,
                () => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<Yharon>()]]),

            ("Exo Mechs", 22f,
                () => DownedBossSystem.downedExoMechs,
                x => DownedBossSystem.downedExoMechs = x,
                () => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<AresBody>()]]),

            ("Supreme Calamitas", 23f,
                () => DownedBossSystem.downedCalamitas,
                x => DownedBossSystem.downedCalamitas = x,
                () => TextureAssets.NpcHeadBoss[SupremeCalamitas.hoodedHeadIconIndex]),

            ("Primordial Wyrm", 23.5f,
                () => DownedBossSystem.downedPrimordialWyrm,
                x => DownedBossSystem.downedPrimordialWyrm = x,
                () => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<PrimordialWyrmHead>()]])
        };

        public override void PostSetupContent()
        {
            foreach (var item in BossTogles)
            {
                testingEfficiency.Call("RegisterBoss", item.name, item.tier, item.getter, item.setter, item.texture);
            }
        }
    }

    [ReinitializeDuringResizeArrays]
    public class TestingEfficiencyIdSets
    {
        //These being set by Calamity is important so that boss reworks can update these on Cal's end in development
        //Otherwise, reworks to bosses would often require their own build, similar to issues CalTestHelpers had before.
        //Because these are ID sets, other mods that may be interested in similar info can also use them regardless of if TE is loaded

        internal static int[] NpcToCountAs = NPCID.Sets.Factory.CreateNamedSet("TestingEfficiency/NpcToCountAs")
            .RegisterIntSet(-1,
            ModContent.NPCType<DevourerofGodsBody>(), ModContent.NPCType<DevourerofGodsHead>(),
            ModContent.NPCType<DevourerofGodsTail>(), ModContent.NPCType<DevourerofGodsHead>(),
            ModContent.NPCType<DesertScourgeBody>(), ModContent.NPCType<DesertScourgeHead>(),
            ModContent.NPCType<DesertScourgeTail>(), ModContent.NPCType<DesertScourgeHead>(),
            ModContent.NPCType<AstrumDeusBody>(), ModContent.NPCType<AstrumDeusHead>(),
            ModContent.NPCType<AstrumDeusTail>(), ModContent.NPCType<AstrumDeusHead>(),
            ModContent.NPCType<AquaticScourgeBody>(), ModContent.NPCType<AquaticScourgeHead>(),
            ModContent.NPCType<AquaticScourgeBodyAlt>(), ModContent.NPCType<AquaticScourgeHead>(),
            ModContent.NPCType<AquaticScourgeTail>(), ModContent.NPCType<AquaticScourgeHead>(),
            ModContent.NPCType<ThanatosBody1>(), ModContent.NPCType<ThanatosHead>(),
            ModContent.NPCType<ThanatosBody2>(), ModContent.NPCType<ThanatosHead>(),
            ModContent.NPCType<ThanatosTail>(), ModContent.NPCType<ThanatosHead>() ,
            ModContent.NPCType<StormWeaverBody>(), ModContent.NPCType<StormWeaverHead>(),
            ModContent.NPCType<StormWeaverTail>(), ModContent.NPCType<StormWeaverHead>(),
            ModContent.NPCType<AresGaussNuke>(), ModContent.NPCType<AresBody>() ,
            ModContent.NPCType<AresLaserCannon>(), ModContent.NPCType<AresBody>() ,
            ModContent.NPCType<AresPlasmaFlamethrower>(), ModContent.NPCType<AresBody>() ,
            ModContent.NPCType<AresTeslaCannon>(), ModContent.NPCType<AresBody>() ,
            ModContent.NPCType<Apollo>(), ModContent.NPCType<Artemis>() ,
            ModContent.NPCType<PerforatorBodyLarge>(), ModContent.NPCType<PerforatorHeadLarge>() ,
            ModContent.NPCType<PerforatorBodyMedium>(), ModContent.NPCType<PerforatorHeadMedium>() ,
            ModContent.NPCType<PerforatorBodySmall>(), ModContent.NPCType<PerforatorHeadSmall>() ,
            ModContent.NPCType<PerforatorTailLarge>(), ModContent.NPCType<PerforatorHeadLarge>() ,
            ModContent.NPCType<PerforatorTailMedium>(), ModContent.NPCType<PerforatorHeadMedium>() ,
            ModContent.NPCType<PerforatorTailSmall>(), ModContent.NPCType<PerforatorHeadSmall>() ,
            ModContent.NPCType<RavagerClawLeft>(), ModContent.NPCType<RavagerBody>(),
            ModContent.NPCType<RavagerClawRight>(), ModContent.NPCType<RavagerBody>(),
            ModContent.NPCType<RavagerHead>(), ModContent.NPCType<RavagerBody>(),
            ModContent.NPCType<RavagerLegLeft>(), ModContent.NPCType<RavagerBody>(),
            ModContent.NPCType<RavagerLegRight>(), ModContent.NPCType<RavagerBody>(),
            ModContent.NPCType<SplitEbonianPaladin>(), ModContent.NPCType<EbonianPaladin>(),
            ModContent.NPCType<SplitCrimulanPaladin>(), ModContent.NPCType<CrimulanPaladin>()
            );


        public static bool[] ShouldMergeInstances = NPCID.Sets.Factory.CreateNamedSet("TestingEfficiency/ShouldMergeInstances")
            .RegisterBoolSet(
                ModContent.NPCType<AstrumDeusHead>(),
                ModContent.NPCType<Catastrophe>(),
                ModContent.NPCType<Cataclysm>(),
                ModContent.NPCType<SoulSeeker>(),
                ModContent.NPCType<SupremeCatastrophe>(),
                ModContent.NPCType<SoulSeekerSupreme>(),
                ModContent.NPCType<SupremeCataclysm>(),
                ModContent.NPCType<DarkEnergy>(),
                ModContent.NPCType<CryogenShield>(),
                ModContent.NPCType<AnahitasIceShield>(),
                ModContent.NPCType<PerforatorHeadSmall>(),
                ModContent.NPCType<PerforatorHeadMedium>(),
                ModContent.NPCType<PerforatorHeadLarge>(),
                ModContent.NPCType<PolterPhantom>(),
                ModContent.NPCType<RavagerHead2>(),
                ModContent.NPCType<SplitCrimulanPaladin>(),
                ModContent.NPCType<SplitEbonianPaladin>(),
                ModContent.NPCType<ProvSpawnHealer>(),
                ModContent.NPCType<ProvSpawnDefense>(),
                ModContent.NPCType<ProvSpawnOffense>()
            );

        public static bool[] ShouldBlacklist = NPCID.Sets.Factory.CreateNamedSet("TestingEfficiency/ShouldBlacklist")
            .RegisterBoolSet(ModContent.NPCType<DevourerofGodsBody>(),
                ModContent.NPCType<DevourerofGodsTail>(),
                ModContent.NPCType<DesertScourgeBody>(),
                ModContent.NPCType<DesertScourgeTail>(),
                ModContent.NPCType<AstrumDeusBody>(),
                ModContent.NPCType<AstrumDeusTail>(),
                ModContent.NPCType<AquaticScourgeBody>(),
                ModContent.NPCType<AquaticScourgeBodyAlt>(),
                ModContent.NPCType<AquaticScourgeTail>(),
                ModContent.NPCType<ThanatosBody1>(),
                ModContent.NPCType<ThanatosBody2>(),
                ModContent.NPCType<ThanatosTail>(),
                ModContent.NPCType<StormWeaverBody>(),
                ModContent.NPCType<StormWeaverTail>(),
                ModContent.NPCType<AresGaussNuke>(),
                ModContent.NPCType<AresLaserCannon>(),
                ModContent.NPCType<AresPlasmaFlamethrower>(),
                ModContent.NPCType<AresTeslaCannon>(),
                ModContent.NPCType<Apollo>(),
                 ModContent.NPCType<RavagerClawLeft>(),
                 ModContent.NPCType<RavagerClawRight>(),
                 ModContent.NPCType<RavagerHead>(),
                 ModContent.NPCType<RavagerLegLeft>(),
                 ModContent.NPCType<RavagerLegRight>(),
                ModContent.NPCType<PerforatorBodyLarge>(),
                ModContent.NPCType<PerforatorBodyMedium>(),
                 ModContent.NPCType<PerforatorBodySmall>(),
                 ModContent.NPCType<PerforatorTailLarge>(),
                 ModContent.NPCType<PerforatorTailMedium>(),
                 ModContent.NPCType<PerforatorTailMedium>(),
                 ModContent.NPCType<PerforatorTailSmall>()
            );

        public static bool[] ShouldTrackAsABoss = NPCID.Sets.Factory.CreateNamedSet("TestingEfficiency/ShouldTrackAsABoss")
            .Description("NPC types that should be whitelisted to count as a boss")
            .RegisterBoolSet(
                ModContent.NPCType<ProfanedGuardianCommander>(),
                ModContent.NPCType<ProfanedGuardianDefender>(),
                ModContent.NPCType<ProfanedGuardianHealer>()
            );
    }
}

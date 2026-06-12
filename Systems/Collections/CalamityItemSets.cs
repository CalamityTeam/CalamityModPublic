using System.Collections.Generic;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Fishing;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Items.Potions.Food;
using CalamityMod.Items.SummonItems.TownPets;
using CalamityMod.Items.Tools;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using ReLogic.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Systems.Collections
{
    [ReinitializeDuringResizeArrays]
    public static class CalamityItemSets
    {
        private static SetFactory Factory = ItemID.Sets.Factory;

        /// <summary>
        /// If <see langword="true"/> for an item type, prevents an item from removing Calamity's summon damage penalty mechanic despite having tool power.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] WeaponWithToolPowerAffectedBySummonPenalty = Factory.CreateNamedSet("WeaponWithToolPowerAffectedBySummonPenalty")
            .Description("Makes items with tool power still trigger the summon damage penalty.")
            .RegisterBoolSet(ItemID.ButchersChainsaw, ItemID.LucyTheAxe, ItemID.Rockfish, ItemType<AxeofPurity>(), ItemType<HydraulicVoltCrasher>(), ItemType<InfernaCutter>(),
                ItemType<PhotonRipper>(), ItemType<Respiteblock>());

        /// <summary>
        /// If <see langword="true"/> for an item type, manually disables Calamity's summon damage penalty mechanic while that item type is held.<br/>
        /// Unused by Calamity itself, and is only used for external mods to add to through mod calls.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] ItemWhichDisablesSummonerNerf = Factory.CreateNamedSet("ItemWhichDisablesSummonerNerf")
            .Description("Makes holding this item manually disable the summon damage penalty.")
            .RegisterBoolSet();

        /// <summary>
        /// If <see langword="true"/> for an item type, holding the item sets <see cref="Player.accFishingLine"/> to true, preventing fishing lines from breaking.<br/>
        /// Should only be set on fishing poles.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] FishingPoleThatNeverBreaks = Factory.CreateNamedSet("FishingPoleThatNeverBreaks")
            .Description("Makes holding this item set Player.accFishingLine, preventing fishing lines from breaking.")
            .RegisterBoolSet(ItemID.GoldenFishingRod, ItemType<EarlyBloomRod>(), ItemType<TheDevourerofCods>());

        /// <summary>
        /// If <see langword="true"/> for an item type, prevents this rogue weapon from triggering Venerated Locket's clone projectile effect when used.<br/>
        /// Primarily used for weapons which shoot short-distance projectiles.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] DisablesVeneratedLocketEffect = Factory.CreateNamedSet("DisablesVeneratedLocketEffect")
            .Description("Prevents this item from triggering Venerated Locket's clone projectiles.")
            .RegisterBoolSet(ItemType<SlickCane>(), ItemType<Mycoroot>(), ItemType<CosmicKunai>());

        /// <summary>
        /// If <see langword="true"/> for an item type, this item is considered to be a magic gun.<br/>
        /// Used for applying Calamity's reworked Meteor armor set bonus.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] MagicGun = Factory.CreateNamedSet("MagicGun")
            .Description("Labels this item as a magic gun, for Meteor armor's set bonus.")
            .RegisterBoolSet(ItemID.BeeGun, ItemID.BubbleGun, ItemID.ChargedBlasterCannon, ItemID.HeatRay, ItemID.LaserMachinegun, ItemID.LaserRifle, ItemID.LeafBlower, ItemID.RainbowGun,
                ItemID.SpaceGun, ItemID.WaspGun, ItemID.ZapinatorGray, ItemID.ZapinatorOrange, ItemType<AbyssShocker>(), ItemType<AcidGun>(), ItemType<AethersWhisper>(), ItemType<AetherfluxCannon>(),
                ItemType<ApoctosisArray>(), ItemType<Cryophobia>(), ItemType<Effervescence>(), ItemType<EidolicWail>(), ItemType<Genesis>(), ItemType<IonBlaster>(), ItemType<NanoPurge>(),
                ItemType<Omicron>(), ItemType<PlasmaCaster>(), ItemType<PlasmaRifle>(), ItemType<PulsePistol>(), ItemType<PurgeGuzzler>(), ItemType<RainbowPartyCannon>(), ItemType<SHPC>(),
                ItemType<TeslaCannon>(), ItemType<TheSwarmer>(), ItemType<Volterion>(), ItemType<Vulcan>(), ItemType<Wingman>());

        /// <summary>
        /// If <see langword="true"/> for an item type, this item is guaranteed to critically strike and converts critical strike chance boosts into extra damage.<br/>
        /// Used to replace the item's critical strike chance tooltip line with one about critical damage.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] ShowScalingCritDamageTooltip = Factory.CreateNamedSet("ShowScalingCritDamageTooltip")
            .Description("Replaces the item's critical strike chance tooltip line with a line about critical damage.")
            .RegisterBoolSet(ItemType<GildedProboscis>(), ItemType<HeliumFlash>(), ItemType<ThreadOfEradication>(), ItemType<TitanArm>(), ItemType<VenusianTrident>());

        /// <summary>
        /// If <see langword="true"/> for an item type, this item is considered to be a rogue bomb.<br/>
        /// Currently unused, and exists as an objective classification for the sake of the Wiki.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] RogueBomb = Factory.CreateNamedSet("RogueBomb")
            .Description("Labels this item as a rogue bomb, only exists for objective classification.")
            .RegisterBoolSet(ItemType<BallisticPoisonBomb>(), ItemType<BlastBarrel>(), ItemType<ContaminatedBile>(), ItemType<ConsecratedWater>(), ItemType<CraniumSmasher>(),
                ItemType<DesecratedWater>(), ItemType<DuststormInABottle>(), ItemType<Exorcism>(), ItemType<LemonNade>(), ItemType<LeonidProgenitor>(), ItemType<MeteorFist>(), ItemType<Penumbra>(), ItemType<Plaguenade>(), 
                ItemType<PlasmaGrenade>(), ItemType<PulseGrenade>(), ItemType<Pumpkaboom>(), ItemType<SeafoamBomb>(), ItemType<SealedSingularity>(), ItemType<SkyfinBombers>(), ItemType<SpentFuelContainer>(), 
                ItemType<StarofDestruction>(), ItemType<Supernova>(), ItemType<TotalityBreakers>(), ItemType<WavePounder>(), ItemType<Whitewater>());

        /// <summary>
        /// If <see langword="true"/> for an item type, this item is considered to be a rogue boomerang.<br/>
        /// Currently unused, and exists as an objective classification for the sake of the Wiki.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] RogueBoomerang = Factory.CreateNamedSet("RogueBoomerang")
            .Description("Labels this item as a rogue boomerang, only exists for objective classification.")
            .RegisterBoolSet(ItemType<AerialTracker>(), ItemType<Brimblade>(), ItemType<Celestus>(), ItemType<DefectiveSphere>(), ItemType<DimensionTearingDisk>(), ItemType<DynamicPursuer>(),
                ItemType<EnchantedAxe>(), ItemType<EpidemicShredder>(), ItemType<Equanimity>(), ItemType<FishboneBoomerang>(), ItemType<FrostcrushValari>(), ItemType<GhoulishGouger>(),
                ItemType<Icebreaker>(), ItemType<InfestedClawmerang>(), ItemType<KelvinCatalyst>(), ItemType<Kylie>(), ItemType<MangroveChakram>(), ItemType<MoltenAmputator>(), ItemType<NanoblackReaper>(), 
                ItemType<ReboundingRainbow>(), ItemType<SamsaraSlicer>(), ItemType<SubductionSlicer>(), ItemType<ToxicantTwister>(), ItemType<Valediction>());

        /// <summary>
        /// If <see langword="true"/> for an item type, this item is considered to be a rogue dagger.<br/>
        /// Currently unused, and exists as an objective classification for the sake of the Wiki.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] RogueDagger = Factory.CreateNamedSet("RogueDagger")
            .Description("Labels this item as a rogue dagger, only exists for objective classification.")
            .RegisterBoolSet(ItemType<AshenStalactite>(), ItemType<Cinquedea>(), ItemType<Crystalline>(), ItemType<FeatherKnife>(), ItemType<GelDart>(), ItemType<GildedDagger>(),
                ItemType<GleamingDagger>(), ItemType<InfernalKris>(), ItemType<Mycoroot>(), ItemType<ShinobiBlade>(), ItemType<SporeKnife>(), ItemType<WulfrumKnife>(), ItemType<CobaltKunai>(),
                ItemType<CorpusAvertor>(), ItemType<CursedDagger>(), ItemType<LeviathanTeeth>(), ItemType<Malachite>(), ItemType<MythrilKnife>(), ItemType<OrichalcumSpikedGemstone>(),
                ItemType<Prismalline>(), ItemType<RadiantStar>(), ItemType<StellarKnife>(), ItemType<StormfrontRazor>(), ItemType<TerrorTalons>(), ItemType<CosmicKunai>(), ItemType<JawsOfOblivion>(),
                ItemType<LunarKunai>(), ItemType<Sacrifice>(), ItemType<Seraphim>(), ItemType<ShatteredDawn>(), ItemType<TarragonThrowingDart>(), ItemType<TimeBolt>(), ItemType<TwistingThunder>(),
                ItemType<UtensilPoker>());

        /// <summary>
        /// If <see langword="true"/> for an item type, this item is considered to be a rogue javelin.<br/>
        /// Currently unused, and exists as an objective classification for the sake of the Wiki.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] RogueJavelin = Factory.CreateNamedSet("RogueJavelin")
            .Description("Labels this item as a rogue javelin, only exists for objective classification.")
            .RegisterBoolSet(ItemType<AntlionSkewer>(), ItemType<CrystalPiercer>(), ItemType<EclipsesFall>(), ItemType<IchorSpear>(), ItemType<Vega>(), ItemType<PalladiumJavelin>(),
                ItemType<PhantasmalRuin>(), ItemType<ProfanedPartisan>(), ItemType<RealityRupture>(), ItemType<ScarletDevil>(), ItemType<ScourgeoftheDesert>(), ItemType<ScourgeoftheSeas>(),
                ItemType<ShardofAntumbra>(), ItemType<SpearofDestiny>(), ItemType<SpearofPaleolith>(), ItemType<Turbulance>(), ItemType<TheAtomSplitter>(), ItemType<WaveSkipper>(), ItemType<Wrathwing>());

        /// <summary>
        /// If <see langword="true"/> for an item type, this item is considered to be a rogue spiky ball.<br/>
        /// Currently unused, and exists as an objective classification for the sake of the Wiki.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] RogueSpikyBall = Factory.CreateNamedSet("RogueSpikyBall")
            .Description("Labels this item as a rogue spiky ball, only exists for objective classification.")
            .RegisterBoolSet(ItemType<BurningStrife>(), ItemType<GodsParanoia>(), ItemType<MetalMonstrosity>(), ItemType<NastyCholla>(), ItemType<SystemBane>(), ItemType<WebBall>());

        /// <summary>
        /// Defines a list of buff IDs to force display as an expandable tooltip with debuff info for enemy infliction on an item.<br/>
        /// Defaults to an empty list.
        /// </summary>
        public static List<int>[] ExtraDebuffTooltip_Enemy = Factory.CreateNamedSet("EnemyDebuffTooltip")
            .Description("Defines buff IDs to force display as expandable tooltip with debuff info for enemy infliction.")
            .RegisterCustomSet<List<int>>(new());

        /// <summary>
        /// Defines a list of buff IDs to force display as an expandable tooltip with debuff info for player infliction on an item.<br/>
        /// Defaults to an empty list.
        /// </summary>
        public static List<int>[] ExtraDebuffTooltip_Player = Factory.CreateNamedSet("PlayerDebuffTooltip")
            .Description("Defines buff IDs to force display as expandable tooltip with debuff info for player infliction.")
            .RegisterCustomSet<List<int>>(new());

        /// <summary>
        /// Does not support vanilla items. If <see langword="true"/> for an item type, this item has special "sales pitch" flavor text from the Shady Salesman.<br/>
        /// This causes the flavor text to draw at the top of the tooltip, with the actual tooltip being drawn below and with a smaller size.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] HasSalesmanText = Factory.CreateNamedSet("HasSalesmanText")
            .Description("Labels this item as having Shady Salesman flavor text, making it draw at the top of the tooltip and the actual tooltip being drawn small below it.")
            .RegisterBoolSet(ItemType<FishStocks>(), ItemType<TrustyOldRod>(), ItemType<RageBait>(), ItemType<GluttonyBlender>(), ItemType<TheMonument>(), ItemType<GreedPot>(), ItemType<BaconOil>(), ItemType<TheSandwich>(), ItemType<TheConcoction>(), ItemType<TheElixir>(), ItemType<TheGift>(), ItemType<OmniGun>(),
            ItemType<CombatVoucher>(), ItemType<AggressiveVoucher>(), ItemType<OddVoucher>(), ItemType<UnbreakableVoucher>(), ItemType<HurriedVoucher>(), ItemType<TheHousingContract>(), ItemType<CorruptionEffigy>(), ItemType<CrimsonEffigy>(), ItemType<TrinketofChi>(), ItemType<FrozenCube>(), ItemType<LuxorsGift>(),
            ItemType<FungalSymbiote>(), ItemType<GladiatorsLocket>(), ItemType<UnstableGraniteCore>(), ItemType<HeartofDarkness>(), ItemType<StressPills>());
    }
}

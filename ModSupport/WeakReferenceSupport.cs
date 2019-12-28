using CalamityMod;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Items.Potions;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.DifficultyItems;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.FurnitureCosmilite;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Placeables.Furniture.CraftingStations;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Tools;
using CalamityMod.Items.Tools.ClimateChange;
using CalamityMod.Items.TreasureBags;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.Yharon;
using CalamityMod.NPCs.Calamitas;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.NPCs.GreatSandShark;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.World;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
namespace CalamityMod
{
    internal class WeakReferenceSupport
    {
        public static void Setup()
        {
            BossChecklistSupport();
            CensusSupport();
        }

        private static void BossChecklistSupport()
        {
            Mod bossChecklist = ModLoader.GetMod("BossChecklist");
            Mod calamity = ModContent.GetInstance<CalamityMod>();
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");

            if (bossChecklist != null)
            {
                // 14 is moonlord, 12 is duke fishron

				//Desert Scourge
				bossChecklist.Call(
				"AddBoss", 
				1.5f, 
				new List<int>() {ModContent.NPCType<DesertScourgeHead>(), ModContent.NPCType<DesertScourgeBody>(), ModContent.NPCType<DesertScourgeTail>()},
				calamity, 
				"Desert Scourge", 
				(Func<bool>)(() => CalamityWorld.downedDesertScourge), 
				ModContent.ItemType<DriedSeafood>(), 
				new List<int>() {ModContent.ItemType<DesertScourgeTrophy>(), ModContent.ItemType<DesertScourgeMask>(), ModContent.ItemType<KnowledgeDesertScourge>()}, 
				new List<int>() {ModContent.ItemType<DesertScourgeBag>(), ItemID.SandBlock, ModContent.ItemType<VictoryShard>(), ItemID.Coral, ItemID.Seashell, ItemID.Starfish, ModContent.ItemType<AquaticDischarge>(), ModContent.ItemType<Barinade>(), ModContent.ItemType<StormSpray>(), ModContent.ItemType<SeaboundStaff>(), ModContent.ItemType<ScourgeoftheDesert>(), ModContent.ItemType<DuneHopper>(), ModContent.ItemType<AeroStone>(), ModContent.ItemType<SandCloak>(), ModContent.ItemType<DeepDiver>(), ModContent.ItemType<OceanCrest>(), ItemID.AnglerTackleBag, ItemID.HighTestFishingLine, ItemID.TackleBox, ItemID.AnglerEarring, ItemID.FishermansGuide, ItemID.WeatherRadio, ItemID.Sextant, ItemID.AnglerHat, ItemID.AnglerVest, ItemID.AnglerPants, ItemID.FishingPotion, ItemID.SonarPotion, ItemID.CratePotion, ItemID.GoldenBugNet, ItemID.LesserHealingPotion}, 
				"Use a [i:" + ModContent.ItemType<DriedSeafood>() + "] in the Desert Biome", 
				"[c/EEE8AA:The scourge of the desert delved back into the sand.]", 
				"CalamityMod/NPCs/DesertScourge/DesertScourge_BossChecklist");

				//Giant Clam
				bossChecklist.Call(
				"AddMiniBoss", 
				1.6f, 
				ModContent.NPCType<GiantClam>(), 
				calamity, 
				"Giant Clam", 
				(Func<bool>)(() => CalamityWorld.downedCLAM), 
				ItemID.None, 
				ItemID.None, 
				new List<int>() { ModContent.ItemType<Navystone>(), ModContent.ItemType<MolluskHusk>(), ModContent.ItemType<ClamCrusher>(), ModContent.ItemType<ClamorRifle>(), ModContent.ItemType<Poseidon>(), ModContent.ItemType<ShellfishStaff>(), ModContent.ItemType<GiantPearl>(), ModContent.ItemType<AmidiasPendant>()}, 
				"Can spawn naturally in the Sunken Sea", 
				"[c/7FFFD4:The Giant Clam returns into hiding in its grotto.]");

				//Crabulon
				bossChecklist.Call(
				"AddBoss", 
				2.5f, 
				ModContent.NPCType<CrabulonIdle>(), 
				calamity, 
				"Crabulon", 
				(Func<bool>)(() => CalamityWorld.downedCrabulon), 
				ModContent.ItemType<DecapoditaSprout>(), 
				new List<int>() { ModContent.ItemType<CrabulonTrophy>(), ModContent.ItemType<CrabulonMask>(), ModContent.ItemType<KnowledgeCrabulon>()}, 
				new List<int>() { ModContent.ItemType<CrabulonBag>(), ItemID.GlowingMushroom, ItemID.MushroomGrassSeeds, ModContent.ItemType<MycelialClaws>(), ModContent.ItemType<Fungicide>(), ModContent.ItemType<HyphaeRod>(), ModContent.ItemType<Mycoroot>(), ModContent.ItemType<Shroomerang>(), ModContent.ItemType<FungalClump>(), ModContent.ItemType<MushroomPlasmaRoot>(), ItemID.LesserHealingPotion}, 
				"Use a [i:" + ModContent.ItemType<DecapoditaSprout>() + "] in the Mushroom Biome", 
				"[c/6495ED:The mycelium crab has lost interest.]");

				//Hive Mind
				bossChecklist.Call(
				"AddBoss", 
				3.5f, 
				new List<int>() {ModContent.NPCType<HiveMind>(), ModContent.NPCType<HiveMindP2>()}, 
				calamity, 
				"Hive Mind", 
				(Func<bool>)(() => CalamityWorld.downedHiveMind), 
				ModContent.ItemType<Teratoma>(), 
				new List<int>() {ModContent.ItemType<HiveMindTrophy>(), ModContent.ItemType<HiveMindMask>(), ModContent.ItemType<KnowledgeHiveMind>()}, 
				new List<int>() {ModContent.ItemType<HiveMindBag>(), ModContent.ItemType<TrueShadowScale>(), ItemID.DemoniteBar, ItemID.RottenChunk, ItemID.CursedFlame, ModContent.ItemType<PerfectDark>(), ModContent.ItemType<LeechingDagger>(), ModContent.ItemType<Shadethrower>(), ModContent.ItemType<ShadowdropStaff>(), ModContent.ItemType<ShaderainStaff>(), ModContent.ItemType<DankStaff>(), ModContent.ItemType<RotBall>(), ModContent.ItemType<FilthyGlove>(), ModContent.ItemType<RottenBrain>(), ItemID.LesserHealingPotion}, 
				"By killing a Cyst in the Corruption or use a [i:" + ModContent.ItemType<Teratoma>() + "] in the Corruption", 
				"[c/9400D3:The corrupted colony began searching for a new breeding ground.]");

				//Perforators
				bossChecklist.Call(
				"AddBoss", 
				3.5f, 
				new List<int>() {ModContent.NPCType<PerforatorHive>()}, 
				calamity, 
				"The Perforators", 
				(Func<bool>)(() => CalamityWorld.downedPerforator), 
				ModContent.ItemType<BloodyWormFood>(), 
				new List<int>() {ModContent.ItemType<PerforatorTrophy>(), ModContent.ItemType<PerforatorMask>(), ModContent.ItemType<KnowledgePerforators>(), ModContent.ItemType<BloodyVein>()}, 
				new List<int>() {ModContent.ItemType<PerforatorBag>(), ModContent.ItemType<BloodSample>(), ItemID.CrimtaneBar, ItemID.Vertebrae, ItemID.Ichor, ModContent.ItemType<VeinBurster>(), ModContent.ItemType<BloodyRupture>(), ModContent.ItemType<SausageMaker>(), ModContent.ItemType<Aorta>(), ModContent.ItemType<Eviscerator>(), ModContent.ItemType<BloodBath>(), ModContent.ItemType<BloodClotStaff>(), ModContent.ItemType<ToothBall>(), ModContent.ItemType<BloodstainedGlove>(), ModContent.ItemType<BloodyWormTooth>(), ItemID.LesserHealingPotion}, 
				"By killing a Cyst in the Crimson or use a [i:" + ModContent.ItemType<BloodyWormFood>() + "] in the Crimson", 
				"[c/DC143C:The parasitic hive began searching for a new host.]");

				//Slime God
				bossChecklist.Call(
				"AddBoss", 
				5.5f, 
				new List<int>() {ModContent.NPCType<SlimeGodCore>(), ModContent.NPCType<SlimeGod>(), ModContent.NPCType<SlimeGodRun>()}, 
				calamity, 
				"Slime God", 
				(Func<bool>)(() => CalamityWorld.downedSlimeGod), 
				ModContent.ItemType<OverloadedSludge>(), 
				new List<int>() {ModContent.ItemType<SlimeGodTrophy>(), ModContent.ItemType<SlimeGodMask>(), ModContent.ItemType<SlimeGodMask2>(), ModContent.ItemType<KnowledgeSlimeGod>(), ModContent.ItemType<StaticRefiner>()}, 
				new List<int>() {ModContent.ItemType<SlimeGodBag>(), ItemID.Gel, ModContent.ItemType<PurifiedGel>(), ModContent.ItemType<OverloadedBlaster>(), ModContent.ItemType<AbyssalTome>(), ModContent.ItemType<EldritchTome>(), ModContent.ItemType<CorroslimeStaff>(), ModContent.ItemType<CrimslimeStaff>(), ModContent.ItemType<GelDart>(), ModContent.ItemType<ManaOverloader>(), ModContent.ItemType<ElectrolyteGelPack>(), ModContent.ItemType<PurifiedJam>(), ItemID.HealingPotion}, 
				"Use an [i:" + ModContent.ItemType<OverloadedSludge>() + "]", 
				"[c/BA5533:The gelatinous monstrosity achieved vengeance for its breathren.]");

				//Cryogen
				bossChecklist.Call(
				"AddBoss", 
				6.5f, 
				ModContent.NPCType<Cryogen>(), 
				calamity, 
				"Cryogen", 
				(Func<bool>)(() => CalamityWorld.downedCryogen), 
				ModContent.ItemType<CryoKey>(), 
				new List<int>() {ModContent.ItemType<CryogenTrophy>(), ModContent.ItemType<CryogenMask>(), ModContent.ItemType<KnowledgeCryogen>()}, 
				new List<int>() {ModContent.ItemType<CryogenBag>(), ItemID.SoulofMight, ModContent.ItemType<CryoBar>(), ModContent.ItemType<EssenceofEleum>(), ItemID.FrostCore, ModContent.ItemType<Avalanche>(), ModContent.ItemType<GlacialCrusher>(), ModContent.ItemType<EffluviumBow>(), ModContent.ItemType<BittercoldStaff>(), ModContent.ItemType<SnowstormStaff>(), ModContent.ItemType<Icebreaker>(), ModContent.ItemType<IceStar>(), ModContent.ItemType<CryoStone>(), ModContent.ItemType<Regenator>(), ModContent.ItemType<SoulofCryogen>(), ModContent.ItemType<FrostFlare>(), ItemID.FrozenKey, ItemID.GreaterHealingPotion}, 
				"Use a [i:" + ModContent.ItemType<CryoKey>() + "] in the Snow Biome", 
				"[c/00FFFF:Cryogen drifts away, carried on a freezing wind.]", 
				"CalamityMod/NPCs/Cryogen/Cryogen_BossChecklist");

				//Brimstone Elemental
				bossChecklist.Call(
				"AddBoss", 
				7.5f, 
				ModContent.NPCType<BrimstoneElemental>(), 
				calamity, 
				"Brimstone Elemental", 
				(Func<bool>)(() => CalamityWorld.downedBrimstoneElemental), 
				ModContent.ItemType<CharredIdol>(), 
				new List<int>() {ModContent.ItemType<BrimstoneElementalTrophy>(), ModContent.ItemType<KnowledgeBrimstoneCrag>(), ModContent.ItemType<KnowledgeBrimstoneElemental>(), ModContent.ItemType<CharredRelic>()}, 
				new List<int>() {ModContent.ItemType<BrimstoneWaifuBag>(), ItemID.SoulofFright, ModContent.ItemType<EssenceofChaos>(), ModContent.ItemType<Bloodstone>(), ModContent.ItemType<Brimlance>(), ModContent.ItemType<SeethingDischarge>(), ModContent.ItemType<Abaddon>(), ModContent.ItemType<RoseStone>(), ModContent.ItemType<Gehenna>(), ModContent.ItemType<Brimrose>(), ItemID.GreaterHealingPotion}, 
				"Use a [i:" + ModContent.ItemType<CharredIdol>() + "] in the Brimstone Crag", 
				"[c/DC143C:Brimstone Elemental withdraws to the ruins of her shrine.]");

				//Aquatic Scourge
				bossChecklist.Call(
				"AddBoss", 
				8.5f, 
				new List<int>() {ModContent.NPCType<AquaticScourgeHead>(), ModContent.NPCType<AquaticScourgeBody>(), ModContent.NPCType<AquaticScourgeBodyAlt>(), ModContent.NPCType<AquaticScourgeTail>()}, 
				calamity, 
				"Aquatic Scourge", 
				(Func<bool>)(() => CalamityWorld.downedAquaticScourge), 
				ModContent.ItemType<Seafood>(), 
				new List<int>() { ModContent.ItemType<AquaticScourgeTrophy>(), ModContent.ItemType<KnowledgeAquaticScourge>(), ModContent.ItemType<KnowledgeSulphurSea>()}, 
				new List<int>() { ModContent.ItemType<AquaticScourgeBag>(), ModContent.ItemType<SulphurousSand>(), ItemID.SoulofSight, ModContent.ItemType<VictoryShard>(), ItemID.Coral, ItemID.Seashell, ItemID.Starfish, ModContent.ItemType<SubmarineShocker>(), ModContent.ItemType<Barinautical>(), ModContent.ItemType<Downpour>(), ModContent.ItemType<DeepseaStaff>(), ModContent.ItemType<ScourgeoftheSeas>(), ModContent.ItemType<SeasSearing>(), ModContent.ItemType<AeroStone>(), ModContent.ItemType<AquaticEmblem>(), ItemID.AnglerTackleBag, ItemID.HighTestFishingLine, ItemID.TackleBox, ItemID.AnglerEarring, ItemID.FishermansGuide, ItemID.WeatherRadio, ItemID.Sextant, ItemID.AnglerHat, ItemID.AnglerVest, ItemID.AnglerPants, ItemID.FishingPotion, ItemID.SonarPotion, ItemID.CratePotion, ItemID.GoldenBugNet, ItemID.GreaterHealingPotion}, 
				"Use a [i:" + ModContent.ItemType<Seafood>() + "] in the Sulphuric Sea or wait for it to spawn in the Sulphuric Sea", 
				"[c/F0E68C:The Aquatic Scourge swam back into the open ocean.]", 
				"CalamityMod/NPCs/AquaticScourge/AquaticScourge_BossChecklist");

				//Calamitas
				bossChecklist.Call(
				"AddBoss", 
				9.7f, 
				new List<int>() {ModContent.NPCType<Calamitas>()}, 
				calamity, 
				"Calamitas", 
				(Func<bool>)(() => CalamityWorld.downedCalamitas), 
				ModContent.ItemType<BlightedEyeball>(), 
				new List<int>() {ModContent.ItemType<CalamitasTrophy>(), ModContent.ItemType<CataclysmTrophy>(), ModContent.ItemType<CatastropheTrophy>(), ModContent.ItemType<KnowledgeCalamitasClone>()}, 
				new List<int>() {ModContent.ItemType<CalamitasBag>(), ModContent.ItemType<EssenceofChaos>(), ModContent.ItemType<CalamityDust>(), ModContent.ItemType<BlightedLens>(), ModContent.ItemType<Bloodstone>(), ModContent.ItemType<CalamitasInferno>(), ModContent.ItemType<TheEyeofCalamitas>(), ModContent.ItemType<BlightedEyeStaff>(), ModContent.ItemType<Animosity>(), ModContent.ItemType<BrimstoneFlamesprayer>(), ModContent.ItemType<BrimstoneFlameblaster>(), ModContent.ItemType<CrushsawCrasher>(), ModContent.ItemType<ChaosStone>(), ModContent.ItemType<CalamityRing>(), ItemID.BrokenHeroSword, ItemID.GreaterHealingPotion}, 
				"Use an [i:" + ModContent.ItemType<BlightedEyeball>() + "] at Night", 
				"[c/FFA500:If you wanted a fight, you should've came more prepared.]");

				//Great Sand Shark
				bossChecklist.Call(
				"AddMiniBoss", 
				10.09f, 
				ModContent.NPCType<GreatSandShark>(), 
				calamity, 
				"Great Sand Shark", 
				(Func<bool>)(() => CalamityWorld.downedGSS), 
				ModContent.ItemType<SandstormsCore>(), 
				new List<int>() {ItemID.MusicBoxSandstorm}, 
				ModContent.ItemType<GrandScale>(), 
				"Kill 10 sand sharks after defeating Plantera or use a [i:" + ModContent.ItemType<SandstormsCore>() + "] in the Desert Biome", 
				"[c/DAA520:The apex predator of the sands disappears into the dunes...]");

				//Leviathan and Siren
				bossChecklist.Call(
				"AddBoss", 
				10.5f, 
				new List<int>() {ModContent.NPCType<Leviathan>(), ModContent.NPCType<Siren>()}, 
				calamity, 
				"Leviathan", 
				(Func<bool>)(() => CalamityWorld.downedLeviathan), 
				ItemID.None, 
				new List<int>() {ModContent.ItemType<LeviathanTrophy>(), ModContent.ItemType<LeviathanMask>(), ModContent.ItemType<KnowledgeOcean>(), ModContent.ItemType<KnowledgeLeviathanandSiren>()}, 
				new List<int>() {ModContent.ItemType<LeviathanBag>(), ModContent.ItemType<Greentide>(), ModContent.ItemType<Leviatitan>(), ModContent.ItemType<SirensSong>(), ModContent.ItemType<Atlantis>(), ModContent.ItemType<BrackishFlask>(), ModContent.ItemType<LeviathanTeeth>(), ModContent.ItemType<LureofEnthrallment>(), ModContent.ItemType<LeviathanAmbergris>(), ModContent.ItemType<TheCommunity>(), ItemID.HotlineFishingHook, ItemID.BottomlessBucket, ItemID.SuperAbsorbantSponge, ItemID.FishingPotion, ItemID.SonarPotion, ItemID.CratePotion, ItemID.GreaterHealingPotion}, 
				"By killing an unknown entity in the Ocean Biome", 
				"[c/7FFFD4:The aquatic entities sink back beneath the ocean depths.]", 
				"CalamityMod/NPCs/Leviathan/SirenandLevi_BossChecklist");

				//Astrum Aureus
				bossChecklist.Call(
				"AddBoss", 
				10.6f, 
				ModContent.NPCType<AstrumAureus>(), 
				calamity, 
				"Astrum Aureus", 
				(Func<bool>)(() => CalamityWorld.downedAstrageldon), 
				ModContent.ItemType<AstralChunk>(), 
				new List<int>() { ModContent.ItemType<AstrageldonTrophy>(), ModContent.ItemType<AureusMask>(), ModContent.ItemType<KnowledgeAstrumAureus>()}, 
				new List<int>() { ModContent.ItemType<AstrageldonBag>(), ModContent.ItemType<Stardust>(), ItemID.FallenStar, ModContent.ItemType<Nebulash>(), ModContent.ItemType<AstralJelly>(), ItemID.HallowedKey, ItemID.FragmentSolar, ItemID.FragmentVortex, ItemID.FragmentNebula, ItemID.FragmentStardust, ModContent.ItemType<StarlightFuelCell>(), ItemID.GreaterHealingPotion}, 
				"Use an [i:" + ModContent.ItemType<AstralChunk>() + "] at Night in the Astral Biome", 
				"[c/FFD700:Astrum Aureus’ program has been executed. Initiate recall.]", 
				"CalamityMod/NPCs/AstrumAureus/AstrumAureus_BossChecklist");

				//Plaguebringer Goliath
				bossChecklist.Call(
				"AddBoss", 
				11.5f, 
				ModContent.NPCType<PlaguebringerGoliath>(), 
				calamity, 
				"Plaguebringer Goliath", 
				(Func<bool>)(() => CalamityWorld.downedPlaguebringer), 
				ModContent.ItemType<Abomination>(), 
				new List<int>() {ModContent.ItemType<PlaguebringerGoliathTrophy>(), ModContent.ItemType<PlaguebringerGoliathMask>(), ModContent.ItemType<KnowledgePlaguebringerGoliath>()}, 
				new List<int>() {ModContent.ItemType<PlaguebringerGoliathBag>(), ModContent.ItemType<PlagueCellCluster>(), ModContent.ItemType<VirulentKatana>(), ModContent.ItemType<DiseasedPike>(), ModContent.ItemType<ThePlaguebringer>(), ModContent.ItemType<Malevolence>(), ModContent.ItemType<PestilentDefiler>(), ModContent.ItemType<TheHive>(), ModContent.ItemType<MepheticSprayer>(), ModContent.ItemType<PlagueStaff>(), ModContent.ItemType<TheSyringe>(), ModContent.ItemType<Malachite>(), ModContent.ItemType<BloomStone>(), ModContent.ItemType<ToxicHeart>(), ItemID.GreaterHealingPotion}, 
				"Use an [i:" + ModContent.ItemType<Abomination>() + "] in the Jungle Biome", 
				"[c/00FF00:HOSTILE SPECIMENS TERMINATED. INITIATE RECALL TO HOME BASE.]");

				//Ravager
				bossChecklist.Call(
				"AddBoss", 
				12.5f, 
				new List<int>() {ModContent.NPCType<RavagerBody>(), ModContent.NPCType<RavagerClawLeft>(), ModContent.NPCType<RavagerClawRight>(), ModContent.NPCType<RavagerHead>(), ModContent.NPCType<RavagerLegLeft>(), ModContent.NPCType<RavagerLegRight>()}, 
				calamity, 
				"Ravager", 
				(Func<bool>)(() => CalamityWorld.downedScavenger), 
				ModContent.ItemType<AncientMedallion>(), 
				new List<int>() {ModContent.ItemType<RavagerTrophy>(), ModContent.ItemType<KnowledgeRavager>()}, 
				new List<int>() {ModContent.ItemType<RavagerBag>(), ModContent.ItemType<Bloodstone>(), ModContent.ItemType<VerstaltiteBar>(), ModContent.ItemType<DraedonBar>(), ModContent.ItemType<CruptixBar>(), ModContent.ItemType<CoreofCinder>(), ModContent.ItemType<CoreofEleum>(), ModContent.ItemType<CoreofChaos>(), ModContent.ItemType<BarofLife>(), ModContent.ItemType<CoreofCalamity>(), ModContent.ItemType<UltimusCleaver>(), ModContent.ItemType<RealmRavager>(), ModContent.ItemType<Hematemesis>(), ModContent.ItemType<SpikecragStaff>(), ModContent.ItemType<CraniumSmasher>(), ModContent.ItemType<BloodPact>(), ModContent.ItemType<FleshTotem>(), ModContent.ItemType<BloodflareCore>(), ModContent.ItemType<InfernalBlood>(), ItemID.GreaterHealingPotion}, 
				"Use an [i:" + ModContent.ItemType<AncientMedallion>() + "]", 
				"[c/B22222:The automaton of misshapen victims went looking for the true perpetrator.]", 
				"CalamityMod/NPCs/Ravager/Ravager_BossChecklist");

				//Astrum Deus
				bossChecklist.Call(
				"AddBoss", 
				13.5f, 
				new List<int>() { ModContent.NPCType<AstrumDeusHeadSpectral>(), ModContent.NPCType<AstrumDeusBodySpectral>(), ModContent.NPCType<AstrumDeusTailSpectral>()}, 
				calamity, 
				"Astrum Deus", 
				(Func<bool>)(() => CalamityWorld.downedStarGod), 
				ModContent.ItemType<Starcore>(), 
				new List<int>() { ModContent.ItemType<AstrumDeusTrophy>(), ModContent.ItemType<AstrumDeusMask>(), ModContent.ItemType<KnowledgeAstrumDeus>(), ModContent.ItemType<KnowledgeAstralInfection>()}, 
				new List<int>() { ModContent.ItemType<AstrumDeusBag>(), ModContent.ItemType<Stardust>(), ModContent.ItemType<TheMicrowave>(), ModContent.ItemType<StarSputter>(), ModContent.ItemType<Starfall>(), ModContent.ItemType<Quasar>(), ModContent.ItemType<AstralBulwark>(), ModContent.ItemType<HideofAstrumDeus>(), ItemID.FragmentSolar, ItemID.FragmentVortex, ItemID.FragmentNebula, ItemID.FragmentStardust, ItemID.GreaterHealingPotion}, 
				"Defeat 3 empowered astral titans or use a [i:" + ModContent.ItemType<Starcore>() + "] at Night", 
				"[c/FFD700:The infected deity retreats to the heavens.]", 
				"CalamityMod/NPCs/AstrumDeus/AstrumDeus_BossChecklist");

				//Profaned Guardians
				bossChecklist.Call(
				"AddBoss", 
				14.5f, 
				new List<int>() { ModContent.NPCType<ProfanedGuardianBoss>()}, 
				calamity, "Profaned Guardians", 
				(Func<bool>)(() => CalamityWorld.downedGuardians), 
				ModContent.ItemType<ProfanedShard>(), 
				ModContent.ItemType<KnowledgeProfanedGuardians>(), 
				new List<int>() { ModContent.ItemType<ProfanedCore>(), ItemID.GreaterHealingPotion}, 
				"Use a [i:" + ModContent.ItemType<ProfanedShard>() + "] in the Hallow or Underworld Biomes", 
				"[c/FFA500:The guardians must protect their goddess at all costs.]", 
				"CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardians_BossChecklist");

				//Bumblebirb
				bossChecklist.Call(
				"AddBoss", 
				14.6f, 
				ModContent.NPCType<Bumblefuck>(), 
				calamity, 
				"Bumblebirb", 
				(Func<bool>)(() => CalamityWorld.downedBumble), 
				ModContent.ItemType<BirbPheromones>(), 
				new List<int>() {ModContent.ItemType<BumblebirbTrophy>(), ModContent.ItemType<KnowledgeBumblebirb>()}, 
				new List<int>() {ModContent.ItemType<BumblebirbBag>(), ModContent.ItemType<EffulgentFeather>(), ModContent.ItemType<GildedProboscis>(), ModContent.ItemType<GoldenEagle>(), ModContent.ItemType<RougeSlash>(), ModContent.ItemType<Swordsplosion>(), ModContent.ItemType<DynamoStemCells>(), ModContent.ItemType<RedLightningContainer>(), ItemID.SuperHealingPotion}, 
				"Use [i:" + ModContent.ItemType<BirbPheromones>() + "] in the Jungle Biome", 
				"[c/FFD700:The failed experiment returns into its reproductive routine.]");

				//Providence
				bossChecklist.Call(
				"AddBoss", 
				15f, 
				new List<int>() {ModContent.NPCType<Providence>(), ModContent.NPCType<ProvSpawnOffense>(), ModContent.NPCType<ProvSpawnDefense>(), ModContent.NPCType<ProvSpawnHealer>()}, 
				calamity, 
				"Providence", 
				(Func<bool>)(() => CalamityWorld.downedProvidence), 
				new List<int>() {ModContent.ItemType<ProfanedCore>(), ModContent.ItemType<ProfanedCoreUnlimited>()}, 
				new List<int>() {ModContent.ItemType<ProvidenceTrophy>(), ModContent.ItemType<ProvidenceMask>(), ModContent.ItemType<KnowledgeProvidence>()}, 
				new List<int>() {ModContent.ItemType<ProvidenceBag>(), ModContent.ItemType<UnholyEssence>(), ModContent.ItemType<DivineGeode>(), ModContent.ItemType<HolyCollider>(), ModContent.ItemType<SolarFlare>(), ModContent.ItemType<TelluricGlare>(), ModContent.ItemType<BlissfulBombardier>(), ModContent.ItemType<PurgeGuzzler>(), ModContent.ItemType<MoltenAmputator>(), ModContent.ItemType<ElysianWings>(), ModContent.ItemType<ElysianAegis>(), ModContent.ItemType<SamuraiBadge>(), ModContent.ItemType<BlazingCore>(), ModContent.ItemType<RuneofCos>(), ItemID.SuperHealingPotion}, 
				"Use a [i:" + ModContent.ItemType<ProfanedCore>() + "] in the Hallow or Underworld Biomes", 
				"[c/FFA500:The Profaned Goddess vanishes in a burning blaze.]", 
				"CalamityMod/NPCs/Providence/Providence_BossChecklist");

				//Ceaseless Void
				bossChecklist.Call(
				"AddBoss", 
				15.1f, 
				new List<int>() {ModContent.NPCType<CeaselessVoid>(), ModContent.NPCType<DarkEnergy>(), ModContent.NPCType<DarkEnergy2>(), ModContent.NPCType<DarkEnergy3>()}, 
				calamity, 
				"Ceaseless Void", 
				(Func<bool>)(() => CalamityWorld.downedSentinel1), 
				ModContent.ItemType<RuneofCos>(), 
				new List<int>() {ModContent.ItemType<CeaselessVoidTrophy>(), ModContent.ItemType<KnowledgeSentinels>()}, 
				new List<int>() {ModContent.ItemType<DarkPlasma>(), ModContent.ItemType<MirrorBlade>(), ModContent.ItemType<ArcanumoftheVoid>(), ModContent.ItemType<TheEvolution>(), ItemID.SuperHealingPotion}, "Use a [i:" + ModContent.ItemType<RuneofCos>() + "] in the Dungeon", 
				"[c/4B0082:The rift in time and space has moved away from your reach.]");

				//Storm Weaver
				bossChecklist.Call(
				"AddBoss", 
				15.2f, 
				new List<int>() {ModContent.NPCType<StormWeaverHead>(), ModContent.NPCType<StormWeaverBody>(), ModContent.NPCType<StormWeaverTail>(), ModContent.NPCType<StormWeaverHeadNaked>(), ModContent.NPCType<StormWeaverBodyNaked>(), ModContent.NPCType<StormWeaverTailNaked>()}, 
				calamity, 
				"Storm Weaver", 
				(Func<bool>)(() => CalamityWorld.downedSentinel2), ModContent.ItemType<RuneofCos>(), 
				new List<int>() {ModContent.ItemType<WeaverTrophy>(), ModContent.ItemType<KnowledgeSentinels>()}, 
				new List<int>() {ModContent.ItemType<ArmoredShell>(), ModContent.ItemType<TheStorm>(), ModContent.ItemType<StormDragoon>(), ItemID.SuperHealingPotion}, 
				"Use a [i:" + ModContent.ItemType<RuneofCos>() + "] in Space", 
				"[c/EE82EE:Storm Weaver hid itself once again within the stormfront.]", 
				"CalamityMod/NPCs/StormWeaver/StormWeaver_BossChecklist");

				//Signus
				bossChecklist.Call(
				"AddBoss", 
				15.3f, 
				ModContent.NPCType<Signus>(), 
				calamity, 
				"Signus", 
				(Func<bool>)(() => CalamityWorld.downedSentinel3), 
				ModContent.ItemType<RuneofCos>(), 
				new List<int>() {ModContent.ItemType<SignusTrophy>(), ModContent.ItemType<KnowledgeSentinels>()}, 
				new List<int>() {ModContent.ItemType<TwistingNether>(), ModContent.ItemType<Cosmilamp>(), ModContent.ItemType<CosmicKunai>(), ModContent.ItemType<SpectralVeil>(), ItemID.SuperHealingPotion}, 
				"Use a [i:" + ModContent.ItemType<RuneofCos>() + "] in the Underworld", 
				"[c/BA55D3:The Devourer's assassin has finished its easy task.]");

				//Polterghast
				bossChecklist.Call(
				"AddBoss", 
				15.5f, 
				new List<int>() {ModContent.NPCType<Polterghast>(), ModContent.NPCType<PolterPhantom>()}, 
				calamity, 
				"Polterghast", 
				(Func<bool>)(() => CalamityWorld.downedPolterghast), 
				ModContent.ItemType<NecroplasmicBeacon>(), 
				new List<int>() {ModContent.ItemType<PolterghastTrophy>(), ModContent.ItemType<KnowledgePolterghast>()}, 
				new List<int>() {ModContent.ItemType<PolterghastBag>(), ModContent.ItemType<RuinousSoul>(), ModContent.ItemType<Phantoplasm>(), ModContent.ItemType<TerrorBlade>(), ModContent.ItemType<BansheeHook>(), ModContent.ItemType<DaemonsFlame>(), ModContent.ItemType<FatesReveal>(), ModContent.ItemType<GhastlyVisage>(), ModContent.ItemType<EtherealSubjugator>(), ModContent.ItemType<GhoulishGouger>(), ModContent.ItemType<Affliction>(), ModContent.ItemType<Ectoheart>(), ItemID.SuperHealingPotion}, 
				"Kill 30 phantom spirits or use a [i:" + ModContent.ItemType<NecroplasmicBeacon>() + "] in the Dungeon", 
				"[c/B0E0E6:The volatile spirits disperse throughout the depths of the dungeon.]");

				//Devourer of Gods
				bossChecklist.Call(
				"AddBoss", 
				16f, 
				new List<int>() {ModContent.NPCType<DevourerofGodsHead>(), ModContent.NPCType<DevourerofGodsBody>(), ModContent.NPCType<DevourerofGodsTail>()}, 
				calamity, 
				"Devourer of Gods", 
				(Func<bool>)(() => CalamityWorld.downedDoG), ModContent.ItemType<CosmicWorm>(), 
				new List<int>() {ModContent.ItemType<DevourerofGodsTrophy>(), ModContent.ItemType<KnowledgeDevourerofGods>(), ModContent.ItemType<CosmicPlushie>()}, 
				new List<int>() {ModContent.ItemType<DevourerofGodsBag>(), ModContent.ItemType<CosmiliteBar>(), ModContent.ItemType<CosmiliteBrick>(), ModContent.ItemType<Excelsus>(), ModContent.ItemType<EradicatorMelee>(), ModContent.ItemType<TheObliterator>(), ModContent.ItemType<Deathwind>(), ModContent.ItemType<DeathhailStaff>(), ModContent.ItemType<StaffoftheMechworm>(), ModContent.ItemType<Eradicator>(), ModContent.ItemType<Skullmasher>(), ModContent.ItemType<Norfleet>(), ModContent.ItemType<CosmicDischarge>(), ModContent.ItemType<NebulousCore>(), ModContent.ItemType<Fabsol>(), ModContent.ItemType<SupremeHealingPotion>()}, 
				"Use a [i:" + ModContent.ItemType<CosmicWorm>() + "]", 
				"[c/00FFFF:The Devourer of Gods has slain everyone and feasted on their essence.]", 
				"CalamityMod/NPCs/DevourerofGods/DevourerofGods_BossChecklist");

				//Yharon
				bossChecklist.Call(
				"AddBoss", 
				17f, 
				ModContent.NPCType<Yharon>(), 
				calamity, 
				"Yharon", 
				(Func<bool>)(() => CalamityWorld.downedYharon), 
				ModContent.ItemType<ChickenEgg>(), 
				new List<int>() {ModContent.ItemType<YharonTrophy>(), ModContent.ItemType<YharonMask>(), ModContent.ItemType<KnowledgeYharon>(), ModContent.ItemType<ForgottenDragonEgg>(), ModContent.ItemType<FoxDrive>()}, 
				new List<int>() {ModContent.ItemType<YharonBag>(), ModContent.ItemType<HellcasterFragment>(), ModContent.ItemType<DragonRage>(), ModContent.ItemType<TheBurningSky>(), ModContent.ItemType<DragonsBreath>(), ModContent.ItemType<ChickenCannon>(), ModContent.ItemType<PhoenixFlameBarrage>(), ModContent.ItemType<AngryChickenStaff>(), ModContent.ItemType<ProfanedTrident>(), ModContent.ItemType<VoidVortex>(), ModContent.ItemType<YharimsCrystal>(), ModContent.ItemType<YharimsGift>(), ModContent.ItemType<DrewsWings>(), ModContent.ItemType<BossRush>(), ModContent.ItemType<OmegaHealingPotion>()}, 
				"Use a [i:" + ModContent.ItemType<ChickenEgg>() + "] in the Jungle Biome", 
				(CalamityWorld.buffedEclipse ? "[c/FFA500:Yharon prepared itself for the worst possible outcome.]" : "[c/FFA500:Yharon found you too weak to stay near your gravestone.]"), 
				"CalamityMod/NPCs/Yharon/Yharon_BossChecklist");

				//Supreme Calamitas
				bossChecklist.Call(
				"AddBoss", 
				18f, 
				new List<int>() {ModContent.NPCType<SupremeCalamitas>()}, 
				calamity, 
				"Supreme Calamitas", 
				(Func<bool>)(() => CalamityWorld.downedSCal), ModContent.ItemType<EyeofExtinction>(), 
				new List<int>() {ModContent.ItemType<KnowledgeCalamitas>(), ModContent.ItemType<Levi>()}, 
				new List<int>() {ModContent.ItemType<CalamitousEssence>(), ModContent.ItemType<Animus>(), ModContent.ItemType<Azathoth>(), ModContent.ItemType<Contagion>(), ModContent.ItemType<CrystylCrusher>(), ModContent.ItemType<DraconicDestruction>(), ModContent.ItemType<Earth>(), ModContent.ItemType<Fabstaff>(), ModContent.ItemType<RoyalKnivesMelee>(), ModContent.ItemType<RoyalKnives>(), ModContent.ItemType<NanoblackReaperMelee>(), ModContent.ItemType<NanoblackReaperRogue>(), ModContent.ItemType<RedSun>(), ModContent.ItemType<ScarletDevil>(), ModContent.ItemType<SomaPrime>(), ModContent.ItemType<BlushieStaff>(), ModContent.ItemType<Svantechnical>(), ModContent.ItemType<Judgement>(), ModContent.ItemType<TriactisTruePaladinianMageHammerofMightMelee>(), ModContent.ItemType<TriactisTruePaladinianMageHammerofMight>(), ModContent.ItemType<Megafleet>(), ModContent.ItemType<Vehemenc>(), ModContent.ItemType<OmegaHealingPotion>()}, 
				"Use an [i:" + ModContent.ItemType<EyeofExtinction>() + "]", 
				"[c/FFA500:Please don't waste my time.]");

				//King Slime
				bossChecklist.Call(
				"AddToBossLoot", 
				"Terraria", 
				"KingSlime", 
				new List<int>() {ModContent.ItemType<CrownJewel>()});

				bossChecklist.Call(
				"AddToBossCollection", 
				"Terraria", 
				"KingSlime", 
				new List<int>() {ModContent.ItemType<KnowledgeKingSlime>()});

				//Eye of Cthulhu
				bossChecklist.Call(
				"AddToBossLoot", 
				"Terraria", 
				"EyeofCthulhu", 
				new List<int>() {ModContent.ItemType<VictoryShard>(), ModContent.ItemType<TeardropCleaver>(), ModContent.ItemType<CounterScarf>()});

				bossChecklist.Call(
				"AddToBossCollection", 
				"Terraria", 
				"EyeofCthulhu", 
				new List<int>() {ModContent.ItemType<KnowledgeEyeofCthulhu>()});

				//Eater of Worlds
				bossChecklist.Call(
				"AddToBossCollection", 
				"Terraria", 
				"EaterofWorldsHead", 
				new List<int>() {ModContent.ItemType<KnowledgeEaterofWorlds>(), ModContent.ItemType<KnowledgeCorruption>()});

				//Brain of Cthulhu
				bossChecklist.Call(
				"AddToBossCollection", 
				"Terraria", 
				"BrainofCthulhu", 
				new List<int>() {ModContent.ItemType<KnowledgeBrainofCthulhu>(), ModContent.ItemType<KnowledgeCrimson>()});

				//Queen Bee
				bossChecklist.Call(
				"AddToBossLoot", 
				"Terraria", 
				"QueenBee", 
				new List<int>() {ModContent.ItemType<HardenedHoneycomb>()});

				bossChecklist.Call(
				"AddToBossCollection", 
				"Terraria", 
				"QueenBee", 
				new List<int>() {ModContent.ItemType<KnowledgeQueenBee>()});

				//Skeletron
				bossChecklist.Call(
				"AddToBossLoot", 
				"Terraria", 
				"SkeletronHead", 
				new List<int>() {ModContent.ItemType<ClothiersWrath>()});

				bossChecklist.Call(
				"AddToBossCollection", 
				"Terraria", 
				"SkeletronHead", 
				new List<int>() {ModContent.ItemType<KnowledgeSkeletron>()});

				//Wall of Flesh
				bossChecklist.Call(
				"AddToBossLoot", 
				"Terraria", 
				"WallofFlesh", 
				new List<int>() {ModContent.ItemType<Meowthrower>(), ModContent.ItemType<BlackHawkRemote>(), ModContent.ItemType<BlastBarrel>(), ModContent.ItemType<RogueEmblem>(), ModContent.ItemType<MLGRune>(), ItemID.CorruptionKey, ItemID.CrimsonKey});

				bossChecklist.Call(
				"AddToBossCollection", 
				"Terraria", 
				"WallofFlesh", 
				new List<int>() {ModContent.ItemType<KnowledgeWallofFlesh>(), ModContent.ItemType<KnowledgeUnderworld>(), ModContent.ItemType<IbarakiBox>()});

				//The Twins
				bossChecklist.Call(
				"AddToBossCollection", 
				"Terraria", 
				"TheTwins", 
				new List<int>() {ModContent.ItemType<KnowledgeTwins>(), ModContent.ItemType<KnowledgeMechs>()});

				//The Destroyer
				bossChecklist.Call(
				"AddToBossLoot", 
				"Terraria", 
				"TheDestroyer", 
				new List<int>() {ModContent.ItemType<SHPC>()});

				bossChecklist.Call(
				"AddToBossCollection", 
				"Terraria", 
				"TheDestroyer", 
				new List<int>() {ModContent.ItemType<KnowledgeDestroyer>(), ModContent.ItemType<KnowledgeMechs>()});

				//Skeletron Prime
				bossChecklist.Call(
				"AddToBossCollection", 
				"Terraria", 
				"SkeletronPrime", 
				new List<int>() {ModContent.ItemType<KnowledgeSkeletronPrime>(), ModContent.ItemType<KnowledgeMechs>()});

				//Plantera
				bossChecklist.Call(
				"AddToBossLoot", 
				"Terraria", 
				"Plantera", 
				new List<int>() {ModContent.ItemType<LivingShard>(), ModContent.ItemType<BlossomFlux>(), ItemID.JungleKey});

				bossChecklist.Call(
				"AddToBossCollection", 
				"Terraria", 
				"Plantera", 
				new List<int>() {ModContent.ItemType<KnowledgePlantera>()});

				bossChecklist.Call(
				"AddToBossSpawnItems", 
				"Terraria", 
				"Plantera", 
				new List<int>() {ModContent.ItemType<BulbofDoom>()});

				//Golem
				bossChecklist.Call(
				"AddToBossLoot", 
				"Terraria", 
				"Golem", 
				new List<int>() {ModContent.ItemType<EssenceofCinder>(), ModContent.ItemType<AegisBlade>()});

				bossChecklist.Call(
				"AddToBossCollection", 
				"Terraria", 
				"Golem", 
				new List<int>() {ModContent.ItemType<KnowledgeGolem>()});

				bossChecklist.Call(
				"AddToBossSpawnItems", 
				"Terraria", 
				"Golem", 
				new List<int>() {ModContent.ItemType<OldPowerCell>()});

				//Duke Fishron
				bossChecklist.Call(
				"AddToBossLoot", 
				"Terraria", 
				"DukeFishron", 
				new List<int>() {ModContent.ItemType<DukesDecapitator>(), ModContent.ItemType<BrinyBaron>()});

				bossChecklist.Call(
				"AddToBossCollection", 
				"Terraria", 
				"DukeFishron", 
				new List<int>() {ModContent.ItemType<KnowledgeDukeFishron>()});

				//Lunatic Cultist
				bossChecklist.Call(
				"AddToBossLoot", 
				"Terraria", 
				"CultistBoss", 
				new List<int>() {ModContent.ItemType<StardustStaff>(), ModContent.ItemType<ThornBlossom>()});

				bossChecklist.Call(
				"AddToBossCollection", 
				"Terraria", 
				"CultistBoss", 
				new List<int>() {ModContent.ItemType<KnowledgeLunaticCultist>(), ModContent.ItemType<KnowledgeBloodMoon>()});

				bossChecklist.Call(
				"AddToBossSpawnItems", 
				"Terraria", 
				"CultistBoss", 
				new List<int>() {ModContent.ItemType<EidolonTablet>()});

				//Moon Lord
				bossChecklist.Call(
				"AddToBossLoot", 
				"Terraria", 
				"MoonLord", 
				new List<int>() {ModContent.ItemType<UtensilPoker>(), ModContent.ItemType<GrandDad>(), ModContent.ItemType<Infinity>(), ModContent.ItemType<MLGRune2>()});

				bossChecklist.Call(
				"AddToBossCollection", 
				"Terraria", 
				"MoonLord", 
				new List<int>() {ModContent.ItemType<KnowledgeMoonLord>()});

				//Betsy
				bossChecklist.Call(
				"AddToBossLoot", 
				"Terraria", 
				"DD2Betsy", 
				new List<int>() {ModContent.ItemType<Vesuvius>()});

				//Blood Moon
				bossChecklist.Call(
				"AddToBossLoot", 
				"Terraria", 
				"Blood Moon", 
				new List<int>() {ModContent.ItemType<BloodOrb>(), ModContent.ItemType<BouncingEyeball>(), ModContent.ItemType<Carnage>()});

				bossChecklist.Call(
				"AddToBossSpawnItems", 
				"Terraria", 
				"Blood Moon", 
				new List<int>() {ModContent.ItemType<BloodIdol>()});

				//Goblin Army
				bossChecklist.Call(
				"AddToBossLoot", 
				"Terraria", 
				"Goblin Army", 
				new List<int>() {ModContent.ItemType<PlasmaRod>(), ModContent.ItemType<Warblade>(), ModContent.ItemType<TheFirstShadowflame>(), ModContent.ItemType<BurningStrife>()});

				//Pirates
				bossChecklist.Call(
				"AddToBossLoot", 
				"Terraria", 
				"Pirate Invasion", 
				new List<int>() {ModContent.ItemType<RaidersGlory>(), ModContent.ItemType<Arbalest>(), ModContent.ItemType<ProporsePistol>()});

				//Solar Eclipse
				bossChecklist.Call(
				"AddToBossLoot", 
				"Terraria", 
				"Solar Eclipse", 
				new List<int>() {ModContent.ItemType<SolarVeil>(), ModContent.ItemType<DefectiveSphere>(), ModContent.ItemType<DarksunFragment>()});

				//Pumpkin Moon
				bossChecklist.Call(
				"AddToBossLoot", 
				"Terraria", 
				"Pumpkin Moon", 
				new List<int>() {ModContent.ItemType<NightmareFuel>()});

				//Pumpking
				bossChecklist.Call(
				"AddToBossLoot", 
				"Terraria", 
				"Pumpking", 
				new List<int>() {ModContent.ItemType<NightmareFuel>()});

				//Frost Moon
				bossChecklist.Call(
				"AddToBossLoot", 
				"Terraria", 
				"Frost Moon", 
				new List<int>() {ModContent.ItemType<HolidayHalberd>(), ModContent.ItemType<EndothermicEnergy>()});

				//Ice Queen
				bossChecklist.Call(
				"AddToBossLoot", 
				"Terraria", 
				"Ice Queen", 
				new List<int>() {ModContent.ItemType<EndothermicEnergy>()});

				//Martian Madness
				bossChecklist.Call(
				"AddToBossLoot", 
				"Terraria", 
				"Martian Madness", 
				new List<int>() {ModContent.ItemType<Wingman>(), ModContent.ItemType<NullificationRifle>()});

				//Martian Saucer
				bossChecklist.Call(
				"AddToBossLoot", 
				"Terraria", 
				"Martian Saucer", 
				new List<int>() {ModContent.ItemType<NullificationRifle>()});

				//Lunar Events
				bossChecklist.Call(
				"AddToBossLoot", 
				"Terraria", 
				"Lunar Event", 
				new List<int>() {ModContent.ItemType<MeldBlob>()});
            }
        }

        private static void CensusSupport()
        {
            Mod censusMod = ModLoader.GetMod("Census");
            if (censusMod != null)
            {
                censusMod.Call("TownNPCCondition", ModContent.NPCType<SEAHOE>(), "Defeat a Giant Clam");
                censusMod.Call("TownNPCCondition", ModContent.NPCType<THIEF>(), "Have a [i:" + ItemID.PlatinumCoin + "] in your inventory after defeating Skeletron");
                censusMod.Call("TownNPCCondition", ModContent.NPCType<FAP>(), "Have [i:" + ModContent.ItemType<FabsolsVodka>() + "] in your inventory in Hardmode");
                censusMod.Call("TownNPCCondition", ModContent.NPCType<DILF>(), "Defeat Cryogen");
            }
        }
    }
}

using CalamityMod.Buffs;
using CalamityMod.Buffs.Alcohol;
using CalamityMod.Buffs.Cooldowns;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Potions;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Buffs.Summon.Whips;
using CalamityMod.DataStructures;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Summon;
using ReLogic.Reflection;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Systems.Collections
{
    [ReinitializeDuringResizeArrays]
    public static class CalamityBuffSets
    {
        private static SetFactory Factory = BuffID.Sets.Factory;

        /// <summary>
        /// If <see langword="true"/> for a buff type, then that buff will have its duration extended with The Amalgam equipped.<br/>
        /// Also used to remove buffs when contacting the Whispering Maelstrom in the Get fixed boi seed.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] BuffedByAmalgam = Factory.CreateNamedSet("BuffedByAmalgam")
            .Description("Allows this buff to have its duration extended with The Amalgam equipped. Also used to remove buffs when contacting the Whispering Maelstrom in Get fixed boi.")
            .RegisterBoolSet(BuffID.ObsidianSkin, BuffID.Regeneration, BuffID.Swiftness, BuffID.Gills, BuffID.Ironskin, BuffID.ManaRegeneration, BuffID.MagicPower, BuffID.Featherfall,
                BuffID.Spelunker, BuffID.Invisibility, BuffID.Shine, BuffID.NightOwl, BuffID.Battle, BuffID.Thorns, BuffID.WaterWalking, BuffID.Archery, BuffID.Hunter, BuffID.Gravitation,
                BuffID.Tipsy, BuffID.WellFed, BuffID.WellFed2, BuffID.WellFed3, BuffID.Honey, BuffID.WeaponImbueVenom, BuffID.WeaponImbueCursedFlames, BuffID.WeaponImbueFire,
                BuffID.WeaponImbueGold, BuffID.WeaponImbueIchor, BuffID.WeaponImbueNanites, BuffID.WeaponImbueConfetti, BuffID.WeaponImbuePoison, BuffID.Lucky, BuffID.Mining,
                BuffID.Heartreach, BuffID.Calm, BuffID.Builder, BuffID.Titan, BuffID.Flipper, BuffID.Summoning, BuffID.Dangersense, BuffID.AmmoReservation, BuffID.Lifeforce, BuffID.Endurance,
                BuffID.Rage, BuffID.Inferno, BuffID.Wrath, BuffID.Lovestruck, BuffID.Stinky, BuffID.Fishing, BuffID.Sonar, BuffID.Crate, BuffID.Warmth, BuffID.SugarRush,
                BuffType<AnechoicCoatingBuff>(), BuffType<AstralInjectionBuff>(), BuffType<BaguetteBuff>(), BuffType<BloodfinBoost>(), BuffType<BoundingBuff>(), BuffType<CalciumBuff>(),
                BuffType<CeaselessHunger>(), BuffType<GravityNormalizerBuff>(), BuffType<Omniscience>(), BuffType<PhotosynthesisBuff>(), BuffType<ShadowBuff>(), BuffType<Soaring>(),
                BuffType<SulphurskinBuff>(), BuffType<WeaponImbueBrimstone>(), BuffType<WeaponImbueCrumbling>(), BuffType<WeaponImbueHolyFlames>(), BuffType<Zen>(), BuffType<Zerg>(),
                BuffType<BaconOilBuff>(), BuffType<BloodyMaryBuff>(), BuffType<CaribbeanRumBuff>(), BuffType<CinnamonRollBuff>(), BuffType<EverclearBuff>(), BuffType<EvergreenGinBuff>(),
                BuffType<PurpleHazeBuff>(), BuffType<FireballBuff>(), BuffType<GrapeBeerBuff>(), BuffType<MargaritaBuff>(), BuffType<MoonshineBuff>(), BuffType<MoscowMuleBuff>(),
                BuffType<RedWineBuff>(), BuffType<RumBuff>(), BuffType<ScrewdriverBuff>(), BuffType<StarBeamRyeBuff>(), BuffType<TequilaBuff>(), BuffType<TequilaSunriseBuff>(),
                BuffType<Trippy>(), BuffType<VodkaBuff>(), BuffType<WhiskeyBuff>(), BuffType<WhiteWineBuff>());

        /// <summary>
        /// If <see langword="true"/> for a buff type, then that buff is a persistent buff.<br/>
        /// Used by The Amalgam to prevent removing their persistence when the accessory is unequipped.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] IsPersistentBuff = Factory.CreateNamedSet("IsPersistentBuff")
            .Description("Prevents persistent buffs from having their persistence removed by The Amalgam when it is unequipped.")
            .RegisterBoolSet(BuffID.WeaponImbueVenom, BuffID.WeaponImbueCursedFlames, BuffID.WeaponImbueFire, BuffID.WeaponImbueGold, BuffID.WeaponImbueIchor, BuffID.WeaponImbueNanites,
                BuffID.WeaponImbueConfetti, BuffID.WeaponImbuePoison, BuffType<WeaponImbueBrimstone>(), BuffType<WeaponImbueCrumbling>(), BuffType<WeaponImbueHolyFlames>(),
                BuffType<BaconOilBuff>(), BuffType<BloodyMaryBuff>(), BuffType<CaribbeanRumBuff>(), BuffType<CinnamonRollBuff>(), BuffType<EverclearBuff>(), BuffType<EvergreenGinBuff>(),
                BuffType<FireballBuff>(), BuffType<GrapeBeerBuff>(), BuffType<ManhattanBuff>(), BuffType<MargaritaBuff>(), BuffType<MoonshineBuff>(), BuffType<MoscowMuleBuff>(),
                BuffType<OldFashionedBuff>(), BuffType<PurpleHazeBuff>(), BuffType<RedWineBuff>(), BuffType<RumBuff>(), BuffType<ScrewdriverBuff>(), BuffType<StarBeamRyeBuff>(),
                BuffType<TequilaBuff>(), BuffType<TequilaSunriseBuff>(), BuffType<VodkaBuff>(), BuffType<WhiskeyBuff>(), BuffType<WhiteWineBuff>());

        /// <summary>
        /// If <see langword="true"/> for a buff type, then that buff is considered to be a debuff.<br/>
        /// This general-purpose set has several different uses, including Crown Jewel and its upgrades' debuff effects, and removing debuffs with Cleansing Jelly and its upgrades' auras.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] IsDebuff = Factory.CreateNamedSet("IsDebuff")
            .Description("General-purpose set with several different uses, including Crown Jewel line regen, removal via Cleansing Jelly line aura, etc.")
            .RegisterBoolSet(BuffID.Poisoned, BuffID.Darkness, BuffID.Cursed, BuffID.OnFire, BuffID.Bleeding, BuffID.Confused, BuffID.Slow, BuffID.Weak, BuffID.Silenced, BuffID.BrokenArmor,
                BuffID.CursedInferno, BuffID.Frostburn, BuffID.Chilled, BuffID.Frozen, BuffID.Burning, BuffID.Suffocation, BuffID.Ichor, BuffID.Venom, BuffID.Blackout, BuffID.Electrified,
                BuffID.Rabies, BuffID.Webbed, BuffID.Stoned, BuffID.Dazed, BuffID.VortexDebuff, BuffID.WitheredArmor, BuffID.WitheredWeapon, BuffID.ShadowFlame, BuffID.OgreSpit, BuffID.BetsysCurse,
                BuffID.Wet, BuffID.Slimed, BuffID.OnFire3, BuffID.Frostburn2, BuffType<SulphuricPoisoning>(), BuffType<Shadowflame>(), BuffType<Daybroken>(), BuffType<BrimstoneFlames>(), BuffType<BurningBlood>(),
                BuffType<BrainRot>(), BuffType<ElementalMix>(), BuffType<GodSlayerInferno>(), BuffType<AstralInfectionDebuff>(), BuffType<HolyFlames>(), BuffType<Irradiated>(),
                BuffType<Plague>(), BuffType<CrushDepth>(), BuffType<HadopelagicPressure>(), BuffType<RiptideDebuff>(), BuffType<MarkedforDeath>(), BuffType<HeavyBleeding>(),
                BuffType<Laceration>(), BuffType<AbsorberAffliction>(), BuffType<ArmorCrunch>(), BuffType<Crumbling>(), BuffType<Vaporfied>(), BuffType<Eutrophication>(), BuffType<Dragonfire>(),
                BuffType<VermillionFlux>(), BuffType<AuricRebuke>(), BuffType<StaticDischarge>(), BuffType<Nightwither>(), BuffType<Voidfrost>(), BuffType<VulnerabilityHex>(),
                BuffType<MiracleBlight>(), BuffType<WhisperingDeath>(), BuffType<FrozenLungs>(), BuffType<FishAlert>(), BuffType<HolyInferno>(), BuffType<IcarusFolly>(),
                BuffType<DoGExtremeGravity>(), BuffType<PopoNoselessBuff>(), BuffType<SagePoison>(), BuffType<SearingLava>(), BuffType<WeakBrimstoneFlames>(), BuffType<Withered>(),
                BuffType<ManaBurn>(), BuffType<DemonicFlames>(), BuffType<Bane>(), BuffType<Shred>(), BuffType<WindChilled>());

        /// <summary>
        /// If <see langword="true"/> for a buff type, then that buff is a whip tag buff on the player.<br/>
        /// Used to prevent "whip stacking", the ability to grant the player multiple whip buff effects at once.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] IsSummonTagBuff = Factory.CreateNamedSet("IsSummonTagBuff")
            .Description("Whip tag buffs for the player, used to prevent whip stacking.")
            .RegisterBoolSet(BuffID.CoolWhipPlayerBuff, BuffID.ScytheWhipPlayerBuff, BuffID.SwordWhipPlayerBuff, BuffID.ThornWhipPlayerBuff, BuffType<ProfanedCrystalWhipBuff>());

        /// <summary>
        /// Associates a buff type with its <see cref="SummonTag"/> structure. If a buff type is not a key in this dictionary, then it has no associated <see cref="SummonTag"/>.<br/>
        /// Used for several different effects, such as applying tag effects to NPCs, preventing "whip stacking" on NPCs, and drawing the tag effect icon below the NPC. 
        /// </summary>
        public static SummonTag[] SummonTagDebuff = Factory.CreateNamedSet("SummonTagDebuff")
            .Description("Associates a buff type with its SummonTag structure. Used for several different effects, such as applying tag effects to NPCs, preventing whip stacking on NPCs, and drawing the tag effect icon below the NPC.")
            .RegisterCustomSet<SummonTag>(null,
            BuffID.BlandWhipEnemyDebuff, SummonTag.LeatherWhip,
            BuffID.BoneWhipNPCDebuff, SummonTag.SpinalTap,
            BuffID.CoolWhipNPCDebuff, SummonTag.CoolWhip,
            BuffID.FlameWhipEnemyDebuff, SummonTag.Firecracker,
            BuffID.MaceWhipNPCDebuff, SummonTag.MorningStar,
            BuffID.RainbowWhipNPCDebuff, SummonTag.Kaleidoscope,
            BuffID.ScytheWhipEnemyDebuff, SummonTag.DarkHarvest,
            BuffID.SwordWhipNPCDebuff, SummonTag.Durendal ,
            BuffID.ThornWhipNPCDebuff, SummonTag.Snapthorn,
            BuffType<CnidarianSummonTagBuff>(), Cnidarian.summonTag,
            BuffType<ForbiddenStealthSummonTagBuff>(), ForbiddenCirclet.summonTag,
            BuffType<ProfanedCrystalWhipDebuff>(), ProfanedSoulCrystal.SummonTag,
            BuffType<VoidConcentrationSummonTagBuff>(), VoidConcentrationStaff.summonTag
        );

        private static DebuffData Alch(int level) => new DebuffData() { AlcoholLevel = level };
        public static DebuffData[] DebuffDataset = BuffID.Sets.Factory.CreateNamedSet("DebuffData")
            .Description("Stores DebuffData for a particular debuff")
            .RegisterCustomSet<DebuffData>(null,
                BuffID.OnFire, DebuffData.OnFire,
                BuffID.OnFire3, DebuffData.Hellfire,
                BuffID.CursedInferno, DebuffData.CursedInferno,
                BuffID.ShadowFlame, DebuffData.Shadowflame,
                BuffID.Daybreak, DebuffData.Daybroken,
                BuffID.Burning, DebuffData.Burning,
                BuffID.Frostburn, DebuffData.Frostburn,
                BuffID.Frostburn2, DebuffData.Frostbite,
                BuffID.Poisoned, DebuffData.Poisoned,
                BuffID.Venom, DebuffData.AcidVenom,
                BuffID.Electrified, DebuffData.Electrified,
                BuffID.Oiled, DebuffData.Oiled,

                BuffID.Tipsy, Alch(1),
                BuffType<BloodyMaryBuff>(), Alch(1),
                BuffType<CaribbeanRumBuff>(), Alch(1),
                BuffType<CinnamonRollBuff>(), Alch(1),
                BuffType<EvergreenGinBuff>(), Alch(1),
                BuffType<FireballBuff>(), Alch(1),
                BuffType<GrapeBeerBuff>(), Alch(1),
                BuffType<ManhattanBuff>(), Alch(1),
                BuffType<MargaritaBuff>(), Alch(1),
                BuffType<MoonshineBuff>(), Alch(1),
                BuffType<MoscowMuleBuff>(), Alch(1),
                BuffType<OldFashionedBuff>(), Alch(1),
                BuffType<PurpleHazeBuff>(), Alch(1),
                BuffType<RedWineBuff>(), Alch(1),
                BuffType<RumBuff>(), Alch(1),
                BuffType<ScrewdriverBuff>(), Alch(1),
                BuffType<StarBeamRyeBuff>(), Alch(1),
                BuffType<TequilaBuff>(), Alch(1),
                BuffType<TequilaSunriseBuff>(), Alch(1),
                BuffType<VodkaBuff>(), Alch(1),
                BuffType<WhiskeyBuff>(), Alch(1),
                BuffType<WhiteWineBuff>(), Alch(1),

                BuffType<EverclearBuff>(), Alch(2),
                BuffType<BaconOilBuff>(), Alch(3)
            );
        public static int GetBuffIDFromAlcoholType(AlcoholType type)
        {
            return type switch
            {
                AlcoholType.BaconOil => BuffType<BaconOilBuff>(),
                AlcoholType.BloodyMary => BuffType<BloodyMaryBuff>(),
                AlcoholType.CaribbeanRum => BuffType<CaribbeanRumBuff>(),
                AlcoholType.CinnamonRoll => BuffType<CinnamonRollBuff>(),
                AlcoholType.Everclear => BuffType<EverclearBuff>(),
                AlcoholType.EvergreenGin => BuffType<EvergreenGinBuff>(),
                AlcoholType.Fireball => BuffType<FireballBuff>(),
                AlcoholType.GrapeBeer => BuffType<GrapeBeerBuff>(),
                AlcoholType.Manhattan => BuffType<ManhattanBuff>(),
                AlcoholType.Margarita => BuffType<MargaritaBuff>(),
                AlcoholType.Moonshine => BuffType<MoonshineBuff>(),
                AlcoholType.MoscowMule => BuffType<MoscowMuleBuff>(),
                AlcoholType.OldFashioned => BuffType<OldFashionedBuff>(),
                AlcoholType.PurpleHaze => BuffType<PurpleHazeBuff>(),
                AlcoholType.RedWine => BuffType<RedWineBuff>(),
                AlcoholType.Rum => BuffType<RumBuff>(),
                AlcoholType.Screwdriver => BuffType<ScrewdriverBuff>(),
                AlcoholType.StarBeamRye => BuffType<StarBeamRyeBuff>(),
                AlcoholType.Tequila => BuffType<TequilaBuff>(),
                AlcoholType.TequilaSunrise => BuffType<TequilaSunriseBuff>(),
                AlcoholType.Vodka => BuffType<VodkaBuff>(),
                AlcoholType.Whiskey => BuffType<WhiskeyBuff>(),
                AlcoholType.WhiteWine => BuffType<WhiteWineBuff>(),

                // Vanilla treats both Ale and Sake as the "Tipsy" buff
                AlcoholType.Ale => BuffID.Tipsy,
                AlcoholType.Sake => BuffID.Tipsy,

                _ => -1
            };
        }
    }
}

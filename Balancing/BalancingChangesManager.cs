using System.Collections.Generic;
using System.Linq;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Summon.MirrorofKalandraMinions;
using CalamityMod.Projectiles.Typeless;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Balancing
{
    // TODO -- This can be made into a ModSystem with simple OnModLoad and Unload hooks.
    public static class BalancingChangesManager
    {
        internal static List<IBalancingRule[]> UniversalBalancingChanges = null;
        internal static List<NPCBalancingChange> NPCSpecificBalancingChanges = null;

        // Balancing changes in this method are sorted based on place in progression, NPC name (from A-Z), and strength of resistance in ascending order, in that level of priority.
        // For ease of change, changes that are not exclusive to one specific weapon are not bundled into one line if they share the same resistance factor.
        // To give an example of this, Thanatos having a 50% resist to Chicken Cannon and Prismatic Breaker should be two distinct lines with a 0.5x factor instead of hamfisting them all
        // into one single resist that may have to be split later.
        internal static void Load()
        {
            // Dirty shorthand for true melee resists, because they're super common and other class resists aren't.
            IBalancingRule ResistTrueMelee(float f) => new ClassResistBalancingRule(f, TrueMeleeDamageClass.Instance);

            // Declare specific filters.
            bool BigGaelsSkullFilter(Projectile p) =>
                p.type == ProjectileType<GaelSkull>() && p.ai[1] == 1;

            bool DragonRageFilter(Projectile p) =>
                p.type == ProjectileType<DragonRageStaff>() || p.type == ProjectileType<DragonRageFireball>() || (p.type == ProjectileType<FuckYou>() && p.CountsAsClass<MeleeDamageClass>());

            // bool MonkStaffT3Filter(Projectile p) =>
            //     p.type == ProjectileID.MonkStaffT3_AltShot || (p.type == ProjectileID.Electrosphere && Main.player[p.owner].ActiveItem().type == ItemID.MonkStaffT3);

            bool MushroomSpearFilter(Projectile p) =>
                p.type == ProjectileID.Mushroom && Main.player[p.owner].ActiveItem().type == ItemID.MushroomSpear;

            bool SpectreMaskSetBonusFilter(Projectile p) =>
                p.type == ProjectileID.SpectreWrath && Main.player[p.owner].ghostHurt;

            bool AotCThrowCombo(Projectile p) =>
                p.type == ProjectileType<ArkoftheCosmosSwungBlade>() && (p.ai[0] == 2 || p.ai[0] == 3);

            UniversalBalancingChanges = new List<IBalancingRule[]>()
            {
                // Nerf Crystal bullet shards by 45%
                // Currently crystal bullet projectiles deal 50% of the bullet's damage which is absurd in vanilla, this nerfs them to 27.5%
                Do(new ProjectileResistBalancingRule(0.55f, ProjectileID.CrystalShard)),

                // Nerf Luminite Arrow trails by 50%.
                Do(new ProjectileResistBalancingRule(0.5f, ProjectileID.MoonlordArrowTrail)),
                
                // Nerf Seedler seeds by 25%.
                Do(new ProjectileResistBalancingRule(0.75f, ProjectileID.SeedlerNut, ProjectileID.SeedlerThorn)),

                // Nerf Cursed Dart flames by 50%.
                Do(new ProjectileResistBalancingRule(0.5f, ProjectileID.CursedDartFlame)),

                // Nerf Mushroom Spear projectiles by 50%.
                Do(new ProjectileSpecificRequirementBalancingRule(0.5f, MushroomSpearFilter)),

                // Nerf Spectre Mask set bonus projectiles by 30%.
                Do(new ProjectileSpecificRequirementBalancingRule(0.7f, SpectreMaskSetBonusFilter)),

            };

            NPCSpecificBalancingChanges = new List<NPCBalancingChange>();

            #region Desert Scourge
            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.DesertScourgeIDs, Do(ResistTrueMelee(0.5f))));
            #endregion

            #region Crabulon
            // 50% resist to true melee.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<Crabulon>(), ResistTrueMelee(0.5f)));
            #endregion

            #region Brain of Cthulhu: Creepers
            // 50% resist to true melee.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.Creeper, Do(ResistTrueMelee(0.5f))));

            // 50% resist to Demon Scythe.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.Creeper, Do(new ProjectileResistBalancingRule(0.5f, ProjectileID.DemonScythe))));
            #endregion

            #region Eater of Worlds
            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.EaterofWorldsIDs, Do(ResistTrueMelee(0.5f))));

            // 50% resist to Demon Scythe.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.EaterofWorldsIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileID.DemonScythe))));

            // 40% resist to Sky Glaze.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.EaterofWorldsIDs, Do(new ProjectileResistBalancingRule(0.6f, ProjectileType<StickyFeather>()))));
            #endregion

            #region The Perforators
            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.PerforatorIDs, Do(ResistTrueMelee(0.5f))));
            #endregion

            #region Wall of Flesh
            // 40% resist to Corro/Crimslime Staff.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.WallofFleshEye, Do(new ProjectileResistBalancingRule(0.6f, ProjectileType<CrimslimeMinion>(), ProjectileType<CorroslimeMinion>()))));
            #endregion

            #region Aquatic Scourge
            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AquaticScourgeIDs, Do(ResistTrueMelee(0.5f))));

            // 50% resist to Adamantite Throwing Axe's lightning.
            // Twisting Thunder and Gael's Greatsword use this projectile too, but they are far beyond progression, so who cares!
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AquaticScourgeIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileID.CultistBossLightningOrbArc))));

            // 50% resist to Dormant Brimseeker.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AquaticScourgeIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<DormantBrimseekerBab>()))));

            // 50% resist to Mounted Scanner.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AquaticScourgeIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<MountedScannerLaser>()))));

            // 40% resist to Cryophobia.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AquaticScourgeIDs, Do(new ProjectileResistBalancingRule(0.6f, ProjectileType<CryoBlast>()))));

            // 40% resist to SHPC.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AquaticScourgeIDs, Do(new ProjectileResistBalancingRule(0.6f, ProjectileType<SHPExplosion>()))));

            // 30% resist to Meowthrower.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AquaticScourgeIDs, Do(new ProjectileResistBalancingRule(0.7f, ProjectileType<MeowFire>()))));

            // 25% resist to Snowstorm Staff.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AquaticScourgeIDs, Do(new ProjectileResistBalancingRule(0.75f, ProjectileType<Snowflake>(), ProjectileType<SnowflakeIceStar>()))));
            #endregion

            #region The Destroyer
            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.DestroyerIDs, Do(ResistTrueMelee(0.5f))));

            // 50% resist to Adamantite Throwing Axe's lightning.
            // See Aquatic Scourge comment.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.DestroyerIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileID.CultistBossLightningOrbArc))));

            // 50% resist to Aftershock's rocks.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.DestroyerIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<AftershockRock>()))));

            // 50% resist to Avalanche's Ice Bombs and shards.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.DestroyerIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<IceBombFriendly>(), ProjectileType<FrostShardFriendly>()))));

            // 50% resist to Dormant Brimseeker.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.DestroyerIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<DormantBrimseekerBab>()))));

            // 45% resist to Mounted Scanner.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.DestroyerIDs, Do(new ProjectileResistBalancingRule(0.55f, ProjectileType<MountedScannerLaser>()))));

            // 40% resist to Meowthrower.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.DestroyerIDs, Do(new ProjectileResistBalancingRule(0.6f, ProjectileType<MeowFire>()))));

            // 25% resist to SHPC.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.DestroyerIDs, Do(new ProjectileResistBalancingRule(0.75f, ProjectileType<SHPExplosion>()))));

            // 25% resist to Snowstorm Staff.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.DestroyerIDs, Do(new ProjectileResistBalancingRule(0.75f, ProjectileType<Snowflake>(), ProjectileType<SnowflakeIceStar>()))));
            #endregion

            #region Astrum Aureus
            // 30% resist to The Ballista's greatarrows.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<AstrumAureus>(), Do(new ProjectileResistBalancingRule(0.7f, ProjectileType<BallistaGreatArrow>()))));
            #endregion

            #region Ravager
            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.RavagerIDs, Do(ResistTrueMelee(0.5f))));

            // 50% resist to Aegis Blade's explosions because the true melee resist ain't enough.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.RavagerIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<AegisBlast>()))));

            // 50% resist to Icicle Arrows.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.RavagerIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<IcicleArrowProj>()))));

            // 35% resist to Flying Dragon.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.RavagerIDs, Do(new ProjectileResistBalancingRule(0.65f, ProjectileID.DD2SquireSonicBoom))));

            // 30% resist to Aurora Blazer.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.RavagerIDs, Do(new ProjectileResistBalancingRule(0.7f, ProjectileType<AuroraFire>()))));

            // 30% resist to Flak Kraken.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.RavagerIDs, Do(new ProjectileResistBalancingRule(0.7f, ProjectileType<FlakKrakenProjectile>()))));

            // 25% resist to Lucrecia.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.RavagerIDs, Do(new ProjectileResistBalancingRule(0.75f, ProjectileType<DNA>()))));

            // 20% resist to The Hive.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.RavagerIDs, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<HiveNuke>(), ProjectileType<HiveMissile>()))));
            #endregion

            #region Duke Fishron
            // 20% vulnerability to The Hive's bees.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.DukeFishron, Do(new ProjectileResistBalancingRule(1.2f, ProjectileType<BasicPlagueBee>()))));

            // 25% vulnerability to Resurrection Butterfly.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.DukeFishron, Do(new ProjectileResistBalancingRule(1.25f, ProjectileType<SakuraBullet>(), ProjectileType<PurpleButterfly>()))));
            #endregion

            #region Empress of Light
            // 25% resist to Abyss Blade's blade.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.HallowBoss, Do(new ProjectileResistBalancingRule(0.75f, ProjectileType<AbyssBladeProjectile>()))));

            // 20% resist to Plague Tainted SMG's drones.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.HallowBoss, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<PlagueTaintedDrone>()))));
            #endregion

            #region Lunatic Cultist
            // 50% resist to Aegis Blade's explosions.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.CultistBoss, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<AegisBlast>()))));

            // 35% resist to Subduction Slicer's flame pillars.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.CultistBoss, Do(new ProjectileResistBalancingRule(0.65f, ProjectileType<SubductionFlameburst>()))));

            // 20% resist to Art Attack.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.CultistBoss, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<ArtAttackStrike>()))));

            // 20% resist to Nightglow.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.CultistBoss, Do(new ProjectileResistBalancingRule(0.8f, ProjectileID.FairyQueenMagicItemShot))));
            #endregion

            #region Astrum Deus
            // 75% resist to Plaguenades.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AstrumDeusIDs, Do(new ProjectileResistBalancingRule(0.25f, ProjectileType<PlaguenadeBee>(), ProjectileType<PlaguenadeProj>()))));

            // 70% resist to Stardust Dragon Staff.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AstrumDeusIDs, Do(new ProjectileResistBalancingRule(0.3f, ProjectileID.StardustDragon1, ProjectileID.StardustDragon2, ProjectileID.StardustDragon3, ProjectileID.StardustDragon4))));

            // 70% resist to Charged Blaster Cannon.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AstrumDeusIDs, Do(new ProjectileResistBalancingRule(0.3f, ProjectileID.ChargedBlasterLaser))));

            // 60% resist to Aurora Blazer.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AstrumDeusIDs, Do(new ProjectileResistBalancingRule(0.4f, ProjectileType<AuroraFire>()))));

            // 55% resist to Flak Kraken.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AstrumDeusIDs, Do(new ProjectileResistBalancingRule(0.45f, ProjectileType<FlakKrakenProjectile>()))));

            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AstrumDeusIDs, Do(ResistTrueMelee(0.5f))));

            // 35% resist to Ballistic Poison Bomb's clouds.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AstrumDeusIDs, Do(new ProjectileResistBalancingRule(0.65f, ProjectileType<BallisticPoisonCloud>()))));

            // 35% resist to Icicle Arrows.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AstrumDeusIDs, Do(new ProjectileResistBalancingRule(0.65f, ProjectileType<IcicleArrowProj>()))));

            // 20% resist to Cluster Fragments.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AstrumDeusIDs, Do(new ProjectileResistBalancingRule(0.8f, ProjectileID.ClusterFragmentsI, ProjectileID.ClusterFragmentsII))));
            #endregion

            #region Moon Lord
            // 20% resist to Mercurial Tides (True Biome Blade).
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.MoonLordCore, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<MercurialTides>(), ProjectileType<MercurialTidesMonolith>(), ProjectileType<MercurialTidesBlast>()))));

            // 20% resist to The Hive's nuke.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.MoonLordCore, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<HiveNuke>()))));
            #endregion

            #region Profaned Guardians
            // 50% resist to true melee for the Defense Guardian's rocks.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<ProfanedRocks>(), ResistTrueMelee(0.5f)));
            #endregion

            #region Providence
            // 80% resist to Hell's Sun.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<Providence>(), new ProjectileResistBalancingRule(0.2f, ProjectileType<HellsSunProj>())));

            // 35% resist to Elemental Lance. This thing desperately needs a rework.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<Providence>(), new ProjectileResistBalancingRule(0.65f, ProjectileType<SpatialSpear>(), ProjectileType<SpatialSpear2>(), ProjectileType<SpatialSpear3>(), ProjectileType<SpatialSpear4>())));
            #endregion

            #region Ceaseless Void: Dark Energies
            // 50% resist to true melee.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<DarkEnergy>(), ResistTrueMelee(0.5f)));

            // 40% resist to Stellar Torus Staff.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<DarkEnergy>(), new ProjectileResistBalancingRule(0.6f, ProjectileType<StellarTorusBeam>())));
            #endregion

            #region Storm Weaver
            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.StormWeaverIDs, Do(ResistTrueMelee(0.5f))));

            // 50% resist to Dazzling Stabber Staff.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.StormWeaverIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<DazzlingStabber>()))));

            // 50% resist to Elemental Axe.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.StormWeaverIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<ElementalAxeMinion>()))));

            // 50% resist to Pristine Fury's alt fire.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.StormWeaverIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<PristineSecondary>()))));

            // 50% resist to Tactician's Trump Card's explosions.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.StormWeaverIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<TacticiansElectricBoom>()))));

            // 25% resist to Molten Amputator's blobs.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.StormWeaverIDs, Do(new ProjectileResistBalancingRule(0.75f, ProjectileType<MoltenBlobThrown>()))));
            #endregion

            #region Old Duke
            // 20% resist to Time Bolt.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<OldDuke>(), new ProjectileResistBalancingRule(0.8f, ProjectileType<TimeBoltKnife>())));
            #endregion

            #region The Devourer of Gods
            // 80% resist to Wave Pounder.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.DevourerOfGodsIDs, Do(new ProjectileResistBalancingRule(0.2f, ProjectileType<WavePounderBoom>()))));

            // 50% resist to Eidolic Wail.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.DevourerOfGodsIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<EidolicWailSoundwave>()))));

            // 35% resist to Venusian Trident's explosions.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.DevourerOfGodsIDs, Do(new ProjectileResistBalancingRule(0.65f, ProjectileType<VenusianBolt>(), ProjectileType<VenusianExplosion>()))));

            // 20% resist to Valediction's typhoons; will catch Nuclear Fury as well but that doesn't matter.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.DevourerOfGodsIDs, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<NuclearFuryProjectile>()))));

            // 15% vulnerability to Time Bolt stealth strikes.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.DevourerOfGodsIDs, Do(new StealthStrikeBalancingRule(1.15f, ProjectileType<TimeBoltKnife>()))));
            #endregion The Devourer of Gods

            #region Yharon
            // 15% resist to Time Bolt.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<Yharon>(), Do(new ProjectileResistBalancingRule(0.85f, ProjectileType<TimeBoltKnife>()))));
            #endregion

            #region Exo Mechs: Ares
            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(ResistTrueMelee(0.5f))));

            // 30% resist to Dynamic Pursuer.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(new ProjectileResistBalancingRule(0.7f, ProjectileType<DynamicPursuerProjectile>(), ProjectileType<DynamicPursuerLaser>(), ProjectileType<DynamicPursuerElectricity>()))));

            // 25% resist to Aetherflux Cannon.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(new ProjectileResistBalancingRule(0.75f, ProjectileType<PhasedGodRay>()))));

            // 20% resist to the Spin Throw part of the Ark of the Cosmos' combo.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(new ProjectileSpecificRequirementBalancingRule(0.8f, AotCThrowCombo))));

            // 20% resist to Dragon Rage projectiles.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(new ProjectileSpecificRequirementBalancingRule(0.8f, DragonRageFilter))));

            // 20% resist to Eclipse's Fall.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<EclipsesSmol>(), ProjectileType<EclipsesFallMain>()))));

            // 20% resist to Rancor.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<RancorLaserbeam>()))));

            // 20% resist to Yharim's Crystal.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<YharimsCrystalBeam>()))));

            // 20% resist to Zenith.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(new ProjectileResistBalancingRule(0.8f, ProjectileID.FinalFractal))));
            #endregion

            #region Exo Mechs: Artemis and Apollo
            // Artemis and Apollo have to be defined here because they aren't a pre-existing list.
            // TODO -- NPC sets (mostly worm bosses) should probably be their own holding class.
            int[] exoTwins = new int[] { NPCType<Artemis>(), NPCType<Apollo>() };

            // 10% resist to Eclipse's Fall's mini spears.
            NPCSpecificBalancingChanges.AddRange(Bundle(exoTwins, Do(new ProjectileResistBalancingRule(0.9f, ProjectileType<EclipsesSmol>()))));
            #endregion

            #region Exo Mechs: Thanatos
            // 65% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(ResistTrueMelee(0.35f))));

            // 65% resist to Stellar Torus Staff.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.35f, ProjectileType<StellarTorusBeam>()))));

            // 65% resist to The Enforcer's projectiles.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.35f, ProjectileType<EssenceFlame2>()))));

            // 65% resist to The Final Dawn's lunge.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.35f, ProjectileType<FinalDawnThrow2>()))));

            // 60% resist to Dynamic Pursuer.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.4f, ProjectileType<DynamicPursuerProjectile>(), ProjectileType<DynamicPursuerLaser>(), ProjectileType<DynamicPursuerElectricity>()))));

            // 50% resist to Chicken Cannon.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<ChickenExplosion>()))));

            // 50% resist to Rancor.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<RancorLaserbeam>()))));

            // 50% resist to Tarragon Throwing Dart's thorns. (LOL)
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<TarraThornRight>()))));

            // 50% resist to Omicron's beam.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<OmicronBeam>()))));

            // 50% resist to Vehemence's skulls.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<VehemenceSkull>()))));

            // 50% resist to Yharim's Crystal.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<YharimsCrystalBeam>()))));

            // 45% resist to Wrathwing's stealth strike fireballs.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.55f, ProjectileType<WrathwingCinder>()))));

            // 35% resist to Prismatic Breaker's laser beam.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.65f, ProjectileType<PrismaticBeam>()))));

            // 30% resist to Eclipse's Fall.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.7f, ProjectileType<EclipsesFallMain>()))));

            // 30% resist to Sirius.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.7f, ProjectileType<SiriusExplosion>()))));

            // 30% resist to Ultima's rays.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.7f, ProjectileType<UltimaRay>()))));

            // 30% resist to Dragon Scales and The Wand's tornadoes.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.7f, ProjectileType<InfernadoFriendly>(), ProjectileType<DragonScalesInfernado>()))));

            // 25% resist to the Spin Throw part of the Ark of the Cosmos' combo.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileSpecificRequirementBalancingRule(0.75f, AotCThrowCombo))));

            // 25% resist to Dragon Rage projectiles.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileSpecificRequirementBalancingRule(0.75f, DragonRageFilter))));

            // 25% resist to Gael's Greatsword's big skulls.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileSpecificRequirementBalancingRule(0.75f, BigGaelsSkullFilter))));

            // 25% resist to Mirror of Kalandra's Paradoxica minion.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.75f, ProjectileType<Paradoxica>()))));

            // 25% resist to Zenith.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.75f, ProjectileID.FinalFractal))));

            // 20% resist to The Anomaly's Nanogun's bomb explosions.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<AnomalysNanogunMPFBBoom>()))));

            // 20% resist to Ariane's aura (Lilies of Finality).
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<LiliesOfFinalityAoE>()))));

            // 20% resist to Blood Boiler.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<BloodBoilerFire>()))));

            // 20% resist to Eradicator's beams.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<NebulaShot>()))));

            // 20% resist to Plasma Grenade and Dynamic Pursuer's explosions.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<PlasmaGrenadeProjectile>(), ProjectileType<PlasmaGrenadeSmallExplosion>(), ProjectileType<MassivePlasmaExplosion>()))));

            // 20% resist to Voltaic Climax / Void Vortex hitscan beams.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<ClimaxBeam>()))));

            // 15% resist to The Final Dawn's AoE sweep flames.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.85f, ProjectileType<FinalDawnFlame>()))));

            // 15% resist to God Slayer Slugs.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.85f, ProjectileType<GodSlayerSlugProj>()))));

            // 15% resist to Gruesome Eminence.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.85f, ProjectileType<SpiritCongregation>()))));

            // 15% resist to Luminite Bullets.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.85f, ProjectileID.MoonlordBullet))));
            #endregion

            #region Supreme Calamitas: Brimstone Hearts
            // 30% resist to Surge Driver's alt click comets.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<BrimstoneHeart>(), new ProjectileResistBalancingRule(0.7f, ProjectileType<PrismComet>())));

            // 30% resist to Supernova
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<BrimstoneHeart>(), new ProjectileResistBalancingRule(0.7f, ProjectileType<SupernovaBoom>(), ProjectileType<SupernovaStealthBoom>())));

            // 20% resist to Executioner's Blade stealth strikes.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<BrimstoneHeart>(), new ProjectileResistBalancingRule(0.8f, ProjectileType<ExecutionersBladeStealthProj>())));
            #endregion

            #region Supreme Calamitas: Soul Seekers
            // 85% resist to Chicken Cannon.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.15f, ProjectileType<ChickenExplosion>())));

            // 50% resist to Subsuming Vortex.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.5f, ProjectileType<ExoVortex>(), ProjectileType<ExoVortex2>(), ProjectileType<EnormousConsumingVortex>())));

            // 40% resist to Supernova
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.6f, ProjectileType<SupernovaBoom>(), ProjectileType<SupernovaStealthBoom>())));

            // 30% resist to Zenith.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.7f, ProjectileID.FinalFractal)));

            // 25% resist to Yharim's Crystal.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.75f, ProjectileType<YharimsCrystalBeam>())));

            // 10% resist to Celestus.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.9f, ProjectileType<CelestusProj>(), ProjectileType<CelestusMiniScythe>())));

            // 10% resist to Executioner's Blade stealth strikes.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.9f, ProjectileType<ExecutionersBladeStealthProj>())));

            // 10% resist to Surge Driver's alt click comets.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.9f, ProjectileType<PrismComet>())));

            // 15% vulnerability to The Atom Splitter. (Yes, this is kinda weird, but it's what Piky asked for).
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(1.15f, ProjectileType<TheAtomSplitterProjectile>(), ProjectileType<TheAtomSplitterDuplicate>())));
            #endregion
        }

        internal static void Unload()
        {
            UniversalBalancingChanges = null;
            NPCSpecificBalancingChanges = null;
        }

        public static void ApplyFromProjectile(NPC npc, ref NPC.HitModifiers modifiers, Projectile proj)
        {
            // Apply universal balancing rules.
            foreach (IBalancingRule[] balancingRules in UniversalBalancingChanges)
            {
                foreach (IBalancingRule balancingRule in balancingRules)
                {
                    if (balancingRule.AppliesTo(npc, modifiers, proj))
                        balancingRule.ApplyBalancingChange(npc, ref modifiers);
                }
            }

            // As well as rules specific to NPCs.
            foreach (NPCBalancingChange balanceChange in NPCSpecificBalancingChanges)
            {
                if (npc.type != balanceChange.NPCType)
                    continue;

                foreach (IBalancingRule balancingRule in balanceChange.BalancingRules)
                {
                    if (balancingRule.AppliesTo(npc, modifiers, proj))
                        balancingRule.ApplyBalancingChange(npc, ref modifiers);
                }
            }
        }

        // This function simply concatenates a bunch of balancing rules into an array.
        // It looks a lot nicer than constantly typing "new IBalancingRule[]".
        internal static IBalancingRule[] Do(params IBalancingRule[] rules) => rules;

        // Shorthand for bundling balancing balancing rules in such a way that it applies to multiple NPCs at once.
        // This is useful for preventing having to copy-paste/store the rules and apply it to each NPC individually.
        internal static NPCBalancingChange[] Bundle(IEnumerable<int> npcIDs, params IBalancingRule[] rules)
        {
            NPCBalancingChange[] changes = new NPCBalancingChange[npcIDs.Count()];
            for (int i = 0; i < changes.Length; i++)
                changes[i] = new NPCBalancingChange(npcIDs.ElementAt(i), rules);
            return changes;
        }
    }
}

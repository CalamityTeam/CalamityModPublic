using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Projectiles.DraedonsArsenal;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using CalamityMod.NPCs.ProfanedGuardians;

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
                // Nerf Luminite Arrow trails by 50%.
                Do(new ProjectileResistBalancingRule(0.5f, ProjectileID.MoonlordArrowTrail)),
                
                // Nerf Seedler seeds by 66.6%.
                Do(new ProjectileResistBalancingRule(1f / 3f, ProjectileID.SeedlerNut, ProjectileID.SeedlerThorn)),

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

            // 50% resist to Demon Scythes.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.Creeper, Do(new ProjectileResistBalancingRule(0.5f, ProjectileID.DemonScythe))));
            #endregion

            #region Eater of Worlds
            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.EaterofWorldsIDs, Do(ResistTrueMelee(0.5f))));

            // 50% resist to Demon Scythes.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.EaterofWorldsIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileID.DemonScythe))));

            // 40% resist to Sky Glaze.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.EaterofWorldsIDs, Do(new ProjectileResistBalancingRule(0.6f, ProjectileType<StickyFeather>()))));

            // 30% resist to Jester Arrows.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.EaterofWorldsIDs, Do(new ProjectileResistBalancingRule(0.7f, ProjectileID.JestersArrow))));
            #endregion

            #region The Perforators
            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.PerforatorIDs, Do(ResistTrueMelee(0.5f))));
            #endregion

            #region Aquatic Scourge
            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AquaticScourgeIDs, Do(ResistTrueMelee(0.5f))));

            // 50% resist to Dormant Brimseekers.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AquaticScourgeIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<DormantBrimseekerBab>()))));

            // 40% resist to Cryophobia.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AquaticScourgeIDs, Do(new ProjectileResistBalancingRule(0.6f, ProjectileType<CryoBlast>()))));
            #endregion

            #region The Destroyer
            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.DestroyerIDs, Do(ResistTrueMelee(0.5f))));

            // 50% resist to Dormant Brimseekers.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.DestroyerIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<DormantBrimseekerBab>()))));

            // 40% resist to Cryophobia.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.DestroyerIDs, Do(new ProjectileResistBalancingRule(0.6f, ProjectileType<CryoBlast>()))));

            // 15% resist to Snowstorm Staff.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.DestroyerIDs, Do(new ProjectileResistBalancingRule(0.85f, ProjectileType<Snowflake>()))));
            #endregion

            #region Astrum Aureus
            //35% resist to The Ballista's greatarrows.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<AstrumAureus>(), Do(new ProjectileResistBalancingRule(0.65f, ProjectileType<BallistaGreatArrow>()))));
            #endregion

            #region Ravager
            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.RavagerIDs, Do(ResistTrueMelee(0.5f))));

            // 40% resist to Flying Dragon.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.RavagerIDs, Do(new ProjectileResistBalancingRule(0.6f, ProjectileID.DD2SquireSonicBoom))));

            #endregion

            #region Duke Fishron
            // 35% vulnerability to Resurrection Butterfly.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.DukeFishron, Do(new ProjectileResistBalancingRule(1.35f, ProjectileType<SakuraBullet>(), ProjectileType<PurpleButterfly>()))));
            #endregion

            #region Empress of Light
            // 20% resist to Plague Tainted SMG's drones.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.HallowBoss, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<PlagueTaintedDrone>()))));
            #endregion

            #region Lunatic Cultist
            // 25% resist to Resurrection Butterfly.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.CultistBoss, Do(new ProjectileResistBalancingRule(0.75f, ProjectileType<SakuraBullet>(), ProjectileType<PurpleButterfly>()))));

            // 20% resist to Art Attack.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.CultistBoss, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<ArtAttackStrike>()))));

            // 20% resist to Nightglow.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.CultistBoss, Do(new ProjectileResistBalancingRule(0.8f, ProjectileID.FairyQueenMagicItemShot))));
            #endregion

            #region Astrum Deus
            // 75% resist to Plaguenades.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AstrumDeusIDs, Do(new ProjectileResistBalancingRule(0.25f, ProjectileType<PlaguenadeBee>(), ProjectileType<PlaguenadeProj>()))));

            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AstrumDeusIDs, Do(ResistTrueMelee(0.5f))));

            // 25% resist to Resurrection Butterfly.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AstrumDeusIDs, Do(new ProjectileResistBalancingRule(0.75f, ProjectileType<SakuraBullet>(), ProjectileType<PurpleButterfly>()))));
            #endregion

            #region Moon Lord
            // 90% resist to Mercurial Tides (True Biome Blade).
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.MoonLordCore, Do(new ProjectileResistBalancingRule(0.1f, ProjectileType<MercurialTides>(), ProjectileType<MercurialTidesMonolith>(), ProjectileType<MercurialTidesBlast>()))));
            #endregion

            #region Profaned Guardians
            // 50% resist to true melee for the Defense Guardian's rocks.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<ProfanedRocks>(), ResistTrueMelee(0.5f)));
            #endregion

            #region Providence
            // 80% resist to Hell's Sun.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<Providence>(), new ProjectileResistBalancingRule(0.2f, ProjectileType<HellsSunProj>())));
            #endregion

            #region Ceaseless Void: Dark Energies
            // 50% resist to true melee.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<DarkEnergy>(), ResistTrueMelee(0.5f)));
            #endregion

            #region Storm Weaver
            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.StormWeaverIDs, Do(ResistTrueMelee(0.5f))));

            // 50% resist to Dazzling Stabbers.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.StormWeaverIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<DazzlingStabber>()))));

            // 50% resist to Elemental Axes.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.StormWeaverIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<ElementalAxeMinion>()))));

            // 50% resist to Pristine Fury's alt fire.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.StormWeaverIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<PristineSecondary>()))));

            // 50% resist to Tactician's Trump Card explosions.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.StormWeaverIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<TacticiansElectricBoom>()))));

            // 25% resist to Molten Amputator blobs.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.StormWeaverIDs, Do(new ProjectileResistBalancingRule(0.75f, ProjectileType<MoltenBlobThrown>()))));
            #endregion

            #region Old Duke
            // 60% resist to Last Mourning.
            // This will technically catch pumpkins from the Horseman's Blade but that shouldn't really matter.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<OldDuke>(), new ProjectileResistBalancingRule(0.4f, ProjectileType<MourningSkull>(), ProjectileID.FlamingJack)));

            // 20% resist to Time Bolt.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<OldDuke>(), new ProjectileResistBalancingRule(0.8f, ProjectileType<TimeBoltKnife>())));
            #endregion

            #region The Devourer of Gods
            // 15% vulnerability to Time Bolt stealth strikes.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.DevourerOfGodsIDs, Do(new StealthStrikeBalancingRule(1.15f, ProjectileType<TimeBoltKnife>()))));
            #endregion The Devourer of Gods

            #region Yharon
            // 15% resist to Time Bolt.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(new ProjectileResistBalancingRule(0.85f, ProjectileType<TimeBoltKnife>()))));

            // 10% resist to Old Reaper.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(new ProjectileResistBalancingRule(0.9f, ProjectileType<ReaperProjectile>()))));

            #endregion

            #region Exo Mechs: Ares
            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(ResistTrueMelee(0.5f))));

            // 30% resist to Dynamic Pursuer.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(new ProjectileResistBalancingRule(0.7f, ProjectileType<DynamicPursuerProjectile>(), ProjectileType<DynamicPursuerLaser>(), ProjectileType<DynamicPursuerElectricity>()))));

            // 30% resist to the Spin Throw part of the Ark of the Cosmos' combo.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(new ProjectileSpecificRequirementBalancingRule(0.7f, AotCThrowCombo))));

            // 25% resist to Aetherflux Cannon.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(new ProjectileResistBalancingRule(0.75f, ProjectileType<PhasedGodRay>()))));
            
            // 25% resist to Murasama.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(new ProjectileResistBalancingRule(0.75f, ProjectileType<MurasamaSlash>()))));

            // 20% resist to Dragon Rage projectiles.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(new ProjectileSpecificRequirementBalancingRule(0.8f, DragonRageFilter))));

            // 20% resist to Eclipse's Fall.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<EclipsesSmol>(), ProjectileType<EclipsesFallMain>()))));

            // 20% resist to Yharim's Crystal.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<YharimsCrystalBeam>()))));

            // 20% resist to Zenith.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(new ProjectileResistBalancingRule(0.8f, ProjectileID.FinalFractal))));

            // 15% resist to Enforcer projectiles.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(new ProjectileResistBalancingRule(0.85f, ProjectileType<EssenceFlame2>()))));
            #endregion

            #region Exo Mechs: Artemis and Apollo
            // Artemis and Apollo have to be defined here because they aren't a pre-existing list.
            // TODO -- NPC sets (mostly worm bosses) should probably be their own holding class.
            int[] exoTwins = new int[] { NPCType<Artemis>(), NPCType<Apollo>() };

            // 20% resist to Dynamic Pursuer.
            NPCSpecificBalancingChanges.AddRange(Bundle(exoTwins, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<DynamicPursuerProjectile>(), ProjectileType<DynamicPursuerLaser>(), ProjectileType<DynamicPursuerElectricity>()))));

            // 10% resist to Eclipse's Fall stealth strike.
            NPCSpecificBalancingChanges.AddRange(Bundle(exoTwins, Do(new ProjectileResistBalancingRule(0.9f, ProjectileType<EclipsesSmol>()))));

            // 10% resist to Seared Pan.
            NPCSpecificBalancingChanges.AddRange(Bundle(exoTwins, Do(new ProjectileResistBalancingRule(0.9f, ProjectileType<SearedPanProjectile>(), ProjectileType<PanSpark>(), ProjectileType<NiceCock>()))));
            #endregion

            #region Exo Mechs: Thanatos
            // 65% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(ResistTrueMelee(0.35f))));

            // 65% resist to Enforcer projectiles.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.35f, ProjectileType<EssenceFlame2>()))));

            // 65% resist to Dynamic Pursuer. (Do 40% if lightning pierce is removed)
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.35f, ProjectileType<DynamicPursuerProjectile>(), ProjectileType<DynamicPursuerLaser>(), ProjectileType<DynamicPursuerElectricity>()))));

            // 65% resist to Final Dawn lunge.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.35f, ProjectileType<FinalDawnThrow2>()))));

            // 50% resist to Chicken Cannon.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<ChickenExplosion>()))));

            // 50% resist to Tarragon Throwing Darts Thorns. (LOL)
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<TarraThornRight>()))));

            // 50% resist to Vehemence skulls.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<VehemenceSkull>(), ProjectileType<PrismaticBeam>()))));

            // 50% resist to Wrathwing stealth strike.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<WrathwingCinder>()))));

            // 50% resist to Yharim's Crystal.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<YharimsCrystalBeam>()))));

            // 40% resist to Rancor.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.6f, ProjectileType<RancorLaserbeam>()))));

            // 35% resist to Anomaly's Nanogun bomb explosions.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.65f, ProjectileType<AnomalysNanogunMPFBBoom>()))));

            // 30% resist to the Spin Throw part of the Ark of the Cosmos' combo.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileSpecificRequirementBalancingRule(0.7f, AotCThrowCombo))));

            // 30% resist to Eclipse Fall.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.7f, ProjectileType<EclipsesFallMain>()))));

            // 30% resist to Pulse Rifle.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.7f, ProjectileType<PulseRifleShot>()))));
            
            // 30% resist to Sirius.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.7f, ProjectileType<SiriusExplosion>()))));

            // 30% resist to Dragon Scales and The Wand's tornadoes
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.7f, ProjectileType<InfernadoFriendly>(), ProjectileType<DragonScalesInfernado>()))));

            // 30% resist to Prismatic Breaker's laser beam.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.7f, ProjectileType<PrismaticBeam>()))));

            // 25% resist to Dragon Rage projectiles.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileSpecificRequirementBalancingRule(0.75f, DragonRageFilter))));

            // 25% resist to Gael's Greatsword's big skulls.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileSpecificRequirementBalancingRule(0.75f, BigGaelsSkullFilter))));

            // 15% resist to Godslayer Slugs.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.85f, ProjectileType<GodSlayerSlugProj>()))));

            // 15% resist to Luminite Bullets.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.85f, ProjectileID.MoonlordBullet))));

            // 25% resist to Zenith.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.75f, ProjectileID.FinalFractal))));

            // 20% resist to Blood Boiler fire.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<BloodBoilerFire>()))));

            // 20% resist to Eradicator beams.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<NebulaShot>()))));

            // 20% resist to Voltaic Climax / Void Vortex hitscan beams.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<ClimaxBeam>()))));

            // 20% resist to Plasma grenades and Dynamic Pursuer's explosions
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<PlasmaGrenadeProjectile>(),ProjectileType<PlasmaGrenadeSmallExplosion>(), ProjectileType<MassivePlasmaExplosion>()))));

            // 15% resist to Final Dawn AoE sweep flames.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.85f, ProjectileType<FinalDawnFlame>()))));

            // 15% resist to Gruesome Eminence.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.85f, ProjectileType<SpiritCongregation>()))));

            #endregion

            #region Supreme Calamitas: Brimstone Hearts
            // 30% resist to Surge Driver's alt click comets.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<BrimstoneHeart>(), new ProjectileResistBalancingRule(0.7f, ProjectileType<PrismComet>())));

            // 20% resist to Executioner's Blade stealth strikes.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<BrimstoneHeart>(), new ProjectileResistBalancingRule(0.8f, ProjectileType<ExecutionersBladeStealthProj>())));
            #endregion

            #region Supreme Calamitas: Soul Seekers
            // 85% resist to Chicken Cannon.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.15f, ProjectileType<ChickenExplosion>())));

            // 50% resist to Subsuming Vortex.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.5f, ProjectileType<ExoVortex>(), ProjectileType<ExoVortex2>(), ProjectileType<EnormousConsumingVortex>())));

            // 30% resist to Murasama.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.7f, ProjectileType<MurasamaSlash>())));

            // 30% resist to Zenith.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.7f, ProjectileID.FinalFractal)));

            // 25% resist to Yharim's Crystal.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.75f, ProjectileType<YharimsCrystalBeam>())));

            // 10% resist to Surge Driver's alt click comets.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.9f, ProjectileType<PrismComet>())));

            // 10% resist to Executioner's Blade stealth strikes.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.9f, ProjectileType<ExecutionersBladeStealthProj>())));

            // 10% resist to Celestus.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.9f, ProjectileType<CelestusProj>(), ProjectileType<CelestusMiniScythe>())));

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

using System.Collections.Generic;
using System.Linq;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.NormalNPCs.HorribleHog;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
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
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Balancing
{
    public sealed class BalancingChangesManager : ModSystem
    {
        internal static List<NPCBalancingChange> NPCSpecificBalancingChanges = null;

        // Balancing changes in this method are sorted based on place in progression, NPC name (from A-Z), and strength of resistance in ascending order, in that level of priority.
        // For ease of change, changes that are not exclusive to one specific weapon are not bundled into one line if they share the same resistance factor.
        // To give an example of this, Thanatos having a 50% resist to Chicken Cannon and Rancor should be two distinct lines with a 0.5x factor instead of hamfisting them all into
        // one single resist that may have to be split later.
        public override void SetStaticDefaults()
        {
            // Dirty shorthand for true melee resists, because they're super common and other class resists aren't.
            // If you add/change a true melee resist here, be sure to add/change the resist in CalamityGlobalNPC ModifyHitByItem as well.
            IBalancingRule ResistTrueMelee(float f) => new ClassResistBalancingRule(f, TrueMeleeDamageClass.Instance);

            // Declare specific filters.
            bool BigGaelsSkullFilter(Projectile p) =>
                p.type == ProjectileType<GaelSkull>() && p.ai[1] == 1;

            bool DragonRageFilter(Projectile p) =>
                p.type == ProjectileType<DragonRageStaff>() || p.type == ProjectileType<DragonRageFireball>() || (p.type == ProjectileType<FuckYou>() && p.CountsAsClass<MeleeDamageClass>());

            bool AotCThrowCombo(Projectile p) =>
                p.type == ProjectileType<ArkoftheCosmosSwungBlade>() && (p.ai[0] == 2 || p.ai[0] == 3);

            bool HiveBeeFilter(Projectile p) =>
                p.type == ProjectileType<BasicPlagueBee>() && Main.player[p.owner].HeldItem.type == ItemType<TheHive>();

            NPCSpecificBalancingChanges = new List<NPCBalancingChange>();

            // ENEMIES
            #region Horrible Hog
            // Take 300% damage from Finch Staff.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<HorribleHog>(), Do(new ProjectileResistBalancingRule(3f, ProjectileID.BabyBird))));
            #endregion

            // BOSSES
            #region Crabulon
            // 20% resist to true melee.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<Crabulon>(), ResistTrueMelee(0.8f)));
            #endregion

            #region Eater of Worlds
            // 50% resist to Demon Scythe.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.EaterOfWorlds, Do(new ProjectileResistBalancingRule(0.5f, ProjectileID.DemonScythe))));

            // 40% resist to Sky Glaze.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.EaterOfWorlds, Do(new ProjectileResistBalancingRule(0.6f, ProjectileType<StickyFeather>()))));

            // 25% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.EaterOfWorlds, Do(ResistTrueMelee(0.75f))));

            // 40% resist to Lemon 'nade
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.EaterOfWorlds, Do(new ProjectileResistBalancingRule(0.60f, ProjectileType<LemonNadeProjectile>()))));
            #endregion

            #region Brain of Cthulhu: Creepers
            // 50% resist to Demon Scythe.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.Creeper, Do(new ProjectileResistBalancingRule(0.5f, ProjectileID.DemonScythe))));

            // 25% resist to true melee.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.Creeper, Do(ResistTrueMelee(0.75f))));
            #endregion

            #region The Perforators
            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Perforators, Do(ResistTrueMelee(0.5f))));
            #endregion

            #region Wall of Flesh
            // 40% resist to Corro/Crimslime Staff.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.WallofFleshEye, Do(new ProjectileResistBalancingRule(0.6f, ProjectileType<CrimslimeMinion>(), ProjectileType<CorroslimeMinion>()))));
            #endregion

            #region Aquatic Scourge
            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.AquaticScourge, Do(ResistTrueMelee(0.5f))));

            // 50% resist to Adamantite Throwing Axe's lightning.
            // Twisting Thunder and Gael's Greatsword use this projectile too, but they are far beyond progression, so who cares!
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.AquaticScourge, Do(new ProjectileResistBalancingRule(0.5f, ProjectileID.CultistBossLightningOrbArc))));

            // 50% resist to Dormant Brimseeker.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.AquaticScourge, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<DormantBrimseekerBab>()))));

            // 50% resist to Mounted Scanner.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.AquaticScourge, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<MountedScannerLaser>()))));

            // 40% resist to Cryophobia.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.AquaticScourge, Do(new ProjectileResistBalancingRule(0.6f, ProjectileType<CryoBlast>()))));

            // 30% resist to Meowthrower.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.AquaticScourge, Do(new ProjectileResistBalancingRule(0.7f, ProjectileType<MeowFire>()))));

            // 25% resist to Snowstorm Staff.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.AquaticScourge, Do(new ProjectileResistBalancingRule(0.75f, ProjectileType<Snowflake>(), ProjectileType<SnowflakeIceStar>()))));
            #endregion

            #region The Destroyer
            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Destroyer, Do(ResistTrueMelee(0.5f))));

            // 50% resist to Adamantite Throwing Axe's lightning.
            // See Aquatic Scourge comment.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Destroyer, Do(new ProjectileResistBalancingRule(0.5f, ProjectileID.CultistBossLightningOrbArc))));

            // 50% resist to Dormant Brimseeker.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Destroyer, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<DormantBrimseekerBab>()))));

            // 45% resist to Mounted Scanner.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Destroyer, Do(new ProjectileResistBalancingRule(0.55f, ProjectileType<MountedScannerLaser>()))));

            // 30% resist to Meowthrower.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Destroyer, Do(new ProjectileResistBalancingRule(0.7f, ProjectileType<MeowFire>()))));

            // 25% resist to Avalanche's Ice Bombs and shards.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Destroyer, Do(new ProjectileResistBalancingRule(0.75f, ProjectileType<IceBombFriendly>(), ProjectileType<FrostShardFriendly>()))));

            // 25% resist to Snowstorm Staff.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Destroyer, Do(new ProjectileResistBalancingRule(0.75f, ProjectileType<Snowflake>(), ProjectileType<SnowflakeIceStar>()))));
            #endregion

            #region Astrum Aureus
            // 25% resist to true melee.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<AstrumAureus>(), ResistTrueMelee(0.75f)));

            // 25% resist to The Ballista's greatarrows.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<AstrumAureus>(), Do(new ProjectileResistBalancingRule(0.75f, ProjectileType<BallistaGreatArrow>()))));
            #endregion

            #region Duke Fishron
            // 20% vulnerability to The Hive's bees.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.DukeFishron, Do(new ProjectileSpecificRequirementBalancingRule(1.2f, HiveBeeFilter))));

            // 25% vulnerability to Resurrection Butterfly.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.DukeFishron, Do(new ProjectileResistBalancingRule(1.25f, ProjectileType<SakuraBullet>(), ProjectileType<PurpleButterfly>()))));
            #endregion

            #region Empress of Light
            // 25% resist to Abyss Blade's blade.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.HallowBoss, Do(new ProjectileResistBalancingRule(0.75f, ProjectileType<AbyssBladeProjectile>()))));

            // 20% resist to Plague Tainted SMG's drones.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.HallowBoss, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<PlagueTaintedDrone>()))));
            #endregion

            #region Ravager
            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Ravager, Do(ResistTrueMelee(0.5f))));

            // 50% resist to Aegis Blade's explosions because the true melee resist ain't enough.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Ravager, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<AegisBlast>()))));

            // 35% resist to Flying Dragon.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Ravager, Do(new ProjectileResistBalancingRule(0.65f, ProjectileID.DD2SquireSonicBoom))));

            // 20% resist to Aurora Blazer.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Ravager, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<AuroraFire>()))));

            // 20% resist to The Hive.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Ravager, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<HiveNuke>(), ProjectileType<HiveMissile>()))));
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
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.AstrumDeus, Do(new ProjectileResistBalancingRule(0.25f, ProjectileType<PlaguenadeBee>(), ProjectileType<PlaguenadeProj>()))));

            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.AstrumDeus, Do(ResistTrueMelee(0.5f))));

            // 50% resist to Atlantis (Atlantis gaming!).
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.AstrumDeus, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<AtlantisSpear>()))));

            // 50% resist to Charged Blaster Cannon.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.AstrumDeus, Do(new ProjectileResistBalancingRule(0.5f, ProjectileID.ChargedBlasterLaser))));

            //45% resist to Duststorm in a Bottle
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.AstrumDeus, Do(new ProjectileResistBalancingRule(0.55f, ProjectileType<DuststormCloud>(), ProjectileType<DuststormCloudExplosion>(), ProjectileType<DuststormInABottleProj>()))));

            // 40% resist to Stardust Dragon Staff.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.AstrumDeus, Do(new ProjectileResistBalancingRule(0.6f, ProjectileID.StardustDragon1, ProjectileID.StardustDragon2, ProjectileID.StardustDragon3, ProjectileID.StardustDragon4))));

            // 35% resist to Aerial Bane.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.AstrumDeus, Do(new ProjectileResistBalancingRule(0.65f, ProjectileID.DD2BetsyArrow))));

            // 30% resist to Aurora Blazer.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.AstrumDeus, Do(new ProjectileResistBalancingRule(0.7f, ProjectileType<AuroraFire>()))));

            // 30% resist to Flak Kraken.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.AstrumDeus, Do(new ProjectileResistBalancingRule(0.7f, ProjectileType<FlakKrakenProjectile>()))));

            // 25% resist to Solar Eruption.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.AstrumDeus, Do(new ProjectileResistBalancingRule(0.75f, ProjectileID.SolarWhipSword, ProjectileID.SolarWhipSwordExplosion))));

            // 20% resist to Ballistic Poison Bomb's clouds.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.AstrumDeus, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<BallisticPoisonCloud>()))));

            // 20% resist to Cluster Rocket fragments.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.AstrumDeus, Do(new ProjectileResistBalancingRule(0.8f, ProjectileID.ClusterFragmentsI, ProjectileID.ClusterFragmentsII))));
            #endregion

            #region Moon Lord
            // 20% resist to The Hive's nuke.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.MoonLordCore, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<HiveNuke>()))));
            #endregion

            #region Profaned Guardians
            // 50% resist to true melee for the Defense Guardian's rocks.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<ProfanedRocks>(), ResistTrueMelee(0.5f)));
            #endregion

            #region Providence
            // 35% resist to Vanishing Point. This thing desperately needs a rework.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<Providence>(), new ProjectileResistBalancingRule(0.65f, ProjectileType<SpatialSpear>(), ProjectileType<SpatialSpear2>(), ProjectileType<SpatialSpear3>(), ProjectileType<SpatialSpear4>())));
            #endregion

            #region Ceaseless Void: Dark Energies
            // 50% resist to true melee.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<DarkEnergy>(), ResistTrueMelee(0.5f)));

            // 50% resist to Galactus Blade.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<DarkEnergy>(), new ProjectileResistBalancingRule(0.5f, ProjectileType<GalacticaComet>())));

            // 40% resist to Stellar Torus Staff.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<DarkEnergy>(), new ProjectileResistBalancingRule(0.6f, ProjectileType<StellarTorusBeam>())));
            #endregion

            #region Storm Weaver
            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.StormWeaver, Do(ResistTrueMelee(0.5f))));

            // 50% resist to Dazzling Stabber Staff.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.StormWeaver, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<DazzlingStabber>()))));

            // 50% resist to Last Prism.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.StormWeaver, Do(new ProjectileResistBalancingRule(0.5f, ProjectileID.LastPrismLaser))));

            // 50% resist to Legion of Celestia.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.StormWeaver, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<CelestialAxeMinion>()))));

            // 50% resist to Tactician's Trump Card's explosions.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.StormWeaver, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<TacticiansElectricBoom>()))));

            // 40% resist to Devil's Sunrise's slashes.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.StormWeaver, Do(new ProjectileResistBalancingRule(0.6f, ProjectileType<DevilsSunriseProj>()))));

            // 35% resist to Event Horizon.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.StormWeaver, Do(new ProjectileResistBalancingRule(0.65f, ProjectileType<EventHorizonStar>(), ProjectileType<EventHorizonBlackhole>()))));
            #endregion

            #region Old Duke
            // 20% resist to Time Bolt.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<OldDuke>(), new ProjectileResistBalancingRule(0.8f, ProjectileType<TimeBoltKnife>())));
            #endregion

            #region The Devourer of Gods
            // 65% resist to Wave Pounder.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.DevourerOfGods, Do(new ProjectileResistBalancingRule(0.35f, ProjectileType<WavePounderBoom>()))));

            // 60% resist to Corinth Prime's grenades.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.DevourerOfGods, Do(new ProjectileResistBalancingRule(0.4f, ProjectileType<CorinthPrimeAirburst>()))));
            
            // 35% resist to Sulphuric Acid Cannon's explosions.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.DevourerOfGods, Do(new ProjectileResistBalancingRule(0.65f, ProjectileType<SulphuricAcidCannonExplosion>()))));

            // 20% resist to Valediction's typhoons; will catch Nuclear Fury as well but that doesn't matter.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.DevourerOfGods, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<NuclearFuryProjectile>()))));

            // 15% vulnerability to Time Bolt stealth strikes.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.DevourerOfGods, Do(new StealthStrikeBalancingRule(1.15f, ProjectileType<TimeBoltKnife>()))));
            #endregion The Devourer of Gods

            #region Yharon
            // 15% resist to The Old Reaper's stealth strike rain.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<Yharon>(), Do(new ProjectileResistBalancingRule(0.85f, ProjectileType<RadiationRain>()))));

            // 15% resist to Time Bolt.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<Yharon>(), Do(new ProjectileResistBalancingRule(0.85f, ProjectileType<TimeBoltKnife>()))));
            #endregion

            #region Exo Mechs: Ares
            // 35% resist to Devil's Devastation's slash.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Ares, Do(new ProjectileResistBalancingRule(0.65f, ProjectileType<DevilsDevastationHoldout>()))));

            // 30% resist to Dynamic Pursuer.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Ares, Do(new ProjectileResistBalancingRule(0.7f, ProjectileType<DynamicPursuerProjectile>(), ProjectileType<DynamicPursuerLaser>(), ProjectileType<DynamicPursuerElectricity>()))));

            // 25% resist to Aetherflux Cannon.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Ares, Do(new ProjectileResistBalancingRule(0.75f, ProjectileType<PhasedGodRay>()))));

            // 20% resist to the Spin Throw part of the Ark of the Cosmos' combo.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Ares, Do(new ProjectileSpecificRequirementBalancingRule(0.8f, AotCThrowCombo))));

            // 20% resist to Dragon Rage projectiles.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Ares, Do(new ProjectileSpecificRequirementBalancingRule(0.8f, DragonRageFilter))));

            // 20% resist to Rancor.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Ares, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<RancorLaserbeam>()))));

            // 20% resist to Yharim's Crystal.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Ares, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<YharimsCrystalBeam>()))));

            // 20% resist to Zenith.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Ares, Do(new ProjectileResistBalancingRule(0.8f, ProjectileID.FinalFractal))));
            #endregion

            #region Exo Mechs: Artemis and Apollo
            // Artemis and Apollo have to be defined here because they aren't a pre-existing list.
            // TODO -- NPC sets (mostly worm bosses) should probably be their own holding class.
            // Commented out since they currently have no resists.
            // int[] exoTwins = new int[] { NPCType<Artemis>(), NPCType<Apollo>() };
            #endregion

            #region Exo Mechs: Thanatos
            // 65% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Thanatos, Do(ResistTrueMelee(0.35f))));

            // 65% resist to The Final Dawn's lunge.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Thanatos, Do(new ProjectileResistBalancingRule(0.35f, ProjectileType<FinalDawnThrow2>()))));

            // 65% resist to Stellar Torus Staff.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Thanatos, Do(new ProjectileResistBalancingRule(0.35f, ProjectileType<StellarTorusBeam>()))));

            // 60% resist to Dynamic Pursuer.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Thanatos, Do(new ProjectileResistBalancingRule(0.4f, ProjectileType<DynamicPursuerProjectile>(), ProjectileType<DynamicPursuerLaser>(), ProjectileType<DynamicPursuerElectricity>()))));

            // 50% resist to Chicken Cannon.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Thanatos, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<ChickenExplosion>()))));

            // 50% resist to Rancor.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Thanatos, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<RancorLaserbeam>()))));

            // 50% resist to Vehemence's skulls.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Thanatos, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<VehemenceSkull>()))));

            // 50% resist to Yharim's Crystal.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Thanatos, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<YharimsCrystalBeam>()))));

            // 45% resist to Wrathwing's stealth strike fireballs.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Thanatos, Do(new ProjectileResistBalancingRule(0.55f, ProjectileType<WrathwingCinder>()))));

            // 40% resist to Omicron's beam.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Thanatos, Do(new ProjectileResistBalancingRule(0.6f, ProjectileType<OmicronBeam>()))));

            // 35% resist to Prismatic Breaker's deathray.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Thanatos, Do(new ProjectileResistBalancingRule(0.65f, ProjectileType<PrismaticRay>()))));

            // 30% resist to Dragon Scales and The Wand's tornadoes.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Thanatos, Do(new ProjectileResistBalancingRule(0.7f, ProjectileType<InfernadoFriendly>(), ProjectileType<DragonScalesInfernado>()))));

            // 25% resist to the Spin Throw part of the Ark of the Cosmos' combo.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Thanatos, Do(new ProjectileSpecificRequirementBalancingRule(0.75f, AotCThrowCombo))));

            // 25% resist to Dragon Rage projectiles.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Thanatos, Do(new ProjectileSpecificRequirementBalancingRule(0.75f, DragonRageFilter))));

            // 25% resist to Gael's Greatsword's big skulls.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Thanatos, Do(new ProjectileSpecificRequirementBalancingRule(0.75f, BigGaelsSkullFilter))));

            // 25% resist to Zenith.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Thanatos, Do(new ProjectileResistBalancingRule(0.75f, ProjectileID.FinalFractal))));

            // 20% resist to Ariane's aura (Lilies of Finality).
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Thanatos, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<LiliesOfFinalityAoE>()))));

            // 20% resist to Mirror of Kalandra's Paradoxica minion.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Thanatos, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<Paradoxica>()))));

            // 20% resist to Voltaic Climax / Void Vortex hitscan beams.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Thanatos, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<ClimaxBeam>()))));

            // 15% resist to The Final Dawn's AoE sweep flames.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Thanatos, Do(new ProjectileResistBalancingRule(0.85f, ProjectileType<FinalDawnFlame>()))));

            // 15% resist to Gruesome Eminence.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Thanatos, Do(new ProjectileResistBalancingRule(0.85f, ProjectileType<SpiritCongregation>()))));

            // 15% resist to Ultima's rays.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityNPCTypeSets.Thanatos, Do(new ProjectileResistBalancingRule(0.85f, ProjectileType<UltimaRay>()))));
            #endregion

            #region Supreme Calamitas: Brimstone Hearts
            // 34% resist to Celestus' mini scythes.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<BrimstoneHeart>(), new ProjectileResistBalancingRule(0.66f, ProjectileType<CelestusMiniScythe>())));

            // 30% resist to Supernova.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<BrimstoneHeart>(), new ProjectileResistBalancingRule(0.7f, ProjectileType<SupernovaBoom>(), ProjectileType<SupernovaStealthBoom>())));

            // 30% resist to Surge Driver's alt click comets.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<BrimstoneHeart>(), new ProjectileResistBalancingRule(0.7f, ProjectileType<PrismComet>())));
            #endregion

            #region Supreme Calamitas: Soul Seekers
            // 80% resist to Chicken Cannon.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.2f, ProjectileType<ChickenExplosion>())));

            // 40% resist to Subsuming Vortex.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.6f, ProjectileType<ExoVortex>(), ProjectileType<ExoVortex2>(), ProjectileType<EnormousConsumingVortex>())));

            // 40% resist to Supernova.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.6f, ProjectileType<SupernovaBoom>(), ProjectileType<SupernovaStealthBoom>())));

            // 30% resist to Zenith.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.7f, ProjectileID.FinalFractal)));

            // 25% resist to Yharim's Crystal.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.75f, ProjectileType<YharimsCrystalBeam>())));
            #endregion
        }

        public override void Unload()
        {
            NPCSpecificBalancingChanges = null;
        }

        public static void ApplyFromProjectile(NPC npc, ref NPC.HitModifiers modifiers, Projectile proj)
        {
            // Apply NPC-specific balancing rules.
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

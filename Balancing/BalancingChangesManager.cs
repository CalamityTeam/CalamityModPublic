using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using System.Collections.Generic;
using System.Linq;
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
            // Declare specific filters.
            bool DragonRageFilter(Projectile p) =>
                p.type == ProjectileType<DragonRageStaff>() || p.type == ProjectileType<DragonRageFireball>() || (p.type == ProjectileType<FuckYou>() && p.CountsAsClass<MeleeDamageClass>());

            bool MonkStaffT3Filter(Projectile p) =>
                p.type == ProjectileID.MonkStaffT3_AltShot || (p.type == ProjectileID.Electrosphere && Main.player[p.owner].ActiveItem().type == ItemID.MonkStaffT3);

            bool MushroomSpearFilter(Projectile p) =>
                p.type == ProjectileID.Mushroom && Main.player[p.owner].ActiveItem().type == ItemID.MushroomSpear;

            bool TrueNightsEdgeFilter(Projectile p) =>
                p.type == ProjectileID.NightBeam && Main.player[p.owner].ActiveItem().type == ItemID.TrueNightsEdge;

            bool TerraBladeFilter(Projectile p) =>
                p.type == ProjectileID.TerraBeam && Main.player[p.owner].ActiveItem().type == ItemID.TerraBlade;

            bool SpectreMaskSetBonusFilter(Projectile p) =>
                p.type == ProjectileID.SpectreWrath && Main.player[p.owner].ghostHurt;

            bool AotCThrowCombo(Projectile p) =>
                p.type == ProjectileType<ArkoftheCosmosSwungBlade>() && (p.ai[0] == 2 || p.ai[0] == 3);

            UniversalBalancingChanges = new List<IBalancingRule[]>()
            {
                // Nerf Sky Dragon's Fury's alt fire projectiles due to its strong primary fire buff.
                // TODO -- Vanilla 1.4 already nerfs the projectiles to 0.5x, so this nerf is disabled for now.
                // The multiplier can be changed as needed.
                // Do(new ProjectileSpecificRequirementBalancingRule(0.25f, MonkStaffT3Filter)),

                // Nerf Seedler seeds by 66.6%.
                Do(new ProjectileResistBalancingRule(1f / 3f, ProjectileID.SeedlerNut, ProjectileID.SeedlerThorn)),

                // Nerf Cursed Dart flames by 50%.
                Do(new ProjectileResistBalancingRule(0.5f, ProjectileID.CursedDartFlame)),

                // Nerf True Night's Edge projectiles by 33% to cancel out the 1.5x multiplier it gets.
                Do(new ProjectileSpecificRequirementBalancingRule(1f / 3f * 2f, TrueNightsEdgeFilter)),

                // Nerf Terra Blade projectiles by 33% to cancel out the 1.5x multiplier it gets.
                Do(new ProjectileSpecificRequirementBalancingRule(1f / 3f * 2f, TerraBladeFilter)),

                // Nerf Mushroom Spear projectiles by 50%.
                Do(new ProjectileSpecificRequirementBalancingRule(0.5f, MushroomSpearFilter)),

                // Nerf Spectre Mask set bonus projectiles by 30%.
                Do(new ProjectileSpecificRequirementBalancingRule(0.7f, SpectreMaskSetBonusFilter)),
            };

            NPCSpecificBalancingChanges = new List<NPCBalancingChange>();

            #region Desert Scourge

            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.DesertScourgeIDs, Do(new TrueMeleeResistBalancingRule(0.5f))));

            #endregion Desert Scourge

            #region Crabulon

            // 50% resist to true melee.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<Crabulon>(), new TrueMeleeResistBalancingRule(0.5f)));

            #endregion

            #region Brain of Cthulhu / Creepers

            // 50% resist to true melee.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.Creeper, Do(new TrueMeleeResistBalancingRule(0.5f))));

            // 50% resist to Demon Scythes.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.Creeper, Do(new ProjectileResistBalancingRule(0.5f, ProjectileID.DemonScythe))));

            #endregion Brain of Cthulhu / Creepers

            #region Eater of Worlds

            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.EaterofWorldsIDs, Do(new TrueMeleeResistBalancingRule(0.5f))));

            // 50% resist to Demon Scythes.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.EaterofWorldsIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileID.DemonScythe))));

            // 40% resist to Sky Glaze.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.EaterofWorldsIDs, Do(new ProjectileResistBalancingRule(0.6f, ProjectileType<StickyFeather>()))));

            // 30% resist to Jester Arrows
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.EaterofWorldsIDs, Do(new ProjectileResistBalancingRule(0.7f, ProjectileID.JestersArrow))));

            #endregion Eater of Worlds

            #region The Perforators

            // 50% resist to true melee
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.PerforatorIDs, Do(new TrueMeleeResistBalancingRule(0.5f))));

            #endregion The Perforators

            #region The Aquatic Scourge

            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AquaticScourgeIDs, Do(new TrueMeleeResistBalancingRule(0.5f))));

            // 50% resist to Dormant Brimseekers.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AquaticScourgeIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<DormantBrimseekerBab>()))));

            // 40% resist to Cryophobia
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AquaticScourgeIDs, Do(new ProjectileResistBalancingRule(0.6f, ProjectileType<CryoBlast>()))));

            #endregion The Aquatic Scourge

            #region The Destroyer

            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.DestroyerIDs, Do(new TrueMeleeResistBalancingRule(0.5f))));

            // 50% resist to Dormant Brimseekers.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.DestroyerIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<DormantBrimseekerBab>()))));

            // 40% resist to Cryophobia
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AquaticScourgeIDs, Do(new ProjectileResistBalancingRule(0.6f, ProjectileType<CryoBlast>()))));

            // 15% resust to Snowstorm Staff
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.DestroyerIDs, Do(new ProjectileResistBalancingRule(0.85f, ProjectileType<Snowflake>()))));

            #endregion The Destroyer

            #region Ravager

            // 50% resist to true melee.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<RavagerBody>(), new TrueMeleeResistBalancingRule(0.5f)));

            #endregion Ravager

            #region Duke Fishron

            // 35% vulnerability to Resurrection Butterfly.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.CultistBoss, Do(new ProjectileResistBalancingRule(1.35f, ProjectileType<SakuraBullet>(), ProjectileType<PurpleButterfly>()))));

            #endregion Duke Fishron

            #region Cultist

            // 25% resist to Resurrection Butterfly.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.CultistBoss, Do(new ProjectileResistBalancingRule(0.75f, ProjectileType<SakuraBullet>(), ProjectileType<PurpleButterfly>()))));

            #endregion Cultist

            #region Astrum Deus

            // 75% resist to Plaguenades.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AstrumDeusIDs, Do(new ProjectileResistBalancingRule(0.25f, ProjectileType<PlaguenadeBee>(), ProjectileType<PlaguenadeProj>()))));

            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AstrumDeusIDs, Do(new TrueMeleeResistBalancingRule(0.5f))));

            // 25% resist to Resurrection Butterfly.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AstrumDeusIDs, Do(new ProjectileResistBalancingRule(0.75f, ProjectileType<SakuraBullet>(), ProjectileType<PurpleButterfly>()))));

            #endregion Astrum Deus

            #region Moon Lord
            // 30% resist to the shockwave attunement from the True biome blade.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCID.MoonLordCore, Do(new ProjectileResistBalancingRule(0.1f, ProjectileType<MercurialTides>(), ProjectileType<MercurialTidesMonolith>(), ProjectileType<MercurialTidesBlast>()))));
            #endregion

            #region Ceaseless Void / Dark Energies

            // 50% resist to true melee.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<DarkEnergy>(), new TrueMeleeResistBalancingRule(0.5f)));

            #endregion Ceaseless Void / Dark Energies

            #region Storm Weaver

            // 50% resist to true melee
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.StormWeaverIDs, Do(new TrueMeleeResistBalancingRule(0.5f))));

            // 50% resist to Dazzling Stabbers.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.StormWeaverIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<DazzlingStabber>()))));

            // 50% resist to Elemental Axes.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.StormWeaverIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<ElementalAxeMinion>()))));

            // 50% resist to Pristine Fury.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.StormWeaverIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<PristineFire>()))));

            // 50% resist to Tactician's Trump Card explosions.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.StormWeaverIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<TacticiansElectricBoom>()))));

            // 25% resist to Molten Amputator blobs.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.StormWeaverIDs, Do(new ProjectileResistBalancingRule(0.75f, ProjectileType<MoltenBlobThrown>()))));

            #endregion Storm Weaver

            #region Old Duke

            // 60% resist to Last Mourning.
            // This will technically catch pumpkins from the Horseman's Blade but that shouldn't really matter.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<OldDuke>(), new ProjectileResistBalancingRule(0.4f, ProjectileType<MourningSkull>(), ProjectileID.FlamingJack)));

            // 20% resist to Time Bolt.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<OldDuke>(), new ProjectileResistBalancingRule(0.8f, ProjectileType<TimeBoltKnife>())));

            #endregion Old Duke

            #region The Devourer of Gods

            // 15% vulnerability to Time Bolt stealth strikes.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.DevourerOfGodsIDs, Do(new StealthStrikeBalancingRule(1.15f, ProjectileType<TimeBoltKnife>()))));

            #endregion The Devourer of Gods

            #region Ares

            // 75% resist to Dragon Rage projectiles.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(new ProjectileSpecificRequirementBalancingRule(0.25f, DragonRageFilter))));

            // 60% resist to Murasama.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(new ProjectileResistBalancingRule(0.4f, ProjectileType<MurasamaSlash>()))));

            // 50% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(new TrueMeleeResistBalancingRule(0.5f))));

            // 50% resist to Enforcer projectiles.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<EssenceFlame2>()))));

            // 30% resist to the Spin Throw part of the Ark of the Cosmos' combo
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(new ProjectileSpecificRequirementBalancingRule(0.7f, AotCThrowCombo))));

            // 20% resist to Eclipse's Fall stealth strike.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.AresIDs, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<EclipsesSmol>()))));

            #endregion Ares

            #region Artemis and Apollo

            int[] exoTwins = new int[] { NPCType<Artemis>(), NPCType<Apollo>() };

            // 10% resist to Eclipse's Fall stealth strike.
            NPCSpecificBalancingChanges.AddRange(Bundle(exoTwins, Do(new ProjectileResistBalancingRule(0.9f, ProjectileType<EclipsesSmol>()))));

            #endregion Artemis and Apollo

            #region Thanatos

            // 95% resist to Seraphim lasers (what the actual fuck?).
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.05f, ProjectileType<SeraphimBeamLarge>()))));

            // 75% resist to Celestus.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.25f, ProjectileType<CelestusBoomerang>(), ProjectileType<Celestus2>()))));

            // 65% resist to true melee.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new TrueMeleeResistBalancingRule(0.35f))));

            // 50% resist to Chicken Cannon.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<ChickenExplosion>()))));

            // 50% resist to Prismatic Breaker. (why is this more than BOTH Rancor and Yharim's Crystal?)
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<PrismaticBeam>()))));

            // 50% resist to Vehemence skulls.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.5f, ProjectileType<VehemenceSkull>(), ProjectileType<PrismaticBeam>()))));

            // 40% resist to Rancor.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.6f, ProjectileType<RancorLaserbeam>()))));

            // 40% resist to Wrathwing stealth strike.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.6f, ProjectileType<WrathwingCinder>()))));

            // 40% resist to Yharim's Crystal.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.6f, ProjectileType<YharimsCrystalBeam>()))));

            // 30% resist to the Spin Throw part of the Ark of the Cosmos' combo
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileSpecificRequirementBalancingRule(0.7f, AotCThrowCombo))));

            // 30% resist to Sirius.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.7f, ProjectileType<SiriusExplosion>()))));

            // 25% resist to Godslayer Slugs.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.75f, ProjectileType<GodSlayerSlugProj>()))));

            // 25% resist to Luminite Bullets.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.75f, ProjectileID.MoonlordBullet))));

            // 20% resist to Eradicator beams.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<NebulaShot>()))));

            // 20% resist to Voltaic Climax / Void Vortex hitscan beams.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<ClimaxBeam>()))));

            // 20% resist to Blood Boiler fire.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.8f, ProjectileType<BloodBoilerFire>()))));

            // 15% resist to Gruesome Eminence.
            NPCSpecificBalancingChanges.AddRange(Bundle(CalamityLists.ThanatosIDs, Do(new ProjectileResistBalancingRule(0.85f, ProjectileType<SpiritCongregation>()))));

            #endregion Thanatos

            #region Supreme Calamitas / Brimstone Heart

            // 30% resist to Surge Driver's alt click comets.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<BrimstoneHeart>(), new ProjectileResistBalancingRule(0.7f, ProjectileType<PrismComet>())));

            // 20% resist to Executioner's Blade stealth strikes.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<BrimstoneHeart>(), new ProjectileResistBalancingRule(0.8f, ProjectileType<ExecutionersBladeStealthProj>())));

            // 15% resist to Seraphim lasers.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<BrimstoneHeart>(), new ProjectileResistBalancingRule(0.85f, ProjectileType<SeraphimBeamLarge>())));

            #endregion

            #region Supreme Calamitas / Soul Seekers

            // 85% resist to Chicken Cannon.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.15f, ProjectileType<ChickenExplosion>())));

            // 30% resist to Murasama.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.7f, ProjectileType<MurasamaSlash>())));

            // 25% resist to Yharim's Crystal.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.75f, ProjectileType<YharimsCrystalBeam>())));

            // 10% resist to Surge Driver's alt click comets.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.9f, ProjectileType<PrismComet>())));

            // 10% resist to Executioner's Blade stealth strikes.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.9f, ProjectileType<ExecutionersBladeStealthProj>())));

            // 10% resist to Celestus.
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(0.9f, ProjectileType<CelestusBoomerang>(), ProjectileType<Celestus2>())));

            // 15% vulnerability to The Atom Splitter (Yes, this is kinda weird, but it's what Piky asked for).
            NPCSpecificBalancingChanges.Add(new NPCBalancingChange(NPCType<SoulSeekerSupreme>(), new ProjectileResistBalancingRule(1.15f, ProjectileType<TheAtomSplitterProjectile>(), ProjectileType<TheAtomSplitterDuplicate>())));

            #endregion Supreme Calamitas / Soul Seekers
        }

        internal static void Unload()
        {
            UniversalBalancingChanges = null;
            NPCSpecificBalancingChanges = null;
        }

        public static void ApplyFromProjectile(NPC npc, ref int damage, Projectile proj)
        {
            NPCHitContext hitContext = NPCHitContext.ConstructFromProjectile(proj);

            // Apply universal balancing rules.
            foreach (IBalancingRule[] balancingRules in UniversalBalancingChanges)
            {
                foreach (IBalancingRule balancingRule in balancingRules)
                {
                    if (balancingRule.AppliesTo(npc, hitContext))
                        balancingRule.ApplyBalancingChange(npc, ref damage);
                }
            }

            // As well as rules specific to NPCs.
            foreach (NPCBalancingChange balanceChange in NPCSpecificBalancingChanges)
            {
                if (npc.type != balanceChange.NPCType)
                    continue;

                foreach (IBalancingRule balancingRule in balanceChange.BalancingRules)
                {
                    if (balancingRule.AppliesTo(npc, hitContext))
                        balancingRule.ApplyBalancingChange(npc, ref damage);
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

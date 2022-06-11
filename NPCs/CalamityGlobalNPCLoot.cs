using System;
using System.Threading;
using CalamityMod.Events;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Tiles.Ores;
using CalamityMod.World;
using CalamityMod.World.Planets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.NPCs.AcidRain;

namespace CalamityMod.NPCs
{
    public partial class CalamityGlobalNPC : GlobalNPC
    {
        #region Modify NPC Loot Main Hook
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            // Internal function to determine whether this NPC is the second Twin killed in a fight, regardless of which Twin it is.
            bool IsLastTwinStanding(DropAttemptInfo info)
            {
                NPC npc = info.npc;
                if (npc is null)
                    return false;
                if (npc.type == NPCID.Retinazer)
                    return !NPC.AnyNPCs(NPCID.Spazmatism);
                else if (npc.type == NPCID.Spazmatism)
                    return !NPC.AnyNPCs(NPCID.Retinazer);
                return false;
            }

            // Internal function to determine whether this NPC should drop the Mechanical Bosses combined lore item
            // Drops on the first mech boss killed (so the 2nd twin, Destroyer, or Skeletron Prime)
            bool ShouldDropMechLore(DropAttemptInfo info)
            {
                NPC npc = info.npc;
                if (npc is null)
                    return false;
                bool lastTwinStanding = IsLastTwinStanding(info);
                return !NPC.downedMechBossAny && (lastTwinStanding || npc.type == NPCID.TheDestroyer || npc.type == NPCID.SkeletronPrime);
            }

            switch (npc.type)
            {
                #region Surface
                // Goldfish, Walking Goldfish
                // Pineapple Pet @ 0.2%
                case NPCID.Goldfish:
                case NPCID.GoldfishWalker:
                    npcLoot.Add(ModContent.ItemType<PineapplePet>(), 500);
                    break;

                // Possessed Armor
                // Psychotic Amulet @ 2.5% Normal, 5% Expert+
                case NPCID.PossessedArmor:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<PsychoticAmulet>(), 40, 20));
                    break;

                // Werewolf
                // Moon Charm @ 5% INSTEAD OF 1.67%
                case NPCID.Werewolf:
                    npcLoot.ChangeDropRate(ItemID.MoonCharm, 1, 20);
                    break;
                #endregion

                #region Sky / Space
                // Harpy
                // Sky Glaze @ 3.33% IF Eye of Cthulhu dead
                // Essence of Cinder @ 50% IF Hardmode and not statue spawned
                case NPCID.Harpy:
                    npcLoot.AddIf(() => NPC.downedBoss1, ModContent.ItemType<SkyGlaze>(), 30);
                    npcLoot.AddIf(() => Main.hardMode && !npc.SpawnedFromStatue, ModContent.ItemType<EssenceofSunlight>(), 2);
                    break;

                // Angry Nimbus
                // Essence of Cinder @ 50% Normal, 100% Expert+
                case NPCID.AngryNimbus:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<EssenceofSunlight>(), 2, 1));
                    break;

                // Wyvern Head
                // 1-2 Essence of Cinder @ 100%
                case NPCID.WyvernHead:
                    npcLoot.Add(ModContent.ItemType<EssenceofSunlight>(), 1, 1, 2);
                    break;
                #endregion

                #region Underground
                // Giant Shelly
                // Giant Shell @ 14.29% Normal, 25% Expert+
                case NPCID.GiantShelly:
                case NPCID.GiantShelly2:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<GiantShell>(), 7, 4));
                    break;

                // Crawdad
                // Craw Carapace @ 14.29% Normal, 25% Expert+
                case NPCID.Crawdad:
                case NPCID.Crawdad2:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<CrawCarapace>(), 7, 4));
                    break;

                // Anomura Fungus
                // Fungal Carapace @ 14.29% Normal, 25% Expert+
                case NPCID.AnomuraFungus:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<FungalCarapace>(), 7, 4));
                    break;

                // Tim
                // Plasma Rod @ 33.33% Normal, 50% Expert+
                case NPCID.Tim:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<PlasmaRod>(), 3, 2));
                    break;

                // Mimic
                // Drops all of its items Calamity Style @ 33.33% each
                // This requires erasing its vanilla behavior.
                case NPCID.Mimic:
                    try
                    {
                        // Remove the vanilla loot rule which controls all Mimic drops.
                        var mimicLootRules = npcLoot.Get(false);
                        mimicLootRules.RemoveAll((rule) => rule is OneFromOptionsDropRule mimicItems && mimicItems.dropIds[0] == ItemID.DualHook);

                        var mimicItems = new int[]
                        {
                            ItemID.MagicDagger,
                            ItemID.CrossNecklace,
                            ItemID.PhilosophersStone,
                            ItemID.StarCloak,
                            ItemID.TitanGlove,
                            ItemID.DualHook
                        };

                        // Mimics will not drop any items if spawned from statues.
                        var notStatue = npcLoot.DefineConditionalDropSet(new Conditions.NotFromStatue());
                        notStatue.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, mimicItems));
                    }
                    catch (ArgumentNullException) { }
                    break;
                #endregion

                #region Desert
                // Vulture
                // 1-2 Desert Feather @ 100%
                case NPCID.Vulture:
                    npcLoot.Add(ModContent.ItemType<DesertFeather>(), 1, 1, 2);
                    break;

                // Antlion, Antlion Charger, Giant Antlion Charger, Antlion Swarmer, Giant Antlion Swarmer
                // Mandible Claws @ 2% Normal, 3.33% Expert+
                // Mandible Bow @ 2% Normal, 3.33% Expert+
                case NPCID.Antlion:
                case NPCID.WalkingAntlion:
                case NPCID.GiantWalkingAntlion:
                case NPCID.FlyingAntlion:
                case NPCID.GiantFlyingAntlion:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<MandibleClaws>(), 50, 30));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<MandibleBow>(), 50, 30));
                    break;

                // Tomb Crawler
                // Burnt Sienna @ 4% Normal, 6.67% Expert+
                case NPCID.TombCrawlerHead:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<BurntSienna>(), 25, 15));
                    break;

                // Dune Splicer
                // Terra-cotta @ 6.67%
                case NPCID.DuneSplicerHead:
                    npcLoot.AddIf(() => NPC.downedPlantBoss, ModContent.ItemType<Terracotta>(), 15);
                    break;

                // Sand Elemental
                // Elemental in a Bottle @ 20% Normal, 33.33% Expert+
                // Rare Elemental in a Bottle @ 10% Normal, 16.67% Expert+
                case NPCID.SandElemental:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<WifeinaBottle>(), 5, 3));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<WifeinaBottlewithBoobs>(), 10, 6));
                    break;
                #endregion

                #region Ice
                // Icy Merman, Icy Tortoise, Ice Elemental
                // Essence of Eleum @ 50% Normal, 100% Expert+
                case NPCID.IcyMerman:
                case NPCID.IceTortoise:
                case NPCID.IceElemental:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<EssenceofEleum>(), 2, 1));
                    break;

                // Ice Mimic
                // Drops all of its items Calamity Style @ 33.33% each
                // This requires erasing its bizarre vanilla behavior involving the Toy Sled.
                case NPCID.IceMimic:
                    try
                    {
                        // Remove the vanilla loot rule which controls all Ice Mimic drops.
                        var iceMimicRootRules = npcLoot.Get(false);
                        iceMimicRootRules.RemoveAll((rule) => rule is CommonDrop sledDrop && sledDrop.itemId == ItemID.ToySled);

                        var iceMimicItems = new int[]
                        {
                            ItemID.Frostbrand,
                            ItemID.IceBow,
                            ItemID.FlowerofFrost,
                            ItemID.ToySled,
                        };

                        // Ice Mimics will not drop any items if spawned from statues.
                        var notStatue = npcLoot.DefineConditionalDropSet(new Conditions.NotFromStatue());
                        notStatue.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, iceMimicItems));
                    }
                    catch (ArgumentNullException) { }
                    break;

                // Ice Golem
                // 1-2 Essence of Eleum @ 100%
                case NPCID.IceGolem:
                    npcLoot.Add(ModContent.ItemType<EssenceofEleum>(), 1, 1, 2);
                    break;
                #endregion

                #region Aquatic / Ocean
                // Pink Jellyfish
                // Life Jelly @ 14.29% Normal, 25% Expert+
                case NPCID.PinkJellyfish:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<LifeJelly>(), 7, 4));
                    break;

                // Blue Jellyfish
                // Mana Jelly @ 14.29% Normal, 25% Expert+
                case NPCID.BlueJellyfish:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<ManaJelly>(), 7, 4));
                    break;

                // Green Jellyfish
                // Vital Jelly @ 14.29% Normal, 25% Expert+
                case NPCID.GreenJellyfish:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<VitalJelly>(), 7, 4));
                    break;

                // Sea Snail
                // Sea Shell @ 100%
                case NPCID.SeaSnail:
                    npcLoot.Add(ModContent.ItemType<SeaShell>());
                    break;

                // Shark
                // Shark Tooth Necklace @ 4% Normal, 6.67% Expert+
                // Joyful Heart @ 4% Normal, 6.67% Expert+
                case NPCID.Shark:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.SharkToothNecklace, 25, 15));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<JoyfulHeart>(), 25, 15));
                    break;

                // Blood Jelly, Fungo Fish
                // Jellyfish Necklace @ 1%
                case NPCID.BloodJelly:
                case NPCID.FungoFish:
                    npcLoot.Add(ItemID.JellyfishNecklace, 100);
                    break;
                #endregion

                #region Corruption, Crimson, Hallow
                // Gastropod
                // 5-10 Pink Gel @ 100%
                case NPCID.Gastropod:
                    npcLoot.Add(ItemID.PinkGel, 1, 5, 10);
                    break;

                // Clinger
                // Cursed Dagger @ 4% Normal, 6.67% Expert+
                case NPCID.Clinger:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<CursedDagger>(), 25, 15));
                    break;

                // Ichor Sticker
                // Ichor Spear @ 4% Normal, 6.67% Expert+
                case NPCID.IchorSticker:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<IchorSpear>(), 25, 15));
                    break;

                // Biome Mimics
                // Celestial Claymore @ 14.29% Normal, 25% Expert+
                case NPCID.BigMimicHallow:
                case NPCID.BigMimicCorruption:
                case NPCID.BigMimicCrimson:
                case NPCID.BigMimicJungle:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<CelestialClaymore>(), 7, 4));
                    break;
                #endregion

                #region Jungle
                // Jungle Slime, Spiked Jungle Slime, Arapaima
                // Murky Paste @ 33.33% Normal, 50% Expert+
                case NPCID.JungleSlime:
                case NPCID.SpikedJungleSlime:
                case NPCID.Arapaima:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<MurkyPaste>(), 3, 2));
                    break;

                // Giant Tortoise
                // Giant Tortoise Shell @ 14.29% Normal, 25% Expert+
                case NPCID.GiantTortoise:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<GiantTortoiseShell>(), 7, 4));
                    break;

                // Angry Trapper
                // Trapper Bulb @ 50% Normal, 100% Expert+
                case NPCID.AngryTrapper:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<TrapperBulb>(), 2, 1));
                    break;

                // Derpling, Cochineal Beetle, Cyan Beetle, Lac Beetle
                // Beetle Juice @ 33.33% Normal, 50% Expert+
                case NPCID.Derpling:
                case NPCID.CochinealBeetle:
                case NPCID.CyanBeetle:
                case NPCID.LacBeetle:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<BeetleJuice>(), 3, 2));
                    break;

                // Moth
                // Butterfly Dust @ 100% INSTEAD OF 50%
                case NPCID.Moth:
                    npcLoot.ChangeDropRate(ItemID.ButterflyDust, 1, 1);
                    break;
                #endregion

                #region Dungeon
                // Dark Caster
                // Ancient Shiv @ 4% Normal, 6.67% Expert+
                // Shinobi Blade @ 4% Normal, 6.67% Expert+
                // Staff of Necrosteocytes @ 4% Normal, 6.67% Expert+
                case NPCID.DarkCaster:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<AncientShiv>(), 25, 15));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<ShinobiBlade>(), 25, 15));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<StaffOfNecrosteocytes>(), 25, 15));
                    break;

                // Hardmode Dungeon Melee Skeletons
                // Wisp in a Bottle @ 0.5% INSTEAD OF 0.25%
                case NPCID.RustyArmoredBonesAxe:
                case NPCID.RustyArmoredBonesFlail:
                case NPCID.RustyArmoredBonesSword:
                case NPCID.RustyArmoredBonesSwordNoArmor:
                case NPCID.BlueArmoredBones:
                case NPCID.BlueArmoredBonesMace:
                case NPCID.BlueArmoredBonesNoPants:
                case NPCID.BlueArmoredBonesSword:
                case NPCID.HellArmoredBones:
                case NPCID.HellArmoredBonesSpikeShield:
                case NPCID.HellArmoredBonesMace:
                case NPCID.HellArmoredBonesSword:
                    npcLoot.ChangeDropRate(ItemID.WispinaBottle, 1, 200);
                    break;

                // Necromancer
                // Wrath of the Ancients @ 4% Normal, 6.67% Expert+
                case NPCID.Necromancer:
                case NPCID.NecromancerArmored:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<WrathoftheAncients>(), 25, 15));
                    break;

                // Giant Cursed Skull
                // Keelhaul @ 6.67% IF Leviathan dead
                case NPCID.GiantCursedSkull:
                    npcLoot.AddIf(() => DownedBossSystem.downedLeviathan, ModContent.ItemType<Keelhaul>(), 15);
                    break;

                // Bone Lee
                // Black Belt @ 25% INSTEAD OF 8.33%
                // Tabi @ 25% INSTEAD OF 8.33%
                case NPCID.BoneLee:
                    npcLoot.ChangeDropRate(ItemID.BlackBelt, 1, 4);
                    npcLoot.ChangeDropRate(ItemID.Tabi, 1, 4);
                    break;

                // Paladin
                // Paladin's Hammer @ 15% INSTEAD OF 6.67%
                // Paladin's Shield @ 20% INSTEAD OF 9.33%
                case NPCID.Paladin:
                    npcLoot.ChangeDropRate(ItemID.PaladinsHammer, 3, 20);
                    npcLoot.ChangeDropRate(ItemID.PaladinsShield, 1, 5);
                    break;
                #endregion

                #region Hell
                // Fire Imp
                // Ashen Stalactite @ 10% Normal, 16.67% Expert+
                case NPCID.FireImp:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<AshenStalactite>(), 10, 6));
                    break;

                // Demon, Voodoo Demon
                // Demonic Bone Ash @ 33.33% Normal, 50% Expert+
                // Bladecrest Oathsword @ 4% Normal, 6.67% Expert+
                case NPCID.Demon:
                case NPCID.VoodooDemon:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<DemonicBoneAsh>(), 3, 2));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<BladecrestOathsword>(), 25, 15));
                    break;

                // Bone Serpent
                // Demonic Bone Ash @ 33.33% Normal, 50% Expert+
                // Old Lord Oathsword @ 8.33% Normal, 14.29% Expert+
                case NPCID.BoneSerpentHead:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<DemonicBoneAsh>(), 3, 2));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<OldLordOathsword>(), 12, 7));
                    break;

                // Red Devil
                // Fire Feather @ 10% INSTEAD OF 1.33%
                // Demonic Bone Ash @ 33.33% Normal, 50% Expert+
                // Essence of Chaos @ 50% Normal, 100% Expert+
                case NPCID.RedDevil:
                    npcLoot.ChangeDropRate(ItemID.FireFeather, 1, 10);
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<DemonicBoneAsh>(), 3, 2));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<EssenceofChaos>(), 2, 1));
                    break;
                #endregion

                #region Blood Moon
                // Drippler
                // Bouncing Eyeball @ 2.5% Normal, 5% Expert+
                case NPCID.Drippler:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<BouncingEyeball>(), 40, 20));
                    break;

                // All Blood Moon fishing enemies
                // Drop more Blood Orbs @ 100%
                case NPCID.EyeballFlyingFish:
                case NPCID.ZombieMerman:
                    npcLoot.Add(ModContent.ItemType<BloodOrb>(), 1, 3, 6);
                    break;

                case NPCID.GoblinShark:
                case NPCID.BloodEelHead:
                    npcLoot.Add(ModContent.ItemType<BloodOrb>(), 1, 15, 30);
                    break;

                case NPCID.BloodNautilus:
                    npcLoot.Add(ModContent.ItemType<BloodOrb>(), 1, 20, 40);
                    break;
                #endregion

                #region Goblin Army
                // Goblin Warrior
                // Warblade @ 4% Normal, 6.67% Expert+
                case NPCID.GoblinWarrior:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Warblade>(), 25, 15));
                    break;

                // Goblin Sorcerer
                // Plasma Rod @ 4% Normal, 6.67% Expert+
                case NPCID.GoblinSorcerer:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<PlasmaRod>(), 25, 15));
                    break;

                // Goblin Summoner
                // Burning Strife @ 20% Normal, 33.33% Expert+
                // The First Shadowflame @ 20% Normal, 33.33% Expert+
                case NPCID.GoblinSummoner:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<BurningStrife>(), 5, 3));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<TheFirstShadowflame>(), 5, 3));
                    break;
                #endregion

                #region Pirates
                // Pirate Crossbower
                // Raider's Glory @ 4% Normal, 6.67% Expert+
                case NPCID.PirateCrossbower:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<RaidersGlory>(), 25, 15));
                    break;

                // Pirate Deadeye
                // Proporse Pistol @ 4% Normal, 6.67% Expert+
                case NPCID.PirateDeadeye:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<ProporsePistol>(), 25, 15));
                    break;
                #endregion

                #region Solar Eclipse
                // Weak Solar Eclipse Enemies: Frankenstein, Swamp Thing, Fritz, Creature from the Deep
                // Darksun Fragment @ 10% IF Devourer of Gods dead
                case NPCID.Frankenstein:
                case NPCID.SwampThing:
                case NPCID.Fritz:
                case NPCID.CreatureFromTheDeep:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<DarksunFragment>(), 10);
                    break;

                // Medium Solar Eclipse Enemies: The Possessed, Butcher, Dr. Man Fly
                // Darksun Fragment @ 50% IF Devourer of Gods dead
                case NPCID.ThePossessed:
                case NPCID.Butcher:
                case NPCID.DrManFly:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<DarksunFragment>(), 2);
                    break;

                // Reaper, Psycho
                // 2-4 Solar Veil @ 50% IF Clone or Plant dead
                // Darksun Fragment @ 50% IF Devourer of Gods dead
                case NPCID.Reaper:
                case NPCID.Psycho:
                    npcLoot.AddIf(() => DownedBossSystem.downedCalamitas || NPC.downedPlantBoss, ModContent.ItemType<SolarVeil>(), 2, 2, 4);
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<DarksunFragment>(), 2);
                    break;

                // Vampire / Vampire Bat (same enemy)
                // Moon Stone @ 15% INSTEAD OF 2.86%
                // Bat Hook @ 2.5% Normal, 5% Expert+
                // 2-4 Solar Veil @ 50% IF Clone or Plant dead
                // Darksun Fragment @ 50% IF Devourer of Gods dead
                case NPCID.VampireBat:
                case NPCID.Vampire:
                    npcLoot.ChangeDropRate(ItemID.MoonStone, 3, 20);
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.BatHook, 40, 20));
                    npcLoot.AddIf(() => DownedBossSystem.downedCalamitas || NPC.downedPlantBoss, ModContent.ItemType<SolarVeil>(), 2, 2, 4);
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<DarksunFragment>(), 2);
                    break;

                // Eyezor
                // 1-2 Darksun Fragment @ 100% IF Devourer of Gods dead
                case NPCID.Eyezor:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<DarksunFragment>(), 1, 1, 2);
                    break;

                // Nailhead
                // 3-5 Darksun Fragment @ 100% IF Devourer of Gods dead
                case NPCID.Nailhead:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<DarksunFragment>(), 1, 3, 5);
                    break;

                // Mothron
                // 20-30 Darksun Fragment @ 100% IF Devourer of Gods dead
                case NPCID.Mothron:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<DarksunFragment>(), 1, 20, 30);
                    break;

                // Deadly Sphere
                // Defective Sphere @ 20% Normal, 33.33% Expert+
                // Darksun Fragment @ 50% IF Devourer of Gods dead
                case NPCID.DeadlySphere:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<DefectiveSphere>(), 5, 3));
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<DarksunFragment>(), 2);
                    break;
                #endregion

                #region Pumpkin Moon
                // Splinterling
                // Nightmare Fuel @ 50% IF Devourer of Gods dead
                case NPCID.Splinterling:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<NightmareFuel>(), 2);
                    break;

                // Hellhound, Poltergeist
                // 1-2 Nightmare Fuel @ 50% IF Devourer of Gods dead
                case NPCID.Hellhound:
                case NPCID.Poltergeist:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<NightmareFuel>(), 2, 1, 2);
                    break;

                // Headless Horseman
                // 3-5 Nightmare Fuel @ 100% IF Devourer of Gods dead
                case NPCID.HeadlessHorseman:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<NightmareFuel>(), 1, 3, 5);
                    break;

                // Mourning Wood
                // 5-10 Nightmare Fuel @ 100% IF Devourer of Gods dead
                case NPCID.MourningWood:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<NightmareFuel>(), 1, 5, 10);
                    break;

                // Pumpking
                // 10-20 Nightmare Fuel @ 100% IF Devourer of Gods dead
                case NPCID.Pumpking:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<NightmareFuel>(), 1, 10, 20);
                    break;
                #endregion

                #region Frost Moon
                // Weak Frost Moon Enemies: Nutcracker, Elf Copter, Flocko
                // Endothermic Energy @ 50% IF Devourer of Gods dead
                case NPCID.Nutcracker:
                case NPCID.NutcrackerSpinning:
                case NPCID.ElfCopter:
                case NPCID.Flocko:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<EndothermicEnergy>(), 2);
                    break;

                // Medium Frost Moon Enemies: Krampus, Yeti, Present Mimic
                // 1-2 Endothermic Energy @ 50% IF Devourer of Gods dead
                case NPCID.Krampus:
                case NPCID.Yeti:
                case NPCID.PresentMimic:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<EndothermicEnergy>(), 2, 1, 2);
                    break;

                // Everscream
                // 3-5 Endothermic Energy @ 100% IF Devourer of Gods dead
                case NPCID.Everscream:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<EndothermicEnergy>(), 1, 3, 5);
                    break;

                // Santa-NK1
                // 5-10 Endothermic Energy @ 100% IF Devourer of Gods dead
                case NPCID.SantaNK1:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<EndothermicEnergy>(), 1, 5, 10);
                    break;

                // Ice Queen
                // 10-20 Endothermic Energy @ 100% IF Devourer of Gods dead
                case NPCID.IceQueen:
                    npcLoot.AddIf(() => DownedBossSystem.downedDoG, ModContent.ItemType<EndothermicEnergy>(), 1, 10, 20);
                    break;
                #endregion

                #region Martian Madness
                // Martian Madness On-Foot Soldiers
                // 1-4 Shock Grenade @ 25%
                case NPCID.BrainScrambler:
                case NPCID.GrayGrunt:
                case NPCID.GigaZapper:
                case NPCID.MartianEngineer:
                case NPCID.RayGunner:
                case NPCID.ScutlixRider:
                    npcLoot.Add(ModContent.ItemType<ShockGrenade>(), 4, 1, 4);
                    break;

                // Martian Officer
                // 3-8 Shock Grenade @ 33.33%
                case NPCID.MartianOfficer:
                    npcLoot.Add(ModContent.ItemType<ShockGrenade>(), 3, 3, 8);
                    break;

                // Martian Walker
                // Wingman @ 14.29% Normal, 25% Expert+
                case NPCID.MartianWalker:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Wingman>(), 7, 4));
                    break;

                // Martian Saucer
                // Nullification Pistol @ 14.29% Normal, 25% Expert+
                case NPCID.MartianSaucerCore:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<NullificationRifle>(), 7, 4));
                    break;
                #endregion

                #region Celestial Pillars
                // Solar Pillar Enemies
                // Solar Fragment @ 20% Normal, 25% Expert+
                // Meld Blob @ 20% Normal, 25% Expert+
                case NPCID.SolarSpearman: // Drakanian
                case NPCID.SolarSolenian: // Selenian
                case NPCID.SolarCorite:
                case NPCID.SolarSroller:
                case NPCID.SolarDrakomireRider:
                case NPCID.SolarDrakomire:
                case NPCID.SolarCrawltipedeHead:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.FragmentSolar, 5, 4));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<MeldBlob>(), 5, 4));
                    break;

                // Vortex Pillar Enemies
                // Vortex Fragment @ 20% Normal, 25% Expert+
                // Meld Blob @ 20% Normal, 25% Expert+
                case NPCID.VortexSoldier:     // Vortexian
                case NPCID.VortexLarva:       // Alien Larva
                case NPCID.VortexHornet:      // Alien Hornet
                case NPCID.VortexHornetQueen: // Alien Queen
                case NPCID.VortexRifleman:    // Storm Diver
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.FragmentVortex, 5, 4));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<MeldBlob>(), 5, 4));
                    break;

                // Nebula Pillar Enemies
                // Nebula Fragment @ 20% Normal, 25% Expert+
                // Meld Blob @ 20% Normal, 25% Expert+
                case NPCID.NebulaBrain:    // Nebula Floater
                case NPCID.NebulaSoldier:  // Predictor
                case NPCID.NebulaHeadcrab: // Brain Suckler
                case NPCID.NebulaBeast:    // Evolution Beast
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.FragmentNebula, 5, 4));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<MeldBlob>(), 5, 4));
                    break;

                // Stardust Pillar Enemies (except Small Star Cell because they duplicate infinitely)
                // Stardust Fragment @ 20% Normal, 25% Expert+
                // Meld Blob @ 20% Normal, 25% Expert+
                case NPCID.StardustSoldier:      // Stargazer
                case NPCID.StardustSpiderBig:    // Twinkle Popper
                case NPCID.StardustJellyfishBig: // Flow Invader
                case NPCID.StardustCellBig:      // Star Cell
                case NPCID.StardustWormHead:     // Milkyway Weaver
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.FragmentStardust, 5, 4));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<MeldBlob>(), 5, 4));
                    break;
                #endregion

                #region Town NPCs
                // Clothier
                // Clothiers Wrath @ 100% IF Hardmode
                case NPCID.Clothier:
                    npcLoot.AddIf(() => Main.hardMode, ModContent.ItemType<ClothiersWrath>());
                    break;

                // Angler
                // Golden Fishing Rod @ 100% IF fed to a Trasher
                // OTHERWISE,
                // Golden Fishing Rod @ 8.33% IF Hardmode
                case NPCID.Angler:
                    LeadingConditionRule trasherLCR = new(DropHelper.AnglerFedToTrasherCondition);
                    trasherLCR.Add(ItemID.GoldenFishingRod);
                    trasherLCR.OnFailedConditions(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ItemID.GoldenFishingRod, 12));
                    break;
                #endregion

                #region Vanilla Bosses
                case NPCID.KingSlime:
                    // Drop a huge spray of Gel items in chunks of 4
                    // More gel is not dropped on Expert because he has more minions, which increases the amount of gel provided.
                    DropOneByOne.Parameters kingSlimeGelSpray = new()
                    {
                        ChanceNumerator = 1,
                        ChanceDenominator = 1,
                        MinimumStackPerChunkBase = 4,
                        MaximumStackPerChunkBase = 4,
                        MinimumItemDropsCount = 18, // 18 * 4 = 72
                        MaximumItemDropsCount = 25, // 25 * 4 = 100
                    };
                    npcLoot.Add(new DropOneByOne(ItemID.Gel, kingSlimeGelSpray));

                    // Expert+ drops are also available on Normal
                    npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.RoyalGel));

                    // Would be in the bag otherwise
                    npcLoot.AddNormalOnly(ModContent.ItemType<CrownJewel>(), 10);

                    // Lore
                    npcLoot.AddConditionalPerPlayer(() => !NPC.downedSlimeKing, ModContent.ItemType<KnowledgeKingSlime>());
                    break;

                case NPCID.EyeofCthulhu:
                    // Expert+ drops are also available on Normal
                    npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.EoCShield));

                    // Would be in the bag otherwise
                    npcLoot.AddNormalOnly(ModContent.ItemType<TeardropCleaver>(), 10);
                    npcLoot.AddNormalOnly(ModContent.ItemType<DeathstareRod>(), 4);

                    // Lore
                    npcLoot.AddConditionalPerPlayer(() => !NPC.downedBoss1, ModContent.ItemType<KnowledgeEyeofCthulhu>());
                    break;

                case NPCID.EaterofWorldsHead:
                    // Expert+ drops are also available on Normal
                    LeadingConditionRule EoWKill = new(DropHelper.If((info) => info.npc.boss));
                    EoWKill.Add(DropHelper.PerPlayer(ItemID.WormScarf));
                    npcLoot.AddNormalOnly(EoWKill);

                    // Corruption World OR Drunk World: Corruption Lore
                    LeadingConditionRule eowCorruptionLore = new(DropHelper.If((info) => info.npc.boss && (!WorldGen.crimson || WorldGen.drunkWorldGen) && !NPC.downedBoss2));
                    eowCorruptionLore.Add(ModContent.ItemType<KnowledgeCorruption>());
                    eowCorruptionLore.Add(ModContent.ItemType<KnowledgeEaterofWorlds>());
                    npcLoot.Add(eowCorruptionLore);

                    // Crimson World OR Drunk World: Crimson Lore
                    LeadingConditionRule eowCrimsonLore = new(DropHelper.If((info) => info.npc.boss && (WorldGen.crimson || WorldGen.drunkWorldGen) && !NPC.downedBoss2));
                    eowCrimsonLore.Add(ModContent.ItemType<KnowledgeCrimson>());
                    eowCrimsonLore.Add(ModContent.ItemType<KnowledgeBrainofCthulhu>());
                    npcLoot.Add(eowCrimsonLore);
                    break;

                case NPCID.BrainofCthulhu:
                    // Expert+ drops are also available on Normal
                    npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.BrainOfConfusion));

                    // Corruption World OR Drunk World: Corruption Lore
                    LeadingConditionRule bocCorruptionLore = new(DropHelper.If(() => (!WorldGen.crimson || WorldGen.drunkWorldGen) && !NPC.downedBoss2));
                    bocCorruptionLore.Add(ModContent.ItemType<KnowledgeCorruption>());
                    bocCorruptionLore.Add(ModContent.ItemType<KnowledgeEaterofWorlds>());
                    npcLoot.Add(bocCorruptionLore);

                    // Crimson World OR Drunk World: Crimson Lore
                    LeadingConditionRule bocCrimsonLore = new(DropHelper.If(() => (WorldGen.crimson || WorldGen.drunkWorldGen) && !NPC.downedBoss2));
                    bocCrimsonLore.Add(ModContent.ItemType<KnowledgeCrimson>());
                    bocCrimsonLore.Add(ModContent.ItemType<KnowledgeBrainofCthulhu>());
                    npcLoot.Add(bocCrimsonLore);
                    break;

                case NPCID.QueenBee:
                    // Remove the vanilla loot rule for Queen Bee's weapon drops.
                    npcLoot.RemoveWhere((rule) =>
                    {
                        if (rule is DropBasedOnExpertMode expertDrop)
                            return expertDrop.ruleForNormalMode is OneFromOptionsNotScaledWithLuckDropRule weapons && weapons.dropIds[0] == ItemID.BeeGun;
                        return false;
                    });

                    // Define a replacement rule which drops the weapons Calamity style.
                    npcLoot.AddNormalOnly(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, ItemID.BeeKeeper, ItemID.BeesKnees, ItemID.BeeGun));

                    // Expert+ drops are also available on Normal
                    npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.HiveBackpack));
                    npcLoot.AddNormalOnly(ModContent.ItemType<TheBee>(), 10);

                    // Would be in the bag otherwise
                    npcLoot.AddNormalOnly(ModContent.ItemType<HardenedHoneycomb>(), 1, 30, 50);

                    // Queen Bee drops Stingers in Calamity
                    npcLoot.Add(ItemID.Stinger, 1, 8, 12);

                    // Lore
                    npcLoot.AddConditionalPerPlayer(() => !NPC.downedQueenBee, ModContent.ItemType<KnowledgeQueenBee>());
                    break;

                case NPCID.SkeletronHead:
                    // Drop a huge spray of Bone items in chunks of 5
                    DropOneByOne.Parameters skeletronBoneSpray = new()
                    {
                        ChanceNumerator = 1,
                        ChanceDenominator = 1,
                        MinimumStackPerChunkBase = 5,
                        MaximumStackPerChunkBase = 5,
                        MinimumItemDropsCount = 14, // 14 * 5 = 70
                        MaximumItemDropsCount = 20, // 20 * 5 = 100
                    };
                    npcLoot.Add(new DropOneByOne(ItemID.Bone, skeletronBoneSpray));

                    // Expert+ drops are also available on Normal
                    npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.BoneGlove));

                    // Lore
                    npcLoot.AddConditionalPerPlayer(() => !NPC.downedBoss3, ModContent.ItemType<KnowledgeSkeletron>());
                    break;

                case NPCID.WallofFlesh:
                    // Remove the vanilla loot rule for Wall of Flesh's weapon drops.
                    npcLoot.RemoveWhere((rule) =>
                    {
                        if (rule is LeadingConditionRule wofLCR)
                            return wofLCR.ChainedRules[0] is OneFromOptionsNotScaledWithLuckDropRule weapons && weapons.dropIds[0] == ItemID.BreakerBlade;
                        return false;
                    });

                    // Define a replacement rule which drops the weapons Calamity style.
                    var wofWeapons = new int[]
                    {
                        ItemID.BreakerBlade,
                        ItemID.ClockworkAssaultRifle,
                        ModContent.ItemType<Meowthrower>(),
                        ItemID.LaserRifle,
                        ModContent.ItemType<BlackHawkRemote>(),
                        ItemID.FireWhip,
                        ModContent.ItemType<BlastBarrel>(),
                    };
                    npcLoot.AddNormalOnly(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, wofWeapons));

                    // Remove the vanilla loot rule for Wall of Flesh's emblem drops.
                    npcLoot.RemoveWhere((rule) =>
                    {
                        if (rule is LeadingConditionRule wofLCR)
                            return wofLCR.ChainedRules[0] is OneFromOptionsNotScaledWithLuckDropRule emblems && emblems.dropIds[0] == ItemID.WarriorEmblem;
                        return false;
                    });

                    // Define a replacement rule which drops the emblems Calamity style.
                    var wofEmblems = new int[]
                    {
                        ItemID.WarriorEmblem,
                        ItemID.RangerEmblem,
                        ItemID.SorcererEmblem,
                        ItemID.SummonerEmblem,
                        ModContent.ItemType<RogueEmblem>(),
                    };
                    npcLoot.AddNormalOnly(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, wofEmblems));

                    // Would be in the bag otherwise
                    npcLoot.AddNormalOnly(ModContent.ItemType<Carnage>(), 10);

                    // Drop Hermit's Box directly for EACH player, regardles of Expert or not. 100% chance on first kill, 10% chance afterwards.
                    LeadingConditionRule firstWoFKill = new(DropHelper.If(() => !Main.hardMode));
                    firstWoFKill.Add(DropHelper.PerPlayer(ModContent.ItemType<HermitsBoxofOneHundredMedicines>()));
                    npcLoot.AddNormalOnly(firstWoFKill);
                    LeadingConditionRule subsequentWoFKills = new(DropHelper.If(() => Main.hardMode));
                    subsequentWoFKills.Add(DropHelper.PerPlayer(ModContent.ItemType<HermitsBoxofOneHundredMedicines>(), 10));
                    npcLoot.AddNormalOnly(subsequentWoFKills);

                    // Expert+ drops are also available on Normal
                    npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.DemonHeart));

                    // WoF drops Evil Keys
                    npcLoot.Add(ItemID.CorruptionKey, 3);
                    npcLoot.Add(ItemID.CrimsonKey, 3);

                    // Lore
                    LeadingConditionRule wofLore = npcLoot.AddConditionalPerPlayer(() => !Main.hardMode, ModContent.ItemType<KnowledgeUnderworld>());
                    wofLore.Add(DropHelper.PerPlayer(ModContent.ItemType<KnowledgeWallofFlesh>()));
                    break;

                case NPCID.QueenSlimeBoss:
                    // Expert+ drops are also available on Normal
                    npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.VolatileGelatin));

                    // Queen Slime drops the Hallowed Key
                    npcLoot.Add(ItemID.HallowedKey, 3);
                    break;

                case NPCID.TheDestroyer:
                    // Remove the vanilla loot rule for Hallowed Bars.
                    npcLoot.RemoveWhere((rule) => rule is ItemDropWithConditionRule conditionalRule && conditionalRule.itemId == ItemID.HallowedBar);

                    // Define a replacement rule which respects the Early Hardmode Progression Rework.
                    npcLoot.AddNormalOnly(ItemDropRule.ByCondition(DropHelper.HallowedBarsCondition, ItemID.HallowedBar, 1, 15, 30));

                    // Expert+ drops are also available on Normal
                    npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.MechanicalWagonPiece));

                    // Lore
                    npcLoot.AddConditionalPerPlayer(() => !NPC.downedMechBoss1, ModContent.ItemType<KnowledgeDestroyer>());
                    npcLoot.AddConditionalPerPlayer(ShouldDropMechLore, ModContent.ItemType<KnowledgeMechs>());
                    break;

                case NPCID.Retinazer:
                case NPCID.Spazmatism:
                    // Remove the vanilla loot rule for Hallowed Bars. This requires digging through the vanilla equivalent of "Last Twin Standing".
                    try
                    {
                        var twinsRootRules = npcLoot.Get(false);
                        IItemDropRule vanillaLastTwinStanding = twinsRootRules.Find((rule) => rule is LeadingConditionRule twinsLCR1 && twinsLCR1.condition is Conditions.MissingTwin);
                        if (vanillaLastTwinStanding is LeadingConditionRule LCR_LTS)
                        {
                            IItemDropRuleChainAttempt twinsChain1 = LCR_LTS.ChainedRules.Find((chainAttempt) =>
                                chainAttempt is Chains.TryIfSucceeded c && c.RuleToChain is LeadingConditionRule twinsLCR2 && twinsLCR2.condition is Conditions.NotExpert);
                            if (twinsChain1.RuleToChain is LeadingConditionRule LCR_NotExpert)
                            {
                                LCR_NotExpert.ChainedRules.RemoveAll((chainAttempt) =>
                                    chainAttempt is Chains.TryIfSucceeded c && c.RuleToChain is CommonDrop bars && bars.itemId == ItemID.HallowedBar);
                            }
                        }

                        // Define a replacement rule which respects the Early Hardmode Progression Rework.
                        npcLoot.AddNormalOnly(ItemDropRule.ByCondition(DropHelper.HallowedBarsCondition, ItemID.HallowedBar, 1, 15, 30));
                    }
                    catch (ArgumentNullException) { }

                    // Expert+ drops are also available on Normal. These are done manually due to Last Twin Standing.
                    npcLoot.AddIf((info) => !Main.expertMode && IsLastTwinStanding(info), ItemID.MechanicalWheelPiece);
                    npcLoot.AddIf((info) => !Main.expertMode && IsLastTwinStanding(info), ModContent.ItemType<Arbalest>(), 10);

                    // Lore
                    npcLoot.AddConditionalPerPlayer((info) => !NPC.downedMechBoss2 && IsLastTwinStanding(info), ModContent.ItemType<KnowledgeTwins>());
                    npcLoot.AddConditionalPerPlayer(ShouldDropMechLore, ModContent.ItemType<KnowledgeMechs>());
                    break;

                case NPCID.SkeletronPrime:
                    // Remove the vanilla loot rule for Hallowed Bars.
                    npcLoot.RemoveWhere((rule) => rule is ItemDropWithConditionRule conditionalRule && conditionalRule.itemId == ItemID.HallowedBar);

                    // Define a replacement rule which respects the Early Hardmode Progression Rework.
                    npcLoot.AddNormalOnly(ItemDropRule.ByCondition(DropHelper.HallowedBarsCondition, ItemID.HallowedBar, 1, 15, 30));

                    // Expert+ drops are also available on Normal
                    npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.MechanicalBatteryPiece));

                    // Lore
                    npcLoot.AddConditionalPerPlayer(() => !NPC.downedMechBoss3, ModContent.ItemType<KnowledgeSkeletronPrime>());
                    npcLoot.AddConditionalPerPlayer(ShouldDropMechLore, ModContent.ItemType<KnowledgeMechs>());
                    break;

                case NPCID.Plantera:
                    // Remove the vanilla loot rule for Plantera's weapon drops. This requires digging through the bizarre choices made in Plantera's drops.
                    // Namely, Plantera always drops the Grenade Launcher on first kill and ignores her normal loot table.
                    try
                    {
                        var planteraRootRules = npcLoot.Get(false);
                        IItemDropRule notExpert = planteraRootRules.Find((rule) => rule is LeadingConditionRule planteraLCR1 && planteraLCR1.condition is Conditions.NotExpert);
                        if (notExpert is LeadingConditionRule LCR_NotExpert)
                        {
                            IItemDropRuleChainAttempt planteraWeaponsChain = LCR_NotExpert.ChainedRules.Find((chainAttempt) =>
                                chainAttempt is Chains.TryIfSucceeded c && c.RuleToChain is LeadingConditionRule planteraLCR2 && planteraLCR2.condition is Conditions.FirstTimeKillingPlantera);
                            if (planteraWeaponsChain.RuleToChain is LeadingConditionRule LCR_FirstPlantera)
                            {
                                // Calamity removes this behavior entirely. Nothing special happens the first time you kill Plantera.
                                LCR_FirstPlantera.ChainedRules.Clear();

                                // Define a replacement rule which drops the weapons Calamity style.
                                // Yes, this means Plantera no longer drops Rocket Is when she drops the Grenade Launcher.
                                // I could not care less at this point.
                                var planteraWeapons = new int[]
                                {
                                    ItemID.FlowerPow,
                                    ItemID.Seedler,
                                    ItemID.GrenadeLauncher,
                                    ItemID.VenusMagnum,
                                    ItemID.LeafBlower,
                                    ItemID.NettleBurst,
                                    ItemID.WaspGun,
                                    ItemID.PygmyStaff,
                                };
                                LCR_NotExpert.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, planteraWeapons));
                            }

                            // Also, the Pygmy Staff drops separately from her normal weapon pool. Calamity fixes this.
                            LCR_NotExpert.ChainedRules.RemoveAll((chainAttempt) =>
                                chainAttempt is Chains.TryIfSucceeded c && c.RuleToChain is CommonDrop pygmyStaff && pygmyStaff.itemId == ItemID.PygmyStaff);
                        }
                    }
                    catch (ArgumentNullException) { }

                    // Expert+ drops are also available on Normal
                    npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.SporeSac));

                    // Would be in the bag otherwise
                    npcLoot.AddNormalOnly(ModContent.ItemType<BloomStone>(), 10);
                    npcLoot.AddNormalOnly(ModContent.ItemType<BlossomFlux>(), 10);
                    npcLoot.AddNormalOnly(DropHelper.PerPlayer(ModContent.ItemType<LivingShard>(), 1, 12, 18));

                    // Plantera drops Jungle Key
                    npcLoot.Add(ItemID.JungleKey, 3);

                    // Lore
                    npcLoot.AddConditionalPerPlayer(() => !NPC.downedPlantBoss, ModContent.ItemType<KnowledgePlantera>());
                    break;

                case NPCID.HallowBoss:
                    // Remove the vanilla loot rule for Empress of Light's weapon drops. This requires digging through her loot rule tree.
                    try
                    {
                        var empressRootRules = npcLoot.Get(false);
                        IItemDropRule notExpert = empressRootRules.Find((rule) => rule is LeadingConditionRule empressLCR && empressLCR.condition is Conditions.NotExpert);
                        if (notExpert is LeadingConditionRule LCR_NotExpert)
                        {
                            LCR_NotExpert.ChainedRules.RemoveAll((chainAttempt) =>
                                chainAttempt is Chains.TryIfSucceeded c && c.RuleToChain is OneFromOptionsDropRule empressWeapons);

                            // Define a replacement rule which drops the items Calamity style.
                            // This includes her wings, because they have a pathetically low drop rate normally.
                            var empressItems = new int[]
                            {
                                ItemID.FairyQueenMagicItem,
                                ItemID.FairyQueenRangedItem,
                                ItemID.EmpressBlade,
                                ItemID.RainbowWhip,
                                ItemID.PiercingStarlight,
                                ItemID.RainbowWings
                            };
                            LCR_NotExpert.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, empressItems));
                        }

                        // Remove the vanilla loot rule for Empress Wings because it's part of the Calamity Style set.
                        empressRootRules.RemoveAll((rule) =>
                            rule is ItemDropWithConditionRule conditionalRule && conditionalRule.condition is Conditions.NotExpert && conditionalRule.itemId == ItemID.RainbowWings);
                    }
                    catch (ArgumentNullException) { }

                    // Expert+ drops are also available on Normal
                    npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.EmpressFlightBooster));
                    break;

                case NPCID.Golem:
                    // Remove the vanilla loot rule for Golem's weapon drops. This requires digging through its loot rule tree.
                    try
                    {
                        var golemRootRules = npcLoot.Get(false);
                        IItemDropRule notExpert = golemRootRules.Find((rule) => rule is LeadingConditionRule golemLCR && golemLCR.condition is Conditions.NotExpert);
                        if (notExpert is LeadingConditionRule LCR_NotExpert)
                        {
                            LCR_NotExpert.ChainedRules.RemoveAll((chainAttempt) =>
                                chainAttempt is Chains.TryIfSucceeded c && c.RuleToChain is OneFromRulesRule golemItems);

                            // Define a replacement rule which drops the items Calamity style.
                            // Yes, this means Golem no longer drops Stynger Bolts when it drops the Stynger.
                            // I could not care less at this point.
                            var golemItems = new int[]
                            {
                                ItemID.GolemFist,
                                ItemID.PossessedHatchet,
                                ItemID.Stynger,
                                ItemID.HeatRay,
                                ItemID.StaffofEarth,
                                ItemID.EyeoftheGolem,
                                ItemID.SunStone,
                            };
                            LCR_NotExpert.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, golemItems));
                        }
                    }
                    catch (ArgumentNullException) { }

                    // Expert+ drops are also available on Normal
                    var normalOnly = npcLoot.DefineNormalOnlyDropSet();
                    normalOnly.Add(DropHelper.PerPlayer(ItemID.ShinyStone));

                    // Would be in the bag otherwise
                    normalOnly.Add(ModContent.ItemType<EssenceofSunlight>(), 1, 5, 10);
                    normalOnly.Add(ModContent.ItemType<AegisBlade>(), 10);

                    // If Golem has never been killed, provide a Picksaw to all players. This only applies in Normal Mode.
                    // The Golem Treasure Bag is guaranteed to provide a Picksaw if one is not yet in the inventory.
                    LeadingConditionRule firstGolemKill = new(DropHelper.If(() => !NPC.downedGolemBoss));
                    firstGolemKill.Add(DropHelper.PerPlayer(ItemID.Picksaw));
                    normalOnly.Add(firstGolemKill);

                    // Lore
                    npcLoot.AddConditionalPerPlayer(() => !NPC.downedGolemBoss, ModContent.ItemType<KnowledgeGolem>());
                    break;

                case NPCID.DD2Betsy:
                    // Remove the vanilla loot rule for Betsy's weapon drops. This requires digging through her loot rule tree.
                    try
                    {
                        var betsyRootRules = npcLoot.Get(false);
                        IItemDropRule notExpert = betsyRootRules.Find((rule) => rule is LeadingConditionRule betsyLCR && betsyLCR.condition is Conditions.NotExpert);
                        if (notExpert is LeadingConditionRule LCR_NotExpert)
                        {
                            LCR_NotExpert.ChainedRules.RemoveAll((chainAttempt) =>
                                chainAttempt is Chains.TryIfSucceeded c && c.RuleToChain is OneFromOptionsNotScaledWithLuckDropRule betsyWeapons);

                            // Define a replacement rule which drops the weapons Calamity style.
                            var betsyWeapons = new int[]
                            {
                                ItemID.DD2SquireBetsySword, // Flying Dragon
                                ItemID.MonkStaffT3,         // Sky Dragon's Fury
                                ItemID.DD2BetsyBow,         // Aerial Bane
                                ItemID.ApprenticeStaffT3,   // Betsy's Wrath
                            };
                            LCR_NotExpert.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, betsyWeapons));
                        }
                    }
                    catch (ArgumentNullException) { }
                    break;

                case NPCID.DukeFishron:
                    // Remove the vanilla loot rule for Duke Fishron's weapon drops. This requires digging through his loot rule tree.
                    try
                    {
                        var dukeRootRules = npcLoot.Get(false);
                        IItemDropRule notExpert = dukeRootRules.Find((rule) => rule is LeadingConditionRule dukeLCR && dukeLCR.condition is Conditions.NotExpert);
                        if (notExpert is LeadingConditionRule LCR_NotExpert)
                        {
                            LCR_NotExpert.ChainedRules.RemoveAll((chainAttempt) =>
                                chainAttempt is Chains.TryIfSucceeded c && c.RuleToChain is OneFromOptionsDropRule dukeWeapons);

                            // Define a replacement rule which drops the items Calamity style.
                            // This includes his wings, because they have a pathetically low drop rate normally.
                            var dukeItems = new int[]
                            {
                                ItemID.Flairon,
                                ItemID.Tsunami,
                                ItemID.BubbleGun,
                                ItemID.RazorbladeTyphoon,
                                ItemID.TempestStaff,
                                ModContent.ItemType<DukesDecapitator>(),
                                ItemID.FishronWings,
                            };
                            LCR_NotExpert.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, dukeItems));
                        }

                        // Remove the vanilla loot rule for Fishron Wings because it's part of the Calamity Style set.
                        dukeRootRules.RemoveAll((rule) =>
                            rule is ItemDropWithConditionRule conditionalRule && conditionalRule.condition is Conditions.NotExpert && conditionalRule.itemId == ItemID.FishronWings);
                    }
                    catch (ArgumentNullException) { }

                    // Expert+ drops are also available on Normal
                    npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.ShrimpyTruffle));

                    // Would be in the bag otherwise
                    npcLoot.AddNormalOnly(ModContent.ItemType<BrinyBaron>(), 10);

                    // Lore
                    npcLoot.AddConditionalPerPlayer(() => !NPC.downedFishron, ModContent.ItemType<KnowledgeDukeFishron>());
                    break;

                case NPCID.CultistBoss:
                    // Lore
                    npcLoot.AddConditionalPerPlayer(() => !NPC.downedAncientCultist, ModContent.ItemType<KnowledgeLunaticCultist>());

                    // Special Blood Moon Lore
                    npcLoot.AddConditionalPerPlayer(() => Main.bloodMoon, ModContent.ItemType<KnowledgeBloodMoon>());
                    break;

                case NPCID.MoonLordCore:
                    // Remove the vanilla loot rule for Moon Lord's weapon drops. This requires digging through his loot rule tree.
                    try
                    {
                        var moonLordRootRules = npcLoot.Get(false);
                        IItemDropRule notExpert = moonLordRootRules.Find((rule) => rule is LeadingConditionRule moonLordLCR && moonLordLCR.condition is Conditions.NotExpert);
                        if (notExpert is LeadingConditionRule LCR_NotExpert)
                        {
                            LCR_NotExpert.ChainedRules.RemoveAll((chainAttempt) =>
                                chainAttempt is Chains.TryIfSucceeded c && c.RuleToChain is OneFromOptionsNotScaledWithLuckDropRule moonLordWeapons);

                            // Define a replacement rule which drops the weapons Calamity style.
                            var moonLordWeapons = new int[]
                            {
                                ItemID.Meowmere,
                                ItemID.StarWrath,
                                ItemID.Terrarian,
                                ItemID.Celeb2,
                                ItemID.SDMG,
                                ItemID.LastPrism,
                                ItemID.LunarFlareBook,
                                ItemID.MoonlordTurretStaff, // Lunar Portal Staff
                                ItemID.RainbowCrystalStaff,
                                ModContent.ItemType<UtensilPoker>(),
                            };
                            LCR_NotExpert.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, moonLordWeapons));
                        }
                    }
                    catch (ArgumentNullException) { }

                    // Expert+ drops are also available on Normal
                    npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.GravityGlobe));
                    npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.SuspiciousLookingTentacle));
                    npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.LongRainbowTrailWings));

                    // Would be in the bag otherwise
                    npcLoot.AddNormalOnly(DropHelper.PerPlayer(ModContent.ItemType<CelestialOnion>()));

                    // Lore
                    npcLoot.AddConditionalPerPlayer(() => !NPC.downedMoonlord, ModContent.ItemType<KnowledgeMoonLord>());
                    break;

                default:
                    break;
                #endregion
            }

            // All Hardmode Dungeon Enemies
            // Ectoplasm @ 20%
            if (CalamityLists.dungeonEnemyBuffList.Contains(npc.type))
                npcLoot.Add(ItemID.Ectoplasm, 5);

            // All Moss Hornets
            // Stinger @ 50% Normal, 100% Expert+
            // Needler @ 4% Normal, 6.67% Expert+
            if (CalamityLists.mossHornetList.Contains(npc.type))
            {
                npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.Stinger, 2, 1));
                npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Needler>(), 25, 15));
            }

            // All Skeletons
            // Waraxe @ 6.67% IF Not Hardmode
            // Ancient Bone Dust @ 20% Normal, 33.33% Expert+
            if (CalamityLists.skeletonList.Contains(npc.type))
            {
                npcLoot.AddIf(() => !Main.hardMode, ModContent.ItemType<Waraxe>(), 15);
                npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<AncientBoneDust>(), 5, 3));
            }
        }
        #endregion

        #region Modify Global Loot Main Hook
        public override void ModifyGlobalLoot(GlobalLoot globalLoot)
        {
            // Gold armor set bonus: 4% chance to drop 1 Gold from all valid enemies
            // See the condition lambda in DropHelper for details
            // Does not show up in the Bestiary
            LeadingConditionRule goldNormalEnemiesDrop = new LeadingConditionRule(DropHelper.GoldSetBonusGoldCondition);
            goldNormalEnemiesDrop.Add(ItemID.GoldCoin, 25, hideLootReport: true);
            globalLoot.Add(goldNormalEnemiesDrop);

            // Same as above, except Gold armor makes all bosses drop 3 gold
            LeadingConditionRule goldBossDrop = new LeadingConditionRule(DropHelper.GoldSetBonusBossCondition);
            goldBossDrop.Add(ItemID.GoldCoin, minQuantity: 3, maxQuantity: 3, hideLootReport: true);
            globalLoot.Add(goldBossDrop);
            
            // Tarragon armor set bonus: 20% chance to drop hearts from all valid enemies
            // See the condition lambda in DropHelper for details
            // Does not show up in the Bestiary
            LeadingConditionRule tarragonDrop = new LeadingConditionRule(DropHelper.TarragonSetBonusHeartCondition);
            tarragonDrop.Add(ItemID.Heart, 5, hideLootReport: true);
            globalLoot.Add(tarragonDrop);

            // Blood Orb drops: 20% chance from any valid enemy on the surface during a Blood Moon
            // See the condition lambda in DropHelper for details
            // Does not show up in the Bestiary
            LeadingConditionRule bloodOrbDrop = new LeadingConditionRule(DropHelper.BloodOrbBaseCondition);
            bloodOrbDrop.Add(ModContent.ItemType<BloodOrb>(), 5, hideLootReport: true);

            // Bloodflare set bonus Blood Orb drops: 50% chance from any valid enemy on the surface during a Blood Moon
            // See the condition lambda in DropHelper for details
            // Does not show up in the Bestiary
            LeadingConditionRule bloodflareBloodOrbDrop = new LeadingConditionRule(DropHelper.BloodOrbBloodflareCondition);
            bloodflareBloodOrbDrop.Add(ModContent.ItemType<BloodOrb>(), 2, hideLootReport: true);
        }
        #endregion

        #region On Kill Main Hook
        public override void OnKill(NPC npc)
        {
            // Boss Rush on-kill effects
            if (BossRushEvent.BossRushActive)
            {
                // Progress the Boss Rush event
                BossRushEvent.OnBossKill(npc, Mod);

                // Block anything except the Rock from dropping
                DropHelper.BlockEverything(ModContent.ItemType<Rock>());
            }

            // Acid Rain on-kill effects
            if (AcidRainEvent.AcidRainEventIsOngoing)
                AcidRainEvent.OnEnemyKill(npc);

            // Stop Death Mode splitting worms from dropping excessive loot
            if (CalamityWorld.death && !SplittingWormLootBlockWrapper(npc, Mod))
                DropHelper.BlockEverything();

            // Stop Eater of Worlds segments and Brain of Cthulhu Creepers from dropping hearts or partial loot in Rev+
            if (CalamityWorld.revenge && (CalamityLists.EaterofWorldsIDs.Contains(npc.type) || npc.type == NPCID.Creeper))
                DropHelper.BlockDrops(ItemID.Heart, ItemID.DemoniteOre, ItemID.ShadowScale, ItemID.CrimtaneOre, ItemID.TissueSample);

            // Stop certain boss minions from providing hearts in Rev+
            if (CalamityWorld.revenge && CalamityLists.heartDropBlockList.Contains(npc.type))
                DropHelper.BlockDrops(ItemID.Heart);

            // Check whether bosses should be spawned naturally as a result of this NPC's death.
            CheckBossSpawn(npc);

            // Determine whether this NPC is the second Twin killed in a fight, regardless of which Twin it is.
            bool lastTwinStanding = false;
            if (npc.type == NPCID.Retinazer)
                lastTwinStanding = !NPC.AnyNPCs(NPCID.Spazmatism);
            else if (npc.type == NPCID.Spazmatism)
                lastTwinStanding = !NPC.AnyNPCs(NPCID.Retinazer);

            // On-kill NON-LOOT behavior for Eater of Worlds
            if ((npc.boss && (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail)) || npc.type == NPCID.BrainofCthulhu)
            {
                SetNewShopVariable(new int[] { NPCID.Merchant, NPCID.ArmsDealer, NPCID.Dryad, NPCID.Demolitionist }, NPC.downedBoss2);
                SetNewBossJustDowned(npc);
            }

            // On-kill NON-LOOT behavior for every other vanilla boss
            switch (npc.type)
            {
                case NPCID.KingSlime:
                    SetNewShopVariable(new int[] { NPCID.Dryad }, NPC.downedSlimeKing);
                    SetNewBossJustDowned(npc);
                    break;

                case NPCID.EyeofCthulhu:
                    SetNewShopVariable(new int[] { NPCID.Merchant, NPCID.Dryad, NPCID.Demolitionist }, NPC.downedBoss1);
                    SetNewBossJustDowned(npc);
                    break;

                case NPCID.Deerclops:
                    SetNewBossJustDowned(npc);
                    break;

                case NPCID.QueenBee:
                    SetNewShopVariable(new int[] { NPCID.ArmsDealer, NPCID.Dryad }, NPC.downedQueenBee);
                    SetNewBossJustDowned(npc);
                    break;

                case NPCID.SkeletronHead:
                    SetNewShopVariable(new int[] { NPCID.Merchant, NPCID.Dryad, NPCID.Demolitionist }, NPC.downedBoss3);
                    SetNewBossJustDowned(npc);
                    break;

                case NPCID.WallofFlesh:
                    SetNewShopVariable(new int[] { NPCID.Merchant, NPCID.ArmsDealer, NPCID.Dryad, NPCID.Painter, NPCID.WitchDoctor, NPCID.Stylist, NPCID.DyeTrader, NPCID.Demolitionist, NPCID.PartyGirl, NPCID.Clothier, NPCID.SkeletonMerchant, ModContent.NPCType<THIEF>() }, Main.hardMode);
                    SetNewBossJustDowned(npc);

                    if (!Main.hardMode)
                    {
                        // Increase altar count to allow natural mech boss spawning.
                        if (CalamityConfig.Instance.EarlyHardmodeProgressionRework)
                            WorldGen.altarCount++;

                        string key2 = "Mods.CalamityMod.UglyBossText";
                        Color messageColor2 = Color.Aquamarine;
                        CalamityUtils.DisplayLocalizedText(key2, messageColor2);

                        // TODO -- this should probably be moved to a thread like Aureus meteor
                        if (CalamityConfig.Instance.EarlyHardmodeProgressionRework)
                        {
                            string key3 = "Mods.CalamityMod.HardmodeOreTier1Text";
                            Color messageColor3 = new Color(50, 255, 130);
                            CalamityUtils.SpawnOre(TileID.Cobalt, 12E-05, 0.4f, 0.6f, 3, 8);
                            CalamityUtils.SpawnOre(TileID.Palladium, 12E-05, 0.4f, 0.6f, 3, 8);
                            CalamityUtils.DisplayLocalizedText(key3, messageColor3);
                        }
                    }
                    break;

                case NPCID.QueenSlimeBoss:
                    SetNewBossJustDowned(npc);
                    break;

                case NPCID.TheDestroyer:
                    SetNewShopVariable(new int[] { NPCID.DD2Bartender, NPCID.Stylist, NPCID.Truffle, ModContent.NPCType<THIEF>() }, NPC.downedMechBossAny);
                    SetNewShopVariable(new int[] { NPCID.Stylist, ModContent.NPCType<DILF>(), ModContent.NPCType<FAP>(), ModContent.NPCType<THIEF>() }, NPC.downedMechBoss1 || !NPC.downedMechBoss2 || !NPC.downedMechBoss3);
                    SetNewBossJustDowned(npc);

                    if (!NPC.downedMechBoss1 && CalamityConfig.Instance.EarlyHardmodeProgressionRework)
                        SpawnMechBossHardmodeOres();
                    break;

                case NPCID.Spazmatism:
                case NPCID.Retinazer:
                    if (lastTwinStanding)
                    {
                        SetNewShopVariable(new int[] { NPCID.DD2Bartender, NPCID.Stylist, NPCID.Truffle, ModContent.NPCType<THIEF>() }, NPC.downedMechBossAny);
                        SetNewShopVariable(new int[] { NPCID.Stylist, ModContent.NPCType<DILF>(), ModContent.NPCType<FAP>(), ModContent.NPCType<THIEF>() }, !NPC.downedMechBoss1 || NPC.downedMechBoss2 || !NPC.downedMechBoss3);
                        SetNewBossJustDowned(npc);

                        if (!NPC.downedMechBoss2 && CalamityConfig.Instance.EarlyHardmodeProgressionRework)
                            SpawnMechBossHardmodeOres();
                    }
                    break;

                case NPCID.SkeletronPrime:
                    SetNewShopVariable(new int[] { NPCID.DD2Bartender, NPCID.Stylist, NPCID.Truffle, ModContent.NPCType<THIEF>() }, NPC.downedMechBossAny);
                    SetNewShopVariable(new int[] { NPCID.Stylist, ModContent.NPCType<DILF>(), ModContent.NPCType<FAP>(), ModContent.NPCType<THIEF>() }, !NPC.downedMechBoss1 || !NPC.downedMechBoss2 || NPC.downedMechBoss3);
                    SetNewBossJustDowned(npc);

                    if (!NPC.downedMechBoss3 && CalamityConfig.Instance.EarlyHardmodeProgressionRework)
                        SpawnMechBossHardmodeOres();
                    break;

                case NPCID.Plantera:
                    SetNewShopVariable(new int[] { NPCID.WitchDoctor, NPCID.Truffle, ModContent.NPCType<FAP>(), ModContent.NPCType<THIEF>() }, NPC.downedPlantBoss);
                    SetNewBossJustDowned(npc);

                    // Spawn Perennial Ore if Plantera has never been killed
                    if (!NPC.downedPlantBoss)
                    {
                        string key = "Mods.CalamityMod.PlantOreText";
                        Color messageColor = Color.GreenYellow;
                        string key2 = "Mods.CalamityMod.SandSharkText3";
                        Color messageColor2 = Color.Goldenrod;

                        // TODO -- this should probably be moved to a thread like Aureus meteor
                        CalamityUtils.SpawnOre(ModContent.TileType<PerennialOre>(), 12E-05, 0.5f, 0.7f, 3, 8, TileID.Dirt, TileID.Stone);

                        CalamityUtils.DisplayLocalizedText(key, messageColor);
                        CalamityUtils.DisplayLocalizedText(key2, messageColor2);
                    }
                    break;

                case NPCID.HallowBoss:
                    SetNewBossJustDowned(npc);
                    break;

                case NPCID.Everscream:
                    SetNewShopVariable(new int[] { ModContent.NPCType<DILF>() }, NPC.downedChristmasTree || !NPC.downedChristmasSantank || !NPC.downedChristmasIceQueen);
                    break;

                case NPCID.SantaNK1:
                    SetNewShopVariable(new int[] { ModContent.NPCType<DILF>() }, !NPC.downedChristmasTree || NPC.downedChristmasSantank || !NPC.downedChristmasIceQueen);
                    break;

                case NPCID.IceQueen:
                    SetNewShopVariable(new int[] { ModContent.NPCType<DILF>() }, !NPC.downedChristmasTree || !NPC.downedChristmasSantank || NPC.downedChristmasIceQueen);
                    break;

                case NPCID.Golem:
                    SetNewShopVariable(new int[] { NPCID.ArmsDealer, NPCID.Cyborg, NPCID.Steampunker, NPCID.Wizard, NPCID.WitchDoctor, NPCID.DD2Bartender, ModContent.NPCType<FAP>(), ModContent.NPCType<THIEF>() }, NPC.downedGolemBoss);
                    SetNewBossJustDowned(npc);

                    // If Golem has never been killed, send a message about the Plague.
                    if (!NPC.downedGolemBoss)
                    {
                        string key3 = "Mods.CalamityMod.BabyBossText";
                        Color messageColor3 = Color.Lime;

                        CalamityUtils.DisplayLocalizedText(key3, messageColor3);
                    }
                    break;

                case NPCID.DD2Betsy:
                    // Mark Betsy as dead (Vanilla does not keep track of her)
                    DownedBossSystem.downedBetsy = true;
                    CalamityNetcode.SyncWorld();
                    break;

                case NPCID.DukeFishron:
                    SetNewBossJustDowned(npc);
                    break;

                case NPCID.CultistBoss:
                    SetNewBossJustDowned(npc);

                    // Deus text (this is not a loot function)
                    if (!NPC.downedAncientCultist)
                    {
                        string key4 = "Mods.CalamityMod.DeusText";
                        Color messageColor4 = Color.Gold;

                        CalamityUtils.DisplayLocalizedText(key4, messageColor4);
                    }
                    break;

                case NPCID.MoonLordCore:
                    SetNewShopVariable(new int[] { ModContent.NPCType<THIEF>() }, NPC.downedMoonlord);
                    SetNewBossJustDowned(npc);

                    string key5 = "Mods.CalamityMod.MoonBossText";
                    Color messageColor5 = Color.Orange;
                    string key6 = "Mods.CalamityMod.MoonBossText2";
                    Color messageColor6 = Color.Violet;
                    string key7 = "Mods.CalamityMod.ProfanedBossText2";
                    Color messageColor7 = Color.Cyan;
                    string key8 = "Mods.CalamityMod.FutureOreText";
                    Color messageColor8 = Color.LightGray;

                    if (!CalamityWorld.HasGeneratedLuminitePlanetoids)
                    {
                        // Generate luminite planetoids.
                        // This operation is done on a separate thread to lighten the load on servers so that they
                        // can focus on more critical operations asychronously and ideally avoid a time-out crash.
                        // Very few operations in Terraria utilize the pool, so it is highly unlikely that no threads will remain in it.
                        ThreadPool.QueueUserWorkItem(_ => LuminitePlanet.GenerateLuminitePlanetoids());

                        CalamityWorld.HasGeneratedLuminitePlanetoids = true;

                        // If the moon lord is already marked as dead, an associated world sync packet will not be sent automatically
                        // Send one manually.
                        if (NPC.downedMoonlord)
                            CalamityNetcode.SyncWorld();
                    }

                    // Spawn Exodium planetoids and send messages about Providence, Bloodstone, Phantoplasm, etc. if ML has not been killed yet
                    if (!NPC.downedMoonlord)
                    {
                        CalamityUtils.DisplayLocalizedText(key5, messageColor5);
                        CalamityUtils.DisplayLocalizedText(key6, messageColor6);
                        CalamityUtils.DisplayLocalizedText(key7, messageColor7);
                        CalamityUtils.DisplayLocalizedText(key8, messageColor8);
                    }
                    break;
            }
        }
        #endregion

        #region Spawn Mech Boss Hardmode Ores
        // TODO -- not loot code, should be moved eventually, and placed into a thread like Aureus meteor
        private void SpawnMechBossHardmodeOres()
        {
            if (!NPC.downedMechBossAny)
            {
                string key = "Mods.CalamityMod.HardmodeOreTier2Text";
                Color messageColor = new Color(50, 255, 130);
                CalamityUtils.SpawnOre(TileID.Mythril, 12E-05, 0.5f, 0.7f, 3, 8);
                CalamityUtils.SpawnOre(TileID.Orichalcum, 12E-05, 0.5f, 0.7f, 3, 8);
                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }
            else if ((!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1))
            {
                string key = "Mods.CalamityMod.HardmodeOreTier3Text";
                Color messageColor = new Color(50, 255, 130);
                CalamityUtils.SpawnOre(TileID.Adamantite, 12E-05, 0.6f, 0.8f, 3, 8);
                CalamityUtils.SpawnOre(TileID.Titanium, 12E-05, 0.6f, 0.8f, 3, 8);
                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }
            else
            {
                string key = "Mods.CalamityMod.HardmodeOreTier4Text";
                Color messageColor = new Color(50, 255, 130);
                CalamityUtils.SpawnOre(ModContent.TileType<HallowedOre>(), 12E-05, 0.45f, 0.8f, 3, 8, TileID.Pearlstone, TileID.HallowHardenedSand, TileID.HallowSandstone, TileID.HallowedIce);
                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }
        }
        #endregion

        #region Splitting Worm Loot
        internal static bool SplittingWormLootBlockWrapper(NPC npc, Mod mod)
        {
            if (!CalamityWorld.death)
                return true;

            switch (npc.type)
            {
                case NPCID.DiggerHead:
                case NPCID.DiggerBody:
                case NPCID.DiggerTail:
                    return SplittingWormLoot(npc, mod, 0);
                case NPCID.SeekerHead:
                case NPCID.SeekerBody:
                case NPCID.SeekerTail:
                    return SplittingWormLoot(npc, mod, 1);
                case NPCID.DuneSplicerHead:
                case NPCID.DuneSplicerBody:
                case NPCID.DuneSplicerTail:
                    return SplittingWormLoot(npc, mod, 2);
                default:
                    return true;
            }
        }

        internal static bool SplittingWormLoot(NPC npc, Mod mod, int wormType)
        {
            switch (wormType)
            {
                case 0: return CheckSegments(NPCID.DiggerHead, NPCID.DiggerBody, NPCID.DiggerTail);
                case 1: return CheckSegments(NPCID.SeekerHead, NPCID.SeekerBody, NPCID.SeekerTail);
                case 2: return CheckSegments(NPCID.DuneSplicerHead, NPCID.DuneSplicerBody, NPCID.DuneSplicerTail);
                default:
                    break;
            }

            bool CheckSegments(int head, int body, int tail)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (i != npc.whoAmI && Main.npc[i].active && (Main.npc[i].type == head || Main.npc[i].type == body || Main.npc[i].type == tail))
                    {
                        return false;
                    }
                }
                return true;
            }

            return true;
        }
        #endregion

        #region Check Boss Spawn
        // TODO -- not loot code, should be moved eventually
        private void CheckBossSpawn(NPC npc)
        {
            if ((npc.type == ModContent.NPCType<PhantomSpirit>() || npc.type == ModContent.NPCType<PhantomSpiritS>() || npc.type == ModContent.NPCType<PhantomSpiritM>() ||
                npc.type == ModContent.NPCType<PhantomSpiritL>()) && !NPC.AnyNPCs(ModContent.NPCType<Polterghast.Polterghast>()) && !DownedBossSystem.downedPolterghast)
            {
                CalamityMod.ghostKillCount++;
                if (CalamityMod.ghostKillCount == 10)
                {
                    string key = "Mods.CalamityMod.GhostBossText2";
                    Color messageColor = Color.Cyan;

                    CalamityUtils.DisplayLocalizedText(key, messageColor);
                }
                else if (CalamityMod.ghostKillCount == 20)
                {
                    string key = "Mods.CalamityMod.GhostBossText3";
                    Color messageColor = Color.Cyan;

                    CalamityUtils.DisplayLocalizedText(key, messageColor);
                }

                if (CalamityMod.ghostKillCount >= 30 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int lastPlayer = npc.lastInteraction;

                    if (!Main.player[lastPlayer].active || Main.player[lastPlayer].dead)
                    {
                        lastPlayer = npc.FindClosestPlayer();
                    }

                    if (lastPlayer >= 0)
                    {
                        NPC.SpawnOnPlayer(lastPlayer, ModContent.NPCType<Polterghast.Polterghast>());
                        CalamityMod.ghostKillCount = 0;
                    }
                }
            }

            if (NPC.downedPlantBoss && (npc.type == NPCID.SandShark || npc.type == NPCID.SandsharkHallow || npc.type == NPCID.SandsharkCorrupt || npc.type == NPCID.SandsharkCrimson) && !NPC.AnyNPCs(ModContent.NPCType<GreatSandShark.GreatSandShark>()))
            {
                CalamityMod.sharkKillCount++;
                if (CalamityMod.sharkKillCount == 4)
                {
                    string key = "Mods.CalamityMod.SandSharkText";
                    Color messageColor = Color.Goldenrod;

                    CalamityUtils.DisplayLocalizedText(key, messageColor);
                }
                else if (CalamityMod.sharkKillCount == 8)
                {
                    string key = "Mods.CalamityMod.SandSharkText2";
                    Color messageColor = Color.Goldenrod;

                    CalamityUtils.DisplayLocalizedText(key, messageColor);
                }
                if (CalamityMod.sharkKillCount >= 10 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active)
                    {
                        SoundEngine.PlaySound(Mauler.RoarSound, Main.player[Main.myPlayer].position);
                    }

                    int lastPlayer = npc.lastInteraction;

                    if (!Main.player[lastPlayer].active || Main.player[lastPlayer].dead)
                    {
                        lastPlayer = npc.FindClosestPlayer();
                    }

                    if (lastPlayer >= 0)
                    {
                        NPC.SpawnOnPlayer(lastPlayer, ModContent.NPCType<GreatSandShark.GreatSandShark>());
                        CalamityMod.sharkKillCount = -5;
                    }
                }
            }
        }
        #endregion
    }
}

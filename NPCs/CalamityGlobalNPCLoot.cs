using System;
using System.Collections.Generic;
using System.Threading;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.Fishing;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.Tools;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Weapons.Typeless;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Systems.Collections;
using CalamityMod.Tiles.Ores;
using CalamityMod.World;
using CalamityMod.World.Planets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.NPCs.CalamityGlobalTownNPC;

namespace CalamityMod.NPCs
{
    public partial class CalamityGlobalNPC : GlobalNPC
    {
        public static readonly SoundStyle PlagueSound = new("CalamityMod/Sounds/Custom/PlagueUnleash");

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

            // Convenient shortcut for all Master drops moved to Revengeance
            LeadingConditionRule rev = npcLoot.DefineConditionalDropSet(DropHelper.RevNoMaster);

            // Convenient shortcut for all GFB drops
            LeadingConditionRule GFB = npcLoot.DefineConditionalDropSet(DropHelper.GFB);

            LeadingConditionRule pMoon = new LeadingConditionRule(new Conditions.PumpkinMoonDropGatingChance());
            LeadingConditionRule fMoon = new LeadingConditionRule(new Conditions.FrostMoonDropGatingChance());

            // Progression shortcuts
            LeadingConditionRule postEoC = npcLoot.DefineConditionalDropSet(DropHelper.PostEoC());
            LeadingConditionRule hardmode = npcLoot.DefineConditionalDropSet(DropHelper.Hardmode());
            LeadingConditionRule postCal = npcLoot.DefineConditionalDropSet(DropHelper.PostCal());
            LeadingConditionRule postLevi = npcLoot.DefineConditionalDropSet(DropHelper.PostLevi());
            LeadingConditionRule postDoG = npcLoot.DefineConditionalDropSet(DropHelper.PostDoG());

            switch (npc.type)
            {
                #region Surface
                // Goldfish, Walking Goldfish
                // Pineapple Pet @ 0.2%
                case NPCID.Goldfish:
                case NPCID.GoldfishWalker:
                    npcLoot.Add(ModContent.ItemType<PineapplePet>(), 500);
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
                // Essence of Sunlight @ 50% IF Hardmode and not statue spawned
                case NPCID.Harpy:
                    postEoC.Add(ModContent.ItemType<SkyGlaze>(), 30);
                    hardmode.AddIf(() => !npc.SpawnedFromStatue, ModContent.ItemType<EssenceofSunlight>(), 2);
                    break;

                // Angry Nimbus
                // Essence of Sunlight @ 50%
                case NPCID.AngryNimbus:
                    npcLoot.Add(ModContent.ItemType<EssenceofSunlight>(), 2);
                    break;

                // Wyvern Head
                // 8-10 Essence of Sunlight @ 100%, 10-12 Expert+
                // TODO: Move Aero Stone to the upcoming sky structure whenever it's implemented
                // Aero Stone @ 25% Normal, 33.3% Expert+
                case NPCID.WyvernHead:
                    npcLoot.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<EssenceofSunlight>(), 1, 8, 10, 10, 12));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<AeroStone>(), 4, 3));
                    break;
                #endregion

                #region Underground
                // Giant Shelly
                // Giant Shell @ 100% IF Hardmode
                // OTHERWISE,
                // Giant Shell @ 14.29% Normal, 25% Expert+
                case NPCID.GiantShelly:
                case NPCID.GiantShelly2:
                    hardmode.Add(ModContent.ItemType<GiantShell>());
                    hardmode.OnFailedConditions(ItemDropRule.NormalvsExpert(ModContent.ItemType<GiantShell>(), 7, 4));
                    break;

                // Crawdad
                // Craw Carapace @ 100% IF Hardmode
                // OTHERWISE,
                // Craw Carapace @ 14.29% Normal, 25% Expert+
                case NPCID.Crawdad:
                case NPCID.Crawdad2:
                    hardmode.Add(ModContent.ItemType<CrawCarapace>());
                    hardmode.OnFailedConditions(ItemDropRule.NormalvsExpert(ModContent.ItemType<CrawCarapace>(), 7, 4));
                    break;

                // Medusa
                // Pocket Mirror @ 7.14% Normal, 14.29% Expert+ (2.5% Normal, 5% Expert+ in vanilla)
                case NPCID.Medusa:
                    // Remove the vanilla loot rule for Pocket Mirror.
                    npcLoot.RemoveWhere((rule) => rule is CommonDrop conditionalRule && conditionalRule.itemId == ItemID.PocketMirror);

                    // Define a replacement rule which has an increased chance.
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.PocketMirror, 14, 7));
                    break;

                // Tim
                // Plasma Rod @ 33.33% Normal, 50% Expert+
                case NPCID.Tim:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<PlasmaRod>(), 3, 2));
                    break;

                // Skeleton Archer
                // Magic Quiver @ 5% (2.5% in Vanilla)
                // Marrow @ 2% (0.5% in Vanilla)
                case NPCID.SkeletonArcher:
                    npcLoot.ChangeDropRate(ItemID.MagicQuiver, 1, 20);
                    npcLoot.ChangeDropRate(ItemID.Marrow, 1, 50);
                    break;

                // Armored Skeleton
                // Beam Sword @ 2% (0.67% in Vanilla)
                case NPCID.ArmoredSkeleton:
                    npcLoot.ChangeDropRate(ItemID.BeamSword, 1, 50);
                    break;

                // Mimic
                // Drops all of its items Calamity Style @ 25% each
                // This requires erasing its vanilla behavior.
                case NPCID.Mimic:
                    try
                    {
                        npcLoot.RemoveWhere((rule) =>
                        {
                            if (rule is OneFromOptionsDropRule vanillaItems)
                                return vanillaItems.dropIds[0] == ItemID.DualHook;
                            return false;
                        });

                        // Yes, Mimics have three separate loot tables which we must reintegrate
                        int[] normalMimicItems =
                        [
                            ItemID.MagicDagger,
                            ItemID.CrossNecklace,
                            ItemID.PhilosophersStone,
                            ItemID.StarCloak,
                            ItemID.TitanGlove,
                            ItemID.DualHook
                        ];
                        int[] remixPreHardMimicItems =
                        [
                            ItemID.BandofRegeneration,
                            ItemID.MagicMirror,
                            ItemID.CloudinaBottle,
                            ItemID.HermesBoots,
                            ItemID.ShoeSpikes,
                            ItemID.Mace
                        ];
                        int[] remixHardmodeMimicItems =
                        [
                            ItemID.WandofSparking,
                            ItemID.CrossNecklace,
                            ItemID.PhilosophersStone,
                            ItemID.StarCloak,
                            ItemID.TitanGlove,
                            ItemID.DualHook
                        ];

                        // Mimics will not drop any items if spawned from statues.
                        var notRemix = npcLoot.DefineConditionalDropSet(DropHelper.If(() => !(npc.SpawnedFromStatue || Main.remixWorld)));
                        var remixPreHM = npcLoot.DefineConditionalDropSet(DropHelper.If(() => !npc.SpawnedFromStatue && Main.remixWorld && !Main.hardMode));
                        var remixHardmode = npcLoot.DefineConditionalDropSet(DropHelper.If(() => !npc.SpawnedFromStatue && Main.remixWorld && Main.hardMode));
                        notRemix.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, normalMimicItems));
                        remixPreHM.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, remixPreHardMimicItems));
                        remixHardmode.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, remixHardmodeMimicItems));
                    }
                    catch (ArgumentNullException) { }
                    break;
                #endregion

                #region Desert
                // Antlion
                // Antlion Skewer @ 5%
                case NPCID.Antlion:
                    npcLoot.Add(ModContent.ItemType<AntlionSkewer>(), 20);
                    break;

                // Tomb Crawler
                // Burnt Sienna @ 4% Normal, 6.67% Expert+
                case NPCID.TombCrawlerHead:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<BurntSienna>(), 25, 15));
                    break;

                // Desert Spirit
                // Nazar @ 1% Normal, 2% Expert+
                case NPCID.DesertDjinn:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.Nazar, 100, 50));
                    break;

                // Dreamer Ghoul
                // Trifold Map @ 1% Normal, 2% Expert+
                // Light Shard @ 10%
                case NPCID.DesertGhoulHallow:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.TrifoldMap, 100, 50));
                    npcLoot.ChangeDropRate(ItemID.LightShard, 1, 10);
                    break;

                // Crystal Thresher
                // Light Shard @ 10%
                case NPCID.SandsharkHallow:
                    npcLoot.ChangeDropRate(ItemID.LightShard, 1, 10);
                    break;

                // Sand Elemental
                // Elemental in a Bottle @ 20% Normal, 33.33% Expert+
                // Oasis Elemental in a Bottle @ 10% Normal, 16.67% Expert+
                // Desert Key @ 10%
                case NPCID.SandElemental:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<ElementalinaBottle>(), 5, 3));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<OasisElementalinaBottle>(), 10, 6));
                    npcLoot.Add(ItemID.DungeonDesertKey, 10);
                    break;

                // Tainted Ghoul, Vile Ghoul, Bone Biter, Flesh Reaver
                // Dark Shard @ 10%
                case NPCID.DesertGhoulCorruption:
                case NPCID.DesertGhoulCrimson:
                case NPCID.SandsharkCorrupt:
                case NPCID.SandsharkCrimson:
                    npcLoot.ChangeDropRate(ItemID.DarkShard, 1, 10);
                    break;
                #endregion

                #region Ice

                // Ice Bat
                // Frosty Bat Bottle @ 7.14%
                case NPCID.IceBat:
                    npcLoot.Add(ModContent.ItemType<FrostyBatBottle>(), 14);
                    break;

                // Armored Viking
                // Armor Polish @ 1% Normal, 2% Expert
                case NPCID.ArmoredViking:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.ArmorPolish, 100, 50));
                    break;

                // Icy Merman, Icy Tortoise, Ice Elemental, Wolf
                // Essence of Eleum @ 100%
                case NPCID.IcyMerman:
                case NPCID.IceElemental:
                case NPCID.Wolf:
                    npcLoot.Add(ModContent.ItemType<EssenceofEleum>());
                    break;

                // Modify Frozen Turtle Shell drop rate from 2% to 5%
                case NPCID.IceTortoise:
                    npcLoot.Add(ModContent.ItemType<EssenceofEleum>());
                    List<IItemDropRule> rules = npcLoot.Get(false);
                    foreach (IItemDropRule rule in rules)
                    {
                        if (rule is CommonDrop drop && drop.itemId == ItemID.FrozenTurtleShell)
                        {
                            drop.chanceDenominator = 20;
                            return;
                        }
                    }
                    break;

                // Ice Mimic
                // Drops all of its items Calamity Style @ 25% each, Toy Sled is drops on its own at a 5% chance
                // Since one weapon is guaranteed to drop, it is at least 33.33% chance for a specific weapon
                case NPCID.IceMimic:
                    try
                    {
                        npcLoot.RemoveWhere((rule) =>
                        {
                            if (rule is OneFromOptionsDropRule vanillaItems)
                                return vanillaItems.dropIds[0] == ItemID.Frostbrand;
                            return false;
                        });
                        npcLoot.RemoveWhere((rule) =>
                        {
                            if (rule is CommonDrop sledDrop)
                                return sledDrop.itemId == ItemID.ToySled;
                            return false;
                        });

                        // Yes, Ice Mimics have three separate loot tables which me must reintegrate
                        int[] normalIceMimicItems =
                        [
                            ItemID.Frostbrand,
                            ItemID.IceBow,
                            ItemID.FlowerofFrost
                        ];
                        int[] remixPreHardIceMimicItems =
                        [
                            ItemID.IceBoomerang,
                            ItemID.IceBlade,
                            ItemID.IceBow,
                            ItemID.IceSkates,
                            ItemID.BlizzardinaBottle,
                            ItemID.FlurryBoots
                        ];
                        int[] remixHardmodeIceMimicItems =
                        [
                            ItemID.Frostbrand,
                            ItemID.SnowballCannon,
                            ItemID.FlowerofFrost
                        ];

                        // Ice Mimics will not drop any items if spawned from statues.
                        var notRemix = npcLoot.DefineConditionalDropSet(DropHelper.If(() => !(npc.SpawnedFromStatue || Main.remixWorld)));
                        var remixPreHM = npcLoot.DefineConditionalDropSet(DropHelper.If(() => !npc.SpawnedFromStatue && Main.remixWorld && !Main.hardMode));
                        var remixHardmode = npcLoot.DefineConditionalDropSet(DropHelper.If(() => !npc.SpawnedFromStatue && Main.remixWorld && Main.hardMode));
                        notRemix.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, normalIceMimicItems));
                        remixPreHM.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, remixPreHardIceMimicItems));
                        remixHardmode.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, remixHardmodeIceMimicItems));

                        // Independent from all of the Remix madness, Ice Mimics have a 5% chance to drop Toy Sled.
                        npcLoot.DefineConditionalDropSet(new Conditions.NotFromStatue()).Add(ItemID.ToySled, 20);
                    }
                    catch (ArgumentNullException) { }
                    break;

                // Ice Golem
                // 8-10 Essence of Eleum @ 100%, 10-12 Expert+
                // Frozen Key @ 10%
                case NPCID.IceGolem:
                    npcLoot.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<EssenceofEleum>(), 1, 8, 10, 10, 12));
                    npcLoot.Add(ItemID.FrozenKey, 10);
                    break;
                #endregion

                #region Aquatic / Ocean
                // Pink Jellyfish
                // Life Jelly @ 10% Normal, 14.29% Expert+
                // Jellyfish Necklace @ 3.33% (1% in vanilla)
                case NPCID.PinkJellyfish:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<LifeJelly>(), 10, 7));
                    npcLoot.ChangeDropRate(ItemID.JellyfishNecklace, 1, 30);
                    break;

                // Blue Jellyfish
                // Cleansing Jelly @ 10% Normal, 14.29% Expert+
                case NPCID.BlueJellyfish:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<CleansingJelly>(), 10, 7));
                    npcLoot.ChangeDropRate(ItemID.JellyfishNecklace, 1, 30);
                    break;

                // Green Jellyfish
                // Vital Jelly @ 12.5% Normal, 20% Expert+
                case NPCID.GreenJellyfish:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<VitalJelly>(), 8, 5));
                    npcLoot.ChangeDropRate(ItemID.JellyfishNecklace, 1, 30);
                    break;

                // Piranha, Arapaima, Blood Feeder
                // Adhesive Bandage @ 1% Normal, 2% Expert+
                case NPCID.Piranha:
                case NPCID.Arapaima:
                case NPCID.BloodFeeder:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.AdhesiveBandage, 100, 50));
                    break;

                // Shark
                // Shark Tooth Necklace @ 4% Normal, 6.67% Expert+
                // Joyful Heart @ 5%
                // Sharky Plush @ 1%
                case NPCID.Shark:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.SharkToothNecklace, 25, 15));
                    npcLoot.Add(ModContent.ItemType<JoyfulHeart>(), 20);
                    npcLoot.Add(ModContent.ItemType<SharkyPlush>(), 100);
                    break;

                // Blood Jelly, Fungo Fish
                // Jellyfish Necklace @ 3.33%
                case NPCID.BloodJelly:
                case NPCID.FungoFish:
                    npcLoot.Add(ItemID.JellyfishNecklace, 30);
                    break;
                #endregion

                #region Corruption, Crimson, Hallow
                // Face Monster, Devourer
                // Vitamins @ 1% Normal, 2% Expert+
                case NPCID.FaceMonster:
                case NPCID.DevourerHead:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.Vitamins, 100, 50));
                    break;

                // Chaos Elemental
                // Rod of Discord @ 1% Normal, 2% Expert+
                case NPCID.ChaosElemental:
                    // Remove the vanilla loot rule for Rod of Discord.
                    npcLoot.RemoveWhere((rule) => rule is CommonDrop conditionalRule && conditionalRule.itemId == ItemID.RodofDiscord);
                    // Define a replacement rule which has an increased chance.
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.RodofDiscord, 100, 50));
                    break;

                // Illuminant Bat
                // Trifold Map @ 1% Normal, 2% Expert+
                case NPCID.IlluminantBat:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.TrifoldMap, 100, 50));
                    break;

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

                // Corrupt Mimic
                // Celestial Claymore @ 14.29% Normal, 25% Expert+
                case NPCID.BigMimicCorruption:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<CelestialClaymore>(), 7, 4));
                    npcLoot.Add(ItemID.CorruptionKey, 10);
                    break;

                // Crimson Mimic
                // Celestial Claymore @ 14.29% Normal, 25% Expert+
                // Crimson Key @ 10%
                case NPCID.BigMimicCrimson:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<CelestialClaymore>(), 7, 4));
                    npcLoot.Add(ItemID.CrimsonKey, 10);
                    break;

                // Hallowed Mimic
                // Celestial Claymore @ 14.29% Normal, 25% Expert+
                // Hallowed Key @ 10%
                case NPCID.BigMimicHallow:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<CelestialClaymore>(), 7, 4));
                    npcLoot.Add(ItemID.HallowedKey, 10);
                    break;
                #endregion

                #region Jungle
                // Moss Hornet
                // Needler @ 4% Normal, 6.67% Expert+
                // This automatically covers all variations
                case NPCID.MossHornet:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Needler>(), 25, 15));
                    break;

                // Moth
                // Butterfly Dust @ 100% INSTEAD OF 50%
                case NPCID.Moth:
                    npcLoot.ChangeDropRate(ItemID.ButterflyDust, 1, 1);
                    break;

                // Jungle Mimic
                // Celestial Claymore @ 14.29% Normal, 25% Expert+
                case NPCID.BigMimicJungle:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<CelestialClaymore>(), 7, 4));
                    break;

                // Lihzahrd + Flying Snake
                // Jungle Key @ 0.67%
                case NPCID.Lihzahrd:
                case NPCID.LihzahrdCrawler:
                case NPCID.FlyingSnake:
                    npcLoot.Add(ItemID.JungleKey, 150);
                    break;
                #endregion

                #region Dungeon
                // Dark Caster
                // Shinobi Blade @ 4% Normal, 6.67% Expert+
                // Staff of Necrosteocytes @ 4% Normal, 6.67% Expert+
                case NPCID.DarkCaster:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<ShinobiBlade>(), 15, 10));
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<StaffOfNecrosteocytes>(), 15, 10));
                    break;

                // The ectoplasm extra drops got moved to the list section; just like moss hornets

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

                // Giant Cursed Skull
                // Keelhaul @ 6.67% IF Leviathan dead
                case NPCID.GiantCursedSkull:
                    postLevi.Add(ModContent.ItemType<Keelhaul>(), 15);
                    break;

                // Bone Lee
                // Black Belt @ 25% INSTEAD OF 8.33%
                // Tabi @ 25% INSTEAD OF 8.33%
                case NPCID.BoneLee:
                    npcLoot.ChangeDropRate(ItemID.BlackBelt, 1, 4);
                    npcLoot.ChangeDropRate(ItemID.Tabi, 1, 4);
                    break;

                // Necromancers
                // Nazar @ 1% Normal, 2% Expert+
                case NPCID.Necromancer:
                case NPCID.NecromancerArmored:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.FastClock, 100, 50));
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

                // Hellbat
                // Toasty Bat Bottle @ 3.33%
                // Magma Stone @ 1.33% (0.67% in Vanilla)
                case NPCID.Hellbat:
                    npcLoot.Add(ModContent.ItemType<ToastyBatBottle>(), 30);
                    npcLoot.ChangeDropRate(ItemID.MagmaStone, 1, 75);
                    break;

                // Lava Bat
                // Magma Stone @ 3.33% (2% in Vanilla)
                case NPCID.Lavabat:
                    npcLoot.ChangeDropRate(ItemID.MagmaStone, 1, 30);
                    break;

                // Fire Imp
                // Ashen Stalactite @ 4%, 14.28% after defeating EoW/BoC
                case NPCID.FireImp:
                    npcLoot.AddIf((info) => !NPC.downedBoss2, ModContent.ItemType<AshenStalactite>(), 25);
                    npcLoot.AddIf((info) => NPC.downedBoss2, ModContent.ItemType<AshenStalactite>(), 7);
                    break;

                // Demon, Voodoo Demon
                // Bladecrest Oathsword @ 2%, 6.67% after defeating EoW/BoC
                // Demon Scythe @ 5% (2.86% in Vanilla)
                case NPCID.Demon:
                case NPCID.VoodooDemon:
                    npcLoot.AddIf((info) => !NPC.downedBoss2, ModContent.ItemType<BladecrestOathsword>(), 50);
                    npcLoot.AddIf((info) => NPC.downedBoss2, ModContent.ItemType<BladecrestOathsword>(), 15);
                    npcLoot.ChangeDropRate(ItemID.DemonScythe, 1, 20);
                    break;

                // Bone Serpent
                // Old Lord Oathsword @ 4% Normal, 14.29% after defeating EoW/BoC
                case NPCID.BoneSerpentHead:
                    npcLoot.AddIf((info) => !NPC.downedBoss2, ModContent.ItemType<OldLordClaymore>(), 25);
                    npcLoot.AddIf((info) => NPC.downedBoss2, ModContent.ItemType<OldLordClaymore>(), 4);
                    break;

                // Red Devil
                // Fire Feather @ 10% INSTEAD OF 1.33%
                // Abbadon @ 8.33% Normal, 14.29% Expert+
                // Essence of Chaos @ 50%
                case NPCID.RedDevil:
                    npcLoot.ChangeDropRate(ItemID.FireFeather, 1, 10);
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Abaddon>(), 12, 7));
                    npcLoot.Add(ModContent.ItemType<EssenceofHavoc>(), 2);
                    break;
                #endregion

                #region Graveyard
                // Alternate Blood Orb obtainment methods (20%)
                case NPCID.MaggotZombie:
                    postEoC.Add(ModContent.ItemType<BloodOrb>(), 5);
                    break;

                // 100%, 3-6 each
                case NPCID.TheBride:
                case NPCID.TheGroom:
                    postEoC.Add(ModContent.ItemType<BloodOrb>(), 1, 3, 6);
                    break;

                // Ghost Bracelet @ 5% (Dandy requests this drops at a "high chance" but inventory clutter is real so)
                case NPCID.Ghost:
                    npcLoot.Add(ModContent.ItemType<GhostBracelet>(), 20);
                    break;
                #endregion

                #region Blood Moon
                // After EoC, All Blood Moon enemies
                // Drop Blood Orbs @ 100% (25% for common enemies)
                case NPCID.BloodZombie:
                case NPCID.Drippler:
                    postEoC.Add(ModContent.ItemType<BloodOrb>(), 4);
                    break;

                //Hardmode enemies drop orbs even if EoC is not dead
                //99% of players will not encounter this and challenge runners will appreciate it
                case NPCID.Clown:
                    npcLoot.Add(ModContent.ItemType<BloodOrb>(), 1, 6, 12);
                    break;

                // Wandering Eye Fish, Zombie Merman
                // Bouncing Eyeball @ 12.5%
                case NPCID.EyeballFlyingFish:
                case NPCID.ZombieMerman:
                    npcLoot.Add(ModContent.ItemType<BouncingEyeball>(), 8);
                    postEoC.Add(ModContent.ItemType<BloodOrb>(), 1, 10, 12);
                    break;

                case NPCID.GoblinShark:
                case NPCID.BloodEelHead:
                    npcLoot.Add(ModContent.ItemType<BloodOrb>(), 1, 40, 48);
                    break;

                // Dreadnautilus drops the Blood Moon lore
                case NPCID.BloodNautilus:
                    npcLoot.Add(ModContent.ItemType<BloodOrb>(), 1, 100, 120);
                    npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedDreadnautilus, ModContent.ItemType<LoreBloodMoon>(), desc: DropHelper.FirstKillText);
                    break;
                #endregion

                #region Goblin Army
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

                #region Old One's Army
                // Dark Mage T1
                // Dark Mage's Tome drops in Revengeance
                case NPCID.DD2DarkMageT1:
                    rev.Add(ItemID.DarkMageBookMountItem, 4);
                    break;

                // Dark Mage T3
                // Master items drop in Revengeance
                case NPCID.DD2DarkMageT3:
                    rev.Add(ItemID.DarkMageMasterTrophy);
                    rev.Add(ItemID.DarkMageBookMountItem, 4);
                    break;

                // Ogre T3
                // Master items drop in Revengeance
                case NPCID.DD2OgreT3:
                    rev.Add(ItemID.OgreMasterTrophy);
                    rev.Add(ItemID.DD2OgrePetItem, 4);
                    break;
                #endregion

                #region Frost Legion
                // All Frost Legion enemies
                // Essence of Eleum @ 20%
                case NPCID.SnowmanGangsta:
                case NPCID.MisterStabby:
                case NPCID.SnowBalla:
                    npcLoot.Add(ModContent.ItemType<EssenceofEleum>(), 5);
                    break;
                #endregion

                #region Pirate Invasion
                // Pirate deadeye
                // Midas Prime @ 4% Normal, 6.67% Expert+
                case NPCID.PirateDeadeye:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<MidasPrime>(), 25, 15));
                    break;

                // Flying Dutchman
                // Master items drop in Revengeance
                case NPCID.PirateShip:
                    rev.Add(ItemID.FlyingDutchmanMasterTrophy);
                    rev.Add(ItemID.PirateShipMountItem, 4);
                    break;
                #endregion

                #region Solar Eclipse
                // Weak Solar Eclipse Enemies: Frankenstein, Swamp Thing, Fritz, Creature from the Deep
                // Darksun Fragment @ 10% IF Devourer of Gods dead
                case NPCID.Frankenstein:
                case NPCID.SwampThing:
                case NPCID.Fritz:
                case NPCID.CreatureFromTheDeep:
                    postDoG.Add(ModContent.ItemType<DarksunFragment>(), 10);
                    break;

                // Medium Solar Eclipse Enemies: The Possessed, Butcher, Dr. Man Fly
                // Darksun Fragment @ 50% IF Devourer of Gods dead
                case NPCID.ThePossessed:
                case NPCID.Butcher:
                case NPCID.DrManFly:
                    postDoG.Add(ModContent.ItemType<DarksunFragment>(), 2);
                    break;

                // Reaper, Psycho
                // 2-4 Solar Veil @ 50% IF Cal Clone dead
                // Darksun Fragment @ 50% IF Devourer of Gods dead
                case NPCID.Reaper:
                case NPCID.Psycho:
                    postCal.Add(ModContent.ItemType<SolarVeil>(), 2, 2, 4);
                    postDoG.Add(ModContent.ItemType<DarksunFragment>(), 2);
                    break;

                // Vampire / Vampire Bat (same enemy)
                // Moon Stone @ 15% INSTEAD OF 2.86%
                // Bat Hook @ 2.5% Normal, 5% Expert+
                // 2-4 Solar Veil @ 50% IF Cal Clone dead
                // Darksun Fragment @ 50% IF Devourer of Gods dead
                case NPCID.VampireBat:
                case NPCID.Vampire:
                    npcLoot.ChangeDropRate(ItemID.MoonStone, 3, 20);
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.BatHook, 40, 20));
                    postCal.Add(ModContent.ItemType<SolarVeil>(), 2, 2, 4);
                    postDoG.Add(ModContent.ItemType<DarksunFragment>(), 2);
                    break;

                // Eyezor
                // 1-2 Darksun Fragment @ 100% IF Devourer of Gods dead
                case NPCID.Eyezor:
                    postDoG.Add(ModContent.ItemType<DarksunFragment>(), 1, 1, 2);
                    break;

                // Nailhead
                // 3-5 Darksun Fragment @ 100% IF Devourer of Gods dead
                case NPCID.Nailhead:
                    postDoG.Add(ModContent.ItemType<DarksunFragment>(), 1, 3, 5);
                    break;

                // Mothron
                case NPCID.Mothron:
                    // Make all drops accessible pre-Mechs as Mothrons already only spawn post-Plantera (linearity rule removal)
                    // This is identical to vanilla drop rates
                    // This loot is already reported in vanilla regardless of condition so we don't need to report it twice
                    LeadingConditionRule postMechs = new LeadingConditionRule(new Conditions.DownedAllMechBosses());
                    postMechs.OnFailedConditions(ItemDropRule.ExpertGetsRerolls(ItemID.BrokenHeroSword, 4, 1), hideLootReport: true);
                    postMechs.OnFailedConditions(ItemDropRule.ExpertGetsRerolls(ItemID.MothronWings, 20, 1), hideLootReport: true);
                    postMechs.OnFailedConditions(ItemDropRule.ExpertGetsRerolls(ItemID.TheEyeOfCthulhu, 3, 1), hideLootReport: true);
                    npcLoot.Add(postMechs);

                    // 20-30 Darksun Fragment @ 100% IF Devourer of Gods dead
                    postDoG.Add(ModContent.ItemType<DarksunFragment>(), 1, 20, 30);
                    break;

                // Deadly Sphere
                // Defective Sphere @ 4% Normal, 6.67% Expert+
                // Darksun Fragment @ 50% IF Devourer of Gods dead
                case NPCID.DeadlySphere:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<DefectiveSphere>(), 25, 15));
                    postDoG.Add(ModContent.ItemType<DarksunFragment>(), 2);
                    break;
                #endregion

                #region Pumpkin Moon
                // Splinterling
                // Nightmare Fuel @ 50% IF Devourer of Gods dead
                case NPCID.Splinterling:
                    postDoG.Add(ModContent.ItemType<NightmareFuel>(), 2);
                    break;

                // Hellhound, Poltergeist
                // 1-2 Nightmare Fuel @ 50% IF Devourer of Gods dead
                case NPCID.Hellhound:
                case NPCID.Poltergeist:
                    postDoG.Add(ModContent.ItemType<NightmareFuel>(), 2, 1, 2);
                    break;

                // Headless Horseman
                // 3-5 Nightmare Fuel @ 100% IF Devourer of Gods dead
                case NPCID.HeadlessHorseman:
                    postDoG.Add(ModContent.ItemType<NightmareFuel>(), 1, 3, 5);
                    break;

                // Mourning Wood
                case NPCID.MourningWood:
                    // 5-10 Nightmare Fuel @ 100% IF Devourer of Gods dead
                    postDoG.Add(ModContent.ItemType<NightmareFuel>(), 1, 5, 10);

                    // Master items drop in Revengeance
                    pMoon.OnSuccess(ItemDropRule.ByCondition(DropHelper.RevNoMaster, ItemID.MourningWoodMasterTrophy));
                    pMoon.OnSuccess(ItemDropRule.ByCondition(DropHelper.RevNoMaster, ItemID.SpookyWoodMountItem, 4));
                    npcLoot.Add(pMoon);
                    break;

                // Pumpking
                case NPCID.Pumpking:
                    // 10-20 Nightmare Fuel @ 100% IF Devourer of Gods dead
                    postDoG.Add(ModContent.ItemType<NightmareFuel>(), 1, 10, 20);

                    // Master items drop in Revengeance
                    pMoon.OnSuccess(ItemDropRule.ByCondition(DropHelper.RevNoMaster, ItemID.PumpkingMasterTrophy));
                    pMoon.OnSuccess(ItemDropRule.ByCondition(DropHelper.RevNoMaster, ItemID.PumpkingPetItem, 4));
                    npcLoot.Add(pMoon);
                    break;
                #endregion

                #region Frost Moon
                // Weak Frost Moon Enemies: Nutcracker, Elf Copter, Flocko
                // Endothermic Energy @ 50% IF Devourer of Gods dead
                case NPCID.Nutcracker:
                case NPCID.NutcrackerSpinning:
                case NPCID.ElfCopter:
                case NPCID.Flocko:
                    postDoG.Add(ModContent.ItemType<EndothermicEnergy>(), 2);
                    break;

                // Medium Frost Moon Enemies: Krampus, Yeti, Present Mimic
                // 1-2 Endothermic Energy @ 50% IF Devourer of Gods dead
                case NPCID.Krampus:
                case NPCID.Yeti:
                case NPCID.PresentMimic:
                    postDoG.Add(ModContent.ItemType<EndothermicEnergy>(), 2, 1, 2);
                    break;

                // Everscream
                case NPCID.Everscream:
                    // 3-5 Endothermic Energy @ 100% IF Devourer of Gods dead
                    postDoG.Add(ModContent.ItemType<EndothermicEnergy>(), 1, 3, 5);

                    // Master items drop in Revengeance
                    fMoon.OnSuccess(ItemDropRule.ByCondition(DropHelper.RevNoMaster, ItemID.EverscreamMasterTrophy));
                    fMoon.OnSuccess(ItemDropRule.ByCondition(DropHelper.RevNoMaster, ItemID.EverscreamPetItem, 4));
                    npcLoot.Add(fMoon);
                    break;

                // Santa-NK1
                case NPCID.SantaNK1:
                    // 5-10 Endothermic Energy @ 100% IF Devourer of Gods dead
                    postDoG.Add(ModContent.ItemType<EndothermicEnergy>(), 1, 5, 10);

                    // Master items drop in Revengeance
                    fMoon.OnSuccess(ItemDropRule.ByCondition(DropHelper.RevNoMaster, ItemID.SantankMasterTrophy));
                    fMoon.OnSuccess(ItemDropRule.ByCondition(DropHelper.RevNoMaster, ItemID.SantankMountItem, 4));
                    npcLoot.Add(fMoon);
                    break;

                // Ice Queen
                case NPCID.IceQueen:
                    // 10-20 Endothermic Energy @ 100% IF Devourer of Gods dead
                    postDoG.Add(ModContent.ItemType<EndothermicEnergy>(), 1, 10, 20);

                    // Master items drop in Revengeance
                    fMoon.OnSuccess(ItemDropRule.ByCondition(DropHelper.RevNoMaster, ItemID.IceQueenMasterTrophy));
                    fMoon.OnSuccess(ItemDropRule.ByCondition(DropHelper.RevNoMaster, ItemID.IceQueenPetItem, 4));
                    npcLoot.Add(fMoon);
                    break;
                #endregion

                #region Martian Madness
                // Martian Madness On-Foot Soldiers
                // 1% chance to drop any of the three Calamity martian drops
                case NPCID.BrainScrambler:
                case NPCID.GrayGrunt:
                case NPCID.GigaZapper:
                case NPCID.RayGunner:
                case NPCID.ScutlixRider:
                    npcLoot.Add(ModContent.ItemType<DoomsdayDevice>(), 100);
                    npcLoot.Add(ModContent.ItemType<Wingman>(), 100);
                    npcLoot.Add(ModContent.ItemType<NullificationPistol>(), 100);
                    break;

                // Martian Engineer
                // 10% chance to drop Wingman
                case NPCID.MartianEngineer:
                    npcLoot.Add(ModContent.ItemType<Wingman>(), 10);
                    break;

                // Martian Officer
                // 8% chance to drop Shock Grenade
                case NPCID.MartianOfficer:
                    npcLoot.Add(ModContent.ItemType<DoomsdayDevice>(), 12);
                    break;

                // Martian Walker
                // 12% chance to drop Nullification Pistol
                case NPCID.MartianWalker:
                    npcLoot.Add(ModContent.ItemType<NullificationPistol>(), 8);
                    break;

                // Martian Saucer
                case NPCID.MartianSaucerCore:
                    // Drops all of its weapons Calamity Style @ 25% each
                    // This requires erasing its vanilla behavior.
                    try
                    {
                        npcLoot.RemoveWhere((rule) =>
                        {
                            if (rule is OneFromOptionsNotScaledWithLuckDropRule vanillaItems)
                                return vanillaItems.dropIds[0] == ItemID.Xenopopper;
                            return false;
                        });

                        int[] saucerItems = new int[]
                        {
                            ItemID.Xenopopper,
                            ItemID.XenoStaff,
                            ItemID.LaserMachinegun,
                            ItemID.ElectrosphereLauncher,
                            ItemID.InfluxWaver,
                        };

                        npcLoot.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, saucerItems));

                        //Cosmic Car Key is also in the vanilla selection pool. Pull it out.
                        npcLoot.Add(ItemID.CosmicCarKey, 4);
                    }
                    catch (ArgumentNullException) { }

                    // Master items drop in Revengeance
                    rev.Add(ItemID.UFOMasterTrophy);
                    rev.Add(ItemID.MartianPetItem, 4);
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
                    hardmode.Add(ModContent.ItemType<ClothiersWrath>());
                    break;

                // Merchant
                // 20-30 Gel, 30-50 Rope & 10-15 Bombs @ 100% IF named "Morshu"
                case NPCID.Merchant:
                    LeadingConditionRule morshuLCR = new LeadingConditionRule(DropHelper.If((info) => info.npc.GivenName == "Morshu", false));
                    morshuLCR.Add(ItemID.Gel, 1, 20, 30, true); // lamp oil?
                    morshuLCR.Add(ItemID.Rope, 1, 30, 50, true); // rope?
                    morshuLCR.Add(ItemID.Bomb, 1, 10, 15, true); // bombs? you want it?
                    npcLoot.Add(morshuLCR); // it's yours, my friend
                    break;

                // Angler
                // Golden Fishing Rod @ 8.33% IF Hardmode
                case NPCID.Angler:
                    hardmode.Add(ItemID.GoldenFishingRod, 12, 1, 1);
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

                    // 16NOV2025: Ozzatron: following overwhelming dev vote, Expert+ drops are NOT available in Classic
                    // npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.RoyalGel));

                    // Would be in the bag otherwise
                    npcLoot.AddNormalOnly(ItemDropRule.ByCondition(DropHelper.Remix, ItemID.Keybrand, DropHelper.NormalWeaponDropRateInt));
                    npcLoot.AddNormalOnly(ItemDropRule.ByCondition(DropHelper.NotRemix, ItemID.Katana, DropHelper.NormalWeaponDropRateInt));

                    npcLoot.AddNormalOnly(ModContent.ItemType<CrownJewel>(), DropHelper.NormalWeaponDropRateFraction);
                    npcLoot.AddNormalOnly(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                    // Master items drop in Revengeance
                    rev.Add(ItemID.KingSlimeMasterTrophy);
                    rev.Add(ItemID.KingSlimePetItem, 4);

                    // GFB Aureus Cell drop
                    GFB.Add(DropHelper.PerPlayer(ModContent.ItemType<AureusCell>(), 1, 45, 55));

                    // Lore
                    npcLoot.AddConditionalPerPlayer(() => !NPC.downedSlimeKing, ModContent.ItemType<LoreKingSlime>(), desc: DropHelper.FirstKillText);
                    break;

                case NPCID.EyeofCthulhu:
                    // 16NOV2025: Ozzatron: following overwhelming dev vote, Expert+ drops are NOT available in Classic
                    // npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.EoCShield));

                    // Would be in the bag otherwise
                    npcLoot.AddNormalOnly(ModContent.ItemType<TeardropCleaver>(), DropHelper.NormalWeaponDropRateFraction);
                    npcLoot.AddNormalOnly(ModContent.ItemType<DeathstareRod>(), DropHelper.NormalWeaponDropRateFraction);
                    npcLoot.AddNormalOnly(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                    // Master items drop in Revengeance
                    rev.Add(ItemID.EyeofCthulhuMasterTrophy);
                    rev.Add(ItemID.EyeOfCthulhuPetItem, 4);

                    // GFB Optic Staff drop
                    GFB.Add(DropHelper.PerPlayer(ItemID.OpticStaff), hideLootReport: true);

                    // Lore
                    npcLoot.AddConditionalPerPlayer(() => !NPC.downedBoss1, ModContent.ItemType<LoreEyeofCthulhu>(), desc: DropHelper.FirstKillText);
                    break;

                case NPCID.EaterofWorldsHead:
                case NPCID.EaterofWorldsBody:
                case NPCID.EaterofWorldsTail:
                    // 16NOV2025: Ozzatron: following overwhelming dev vote, Expert+ drops are NOT available in Classic
                    LeadingConditionRule EoWKill = new(DropHelper.If((info) => info.npc.boss));
                    // EoWKill.Add(DropHelper.PerPlayer(ItemID.WormScarf));
                    EoWKill.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
                    npcLoot.AddNormalOnly(EoWKill);

                    // Master items drop in Revengeance
                    rev.AddIf((info) => info.npc.boss, ItemID.EaterofWorldsMasterTrophy);
                    rev.AddIf((info) => info.npc.boss, ItemID.EaterOfWorldsPetItem, 4);

                    // GFB Light Disc drop
                    LeadingConditionRule EoWKillGFB = new(DropHelper.If((info) => info.npc.boss));
                    EoWKillGFB.Add(DropHelper.PerPlayer(ItemID.LightDisc), hideLootReport: true);
                    npcLoot.DefineConditionalDropSet(DropHelper.GFB).Add(EoWKillGFB);

                    // Corruption World OR Drunk World: Corruption Lore
                    LeadingConditionRule eowCorruptionLore = new(DropHelper.If((info) => info.npc.boss && (!WorldGen.crimson || WorldGen.drunkWorldGen) && !NPC.downedBoss2, desc: DropHelper.FirstKillText));
                    eowCorruptionLore.Add(ModContent.ItemType<LoreCorruption>(), hideLootReport: WorldGen.crimson && !WorldGen.drunkWorldGen);
                    eowCorruptionLore.Add(ModContent.ItemType<LoreEaterofWorlds>(), hideLootReport: WorldGen.crimson && !WorldGen.drunkWorldGen);
                    npcLoot.Add(eowCorruptionLore);

                    // Crimson World OR Drunk World: Crimson Lore
                    LeadingConditionRule eowCrimsonLore = new(DropHelper.If((info) => info.npc.boss && (WorldGen.crimson || WorldGen.drunkWorldGen) && !NPC.downedBoss2, desc: DropHelper.FirstKillText));
                    eowCrimsonLore.Add(ModContent.ItemType<LoreCrimson>(), hideLootReport: !WorldGen.crimson && !WorldGen.drunkWorldGen);
                    eowCrimsonLore.Add(ModContent.ItemType<LoreBrainofCthulhu>(), hideLootReport: !WorldGen.crimson && !WorldGen.drunkWorldGen);
                    npcLoot.Add(eowCrimsonLore);
                    break;

                case NPCID.BrainofCthulhu:
                    // 16NOV2025: Ozzatron: following overwhelming dev vote, Expert+ drops are NOT available in Classic
                    // npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.BrainOfConfusion));

                    // Would be in the bag otherwise
                    npcLoot.AddNormalOnly(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                    // Master items drop in Revengeance
                    rev.Add(ItemID.BrainofCthulhuMasterTrophy);
                    rev.Add(ItemID.BrainOfCthulhuPetItem, 4);

                    // GFB Occult Skull Crown drop
                    GFB.Add(DropHelper.PerPlayer(ModContent.ItemType<OccultSkullCrown>()), hideLootReport: true);

                    // Corruption World OR Drunk World: Corruption Lore
                    LeadingConditionRule bocCorruptionLore = new(DropHelper.If(() => (!WorldGen.crimson || WorldGen.drunkWorldGen) && !NPC.downedBoss2, desc: DropHelper.FirstKillText));
                    bocCorruptionLore.Add(ModContent.ItemType<LoreCorruption>(), hideLootReport: WorldGen.crimson && !WorldGen.drunkWorldGen);
                    bocCorruptionLore.Add(ModContent.ItemType<LoreEaterofWorlds>(), hideLootReport: WorldGen.crimson && !WorldGen.drunkWorldGen);
                    npcLoot.Add(bocCorruptionLore);

                    // Crimson World OR Drunk World: Crimson Lore
                    LeadingConditionRule bocCrimsonLore = new(DropHelper.If(() => (WorldGen.crimson || WorldGen.drunkWorldGen) && !NPC.downedBoss2, desc: DropHelper.FirstKillText));
                    bocCrimsonLore.Add(ModContent.ItemType<LoreCrimson>(), hideLootReport: !WorldGen.crimson && !WorldGen.drunkWorldGen);
                    bocCrimsonLore.Add(ModContent.ItemType<LoreBrainofCthulhu>(), hideLootReport: !WorldGen.crimson && !WorldGen.drunkWorldGen);
                    npcLoot.Add(bocCrimsonLore);
                    break;

                case NPCID.Deerclops:
                    // Remove the vanilla loot rule for Deerclops' weapon drops.
                    try
                    {
                        var deerRootRules = npcLoot.Get(false);
                        IItemDropRule notExpert = deerRootRules.Find((rule) => rule is LeadingConditionRule deerLCR && deerLCR.condition is Conditions.NotExpert);
                        if (notExpert is LeadingConditionRule LCR_NotExpert)
                        {
                            LCR_NotExpert.ChainedRules.RemoveAll((chainAttempt) =>
                                chainAttempt is Chains.TryIfSucceeded c && c.RuleToChain is OneFromRulesRule weapons);

                            // Define a replacement rule which drops the weapons Calamity style.
                            var deerWeapons = new int[]
                            {
                                ItemID.LucyTheAxe,
                                ItemID.PewMaticHorn,
                                ItemID.WeatherPain,
                                ItemID.HoundiusShootius
                            };
                            LCR_NotExpert.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, deerWeapons));
                        }
                    }
                    catch (ArgumentNullException) { }

                    // 16NOV2025: Ozzatron: following overwhelming dev vote, Expert+ drops are NOT available in Classic
                    // npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.BoneHelm));

                    // Would be in the bag otherwise
                    npcLoot.AddNormalOnly(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                    // Master items drop in Revengeance
                    rev.Add(ItemID.DeerclopsMasterTrophy);
                    rev.Add(ItemID.DeerclopsPetItem, 4);
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
                    npcLoot.AddNormalOnly(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, ItemID.BeeKeeper, ItemID.BeesKnees, ItemID.BeeGun, ModContent.ItemType<HardenedHoneycomb>()));

                    // 16NOV2025: Ozzatron: following overwhelming dev vote, Expert+ drops are NOT available in Classic
                    // npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.HiveBackpack));

                    npcLoot.AddNormalOnly(ModContent.ItemType<TheBee>(), DropHelper.NormalWeaponDropRateFraction);

                    // Would be in the bag otherwise
                    npcLoot.AddNormalOnly(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                    // Queen Bee drops Stingers in Calamity
                    npcLoot.Add(ItemID.Stinger, 1, 8, 12);

                    // Master items drop in Revengeance
                    rev.Add(ItemID.QueenBeeMasterTrophy);
                    rev.Add(ItemID.QueenBeePetItem, 4);

                    // GFB Lavaproof Bug Net and Alchemical Flask drop
                    GFB.Add(DropHelper.PerPlayer(ItemID.FireproofBugNet), hideLootReport: true);
                    GFB.Add(DropHelper.PerPlayer(ModContent.ItemType<AlchemicalDecanter>()), hideLootReport: true);

                    // Lore
                    npcLoot.AddConditionalPerPlayer(() => !NPC.downedQueenBee, ModContent.ItemType<LoreQueenBee>(), desc: DropHelper.FirstKillText);
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

                    // 16NOV2025: Ozzatron: following overwhelming dev vote, Expert+ drops are NOT available in Classic
                    // npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.BoneGlove));

                    // Would be in the bag otherwise
                    npcLoot.AddNormalOnly(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                    // Master items drop in Revengeance
                    rev.Add(ItemID.SkeletronMasterTrophy);
                    rev.Add(ItemID.SkeletronPetItem, 4);

                    // GFB Flamethrower drop
                    GFB.Add(DropHelper.PerPlayer(ItemID.Flamethrower), hideLootReport: true);

                    // Lore
                    npcLoot.AddConditionalPerPlayer(() => !NPC.downedBoss3, ModContent.ItemType<LoreSkeletron>(), desc: DropHelper.FirstKillText);
                    break;

                case NPCID.WallofFlesh:
                    // Remove the vanilla loot rule for Wall of Flesh's weapon drops.
                    try
                    {
                        var wofRootRules = npcLoot.Get(false);
                        // Emblem rule happens first, so use FindLast
                        IItemDropRule notExpert = wofRootRules.FindLast((rule) => rule is LeadingConditionRule wofLCR && wofLCR.condition is Conditions.NotExpert);
                        if (notExpert is LeadingConditionRule LCR_NotExpert)
                        {
                            LCR_NotExpert.ChainedRules.RemoveAll((chainAttempt) =>
                                chainAttempt is Chains.TryIfSucceeded c && c.RuleToChain is OneFromOptionsNotScaledWithLuckDropRule weapons && weapons.dropIds[0] == ItemID.BreakerBlade);

                            // Define a replacement rule which drops the weapons Calamity style.
                            var wofWeapons = new int[]
                            {
                                ItemID.BreakerBlade,
                                ModContent.ItemType<Carnage>(),
                                ItemID.ClockworkAssaultRifle,
                                ModContent.ItemType<Meowthrower>(),
                                ItemID.LaserRifle,
                                ModContent.ItemType<BlackHawkRemote>(),
                                ItemID.FireWhip, // Firecracker
                                ModContent.ItemType<BlastBarrel>(),
                            };
                            LCR_NotExpert.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, wofWeapons));
                        }
                    }
                    catch (ArgumentNullException) { }

                    // Remove the vanilla loot rule for Wall of Flesh's emblem drops.
                    try
                    {
                        var wofRootRules = npcLoot.Get(false);
                        IItemDropRule notExpert = wofRootRules.Find((rule) => rule is LeadingConditionRule wofLCR && wofLCR.condition is Conditions.NotExpert);
                        if (notExpert is LeadingConditionRule LCR_NotExpert)
                        {
                            LCR_NotExpert.ChainedRules.RemoveAll((chainAttempt) =>
                                chainAttempt is Chains.TryIfSucceeded c && c.RuleToChain is OneFromOptionsNotScaledWithLuckDropRule emblems && emblems.dropIds[0] == ItemID.WarriorEmblem);

                            // Define a replacement rule which drops the emblems Calamity style.
                            var wofEmblems = new int[]
                            {
                                ItemID.WarriorEmblem,
                                ItemID.RangerEmblem,
                                ItemID.SorcererEmblem,
                                ItemID.SummonerEmblem,
                                ModContent.ItemType<RogueEmblem>(),
                            };
                            LCR_NotExpert.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, wofEmblems));
                        }
                    }
                    catch (ArgumentNullException) { }

                    // Drop Hermit's Box directly for EACH player, regardles of Expert or not. 100% chance on first kill, 10% chance afterwards.
                    // The special first kill rule is unlisted in the Bestiary
                    LeadingConditionRule firstWoFKill = new(DropHelper.If(() => !Main.hardMode));
                    firstWoFKill.Add(DropHelper.PerPlayer(ModContent.ItemType<HermitsBoxofOneHundredMedicines>()), hideLootReport: true);
                    npcLoot.Add(firstWoFKill);
                    LeadingConditionRule subsequentWoFKills = new(DropHelper.If(() => Main.hardMode));
                    subsequentWoFKills.Add(DropHelper.PerPlayer(ModContent.ItemType<HermitsBoxofOneHundredMedicines>(), 10));
                    npcLoot.Add(subsequentWoFKills);

                    // Expert+ drops are also available on Normal
                    // However, Demon Heart does not work in Normal mode, so it's best to not drop it
                    // npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.DemonHeart));

                    // Would be in the bag otherwise
                    npcLoot.AddNormalOnly(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                    // Master items drop in Revengeance
                    rev.Add(ItemID.WallofFleshMasterTrophy);
                    rev.Add(ItemID.WallOfFleshGoatMountItem, 4);

                    // GFB Eye of Magnus and Flesh Wall drops
                    GFB.Add(DropHelper.PerPlayer(ModContent.ItemType<EyeofMagnus>()), hideLootReport: true);
                    GFB.Add(DropHelper.PerPlayer(ItemID.FleshBlockWall), hideLootReport: true);

                    // Lore
                    npcLoot.AddConditionalPerPlayer(() => !Main.hardMode, ModContent.ItemType<LoreUnderworld>(), desc: DropHelper.FirstKillText);
                    npcLoot.AddConditionalPerPlayer(() => !Main.hardMode, ModContent.ItemType<LoreWallofFlesh>(), desc: DropHelper.FirstKillText);
                    break;

                case NPCID.QueenSlimeBoss:
                    // 16NOV2025: Ozzatron: following overwhelming dev vote, Expert+ drops are NOT available in Classic
                    // npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.VolatileGelatin));

                    // Would be in the bag otherwise
                    npcLoot.AddNormalOnly(ItemID.SoulofLight, 1, 15, 20);
                    npcLoot.AddNormalOnly(ItemID.PinkGel, 1, 15, 20);
                    npcLoot.AddNormalOnly(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                    // Master items drop in Revengeance
                    rev.Add(ItemID.QueenSlimeMasterTrophy);
                    rev.Add(ItemID.QueenSlimePetItem, 4);

                    // GFB Bottomless Shimmer Bucket drop
                    GFB.Add(DropHelper.PerPlayer(ItemID.BottomlessShimmerBucket), hideLootReport: true);

                    // Lore
                    npcLoot.AddConditionalPerPlayer(() => !NPC.downedQueenSlime, ModContent.ItemType<LoreQueenSlime>(), desc: DropHelper.FirstKillText);

                    break;

                case NPCID.TheDestroyer:
                    // Remove the vanilla loot rule for Hallowed Bars.
                    npcLoot.RemoveWhere((rule) => rule is ItemDropWithConditionRule conditionalRule && conditionalRule.itemId == ItemID.HallowedBar);

                    // Define a replacement rule which respects the Early Hardmode Progression Rework.
                    npcLoot.AddNormalOnly(ItemDropRule.ByCondition(DropHelper.HallowedBarsCondition, ItemID.HallowedBar, 1, 15, 30));

                    // 16NOV2025: Ozzatron: following overwhelming dev vote, Expert+ drops are NOT available in Classic
                    // npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.MechanicalWagonPiece));

                    // Would be in the bag otherwise
                    npcLoot.AddNormalOnly(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                    // Master items drop in Revengeance
                    rev.Add(ItemID.DestroyerMasterTrophy);
                    rev.Add(ItemID.DestroyerPetItem, 4);

                    // GFB Bloodworm drop
                    GFB.Add(DropHelper.PerPlayer(ModContent.ItemType<BloodwormItem>()), hideLootReport: true);

                    // Lore
                    npcLoot.AddConditionalPerPlayer(() => !NPC.downedMechBoss1, ModContent.ItemType<LoreDestroyer>(), desc: DropHelper.FirstKillText);
                    npcLoot.AddConditionalPerPlayer(ShouldDropMechLore, ModContent.ItemType<LoreMechs>(), desc: DropHelper.MechBossText);
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

                    // 16NOV2025: Ozzatron: following overwhelming dev vote, Expert+ drops are NOT available in Classic
                    // These are done manually due to Last Twin Standing.
                    // npcLoot.AddIf((info) => !Main.expertMode && IsLastTwinStanding(info), ItemID.MechanicalWheelPiece);

                    // Would be in the bag otherwise
                    npcLoot.AddIf((info) => !Main.expertMode && IsLastTwinStanding(info), ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                    // Master items drop in Revengeance
                    rev.AddIf((info) => IsLastTwinStanding(info), ItemID.TwinsMasterTrophy);
                    rev.AddIf((info) => IsLastTwinStanding(info), ItemID.TwinsPetItem, 4);

                    // GFB The Eye of Cthulhu drop
                    LeadingConditionRule TwinsKillGFB = new(DropHelper.If((info) => IsLastTwinStanding(info)));
                    TwinsKillGFB.Add(DropHelper.PerPlayer(ItemID.TheEyeOfCthulhu), hideLootReport: true);
                    npcLoot.DefineConditionalDropSet(DropHelper.GFB).Add(TwinsKillGFB);

                    // Lore
                    npcLoot.AddConditionalPerPlayer((info) => !NPC.downedMechBoss2 && IsLastTwinStanding(info), ModContent.ItemType<LoreTwins>(), desc: DropHelper.FirstKillText);
                    npcLoot.AddConditionalPerPlayer(ShouldDropMechLore, ModContent.ItemType<LoreMechs>(), desc: DropHelper.MechBossText);
                    break;

                case NPCID.SkeletronPrime:
                    // Remove the vanilla loot rule for Hallowed Bars.
                    npcLoot.RemoveWhere((rule) => rule is ItemDropWithConditionRule conditionalRule && conditionalRule.itemId == ItemID.HallowedBar);

                    // Define a replacement rule which respects the Early Hardmode Progression Rework.
                    npcLoot.AddNormalOnly(ItemDropRule.ByCondition(DropHelper.HallowedBarsCondition, ItemID.HallowedBar, 1, 15, 30));

                    // 16NOV2025: Ozzatron: following overwhelming dev vote, Expert+ drops are NOT available in Classic
                    // npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.MechanicalBatteryPiece));

                    // Would be in the bag otherwise
                    npcLoot.AddNormalOnly(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                    // Master items drop in Revengeance
                    rev.Add(ItemID.SkeletronPrimeMasterTrophy);
                    rev.Add(ItemID.SkeletronPrimePetItem, 4);

                    // GFB Bone Wings drop
                    GFB.Add(DropHelper.PerPlayer(ItemID.BoneWings), hideLootReport: true);

                    // Lore
                    npcLoot.AddConditionalPerPlayer(() => !NPC.downedMechBoss3, ModContent.ItemType<LoreSkeletronPrime>(), desc: DropHelper.FirstKillText);
                    npcLoot.AddConditionalPerPlayer(ShouldDropMechLore, ModContent.ItemType<LoreMechs>(), desc: DropHelper.MechBossText);
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

                    // 16NOV2025: Ozzatron: following overwhelming dev vote, Expert+ drops are NOT available in Classic
                    // npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.SporeSac));

                    // Would be in the bag otherwise
                    npcLoot.AddNormalOnly(ModContent.ItemType<BloomStone>(), 10);
                    npcLoot.AddNormalOnly(ModContent.ItemType<BlossomFlux>(), 10);
                    npcLoot.AddNormalOnly(DropHelper.PerPlayer(ModContent.ItemType<LivingShard>(), 1, 30, 40));
                    npcLoot.AddNormalOnly(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                    // Master items drop in Revengeance
                    rev.Add(ItemID.PlanteraMasterTrophy);
                    rev.Add(ItemID.PlanteraPetItem, 4);

                    // GFB Life Fruit drop
                    GFB.Add(DropHelper.PerPlayer(ItemID.LifeFruit, 1, 1, 9999), true);

                    // Lore
                    npcLoot.AddConditionalPerPlayer(() => !NPC.downedPlantBoss, ModContent.ItemType<LorePlantera>(), desc: DropHelper.FirstKillText);
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

                            // Remove the vanilla loot rule for Picksaw because it has its own drop rule set below.
                            golemRootRules.RemoveAll((rule) =>
                                rule is ItemDropWithConditionRule conditionalRule && conditionalRule.condition is Conditions.NotExpert && conditionalRule.itemId == ItemID.Picksaw);
                        }
                    }
                    catch (ArgumentNullException) { }

                    var normalOnly = npcLoot.DefineNormalOnlyDropSet();

                    // 16NOV2025: Ozzatron: following overwhelming dev vote, Expert+ drops are NOT available in Classic
                    // normalOnly.Add(DropHelper.PerPlayer(ItemID.ShinyStone));

                    // Would be in the bag otherwise
                    normalOnly.Add(ModContent.ItemType<EssenceofSunlight>(), 1, 8, 10);
                    normalOnly.Add(ModContent.ItemType<AegisBlade>(), 10);
                    npcLoot.AddNormalOnly(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                    // If Golem has never been killed, provide a Picksaw to all players.
                    LeadingConditionRule firstGolemKill = new(DropHelper.If(() => !NPC.downedGolemBoss));
                    firstGolemKill.Add(DropHelper.PerPlayer(ItemID.Picksaw));
                    npcLoot.Add(firstGolemKill);

                    // Master items drop in Revengeance
                    rev.Add(ItemID.GolemMasterTrophy);
                    rev.Add(ItemID.GolemPetItem, 4);

                    // GFB Vision Potion drop
                    GFB.Add(DropHelper.PerPlayer(ItemID.NightOwlPotion, 1, 1, 9999), true);
                    GFB.Add(DropHelper.PerPlayer(ItemID.ShinePotion, 1, 1, 9999), true);
                    GFB.Add(DropHelper.PerPlayer(ItemID.HunterPotion, 1, 1, 9999), true);
                    GFB.Add(DropHelper.PerPlayer(ItemID.TrapsightPotion, 1, 1, 9999), true);
                    GFB.Add(DropHelper.PerPlayer(ItemID.SpelunkerPotion, 1, 1, 9999), true);
                    GFB.Add(DropHelper.PerPlayer(ModContent.ItemType<PotionofOmniscience>(), 1, 1, 9999), true);

                    // Lore
                    npcLoot.AddConditionalPerPlayer(() => !NPC.downedGolemBoss, ModContent.ItemType<LoreGolem>(), desc: DropHelper.FirstKillText);
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

                    // Master items drop in Revengeance
                    rev.Add(ItemID.BetsyMasterTrophy);
                    rev.Add(ItemID.DD2BetsyPetItem, 4);

                    break;

                case NPCID.DukeFishron:
                    // Remove the vanilla loot rule for Duke Fishron's weapon drops. This requires digging through his loot rule tree.
                    try
                    {
                        // Remove the vanilla loot rule for Fishron Wings because it's part of the Calamity Style set.
                        npcLoot.RemoveWhere((rule) => rule is ItemDropWithConditionRule conditionalRule && conditionalRule.itemId == ItemID.FishronWings);

                        // 02JAN2024: Ozzatron: Fixed silent breakage of Duke Fishron's weapon drops alteration caused by the 1.4.4 port and the Remix seed.
                        var dukeRootRules = npcLoot.Get(false);
                        IItemDropRule notRemix = dukeRootRules.Find((rule) => rule is LeadingConditionRule dukeLCR && dukeLCR.condition is Conditions.NotRemixSeed);
                        if (notRemix is not LeadingConditionRule LCR_NotRemix)
                            goto DukeEditFailed;
                        var chain = notRemix.ChainedRules.Find((chain) => chain.RuleToChain is LeadingConditionRule dukeLCR2 && dukeLCR2.condition is Conditions.NotExpert);
                        IItemDropRule notExpert = chain.RuleToChain;
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
                    }
                    catch (ArgumentNullException) { }
DukeEditFailed:

// 16NOV2025: Ozzatron: following overwhelming dev vote, Expert+ drops are NOT available in Classic
// npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.ShrimpyTruffle));

// Would be in the bag otherwise
                    npcLoot.AddNormalOnly(ModContent.ItemType<BrinyBaron>(), 10);
                    npcLoot.AddNormalOnly(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                    // Master items drop in Revengeance
                    rev.Add(ItemID.DukeFishronMasterTrophy);
                    rev.Add(ItemID.DukeFishronPetItem, 4);

                    // GFB Old Die and Fish drop
                    GFB.Add(DropHelper.PerPlayer(ModContent.ItemType<OldDie>()), hideLootReport: true);
                    GFB.Add(DropHelper.PerPlayer(ItemID.Fish), hideLootReport: true);
                    GFB.Add(DropHelper.PerPlayer(ItemID.FishBowl), hideLootReport: true);
                    GFB.Add(DropHelper.PerPlayer(ItemID.FishCostumeFinskirt), hideLootReport: true);
                    GFB.Add(DropHelper.PerPlayer(ItemID.FishCostumeMask), hideLootReport: true);
                    GFB.Add(DropHelper.PerPlayer(ItemID.FishCostumeShirt), hideLootReport: true);
                    GFB.Add(DropHelper.PerPlayer(ItemID.FishermansGuide), hideLootReport: true);
                    GFB.Add(DropHelper.PerPlayer(ItemID.FisherofSouls), hideLootReport: true);
                    GFB.Add(DropHelper.PerPlayer(ItemID.FishFinder), hideLootReport: true);
                    GFB.Add(DropHelper.PerPlayer(ItemID.FishHook), hideLootReport: true);
                    GFB.Add(DropHelper.PerPlayer(ItemID.FishingBobber), hideLootReport: true);
                    GFB.Add(DropHelper.PerPlayer(ItemID.FishingPotion, 1, 1, 9999), true);
                    GFB.Add(DropHelper.PerPlayer(ItemID.FishingSeaweed, 1, 1, 9999), true);
                    GFB.Add(DropHelper.PerPlayer(ItemID.FishMinecart), hideLootReport: true);
                    GFB.Add(DropHelper.PerPlayer(ItemID.Fishotron), hideLootReport: true);
                    GFB.Add(DropHelper.PerPlayer(ItemID.Fishron), hideLootReport: true);
                    GFB.Add(DropHelper.PerPlayer(ItemID.FishStatue, 1, 1, 9999), true);
                    GFB.Add(DropHelper.PerPlayer(ModContent.ItemType<FishboneBoomerang>()), hideLootReport: true);
                    GFB.Add(DropHelper.PerPlayer(ModContent.ItemType<FishofEleum>(), 1, 1, 9999), true);
                    GFB.Add(DropHelper.PerPlayer(ModContent.ItemType<FishofFlight>(), 1, 1, 9999), true);

                    // Lore
                    npcLoot.AddConditionalPerPlayer(() => !NPC.downedFishron, ModContent.ItemType<LoreDukeFishron>(), desc: DropHelper.FirstKillText);
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
                                ItemID.PiercingStarlight, // Starlight
                                ItemID.FairyQueenRangedItem, // Eventide
                                ItemID.FairyQueenMagicItem, // Nightglow
                                ItemID.SparkleGuitar, // Stellar Tune
                                ItemID.RainbowWhip, // Kaleidoscope
                                ItemID.RainbowWings // Empress Wings
                            };
                            LCR_NotExpert.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, empressItems));

                            // Remove rules for Empress Wings and Stellar Tune because they're in the Calamity Style set.
                            LCR_NotExpert.ChainedRules.RemoveAll((chain) =>
                            {
                                var rule = chain.RuleToChain;
                                return rule is CommonDrop c && (c.itemId == ItemID.RainbowWings || c.itemId == ItemID.SparkleGuitar);
                            });
                        }
                    }
                    catch (ArgumentNullException) { }

                    // 16NOV2025: Ozzatron: following overwhelming dev vote, Expert+ drops are NOT available in Classic
                    // npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.EmpressFlightBooster));

                    // Would be in the bag otherwise
                    npcLoot.AddNormalOnly(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                    // Master items drop in Revengeance
                    rev.Add(ItemID.FairyQueenMasterTrophy);
                    rev.Add(ItemID.FairyQueenPetItem, 4);

                    // GFB Purple Haze and Terraformer drop
                    GFB.Add(DropHelper.PerPlayer(ModContent.ItemType<PurpleHaze>(), 1, 1, 9999), true);
                    GFB.Add(DropHelper.PerPlayer(ItemID.Clentaminator2), hideLootReport: true);

                    // Lore
                    npcLoot.AddConditionalPerPlayer(() => !NPC.downedEmpressOfLight, ModContent.ItemType<LoreEmpressofLight>(), desc: DropHelper.FirstKillText);

                    break;

                case NPCID.CultistBoss:
                    // Master items drop in Revengeance
                    rev.Add(ItemID.LunaticCultistMasterTrophy);
                    rev.Add(ItemID.LunaticCultistPetItem, 4);

                    npcLoot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                    // GFB Luminite Brick drop
                    GFB.Add(DropHelper.PerPlayer(ItemID.LunarBrick, 1, 1, 9999), true);

                    // Lore
                    npcLoot.AddConditionalPerPlayer(() => !NPC.downedAncientCultist, ModContent.ItemType<LorePrelude>(), desc: DropHelper.FirstKillText);
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

                    // 16NOV2025: Ozzatron: following overwhelming dev vote, Expert+ drops are NOT available in Classic
                    // npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.GravityGlobe));
                    // npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.SuspiciousLookingTentacle));
                    // npcLoot.AddNormalOnly(DropHelper.PerPlayer(ItemID.LongRainbowTrailWings));

                    // Would be in the bag otherwise
                    // 16NOV2025: Ozzatron (executive decision): Celestial Onion remains available as a 6th slot in Classic.
                    npcLoot.AddNormalOnly(DropHelper.PerPlayer(ModContent.ItemType<CelestialOnion>()));
                    npcLoot.AddNormalOnly(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                    // Master items drop in Revengeance
                    rev.Add(ItemID.MoonLordMasterTrophy);
                    rev.Add(ItemID.MoonLordPetItem, 4);

                    // GFB Calamari's Lament drop
                    GFB.Add(DropHelper.PerPlayer(ModContent.ItemType<CalamarisLament>()), hideLootReport: true);

                    // Lore
                    npcLoot.AddConditionalPerPlayer(() => !NPC.downedMoonlord, ModContent.ItemType<LoreRequiem>(), desc: DropHelper.FirstKillText);
                    break;

                default:
                    break;
                    #endregion
            }
            //If the enemy is part of a list, place it here as in the section before it no longer works
            #region Enemy Lists
            // All Skeletons
            // Ancient Bone Dust @ 20% Normal, 33.33% Expert+
            if (CalamityNPCTypeSets.Skeleton[npc.type])
                npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<AncientBoneDust>(), 5, 3));

            // All Hardmode Dungeon Enemies
            // Ectoplasm @ 20%
            if (CalamityNPCSets.IsBuffedDungeonEnemy[npc.type])
                npcLoot.Add(ItemID.Ectoplasm, 5);
            #endregion
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
        }
        #endregion

        #region Pre Kill
        public override bool PreKill(NPC npc)
        {
            if (npc.Calamity().preventDrops)
            {
                // This NEEDS to have an exception to function, so I put a very unlikely item to ever drop from an enemy
                DropHelper.BlockEverything(ModContent.ItemType<AbyssalWarhammer>());
                npc.value = 0;
            }

            // Stop Eater of Worlds segments and Brain of Cthulhu Creepers from dropping partial loot in Rev+
            if (CalamityWorld.revenge && (CalamityNPCTypeSets.EaterOfWorlds.Contains(npc.type) || npc.type == NPCID.Creeper))
                DropHelper.BlockDrops(ItemID.DemoniteOre, ItemID.ShadowScale, ItemID.CrimtaneOre, ItemID.TissueSample);

            // Boss Rush pre-kill effects
            if (BossRushEvent.BossRushActive)
            {
                // Block anything except the Rock from dropping
                DropHelper.BlockEverything(ModContent.ItemType<Rock>());
            }
            return true;
        }
        #endregion

        #region On Kill Main Hook
        public override void OnKill(NPC npc)
        {
            if (npc.Calamity().coinDropMult != 1)
                npc.value = (int)(npc.value * npc.Calamity().coinDropMult);

            // Boss Rush on-kill effects
            if (BossRushEvent.BossRushActive)
            {
                // Progress the Boss Rush event
                BossRushEvent.OnBossKill(npc, Mod);
            }

            // Acid Rain on-kill effects
            if (AcidRainEvent.AcidRainEventIsOngoing)
                AcidRainEvent.OnEnemyKill(npc);

            // Determine whether this NPC is the second Twin killed in a fight, regardless of which Twin it is.
            bool lastTwinStanding = false;
            if (npc.type == NPCID.Retinazer)
                lastTwinStanding = !NPC.AnyNPCs(NPCID.Spazmatism);
            else if (npc.type == NPCID.Spazmatism)
                lastTwinStanding = !NPC.AnyNPCs(NPCID.Retinazer);

            // On-kill NON-LOOT behavior for Eater of Worlds
            if ((npc.boss && (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail)) || npc.type == NPCID.BrainofCthulhu)
            {
                SetNewShopVariable(new int[] { NPCID.Merchant, NPCID.ArmsDealer, NPCID.Dryad }, NPC.downedBoss2);
                SetNewBossJustDowned(npc);
            }

            // On-kill NON-LOOT behavior for every other vanilla boss (and Dreadnautilus)
            switch (npc.type)
            {
                case NPCID.KingSlime:
                    SetNewShopVariable(new int[] { NPCID.Dryad }, NPC.downedSlimeKing);
                    SetNewBossJustDowned(npc);
                    break;

                case NPCID.EyeofCthulhu:
                    SetNewShopVariable(new int[] { NPCID.Merchant, NPCID.Dryad }, NPC.downedBoss1);
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
                    SetNewShopVariable(new int[] { NPCID.Merchant, NPCID.Dryad }, NPC.downedBoss3);
                    SetNewBossJustDowned(npc);

                    // First kill: Notify of Abyss chests being unlocked.
                    if (!NPC.downedBoss3 && !BossRushEvent.BossRushActive)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            World.Abyss.UnlockAllAbyssChests();
                        }
                        string keysk = "Mods.CalamityMod.Status.Progression.SkeletronAbyssChestNotification";
                        CalamityUtils.BroadcastLocalizedText(keysk, new Color(76, 181, 76));
                    }
                    break;

                case NPCID.WallofFlesh:
                    SetNewShopVariable(new int[] { NPCID.Merchant, NPCID.ArmsDealer, NPCID.Dryad, NPCID.Painter, NPCID.WitchDoctor, NPCID.Stylist, NPCID.DyeTrader, NPCID.Demolitionist, NPCID.PartyGirl, NPCID.Clothier, NPCID.SkeletonMerchant }, Main.hardMode);
                    SetNewBossJustDowned(npc);

                    if (!Main.hardMode && !BossRushEvent.BossRushActive)
                    {
                        // Increase altar count to allow natural mech boss spawning.
                        if (CalamityServerConfig.Instance.EarlyHardmodeProgressionRework)
                            WorldGen.altarCount++;

                        string key2 = "Mods.CalamityMod.Status.Progression.UglyBossText";
                        Color messageColor2 = Color.Aquamarine;
                        CalamityUtils.BroadcastLocalizedText(key2, messageColor2);
                    }
                    break;

                case NPCID.BloodNautilus:
                    // Mark Dreadnautilus as dead (Vanilla does not keep track of it)
                    DownedBossSystem.downedDreadnautilus = true;
                    CalamityNetcode.SyncWorld();
                    break;

                case NPCID.QueenSlimeBoss:
                    SetNewBossJustDowned(npc);
                    break;

                case NPCID.TheDestroyer:
                    SetNewShopVariable(new int[] { NPCID.Demolitionist, NPCID.DD2Bartender, NPCID.Stylist, NPCID.Truffle }, NPC.downedMechBossAny);
                    SetNewShopVariable(new int[] { NPCID.Stylist, ModContent.NPCType<Archmage>(), ModContent.NPCType<Bandit>() }, NPC.downedMechBoss1 || !NPC.downedMechBoss2 || !NPC.downedMechBoss3);
                    SetNewBossJustDowned(npc);

                    if (!NPC.downedMechBoss1 && CalamityServerConfig.Instance.EarlyHardmodeProgressionRework && !BossRushEvent.BossRushActive)
                        SpawnMechBossHardmodeOres();
                    break;

                case NPCID.Spazmatism:
                case NPCID.Retinazer:
                    if (lastTwinStanding)
                    {
                        SetNewShopVariable(new int[] { NPCID.Demolitionist, NPCID.DD2Bartender, NPCID.Stylist, NPCID.Truffle }, NPC.downedMechBossAny);
                        SetNewShopVariable(new int[] { NPCID.Stylist, ModContent.NPCType<Archmage>(), ModContent.NPCType<Bandit>() }, !NPC.downedMechBoss1 || NPC.downedMechBoss2 || !NPC.downedMechBoss3);
                        SetNewBossJustDowned(npc);

                        if (!NPC.downedMechBoss2 && CalamityServerConfig.Instance.EarlyHardmodeProgressionRework && !BossRushEvent.BossRushActive)
                            SpawnMechBossHardmodeOres();
                    }
                    break;

                case NPCID.SkeletronPrime:
                    SetNewShopVariable(new int[] { NPCID.Demolitionist, NPCID.DD2Bartender, NPCID.Stylist, NPCID.Truffle }, NPC.downedMechBossAny);
                    SetNewShopVariable(new int[] { NPCID.Stylist, ModContent.NPCType<Archmage>(), ModContent.NPCType<Bandit>() }, !NPC.downedMechBoss1 || !NPC.downedMechBoss2 || NPC.downedMechBoss3);
                    SetNewBossJustDowned(npc);

                    if (!NPC.downedMechBoss3 && CalamityServerConfig.Instance.EarlyHardmodeProgressionRework && !BossRushEvent.BossRushActive)
                        SpawnMechBossHardmodeOres();
                    break;

                case NPCID.Plantera:
                    SetNewShopVariable(new int[] { NPCID.WitchDoctor, NPCID.Truffle, NPCID.BestiaryGirl, ModContent.NPCType<Bandit>() }, NPC.downedPlantBoss);
                    SetNewBossJustDowned(npc);

                    // Spawn Perennial Ore if Plantera has never been killed
                    if (!NPC.downedPlantBoss && !BossRushEvent.BossRushActive)
                    {
                        string key = "Mods.CalamityMod.Status.Progression.PlantOreText";
                        Color messageColor = Color.GreenYellow;
                        string key2 = "Mods.CalamityMod.Status.Progression.SandSharkText3";
                        Color messageColor2 = Color.Goldenrod;

                        // TODO -- this should probably be moved to a thread like Aureus meteor
                        CalamityUtils.SpawnOre(ModContent.TileType<PerennialOre>(), 12E-05, 0.65f, 0.85f, 5, 10, TileID.Dirt, TileID.Stone);

                        CalamityUtils.BroadcastLocalizedText(key, messageColor);
                        CalamityUtils.BroadcastLocalizedText(key2, messageColor2);
                    }
                    break;

                case NPCID.HallowBoss:
                    SetNewBossJustDowned(npc);
                    break;

                case NPCID.Everscream:
                    SetNewShopVariable(new int[] { ModContent.NPCType<Archmage>() }, NPC.downedChristmasTree || !NPC.downedChristmasSantank || !NPC.downedChristmasIceQueen);
                    break;

                case NPCID.SantaNK1:
                    SetNewShopVariable(new int[] { ModContent.NPCType<Archmage>() }, !NPC.downedChristmasTree || NPC.downedChristmasSantank || !NPC.downedChristmasIceQueen);
                    break;

                case NPCID.IceQueen:
                    SetNewShopVariable(new int[] { ModContent.NPCType<Archmage>() }, !NPC.downedChristmasTree || !NPC.downedChristmasSantank || NPC.downedChristmasIceQueen);
                    break;

                case NPCID.Golem:
                    SetNewShopVariable(new int[] { NPCID.ArmsDealer, NPCID.Cyborg, NPCID.Steampunker, NPCID.Wizard, NPCID.WitchDoctor, NPCID.DD2Bartender, ModContent.NPCType<Bandit>() }, NPC.downedGolemBoss);
                    SetNewBossJustDowned(npc);

                    // If Golem has never been killed, send a message about the Plague.
                    if (!NPC.downedGolemBoss && !BossRushEvent.BossRushActive)
                    {
                        if (!Main.LocalPlayer.dead && Main.LocalPlayer.active)
                            SoundEngine.PlaySound(PlagueSound, Main.LocalPlayer.Center);

                        string key3 = "Mods.CalamityMod.Status.Progression.BabyBossText";
                        Color messageColor3 = Color.Lime;

                        CalamityUtils.BroadcastLocalizedText(key3, messageColor3);
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
                    break;

                case NPCID.LunarTowerSolar:
                    SetNewShopVariable(new int[] { NPCID.BestiaryGirl }, NPC.downedTowerSolar);
                    break;

                case NPCID.MoonLordCore:
                    SetNewShopVariable(new int[] { NPCID.Princess, ModContent.NPCType<Bandit>() }, NPC.downedMoonlord);
                    SetNewBossJustDowned(npc);

                    string key5 = "Mods.CalamityMod.Status.Progression.MoonBossText";
                    Color messageColor5 = Color.Orange;
                    string key6 = "Mods.CalamityMod.Status.Progression.ProfanedBossText2";
                    Color messageColor6 = Color.Cyan;
                    string key7 = "Mods.CalamityMod.Status.Progression.FutureOreText";
                    Color messageColor7 = Color.LightGray;

                    // No progression stuff should run in Boss Rush.
                    if (!BossRushEvent.BossRushActive)
                    {
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

                        // Spawn Exodium planetoids and send messages about Providence, Exodium, and Necroplasm if ML has not been killed yet
                        if (!NPC.downedMoonlord)
                        {
                            CalamityUtils.BroadcastLocalizedText(key5, messageColor5);
                            CalamityUtils.BroadcastLocalizedText(key6, messageColor6);
                            CalamityUtils.BroadcastLocalizedText(key7, messageColor7);
                        }
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
                string key = "Mods.CalamityMod.Status.Progression.HardmodeOreTier2Text";
                Color messageColor = new Color(50, 255, 130);
                CalamityUtils.SpawnOre(TileID.Mythril, 12E-05, 0.55f, 0.8f, 3, 8);
                CalamityUtils.SpawnOre(TileID.Orichalcum, 12E-05, 0.55f, 0.8f, 3, 8);
                CalamityUtils.BroadcastLocalizedText(key, messageColor);
            }
            else if ((!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1))
            {
                string key = "Mods.CalamityMod.Status.Progression.HardmodeOreTier3Text";
                Color messageColor = new Color(50, 255, 130);
                CalamityUtils.SpawnOre(TileID.Adamantite, 12E-05, 0.65f, 0.9f, 3, 8);
                CalamityUtils.SpawnOre(TileID.Titanium, 12E-05, 0.65f, 0.9f, 3, 8);
                CalamityUtils.BroadcastLocalizedText(key, messageColor);
            }
            else
            {
                string key = "Mods.CalamityMod.Status.Progression.HardmodeOreTier4Text";
                Color messageColor = new Color(50, 255, 130);
                CalamityUtils.SpawnOre(ModContent.TileType<HallowedOre>(), 17E-05, 0.55f, 0.9f, 8, 14, TileID.Pearlstone, TileID.HallowHardenedSand, TileID.HallowSandstone, TileID.HallowedIce);
                CalamityUtils.BroadcastLocalizedText(key, messageColor);
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Critters;
using CalamityMod.Items.Fishing.AstralCatches;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Items.Fishing.SunkenSeaCatches;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Abyss;
using CalamityMod.Items.Placeables.Astral;
using CalamityMod.Items.Placeables.FurnitureDriftwood;
using CalamityMod.Items.Placeables.FurnitureAcidwood;
using CalamityMod.Items.Placeables.Crags;
using CalamityMod.Items.Placeables.SunkenSea;
using CalamityMod.Items.Potions.Food;
using CalamityMod.Items.Tools;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Systems
{
    public class RecipeSystem : ModSystem
    {
        #region ModSystem Hooks
        public override void AddRecipes() => HandleRecipes();

        public override void AddRecipeGroups() => HandleRecipeGroups();

        public override void PostSetupContent() => AddShimmerRecipes();
        #endregion

        #region Recipe Group Definitions
        public static int HardmodeAnvil, HardmodeForge, AnyFood;
        public static int AnyCopperOre, AnySilverOre, AnyGoldOre, AnyEvilOre, AnyCobaltOre, AnyMythrilOre, AnyAdamantiteOre;
        public static int AnyCopperBar, AnySilverBar, AnyGoldBar, AnyEvilBar, AnyCobaltBar, AnyMythrilBar, AnyAdamantiteBar;
        public static int Boss2Material, CursedFlameIchor, AnyEvilWater;
        public static int AnyStoneBlock, AnySnowBlock, AnyIceBlock, AnySiltBlock, AnyEvilBlock, AnyGoodBlock;
        public static int AnyWoodenSword, AnyHallowedHelmet, AnyHallowedPlatemail, AnyHallowedGreaves, AnyGoldCrown, LunarPickaxe, LunarHamaxe;
        public static int AnyManaFlower, AnyQuiver, AnyTombstone, AnyWings;

        private static void ModifyVanillaRecipeGroups()
        {
            // Twinklers count as Fireflies
            RecipeGroup firefly = RecipeGroup.recipeGroups[RecipeGroupID.Fireflies];
            firefly.ValidItems.Add(ItemType<TwinklerItem>());

            RecipeGroup fruit = RecipeGroup.recipeGroups[RecipeGroupID.Fruit];
            fruit.ValidItems.Add(ItemType<Barberry>());
            fruit.ValidItems.Add(ItemType<Cometfruit>());
            fruit.ValidItems.Add(ItemType<Jackfruit>());
            fruit.ValidItems.Add(ItemType<Lotus>());
            fruit.ValidItems.Add(ItemType<Mangosteen>());
            fruit.ValidItems.Add(ItemType<Salak>());

            // Astral, Sunken Sea and Sulphurous Sea sand are Sand
            // This recipe group also naturally includes Hardened Sand, but not Sandstone
            RecipeGroup sand = RecipeGroup.recipeGroups[RecipeGroupID.Sand];
            sand.ValidItems.Add(ItemType<AstralSand>());
            sand.ValidItems.Add(ItemType<HardenedAstralSand>());
            sand.ValidItems.Add(ItemType<Dunesand>());
            sand.ValidItems.Add(ItemType<EutrophicSand>());
            sand.ValidItems.Add(ItemType<HardenedEutrophicSand>());
            sand.ValidItems.Add(ItemType<PolypSand>());
            sand.ValidItems.Add(ItemType<VolcanicSand>());
            sand.ValidItems.Add(ItemType<SulphurousSand>());

            // Acidwood and Driftwood are Wood
            RecipeGroup wood = RecipeGroup.recipeGroups[RecipeGroupID.Wood];
            wood.ValidItems.Add(ItemType<Acidwood>());
            wood.ValidItems.Add(ItemType<Driftwood>());
            // Astral Monolith is decidedly not wood-like enough to be used as generic wood.
        }

        public static void HandleRecipeGroups()
        {
            ModifyVanillaRecipeGroups();

            AddOreAndBarRecipeGroups();
            AddEvilBiomeItemRecipeGroups();
            AddBiomeBlockRecipeGroups();
            AddEquipmentRecipeGroups();

            // Mythril Anvil and Orichalcum Anvil
            RecipeGroup group = new RecipeGroup(() => CalamityUtils.GetTextValue("Misc.RecipeGroup.HardmodeAnvil"), new int[]
            {
                ItemID.MythrilAnvil,
                ItemID.OrichalcumAnvil
            });
            HardmodeAnvil = RecipeGroup.RegisterGroup("HardmodeAnvil", group);

            // Adamantite Forge and Titanium Forge
            group = new RecipeGroup(() => CalamityUtils.GetTextValue("Misc.RecipeGroup.HardmodeForge"), new int[]
            {
                ItemID.AdamantiteForge,
                ItemID.TitaniumForge
            });
            HardmodeForge = RecipeGroup.RegisterGroup("HardmodeForge", group);

            // Food
            AnyFood = RecipeGroup.RegisterGroup("AnyFood", GetFoodItems());
        }

        private static void AddOreAndBarRecipeGroups()
        {
            // Copper and Tin
            RecipeGroup group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.CopperOre)}", new int[]
            {
                ItemID.CopperOre,
                ItemID.TinOre
            });
            AnyCopperOre = RecipeGroup.RegisterGroup("AnyCopperOre", group);

            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.CopperBar)}", new int[]
            {
                ItemID.CopperBar,
                ItemID.TinBar
            });
            AnyCopperBar = RecipeGroup.RegisterGroup("AnyCopperBar", group);

            // Silver and Tungsten
            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.SilverOre)}", new int[]
            {
                ItemID.SilverOre,
                ItemID.TungstenOre
            });
            AnySilverOre = RecipeGroup.RegisterGroup("AnySilverOre", group);

            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.SilverBar)}", new int[]
            {
                ItemID.SilverBar,
                ItemID.TungstenBar,
            });
            AnySilverBar = RecipeGroup.RegisterGroup("AnySilverBar", group);

            // Gold and Platinum
            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.GoldOre)}", new int[]
            {
                ItemID.GoldOre,
                ItemID.PlatinumOre
            });
            AnyGoldOre = RecipeGroup.RegisterGroup("AnyGoldOre", group);

            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.GoldBar)}", new int[]
            {
                ItemID.GoldBar,
                ItemID.PlatinumBar
            });
            AnyGoldBar = RecipeGroup.RegisterGroup("AnyGoldBar", group);

            // Demonite and Crimtane
            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.DemoniteOre)}", new int[]
            {
                ItemID.DemoniteOre,
                ItemID.CrimtaneOre
            });
            AnyEvilOre = RecipeGroup.RegisterGroup("AnyEvilOre", group);

            group = new RecipeGroup(() => CalamityUtils.GetTextValue("Misc.RecipeGroup.AnyEvilBar"), new int[]
            {
                ItemID.DemoniteBar,
                ItemID.CrimtaneBar
            });
            AnyEvilBar = RecipeGroup.RegisterGroup("AnyEvilBar", group);

            // Cobalt and Palladium
            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.CobaltOre)}", new int[]
            {
                ItemID.CobaltOre,
                ItemID.PalladiumOre
            });
            AnyCobaltOre = RecipeGroup.RegisterGroup("AnyCobaltOre", group);

            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.CobaltBar)}", new int[]
            {
                ItemID.CobaltBar,
                ItemID.PalladiumBar
            });
            AnyCobaltBar = RecipeGroup.RegisterGroup("AnyCobaltBar", group);

            // Mythril and Orichalcum
            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.MythrilOre)}", new int[]
            {
                ItemID.MythrilOre,
                ItemID.OrichalcumOre
            });
            AnyMythrilOre = RecipeGroup.RegisterGroup("AnyMythrilOre", group);

            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.MythrilBar)}", new int[]
            {
                ItemID.MythrilBar,
                ItemID.OrichalcumBar
            });
            AnyMythrilBar = RecipeGroup.RegisterGroup("AnyMythrilBar", group);

            // Adamantite and Titanium
            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.AdamantiteOre)}", new int[]
            {
                ItemID.AdamantiteOre,
                ItemID.TitaniumOre
            });
            AnyAdamantiteOre = RecipeGroup.RegisterGroup("AnyAdamantiteOre", group);

            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.AdamantiteBar)}", new int[]
            {
                ItemID.AdamantiteBar,
                ItemID.TitaniumBar
            });
            AnyAdamantiteBar = RecipeGroup.RegisterGroup("AnyAdamantiteBar", group);
        }

        private static void AddEvilBiomeItemRecipeGroups()
        {
            // Shadow Scale and Tissue Sample
            RecipeGroup group = new RecipeGroup(() => CalamityUtils.GetTextValue("Misc.RecipeGroup.Boss2Material"), new int[]
            {
                ItemID.ShadowScale,
                ItemID.TissueSample
            });
            Boss2Material = RecipeGroup.RegisterGroup("Boss2Material", group);

            // Cursed Flame and Ichor
            group = new RecipeGroup(() => CalamityUtils.GetTextValue("Misc.RecipeGroup.CursedFlameIchor"), new int[]
            {
                ItemID.CursedFlame,
                ItemID.Ichor
            });
            CursedFlameIchor = RecipeGroup.RegisterGroup("CursedFlameIchor", group);

            // Unholy Water and Blood Water
            group = new RecipeGroup(() => CalamityUtils.GetTextValue("Misc.RecipeGroup.AnyEvilWater"), new int[]
            {
                ItemID.UnholyWater,
                ItemID.BloodWater
            });
            AnyEvilWater = RecipeGroup.RegisterGroup("AnyEvilWater", group);
        }

        private static void AddBiomeBlockRecipeGroups()
        {
            // Vanilla Stone and Astral Stone
            RecipeGroup group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.StoneBlock)}", new int[]
            {
                ItemID.StoneBlock,
                ItemID.EbonstoneBlock,
                ItemID.CrimstoneBlock,
                ItemID.PearlstoneBlock,
                ItemType<AstralStone>()
            });
            AnyStoneBlock = RecipeGroup.RegisterGroup("AnyStoneBlock", group);

            // Vanilla Snow and Astral Snow
            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.SnowBlock)}", new int[]
            {
                ItemID.SnowBlock,
                ItemType<AstralSnow>()
            });
            AnySnowBlock = RecipeGroup.RegisterGroup("AnySnowBlock", group);

            // Vanilla Ice and Astral Ice
            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.IceBlock)}", new int[]
            {
                ItemID.IceBlock,
                ItemID.PurpleIceBlock,
                ItemID.RedIceBlock,
                ItemID.PinkIceBlock,
                ItemType<AstralIce>()
            });
            AnyIceBlock = RecipeGroup.RegisterGroup("AnyIceBlock", group);

            // Silt, Slush, and Astral Silt, for Ancient Fossil
            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.SiltBlock)}", new int[]
            {
                ItemID.SiltBlock,
                ItemID.SlushBlock,
                ItemType<NovaeSlag>()
            });
            AnySiltBlock = RecipeGroup.RegisterGroup("AnySiltBlock", group);

            // Set of all generic Corruption/Crimson blocks, for Overloaded Sludge
            group = new RecipeGroup(() => CalamityUtils.GetTextValue("Misc.RecipeGroup.AnyEvilBlock"), new int[]
            {
                ItemID.EbonstoneBlock,
                ItemID.CrimstoneBlock,
                ItemID.PurpleIceBlock,
                ItemID.RedIceBlock,
                ItemID.EbonsandBlock,
                ItemID.CrimsandBlock,
                ItemID.CorruptHardenedSand,
                ItemID.CrimsonHardenedSand,
                ItemID.CorruptSandstone,
                ItemID.CrimsonSandstone
            });
            AnyEvilBlock = RecipeGroup.RegisterGroup("AnyEvilBlock", group);

            // Set of all generic Hallow blocks, this recipe group is unused
            group = new RecipeGroup(() => CalamityUtils.GetTextValue("Misc.RecipeGroup.AnyGoodBlock"), new int[]
            {
                ItemID.PearlstoneBlock,
                ItemID.PinkIceBlock,
                ItemID.PearlsandBlock,
                ItemID.HallowHardenedSand,
                ItemID.HallowSandstone
            });
            AnyGoodBlock = RecipeGroup.RegisterGroup("AnyGoodBlock", group);
        }

        private static void AddEquipmentRecipeGroups()
        {
            // Wooden Swords for Broken Biome Blade
            RecipeGroup group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.WoodenSword)}", new int[]
            {
                ItemID.WoodenSword,
                ItemID.BorealWoodSword,
                ItemID.RichMahoganySword,
                ItemID.PalmWoodSword,
                ItemID.EbonwoodSword,
                ItemID.ShadewoodSword,
                ItemID.PearlwoodSword,
                ItemID.AshWoodSword,
                ItemType<AcidwoodSword>(),
                ItemType<DriftwoodSword>()
            });
            AnyWoodenSword = RecipeGroup.RegisterGroup("AnyWoodenSword", group);

            // Hallowed Helmets for Angelic Alliance
            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.HallowedHelmet)}", new int[]
            {
                ItemID.HallowedHelmet,
                ItemID.HallowedHeadgear,
                ItemID.HallowedMask,
                ItemID.HallowedHood,
                ItemID.AncientHallowedHelmet,
                ItemID.AncientHallowedHeadgear,
                ItemID.AncientHallowedMask,
                ItemID.AncientHallowedHood
            });
            AnyHallowedHelmet = RecipeGroup.RegisterGroup("AnyHallowedHelmet", group);

            // Hallowed Plate Mails
            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.HallowedPlateMail)}", new int[]
            {
                ItemID.HallowedPlateMail,
                ItemID.AncientHallowedPlateMail
            });
            AnyHallowedPlatemail = RecipeGroup.RegisterGroup("AnyHallowedPlatemail", group);

            // Hallowed Greaves
            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.HallowedGreaves)}", new int[]
            {
                ItemID.HallowedGreaves,
                ItemID.AncientHallowedGreaves
            });
            AnyHallowedGreaves = RecipeGroup.RegisterGroup("AnyHallowedGreaves", group);

            // Gold and Platinum Crowns for Feather Crown
            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.GoldCrown)}", new int[]
            {
                ItemID.GoldCrown,
                ItemID.PlatinumCrown
            });
            AnyGoldCrown = RecipeGroup.RegisterGroup("AnyGoldCrown", group);

            // Vanilla Luminite Pickaxes and Genesis Pickaxe for Crystyl Crusher
            group = new RecipeGroup(() => CalamityUtils.GetTextValue("Misc.RecipeGroup.LunarPickaxe"), new int[]
            {
                ItemID.SolarFlarePickaxe,
                ItemID.VortexPickaxe,
                ItemID.NebulaPickaxe,
                ItemID.StardustPickaxe,
                ItemType<GenesisPickaxe>()
            });
            LunarPickaxe = RecipeGroup.RegisterGroup("LunarPickaxe", group);

            // Luminite Hamaxes for Grax
            group = new RecipeGroup(() => CalamityUtils.GetTextValue("Misc.RecipeGroup.LunarHamaxe"), new int[]
            {
                ItemID.LunarHamaxeSolar,
                ItemID.LunarHamaxeVortex,
                ItemID.LunarHamaxeNebula,
                ItemID.LunarHamaxeStardust
            });
            LunarHamaxe = RecipeGroup.RegisterGroup("LunarHamaxe", group);

            // Mana Flower+ for Ethereal Talisman
            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.ManaFlower)}", new int[]
            {
                ItemID.ManaFlower,
                ItemID.ArcaneFlower,
                ItemID.MagnetFlower,
                ItemID.ManaCloak
            });
            AnyManaFlower = RecipeGroup.RegisterGroup("AnyManaFlower", group);

            // Magic Quiver+ for Elemental Quiver
            group = new RecipeGroup(() => CalamityUtils.GetTextValue("Misc.RecipeGroup.AnyQuiver"), new int[]
            {
                ItemID.MagicQuiver,
                ItemID.MoltenQuiver,
                ItemID.StalkersQuiver
            });
            AnyQuiver = RecipeGroup.RegisterGroup("AnyQuiver", group);

            // Tombstones for Grave Grimreaver
            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.Tombstone)}", new int[]
            {
                ItemID.Tombstone,
                ItemID.GraveMarker,
                ItemID.CrossGraveMarker,
                ItemID.Headstone,
                ItemID.Gravestone,
                ItemID.Obelisk,
                ItemID.RichGravestone1,
                ItemID.RichGravestone2,
                ItemID.RichGravestone3,
                ItemID.RichGravestone4,
                ItemID.RichGravestone5
            });
            AnyTombstone = RecipeGroup.RegisterGroup("AnyTombstone", group);
        }
        #endregion

        #region Automatic Recipe Groups
        private static RecipeGroup GetFoodItems()
        {
            List<int> foodIds = new List<int>();
            foreach (var i in ContentSamples.ItemsByType)
            {
                Item item = i.Value;
                // 05JUN2024: Ozzatron: support non-vanilla "food-y" buffs (this set by default contains WellFed, WellFed2 and WellFed3)
                if (BuffID.Sets.IsWellFed[item.buffType])
                {
                    foodIds.Add(item.type);
                }
            }
            return new RecipeGroup(() => CalamityUtils.GetTextValue("Misc.RecipeGroup.AnyFood"), [.. foodIds]);
        }
        #endregion

        #region Main Recipes
        public static void HandleRecipes()
        {
            EditVanillaRecipes();

            // Leather from Vertebrae
            Recipe.Create(ItemID.Leather).
                AddIngredient(ItemID.Vertebrae, 2).
                AddTile(TileID.WorkBenches).
                Register();

            // Fallen Stars from Stardust
            Recipe.Create(ItemID.FallenStar).
                AddIngredient<StarblightSoot>(5).
                AddTile(TileID.Anvils).
                Register()
                .DisableDecraft();

            // Stohne smelts into Lihzahrd Bricks
            Recipe.Create(ItemID.LihzahrdBrick).
                AddIngredient(ItemType<Stohne>()).
                AddTile(TileID.LihzahrdFurnace).
                Register();

            // Earlier Rocket Is for early rocket weapons
            Recipe.Create(ItemID.RocketI, 100).
                AddRecipeGroup("IronBar").
                AddIngredient(ItemID.EmptyBullet, 100).
                AddIngredient(ItemID.ExplosivePowder, 4).
                AddTile(TileID.Anvils).
                Register();

            // and Rocket IIs (requires slightly more explosive powder)
            Recipe.Create(ItemID.RocketII, 100).
                AddRecipeGroup("IronBar").
                AddIngredient(ItemID.EmptyBullet, 100).
                AddIngredient(ItemID.ExplosivePowder, 5).
                AddTile(TileID.Anvils).
                Register();

            // Life Crystal
            Recipe.Create(ItemID.LifeCrystal).
                AddRecipeGroup("AnyStoneBlock", 5).
                AddIngredient(ItemID.Ruby, 2).
                AddIngredient(ItemID.HealingPotion).
                AddTile(TileID.Anvils).
                Register();

            // Life Fruit
            Recipe.Create(ItemID.LifeFruit).
                AddIngredient<PlantyMush>(10).
                AddIngredient<LivingShard>().
                AddTile(TileID.MythrilAnvil).
                Register()
                .DisableDecraft();

            // Ultrabright Torch
            Recipe.Create(ItemID.UltrabrightTorch, 33).
                AddIngredient(ItemID.Torch, 33).
                AddIngredient<SeaPrism>().
                AddTile(TileID.Anvils).
                Register()
                .DisableDecraft();

            // Money Trough
            Recipe.Create(ItemID.MoneyTrough).
                AddIngredient(ItemID.PiggyBank).
                AddIngredient(ItemID.Feather, 2).
                AddIngredient<BloodOrb>().
                AddIngredient(ItemID.GoldCoin, 15).
                AddTile(TileID.Anvils).
                Register();

            // Demon Conch
            Recipe.Create(ItemID.DemonConch).
                AddIngredient<ScorchedBone>(20).
                AddIngredient(ItemID.BlackPearl).
                AddTile(TileID.Hellforge).
                Register();

            // Magic Conch
            Recipe.Create(ItemID.MagicConch).
                AddIngredient(ItemID.ShellPileBlock, 20).
                AddIngredient(ItemID.WhitePearl).
                AddTile(TileID.Anvils).
                Register();

            // Alternative Evil Biome items
            Recipe.Create(ItemID.CrimsonRod).
                AddIngredient(ItemID.Vilethorn).
                AddTile(TileID.Anvils).
                AddCondition(Condition.InGraveyard).
                Register()
                .DisableDecraft();

            Recipe.Create(ItemID.Vilethorn).
                AddIngredient(ItemID.CrimsonRod).
                AddTile(TileID.Anvils).
                AddCondition(Condition.InGraveyard).
                Register()
                .DisableDecraft();

            Recipe.Create(ItemID.TheRottedFork).
                AddIngredient(ItemID.BallOHurt).
                AddTile(TileID.Anvils).
                AddCondition(Condition.InGraveyard).
                Register()
                .DisableDecraft();

            Recipe.Create(ItemID.BallOHurt).
                AddIngredient(ItemID.TheRottedFork).
                AddTile(TileID.Anvils).
                AddCondition(Condition.InGraveyard).
                Register()
                .DisableDecraft();

            Recipe.Create(ItemID.TheUndertaker).
                AddIngredient(ItemID.Musket).
                AddTile(TileID.Anvils).
                AddCondition(Condition.InGraveyard).
                Register()
                .DisableDecraft();

            Recipe.Create(ItemID.Musket).
                AddIngredient(ItemID.TheUndertaker).
                AddTile(TileID.Anvils).
                AddCondition(Condition.InGraveyard).
                Register()
                .DisableDecraft();

            Recipe.Create(ItemID.CrimsonHeart).
                AddIngredient(ItemID.ShadowOrb).
                AddTile(TileID.TinkerersWorkbench).
                AddCondition(Condition.InGraveyard).
                Register()
                .DisableDecraft();

            Recipe.Create(ItemID.ShadowOrb).
                AddIngredient(ItemID.CrimsonHeart).
                AddTile(TileID.TinkerersWorkbench).
                AddCondition(Condition.InGraveyard).
                Register()
                .DisableDecraft();

            Recipe.Create(ItemID.BrainOfConfusion).
                AddIngredient(ItemID.WormScarf).
                AddTile(TileID.TinkerersWorkbench).
                AddCondition(Condition.InGraveyard).
                Register()
                .DisableDecraft();

            Recipe.Create(ItemID.WormScarf).
                AddIngredient(ItemID.BrainOfConfusion).
                AddTile(TileID.TinkerersWorkbench).
                AddCondition(Condition.InGraveyard).
                Register()
                .DisableDecraft();

            Recipe.Create(ItemID.TendonHook).
                AddIngredient(ItemID.WormHook).
                AddTile(TileID.TinkerersWorkbench).
                AddCondition(Condition.InGraveyard).
                Register()
                .DisableDecraft();

            Recipe.Create(ItemID.WormHook).
                AddIngredient(ItemID.TendonHook).
                AddTile(TileID.TinkerersWorkbench).
                AddCondition(Condition.InGraveyard).
                Register()
                .DisableDecraft();

            Recipe.Create(ItemID.DartPistol).
                AddIngredient(ItemID.DartRifle).
                AddTile(TileID.Anvils).
                AddCondition(Condition.InGraveyard).
                Register()
                .DisableDecraft();

            Recipe.Create(ItemID.DartRifle).
                AddIngredient(ItemID.DartPistol).
                AddTile(TileID.Anvils).
                AddCondition(Condition.InGraveyard).
                Register()
                .DisableDecraft();

            Recipe.Create(ItemID.ChainGuillotines).
                AddIngredient(ItemID.FetidBaghnakhs).
                AddTile(TileID.Anvils).
                AddCondition(Condition.InGraveyard).
                Register()
                .DisableDecraft();

            Recipe.Create(ItemID.FetidBaghnakhs).
                AddIngredient(ItemID.ChainGuillotines).
                AddTile(TileID.Anvils).
                AddCondition(Condition.InGraveyard).
                Register()
                .DisableDecraft();

            Recipe.Create(ItemID.ClingerStaff).
                AddIngredient(ItemID.SoulDrain).
                AddTile(TileID.Anvils).
                AddCondition(Condition.InGraveyard).
                Register()
                .DisableDecraft();

            Recipe.Create(ItemID.SoulDrain).
                AddIngredient(ItemID.ClingerStaff).
                AddTile(TileID.Anvils).
                AddCondition(Condition.InGraveyard).
                Register()
                .DisableDecraft();

            Recipe.Create(ItemID.PutridScent).
                AddIngredient(ItemID.FleshKnuckles).
                AddTile(TileID.TinkerersWorkbench).
                AddCondition(Condition.InGraveyard).
                Register()
                .DisableDecraft();

            Recipe.Create(ItemID.FleshKnuckles).
                AddIngredient(ItemID.PutridScent).
                AddTile(TileID.TinkerersWorkbench).
                AddCondition(Condition.InGraveyard).
                Register()
                .DisableDecraft();

            AddAstralRecipeVariants();
            AddBloodOrbPotionRecipes();
            AddCookedFood();
            AddMiscItemRecipes();
            AddTombstoneRecipes();
            AddEarlyGameWeaponRecipes();
            AddEarlyGameAccessoryRecipes();
            AddHardmodeItemRecipes();
            AddArmorRecipes();
        }
        #endregion

        #region Vanilla Recipe Edits
        internal static void EditVanillaRecipes()
        {
            // Disable warnings for unused stuff as they can continue to be used freely
#pragma warning disable CS8321

            // Predicates for specifying which recipes to edit
            static Func<Recipe, bool> Vanilla(int itemID) => r => r.Mod is null && r.HasResult(itemID);
            static Func<Recipe, bool> VanillaEach(params int[] itemIDs) => r => r.Mod is null && itemIDs.Any(r.HasResult);
            static Func<Recipe, bool> Produces(int itemID) => r => r.HasResult(itemID);

            // Actions to perform, i.e. the actual recipe edits to execute
            static void Disable(Recipe r) => r.DisableRecipe();
            static Action<Recipe> ChangeResultStack(int stack) => r => r.createItem.stack = stack;
            static Action<Recipe> AddIngredient(int itemID, int stack = 1) => r => r.AddIngredient(itemID, stack);
            static Action<Recipe> AddGroup(int groupID, int stack = 1) => r => r.AddRecipeGroup(groupID, stack);
            static Action<Recipe> ChangeIngredientStack(int itemID, int stack = 1) => r => r.ChangeIngredientStack(itemID, stack);
            static Action<Recipe> ReplaceIngredient(int oldItemID, int newItemID) => r =>
            {
                int idx = r.IngredientIndex(oldItemID);
                if (idx == -1)
                    return;

                // Replace the entire Item, but keep the old stack count.
                Item newIngredient = new Item();
                newIngredient.SetDefaults(newItemID);
                newIngredient.stack = r.requiredItem[idx].stack;
                r.requiredItem[idx] = newIngredient;
            };
            static Action<Recipe> RemoveIngredient(int itemID) => r => r.RemoveIngredient(itemID);
            static Action<Recipe> SwapIngredients(int i1, int i2) => r =>
            {
                if (r.requiredItem.Count < i1 + 1 || r.requiredItem.Count < i2 + 1)
                    return;

                // Swap the entire Items in the List<Item> (uses pointers under the hood).
                // DO NOT do what it tells you to here by making it a tuple notation swap. That does NOT work.
                var store = r.requiredItem[i1];
                r.requiredItem[i1] = r.requiredItem[i2];
                r.requiredItem[i2] = store;
            };
            static Action<Recipe> ReplaceTile(int oldTileID, int newTileID) => r =>
            {
                int idx = r.requiredTile.IndexOf(oldTileID);
                if (idx == -1)
                    return;
                r.requiredTile[idx] = newTileID;
            };

            // Re-enable the above disabled warning
#pragma warning restore CS8321

            var edits = new Dictionary<Func<Recipe, bool>, Action<Recipe>>(128)
            {
                { Vanilla(ItemID.MiniNukeI), Disable },
                { Vanilla(ItemID.MiniNukeII), Disable },
                { Vanilla(ItemID.ReconScope), Disable },

                // Make various things cheaper (sorted by progression)
                { Vanilla(ItemID.Leather), ChangeIngredientStack(ItemID.RottenChunk, 2) },
                { Vanilla(ItemID.JestersArrow), JesterArrowRecipeEdit },
                { Vanilla(ItemID.TeleportationPotion), TeleportationPotionRecipeEdit },
                { Vanilla(ItemID.WormFood), WormFoodRecipeEdit },
                { Vanilla(ItemID.BloodySpine), BloodySpineRecipeEdit },
                { Vanilla(ItemID.GoblinBattleStandard), ChangeIngredientStack(ItemID.TatteredCloth, 5) },
                { Vanilla(ItemID.Beenade), BeenadeRecipeEdit },
                { Vanilla(ItemID.ChlorophyteBar), ChangeIngredientStack(ItemID.ChlorophyteOre, 4) },
                { Vanilla(ItemID.OrichalcumAnvil), ChangeIngredientStack(ItemID.OrichalcumBar, 10) },
                { Vanilla(ItemID.ShroomiteBar), ChangeIngredientStack(ItemID.GlowingMushroom, 5) },
                { Vanilla(ItemID.TrueNightsEdge), TrueNightsEdgeRecipeEdit },
                { Vanilla(ItemID.TrueExcalibur), ChangeIngredientStack(ItemID.ChlorophyteBar, 12) },

                // Tier lock various items to a higher tier (sorted by progression)
                { Vanilla(ItemID.Trimarang), AddIngredient(ItemType<PearlShard>(), 5) },
                { Vanilla(ItemID.NightsEdge), AddIngredient(ItemType<PurifiedGel>(), 5) },
                { Vanilla(ItemID.FairyBoots), AddIngredient(ItemID.SoulofLight, 5) },
                { Vanilla(ItemID.FairyBell), RemoveIngredient(ItemID.SoulofSight) },
                { Vanilla(ItemID.HellfireTreads), AddIngredient(ItemType<EssenceofHavoc>(), 4) },
                { Vanilla(ItemID.SpiritFlame), AddGroup(AnyAdamantiteBar, 2) },
                { Vanilla(ItemID.TerraBlade), AddIngredient(ItemType<LivingShard>(), 12) },
                { Vanilla(ItemID.FireGauntlet), AddIngredient(ItemType<ScoriaBar>(), 5) },
                { Vanilla(ItemID.Zenith), ZenithRecipeEdit },

                // Tier unlock various items to a lower tier (sorted by progression)
                // Move a bunch of mythril anvil locked stuff in early HM to regular anvils to fit progression changes
                { VanillaEach(
                    ItemID.MechanicalEye, ItemID.MechanicalWorm, ItemID.MechanicalSkull, ItemID.MechdusaSummon,
                    ItemID.DaoofPow, ItemID.Chik, ItemID.MeteorStaff, ItemID.CoolWhip,
                    ItemID.AngelWings, ItemID.DemonWings, ItemID.FairyWings, ItemID.FairyBell,
                    ItemID.CursedArrow, ItemID.CursedBullet, ItemID.IchorArrow, ItemID.IchorBullet),
                    ReplaceTile(TileID.MythrilAnvil, TileID.Anvils)
                },
                { Vanilla(ItemID.OpticStaff), RemoveIngredient(ItemID.HallowedBar) },

                // Swap hellstone recipe ordering (they have bars first and it's wrong and irritating)
                { VanillaEach(ItemID.Flamarang, ItemID.PhoenixBlaster, ItemID.FireproofBugNet), SwapIngredients(0, 1) },

                // Swap Beetle Armor recipe ordering (they have beetle husks first and it's wrong and irritating)
                { VanillaEach(ItemID.BeetleHelmet, ItemID.BeetleScaleMail, ItemID.BeetleShell, ItemID.BeetleLeggings), SwapIngredients(0, 1) },

                // Pumpkin & Frost Moon non linearity
                { Vanilla(ItemID.PumpkinMoonMedallion), RemoveIngredient(ItemID.HallowedBar) },
                { Vanilla(ItemID.NaughtyPresent), RemoveIngredient(ItemID.SoulofFright) },

                // Make Enchanted Boomerang slightly harder to obtain
                { Vanilla(ItemID.EnchantedBoomerang), EnchantedBoomerangRecipeEdit },

                // Adjust Fertilizer decrafting and add an alternate recipe
                { Vanilla(ItemID.Fertilizer), FertilizerRecipeEdit },

                // Add 20 Souls of Flight to vanilla Luminite wings
                { VanillaEach(ItemID.WingsSolar, ItemID.WingsVortex, ItemID.WingsNebula, ItemID.WingsStardust), LunarWingsRecipeEdits },
            };

            // Apply all recipe changes.
            IEnumerator<Recipe> recipeEnumerator = Main.recipe.ToList().GetEnumerator();
            while (recipeEnumerator.MoveNext())
            {
                Recipe r = recipeEnumerator.Current;
                foreach (var kv in edits)
                    if (kv.Key.Invoke(r))
                        kv.Value.Invoke(r);
            }
        }

        // If Jester's Arrows give less than 50 per Fallen Star, make it 50
        private static void JesterArrowRecipeEdit(Recipe r)
        {
            int intendedStack = 50;
            if (r.createItem.stack < intendedStack)
                r.createItem.stack = intendedStack;
            r.ChangeIngredientStack(ItemID.WoodenArrow, intendedStack);
        }

        // Increases Teleportation Potion's recipe to give 5 per craft and use 5 bottled waters
        private static void TeleportationPotionRecipeEdit(Recipe r)
        {
            int intendedStack = 5;
            if (r.createItem.stack < intendedStack)
                r.createItem.stack = intendedStack;
            r.ChangeIngredientStack(ItemID.BottledWater, intendedStack);
        }

        // Increases Beenade's recipe to use 4 Grenades and yield 4 Beenades
        private static void BeenadeRecipeEdit(Recipe r)
        {
            int intendedStack = 4;
            if (r.createItem.stack < intendedStack)
                r.createItem.stack = intendedStack;
            r.ChangeIngredientStack(ItemID.Grenade, intendedStack);
        }

        // Change True Night's Edge recipe to require far less mech boss souls
        private static void TrueNightsEdgeRecipeEdit(Recipe r)
        {
            int intendedStack = 3;
            r.ChangeIngredientStack(ItemID.SoulofSight, intendedStack);
            r.ChangeIngredientStack(ItemID.SoulofMight, intendedStack);
            r.ChangeIngredientStack(ItemID.SoulofFright, intendedStack);
        }

        private static void WormFoodRecipeEdit(Recipe r)
        {
            r.ChangeIngredientStack(ItemID.VilePowder, 20);
            r.ChangeIngredientStack(ItemID.RottenChunk, 10);
        }

        private static void BloodySpineRecipeEdit(Recipe r)
        {
            r.ChangeIngredientStack(ItemID.ViciousPowder, 20);
            r.ChangeIngredientStack(ItemID.Vertebrae, 10);
        }

        private static void ZenithRecipeEdit(Recipe r)
        {
            r.AddIngredient(ItemType<AuricBar>(), 5);
            int idx = r.requiredTile.IndexOf(TileID.MythrilAnvil);
            if (idx == -1)
                return;
            r.requiredTile[idx] = TileType<CosmicAnvil>();
        }

        private static void LunarWingsRecipeEdits(Recipe r)
        {
            // Add Soul of Flight then move it to the top
            r.AddIngredient(ItemID.SoulofFlight, 20);

            if (r.requiredItem.Count < 3)
                return;

            var store = r.requiredItem[0];
            r.requiredItem[0] = r.requiredItem[2];
            r.requiredItem[2] = r.requiredItem[1];
            r.requiredItem[1] = store;
        }

        private static void EnchantedBoomerangRecipeEdit(Recipe r)
        {
            // Add Any Gold Bar and make it crafted at an Anvil
            r.AddRecipeGroup(AnyGoldBar, 8);
            r.AddTile(TileID.Anvils);

            // Then move it to the proper spot
            if (r.requiredItem.Count < 3)
                return;

            var store = r.requiredItem[1];
            r.requiredItem[1] = r.requiredItem[2];
            r.requiredItem[2] = store;

            // Increase amount of Fallen Stars used
            r.requiredItem[2].stack = 6;
        }

        private static void FertilizerRecipeEdit(Recipe r)
        {
            // Custom Shimmer result, to prevent Scorched Bone -> Bone shenanigans
            r.AddCustomShimmerResult(ItemID.PoopBlock, 3);
            r.AddCustomShimmerResult(ItemType<AncientBoneDust>(), 3);
            r.AddCustomShimmerResult(ItemID.AshBlock, 3);

            // Alternative recipe using (a bit more) Scorched Bones
            Recipe r2 = Recipe.Create(ItemID.Fertilizer);
            r2.AddIngredient(ItemID.PoopBlock, 3);
            r2.AddIngredient<ScorchedBone>(6);
            r2.AddIngredient(ItemID.AshBlock, 3);
            r2.AddTile(TileID.Bottles);
            r2.Register();
            r2.SortAfterFirstRecipesOf(ItemID.Fertilizer);
            r2.DisableDecraft();
        }
        #endregion

        #region Shimmer Recipes
        /// <summary>
        /// Adds a shimmer recipe, while having the result transform into the ingredient's original result.
        /// <para>This is used for inserting items into various shimmer result trees/loops, like the Class Emblem loop.</para>
        /// </summary>
        private static void InsertShimmerResult(int result, int ingredient)
        {
            ItemID.Sets.ShimmerTransformToItem[result] = ItemID.Sets.ShimmerTransformToItem[ingredient];
            ItemID.Sets.ShimmerTransformToItem[ingredient] = result;
        }

        public static void AddShimmerRecipes()
        {
            // shorthand for the ID set
            int[] convert = ItemID.Sets.ShimmerTransformToItem;

            InsertShimmerResult(ItemType<RogueEmblem>(), ItemID.SummonerEmblem);

            // Cyclical Travelling Merchant Cell Phone materials
            convert[ItemID.DPSMeter] = ItemID.LifeformAnalyzer;
            convert[ItemID.LifeformAnalyzer] = ItemID.Stopwatch;
            convert[ItemID.Stopwatch] = ItemID.DPSMeter;

            // Enchanted Sword and Terragrim shimmer into each other
            convert[ItemID.EnchantedSword] = ItemID.Terragrim;
            convert[ItemID.Terragrim] = ItemID.EnchantedSword;

            // Allow Cascade and Gray Zapinator to be obtained in Hardmode
            convert[ItemID.HelFire] = ItemID.Cascade;
            convert[ItemID.ZapinatorOrange] = ItemID.ZapinatorGray;
        }
        #endregion

        #region Astral Recipe Variants
        private static void AddAstralRecipeVariants()
        {
            #region Astral Clay
            // Intentionally excluding Red Brick and Red Stucco recipes
            // Argon Moss Brick
            Recipe r = Recipe.Create(ItemID.ArgonMossBlock, 10);
            r.AddIngredient(ItemID.ArgonMoss);
            r.AddIngredient<AstralClay>(10);
            r.AddTile(TileID.Furnaces);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.ArgonMossBlock);
            r.DisableDecraft();

            // Bowl
            r = Recipe.Create(ItemID.Bowl);
            r.AddIngredient<AstralClay>(2);
            r.AddTile(TileID.Furnaces);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.Bowl);
            r.DisableDecraft();

            // Clay Pot
            r = Recipe.Create(ItemID.ClayPot);
            r.AddIngredient<AstralClay>(5);
            r.AddTile(TileID.Furnaces);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.ClayPot);
            r.DisableDecraft();

            // Helium Moss Brick
            r = Recipe.Create(ItemID.RainbowMossBlock, 10);
            r.AddIngredient(ItemID.RainbowMoss);
            r.AddIngredient<AstralClay>(10);
            r.AddTile(TileID.Furnaces);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.RainbowMossBlock);
            r.DisableDecraft();

            // Krypton Moss Brick
            r = Recipe.Create(ItemID.KryptonMossBlock, 10);
            r.AddIngredient(ItemID.KryptonMoss);
            r.AddIngredient<AstralClay>(10);
            r.AddTile(TileID.Furnaces);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.KryptonMossBlock);
            r.DisableDecraft();

            // Lava Moss Brick
            r = Recipe.Create(ItemID.LavaMossBlock, 10);
            r.AddIngredient(ItemID.LavaMoss);
            r.AddIngredient<AstralClay>(10);
            r.AddTile(TileID.Furnaces);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.LavaMossBlock);
            r.DisableDecraft();

            // Neon Moss Brick
            r = Recipe.Create(ItemID.VioletMossBlock, 10);
            r.AddIngredient(ItemID.VioletMoss);
            r.AddIngredient<AstralClay>(10);
            r.AddTile(TileID.Furnaces);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.VioletMossBlock);
            r.DisableDecraft();

            // Pink Vase
            r = Recipe.Create(ItemID.PinkVase);
            r.AddIngredient<AstralClay>(4);
            r.AddTile(TileID.Furnaces);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.PinkVase);
            r.DisableDecraft();

            // Plate
            r = Recipe.Create(ItemID.FoodPlatter);
            r.AddIngredient<AstralClay>(2);
            r.AddTile(TileID.Furnaces);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.FoodPlatter);
            r.DisableDecraft();

            // Teapot
            r = Recipe.Create(ItemID.TeaKettle);
            r.AddIngredient<AstralClay>(12);
            r.AddIngredient(ItemID.Bone, 12);
            r.AddTile(TileID.Furnaces);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.TeaKettle);
            r.DisableDecraft();

            // Wandering Jingasa
            r = Recipe.Create(ItemID.RoninHat);
            r.AddIngredient<AstralClay>(10);
            r.AddIngredient(ItemID.Firefly, 3); // Does not use the recipe group in Vanilla
            r.AddTile(TileID.Loom);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.RoninHat);
            r.DisableDecraft();

            // Xenon Moss Brick
            r = Recipe.Create(ItemID.XenonMossBlock, 10);
            r.AddIngredient(ItemID.XenonMoss);
            r.AddIngredient<AstralClay>(10);
            r.AddTile(TileID.Furnaces);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.XenonMossBlock);
            r.DisableDecraft();
            #endregion

            #region Astral Dirt
            // Intentionally excluding a ton of pure dirt recipes ie. Dirt Bombs, Walls
            // Floret Protector set
            r = Recipe.Create(ItemID.FloretProtectorHelmet);
            r.AddIngredient(ItemID.Glass, 20);
            r.AddIngredient<AstralDirt>(10);
            r.AddIngredient(ItemID.Daybloom);
            r.AddTile(TileID.Loom);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.FloretProtectorHelmet);
            r.DisableDecraft();

            r = Recipe.Create(ItemID.FloretProtectorChestplate);
            r.AddIngredient(ItemID.Silk, 20);
            r.AddIngredient<AstralDirt>(15);
            r.AddTile(TileID.Loom);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.FloretProtectorChestplate);
            r.DisableDecraft();

            r = Recipe.Create(ItemID.FloretProtectorLegs);
            r.AddIngredient(ItemID.Silk, 20);
            r.AddIngredient<AstralDirt>(15);
            r.AddTile(TileID.Loom);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.FloretProtectorLegs);
            r.DisableDecraft();
            #endregion

            #region Astral Ice
            // Ice Torch
            r = Recipe.Create(ItemID.IceTorch, 3);
            r.AddIngredient(ItemID.Torch, 3);
            r.AddIngredient<AstralIce>();
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.IceTorch);
            r.DisableDecraft();
            #endregion

            #region Astral Snow
            // Frozen Banana Daiquiri
            r = Recipe.Create(ItemID.BananaDaiquiri);
            r.AddIngredient(ItemID.Banana);
            r.AddIngredient(ItemID.Bottle);
            r.AddIngredient<AstralSnow>();
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.BananaDaiquiri);
            r.DisableDecraft();

            // Snowball
            r = Recipe.Create(ItemID.Snowball, 15);
            r.AddIngredient<AstralSnow>();
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.Snowball);
            r.DisableDecraft();
            #endregion

            #region Hardened Astral Sand
            // Desert Torch
            r = Recipe.Create(ItemID.DesertTorch, 3);
            r.AddIngredient(ItemID.Torch, 3);
            r.AddIngredient<HardenedAstralSand>();
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.DesertTorch);
            r.DisableDecraft();
            #endregion
        }
        #endregion

        #region Potions from Blood Orbs
        private static void AddBloodOrbPotionRecipes()
        {
            // List of vanilla potions which can be crafted with Blood Orbs
            short[] FiveOrbGroup = new[]
            {
                ItemID.WormholePotion,
                ItemID.TeleportationPotion,
                ItemID.SwiftnessPotion,
                ItemID.FeatherfallPotion,
                ItemID.ShinePotion,
                ItemID.InvisibilityPotion,
                ItemID.NightOwlPotion,
                ItemID.HunterPotion,
                ItemID.TrapsightPotion,
                ItemID.ThornsPotion,
                ItemID.IronskinPotion,
                ItemID.RegenerationPotion,
                ItemID.TitanPotion,
                ItemID.AmmoReservationPotion,
                ItemID.GillsPotion,
                ItemID.WaterWalkingPotion,
                ItemID.FlipperPotion,
                ItemID.BuilderPotion,
                ItemID.MiningPotion,
                ItemID.FishingPotion,
                ItemID.CratePotion,
                ItemID.SonarPotion,
                ItemID.GenderChangePotion,
                ItemID.LovePotion,
                ItemID.StinkPotion,
                ItemID.RecallPotion,
                ItemID.LuckPotionLesser
            };

            short[] TenOrbGroup = new[]
            {
                ItemID.ArcheryPotion,
                ItemID.GravitationPotion,
                ItemID.SpelunkerPotion,
                ItemID.BattlePotion,
                ItemID.CalmingPotion,
                ItemID.MagicPowerPotion,
                ItemID.ManaRegenerationPotion,
                ItemID.WarmthPotion,
                ItemID.ObsidianSkinPotion,
                ItemID.PotionOfReturn,
                ItemID.LuckPotion,
                ItemID.BiomeSightPotion
            };

            short[] FifteenOrbGroup = new[]
            {
                ItemID.WrathPotion,
                ItemID.RagePotion,
                ItemID.EndurancePotion,
                ItemID.LifeforcePotion,
                ItemID.HeartreachPotion,
                ItemID.SummoningPotion,
                ItemID.InfernoPotion,
                ItemID.LuckPotionGreater
            };
            Recipe r;

            foreach (var potion in FiveOrbGroup)
            {
                r = Recipe.Create(potion);
                r.AddIngredient(ItemID.BottledWater);
                r.AddIngredient<BloodOrb>(5);
                r.AddTile(TileID.AlchemyTable);
                r.Register();
                r.SortAfterFirstRecipesOf(potion);
                r.DisableDecraft();
            }
            foreach (var potion in TenOrbGroup)
            {
                r = Recipe.Create(potion);
                r.AddIngredient(ItemID.BottledWater);
                r.AddIngredient<BloodOrb>(10);
                r.AddTile(TileID.AlchemyTable);
                r.Register();
                r.SortAfterFirstRecipesOf(potion);
                r.DisableDecraft();
            }
            foreach (var potion in FifteenOrbGroup)
            {
                r = Recipe.Create(potion);
                r.AddIngredient(ItemID.BottledWater);
                r.AddIngredient<BloodOrb>(15);
                r.AddTile(TileID.AlchemyTable);
                r.Register();
                r.SortAfterFirstRecipesOf(potion);
                r.DisableDecraft();
            }
        }
        #endregion

        #region Cooked Food
        private static void AddCookedFood()
        {
            #region Alternative Recipes
            #region Cooked Fish
            Recipe r = Recipe.Create(ItemID.CookedFish);
            r.AddIngredient<TwinklingPollox>();
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.CookedFish);
            r.DisableDecraft();

            r = Recipe.Create(ItemID.CookedFish);
            r.AddIngredient<PrismaticGuppy>();
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.CookedFish);
            r.DisableDecraft();

            r = Recipe.Create(ItemID.CookedFish);
            r.AddIngredient<CoralskinFoolfish>();
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.CookedFish);
            r.DisableDecraft();

            r = Recipe.Create(ItemID.CookedFish);
            r.AddIngredient<GleamingCucumber>();
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.CookedFish);
            r.DisableDecraft();

            r = Recipe.Create(ItemID.CookedFish);
            r.AddIngredient<MoltenFishron>();
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.CookedFish);
            r.DisableDecraft();

            r = Recipe.Create(ItemID.CookedFish);
            r.AddIngredient<SpecularSturgeon>();
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.CookedFish);
            r.DisableDecraft();

            r = Recipe.Create(ItemID.CookedFish);
            r.AddIngredient<Squidoom>();
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.CookedFish);
            r.DisableDecraft();
            #endregion

            #region Seafood Dinner
            r = Recipe.Create(ItemID.SeafoodDinner);
            r.AddIngredient<AldebaranAlewife>(2);
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.SeafoodDinner);
            r.DisableDecraft();

            r = Recipe.Create(ItemID.SeafoodDinner);
            r.AddIngredient<Bloodfin>(2);
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.SeafoodDinner);
            r.DisableDecraft();

            r = Recipe.Create(ItemID.SeafoodDinner);
            r.AddIngredient<CoastalDemonfish>(2);
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.SeafoodDinner);
            r.DisableDecraft();

            r = Recipe.Create(ItemID.SeafoodDinner);
            r.AddIngredient<Shadowfish>(2);
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.SeafoodDinner);
            r.DisableDecraft();

            r = Recipe.Create(ItemID.SeafoodDinner);
            r.AddIngredient<SunkenSailfish>(2);
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.SeafoodDinner);
            r.DisableDecraft();
            #endregion

            r = Recipe.Create(ItemID.BowlofSoup);
            r.AddIngredient(ItemID.Mushroom);
            r.AddIngredient<SeaMinnowItem>();
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.BowlofSoup);
            r.DisableDecraft();

            r = Recipe.Create(ItemID.Sashimi);
            r.AddIngredient<CragBullhead>();
            r.AddTile(TileID.WorkBenches);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.Sashimi);
            r.DisableDecraft();

            r = Recipe.Create(ItemID.CookedShrimp);
            r.AddIngredient<ProcyonidPrawn>();
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.SortAfterFirstRecipesOf(ItemID.CookedShrimp);
            r.DisableDecraft();
            #endregion

            r = Recipe.Create(ItemID.Bacon);
            r.AddIngredient<PiggyItem>();
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.DisableDecraft();

            r = Recipe.Create(ItemID.Bacon);
            r.AddIngredient<PiggyGoldItem>();
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.DisableDecraft();

            r = Recipe.Create(ItemID.GoldenDelight);
            r.AddIngredient<PiggyGoldItem>();
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.DisableDecraft();
        }
        #endregion

        #region Miscellaneous Items
        private static void AddMiscItemRecipes()
        {
            // Bloody Tear
            Recipe r = Recipe.Create(ItemID.BloodMoonStarter);
            r.AddIngredient<BloodOrb>(10);
            r.AddRecipeGroup("AnyCopperBar", 3);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Snow Globe (Frost Legion) recipe for consistency
            r = Recipe.Create(ItemID.SnowGlobe);
            r.AddRecipeGroup(AnySnowBlock, 10);
            r.AddIngredient(ItemID.Glass, 5);
            r.AddIngredient(ItemID.SoulofLight, 3);
            r.AddIngredient(ItemID.SoulofNight, 3);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Umbrella (for Temporal Umbrella)
            r = Recipe.Create(ItemID.Umbrella);
            r.AddIngredient(ItemID.Silk, 5);
            r.AddRecipeGroup("AnyCopperBar", 2);
            r.AddTile(TileID.Loom);
            r.Register();

            // Lower half Desert items (these are partially destroyed by Sunken Sea)
            // Bast Statue
            r = Recipe.Create(ItemID.CatBast);
            r.AddRecipeGroup("IronBar", 7);
            r.AddRecipeGroup("AnyGoldBar", 3);
            r.AddIngredient(ItemID.Ruby);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Encumbering Stone
            r = Recipe.Create(ItemID.EncumberingStone);
            r.AddRecipeGroup("AnyStoneBlock", 100);
            r.AddTile(TileID.Anvils);
            r.Register();
        }
        #endregion

        #region Early Game Weapons
        private static void AddEarlyGameWeaponRecipes()
        {
            // Wooden Chest weapons
            // Wooden Boomerang
            Recipe r = Recipe.Create(ItemID.WoodenBoomerang);
            r.AddRecipeGroup("Wood", 7);
            r.AddTile(TileID.WorkBenches);
            r.Register();

            // Wand of Sparking
            r = Recipe.Create(ItemID.WandofSparking);
            r.AddRecipeGroup("Wood", 5);
            r.AddIngredient(ItemID.Torch, 3);
            r.AddIngredient(ItemID.FallenStar);
            r.AddCondition(Condition.NotRemixWorld);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Early game minions
            // Finch Staff
            r = Recipe.Create(ItemID.BabyBirdStaff);
            r.AddIngredient(ItemID.Bird);
            r.AddRecipeGroup("Wood", 8);
            r.AddTile(TileID.WorkBenches);
            r.Register();

            // Slime Staff
            r = Recipe.Create(ItemID.SlimeStaff);
            r.AddRecipeGroup("Wood", 6);
            r.AddIngredient(ItemID.Gel, 40);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Key sword components
            // Enchanted Sword
            r = Recipe.Create(ItemID.EnchantedSword);
            r.AddIngredient<PearlShard>(10);
            r.AddRecipeGroup(AnyGoldBar, 12);
            r.AddIngredient(ItemID.Diamond);
            r.AddIngredient(ItemID.Ruby);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Starfury
            r = Recipe.Create(ItemID.Starfury);
            r.AddIngredient<AerialiteBar>(7);
            r.AddIngredient(ItemID.FallenStar, 10);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Muramasa
            r = Recipe.Create(ItemID.Muramasa);
            r.AddIngredient<AerialiteBar>(7);
            r.AddIngredient(ItemID.Bone, 10);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Water Bolt
            r = Recipe.Create(ItemID.WaterBolt);
            r.AddIngredient(ItemID.SpellTome);
            r.AddIngredient(ItemID.Waterleaf, 3);
            r.AddIngredient(ItemID.WaterCandle);
            r.AddTile(TileID.Bookcases);
            r.Register();
            r.DisableDecraft();
        }
        #endregion

        #region Early Game Accessories
        private static void AddEarlyGameAccessoryRecipes()
        {
            // Step Stool (replaced Chest item)
            Recipe r = Recipe.Create(ItemID.PortableStool);
            r.AddRecipeGroup("Wood", 10);
            r.AddTile(TileID.Sawmill);
            r.Register();

            #region Terraspark Boots Line
            // Hermes Boots
            r = Recipe.Create(ItemID.HermesBoots);
            r.AddIngredient(ItemID.Silk, 10);
            r.AddIngredient(ItemID.SwiftnessPotion, 5);
            r.AddTile(TileID.Loom);
            r.Register();

            // Aglet
            r = Recipe.Create(ItemID.Aglet);
            r.AddRecipeGroup(AnyCopperBar, 5);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Anklet of the Wind
            r = Recipe.Create(ItemID.AnkletoftheWind);
            r.AddIngredient(ItemID.JungleSpores, 15);
            r.AddIngredient(ItemID.Cloud, 5);
            r.AddIngredient(ItemID.PinkGel, 5);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Ice Skates
            r = Recipe.Create(ItemID.IceSkates);
            r.AddIngredient(ItemID.FlinxFur, 3);
            r.AddRecipeGroup("IronBar", 5);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Water Walking Boots
            r = Recipe.Create(ItemID.WaterWalkingBoots);
            r.AddIngredient(ItemID.Leather, 5);
            r.AddIngredient(ItemID.WaterWalkingPotion, 5);
            r.AddTile(TileID.Loom);
            r.Register();

            // Lava Charm
            r = Recipe.Create(ItemID.LavaCharm);
            r.AddIngredient(ItemID.LavaBucket, 3);
            r.AddIngredient(ItemID.Obsidian, 5);
            r.AddRecipeGroup(AnyGoldBar, 5);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Obsidian Rose
            r = Recipe.Create(ItemID.ObsidianRose);
            r.AddIngredient(ItemID.JungleRose);
            r.AddIngredient(ItemID.Obsidian, 5);
            r.AddIngredient(ItemID.Hellstone, 5);
            r.AddTile(TileID.Anvils);
            r.Register();
            #endregion

            #region Core Movement Accessories
            // Blizzard in a Bottle
            r = Recipe.Create(ItemID.BlizzardinaBottle);
            r.AddIngredient(ItemID.Bottle);
            r.AddIngredient(ItemID.Cloud, 15);
            r.AddRecipeGroup(AnySnowBlock, 30);
            r.AddIngredient(ItemID.Feather, 3);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Cloud in a Bottle
            r = Recipe.Create(ItemID.CloudinaBottle);
            r.AddIngredient(ItemID.Bottle);
            r.AddIngredient(ItemID.Cloud, 30);
            r.AddIngredient(ItemID.Feather, 2);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Sandstorm in a Bottle
            r = Recipe.Create(ItemID.SandstorminaBottle);
            r.AddIngredient(ItemID.Bottle);
            r.AddIngredient(ItemID.Cloud, 15);
            r.AddRecipeGroup("Sand", 40);
            r.AddIngredient(ItemID.Feather, 3);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Fledgling Wings
            r = Recipe.Create(ItemID.CreativeWings);
            r.AddIngredient<AncientBoneDust>(2);
            r.AddIngredient(ItemID.Cloud, 5);
            r.AddIngredient(ItemID.Feather, 10);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Flying Carpet
            r = Recipe.Create(ItemID.FlyingCarpet);
            r.AddIngredient(ItemID.Silk, 10);
            r.AddIngredient(ItemID.AntlionMandible, 2);
            r.AddIngredient<PearlShard>(5);
            r.AddTile(TileID.Loom);
            r.Register();
            r.DisableDecraft();

            // Frog Leg
            r = Recipe.Create(ItemID.FrogLeg);
            r.AddIngredient(ItemID.Frog, 6);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Lucky Horseshoe
            r = Recipe.Create(ItemID.LuckyHorseshoe);
            r.AddRecipeGroup(AnyGoldBar, 8);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Shiny Red Balloon
            r = Recipe.Create(ItemID.ShinyRedBalloon);
            r.AddIngredient(ItemID.WhiteString);
            r.AddIngredient(ItemID.Cloud, 10);
            r.AddTile(TileID.Solidifier);
            r.Register();
            #endregion

            // Cobalt Shield
            r = Recipe.Create(ItemID.CobaltShield);
            r.AddRecipeGroup(AnyCobaltBar, 5);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Flame Waker Boots
            r = Recipe.Create(ItemID.FlameWakerBoots);
            r.AddIngredient(ItemID.Silk, 8);
            r.AddIngredient(ItemID.HellstoneBar, 5);
            r.AddIngredient(ItemID.Obsidian, 4);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Flower Boots
            r = Recipe.Create(ItemID.FlowerBoots);
            r.AddIngredient(ItemID.Silk, 7);
            r.AddIngredient(ItemID.JungleRose);
            r.AddIngredient(ItemID.JungleGrassSeeds, 5);
            r.AddTile(TileID.Loom);
            r.Register();

            // Hand Warmer
            r = Recipe.Create(ItemID.HandWarmer);
            r.AddIngredient(ItemID.Silk, 10);
            r.AddTile(TileID.Loom);
            r.Register();

            // Radar
            r = Recipe.Create(ItemID.Radar);
            r.AddRecipeGroup("IronBar", 5);
            r.AddTile(TileID.Anvils);
            r.Register();

            //Bobber
            r = Recipe.Create(ItemID.FishingBobber);
            r.AddIngredient<Driftwood>(10);
            r.AddIngredient(ItemID.BlackPearl);
            r.AddRecipeGroup("IronBar");
            r.AddTile(TileID.Anvils);
            r.Register();
        }
        #endregion

        #region Armor
        private static void AddArmorRecipes()
        {
            // Snow armor
            Recipe r = Recipe.Create(ItemID.EskimoHood);
            r.AddIngredient(ItemID.Silk, 4);
            r.AddIngredient(ItemID.FlinxFur, 1);
            r.AddTile(TileID.Loom);
            r.Register();

            r = Recipe.Create(ItemID.EskimoCoat);
            r.AddIngredient(ItemID.Silk, 8);
            r.AddIngredient(ItemID.FlinxFur, 2);
            r.AddTile(TileID.Loom);
            r.Register();

            r = Recipe.Create(ItemID.EskimoPants);
            r.AddIngredient(ItemID.Silk, 6);
            r.AddIngredient(ItemID.FlinxFur, 1);
            r.AddTile(TileID.Loom);
            r.Register();
        }
        #endregion

        #region Hardmode Items and Accessories
        private static void AddHardmodeItemRecipes()
        {
            // Pulse Bow
            Recipe r = Recipe.Create(ItemID.PulseBow);
            r.AddIngredient(ItemID.ShroomiteBar, 16);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();
            r.DisableDecraft();

            // Sergeant United Shield
            r = Recipe.Create(ItemID.BouncingShield);
            r.AddRecipeGroup(AnyCobaltBar, 12);
            r.AddIngredient(ItemID.SoulofLight, 4);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Tiershift Recon Scope to post Plantera.
            r = Recipe.Create(ItemID.ReconScope);
            r.AddIngredient(ItemID.RifleScope);
            r.AddIngredient(ItemID.PutridScent);
            r.AddTile(TileID.TinkerersWorkbench);
            r.Register();

            // Tiershift Mini Nuke 1s to post Moon Lord.
            r = Recipe.Create(ItemID.MiniNukeI, 333);
            r.AddIngredient(ItemID.RocketIII, 333);
            r.AddIngredient(ItemID.LunarBar);
            r.AddTile(TileID.LunarCraftingStation);
            r.Register();

            // Tiershift Mini Nuke 2s to post Moon Lord.
            r = Recipe.Create(ItemID.MiniNukeII, 333);
            r.AddIngredient(ItemID.RocketIV, 333);
            r.AddIngredient(ItemID.LunarBar);
            r.AddTile(TileID.LunarCraftingStation);
            r.Register();
        }
        #endregion

        #region Tombstone Recipes
        private static void AddTombstoneRecipes()
        {
            short[] woodenTombstones = new[]
            {
                ItemID.CrossGraveMarker,
                ItemID.GraveMarker
            };

            short[] stoneTombstones = new[]
            {
                ItemID.Gravestone,
                ItemID.Headstone,
                ItemID.Obelisk,
                ItemID.Tombstone
            };

            short[] goldenTombstones = new[]
            {
                ItemID.RichGravestone1,
                ItemID.RichGravestone2,
                ItemID.RichGravestone3,
                ItemID.RichGravestone4,
                ItemID.RichGravestone5
            };

            Recipe r;

            foreach (var tombstone in woodenTombstones)
            {
                r = Recipe.Create(tombstone);
                r.AddRecipeGroup(RecipeGroupID.Wood, 15);
                r.AddTile(TileID.Sawmill);
                r.Register();
                r.DisableDecraft();
            }

            foreach (var tombstone in stoneTombstones)
            {
                r = Recipe.Create(tombstone);
                r.AddRecipeGroup(AnyStoneBlock, 15);
                r.AddTile(TileID.HeavyWorkBench);
                r.Register();
                r.DisableDecraft();
            }

            foreach (var tombstone in goldenTombstones)
            {
                r = Recipe.Create(tombstone);
                r.AddRecipeGroup(AnyStoneBlock, 15);
                r.AddRecipeGroup(AnyGoldBar);
                r.AddTile(TileID.HeavyWorkBench);
                r.Register();
                r.DisableDecraft();
            }
        }
        #endregion
    }
}

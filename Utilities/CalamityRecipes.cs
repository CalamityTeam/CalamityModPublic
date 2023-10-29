using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Critters;
using CalamityMod.Items.Fishing.AstralCatches;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Items.Fishing.SunkenSeaCatches;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Tools;
using CalamityMod.Items.Weapons.Melee;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod
{
    // TODO -- Change this to a ModSystem so it does not need to be manually loaded from CalamityMod
    internal class CalamityRecipes
    {
        #region Recipe Group Definitions
        public static int HardmodeAnvil, HardmodeForge, AnyLargeGem, AnyFood;
        public static int AnyCopperBar, AnySilverBar, AnyGoldBar, AnyEvilBar, AnyCobaltBar, AnyMythrilBar, AnyAdamantiteBar;
        public static int AnyEvilPowder, Boss2Material, CursedFlameIchor, AnyEvilWater, AnyEvilFlask;
        public static int AnyStoneBlock, AnySnowBlock, AnyIceBlock, AnySiltBlock, AnyEvilBlock, AnyGoodBlock;
        public static int AnyWoodenSword, AnyHallowedHelmet, AnyHallowedPlatemail, AnyHallowedGreaves, AnyGoldCrown, LunarPickaxe, LunarHamaxe;
        public static int AnyManaFlower, AnyQuiver, AnyTombstone, AnyWings;

        private static void ModifyVanillaRecipeGroups()
        {
            // Twinklers count as Fireflies
            RecipeGroup firefly = RecipeGroup.recipeGroups[RecipeGroupID.Fireflies];
            firefly.ValidItems.Add(ModContent.ItemType<TwinklerItem>());

            // Astral, Sunken Sea and Sulphurous Sea Sand is Sand
            RecipeGroup sand = RecipeGroup.recipeGroups[RecipeGroupID.Sand];
            sand.ValidItems.Add(ModContent.ItemType<AstralSand>());
            sand.ValidItems.Add(ModContent.ItemType<EutrophicSand>());
            sand.ValidItems.Add(ModContent.ItemType<SulphurousSand>());

            // Acidwood is Wood
            RecipeGroup wood = RecipeGroup.recipeGroups[RecipeGroupID.Wood];
            wood.ValidItems.Add(ModContent.ItemType<Acidwood>());
            // Astral Monolith is decidedly not wood-like enough to be used as generic wood.
        }

        public static void AddRecipeGroups()
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

            // Large Gems (PvP tokens)
            group = new RecipeGroup(() => CalamityUtils.GetTextValue("Misc.RecipeGroup.AnyLargeGem"), new int[]
            {
                ItemID.LargeAmber,
                ItemID.LargeAmethyst,
                ItemID.LargeDiamond,
                ItemID.LargeEmerald,
                ItemID.LargeRuby,
                ItemID.LargeSapphire,
                ItemID.LargeTopaz
            });
            AnyLargeGem = RecipeGroup.RegisterGroup("AnyLargeGem", group);

            // Food
            group = new RecipeGroup(() => CalamityUtils.GetTextValue("Misc.RecipeGroup.AnyFood"), new int[]
            {
                ItemID.Apple,
                ItemID.AppleJuice,
                ItemID.ApplePie,
                ItemID.Apricot,
                ItemID.Bacon,
                ItemID.Banana,
                ItemID.BananaDaiquiri,
                ItemID.BananaSplit,
                ModContent.ItemType<Baguette>(),
                ItemID.BBQRibs,
                ItemID.BlackCurrant,
                ModContent.ItemType<BlasphemousDonut>(),
                ItemID.BloodOrange,
                ItemID.BloodyMoscato,
                ItemID.BowlofSoup,
                ItemID.BunnyStew,
                ItemID.Burger, // mmmmhmm borgor :borgorpat:
                ItemID.MilkCarton, // Carton of Milk
                ItemID.Cherry,
                ItemID.ChickenNugget,
                ItemID.ChocolateChipCookie,
                ItemID.ChristmasPudding,
                ItemID.Coconut,
                ItemID.CoffeeCup,
                ItemID.CookedFish,
                ItemID.CookedMarshmallow,
                ItemID.CookedShrimp,
                ItemID.CreamSoda,
                ModContent.ItemType<DeliciousMeat>(),
                ItemID.Dragonfruit,
                ItemID.Elderberry,
                ItemID.Escargot,
                ItemID.FriedEgg,
                ItemID.Fries,
                ItemID.FroggleBunwich,
                ItemID.BananaDaiquiri, // Frozen Banana Daiquiri
                ItemID.FruitJuice,
                ItemID.FruitSalad,
                ItemID.GingerbreadCookie,
                ItemID.GoldenDelight,
                ItemID.Grapes,
                ItemID.Grapefruit,
                ItemID.GrapeJuice,
                ItemID.GrilledSquirrel,
                ItemID.GrubSoup,
                ModContent.ItemType<HadalStew>(),
                ItemID.Hotdog,
                ItemID.IceCream,
                ItemID.Lemon,
                ItemID.Lemonade,
                ItemID.LobsterTail,
                ItemID.Mango,
                ItemID.Milkshake,
                ItemID.MonsterLasagna,
                ItemID.Nachos,
                ItemID.PadThai,
                ItemID.Peach,
                ItemID.PeachSangria,
                ItemID.Pho,
                ItemID.PinaColada,
                ItemID.Pineapple,
                ItemID.Pizza,
                ItemID.Plum,
                ItemID.PotatoChips,
                ItemID.PrismaticPunch,
                ItemID.PumpkinPie,
                ItemID.Rambutan,
                ItemID.RoastedBird,
                ItemID.RoastedDuck,
                ItemID.SauteedFrogLegs,
                ItemID.SeafoodDinner,
                ItemID.ShrimpPoBoy,
                ItemID.ShuckedOyster,
                ItemID.SmoothieofDarkness,
                ItemID.Spaghetti,
                ItemID.Starfruit,
                ItemID.Steak,
                ItemID.SugarCookie,
                ItemID.Sashimi,
                ItemID.Teacup,
                ItemID.TropicalSmoothie
            });
            AnyFood = RecipeGroup.RegisterGroup("AnyFood", group);
        }

        private static void AddOreAndBarRecipeGroups()
        {
            // Copper and Tin
            RecipeGroup group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.CopperBar)}", new int[]
            {
                ItemID.CopperBar,
                ItemID.TinBar
            });
            AnyCopperBar = RecipeGroup.RegisterGroup("AnyCopperBar", group);

            // Silver and Tungsten
            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.SilverBar)}", new int[]
            {
                ItemID.SilverBar,
                ItemID.TungstenBar,
            });
            AnySilverBar = RecipeGroup.RegisterGroup("AnySilverBar", group);

            // Gold and Platinum
            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.GoldBar)}", new int[]
            {
                ItemID.GoldBar,
                ItemID.PlatinumBar
            });
            AnyGoldBar = RecipeGroup.RegisterGroup("AnyGoldBar", group);

            // Demonite and Crimtane
            group = new RecipeGroup(() => CalamityUtils.GetTextValue("Misc.RecipeGroup.AnyEvilBar"), new int[]
            {
                ItemID.DemoniteBar,
                ItemID.CrimtaneBar
            });
            AnyEvilBar = RecipeGroup.RegisterGroup("AnyEvilBar", group);

            // Cobalt and Palladium
            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.CobaltBar)}", new int[]
            {
                ItemID.CobaltBar,
                ItemID.PalladiumBar
            });
            AnyCobaltBar = RecipeGroup.RegisterGroup("AnyCobaltBar", group);

            // Mythril and Orichalcum
            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.MythrilBar)}", new int[]
            {
                ItemID.MythrilBar,
                ItemID.OrichalcumBar
            });
            AnyMythrilBar = RecipeGroup.RegisterGroup("AnyMythrilBar", group);

            // Adamantite and Titanium
            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.AdamantiteBar)}", new int[]
            {
                ItemID.AdamantiteBar,
                ItemID.TitaniumBar
            });
            AnyAdamantiteBar = RecipeGroup.RegisterGroup("AnyAdamantiteBar", group);
        }

        private static void AddEvilBiomeItemRecipeGroups()
        {
            // Vile and Vicious Powder
            RecipeGroup group = new RecipeGroup(() => CalamityUtils.GetTextValue("Misc.RecipeGroup.AnyEvilPowder"), new int[]
            {
                ItemID.VilePowder,
                ItemID.ViciousPowder
            });
            AnyEvilPowder = RecipeGroup.RegisterGroup("AnyEvilPowder", group);

            // Shadow Scale and Tissue Sample
            group = new RecipeGroup(() => CalamityUtils.GetTextValue("Misc.RecipeGroup.Boss2Material"), new int[]
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

            // Flask of Cursed Flames and Flask of Ichor
            group = new RecipeGroup(() => CalamityUtils.GetTextValue("Misc.RecipeGroup.AnyEvilFlask"), new int[]
            {
                ItemID.FlaskofCursedFlames,
                ItemID.FlaskofIchor
            });
            AnyEvilFlask = RecipeGroup.RegisterGroup("AnyEvilFlask", group);
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
                ModContent.ItemType<AstralStone>()
            });
            AnyStoneBlock = RecipeGroup.RegisterGroup("AnyStoneBlock", group);

            // Vanilla Snow and Astral Snow
            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.SnowBlock)}", new int[]
            {
                ItemID.SnowBlock,
                ModContent.ItemType<AstralSnow>()
            });
            AnySnowBlock = RecipeGroup.RegisterGroup("AnySnowBlock", group);

            // Vanilla Ice and Astral Ice
            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.IceBlock)}", new int[]
            {
                ItemID.IceBlock,
                ItemID.PurpleIceBlock,
                ItemID.RedIceBlock,
                ItemID.PinkIceBlock,
                ModContent.ItemType<AstralIce>()
            });
            AnyIceBlock = RecipeGroup.RegisterGroup("AnyIceBlock", group);

            // Silt, Slush, and Astral Silt, for Ancient Fossil
            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.SiltBlock)}", new int[]
            {
                ItemID.SiltBlock,
                ItemID.SlushBlock,
                ModContent.ItemType<NovaeSlag>()
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
                ItemID.PearlwoodSword
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
                ModContent.ItemType<GenesisPickaxe>()
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

            // Wings for Celestial Tracers
            group = new RecipeGroup(() => CalamityUtils.GetTextValue("Misc.RecipeGroup.AnyWings"), new int[]
            {
                ItemID.DemonWings,
                ItemID.AngelWings,
                ItemID.RedsWings,
                ItemID.ButterflyWings,
                ItemID.FairyWings,
                ItemID.HarpyWings,
                ItemID.BoneWings,
                ItemID.FlameWings,
                ItemID.FrozenWings,
                ItemID.GhostWings,
                ItemID.SteampunkWings,
                ItemID.LeafWings,
                ItemID.BatWings,
                ItemID.BeeWings,
                ItemID.DTownsWings,
                ItemID.WillsWings,
                ItemID.CrownosWings,
                ItemID.CenxsWings,
                ItemID.TatteredFairyWings,
                ItemID.SpookyWings,
                ItemID.Hoverboard,
                ItemID.FestiveWings,
                ItemID.BeetleWings,
                ItemID.FinWings,
                ItemID.FishronWings,
                ItemID.MothronWings,
                ItemID.WingsSolar,
                ItemID.WingsVortex,
                ItemID.WingsNebula,
                ItemID.WingsStardust,
                ItemID.Yoraiz0rWings,
                ItemID.JimsWings,
                ItemID.SkiphsWings,
                ItemID.LokisWings,
                ItemID.BetsyWings,
                ItemID.ArkhalisWings,
                ItemID.LeinforsWings,
                ItemID.BejeweledValkyrieWing,
                ItemID.GhostarsWings,
                ItemID.GroxTheGreatWings,
                ItemID.FoodBarbarianWings,
                ItemID.SafemanWings,
                ItemID.CreativeWings,
                ItemID.RainbowWings,
                ItemID.LongRainbowTrailWings,
                ModContent.ItemType<SkylineWings>(),
                ModContent.ItemType<StarlightWings>(),
                ModContent.ItemType<AureateBooster>(),
                ModContent.ItemType<HadalMantle>(),
                ModContent.ItemType<TarragonWings>(),
                ModContent.ItemType<ExodusWings>(),
                ModContent.ItemType<HadarianWings>(),
                ModContent.ItemType<SilvaWings>()
            });
            AnyWings = RecipeGroup.RegisterGroup("AnyWings", group);
        }
        #endregion

        public static void AddRecipes()
        {
            EditVanillaRecipes();
            
            // Leather from Vertebrae
            Recipe.Create(ItemID.Leather).
                AddIngredient(ItemID.Vertebrae, 2).
                AddTile(TileID.WorkBenches).
                Register();

            // Fallen Stars from Stardust
            Recipe.Create(ItemID.FallenStar).
                AddIngredient<Stardust>(5).
                AddTile(TileID.Anvils).
                Register()
                .DisableDecraft();

            // Black Lens
            Recipe.Create(ItemID.BlackLens).
                AddIngredient(ItemID.Lens).
                AddIngredient(ItemID.BlackDye).
                AddTile(TileID.DyeVat).
                Register();

            // Earlier Rocket Is for early rocket weapons
            Recipe.Create(ItemID.RocketI, 100).
                AddRecipeGroup("IronBar").
                AddIngredient(ItemID.EmptyBullet, 100).
                AddIngredient(ItemID.ExplosivePowder, 4).
                AddTile(TileID.MythrilAnvil).
                Register();
            // and Rocket IIs (requires slightly more explosive powder)
            Recipe.Create(ItemID.RocketII, 100).
                AddRecipeGroup("IronBar").
                AddIngredient(ItemID.EmptyBullet, 100).
                AddIngredient(ItemID.ExplosivePowder, 5).
                AddTile(TileID.MythrilAnvil).
                Register();

            // Life Crystal
            Recipe.Create(ItemID.LifeCrystal).
                AddIngredient(ItemID.StoneBlock, 5).
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
                AddIngredient<DemonicBoneAsh>().
                AddIngredient(ItemID.HellstoneBar, 4).
                AddTile(TileID.Hellforge).
                Register();

            // Lava Fishing Hook
            Recipe.Create(ItemID.LavaFishingHook).
                AddIngredient(ItemID.Seashell).
                AddIngredient(ItemID.HellstoneBar, 10).
                AddTile(TileID.Hellforge).
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

            AddAstralClayRecipes();
            AddBloodOrbPotionRecipes();
            AddCookedFood();
            AddEssentialToolRecipes();
            AddSummonAndProgressionRecipes();
            AddTombstoneRecipes();
            AddEarlyGameWeaponRecipes();
            AddEarlyGameAccessoryRecipes();
            AddHardmodeItemRecipes();
            AddArmorRecipes();
            AddAnkhShieldRecipes();
            AddLivingWoodRecipes();
        }

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
            #pragma warning restore CS8321

            var edits = new Dictionary<Func<Recipe, bool>, Action<Recipe>>(128)
            {
                { Vanilla(ItemID.EnchantedBoomerang), Disable }, // Calamity adds its own recipe

                // Make various things cheaper (sorted by progression)
                { Vanilla(ItemID.Leather), ChangeIngredientStack(ItemID.RottenChunk, 2) },
                { Vanilla(ItemID.JestersArrow), JesterArrowRecipeEdit },
                { Vanilla(ItemID.TeleportationPotion), TeleportationPotionRecipeEdit },
                { Vanilla(ItemID.WormFood), WormFoodRecipeEdit },
                { Vanilla(ItemID.BloodySpine), BloodySpineRecipeEdit },
                { Vanilla(ItemID.GoblinBattleStandard), ChangeIngredientStack(ItemID.TatteredCloth, 5) },
                { Vanilla(ItemID.ChlorophyteBar), ChangeIngredientStack(ItemID.ChlorophyteOre, 4) },

                { Vanilla(ItemID.ShroomiteBar), ChangeIngredientStack(ItemID.GlowingMushroom, 5) },
                { Vanilla(ItemID.TrueNightsEdge), TrueNightsEdgeRecipeEdit },
                { Vanilla(ItemID.TrueExcalibur), ChangeIngredientStack(ItemID.ChlorophyteBar, 12) },

                // Tier lock various items to a higher tier (sorted by progression)
                { Vanilla(ItemID.BundleofBalloons), AddIngredient(ModContent.ItemType<AerialiteBar>(), 3) },
                { r => r.HasResult(ItemID.HorseshoeBundle) && !r.HasIngredient(ItemID.BundleofBalloons), AddIngredient(ModContent.ItemType<AerialiteBar>(), 3) },
                { Vanilla(ItemID.NightsEdge), AddIngredient(ModContent.ItemType<PurifiedGel>(), 5) },
                { Vanilla(ItemID.FairyBoots), AddIngredient(ItemID.SoulofLight, 5) },
                { Vanilla(ItemID.SpiritFlame), AddGroup(AnyAdamantiteBar, 2) },
                { Vanilla(ItemID.TerraBlade), AddIngredient(ModContent.ItemType<LivingShard>(), 12) },
                { Vanilla(ItemID.FireGauntlet), AddIngredient(ModContent.ItemType<ScoriaBar>(), 5) },
                { Vanilla(ItemID.Zenith), AddIngredient(ModContent.ItemType<AuricBar>(), 5) },

                // Tier unlock various items to a lower tier (sorted by progression)
                // Move a bunch of mythril anvil locked stuff in early HM to regular anvils to fit progression changes
                { VanillaEach(
                    ItemID.MechanicalEye, ItemID.MechanicalWorm, ItemID.MechanicalSkull, ItemID.MechdusaSummon,
                    ItemID.DaoofPow, ItemID.Chik, ItemID.MeteorStaff, ItemID.CoolWhip,
                    ItemID.AngelWings, ItemID.DemonWings, ItemID.FairyWings,
                    ItemID.CursedArrow, ItemID.CursedBullet, ItemID.IchorArrow, ItemID.IchorBullet),
                    ReplaceTile(TileID.MythrilAnvil, TileID.Anvils)
                },
                { Vanilla(ItemID.OpticStaff), RemoveIngredient(ItemID.HallowedBar) },

                // Swap hellstone recipe ordering (they have bars first and it's wrong and irritating)
                { VanillaEach(ItemID.Flamarang, ItemID.PhoenixBlaster), SwapIngredients(0, 1) },

                // Swap Beetle Armor recipe ordering (they have beetle husks first and it's wrong and irritating)
                { VanillaEach(ItemID.BeetleHelmet, ItemID.BeetleScaleMail, ItemID.BeetleShell, ItemID.BeetleLeggings), SwapIngredients(0, 1) },

                // Pumpkin & Frost Moon non linearity
                { Vanilla(ItemID.PumpkinMoonMedallion), RemoveIngredient(ItemID.HallowedBar) },
                { Vanilla(ItemID.NaughtyPresent), RemoveIngredient(ItemID.SoulofFright) },

                // Add 20 Souls of Flight to vanilla Luminite wings
                { VanillaEach(ItemID.WingsSolar, ItemID.WingsVortex, ItemID.WingsNebula, ItemID.WingsStardust), AddIngredient(ItemID.SoulofFlight, 20) },

                // Berserker's Glove recipe change now that it doesn't provide melee speed
                { Vanilla(ItemID.BerserkerGlove), ReplaceIngredient(ItemID.PowerGlove, ItemID.TitanGlove) },

                // Standardize the costs of all PHM ore armors to 10/16/14 head/chest/legs
                { Vanilla(ItemID.CopperHelmet), ChangeIngredientStack(ItemID.CopperBar, 10) },
                { Vanilla(ItemID.CopperChainmail), ChangeIngredientStack(ItemID.CopperBar, 16) },
                { Vanilla(ItemID.CopperGreaves), ChangeIngredientStack(ItemID.CopperBar, 14) },
                { Vanilla(ItemID.TinHelmet), ChangeIngredientStack(ItemID.TinBar, 10) },
                { Vanilla(ItemID.TinChainmail), ChangeIngredientStack(ItemID.TinBar, 16) },
                { Vanilla(ItemID.TinGreaves), ChangeIngredientStack(ItemID.TinBar, 14) },
                { Vanilla(ItemID.IronHelmet), ChangeIngredientStack(ItemID.IronBar, 10) },
                { Vanilla(ItemID.IronChainmail), ChangeIngredientStack(ItemID.IronBar, 16) },
                { Vanilla(ItemID.IronGreaves), ChangeIngredientStack(ItemID.IronBar, 14) },
                { Vanilla(ItemID.LeadHelmet), ChangeIngredientStack(ItemID.LeadBar, 10) },
                { Vanilla(ItemID.LeadChainmail), ChangeIngredientStack(ItemID.LeadBar, 16) },
                { Vanilla(ItemID.LeadGreaves), ChangeIngredientStack(ItemID.LeadBar, 14) },
                { Vanilla(ItemID.SilverHelmet), ChangeIngredientStack(ItemID.SilverBar, 10) },
                { Vanilla(ItemID.SilverChainmail), ChangeIngredientStack(ItemID.SilverBar, 16) },
                { Vanilla(ItemID.SilverGreaves), ChangeIngredientStack(ItemID.SilverBar, 14) },
                { Vanilla(ItemID.TungstenHelmet), ChangeIngredientStack(ItemID.TungstenBar, 10) },
                { Vanilla(ItemID.TungstenChainmail), ChangeIngredientStack(ItemID.TungstenBar, 16) },
                { Vanilla(ItemID.TungstenGreaves), ChangeIngredientStack(ItemID.TungstenBar, 14) },
                { Vanilla(ItemID.GoldHelmet), ChangeIngredientStack(ItemID.GoldBar, 10) },
                { Vanilla(ItemID.GoldChainmail), ChangeIngredientStack(ItemID.GoldBar, 16) },
                { Vanilla(ItemID.GoldGreaves), ChangeIngredientStack(ItemID.GoldBar, 14) },
                { Vanilla(ItemID.PlatinumHelmet), ChangeIngredientStack(ItemID.PlatinumBar, 10) },
                { Vanilla(ItemID.PlatinumChainmail), ChangeIngredientStack(ItemID.PlatinumBar, 16) },
                { Vanilla(ItemID.PlatinumGreaves), ChangeIngredientStack(ItemID.PlatinumBar, 14) },

                // Standardize the costs of all HM ore items to the following:
                // 10 BARS : Melee Helm, Ranged Helm, Magic Helm, Pickaxe, Drill, Waraxe, Chainsaw, Sword, Spear, Repeater
                // 20 BARS : Breastplate
                // 15 BARS : Leggings
                { VanillaEach(
                    ItemID.CobaltHelmet, ItemID.CobaltMask, ItemID.CobaltHat, ItemID.CobaltPickaxe, ItemID.CobaltDrill, ItemID.CobaltWaraxe, ItemID.CobaltChainsaw,
                    ItemID.CobaltSword, ItemID.CobaltNaginata, ItemID.CobaltRepeater),
                    ChangeIngredientStack(ItemID.CobaltBar, 10)
                },
                { Vanilla(ItemID.CobaltBreastplate), ChangeIngredientStack(ItemID.CobaltBar, 20) },
                { Vanilla(ItemID.CobaltLeggings), ChangeIngredientStack(ItemID.CobaltBar, 15) },

                { VanillaEach(
                    ItemID.PalladiumHelmet, ItemID.PalladiumMask, ItemID.PalladiumHeadgear, ItemID.PalladiumPickaxe, ItemID.PalladiumDrill, ItemID.PalladiumWaraxe, ItemID.PalladiumChainsaw,
                    ItemID.PalladiumSword, ItemID.PalladiumPike, ItemID.PalladiumRepeater),
                    ChangeIngredientStack(ItemID.PalladiumBar, 10)
                },
                { Vanilla(ItemID.PalladiumBreastplate), ChangeIngredientStack(ItemID.PalladiumBar, 20) },
                { Vanilla(ItemID.PalladiumLeggings), ChangeIngredientStack(ItemID.PalladiumBar, 15) },

                { VanillaEach(
                    ItemID.MythrilHelmet, ItemID.MythrilHat, ItemID.MythrilHood, ItemID.MythrilPickaxe, ItemID.MythrilDrill, ItemID.MythrilWaraxe, ItemID.MythrilChainsaw,
                    ItemID.MythrilSword, ItemID.MythrilHalberd, ItemID.MythrilRepeater),
                    ChangeIngredientStack(ItemID.MythrilBar, 10)
                },
                { Vanilla(ItemID.MythrilChainmail), ChangeIngredientStack(ItemID.MythrilBar, 20) },
                { Vanilla(ItemID.MythrilGreaves), ChangeIngredientStack(ItemID.MythrilBar, 15) },

                { VanillaEach(
                    ItemID.OrichalcumHelmet, ItemID.OrichalcumMask, ItemID.OrichalcumHeadgear, ItemID.OrichalcumPickaxe, ItemID.OrichalcumDrill, ItemID.OrichalcumWaraxe, ItemID.OrichalcumChainsaw,
                    ItemID.OrichalcumSword, ItemID.OrichalcumHalberd, ItemID.OrichalcumRepeater),
                    ChangeIngredientStack(ItemID.OrichalcumBar, 10)
                },
                { Vanilla(ItemID.OrichalcumBreastplate), ChangeIngredientStack(ItemID.OrichalcumBar, 20) },
                { Vanilla(ItemID.OrichalcumLeggings), ChangeIngredientStack(ItemID.OrichalcumBar, 15) },

                { VanillaEach(
                    ItemID.AdamantiteHelmet, ItemID.AdamantiteMask, ItemID.AdamantiteHeadgear, ItemID.AdamantitePickaxe, ItemID.AdamantiteDrill, ItemID.AdamantiteWaraxe, ItemID.AdamantiteChainsaw,
                    ItemID.AdamantiteSword, ItemID.AdamantiteGlaive, ItemID.AdamantiteRepeater),
                    ChangeIngredientStack(ItemID.AdamantiteBar, 10)
                },
                { Vanilla(ItemID.AdamantiteBreastplate), ChangeIngredientStack(ItemID.AdamantiteBar, 20) },
                { Vanilla(ItemID.AdamantiteLeggings), ChangeIngredientStack(ItemID.AdamantiteBar, 15) },

                { VanillaEach(
                    ItemID.TitaniumHelmet, ItemID.TitaniumMask, ItemID.TitaniumHeadgear, ItemID.TitaniumPickaxe, ItemID.TitaniumDrill, ItemID.TitaniumWaraxe, ItemID.TitaniumChainsaw,
                    ItemID.TitaniumSword, ItemID.TitaniumTrident, ItemID.TitaniumRepeater),
                    ChangeIngredientStack(ItemID.TitaniumBar, 10)
                },
                { Vanilla(ItemID.TitaniumBreastplate), ChangeIngredientStack(ItemID.TitaniumBar, 20) },
                { Vanilla(ItemID.TitaniumLeggings), ChangeIngredientStack(ItemID.TitaniumBar, 15) },
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
        #endregion

        #region Astral Clay
        private static void AddAstralClayRecipes()
        {
            // Intentionally excluding Red Brick and Red Stucco recipes

            // Bowl
            Recipe r = Recipe.Create(ItemID.Bowl);
            r.AddIngredient(ModContent.ItemType<AstralClay>(), 2);
            r.AddTile(TileID.Furnaces);
            r.Register();
            r.DisableDecraft();

            // Clay Pot
            r = Recipe.Create(ItemID.ClayPot);
            r.AddIngredient(ModContent.ItemType<AstralClay>(), 5);
            r.AddTile(TileID.Furnaces);
            r.Register();
            r.DisableDecraft();

            // Pink Vase
            r = Recipe.Create(ItemID.PinkVase);
            r.AddIngredient(ModContent.ItemType<AstralClay>(), 4);
            r.AddTile(TileID.Furnaces);
            r.Register();
            r.DisableDecraft();

            // Plate
            r = Recipe.Create(ItemID.FoodPlatter);
            r.AddIngredient(ModContent.ItemType<AstralClay>(), 2);
            r.AddTile(TileID.Furnaces);
            r.Register();
            r.DisableDecraft();

            // Teapot
            r = Recipe.Create(ItemID.TeaKettle);
            r.AddIngredient(ModContent.ItemType<AstralClay>(), 12);
            r.AddIngredient(ItemID.Bone, 12);
            r.AddTile(TileID.Furnaces);
            r.Register();
            r.DisableDecraft();

            // Wandering Jingasa
            r = Recipe.Create(ItemID.RoninHat);
            r.AddIngredient(ModContent.ItemType<AstralClay>(), 10);
            r.AddIngredient(ItemID.Firefly, 3); // Does not use the recipe group in Vanilla
            r.AddTile(TileID.Loom);
            r.Register();
            r.DisableDecraft();
        }
        #endregion

        #region Potions from Blood Orbs
        private static void AddBloodOrbPotionRecipes()
        {
            // List of vanilla potions which can be crafted with Blood Orbs
            short[] potions = new[]
            {
                ItemID.WormholePotion,
                ItemID.TeleportationPotion,
                ItemID.SwiftnessPotion,
                ItemID.FeatherfallPotion,
                ItemID.GravitationPotion,
                ItemID.ShinePotion,
                ItemID.InvisibilityPotion,
                ItemID.NightOwlPotion,
                ItemID.SpelunkerPotion,
                ItemID.HunterPotion,
                ItemID.TrapsightPotion,
                ItemID.BattlePotion,
                ItemID.CalmingPotion,
                ItemID.WrathPotion,
                ItemID.RagePotion,
                ItemID.ThornsPotion,
                ItemID.IronskinPotion,
                ItemID.EndurancePotion,
                ItemID.RegenerationPotion,
                ItemID.LifeforcePotion,
                ItemID.HeartreachPotion,
                ItemID.TitanPotion,
                ItemID.ArcheryPotion,
                ItemID.AmmoReservationPotion,
                ItemID.MagicPowerPotion,
                ItemID.ManaRegenerationPotion,
                ItemID.SummoningPotion,
                ItemID.InfernoPotion,
                ItemID.WarmthPotion,
                ItemID.ObsidianSkinPotion,
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
                ItemID.PotionOfReturn,
                ItemID.LuckPotionLesser,
                ItemID.BiomeSightPotion
            };
            Recipe r;

            foreach (var potion in potions)
            {
                r = Recipe.Create(potion);
                r.AddIngredient(ItemID.BottledWater);
                r.AddIngredient(ModContent.ItemType<BloodOrb>(), 10);
                r.AddTile(TileID.AlchemyTable);
                r.Register();
                r.DisableDecraft();
            }

            r = Recipe.Create(ItemID.LuckPotion);
            r.AddIngredient(ItemID.BottledWater);
            r.AddIngredient(ModContent.ItemType<BloodOrb>(), 20);
            r.AddTile(TileID.AlchemyTable);
            r.Register();
            r.DisableDecraft();

            r = Recipe.Create(ItemID.LuckPotionGreater);
            r.AddIngredient(ItemID.BottledWater);
            r.AddIngredient(ModContent.ItemType<BloodOrb>(), 30);
            r.AddTile(TileID.AlchemyTable);
            r.Register();
            r.DisableDecraft();
        }
        #endregion

        #region Cooked Food
        private static void AddCookedFood()
        {
            Recipe r = Recipe.Create(ItemID.CookedFish);
            r.AddIngredient(ModContent.ItemType<TwinklingPollox>());
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.DisableDecraft();

            r = Recipe.Create(ItemID.CookedFish);
            r.AddIngredient(ModContent.ItemType<PrismaticGuppy>());
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.DisableDecraft();

            r = Recipe.Create(ItemID.Sashimi);
            r.AddIngredient(ModContent.ItemType<CragBullhead>());
            r.AddTile(TileID.WorkBenches);
            r.Register();
            r.DisableDecraft();

            r = Recipe.Create(ItemID.CookedShrimp);
            r.AddIngredient(ModContent.ItemType<ProcyonidPrawn>());
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.DisableDecraft();

            r = Recipe.Create(ItemID.SeafoodDinner);
            r.AddIngredient(ModContent.ItemType<AldebaranAlewife>(), 2);
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.DisableDecraft();

            r = Recipe.Create(ItemID.SeafoodDinner);
            r.AddIngredient(ModContent.ItemType<Bloodfin>(), 2);
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.DisableDecraft();

            r = Recipe.Create(ItemID.SeafoodDinner);
            r.AddIngredient(ModContent.ItemType<CoastalDemonfish>(), 2);
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.DisableDecraft();

            r = Recipe.Create(ItemID.SeafoodDinner);
            r.AddIngredient(ModContent.ItemType<Shadowfish>(), 2);
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.DisableDecraft();

            r = Recipe.Create(ItemID.SeafoodDinner);
            r.AddIngredient(ModContent.ItemType<SunkenSailfish>(), 2);
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.DisableDecraft();

            r = Recipe.Create(ItemID.Bacon);
            r.AddIngredient(ModContent.ItemType<PiggyItem>());
            r.AddTile(TileID.Hellforge);
            r.Register();

            r = Recipe.Create(ItemID.BowlofSoup);
            r.AddIngredient(ItemID.Mushroom);
            r.AddIngredient(ModContent.ItemType<SeaMinnowItem>());
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.DisableDecraft();

            r = Recipe.Create(ItemID.ApplePie);
            r.AddIngredient(ItemID.Apple, 3);
            r.AddTile(TileID.Hellforge);
            r.Register();
            r.DisableDecraft();

            r = Recipe.Create(ItemID.BananaSplit);
            r.AddIngredient(ItemID.Banana);
            r.AddIngredient(ItemID.IceBlock);
            r.AddIngredient(ItemID.MilkCarton);
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.DisableDecraft();

            r = Recipe.Create(ItemID.BBQRibs);
            r.AddIngredient(ItemID.FleshBlock, 6);
            r.AddIngredient(ItemID.Bone, 6);
            r.AddTile(TileID.Hellforge);
            r.Register();
            r.DisableDecraft();

            r = Recipe.Create(ItemID.MilkCarton);
            r.AddIngredient(ItemID.BottledWater);
            r.AddIngredient(ItemID.Bone, 2);
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.DisableDecraft();

            r = Recipe.Create(ItemID.Nachos);
            r.AddIngredient(ItemID.PotatoChips);
            r.AddIngredient(ItemID.MilkCarton);
            r.AddTile(TileID.CookingPots);
            r.Register();
            r.DisableDecraft();
        }
        #endregion

        #region Essential Gameplay Tools
        private static void AddEssentialToolRecipes()
        {
            // Umbrella
            Recipe r = Recipe.Create(ItemID.Umbrella);
            r.AddIngredient(ItemID.Silk, 5);
            r.AddRecipeGroup("AnyCopperBar", 2);
            r.AddTile(TileID.Loom);
            r.Register();
            r.DisableDecraft();

            // Bug Net
            r = Recipe.Create(ItemID.BugNet);
            r.AddIngredient(ItemID.Cobweb, 30);
            r.AddRecipeGroup("AnyCopperBar", 3);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Bloody Tear
            r = Recipe.Create(ItemID.BloodMoonStarter);
            r.AddIngredient(ModContent.ItemType<BloodOrb>(), 10);
            r.AddRecipeGroup("AnyCopperBar", 3);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Staff of Regrowth
            r = Recipe.Create(ItemID.StaffofRegrowth);
            r.AddIngredient(ItemID.RichMahogany, 10);
            r.AddIngredient(ItemID.JungleSpores, 5);
            r.AddIngredient(ItemID.JungleRose);
            r.AddTile(TileID.WorkBenches);
            r.Register();
            r.DisableDecraft();

            // Shadow Key
            r = Recipe.Create(ItemID.ShadowKey);
            r.AddIngredient(ItemID.GoldenKey);
            r.AddIngredient(ItemID.Obsidian, 20);
            r.AddIngredient(ItemID.Bone, 5);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Sky Mill
            r = Recipe.Create(ItemID.SkyMill);
            r.AddIngredient(ItemID.SunplateBlock, 10);
            r.AddIngredient(ItemID.Cloud, 5);
            r.AddIngredient(ItemID.RainCloud, 3);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Ice Machine
            r = Recipe.Create(ItemID.IceMachine);
            r.AddRecipeGroup("AnyIceBlock", 25);
            r.AddRecipeGroup("AnySnowBlock", 15);
            r.AddRecipeGroup("IronBar", 3);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Bast Statue
            r = Recipe.Create(ItemID.CatBast);
            r.AddRecipeGroup("IronBar", 7);
            r.AddRecipeGroup("AnyGoldBar", 3);
            r.AddIngredient(ItemID.Ruby);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Encumbering Stone
            r = Recipe.Create(ItemID.EncumberingStone);
            r.AddIngredient(ItemID.StoneBlock, 100);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Desert Minecart
            r = Recipe.Create(ItemID.DesertMinecart);
            r.AddIngredient(ItemID.SandstoneBrick, 20);
            r.AddRecipeGroup("AnyGoldBar", 6);
            r.AddRecipeGroup("IronBar", 3);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();
        }
        #endregion

        #region Boss Summon and Progression Items
        private static void AddSummonAndProgressionRecipes()
        {
            // Guide Voodoo Doll
            Recipe r = Recipe.Create(ItemID.GuideVoodooDoll);
            r.AddIngredient(ItemID.Leather, 2);
            r.AddRecipeGroup(AnyEvilPowder, 10);
            r.AddTile(TileID.Hellforge);
            r.Register();
            r.DisableDecraft();

            // Frost Legion recipe for consistency
            r = Recipe.Create(ItemID.SnowGlobe);
            r.AddRecipeGroup(AnySnowBlock, 50);
            r.AddIngredient(ItemID.Glass, 10);
            r.AddIngredient(ItemID.SoulofLight, 3);
            r.AddIngredient(ItemID.SoulofNight, 3);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Gelatin Crystal (Queen Slime summon)
            r = Recipe.Create(ItemID.QueenSlimeCrystal);
            r.AddIngredient(ItemID.CrystalShard, 20);
            r.AddIngredient(ItemID.PinkGel, 10);
            r.AddIngredient(ItemID.SoulofLight, 5);
            r.AddTile(TileID.Solidifier);
            r.Register();
            r.DisableDecraft();

            // Temple Key
            r = Recipe.Create(ItemID.TempleKey);
            r.AddIngredient(ItemID.JungleSpores, 15);
            r.AddIngredient(ItemID.RichMahogany, 15);
            r.AddIngredient(ItemID.SoulofLight, 15);
            r.AddIngredient(ItemID.SoulofNight, 15);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();
            r.DisableDecraft();

            // Lihzahrd Power Cell (NOT Calamity's Old Power Cell)
            r = Recipe.Create(ItemID.LihzahrdPowerCell);
            r.AddIngredient(ItemID.LihzahrdBrick, 15);
            r.AddIngredient(ModContent.ItemType<CoreofSunlight>());
            r.AddTile(TileID.LihzahrdFurnace);
            r.Register();
            r.DisableDecraft();
        }
        #endregion

        #region Early Game Weapons
        private static void AddEarlyGameWeaponRecipes()
        {
            // Shuriken
            Recipe r = Recipe.Create(ItemID.Shuriken, 50);
            r.AddRecipeGroup("IronBar");
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Throwing Knife
            r = Recipe.Create(ItemID.ThrowingKnife, 50);
            r.AddRecipeGroup("IronBar");
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Wooden Boomerang
            r = Recipe.Create(ItemID.WoodenBoomerang);
            r.AddIngredient(ItemID.Wood, 7);
            r.AddTile(TileID.WorkBenches);
            r.Register();
            r.DisableDecraft();

            // Wand of Sparking
            r = Recipe.Create(ItemID.WandofSparking);
            r.AddIngredient(ItemID.Wood, 5);
            r.AddIngredient(ItemID.Torch, 3);
            r.AddIngredient(ItemID.FallenStar);
            r.AddCondition(Condition.NotRemixWorld);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Finch Staff
            r = Recipe.Create(ItemID.BabyBirdStaff);
            r.AddIngredient(ItemID.Bird);
            r.AddRecipeGroup("Wood", 8);
            r.AddTile(TileID.WorkBenches);
            r.Register();
            r.DisableDecraft();

            // Starfury w/ Gold Broadsword
            r = Recipe.Create(ItemID.Starfury);
            r.AddIngredient(ItemID.GoldBroadsword);
            r.AddIngredient(ItemID.FallenStar, 10);
            r.AddIngredient(ModContent.ItemType<PearlShard>(), 3);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Starfury w/ Platinum Broadsword
            r = Recipe.Create(ItemID.Starfury);
            r.AddIngredient(ItemID.PlatinumBroadsword);
            r.AddIngredient(ItemID.FallenStar, 10);
            r.AddIngredient(ModContent.ItemType<PearlShard>(), 3);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Enchanted Boomerang
            r = Recipe.Create(ItemID.EnchantedBoomerang);
            r.AddIngredient(ItemID.WoodenBoomerang);
            r.AddIngredient(ModContent.ItemType<PearlShard>(), 6);
            r.AddRecipeGroup(AnyGoldBar, 8);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Enchanted Sword
            r = Recipe.Create(ItemID.EnchantedSword);
            r.AddIngredient(ModContent.ItemType<PearlShard>(), 10);
            r.AddRecipeGroup(AnyGoldBar, 12);
            r.AddIngredient(ItemID.Diamond);
            r.AddIngredient(ItemID.Ruby);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Muramasa
            r = Recipe.Create(ItemID.Muramasa);
            r.AddIngredient(ModContent.ItemType<AerialiteBar>(), 7);
            r.AddIngredient(ItemID.Bone, 10);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Water Bolt w/ Hardmode Spell Tome
            r = Recipe.Create(ItemID.WaterBolt);
            r.AddIngredient(ItemID.SpellTome);
            r.AddIngredient(ItemID.Waterleaf, 3);
            r.AddIngredient(ItemID.WaterCandle);
            r.AddTile(TileID.Bookcases);
            r.Register();
            r.DisableDecraft();

            // Slime Staff
            r = Recipe.Create(ItemID.SlimeStaff);
            r.AddRecipeGroup("Wood", 6);
            r.AddIngredient(ItemID.Gel, 40);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Ice Boomerang
            r = Recipe.Create(ItemID.IceBoomerang);
            r.AddIngredient(ItemID.WoodenBoomerang);
            r.AddRecipeGroup(AnyIceBlock, 20);
            r.AddRecipeGroup(AnySnowBlock, 10);
            r.AddIngredient(ItemID.Shiverthorn);
            r.AddTile(TileID.IceMachine);
            r.Register();
            r.DisableDecraft();
        }
        #endregion

        #region Early Game Accessories
        private static void AddEarlyGameAccessoryRecipes()
        {
            // Cloud in a Bottle
            Recipe r = Recipe.Create(ItemID.CloudinaBottle);
            r.AddIngredient(ItemID.Bottle);
            r.AddIngredient(ItemID.Cloud, 25);
            r.AddIngredient(ItemID.Feather, 2);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Step Stool
            r = Recipe.Create(ItemID.PortableStool);
            r.AddRecipeGroup("Wood", 15);
            r.AddTile(TileID.Sawmill);
            r.Register();
            r.DisableDecraft();

            // Hermes Boots
            r = Recipe.Create(ItemID.HermesBoots);
            r.AddIngredient(ItemID.Silk, 10);
            r.AddIngredient(ItemID.SwiftnessPotion, 5);
            r.AddTile(TileID.Loom);
            r.Register();
            r.DisableDecraft();

            // Blizzard in a Bottle
            r = Recipe.Create(ItemID.BlizzardinaBottle);
            r.AddIngredient(ItemID.Bottle);
            r.AddRecipeGroup(AnySnowBlock, 50);
            r.AddIngredient(ItemID.Feather, 3);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Sandstorm in a Bottle
            r = Recipe.Create(ItemID.SandstorminaBottle);
            r.AddIngredient(ItemID.Bottle);
            r.AddIngredient(ItemID.SandBlock, 70);
            r.AddIngredient(ItemID.AntlionMandible, 5);
            r.AddIngredient(ItemID.Feather, 3);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Frog Leg
            r = Recipe.Create(ItemID.FrogLeg);
            r.AddIngredient(ItemID.Frog, 6);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Flying Carpet
            r = Recipe.Create(ItemID.FlyingCarpet);
            r.AddIngredient(ItemID.Silk, 10);
            r.AddIngredient(ItemID.AntlionMandible, 2);
            r.AddIngredient(ModContent.ItemType<PearlShard>(), 5);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Aglet
            r = Recipe.Create(ItemID.Aglet);
            r.AddRecipeGroup(AnyCopperBar, 5);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Anklet of the Wind
            r = Recipe.Create(ItemID.AnkletoftheWind);
            r.AddIngredient(ItemID.JungleSpores, 15);
            r.AddIngredient(ItemID.Cloud, 15);
            r.AddIngredient(ItemID.PinkGel, 5);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Water Walking Boots
            r = Recipe.Create(ItemID.WaterWalkingBoots);
            r.AddIngredient(ItemID.Leather, 5);
            r.AddIngredient(ItemID.WaterWalkingPotion, 5);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Flame Walker Boots
            r = Recipe.Create(ItemID.FlameWakerBoots);
            r.AddIngredient(ItemID.Silk, 8);
            r.AddIngredient(ItemID.Obsidian, 2);
            r.AddTile(TileID.Loom);
            r.Register();
            r.DisableDecraft();

            // Ice Skates
            r = Recipe.Create(ItemID.IceSkates);
            r.AddRecipeGroup(AnyIceBlock, 20);
            r.AddIngredient(ItemID.Leather, 5);
            r.AddRecipeGroup("IronBar", 5);
            r.AddTile(TileID.IceMachine);
            r.Register();
            r.DisableDecraft();

            // Lucky Horseshoe
            r = Recipe.Create(ItemID.LuckyHorseshoe);
            r.AddRecipeGroup(AnyGoldBar, 8);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Shiny Red Balloon
            r = Recipe.Create(ItemID.ShinyRedBalloon);
            r.AddIngredient(ItemID.WhiteString);
            r.AddIngredient(ItemID.Gel, 60);
            r.AddIngredient(ItemID.Cloud, 20);
            r.AddTile(TileID.Solidifier);
            r.Register();
            r.DisableDecraft();

            // Lava Charm
            r = Recipe.Create(ItemID.LavaCharm);
            r.AddIngredient(ItemID.LavaBucket, 3);
            r.AddIngredient(ItemID.Obsidian, 25);
            r.AddRecipeGroup(AnyGoldBar, 5);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Obsidian Rose
            r = Recipe.Create(ItemID.ObsidianRose);
            r.AddIngredient(ItemID.JungleRose);
            r.AddIngredient(ItemID.Obsidian, 10);
            r.AddIngredient(ItemID.Hellstone, 10);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Feral Claws
            r = Recipe.Create(ItemID.FeralClaws);
            r.AddIngredient(ItemID.Leather, 10);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Radar
            r = Recipe.Create(ItemID.Radar);
            r.AddRecipeGroup("IronBar", 5);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Metal Detector
            r = Recipe.Create(ItemID.MetalDetector);
            r.AddIngredient(ItemID.Wire, 10);
            r.AddIngredient(ItemID.SpelunkerGlowstick, 5);
            r.AddRecipeGroup(AnyCopperBar, 5);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // DPS Meter
            r = Recipe.Create(ItemID.DPSMeter);
            r.AddIngredient(ItemID.Wire, 10);
            r.AddRecipeGroup(AnyGoldBar, 5);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Hand Warmer
            r = Recipe.Create(ItemID.HandWarmer);
            r.AddIngredient(ItemID.Silk, 5);
            r.AddIngredient(ItemID.Shiverthorn);
            r.AddRecipeGroup(AnySnowBlock, 10);
            r.AddTile(TileID.Loom);
            r.Register();
            r.DisableDecraft();

            // Flower Boots
            r = Recipe.Create(ItemID.FlowerBoots);
            r.AddIngredient(ItemID.Silk, 7);
            r.AddIngredient(ItemID.JungleRose);
            r.AddIngredient(ItemID.JungleGrassSeeds, 5);
            r.AddTile(TileID.Loom);
            r.Register();
            r.DisableDecraft();

            // Band of Regeneration
            r = Recipe.Create(ItemID.BandofRegeneration);
            r.AddIngredient(ItemID.Shackle);
            r.AddIngredient(ItemID.LifeCrystal, 1);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Shoe Spikes
            r = Recipe.Create(ItemID.ShoeSpikes);
            r.AddRecipeGroup("IronBar", 5);
            r.AddIngredient(ItemID.Spike, 10);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Flare Gun
            r = Recipe.Create(ItemID.FlareGun);
            r.AddRecipeGroup(AnyCopperBar, 5);
            r.AddIngredient(ItemID.Torch, 10);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();
        }
        #endregion

        #region Armor
        private static void AddArmorRecipes()
        {
            // Eskimo armor
            Recipe r = Recipe.Create(ItemID.EskimoHood);
            r.AddIngredient(ItemID.Silk, 4);
            r.AddIngredient(ItemID.Leather);
            r.AddIngredient(ItemID.BorealWood, 12);
            r.AddTile(TileID.Loom);
            r.Register();
            r.DisableDecraft();

            r = Recipe.Create(ItemID.EskimoCoat);
            r.AddIngredient(ItemID.Silk, 8);
            r.AddIngredient(ItemID.Leather);
            r.AddIngredient(ItemID.BorealWood, 18);
            r.AddTile(TileID.Loom);
            r.Register();
            r.DisableDecraft();

            r = Recipe.Create(ItemID.EskimoPants);
            r.AddIngredient(ItemID.Silk, 6);
            r.AddIngredient(ItemID.Leather);
            r.AddIngredient(ItemID.BorealWood, 15);
            r.AddTile(TileID.Loom);
            r.Register();
            r.DisableDecraft();

            // Pharaoh set
            r = Recipe.Create(ItemID.PharaohsMask);
            r.AddIngredient(ItemID.AncientCloth, 3);
            r.AddTile(TileID.Loom);
            r.Register();
            r.DisableDecraft();

            r = Recipe.Create(ItemID.PharaohsRobe);
            r.AddIngredient(ItemID.AncientCloth, 4);
            r.AddTile(TileID.Loom);
            r.Register();
            r.DisableDecraft();
        }
        #endregion

        #region Ankh Shield Components
        private static void AddAnkhShieldRecipes()
        {
            // Cobalt Shield
            Recipe r = Recipe.Create(ItemID.CobaltShield);
            r.AddRecipeGroup(AnyCobaltBar, 10);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Armor Polish (broken armor)
            r = Recipe.Create(ItemID.ArmorPolish);
            r.AddIngredient(ItemID.Bone, 50);
            r.AddIngredient(ModContent.ItemType<AncientBoneDust>(), 3);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Adhesive Bandage (bleeding)
            r = Recipe.Create(ItemID.AdhesiveBandage);
            r.AddIngredient(ItemID.Silk, 10);
            r.AddIngredient(ItemID.Gel, 50);
            r.AddIngredient(ItemID.HealingPotion);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Bezoar (poison)
            r = Recipe.Create(ItemID.Bezoar);
            r.AddIngredient(ModContent.ItemType<MurkyPaste>(), 3);
            r.AddIngredient(ItemID.Stinger, 7);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Nazar (curse)
            r = Recipe.Create(ItemID.Nazar);
            r.AddIngredient(ItemID.SoulofNight, 15);
            r.AddIngredient(ItemID.Lens, 3);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Vitamins (weakness)
            r = Recipe.Create(ItemID.Vitamins);
            r.AddIngredient(ItemID.BottledWater);
            r.AddIngredient(ItemID.Waterleaf, 5);
            r.AddIngredient(ItemID.Blinkroot, 5);
            r.AddIngredient(ItemID.Daybloom, 5);
            r.AddIngredient(ModContent.ItemType<BloodOrb>(), 10);
            r.AddTile(TileID.AlchemyTable);
            r.Register();
            r.DisableDecraft();

            // Blindfold (darkness)
            r = Recipe.Create(ItemID.Blindfold);
            r.AddIngredient(ItemID.Silk, 30);
            r.AddIngredient(ItemID.TatteredCloth, 3);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Trifold Map (confusion)
            r = Recipe.Create(ItemID.TrifoldMap);
            r.AddIngredient(ItemID.Silk, 20);
            r.AddIngredient(ItemID.SoulofLight, 3);
            r.AddIngredient(ItemID.SoulofNight, 3);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Fast Clock (slow)
            r = Recipe.Create(ItemID.FastClock);
            r.AddIngredient(ItemID.Timer1Second);
            r.AddIngredient(ItemID.PixieDust, 15);
            r.AddIngredient(ItemID.SoulofLight, 5);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Megaphone (silence)
            r = Recipe.Create(ItemID.Megaphone);
            r.AddIngredient(ItemID.Wire, 10);
            r.AddRecipeGroup(AnyCobaltBar, 5);
            r.AddIngredient(ItemID.Ruby, 3);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Pocket Mirror (petrification)
            r = Recipe.Create(ItemID.PocketMirror);
            r.AddIngredient(ItemID.Glass, 10);
            r.AddRecipeGroup(AnyGoldBar, 4);
            r.AddIngredient(ItemID.CrystalShard, 2);
            r.AddIngredient(ItemID.SoulofNight, 2);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();
        }
        #endregion

        #region Living Wood
        private static void AddLivingWoodRecipes()
        {
            // Living Loom
            Recipe r = Recipe.Create(ItemID.LivingLoom);
            r.AddIngredient(ItemID.Loom);
            r.AddIngredient(ItemID.Vine, 2);
            r.AddTile(TileID.Sawmill);
            r.Register();
            r.DisableDecraft();

            // Living Wood Wand
            r = Recipe.Create(ItemID.LivingWoodWand);
            r.AddIngredient(ItemID.Wood, 30);
            r.AddTile(TileID.LivingLoom);
            r.Register();
            r.DisableDecraft();

            // Living Leaf Wand
            r = Recipe.Create(ItemID.LeafWand);
            r.AddIngredient(ItemID.Wood, 30);
            r.AddTile(TileID.LivingLoom);
            r.Register();
            r.DisableDecraft();

            // Living Mahogany Wand
            r = Recipe.Create(ItemID.LivingMahoganyWand);
            r.AddIngredient(ItemID.RichMahogany, 30);
            r.AddTile(TileID.LivingLoom);
            r.Register();
            r.DisableDecraft();

            // Living Mahogany Leaf Wand
            r = Recipe.Create(ItemID.LivingMahoganyLeafWand);
            r.AddIngredient(ItemID.RichMahogany, 30);
            r.AddTile(TileID.LivingLoom);
            r.Register();
            r.DisableDecraft();
        }
        #endregion

        #region Hardmode Items and Accessories
        private static void AddHardmodeItemRecipes()
        {
            // Celestial Magnet
            Recipe r = Recipe.Create(ItemID.CelestialMagnet);
            r.AddIngredient(ItemID.TreasureMagnet);
            r.AddIngredient(ItemID.FallenStar, 5);
            r.AddTile(TileID.Anvils);
            r.Register();
            r.DisableDecraft();

            // Frozen Turtle Shell
            r = Recipe.Create(ItemID.FrozenTurtleShell);
            r.AddIngredient(ItemID.TurtleShell, 3);
            r.AddIngredient(ModContent.ItemType<EssenceofEleum>(), 9);
            r.AddTile(TileID.IceMachine);
            r.Register();
            r.DisableDecraft();

            // Magic Quiver
            r = Recipe.Create(ItemID.MagicQuiver);
            r.AddIngredient(ItemID.EndlessQuiver);
            r.AddIngredient(ItemID.PixieDust, 10);
            r.AddIngredient(ItemID.Lens, 5);
            r.AddIngredient(ItemID.SoulofLight, 8);
            r.AddTile(TileID.CrystalBall);
            r.Register();
            r.DisableDecraft();

            // Pulse Bow
            r = Recipe.Create(ItemID.PulseBow);
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

            // Mechanical Glove directly from correct class emblem for cheaper (compromise)
            r = Recipe.Create(ItemID.MechanicalGlove);
            r.AddIngredient(ItemID.PowerGlove);
            r.AddIngredient(ItemID.WarriorEmblem);
            r.AddIngredient(ItemID.SoulofFright);
            r.AddIngredient(ItemID.SoulofMight);
            r.AddIngredient(ItemID.SoulofSight);
            r.Register();
            r.DisableDecraft();

            // Celestial Emblem directly from correct class emblem for cheaper (compromise)
            r = Recipe.Create(ItemID.CelestialEmblem);
            r.AddIngredient(ItemID.CelestialMagnet);
            r.AddIngredient(ItemID.SorcererEmblem);
            r.AddIngredient(ItemID.SoulofFright);
            r.AddIngredient(ItemID.SoulofMight);
            r.AddIngredient(ItemID.SoulofSight);
            r.Register();
            r.DisableDecraft();
        }
        #endregion

        #region Shimmer Recipes
        /// <summary>
        /// Adds a shimmer recipe, while having the result transform into the ingredient's original result.
        /// <para>This is used for inserting items into various shimmer result trees/loops, like the Class Emblem loop.</para>
        /// </summary>
        public static void InsertShimmerResult(int result, int ingredient)
        {
            ItemID.Sets.ShimmerTransformToItem[result] = ItemID.Sets.ShimmerTransformToItem[ingredient];
            ItemID.Sets.ShimmerTransformToItem[ingredient] = result;
        }

        public static void AddShimmerRecipes()
        {
            // shorthand for the ID set
            int[] convert = ItemID.Sets.ShimmerTransformToItem;

            InsertShimmerResult(ModContent.ItemType<RogueEmblem>(), ItemID.SummonerEmblem);

            // Pyramid loot edits. There's no good way to have this not be hardcoded.
            convert[ItemID.PharaohsMask] = ItemID.PharaohsRobe;
            convert[ItemID.PharaohsRobe] = ItemID.PharaohsMask;

            convert[ItemID.AmberHook] = ItemID.SandstorminaBottle;
            convert[ItemID.SandstorminaBottle] = ItemID.FlyingCarpet;
            convert[ItemID.FlyingCarpet] = ItemID.AmberHook;
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

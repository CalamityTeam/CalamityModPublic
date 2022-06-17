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
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod
{
    // TODO -- Change this to a ModSystem so it does not need to be manually loaded from CalamityMod
    internal class CalamityRecipes
    {
        private static Recipe CreateRecipe(int itemID, int stack = 1) => CalamityMod.Instance.CreateRecipe(itemID, stack);

        #region Recipe Group Definitions
        private static void ModifyVanillaRecipeGroups()
        {
            // Twinklers count as Fireflies
            RecipeGroup firefly = RecipeGroup.recipeGroups[RecipeGroup.recipeGroupIDs["Fireflies"]];
            firefly.ValidItems.Add(ModContent.ItemType<TwinklerItem>());

            // Astral, Sunken Sea and Sulphurous Sea Sand is Sand
            RecipeGroup sand = RecipeGroup.recipeGroups[RecipeGroup.recipeGroupIDs["Sand"]];
            sand.ValidItems.Add(ModContent.ItemType<AstralSand>());
            sand.ValidItems.Add(ModContent.ItemType<EutrophicSand>());
            sand.ValidItems.Add(ModContent.ItemType<SulphurousSand>());

            // Acidwood is Wood
            RecipeGroup wood = RecipeGroup.recipeGroups[RecipeGroup.recipeGroupIDs["Wood"]];
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
            RecipeGroup group = new RecipeGroup(() => "Any Hardmode Anvil", new int[]
            {
                ItemID.MythrilAnvil,
                ItemID.OrichalcumAnvil
            });
            RecipeGroup.RegisterGroup("HardmodeAnvil", group);

            // Adamantite Forge and Titanium Forge
            group = new RecipeGroup(() => "Any Hardmode Forge", new int[]
            {
                ItemID.AdamantiteForge,
                ItemID.TitaniumForge
            });
            RecipeGroup.RegisterGroup("HardmodeForge", group);

            // Large Gems (PvP tokens)
            group = new RecipeGroup(() => "Any Large Gem", new int[]
            {
                ItemID.LargeAmber,
                ItemID.LargeAmethyst,
                ItemID.LargeDiamond,
                ItemID.LargeEmerald,
                ItemID.LargeRuby,
                ItemID.LargeSapphire,
                ItemID.LargeTopaz
            });
            RecipeGroup.RegisterGroup("AnyLargeGem", group);

            // 1.3 Food Items
            // TODO -- "Any Food" doesn't count the numerous 1.4 foods
            group = new RecipeGroup(() => "Any Food Item", new int[]
            {
                ItemID.CookedFish,
                ItemID.CookedMarshmallow,
                ItemID.PadThai,
                ItemID.Pho,
                ItemID.CookedShrimp,
                ItemID.Sashimi,
                ItemID.Bacon,
                ItemID.BowlofSoup,
                ItemID.GrubSoup,
                ItemID.GingerbreadCookie,
                ItemID.SugarCookie,
                ItemID.ChristmasPudding,
                ItemID.PumpkinPie,
                ModContent.ItemType<Baguette>(),
                ModContent.ItemType<DeliciousMeat>(),
                ModContent.ItemType<HadalStew>()
            });
            RecipeGroup.RegisterGroup("AnyFood", group);
        }

        private static void AddOreAndBarRecipeGroups()
        {
            // Copper and Tin
            RecipeGroup group = new RecipeGroup(() => "Any Copper Bar", new int[]
            {
                ItemID.CopperBar,
                ItemID.TinBar
            });
            RecipeGroup.RegisterGroup("AnyCopperBar", group);

            // Silver and Tungsten
            group = new RecipeGroup(() => "Any Silver Bar", new int[]
            {
                ItemID.SilverBar,
                ItemID.TungstenBar,
            });
            RecipeGroup.RegisterGroup("AnySilverBar", group);

            // Gold and Platinum Ore
            group = new RecipeGroup(() => "Any Gold Ore", new int[]
            {
                ItemID.GoldOre,
                ItemID.PlatinumOre
            });
            RecipeGroup.RegisterGroup("AnyGoldOre", group);

            // Gold and Platinum
            group = new RecipeGroup(() => "Any Gold Bar", new int[]
            {
                ItemID.GoldBar,
                ItemID.PlatinumBar
            });
            RecipeGroup.RegisterGroup("AnyGoldBar", group);

            // Demonite and Crimtane
            group = new RecipeGroup(() => "Any Evil Bar", new int[]
            {
                ItemID.DemoniteBar,
                ItemID.CrimtaneBar
            });
            RecipeGroup.RegisterGroup("AnyEvilBar", group);

            // Cobalt and Palladium
            group = new RecipeGroup(() => "Any Cobalt Bar", new int[]
            {
                ItemID.CobaltBar,
                ItemID.PalladiumBar
            });
            RecipeGroup.RegisterGroup("AnyCobaltBar", group);

            // Mythril and Orichalcum
            group = new RecipeGroup(() => "Any Mythril Bar", new int[]
            {
                ItemID.MythrilBar,
                ItemID.OrichalcumBar
            });
            RecipeGroup.RegisterGroup("AnyMythrilBar", group);

            // Adamantite and Titanium
            group = new RecipeGroup(() => "Any Adamantite Bar", new int[]
            {
                ItemID.AdamantiteBar,
                ItemID.TitaniumBar
            });
            RecipeGroup.RegisterGroup("AnyAdamantiteBar", group);
        }

        private static void AddEvilBiomeItemRecipeGroups()
        {
            // Vile and Vicious Powder
            RecipeGroup group = new RecipeGroup(() => "Any Evil Powder", new int[]
            {
                ItemID.VilePowder,
                ItemID.ViciousPowder
            });
            RecipeGroup.RegisterGroup("EvilPowder", group);

            // Shadow Scale and Tissue Sample
            group = new RecipeGroup(() => "Shadow Scale or Tissue Sample", new int[]
            {
                ItemID.ShadowScale,
                ItemID.TissueSample
            });
            RecipeGroup.RegisterGroup("Boss2Material", group);

            // Cursed Flame and Ichor
            group = new RecipeGroup(() => "Cursed Flame or Ichor", new int[]
            {
                ItemID.CursedFlame,
                ItemID.Ichor
            });
            RecipeGroup.RegisterGroup("CursedFlameIchor", group);

            // Unholy Water and Blood Water
            group = new RecipeGroup(() => "Any Evil Water", new int[]
            {
                ItemID.UnholyWater,
                ItemID.BloodWater
            });
            RecipeGroup.RegisterGroup("AnyEvilWater", group);

            // Flask of Cursed Flames and Flask of Ichor
            group = new RecipeGroup(() => "Any Evil Flask", new int[]
            {
                ItemID.FlaskofCursedFlames,
                ItemID.FlaskofIchor
            });
            RecipeGroup.RegisterGroup("AnyEvilFlask", group);
        }

        private static void AddBiomeBlockRecipeGroups()
        {
            // Vanilla Stone and Astral Stone
            RecipeGroup group = new RecipeGroup(() => "Any Stone Block", new int[]
            {
                ItemID.StoneBlock,
                ItemID.EbonstoneBlock,
                ItemID.CrimstoneBlock,
                ItemID.PearlstoneBlock,
                ModContent.ItemType<AstralStone>()
            });
            RecipeGroup.RegisterGroup("AnyStoneBlock", group);

            // Vanilla Snow and Astral Snow
            group = new RecipeGroup(() => "Any Snow Block", new int[]
            {
                ItemID.SnowBlock,
                ModContent.ItemType<AstralSnow>()
            });
            RecipeGroup.RegisterGroup("AnySnowBlock", group);

            // Vanilla Ice and Astral Ice
            group = new RecipeGroup(() => "Any Ice Block", new int[]
            {
                ItemID.IceBlock,
                ItemID.PurpleIceBlock,
                ItemID.RedIceBlock,
                ItemID.PinkIceBlock,
                ModContent.ItemType<AstralIce>()
            });
            RecipeGroup.RegisterGroup("AnyIceBlock", group);

            // Silt, Slush, and Astral Silt
            group = new RecipeGroup(() => "Any Silt", new int[]
            {
                ItemID.SiltBlock,
                ItemID.SlushBlock,
                ModContent.ItemType<NovaeSlag>()
            });
            RecipeGroup.RegisterGroup("SiltGroup", group);

            // Set of all generic Corruption/Crimson blocks
            group = new RecipeGroup(() => "Any Evil Block", new int[]
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
            RecipeGroup.RegisterGroup("AnyEvilBlock", group);

            // Set of all generic Hallow blocks
            group = new RecipeGroup(() => "Any Good Block", new int[]
            {
                ItemID.PearlstoneBlock,
                ItemID.PinkIceBlock,
                ItemID.PearlsandBlock,
                ItemID.HallowHardenedSand,
                ItemID.HallowSandstone
            });
        }
        
        private static void AddEquipmentRecipeGroups()
        {
            // Wooden Swords
            RecipeGroup group = new RecipeGroup(() => "Any Wooden Sword", new int[]
            {
                ItemID.WoodenSword,
                ItemID.BorealWoodSword,
                ItemID.RichMahoganySword,
                ItemID.PalmWoodSword,
                ItemID.EbonwoodSword,
                ItemID.ShadewoodSword,
                ItemID.PearlwoodSword
            });
            RecipeGroup.RegisterGroup("AnyWoodenSword", group);

            // Zapinators
            group = new RecipeGroup(() => "Any Zapinator", new int[]
            {
                ItemID.ZapinatorGray,
                ItemID.ZapinatorOrange
            });
            RecipeGroup.RegisterGroup("AnyZapinator", group);

            // Hallowed Helmets
            group = new RecipeGroup(() => "Any Hallowed Helmet", new int[]
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
            RecipeGroup.RegisterGroup("AnyHallowedHelmet", group);

            // Hallowed Plate Mails
            group = new RecipeGroup(() => "Any Hallowed Platemail", new int[]
            {
                ItemID.HallowedPlateMail,
                ItemID.AncientHallowedPlateMail
            });
            RecipeGroup.RegisterGroup("AnyHallowedPlatemail", group);

            // Hallowed Greaves
            group = new RecipeGroup(() => "Any Hallowed Greaves", new int[]
            {
                ItemID.HallowedGreaves,
                ItemID.AncientHallowedGreaves
            });
            RecipeGroup.RegisterGroup("AnyHallowedGreaves", group);

            // Vanilla Luminite Pickaxes and Genesis Pickaxe
            group = new RecipeGroup(() => "Any Lunar Pickaxe", new int[]
            {
                ItemID.SolarFlarePickaxe,
                ItemID.VortexPickaxe,
                ItemID.NebulaPickaxe,
                ItemID.StardustPickaxe,
                ModContent.ItemType<GenesisPickaxe>()
            });
            RecipeGroup.RegisterGroup("LunarPickaxe", group);

            // Luminite Hamaxes
            group = new RecipeGroup(() => "Any Lunar Hamaxe", new int[]
            {
                ItemID.LunarHamaxeSolar,
                ItemID.LunarHamaxeVortex,
                ItemID.LunarHamaxeNebula,
                ItemID.LunarHamaxeStardust
            });
            RecipeGroup.RegisterGroup("LunarHamaxe", group);

            // Mana Flower+ for Ethereal Talisman
            group = new RecipeGroup(() => "Any Mana Flowers", new int[]
            {
                ItemID.ManaFlower,
                ItemID.ArcaneFlower,
                ItemID.MagnetFlower,
                ItemID.ManaCloak
            }); 
            RecipeGroup.RegisterGroup("ManaFlowersGroup", group);

            // Magic Quiver+ for Elemental Quiver
            group = new RecipeGroup(() => "Any Quivers", new int[]
            {
                ItemID.MagicQuiver,
                ItemID.MoltenQuiver,
                ItemID.StalkersQuiver
            });
            RecipeGroup.RegisterGroup("QuiversGroup", group);

        // Wings
        group = new RecipeGroup(() => "Any Wings", new int[]
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
            RecipeGroup.RegisterGroup("WingsGroup", group);
        }
        #endregion

        public static void AddRecipes()
        {
            EditVanillaRecipes();
            
            // Leather from Vertebrae
            CreateRecipe(ItemID.Leather).
                AddIngredient(ItemID.Vertebrae, 2).
                AddTile(TileID.WorkBenches).
                Register();

            // Fallen Stars from Stardust
            CreateRecipe(ItemID.FallenStar).
                AddIngredient<Stardust>(5).
                AddTile(TileID.Anvils).
                Register();

            // Black Lens
            CreateRecipe(ItemID.BlackLens).
                AddIngredient(ItemID.Lens).
                AddIngredient(ItemID.BlackDye).
                AddTile(TileID.DyeVat).
                Register();

            // Earlier Rocket Is for early rocket weapons
            CreateRecipe(ItemID.RocketI, 20).
                AddIngredient(ItemID.EmptyBullet, 20).
                AddIngredient(ItemID.ExplosivePowder, 1).
                AddTile(TileID.MythrilAnvil).
                Register();

            // Life Crystal
            CreateRecipe(ItemID.LifeCrystal).
                AddIngredient(ItemID.StoneBlock, 5).
                AddIngredient(ItemID.Ruby, 2).
                AddIngredient(ItemID.HealingPotion).
                AddTile(TileID.Anvils).
                Register();

            // Life Fruit
            CreateRecipe(ItemID.LifeFruit).
                AddIngredient<PlantyMush>(10).
                AddIngredient<LivingShard>().
                AddTile(TileID.MythrilAnvil).
                Register();

            // Ultrabright Torch
            CreateRecipe(ItemID.UltrabrightTorch, 33).
                AddIngredient(ItemID.Torch, 33).
                AddIngredient<SeaPrism>().
                AddTile(TileID.Anvils).
                Register();

            // Money Trough
            CreateRecipe(ItemID.MoneyTrough).
                AddIngredient(ItemID.PiggyBank).
                AddIngredient(ItemID.Feather, 2).
                AddIngredient<BloodOrb>().
                AddIngredient(ItemID.GoldCoin, 15).
                AddRecipeGroup("AnyGoldBar", 8).
                AddTile(TileID.Anvils).
                Register();

            // Demon Conch
            CreateRecipe(ItemID.DemonConch).
                AddIngredient<DemonicBoneAsh>().
                AddIngredient(ItemID.HellstoneBar, 4).
                AddTile(TileID.Hellforge).
                Register();

            // Lava Fishing Hook
            CreateRecipe(ItemID.LavaFishingHook).
                AddIngredient(ItemID.Seashell).
                AddIngredient(ItemID.HellstoneBar, 10).
                AddTile(TileID.Hellforge).
                Register();

            AddAstralClayRecipes();
            AddBloodOrbPotionRecipes();
            AddCookedFood();
            AddEssentialToolRecipes();
            AddSummonAndProgressionRecipes();
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
            EditLeatherRecipe();
            EditEnchantedBoomerangRecipe();
            EditPhoenixBlasterRecipe();
            EditFlamarangRecipe();
            EditBundleofBalloonsRecipe();
            EditTrueNightsEdgeRecipe();
            EditTrueExcaliburRecipe();
            EditTerraBladeRecipe();
            EditZenithRecipe();
            EditMagiluminescenceRecipe();
            EditFireGauntletRecipe();
            EditSpiritFlameRecipe();
            EditBeetleArmorRecipes();
            EditGoblinArmySummonRecipe();
            EditEvilBossSummonRecipes();
            MoveHardmodeItemsToAnvils();
            EditPumpkinMoonSummonRecipe();
            EditFrostMoonSummonRecipe();
            EditPhasesaberRecipes();
            EditOpticStaffRecipe();
            EditShroomiteBarRecipe();
            EditChlorophyteBarRecipe();
            EditAnkhCharmRecipe();
            EditCelestialEmblemRecipe();
            EditMechanicalGloveRecipe();

            EditPreHardmodeOreArmorRecipes();
            EditHardmodeOreSetRecipes();
        }

        // Change Leather's recipe to require 2 Rotten Chunks
        private static void EditLeatherRecipe()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.Leather).ToList().ForEach(s =>
            {
                s.requiredItem[0].stack = 2;
            });
        }

        // Changes the stupid vanilla Enchanted Boomerang recipe to be more in line with the Echanted Sword recipe. - Merkalto
        public static void EditEnchantedBoomerangRecipe()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.EnchantedBoomerang).ToList().ForEach(s =>
            {
                s.requiredItem = new List<Item>();
                for (int i = 0; i < 3; i++)
                {
                    s.requiredItem.Add(new Item());
                }
                s.requiredItem[0].SetDefaults(ModContent.ItemType<VictoryShard>(), false);
                s.requiredItem[0].stack = 6;
                s.requiredItem[1].SetDefaults(ItemID.GoldBar, false);
                s.requiredItem[1].stack = 8;
                s.requiredItem[2].SetDefaults(ItemID.WoodenBoomerang, false);
                s.requiredItem[2].stack = 1;

                s.AddTile(TileID.Anvils); // This is here because by default the recipe uses no tiles.
                s.createItem.SetDefaults(ItemID.EnchantedBoomerang, false);
                s.createItem.stack = 1;

                Recipe r = CreateRecipe(ItemID.EnchantedBoomerang);  // Vanilla items don't like custom item groups.
                r.AddIngredient(ModContent.ItemType<VictoryShard>(), 6);
                r.AddIngredient(ItemID.PlatinumBar, 8);
                r.AddIngredient(ItemID.WoodenBoomerang);
                r.AddTile(TileID.Anvils);
                r.Register();
            });
        }

        // Change Phoenix Blaster's recipe to be consistent with literally every other weapon recipes in the game
        private static void EditPhoenixBlasterRecipe()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.PhoenixBlaster).ToList().ForEach(s =>
            {
                for (int i = 0; i < s.requiredItem.Count; i++)
                {
                    s.requiredItem[i] = new Item();
                }
                s.requiredItem[0].SetDefaults(ItemID.Handgun, false);
                s.requiredItem[0].stack = 1;
                s.requiredItem[1].SetDefaults(ItemID.HellstoneBar, false);
                s.requiredItem[1].stack = 10;

                s.createItem.SetDefaults(ItemID.PhoenixBlaster, false);
                s.createItem.stack = 1;
            });
        }

        // Change Flamarang's recipe to be consistent with literally every other weapon recipes in the game
        private static void EditFlamarangRecipe()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.Flamarang).ToList().ForEach(s =>
            {
                for (int i = 0; i < s.requiredItem.Count; i++)
                {
                    s.requiredItem[i] = new Item();
                }
                s.requiredItem[0].SetDefaults(ItemID.EnchantedBoomerang, false);
                s.requiredItem[0].stack = 1;
                s.requiredItem[1].SetDefaults(ItemID.HellstoneBar, false);
                s.requiredItem[1].stack = 10;

                s.createItem.SetDefaults(ItemID.Flamarang, false);
                s.createItem.stack = 1;
            });
        }

        // Change Bundle of Balloons's recipe to require 3 Aerialite Bars (forces the acc to be post-Evils 2)
        private static void EditBundleofBalloonsRecipe()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.BundleofBalloons).ToList().ForEach(s =>
            {
                s.requiredItem = new List<Item>();
                for (int i = 0; i < 4; i++)
                    s.requiredItem.Add(new Item());
                s.requiredItem[0].SetDefaults(ItemID.CloudinaBalloon, false);
                s.requiredItem[0].stack = 1;
                s.requiredItem[1].SetDefaults(ItemID.BlizzardinaBalloon, false);
                s.requiredItem[1].stack = 1;
                s.requiredItem[2].SetDefaults(ItemID.SandstorminaBalloon, false);
                s.requiredItem[2].stack = 1;
                s.requiredItem[3].SetDefaults(ModContent.ItemType<AerialiteBar>(), false);
                s.requiredItem[3].stack = 3;

                s.createItem.SetDefaults(ItemID.BundleofBalloons, false);
                s.createItem.stack = 1;
            });
        }

        // Change True Night's Edge recipe to require 5 of each mech boss soul instead of 20
        private static void EditTrueNightsEdgeRecipe()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.TrueNightsEdge).ToList().ForEach(s =>
            {
                s.requiredItem[1].stack = 3;
                s.requiredItem[2].stack = 3;
                s.requiredItem[3].stack = 3;
            });
        }

        // Change True Excalibur's recipe to require 12 Chlorophyte Bars instead of 24
        private static void EditTrueExcaliburRecipe()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.TrueExcalibur).ToList().ForEach(s =>
            {
                s.requiredItem[1].stack = 12;
            });
        }

        // Change Terra Blade's recipe to require 7 Living Shards (forces the Blade to be post-Plantera)
        private static void EditTerraBladeRecipe()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.TerraBlade).ToList().ForEach(s =>
            {
                for (int i = 0; i < s.requiredItem.Count; i++)
                {
                    s.requiredItem[i] = new Item();
                }
                s.requiredItem[0].SetDefaults(ItemID.TrueNightsEdge, false);
                s.requiredItem[0].stack = 1;
                s.requiredItem[1].SetDefaults(ItemID.TrueExcalibur, false);
                s.requiredItem[1].stack = 1;
                s.requiredItem[2].SetDefaults(ModContent.ItemType<LivingShard>(), false);
                s.requiredItem[2].stack = 7;

                s.createItem.SetDefaults(ItemID.TerraBlade, false);
                s.createItem.stack = 1;
            });
        }

        // Change Zenith's recipe to require 5 Auric Bars (forces Zenith to be post-Yharon)
        private static void EditZenithRecipe()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.Zenith).ToList().ForEach(s =>
            {
                s.requiredItem = new List<Item>();
                for (int i = 0; i < 11; i++)
                    s.requiredItem.Add(new Item());
                s.requiredItem[0].SetDefaults(ItemID.CopperShortsword, false);
                s.requiredItem[0].stack = 1;
                s.requiredItem[1].SetDefaults(ItemID.EnchantedSword, false);
                s.requiredItem[1].stack = 1;
                s.requiredItem[2].SetDefaults(ItemID.BeeKeeper, false);
                s.requiredItem[2].stack = 1;
                s.requiredItem[3].SetDefaults(ItemID.Starfury, false);
                s.requiredItem[3].stack = 1;
                s.requiredItem[4].SetDefaults(ItemID.Seedler, false);
                s.requiredItem[4].stack = 1;
                s.requiredItem[5].SetDefaults(ItemID.TheHorsemansBlade, false);
                s.requiredItem[5].stack = 1;
                s.requiredItem[6].SetDefaults(ItemID.InfluxWaver, false);
                s.requiredItem[6].stack = 1;
                s.requiredItem[7].SetDefaults(ItemID.StarWrath, false);
                s.requiredItem[7].stack = 1;
                s.requiredItem[8].SetDefaults(ItemID.Meowmere, false);
                s.requiredItem[8].stack = 1;
                s.requiredItem[9].SetDefaults(ItemID.TerraBlade, false);
                s.requiredItem[9].stack = 1;
                s.requiredItem[10].SetDefaults(ModContent.ItemType<AuricBar>(), false);
                s.requiredItem[10].stack = 5;

                s.requiredTile[0] = ModContent.TileType<CosmicAnvil>();
                s.createItem.SetDefaults(ItemID.Zenith, false);
                s.createItem.stack = 1;
            });
        }

        // Change Magiluminescence's recipe to require more ingredients
        private static void EditMagiluminescenceRecipe()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.Magiluminescence).ToList().ForEach(s =>
            {
                s.requiredItem[0].stack = 20;
                s.requiredItem[1].stack = 15;
            });
        }

        // Change Fire Gauntlet's recipe to require 5 Chaotic Bars (forces the item to be post-Golem)
        private static void EditFireGauntletRecipe()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.FireGauntlet).ToList().ForEach(s =>
            {
                s.requiredItem = new List<Item>();
                for (int i = 0; i < 3; i++)
                {
                    s.requiredItem.Add(new Item());
                }
                s.requiredItem[0].SetDefaults(ItemID.MagmaStone, false);
                s.requiredItem[0].stack = 1;
                s.requiredItem[1].SetDefaults(ItemID.MechanicalGlove, false);
                s.requiredItem[1].stack = 1;
                s.requiredItem[2].SetDefaults(ModContent.ItemType<ScoriaBar>(), false);
                s.requiredItem[2].stack = 5;

                s.createItem.SetDefaults(ItemID.FireGauntlet, false);
                s.createItem.stack = 1;
            });
        }

        private static void EditSpiritFlameRecipe() // This is here to keep the Forbidden Fragment stuff on the same tier.
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.SpiritFlame).ToList().ForEach(s =>
            {
                s.requiredItem = new List<Item>();
                for (int i = 0; i < 4; i++)
                {
                    s.requiredItem.Add(new Item());
                }
                s.requiredItem[0].SetDefaults(ItemID.DjinnLamp, false);
                s.requiredItem[0].stack = 1;
                s.requiredItem[1].SetDefaults(ItemID.AncientBattleArmorMaterial, false);
                s.requiredItem[1].stack = 2;
                s.requiredItem[2].SetDefaults(ItemID.SoulofNight, false);
                s.requiredItem[2].stack = 12;
                s.requiredItem[3].SetDefaults(ItemID.AdamantiteBar, false);
                s.requiredItem[3].stack = 2;
                s.createItem.SetDefaults(ItemID.SpiritFlame, false);
                s.createItem.stack = 1;

                Recipe r = CreateRecipe(ItemID.SpiritFlame);  // Vanilla items don't like custom item groups.
                r.AddIngredient(ItemID.DjinnLamp);
                r.AddIngredient(ItemID.AncientBattleArmorMaterial, 2);
                r.AddIngredient(ItemID.SoulofNight, 12);
                r.AddIngredient(ItemID.TitaniumBar, 2);
                r.AddTile(TileID.MythrilAnvil);
                r.Register();
            });
        }

        // Change Beetle Armor recipes to have Turtle Armor at the top of them (my dreaded)
        private static void EditBeetleArmorRecipes()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.BeetleHelmet).ToList().ForEach(s =>
            {
                for (int i = 0; i < s.requiredItem.Count; i++)
                {
                    s.requiredItem[i] = new Item();
                }
                s.requiredItem[0].SetDefaults(ItemID.TurtleHelmet, false);
                s.requiredItem[0].stack = 1;
                s.requiredItem[1].SetDefaults(ItemID.BeetleHusk, false);
                s.requiredItem[1].stack = 4;

                s.createItem.SetDefaults(ItemID.BeetleHelmet, false);
                s.createItem.stack = 1;
            });

            rec.Where(x => x.createItem.type == ItemID.BeetleScaleMail).ToList().ForEach(s =>
            {
                for (int i = 0; i < s.requiredItem.Count; i++)
                {
                    s.requiredItem[i] = new Item();
                }
                s.requiredItem[0].SetDefaults(ItemID.TurtleScaleMail, false);
                s.requiredItem[0].stack = 1;
                s.requiredItem[1].SetDefaults(ItemID.BeetleHusk, false);
                s.requiredItem[1].stack = 8;

                s.createItem.SetDefaults(ItemID.BeetleScaleMail, false);
                s.createItem.stack = 1;
            });

            rec.Where(x => x.createItem.type == ItemID.BeetleShell).ToList().ForEach(s =>
            {
                for (int i = 0; i < s.requiredItem.Count; i++)
                {
                    s.requiredItem[i] = new Item();
                }
                s.requiredItem[0].SetDefaults(ItemID.TurtleScaleMail, false);
                s.requiredItem[0].stack = 1;
                s.requiredItem[1].SetDefaults(ItemID.BeetleHusk, false);
                s.requiredItem[1].stack = 8;

                s.createItem.SetDefaults(ItemID.BeetleShell, false);
                s.createItem.stack = 1;
            });

            rec.Where(x => x.createItem.type == ItemID.BeetleLeggings).ToList().ForEach(s =>
            {
                for (int i = 0; i < s.requiredItem.Count; i++)
                {
                    s.requiredItem[i] = new Item();
                }
                s.requiredItem[0].SetDefaults(ItemID.TurtleLeggings, false);
                s.requiredItem[0].stack = 1;
                s.requiredItem[1].SetDefaults(ItemID.BeetleHusk, false);
                s.requiredItem[1].stack = 6;

                s.createItem.SetDefaults(ItemID.BeetleLeggings, false);
                s.createItem.stack = 1;
            });
        }

        private static void EditGoblinArmySummonRecipe()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.GoblinBattleStandard).ToList().ForEach(s =>
            {
                s.requiredItem[0].stack = 5;
            });
        }

        private static void EditEvilBossSummonRecipes()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.BloodySpine || x.createItem.type == ItemID.WormFood).ToList().ForEach(s =>
            {
                s.requiredItem[0].stack = 20;
                s.requiredItem[1].stack = 10;
            });
        }

        /* This houses all Hardmode items that are moved to Anvils in Calamity:
        FIRST ROW: Boss Summons
        SECOND ROW: Weapons
        THIRD ROW: Accessories
        FOURTH ROW: Ammo */
        public static void MoveHardmodeItemsToAnvils()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.MechanicalWorm || x.createItem.type == ItemID.MechanicalEye || x.createItem.type == ItemID.MechanicalSkull ||
            x.createItem.type == ItemID.DaoofPow || x.createItem.type == ItemID.Chik || x.createItem.type == ItemID.MeteorStaff || x.createItem.type == ItemID.CoolWhip ||
            x.createItem.type == ItemID.AngelWings || x.createItem.type == ItemID.DemonWings || x.createItem.type == ItemID.FairyWings ||
            x.createItem.type == ItemID.IchorBullet || x.createItem.type == ItemID.IchorArrow || x.createItem.type == ItemID.CursedBullet || x.createItem.type == ItemID.CursedArrow
            ).ToList().ForEach(s =>
            {
                s.requiredTile[0] = TileID.Anvils;
            });
        }

        public static void EditPumpkinMoonSummonRecipe()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.PumpkinMoonMedallion).ToList().ForEach(s =>
            {
                s.requiredItem[0].SetDefaults(ItemID.Pumpkin, false);
                s.requiredItem[0].stack = 30;
                s.requiredItem[1].SetDefaults(ItemID.Ectoplasm, false);
                s.requiredItem[1].stack = 15;
                s.requiredItem[2].SetDefaults(ItemID.GoldBar, false);
                s.requiredItem[2].stack = 10;
            });

            Recipe r = CreateRecipe(ItemID.PumpkinMoonMedallion);  // Vanilla items don't like custom item groups so I have to do this instead.
            r.AddIngredient(ItemID.Pumpkin, 30);
            r.AddIngredient(ItemID.Ectoplasm, 15);
            r.AddIngredient(ItemID.PlatinumBar, 10);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();
        }

        public static void EditFrostMoonSummonRecipe()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.NaughtyPresent).ToList().ForEach(s =>
            {
                s.requiredItem[0].SetDefaults(ItemID.Silk, false);
                s.requiredItem[0].stack = 20;
                s.requiredItem[1].SetDefaults(ItemID.Ectoplasm, false);
                s.requiredItem[1].stack = 15;
                s.requiredItem[2].SetDefaults(ItemID.GoldBar, false);
                s.requiredItem[2].stack = 10;
            });

            Recipe r = CreateRecipe(ItemID.NaughtyPresent);
            r.AddIngredient(ItemID.Silk, 20);
            r.AddIngredient(ItemID.Ectoplasm, 15);
            r.AddIngredient(ItemID.PlatinumBar, 10);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();
        }


        // Change Phasesaber recipes to require 20 Crystal Shards
        private static void EditPhasesaberRecipes()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.BluePhasesaber || x.createItem.type == ItemID.GreenPhasesaber || x.createItem.type == ItemID.PurplePhasesaber ||
            x.createItem.type == ItemID.RedPhasesaber || x.createItem.type == ItemID.WhitePhasesaber || x.createItem.type == ItemID.YellowPhasesaber ||
            x.createItem.type == ItemID.OrangePhasesaber).ToList().ForEach(s =>
            {
                s.requiredItem[1].stack = 20;
            });
        }

        // Remove Hallowed Bars from Optic Staff
        private static void EditOpticStaffRecipe()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.OpticStaff).ToList().ForEach(s =>
            {
                for (int i = 0; i < s.requiredItem.Count; i++)
                {
                    s.requiredItem[i] = new Item();
                }
                s.requiredItem[0].SetDefaults(ItemID.BlackLens, false);
                s.requiredItem[0].stack = 1;
                s.requiredItem[1].SetDefaults(ItemID.Lens, false);
                s.requiredItem[1].stack = 2;
                s.requiredItem[2].SetDefaults(ItemID.SoulofSight, false);
                s.requiredItem[2].stack = 20;

                s.createItem.SetDefaults(ItemID.OpticStaff, false);
                s.createItem.stack = 1;
            });
        }

        // Change Shroomite Bar's recipe to require 5 Glowing Mushrooms instead
        private static void EditShroomiteBarRecipe()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.ShroomiteBar).ToList().ForEach(s =>
            {
                s.requiredItem[1].stack = 5;
            });
        }

        // Change the recipes to be like 1.4 but even less demanding
        private static void EditChlorophyteBarRecipe()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.ChlorophyteBar).ToList().ForEach(s =>
            {
                s.requiredItem[0].stack = 4;
            });
        }

        // Add Pocket Mirror to the Ankh Charm recipe.
        private static void EditAnkhCharmRecipe()
        {
            {
                List<Recipe> rec = Main.recipe.ToList();
                rec.Where(x => x.createItem.type == ItemID.AnkhCharm).ToList().ForEach(s =>
                {
                    s.requiredItem = new List<Item>();
                    for (int i = 0; i < 6; i++)
                        s.requiredItem.Add(new Item());
                    s.requiredItem[0].SetDefaults(ItemID.ArmorBracing, false);
                    s.requiredItem[0].stack = 1;
                    s.requiredItem[1].SetDefaults(ItemID.MedicatedBandage, false);
                    s.requiredItem[1].stack = 1;
                    s.requiredItem[2].SetDefaults(ItemID.ThePlan, false);
                    s.requiredItem[2].stack = 1;
                    s.requiredItem[3].SetDefaults(ItemID.CountercurseMantra, false);
                    s.requiredItem[3].stack = 1;
                    s.requiredItem[4].SetDefaults(ItemID.Blindfold, false);
                    s.requiredItem[4].stack = 1;
                    s.requiredItem[5].SetDefaults(ItemID.PocketMirror, false);
                    s.requiredItem[5].stack = 1;

                    s.requiredTile[0] = TileID.TinkerersWorkbench;
                    s.createItem.SetDefaults(ItemID.AnkhCharm, false);
                    s.createItem.stack = 1;
                });
            }
        }

        private static void EditCelestialEmblemRecipe()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.CelestialEmblem).ToList().ForEach(s =>
            {
                for (int i = 0; i < s.requiredItem.Count; i++)
                {
                    s.requiredItem[i] = new Item();
                }
                s.requiredItem[0].SetDefaults(ItemID.CelestialMagnet, false);
                s.requiredItem[0].stack = 1;
                s.requiredItem[1].SetDefaults(ItemID.SorcererEmblem, false);
                s.requiredItem[1].stack = 1;

                s.createItem.SetDefaults(ItemID.CelestialEmblem, false);
                s.createItem.stack = 1;
            });
        }

        private static void EditMechanicalGloveRecipe()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.MechanicalGlove).ToList().ForEach(s =>
            {
                for (int i = 0; i < s.requiredItem.Count; i++)
                {
                    s.requiredItem[i] = new Item();
                }
                s.requiredItem[0].SetDefaults(ItemID.PowerGlove, false);
                s.requiredItem[0].stack = 1;
                s.requiredItem[1].SetDefaults(ItemID.WarriorEmblem, false);
                s.requiredItem[1].stack = 1;

                s.createItem.SetDefaults(ItemID.MechanicalGlove, false);
                s.createItem.stack = 1;
            });
        }

        // Changes pre-hardmode ore armor recipes to be consistent for each tier and require far fewer bars.
        private static void EditPreHardmodeOreArmorRecipes()
        {
            List<Recipe> rec = Main.recipe.ToList();

            bool isPHMOreHelmet(Recipe r)
            {
                int itemID = r.createItem.type;
                if (itemID == ItemID.CopperHelmet || itemID == ItemID.TinHelmet)
                    return true;
                if (itemID == ItemID.IronHelmet || itemID == ItemID.AncientIronHelmet || itemID == ItemID.LeadHelmet)
                    return true;
                if (itemID == ItemID.SilverHelmet || itemID == ItemID.TungstenHelmet)
                    return true;
                return itemID == ItemID.GoldHelmet || itemID == ItemID.AncientGoldHelmet || itemID == ItemID.PlatinumHelmet;
            }
            rec.Where(x => isPHMOreHelmet(x)).ToList().ForEach(s => s.requiredItem[0].stack = 10);

            bool isPHMOreChainmail(Recipe r)
            {
                int itemID = r.createItem.type;
                if (itemID == ItemID.CopperChainmail || itemID == ItemID.TinChainmail)
                    return true;
                if (itemID == ItemID.IronChainmail || itemID == ItemID.LeadChainmail)
                    return true;
                if (itemID == ItemID.SilverChainmail || itemID == ItemID.TungstenChainmail)
                    return true;
                return itemID == ItemID.GoldChainmail || itemID == ItemID.PlatinumChainmail;
            }
            rec.Where(x => isPHMOreChainmail(x)).ToList().ForEach(s => s.requiredItem[0].stack = 16);

            bool isPHMOreGreaves(Recipe r)
            {
                int itemID = r.createItem.type;
                if (itemID == ItemID.CopperGreaves || itemID == ItemID.TinGreaves)
                    return true;
                if (itemID == ItemID.IronGreaves || itemID == ItemID.LeadGreaves)
                    return true;
                if (itemID == ItemID.SilverGreaves || itemID == ItemID.TungstenGreaves)
                    return true;
                return itemID == ItemID.GoldGreaves || itemID == ItemID.PlatinumGreaves;
            }
            rec.Where(x => isPHMOreGreaves(x)).ToList().ForEach(s => s.requiredItem[0].stack = 14);
        }

        // Changes hardmode ore recipes to be consistent for each tier, and makes pickaxes/drills cheaper.
        private static void EditHardmodeOreSetRecipes()
        {
            short MeleeHelm;
            short RangedHelm;
            short MagicHelm;
            short Breastplate;
            short Leggings;
            short Pickaxe;
            short Drill;
            short Waraxe;
            short Chainsaw;
            short Sword;
            short Glaive;
            short Repeater;

            // 0 = Cobalt, 1 = Palladium, 2 = Mythril, 3 = Orichalcum, 4 = Adamantite, 5 = Titanium
            for (int HardmodeOre = 0; HardmodeOre < 6; HardmodeOre++)
            {
                switch (HardmodeOre)
                {
                    case 1:
                        MeleeHelm = ItemID.CobaltHelmet;
                        RangedHelm = ItemID.CobaltMask;
                        MagicHelm = ItemID.CobaltHat;
                        Breastplate = ItemID.CobaltBreastplate;
                        Leggings = ItemID.CobaltLeggings;
                        Pickaxe = ItemID.CobaltPickaxe;
                        Drill = ItemID.CobaltDrill;
                        Waraxe = ItemID.CobaltWaraxe;
                        Chainsaw = ItemID.CobaltChainsaw;
                        Sword = ItemID.CobaltSword;
                        Glaive = ItemID.CobaltNaginata;
                        Repeater = ItemID.CobaltRepeater;
                        break;

                    case 2:
                        MeleeHelm = ItemID.PalladiumMask;
                        RangedHelm = ItemID.PalladiumHelmet;
                        MagicHelm = ItemID.PalladiumHeadgear;
                        Breastplate = ItemID.PalladiumBreastplate;
                        Leggings = ItemID.PalladiumLeggings;
                        Pickaxe = ItemID.PalladiumPickaxe;
                        Drill = ItemID.PalladiumDrill;
                        Waraxe = ItemID.PalladiumWaraxe;
                        Chainsaw = ItemID.PalladiumChainsaw;
                        Sword = ItemID.PalladiumSword;
                        Glaive = ItemID.PalladiumPike;
                        Repeater = ItemID.PalladiumRepeater;
                        break;

                    case 3:
                        MeleeHelm = ItemID.MythrilHelmet;
                        RangedHelm = ItemID.MythrilHat;
                        MagicHelm = ItemID.MythrilHood;
                        Breastplate = ItemID.MythrilChainmail;
                        Leggings = ItemID.MythrilGreaves;
                        Pickaxe = ItemID.MythrilPickaxe;
                        Drill = ItemID.MythrilDrill;
                        Waraxe = ItemID.MythrilWaraxe;
                        Chainsaw = ItemID.MythrilChainsaw;
                        Sword = ItemID.MythrilSword;
                        Glaive = ItemID.MythrilHalberd;
                        Repeater = ItemID.MythrilRepeater;
                        break;

                    case 4:
                        MeleeHelm = ItemID.OrichalcumMask;
                        RangedHelm = ItemID.OrichalcumHelmet;
                        MagicHelm = ItemID.OrichalcumHeadgear;
                        Breastplate = ItemID.OrichalcumBreastplate;
                        Leggings = ItemID.OrichalcumLeggings;
                        Pickaxe = ItemID.OrichalcumPickaxe;
                        Drill = ItemID.OrichalcumDrill;
                        Waraxe = ItemID.OrichalcumWaraxe;
                        Chainsaw = ItemID.OrichalcumChainsaw;
                        Sword = ItemID.OrichalcumSword;
                        Glaive = ItemID.OrichalcumHalberd;
                        Repeater = ItemID.OrichalcumRepeater;
                        break;

                    case 5:
                        MeleeHelm = ItemID.AdamantiteHelmet;
                        RangedHelm = ItemID.AdamantiteMask;
                        MagicHelm = ItemID.AdamantiteHeadgear;
                        Breastplate = ItemID.AdamantiteBreastplate;
                        Leggings = ItemID.AdamantiteLeggings;
                        Pickaxe = ItemID.AdamantitePickaxe;
                        Drill = ItemID.AdamantiteDrill;
                        Waraxe = ItemID.AdamantiteWaraxe;
                        Chainsaw = ItemID.AdamantiteChainsaw;
                        Sword = ItemID.AdamantiteSword;
                        Glaive = ItemID.AdamantiteGlaive;
                        Repeater = ItemID.AdamantiteRepeater;
                        break;

                    default:
                        MeleeHelm = ItemID.TitaniumMask;
                        RangedHelm = ItemID.TitaniumHelmet;
                        MagicHelm = ItemID.TitaniumHeadgear;
                        Breastplate = ItemID.TitaniumBreastplate;
                        Leggings = ItemID.TitaniumLeggings;
                        Pickaxe = ItemID.TitaniumPickaxe;
                        Drill = ItemID.TitaniumDrill;
                        Waraxe = ItemID.TitaniumWaraxe;
                        Chainsaw = ItemID.TitaniumChainsaw;
                        Sword = ItemID.TitaniumSword;
                        Glaive = ItemID.TitaniumTrident;
                        Repeater = ItemID.TitaniumRepeater;
                        break;
                }
                List<Recipe> rec = Main.recipe.ToList();

                bool isHelmet(Recipe r)
                {
                    int itemID = r.createItem.type;
                    return itemID == MeleeHelm || itemID == RangedHelm || itemID == MagicHelm;
                }
                bool isTool(Recipe r)
                {
                    int itemID = r.createItem.type;
                    return itemID == Pickaxe || itemID == Drill || itemID == Waraxe || itemID == Chainsaw;
                }
                bool isStandardWeapon(Recipe r)
                {
                    int itemID = r.createItem.type;
                    return itemID == Sword || itemID == Glaive || itemID == Repeater;
                }

                // 12 BARS : Melee Helm, Ranged Helm, Magic Helm, Pickaxe, Drill, Waraxe, Chainsaw, Sword, Glaive, Repeater
                rec.Where(x => isHelmet(x) || isTool(x) || isStandardWeapon(x)).ToList().ForEach(s => s.requiredItem[0].stack = 12);

                // 24 BARS : Breastplate
                rec.Where(x => x.createItem.type == Breastplate).ToList().ForEach(s => s.requiredItem[0].stack = 24);

                // 18 BARS : Leggings
                rec.Where(x => x.createItem.type == Leggings).ToList().ForEach(s => s.requiredItem[0].stack = 18);
            }
        }
        #endregion

        #region Astral Clay
        private static void AddAstralClayRecipes()
        {
            // Bowl
            Recipe r = CreateRecipe(ItemID.Bowl);
            r.AddIngredient(ModContent.ItemType<AstralClay>(), 2);
            r.AddTile(TileID.Furnaces);
            r.Register();

            // Clay Pot
            r = CreateRecipe(ItemID.ClayPot);
            r.AddIngredient(ModContent.ItemType<AstralClay>(), 2);
            r.AddTile(TileID.Furnaces);
            r.Register();

            // Pink Vase
            r = CreateRecipe(ItemID.PinkVase);
            r.AddIngredient(ModContent.ItemType<AstralClay>(), 2);
            r.AddTile(TileID.Furnaces);
            r.Register();
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
                ItemID.RecallPotion
            };
            Recipe r;

            foreach (var potion in potions)
            {
                r = CreateRecipe(potion);
                r.AddIngredient(ModContent.ItemType<BloodOrb>(), 10);
                r.AddIngredient(ItemID.BottledWater);
                r.AddTile(TileID.AlchemyTable);
                r.Register();
            }
        }
        #endregion

        #region Cooked Food
        private static void AddCookedFood()
        {
            Recipe r = CreateRecipe(ItemID.CookedFish);
            r.AddIngredient(ModContent.ItemType<TwinklingPollox>());
            r.AddTile(TileID.CookingPots);
            r.Register();

            r = CreateRecipe(ItemID.CookedFish);
            r.AddIngredient(ModContent.ItemType<PrismaticGuppy>());
            r.AddTile(TileID.CookingPots);
            r.Register();

            r = CreateRecipe(ItemID.CookedFish);
            r.AddIngredient(ModContent.ItemType<CragBullhead>());
            r.AddTile(TileID.CookingPots);
            r.Register();

            r = CreateRecipe(ItemID.CookedFish);
            r.AddIngredient(ModContent.ItemType<SeaMinnowItem>());
            r.AddTile(TileID.CookingPots);
            r.Register();

            r = CreateRecipe(ItemID.CookedShrimp);
            r.AddIngredient(ModContent.ItemType<ProcyonidPrawn>());
            r.AddTile(TileID.CookingPots);
            r.Register();

            r = CreateRecipe(ItemID.Bacon);
            r.AddIngredient(ModContent.ItemType<PiggyItem>());
            r.AddTile(TileID.CookingPots);
            r.Register();
        }
        #endregion

        #region Essential Gameplay Tools
        private static void AddEssentialToolRecipes()
        {
            // Umbrella
            Recipe r = CreateRecipe(ItemID.Umbrella);
            r.AddIngredient(ItemID.Silk, 5);
            r.AddRecipeGroup("AnyCopperBar", 2);
            r.AddTile(TileID.Loom);
            r.Register();

            // Bug Net
            r = CreateRecipe(ItemID.BugNet);
            r.AddIngredient(ItemID.Cobweb, 30);
            r.AddRecipeGroup("AnyCopperBar", 3);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Magic Mirror
            r = CreateRecipe(ItemID.MagicMirror);
            r.AddRecipeGroup("AnySilverBar", 10);
            r.AddIngredient(ItemID.Glass, 10);
            r.AddIngredient(ItemID.FallenStar, 10);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Ice Mirror
            r = CreateRecipe(ItemID.IceMirror);
            r.AddRecipeGroup("AnyIceBlock", 20);
            r.AddIngredient(ItemID.Glass, 10);
            r.AddIngredient(ItemID.FallenStar, 10);
            r.AddRecipeGroup("AnySilverBar", 5);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Bloody Tear
            r = CreateRecipe(ItemID.BloodMoonStarter);
            r.AddIngredient(ModContent.ItemType<BloodOrb>(), 10);
            r.AddRecipeGroup("AnyCopperBar", 3);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Staff of Regrowth
            r = CreateRecipe(ItemID.StaffofRegrowth);
            r.AddIngredient(ItemID.RichMahogany, 10);
            r.AddIngredient(ItemID.JungleSpores, 5);
            r.AddIngredient(ItemID.JungleRose);
            r.AddTile(TileID.WorkBenches);
            r.Register();

            // Shadow Key
            r = CreateRecipe(ItemID.ShadowKey);
            r.AddIngredient(ItemID.GoldenKey);
            r.AddIngredient(ItemID.Obsidian, 20);
            r.AddIngredient(ItemID.Bone, 5);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Sky Mill
            r = CreateRecipe(ItemID.SkyMill);
            r.AddIngredient(ItemID.SunplateBlock, 10);
            r.AddIngredient(ItemID.Cloud, 5);
            r.AddIngredient(ItemID.RainCloud, 3);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Ice Machine
            r = CreateRecipe(ItemID.IceMachine);
            r.AddRecipeGroup("AnyIceBlock", 25);
            r.AddRecipeGroup("AnySnowBlock", 15);
            r.AddRecipeGroup("IronBar", 3);
            r.AddTile(TileID.Anvils);
            r.Register();
        }
        #endregion

        #region Boss Summon and Progression Items
        private static void AddSummonAndProgressionRecipes()
        {
            // Guide Voodoo Doll
            Recipe r = CreateRecipe(ItemID.GuideVoodooDoll);
            r.AddIngredient(ItemID.Leather, 2);
            r.AddRecipeGroup("EvilPowder", 10);
            r.AddTile(TileID.Hellforge);
            r.Register();

            // Frost Legion recipe for consistency
            r = CreateRecipe(ItemID.SnowGlobe);
            r.AddRecipeGroup("AnySnowBlock", 50);
            r.AddIngredient(ItemID.Glass, 10);
            r.AddIngredient(ItemID.SoulofLight, 3);
            r.AddIngredient(ItemID.SoulofNight, 3);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Gelatin Crystal (Queen Slime summon)
            r = CreateRecipe(ItemID.QueenSlimeCrystal);
            r.AddIngredient(ItemID.CrystalShard, 20);
            r.AddIngredient(ItemID.PinkGel, 10);
            r.AddIngredient(ItemID.SoulofLight, 5);
            r.AddTile(TileID.Solidifier);
            r.Register();

            // Temple Key
            r = CreateRecipe(ItemID.TempleKey);
            r.AddIngredient(ItemID.JungleSpores, 15);
            r.AddIngredient(ItemID.RichMahogany, 15);
            r.AddIngredient(ItemID.SoulofNight, 15);
            r.AddIngredient(ItemID.SoulofLight, 15);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();

            // Lihzahrd Power Cell (NOT Calamity's Old Power Cell)
            r = CreateRecipe(ItemID.LihzahrdPowerCell);
            r.AddIngredient(ItemID.LihzahrdBrick, 15);
            r.AddIngredient(ModContent.ItemType<CoreofSunlight>());
            r.AddTile(TileID.LihzahrdFurnace);
            r.Register();
        }
        #endregion

        #region Early Game Weapons
        private static void AddEarlyGameWeaponRecipes()
        {
            // Shuriken
            Recipe r = CreateRecipe(ItemID.Shuriken, 50);
            r.AddRecipeGroup("IronBar");
            r.AddTile(TileID.Anvils);
            r.Register();

            // Throwing Knife
            r = CreateRecipe(ItemID.ThrowingKnife, 50);
            r.AddRecipeGroup("IronBar");
            r.AddTile(TileID.Anvils);
            r.Register();

            // Wooden Boomerang
            r = CreateRecipe(ItemID.WoodenBoomerang);
            r.AddIngredient(ItemID.Wood, 7);
            r.AddTile(TileID.WorkBenches);
            r.Register();

            // Wand of Sparking
            r = CreateRecipe(ItemID.WandofSparking);
            r.AddIngredient(ItemID.Wood, 5);
            r.AddIngredient(ItemID.Torch, 3);
            r.AddIngredient(ItemID.FallenStar);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Starfury w/ Gold Broadsword
            r = CreateRecipe(ItemID.Starfury);
            r.AddIngredient(ItemID.GoldBroadsword);
            r.AddIngredient(ItemID.FallenStar, 10);
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 3);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Starfury w/ Platinum Broadsword
            r = CreateRecipe(ItemID.Starfury);
            r.AddIngredient(ItemID.PlatinumBroadsword);
            r.AddIngredient(ItemID.FallenStar, 10);
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 3);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Enchanted Sword
            r = CreateRecipe(ItemID.EnchantedSword);
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            r.AddRecipeGroup("AnyGoldBar", 12);
            r.AddIngredient(ItemID.Diamond);
            r.AddIngredient(ItemID.Ruby);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Muramasa
            r = CreateRecipe(ItemID.Muramasa);
            r.AddRecipeGroup("AnyCobaltBar", 15);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Water Bolt w/ Hardmode Spell Tome
            r = CreateRecipe(ItemID.WaterBolt);
            r.AddIngredient(ItemID.SpellTome);
            r.AddIngredient(ItemID.Waterleaf, 3);
            r.AddIngredient(ItemID.WaterCandle);
            r.AddTile(TileID.Bookcases);
            r.Register();

            // Slime Staff
            r = CreateRecipe(ItemID.SlimeStaff);
            r.AddRecipeGroup("Wood", 6);
            r.AddIngredient(ItemID.Gel, 40);
            r.AddIngredient(ItemID.PinkGel, 10);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Ice Boomerang
            r = CreateRecipe(ItemID.IceBoomerang);
            r.AddRecipeGroup("AnyIceBlock", 20);
            r.AddRecipeGroup("AnySnowBlock", 10);
            r.AddIngredient(ItemID.Shiverthorn);
            r.AddTile(TileID.IceMachine);
            r.Register();
        }
        #endregion

        #region Early Game Accessories
        private static void AddEarlyGameAccessoryRecipes()
        {
            // Cloud in a Bottle
            Recipe r = CreateRecipe(ItemID.CloudinaBottle);
            r.AddIngredient(ItemID.Feather, 2);
            r.AddIngredient(ItemID.Bottle);
            r.AddIngredient(ItemID.Cloud, 25);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Hermes Boots
            r = CreateRecipe(ItemID.HermesBoots);
            r.AddIngredient(ItemID.Silk, 10);
            r.AddIngredient(ItemID.SwiftnessPotion, 2);
            r.AddTile(TileID.Loom);
            r.Register();

            // Blizzard in a Bottle
            r = CreateRecipe(ItemID.BlizzardinaBottle);
            r.AddIngredient(ItemID.Feather, 4);
            r.AddIngredient(ItemID.Bottle);
            r.AddRecipeGroup("AnySnowBlock", 50);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Sandstorm in a Bottle
            r = CreateRecipe(ItemID.SandstorminaBottle);
            r.AddIngredient(ModContent.ItemType<DesertFeather>(), 10);
            r.AddIngredient(ItemID.Feather, 6);
            r.AddIngredient(ItemID.Bottle);
            r.AddIngredient(ItemID.SandBlock, 70);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Frog Leg
            r = CreateRecipe(ItemID.FrogLeg);
            r.AddIngredient(ItemID.Frog, 6);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Flying Carpet
            r = CreateRecipe(ItemID.FlyingCarpet);
            r.AddIngredient(ItemID.Silk, 10);
            r.AddIngredient(ModContent.ItemType<DesertFeather>(), 3);
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 5);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Aglet
            r = CreateRecipe(ItemID.Aglet);
            r.AddRecipeGroup("AnyCopperBar", 5);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Anklet of the Wind
            r = CreateRecipe(ItemID.AnkletoftheWind);
            r.AddIngredient(ItemID.JungleSpores, 15);
            r.AddIngredient(ItemID.Cloud, 15);
            r.AddIngredient(ItemID.PinkGel, 5);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Water Walking Boots
            r = CreateRecipe(ItemID.WaterWalkingBoots);
            r.AddIngredient(ItemID.Leather, 5);
            r.AddIngredient(ItemID.WaterWalkingPotion, 8);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Flame Walker Boots
            r = CreateRecipe(ItemID.FlameWakerBoots);
            r.AddIngredient(ItemID.Silk, 8);
            r.AddIngredient(ItemID.Obsidian, 2);
            r.AddTile(TileID.Loom);
            r.Register();

            // Ice Skates
            r = CreateRecipe(ItemID.IceSkates);
            r.AddRecipeGroup("AnyIceBlock", 20);
            r.AddIngredient(ItemID.Leather, 5);
            r.AddRecipeGroup("IronBar", 5);
            r.AddTile(TileID.IceMachine);
            r.Register();

            // Lucky Horseshoe
            r = CreateRecipe(ItemID.LuckyHorseshoe);
            r.AddRecipeGroup("AnyGoldBar", 8);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Shiny Red Balloon
            r = CreateRecipe(ItemID.ShinyRedBalloon);
            r.AddIngredient(ItemID.WhiteString);
            r.AddIngredient(ItemID.Gel, 80);
            r.AddIngredient(ItemID.Cloud, 40);
            r.AddTile(TileID.Solidifier);
            r.Register();

            // Lava Charm
            r = CreateRecipe(ItemID.LavaCharm);
            r.AddIngredient(ItemID.LavaBucket, 5);
            r.AddIngredient(ItemID.Obsidian, 25);
            r.AddRecipeGroup("AnyGoldBar", 5);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Obsidian Rose
            r = CreateRecipe(ItemID.ObsidianRose);
            r.AddIngredient(ItemID.JungleRose);
            r.AddIngredient(ItemID.Obsidian, 10);
            r.AddIngredient(ItemID.Hellstone, 10);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Feral Claws
            r = CreateRecipe(ItemID.FeralClaws);
            r.AddIngredient(ItemID.Leather, 10);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Radar
            r = CreateRecipe(ItemID.Radar);
            r.AddRecipeGroup("IronBar", 5);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Metal Detector
            r = CreateRecipe(ItemID.MetalDetector);
            r.AddIngredient(ItemID.Wire, 10);
            r.AddIngredient(ItemID.GoldDust, 5);
            r.AddIngredient(ItemID.SpelunkerGlowstick, 5);
            r.AddRecipeGroup("AnyGoldBar", 5);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();

            // Hand Warmer
            r = CreateRecipe(ItemID.HandWarmer);
            r.AddIngredient(ItemID.Silk, 5);
            r.AddIngredient(ItemID.Shiverthorn);
            r.AddRecipeGroup("AnySnowBlock", 10);
            r.AddTile(TileID.Loom);
            r.Register();

            // Flower Boots
            r = CreateRecipe(ItemID.FlowerBoots);
            r.AddIngredient(ItemID.Silk, 7);
            r.AddIngredient(ItemID.JungleRose);
            r.AddIngredient(ItemID.JungleGrassSeeds, 5);
            r.AddTile(TileID.Loom);
            r.Register();

            // Band of Regeneration
            r = CreateRecipe(ItemID.BandofRegeneration);
            r.AddIngredient(ItemID.Shackle);
            r.AddIngredient(ItemID.LifeCrystal, 1);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Shoe Spikes
            r = CreateRecipe(ItemID.ShoeSpikes);
            r.AddRecipeGroup("IronBar", 5);
            r.AddIngredient(ItemID.Spike, 10);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Flare Gun
            r = CreateRecipe(ItemID.FlareGun);
            r.AddRecipeGroup("AnyCopperBar", 5);
            r.AddIngredient(ItemID.Torch, 10);
            r.AddTile(TileID.Anvils);
            r.Register();
        }
        #endregion

        #region Armor
        private static void AddArmorRecipes()
        {
            // Eskimo armor
            Recipe r = CreateRecipe(ItemID.EskimoHood);
            r.AddIngredient(ItemID.Silk, 4);
            r.AddIngredient(ItemID.Leather);
            r.AddIngredient(ItemID.BorealWood, 12);
            r.AddTile(TileID.Loom);
            r.Register();

            r = CreateRecipe(ItemID.EskimoCoat);
            r.AddIngredient(ItemID.Silk, 8);
            r.AddIngredient(ItemID.Leather);
            r.AddIngredient(ItemID.BorealWood, 18);
            r.AddTile(TileID.Loom);
            r.Register();

            r = CreateRecipe(ItemID.EskimoPants);
            r.AddIngredient(ItemID.Silk, 6);
            r.AddIngredient(ItemID.Leather);
            r.AddIngredient(ItemID.BorealWood, 15);
            r.AddTile(TileID.Loom);
            r.Register();

            // Pharaoh set
            r = CreateRecipe(ItemID.PharaohsMask);
            r.AddIngredient(ItemID.AncientCloth, 3);
            r.AddTile(TileID.Loom);
            r.Register();

            r = CreateRecipe(ItemID.PharaohsRobe);
            r.AddIngredient(ItemID.AncientCloth, 4);
            r.AddTile(TileID.Loom);
            r.Register();

            // Alternative Turtle, Shroomite, and Spectre Armor set recipes
            r = CreateRecipe(ItemID.TurtleHelmet);
            r.AddIngredient(ItemID.ChlorophyteMask);
            r.AddIngredient(ItemID.TurtleShell);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();

            r = CreateRecipe(ItemID.TurtleScaleMail);
            r.AddIngredient(ItemID.ChlorophytePlateMail);
            r.AddIngredient(ItemID.TurtleShell);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();

            r = CreateRecipe(ItemID.TurtleLeggings);
            r.AddIngredient(ItemID.ChlorophyteGreaves);
            r.AddIngredient(ItemID.TurtleShell);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();

            r = CreateRecipe(ItemID.ShroomiteHelmet);
            r.AddIngredient(ItemID.ChlorophyteHelmet);
            r.AddIngredient(ItemID.GlowingMushroom, 60);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();

            r = CreateRecipe(ItemID.ShroomiteHeadgear);
            r.AddIngredient(ItemID.ChlorophyteHelmet);
            r.AddIngredient(ItemID.GlowingMushroom, 60);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();

            r = CreateRecipe(ItemID.ShroomiteMask);
            r.AddIngredient(ItemID.ChlorophyteHelmet);
            r.AddIngredient(ItemID.GlowingMushroom, 60);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();

            r = CreateRecipe(ItemID.ShroomiteBreastplate);
            r.AddIngredient(ItemID.ChlorophytePlateMail);
            r.AddIngredient(ItemID.GlowingMushroom, 120);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();

            r = CreateRecipe(ItemID.ShroomiteLeggings);
            r.AddIngredient(ItemID.ChlorophyteGreaves);
            r.AddIngredient(ItemID.GlowingMushroom, 80);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();

            r = CreateRecipe(ItemID.SpectreHood);
            r.AddIngredient(ItemID.ChlorophyteHeadgear);
            r.AddIngredient(ItemID.Ectoplasm, 6);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();

            r = CreateRecipe(ItemID.SpectreMask);
            r.AddIngredient(ItemID.ChlorophyteHeadgear);
            r.AddIngredient(ItemID.Ectoplasm, 6);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();

            r = CreateRecipe(ItemID.SpectreRobe);
            r.AddIngredient(ItemID.ChlorophytePlateMail);
            r.AddIngredient(ItemID.Ectoplasm, 12);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();

            r = CreateRecipe(ItemID.SpectrePants);
            r.AddIngredient(ItemID.ChlorophyteGreaves);
            r.AddIngredient(ItemID.Ectoplasm, 9);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();

        }
        #endregion

        #region Ankh Shield Components
        private static void AddAnkhShieldRecipes()
        {
            // Cobalt Shield
            Recipe r = CreateRecipe(ItemID.CobaltShield);
            r.AddRecipeGroup("AnyCobaltBar", 10);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Armor Polish (broken armor)
            r = CreateRecipe(ItemID.ArmorPolish);
            r.AddIngredient(ItemID.Bone, 50);
            r.AddIngredient(ModContent.ItemType<AncientBoneDust>(), 3);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();

            // Adhesive Bandage (bleeding)
            r = CreateRecipe(ItemID.AdhesiveBandage);
            r.AddIngredient(ItemID.Silk, 10);
            r.AddIngredient(ItemID.Gel, 50);
            r.AddIngredient(ItemID.HealingPotion);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Bezoar (poison)
            r = CreateRecipe(ItemID.Bezoar);
            r.AddIngredient(ItemID.Stinger, 15);
            r.AddIngredient(ModContent.ItemType<MurkyPaste>());
            r.AddTile(TileID.Anvils);
            r.Register();

            // Nazar (curse)
            r = CreateRecipe(ItemID.Nazar);
            r.AddIngredient(ItemID.SoulofNight, 15);
            r.AddIngredient(ItemID.Lens, 3);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();

            // Vitamins (weakness)
            r = CreateRecipe(ItemID.Vitamins);
            r.AddIngredient(ItemID.BottledWater);
            r.AddIngredient(ItemID.Waterleaf, 5);
            r.AddIngredient(ItemID.Blinkroot, 5);
            r.AddIngredient(ItemID.Daybloom, 5);
            r.AddIngredient(ModContent.ItemType<BeetleJuice>(), 3);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Blindfold (darkness)
            r = CreateRecipe(ItemID.Blindfold);
            r.AddIngredient(ItemID.Silk, 30);
            r.AddIngredient(ItemID.TatteredCloth, 3);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Trifold Map (confusion)
            r = CreateRecipe(ItemID.TrifoldMap);
            r.AddIngredient(ItemID.Silk, 20);
            r.AddIngredient(ItemID.SoulofLight, 3);
            r.AddIngredient(ItemID.SoulofNight, 3);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();

            // Fast Clock (slow)
            r = CreateRecipe(ItemID.FastClock);
            r.AddIngredient(ItemID.Timer1Second);
            r.AddIngredient(ItemID.PixieDust, 15);
            r.AddIngredient(ItemID.SoulofLight, 5);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();

            // Megaphone (silence)
            r = CreateRecipe(ItemID.Megaphone);
            r.AddIngredient(ItemID.Wire, 10);
            r.AddRecipeGroup("AnyCobaltBar", 5);
            r.AddIngredient(ItemID.Ruby, 3);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();

            // Pocket Mirror (petrification, added to Ankh charm+ in Calamity)
            r = CreateRecipe(ItemID.PocketMirror);
            r.AddIngredient(ItemID.Glass, 10);
            r.AddRecipeGroup("AnyGoldBar", 4);
            r.AddIngredient(ItemID.CrystalShard, 2);
            r.AddIngredient(ItemID.SoulofNight, 2);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();
        }
        #endregion

        #region Living Wood
        private static void AddLivingWoodRecipes()
        {
            // Living Loom
            Recipe r = CreateRecipe(ItemID.LivingLoom);
            r.AddIngredient(ItemID.Loom);
            r.AddIngredient(ItemID.Vine, 2);
            r.AddTile(TileID.Sawmill);
            r.Register();

            // Living Wood Wand
            r = CreateRecipe(ItemID.LivingWoodWand);
            r.AddIngredient(ItemID.Wood, 30);
            r.AddTile(TileID.LivingLoom);
            r.Register();

            // Living Leaf Wand
            r = CreateRecipe(ItemID.LeafWand);
            r.AddIngredient(ItemID.Wood, 30);
            r.AddTile(TileID.LivingLoom);
            r.Register();

            // Living Mahogany Wand
            r = CreateRecipe(ItemID.LivingMahoganyWand);
            r.AddIngredient(ItemID.RichMahogany, 30);
            r.AddTile(TileID.LivingLoom);
            r.Register();

            // Living Mahogany Leaf Wand
            r = CreateRecipe(ItemID.LivingMahoganyLeafWand);
            r.AddIngredient(ItemID.RichMahogany, 30);
            r.AddTile(TileID.LivingLoom);
            r.Register();
        }
        #endregion

        #region Hardmode Items and Accessories
        private static void AddHardmodeItemRecipes()
        {
            // Celestial Magnet
            Recipe r = CreateRecipe(ItemID.CelestialMagnet);
            r.AddIngredient(ItemID.FallenStar, 20);
            r.AddIngredient(ItemID.SoulofMight, 10);
            r.AddIngredient(ItemID.SoulofLight, 5);
            r.AddIngredient(ItemID.SoulofNight, 5);
            r.AddRecipeGroup("AnyCobaltBar", 3);
            r.AddTile(TileID.Anvils);
            r.Register();

            // Frozen Turtle Shell
            r = CreateRecipe(ItemID.FrozenTurtleShell);
            r.AddIngredient(ItemID.TurtleShell, 3);
            r.AddIngredient(ModContent.ItemType<EssenceofEleum>(), 9);
            r.AddTile(TileID.IceMachine);
            r.Register();

            // Magic Quiver
            r = CreateRecipe(ItemID.MagicQuiver);
            r.AddIngredient(ItemID.EndlessQuiver);
            r.AddIngredient(ItemID.PixieDust, 10);
            r.AddIngredient(ItemID.Lens, 5);
            r.AddIngredient(ItemID.SoulofLight, 8);
            r.AddTile(TileID.CrystalBall);
            r.Register();

            // Terra Blade w/ True Bloody Edge
            r = CreateRecipe(ItemID.TerraBlade);
            r.AddIngredient(ModContent.ItemType<TrueBloodyEdge>());
            r.AddIngredient(ItemID.TrueExcalibur);
            r.AddIngredient(ModContent.ItemType<LivingShard>(), 7);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();

            // Turtle Shell with Giant Tortoise Shell
            r = CreateRecipe(ItemID.TurtleShell);
            r.AddIngredient(ModContent.ItemType<GiantTortoiseShell>());
            r.Register();

            // Pulse Bow
            r = CreateRecipe(ItemID.PulseBow);
            r.AddIngredient(ItemID.ShroomiteBar, 16);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();

            // Sergeant United Shield
            r = CreateRecipe(ItemID.BouncingShield);
            r.AddRecipeGroup("AnyCobaltBar", 12);
            r.AddIngredient(ItemID.SoulofLight, 4);
            r.AddTile(TileID.Anvils);
            r.Register();
        }
        #endregion
    }
}

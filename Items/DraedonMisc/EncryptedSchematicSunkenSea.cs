using System;
using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.DraedonMisc
{
    public class EncryptedSchematicSunkenSea : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Schematic (Sunken Sea)");
            Tooltip.SetDefault("Finely detailed diagrams of numerous devices and weaponry dance across the holographic screen.\n" +
            "The weaponry I supply to the workers of the laboratories is weak. Hardly suited for battle.\n" +
            "However, they suffice for self defense against any lab mechanisms or creations which may have gone rogue.\n" +
            "Addendum: For those who think themselves powerful, search the upper bounds of this planet’s atmosphere for a structure similar to that of the Sunken Seas.\n" +
            "I will know by the end if you are worthy of battling my creations.\n" +
            "Picking up this item or holding it in your inventory permanently unlocks new recipes");
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.DraedonRust;
            Item.maxStack = 1;
        }

        public override void UpdateInventory(Player player)
        {
            // Since no decrypting is necessary for this item simply placing it in your inventory is sufficient enough to "learn"
            // from it.
            if (Main.netMode != NetmodeID.MultiplayerClient && (!RecipeUnlockHandler.HasUnlockedT1ArsenalRecipes || !RecipeUnlockHandler.HasFoundSunkenSeaSchematic))
            {
                RecipeUnlockHandler.HasFoundSunkenSeaSchematic = true;
                RecipeUnlockHandler.HasUnlockedT1ArsenalRecipes = true;
                CalamityNetcode.SyncWorld();
            }
        }

        // Recipe exists for posierity.
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(10).
                AddIngredient<DubiousPlating>(10).
                AddIngredient(ItemID.Glass, 50).
                AddCondition(SchematicRecipe.ConstructRecipeCondition("Sunken Sea", out Predicate<Recipe> condition), condition).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}

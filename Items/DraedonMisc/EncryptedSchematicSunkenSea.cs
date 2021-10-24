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
            DisplayName.SetDefault("Schematic");
            Tooltip.SetDefault("Finely detailed diagrams of numerous devices and weaponry dance across the holographic screen.\n";
            "The weaponry I supply to the workers of the laboratories is weak. Hardly suited for battle.\n";
            "However, they suffice for self defense against any lab mechanisms or creations which may have gone rogue.\n";
            "Addendum: For those who think themselves powerful, search the upper bounds of this planet’s atmosphere for a structure similar to that of the Sunken Seas.\n";
            "I will know by the end if you are worthy of battling my creations.\n" +
            "Picking up this item or holding it in your inventory permanently unlocks new recipes");
        }

        public override void SetDefaults()
        {
            item.width = 42;
            item.height = 42;
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.DraedonRust;
            item.maxStack = 1;
        }

        public override void UpdateInventory(Player player)
        {
            // Since no decrypting is necessary for this item simply placing it in your inventory is sufficient enough to "learn"
            // from it.
            if (Main.myPlayer == player.whoAmI && (!RecipeUnlockHandler.HasUnlockedT1ArsenalRecipes || !RecipeUnlockHandler.HasFoundSunkenSeaSchematic))
            {
                RecipeUnlockHandler.HasFoundSunkenSeaSchematic = true;
                RecipeUnlockHandler.HasUnlockedT1ArsenalRecipes = true;
                CalamityNetcode.SyncWorld();
            }
        }

        // Recipe exists for posierity.
        public override void AddRecipes()
        {
            SchematicRecipe recipe = new SchematicRecipe(mod, "Sunken Sea");
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 10);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 10);
            recipe.AddIngredient(ItemID.Glass, 50);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}

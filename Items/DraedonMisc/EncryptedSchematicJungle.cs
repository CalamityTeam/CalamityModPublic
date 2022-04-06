using System;
using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.DraedonMisc
{
    public class EncryptedSchematicJungle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Encrypted Schematic");
            Tooltip.SetDefault("Requires a Codebreaker with a fine tuned, long range sensor to decrypt");
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
            if (Main.netMode != NetmodeID.MultiplayerClient && !RecipeUnlockHandler.HasFoundJungleSchematic)
            {
                RecipeUnlockHandler.HasFoundJungleSchematic = true;
                CalamityNetcode.SyncWorld();
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(10).
                AddIngredient<DubiousPlating>(10).
                AddIngredient(ItemID.Glass, 50).
                AddCondition(SchematicRecipe.ConstructRecipeCondition("Jungle", out Predicate<Recipe> condition), condition).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}

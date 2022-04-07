using System;
using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.DraedonMisc
{
    public class EncryptedSchematicIce : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Encrypted Schematic");
            Tooltip.SetDefault("Requires a Codebreaker with a complex voltage regulation system to decrypt");
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
            if (Main.netMode != NetmodeID.MultiplayerClient && !RecipeUnlockHandler.HasFoundIceSchematic)
            {
                RecipeUnlockHandler.HasFoundIceSchematic = true;
                CalamityNetcode.SyncWorld();
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(10).
                AddIngredient<DubiousPlating>(10).
                AddIngredient(ItemID.Glass, 50).
                AddCondition(SchematicRecipe.ConstructRecipeCondition("Ice", out Predicate<Recipe> condition), condition).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}

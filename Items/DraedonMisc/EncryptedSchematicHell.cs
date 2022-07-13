using System;
using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using System.Linq;

namespace CalamityMod.Items.DraedonMisc
{
    public class EncryptedSchematicHell : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Encrypted Schematic (Underworld)");
            Tooltip.SetDefault("Requires a Codebreaker with a sophisticated display to decrypt");
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
            if (Main.netMode != NetmodeID.MultiplayerClient && !RecipeUnlockHandler.HasFoundHellSchematic)
            {
                RecipeUnlockHandler.HasFoundHellSchematic = true;
                CalamityNetcode.SyncWorld();
            }
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip0");

            if (line != null && RecipeUnlockHandler.HasUnlockedT4ArsenalRecipes)
                line.Text = "Has already been decrypted";
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(10).
                AddIngredient<DubiousPlating>(10).
                AddIngredient(ItemID.Glass, 50).
                AddCondition(SchematicRecipe.ConstructRecipeCondition("Hell", out Predicate<Recipe> condition), condition).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}

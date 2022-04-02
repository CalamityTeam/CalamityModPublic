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
            item.width = 42;
            item.height = 42;
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.DraedonRust;
            item.maxStack = 1;
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
            SchematicRecipe recipe = new SchematicRecipe(mod, "Ice");
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 10);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 10);
            recipe.AddIngredient(ItemID.Glass, 50);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}

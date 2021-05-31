using CalamityMod.CustomRecipes;
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
            Tooltip.SetDefault("You can barely make out its text. It states:\n" +
                "Something idk\n" +
                "Having this item in your inventory permanently unlocks special equipment");
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
            if (Main.myPlayer == player.whoAmI && !RecipeUnlockHandler.HasUnlockedT1ArsenalRecipes)
            {
                RecipeUnlockHandler.HasUnlockedT1ArsenalRecipes = true;
                CalamityNetcode.SyncWorld();
            }
        }
    }
}

using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class AntiTumorOintment : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Anti-Tumor Ointment");
            Tooltip.SetDefault("Favorite this item to prevent hive cysts from spawning near you");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 34;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateInventory(Player player)
        {
            if (Item.favorited)
                player.Calamity().disableHiveCystSpawns = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.DemoniteBar, 5).
                AddIngredient(ItemID.BottledWater).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}

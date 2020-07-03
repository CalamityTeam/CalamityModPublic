using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeKingSlime : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("King Slime");
            Tooltip.SetDefault("Only a fool could be caught by this pitiful excuse for a hunter.\n" +
                "Unfortunately, our world has no shortage of those.\n" +
				"Place in your inventory to gain a slight movement speed and jump boost.\n" +
				"However, your defense is slightly reduced due to your gelatinous body.\n" +
				"These effects only occur if the item is favorited.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 1;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void UpdateInventory(Player player)
        {
			if (item.favorited)
				player.Calamity().kingSlimeLore = true;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddTile(TileID.Bookcases);
            r.AddIngredient(ItemID.KingSlimeTrophy);
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            r.AddRecipe();
        }
    }
}

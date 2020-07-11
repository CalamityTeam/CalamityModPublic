using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeProvidence : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Providence, the Profaned Goddess");
            Tooltip.SetDefault("A core surrounded by stone and flame, a simple origin and a simple goal.\n" +
                "What would have become of us had she not been defeated is a frightening concept to consider.\n" +
                "Favorite this item to imbue all projectiles with profaned flames, causing them to inflict extra damage.\n" +
				"However, your max life is decreased due to a part of your soul being required to fuel the profaned flames.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 10;
            item.consumable = false;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
			if (item.favorited)
				modPlayer.providenceLore = true;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddTile(TileID.Bookcases);
            r.AddIngredient(ModContent.ItemType<ProvidenceTrophy>());
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            r.AddRecipe();
        }
    }
}

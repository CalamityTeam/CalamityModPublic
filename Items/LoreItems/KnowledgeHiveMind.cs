using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeHiveMind : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Hive Mind");
            Tooltip.SetDefault("A hive of clustered microbial-infected flesh.\n" +
                "I do not believe killing it will lessen the corruption here.\n" +
                "Favorite this item for all of your projectiles to inflict cursed flames when in the corruption.\n" +
				"However, enemy spawn rates will be greatly reduced while in the corruption due to your overwhelmingly-putrid odor.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 3;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            if (player.ZoneCorrupt && item.favorited)
            {
                CalamityPlayer modPlayer = player.Calamity();
                modPlayer.hiveMindLore = true;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddTile(TileID.Bookcases);
            r.AddIngredient(ModContent.ItemType<HiveMindTrophy>());
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            r.AddRecipe();
        }
    }
}

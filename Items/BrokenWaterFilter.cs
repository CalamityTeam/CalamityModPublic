using CalamityMod.Items.Materials;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items
{
    public class BrokenWaterFilter : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Broken Water Filter");
            Tooltip.SetDefault("Favorite this item to disable natural Acid Rain spawns");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 34;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
        }
        public override void UpdateInventory(Player player)
        {
            if (Item.favorited)
                player.Calamity().noStupidNaturalARSpawns = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SulfuricScale>(20).
                AddIngredient(ItemID.IronBar, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}

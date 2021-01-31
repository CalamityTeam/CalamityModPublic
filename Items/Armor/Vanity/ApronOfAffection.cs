using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Body)]
    public class ApronOfAffection : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ace's Apron of Affection");
            Tooltip.SetDefault("Great for hugging people");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.rare = 5;
            item.value = Item.buyPrice(0, 15, 0, 0);
            item.vanity = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Robe);
            recipe.AddIngredient(ItemID.LovePotion, 10);
            recipe.AddIngredient(ItemID.LifeCrystal);
            recipe.AddTile(TileID.Loom);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.AbyssItems
{
    public class LumenousAmulet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lumenous Amulet");
            Tooltip.SetDefault("Attacks inflict the Crush Depth debuff\n" +
                "While in the abyss you gain 25% increased max life\n" +
                "Provides a moderate amount of light in the abyss");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = Item.buyPrice(0, 15, 0, 0);
            item.rare = 5;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.abyssalAmulet = true;
            modPlayer.lumenousAmulet = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "AbyssalAmulet");
            recipe.AddIngredient(null, "Lumenite", 15);
            recipe.AddIngredient(null, "Tenebris", 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

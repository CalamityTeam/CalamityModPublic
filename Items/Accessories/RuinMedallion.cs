using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class RuinMedallion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ruin Medallion");
            Tooltip.SetDefault("Stealth strikes only expend 50% of your max stealth\n" +
                "6% increased rogue damage, and 6% increased rogue crit chance");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.buyPrice(0, 13, 0, 0);
            item.rare = 6;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.stealthStrikeHalfCost = true;
            modPlayer.throwingCrit += 6;
            modPlayer.throwingDamage += 0.06f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType("CoinofDeceit"));
            recipe.AddIngredient(mod.ItemType("UnholyCore"), 4);
            recipe.AddIngredient(mod.ItemType("EssenceofChaos"), 2);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

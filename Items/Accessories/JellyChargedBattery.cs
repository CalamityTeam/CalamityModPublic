using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class JellyChargedBattery : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Jelly-Charged Battery");
            Tooltip.SetDefault("+1 max minions and 7% minion damage\n" +
                               "Minion attacks spawn orbs of energy and inflict Electrified");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 22;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.accessory = true;
            item.rare = ItemRarityID.LightRed;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().voltaicJelly = true;
            player.Calamity().jellyChargedBattery = true;
            player.minionDamage += 0.07f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<WulfrumBattery>());
            recipe.AddIngredient(ModContent.ItemType<VoltaicJelly>());
            recipe.AddIngredient(ModContent.ItemType<PurifiedGel>(), 10);
            recipe.AddIngredient(ModContent.ItemType<StormlionMandible>(), 2);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

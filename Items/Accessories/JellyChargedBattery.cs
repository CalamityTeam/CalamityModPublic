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
            Item.width = 20;
            Item.height = 22;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().voltaicJelly = true;
            player.Calamity().jellyChargedBattery = true;
            player.GetDamage(DamageClass.Summon) += 0.07f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WulfrumBattery>().
                AddIngredient<VoltaicJelly>().
                AddIngredient<PurifiedGel>(10).
                AddIngredient<StormlionMandible>(2).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}

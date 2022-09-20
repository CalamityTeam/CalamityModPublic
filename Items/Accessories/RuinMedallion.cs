using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class RuinMedallion : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Ruin Medallion");
            Tooltip.SetDefault("Stealth strikes only expend 75% of your max stealth\n" +
                "6% increased rogue damage, and 6% increased rogue crit chance");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 28;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.stealthStrike75Cost = true;
            player.GetCritChance<ThrowingDamageClass>() += 6;
            player.GetDamage<ThrowingDamageClass>() += 0.06f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CoinofDeceit>().
                AddIngredient<UnholyCore>(4).
                AddIngredient<EssenceofChaos>(2).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class AtaxiaSubligar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hydrothermic Subligar");
            Tooltip.SetDefault("5% increased critical strike chance\n" +
                "10% increased movement speed");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 18, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 14;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().AllCritBoost(5);
            player.moveSpeed += 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CruptixBar>(10)
                .AddIngredient(ItemID.HellstoneBar, 5)
                .AddIngredient<CoreofChaos>(2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}

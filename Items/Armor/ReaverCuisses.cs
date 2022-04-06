using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class ReaverCuisses : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaver Cuisses");
            Tooltip.SetDefault("5% increased critical strike chance\n" +
                "12% increased movement speed");
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 18, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.defense = 14;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().AllCritBoost(5);
            player.moveSpeed += 0.12f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<DraedonBar>(10)
                .AddIngredient(ItemID.JungleSpores, 8)
                .AddIngredient<EssenceofCinder>(2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}

using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class TarragonLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tarragon Leggings");
            Tooltip.SetDefault("Leggings of a fabled explorer\n" +
                "10% increased movement speed\n" +
                "8% increased damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 30, 0, 0);
            Item.defense = 32;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.1f;
            player.allDamage += 0.08f;
            player.Calamity().AllCritBoost(8);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<UeliaceBar>(11)
                .AddIngredient<DivineGeode>(12)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}

using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class BloodflareCuisses : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodflare Cuisses");
            Tooltip.SetDefault("17% increased movement speed, 10% increased damage and 7% increased critical strike chance");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.defense = 29;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.17f;
            player.GetDamage<GenericDamageClass>() += 0.1f;
            player.Calamity().AllCritBoost(7);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BloodstoneCore>(13).
                AddIngredient<RuinousSoul>(3).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}

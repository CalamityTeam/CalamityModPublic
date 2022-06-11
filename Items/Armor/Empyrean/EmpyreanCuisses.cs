using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Empyrean
{
    [AutoloadEquip(EquipType.Legs)]
    [LegacyName("XerocCuisses")]
    public class EmpyreanCuisses : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Empyrean Cuisses");
            Tooltip.SetDefault("5% increased rogue damage and critical strike chance\n" +
                    "15% increased movement speed");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 24, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.defense = 24;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<ThrowingDamageClass>() += 5;
            player.GetDamage<ThrowingDamageClass>() += 0.05f;
            player.moveSpeed += 0.15f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MeldConstruct>(18).
                AddIngredient(ItemID.LunarBar, 12).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}

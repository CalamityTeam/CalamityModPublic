using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Reaver
{
    [AutoloadEquip(EquipType.Legs)]
    public class ReaverCuisses : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Reaver Cuisses");
            Tooltip.SetDefault("5% increased critical strike chance\n" +
                "12% increased movement speed");
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.defense = 14;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<GenericDamageClass>() += 5;
            player.moveSpeed += 0.12f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PerennialBar>(10).
                AddIngredient(ItemID.JungleSpores, 8).
                AddIngredient<EssenceofSunlight>(2).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

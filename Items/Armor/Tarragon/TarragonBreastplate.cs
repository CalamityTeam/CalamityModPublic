using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Tarragon
{
    [AutoloadEquip(EquipType.Body)]
    public class TarragonBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Tarragon Breastplate");
            Tooltip.SetDefault("Breastplate of the exiler\n" +
                    "10% increased damage and 5% increased critical strike chance\n" +
                    "+2 life regen and +40 max life");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.lifeRegen = 2;
            Item.value = Item.buyPrice(0, 40, 0, 0);
            Item.defense = 37;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 40;
            player.GetDamage<GenericDamageClass>() += 0.1f;
            player.GetCritChance<GenericDamageClass>() += 5;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UeliaceBar>(15).
                AddIngredient<DivineGeode>(18).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}

using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Aerospec
{
    [AutoloadEquip(EquipType.Body)]
    public class AerospecBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Aerospec Breastplate");
            Tooltip.SetDefault("3% increased critical strike chance");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.defense = 7;
        }

        public override void UpdateEquip(Player player) => player.GetCritChance<GenericDamageClass>() += 3;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AerialiteBar>(11).
                AddIngredient(ItemID.SunplateBlock, 8).
                AddIngredient(ItemID.Feather, 2).
                AddTile(TileID.SkyMill).
                Register();
        }
    }
}

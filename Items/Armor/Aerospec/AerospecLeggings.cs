using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Aerospec
{
    [AutoloadEquip(EquipType.Legs)]
    public class AerospecLeggings : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PreHardmode";

        public static float MoveSpeedBoost = 0.12f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MoveSpeedBoost.ToPercent());

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.defense = 6;
        }

        public override void UpdateEquip(Player player) => player.moveSpeed += MoveSpeedBoost;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AerialiteBar>(10).
                AddIngredient(ItemID.Feather, 2).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}

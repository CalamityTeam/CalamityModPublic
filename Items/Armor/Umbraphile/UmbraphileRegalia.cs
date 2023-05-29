using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Umbraphile
{
    [AutoloadEquip(EquipType.Body)]
    public class UmbraphileRegalia : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Armor";
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 24;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.defense = 16;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ThrowingDamageClass>() += 0.1f;
            player.GetCritChance<ThrowingDamageClass>() += 10;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SolarVeil>(18).
                AddIngredient(ItemID.HallowedBar, 15).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

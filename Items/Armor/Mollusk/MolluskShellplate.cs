using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Mollusk
{
    [AutoloadEquip(EquipType.Body)]
    public class MolluskShellplate : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Armor";
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 22;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 22;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.1f;
            player.GetCritChance<GenericDamageClass>() += 6;
            player.moveSpeed -= 0.15f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MolluskHusk>(15).
                AddIngredient<SeaPrism>(25).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}

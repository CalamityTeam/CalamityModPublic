using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.PlagueReaper
{
    [AutoloadEquip(EquipType.Legs)]
    public class PlagueReaperStriders : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Armor";
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 11;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<RangedDamageClass>() += 3;
            player.moveSpeed += 0.15f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.NecroGreaves).
                AddIngredient<PlagueCellCanister>(21).
                AddIngredient(ItemID.Nanites, 17).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

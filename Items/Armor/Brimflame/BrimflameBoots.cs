using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Brimflame
{
    [AutoloadEquip(EquipType.Legs)]
    public class BrimflameBoots : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Armor.Hardmode";
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.defense = 15;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.05f;
            player.GetDamage<MagicDamageClass>() += 0.05f;
            player.fireWalk = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AshesofCalamity>(5).
                AddIngredient<UnholyCore>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

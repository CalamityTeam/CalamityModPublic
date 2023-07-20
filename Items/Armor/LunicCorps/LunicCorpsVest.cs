using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.LunicCorps
{
    [AutoloadEquip(EquipType.Body)]
    public class LunicCorpsVest : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Hardmode";
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.defense = 20;
            Item.rare = ItemRarityID.Cyan;
            Item.Calamity().donorItem = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<RangedDamageClass>() += 0.03f;
            player.GetCritChance<RangedDamageClass>() += 11;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<RoverDrive>().
                AddIngredient<AstralBar>(11).
                AddIngredient(ItemID.ChlorophyteBar, 11).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}

using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class TitanHeartMantle : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Titan Heart Mantle");
            Tooltip.SetDefault("45% chance to not consume rogue items\n" +
            "5% boosted rogue knockback but 15% lowered rogue attack speed");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 12, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 17;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().titanHeartMantle = true;
            player.Calamity().rogueAmmoCost *= 0.55f;
            // 15% attack speed penalty
            player.GetAttackSpeed<ThrowingDamageClass>() -= 0.15f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AstralMonolith>(20).
                AddIngredient<TitanHeart>().
                AddTile(TileID.Anvils).
                Register();
        }
    }
}

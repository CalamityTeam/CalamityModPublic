using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class UmbraphileRegalia : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Umbraphile Regalia");
            Tooltip.SetDefault("10% increased rogue damage and 10% increased rogue crit");
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 24;
            Item.value = Item.buyPrice(0, 24, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.defense = 16;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().throwingDamage += 0.1f;
            player.Calamity().throwingCrit += 10;
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

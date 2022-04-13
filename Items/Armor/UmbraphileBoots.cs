using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class UmbraphileBoots : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Umbraphile Boots");
            Tooltip.SetDefault("9% increased rogue damage and 6% increased rogue crit\n" +
                               "20% increased movement speed");
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 18, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.defense = 12;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.2f;
            player.Calamity().throwingDamage += 0.09f;
            player.Calamity().throwingCrit += 6;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SolarVeil>(14).
                AddIngredient(ItemID.HallowedBar, 11).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

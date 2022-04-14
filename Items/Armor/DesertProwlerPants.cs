using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class DesertProwlerPants : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Desert Prowler Pants");
            Tooltip.SetDefault("10% increased movement speed and immunity to the Mighty Wind debuff");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 2;
        }

        public override void UpdateEquip(Player player)
        {
            player.buffImmune[BuffID.WindPushed] = true;
            player.moveSpeed += 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DesertFeather>().
                AddIngredient(ItemID.Silk, 5).
                AddTile(TileID.Loom).
                Register();
        }
    }
}

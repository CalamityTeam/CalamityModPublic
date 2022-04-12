using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class SandSharkToothNecklace : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sand Shark Tooth Necklace");
            Tooltip.SetDefault("Increases armor penetration by 10\n" + "6% increased damage");
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 44;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<GenericDamageClass>() += 0.06f;
            player.GetArmorPenetration<GenericDamageClass>() += 10;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SharkToothNecklace).
                AddIngredient(ItemID.AvengerEmblem).
                AddIngredient<GrandScale>().
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}

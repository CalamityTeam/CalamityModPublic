using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Back)]
    public class GiantShell : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Giant Shell");
            Tooltip.SetDefault("15% reduced movement speed\n" +
                "Taking a hit will make you move very fast for a short time");
        }

        public override void SetDefaults()
        {
            Item.defense = 10;
            Item.width = 30;
            Item.height = 28;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed -= 0.15f;
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.gShell = true;
        }
    }
}

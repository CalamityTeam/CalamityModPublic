using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Back)]
    public class GiantShell : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Giant Shell");
            Tooltip.SetDefault("15% reduced movement speed and 5% increased damage reduction\n" +
                "Taking a hit will make you move very fast for a short time");
        }

        public override void SetDefaults()
        {
            item.defense = 6;
            item.width = 30;
            item.height = 28;
            item.value = CalamityGlobalItem.Rarity1BuyPrice;
            item.rare = ItemRarityID.Blue;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.endurance += 0.05f;
            player.moveSpeed -= 0.15f;
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.gShell = true;
        }
    }
}

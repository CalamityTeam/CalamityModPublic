using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Waist)]
    public class DepthCharm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Depths Charm");
            Tooltip.SetDefault("Reduces the damage caused by the pressure of the abyss while out of breath\n" +
                "Removes the bleed effect caused by the upper layers of the abyss");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Orange;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.depthCharm = true;
        }
    }
}

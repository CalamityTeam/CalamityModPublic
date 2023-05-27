using CalamityMod.CalPlayer;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("DukeScales")]
    public class OldDukeScales : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 26;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.oldDukeScales = true;
        }
    }
}

using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class FrostBarrier : ModItem
    {
        public override void SetDefaults()
        {
            Item.defense = 4;
            Item.width = 20;
            Item.height = 24;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.fBarrier = true;
            player.buffImmune[BuffID.Chilled] = true;
        }
    }
}

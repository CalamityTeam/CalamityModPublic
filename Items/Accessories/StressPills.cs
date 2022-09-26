using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class StressPills : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Stress Pills");
            Tooltip.SetDefault("Adrenaline charges 20% faster\n" +
                "Increases your max movement speed and acceleration by 5%\n" +
                "Revengeance drop");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.defense = 4;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.stressPills = true;
        }
    }
}

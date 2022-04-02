using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class DragonScales : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon Scales");
            Tooltip.SetDefault("Only a living dragon holds true treasure\n" +
                               "Rogue projectiles create slow fireballs as they travel\n" +
                               "Stealth strikes create infernados on death\n" +
                               "+10% max run speed and acceleration");
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 34;
            item.value = CalamityGlobalItem.Rarity15BuyPrice;
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dragonScales = true;
        }
    }
}

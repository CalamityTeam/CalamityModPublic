using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class DragonScales : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon Scales");
            Tooltip.SetDefault("Causes all rogue projectiles to emit a slow fireball as it travels\n" +
                               "Stealth strikes create infernados on death\n" +
                               "After Yharon is dead, you are granted 10% movement speed and acceleration\n" +
                               "'Only a living dragon holds true treasure'");
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 34;
            item.value = Item.buyPrice(0, 90, 0, 0);
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 15;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dragonScales = true;
        }
    }
}

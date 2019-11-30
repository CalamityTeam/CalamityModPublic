using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class SandCloak : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sand Cloak");
            Tooltip.SetDefault("+1 defense and 5% increased movement speed\n" +
                "Press C to consume 25% of your maximum stealth to create a protective dust veil which provides +6 defense and +2 life regen\n" + 
                "This effect has a 30 second cooldown before it can be used again");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 44;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 1;
            player.moveSpeed += 0.05f;
            player.Calamity().sandCloak = true;
        }
    }
}

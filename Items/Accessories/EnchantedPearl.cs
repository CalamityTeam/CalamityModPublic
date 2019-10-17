using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class EnchantedPearl : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Enchanted Pearl");
            Tooltip.SetDefault("Increases fishing skill\nCrate potion effect, does not stack with crate potions");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = Item.buyPrice(0, 9, 0, 0);
            item.rare = 7;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.fishingSkill += 10;
            player.cratePotion = true;
        }
    }
}

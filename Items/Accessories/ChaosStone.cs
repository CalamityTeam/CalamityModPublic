using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class ChaosStone : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chaos Stone");
            Tooltip.SetDefault("One of the ancient relics\n" +
                "When drinking mana potions you recieve Mana Burn instead of Mana Sickness\n" +
                "Mana Burn does not apply any magic damage penalties. Instead, you take damage over time relative to the intensity of the debuff");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(8, 7));
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = CalamityGlobalItem.Rarity7BuyPrice;
            item.rare = ItemRarityID.Lime;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) => player.Calamity().ChaosStone = true;
    }
}

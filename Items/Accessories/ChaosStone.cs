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
                "Mana Sickness from drinking mana potions is replaced by Mana Burn\n" +
                "Mana Burn deals damage over time relative to the intensity of the debuff\n" +
                "This debuff does not reduce your magic damage");
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

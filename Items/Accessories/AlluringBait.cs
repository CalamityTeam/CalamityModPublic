using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class AlluringBait : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Alluring Bait");
            Tooltip.SetDefault("30 increased fishing power during the day\n" +
                "45 increased fishing power during the night\n" +
                "60 increased fishing power during a solar eclipse\n" +
                "Greatly increases chance of catching potion ingredient fish");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().alluringBait = true;

            if (Main.eclipse)
                player.fishingSkill += 60;
            else if (!Main.dayTime)
                player.fishingSkill += 45;
            else
                player.fishingSkill += 30;
        }
    }
}

using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class AlluringBait : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Alluring Bait");
            Tooltip.SetDefault("30 increased fishing power\n" +
                "Greatly increases chance of catching potion ingredient fish\n" +
				"Potion ingredient fish yield is increased");
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
            player.fishingSkill += 30;
        }
    }
}

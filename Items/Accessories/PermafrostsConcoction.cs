using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class PermafrostsConcoction : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Permafrost's Concoction");
            Tooltip.SetDefault(@"Increases maximum mana by 50 and reduces mana cost by 15%
Increases life regen as life decreases
Increases life regen when afflicted with Poison, On Fire, or Brimstone Flames
You will survive fatal damage and revive with 30% life on a 3 minute cooldown
You are encased in an ice barrier for 3 seconds when revived");
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 36;
            Item.height = 34;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().permafrostsConcoction = true;
        }
    }
}

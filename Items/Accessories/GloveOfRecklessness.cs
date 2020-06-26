using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class GloveOfRecklessness : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glove Of Recklessness");
            Tooltip.SetDefault("Increases rogue attack speed by 20% but decreases damage by 10%\n" +
                               "Adds inaccuracy to rogue weapons");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 40;
            item.value = CalamityGlobalItem.Rarity7BuyPrice;
            item.accessory = true;
            item.rare = 7;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.gloveOfRecklessness = true;
            modPlayer.throwingDamage -= 0.1f;
        }
    }
}

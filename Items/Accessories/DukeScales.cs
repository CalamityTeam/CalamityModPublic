using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class DukeScales : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Old Duke's Scales");
            Tooltip.SetDefault("While under the effects of a damaging debuff, you gain 10% increased damage and 5% crit\n" +
                "For every 25% of lost life, you gain 6% increased damage and 3% crit\n" +
                "This will max out at 18% increased damage and 9% crit when under 25% life\n" +
                "Provides immunity to poisoned, venom, and sulphuric poisoning");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 26;
            item.accessory = true;
            item.value = CalamityGlobalItem.Rarity13BuyPrice;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dukeScales = true;
        }
    }
}

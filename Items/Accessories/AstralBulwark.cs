using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Buffs;
namespace CalamityMod.Items.Accessories
{
    public class AstralBulwark : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Bulwark");
            Tooltip.SetDefault("Taking damage drops astral stars from the sky\n" +
                               "Provides immunity to the astral infection debuff");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = Item.buyPrice(0, 15, 0, 0);
            item.expert = true;
            item.rare = 9;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.aBulwark = true;
            player.buffImmune[ModContent.BuffType<AstralInfectionDebuff>()] = true;
        }
    }
}

using CalamityMod.Buffs.StatDebuffs;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Items.Accessories
{
    public class LeadCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lead Core");
            Tooltip.SetDefault("Grants immunity to the irradiated debuff");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.rare = 5;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<Irradiated>()] = true;
        }
    }
}

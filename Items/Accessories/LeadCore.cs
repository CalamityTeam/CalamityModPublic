using CalamityMod.Buffs.StatDebuffs;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

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
            Item.width = 26;
            Item.height = 26;
            Item.rare = ItemRarityID.Pink;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<Irradiated>()] = true;
        }
    }
}

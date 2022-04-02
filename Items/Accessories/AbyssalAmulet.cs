using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Neck)]
    public class AbyssalAmulet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abyssal Amulet");
            Tooltip.SetDefault("Attacks inflict the Crush Depth debuff\n" +
                "Grants immunity to the Crush Depth debuff\n" +
                "While in the abyss you gain 10% increased max life");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Orange;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.abyssalAmulet = true;
            player.buffImmune[ModContent.BuffType<CrushDepth>()] = true;
        }
    }
}

using CalamityMod.Buffs.StatDebuffs;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class NuclearRod : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Nuclear Rod");
            Tooltip.SetDefault("Minions release an irradiated aura on enemy hits\n" +
                               "+1 max minion\n" +
                               "Grants immunity to Irradiated");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().nuclearRod = true;
            player.buffImmune[ModContent.BuffType<Irradiated>()] = true;
        }
    }
}

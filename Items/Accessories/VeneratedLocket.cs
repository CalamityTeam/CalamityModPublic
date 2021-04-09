
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class VeneratedLocket : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Venerated Locket");
            Tooltip.SetDefault("10% increased rogue damage\n" +
                "Using a rogue weapon summons a copy of the projectile that falls from the sky\n" +
                "Stealth strikes cause a circular fan of seeking cosmilite knives to be thrown\n" +
                "You'll never be alone, no matter where you go");
        }

        public override void SetDefaults()
        {
            item.width = 34;
            item.height = 36;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().throwingDamage += 0.1f;
            player.Calamity().veneratedLocket = true;
        }
    }
}

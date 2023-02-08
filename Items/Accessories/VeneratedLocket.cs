using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class VeneratedLocket : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Venerated Locket");
            Tooltip.SetDefault("10% increased rogue damage\n" +
                "Using a rogue weapon summons a copy of the projectile that falls from the sky\n" +
                "Stealth strikes cause a circular fan of seeking cosmilite knives to be thrown\n" +
                "You'll never be alone, no matter where you go");
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 36;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<ThrowingDamageClass>() += 0.10f;
            player.Calamity().veneratedLocket = true;
        }
    }
}

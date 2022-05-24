using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class PsychoticAmulet : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Psychotic Amulet");
            Tooltip.SetDefault("Boosts rogue and ranged damage and critical strike chance by 5%\n" +
                               "Grants a massive boost to these stats if you aren't moving");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.pAmulet = true;
            player.shroomiteStealth = true;
            player.GetDamage<ThrowingDamageClass>() += 0.05f;
            player.GetCritChance<ThrowingDamageClass>() += 5;
            player.GetDamage<RangedDamageClass>() += 0.05f;
            player.GetCritChance<RangedDamageClass>() += 5;
        }
    }
}

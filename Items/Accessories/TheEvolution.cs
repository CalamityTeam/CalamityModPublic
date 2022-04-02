using CalamityMod.CalPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class TheEvolution : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Evolution");
            Tooltip.SetDefault("You reflect projectiles when they hit you\n" +
                                "Reflected projectiles deal no damage to you\n" +
                                "This reflect has a 120 second cooldown which is shared with all other dodges and reflects\n" +
                                "If this effect triggers you get a health regeneration boost for 5 seconds\n" +
                                "If the same enemy projectile type hits you again you will resist its damage by 15%");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 10));
        }

        public override void SetDefaults()
        {
            item.width = 58;
            item.height = 44;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.rare = ItemRarityID.Purple;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.projRefRare = true;
        }
    }
}

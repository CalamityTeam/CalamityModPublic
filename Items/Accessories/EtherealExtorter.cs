using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class EtherealExtorter : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ethereal Extorter");
            Tooltip.SetDefault(@"Rogue projectiles explode into homing souls on death
10% increased rogue damage and +10 maximum stealth, however, life regen is reduced by 1");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.etherealExtorter = true;
            modPlayer.throwingDamage += 0.1f;
            player.lifeRegen -= 1;
            modPlayer.rogueStealthMax += 0.1f;
        }
    }
}

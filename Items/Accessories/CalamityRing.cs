using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class CalamityRing : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Void of Calamity");
            Tooltip.SetDefault("Cursed? Reduces damage reduction by 10%\n" +
            "15% increase to all damage\n" +
            "Brimstone fire rains down while invincibility is active");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 22;
            item.value = CalamityGlobalItem.Rarity7BuyPrice;
            item.rare = ItemRarityID.Lime;
            item.accessory = true;
        }

        public override bool CanEquipAccessory(Player player, int slot) => !player.Calamity().calamityRing;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.calamityRing = true;
            player.allDamage += 0.15f;
            player.endurance -= 0.1f;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.immune)
                {
                    if (player.miscCounter % 10 == 0)
                    {
                        CalamityUtils.ProjectileRain(player.Center, 400f, 100f, 500f, 800f, 22f, ModContent.ProjectileType<StandingFire>(), (int)(30 * player.AverageDamage()), 5f, player.whoAmI);
                    }
                }
            }
        }
    }
}

using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class ThiefsDime : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thief's Dime");
            Tooltip.SetDefault("Those scurvy dogs don’t know the first thing about making bank\n" +
			"Summons a coin that revolves around you and steals money from enemies");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.accessory = true;
        }

        public override bool CanEquipAccessory(Player player, int slot)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.thiefsDime)
            {
                return false;
            }
            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.thiefsDime = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.ownedProjectileCounts[mod.ProjectileType("ThiefsDimeProj")] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, mod.ProjectileType("ThiefsDimeProj"), 100, 6f, Main.myPlayer, 0f, 0f);
                }
            }
        }
    }
}

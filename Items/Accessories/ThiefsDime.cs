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
            return !player.Calamity().thiefsDime;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().thiefsDime = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<ThiefsDimeProj>()] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<ThiefsDimeProj>(), 100, 6f, Main.myPlayer, 0f, 0f);
                }
            }
        }
    }
}

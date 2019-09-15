using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Accessories
{
    public class EyeoftheStorm : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Eye of the Storm");
			Tooltip.SetDefault("Summons a cloud elemental to fight for you");
		}

	    public override void SetDefaults()
	    {
	        item.width = 20;
	        item.height = 26;
            item.value = Item.buyPrice(0, 15, 0, 0);
            item.rare = 5;
	        item.accessory = true;
	    }

        public override bool CanEquipAccessory(Player player, int slot)
        {
            CalamityPlayer modPlayer = player.GetCalamityPlayer();
            if (modPlayer.elementalHeart)
            {
                return false;
            }
            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
		{
	    	CalamityPlayer modPlayer = player.GetCalamityPlayer();
			modPlayer.cloudWaifu = true;
			if (player.whoAmI == Main.myPlayer)
			{
				if (player.FindBuffIndex(mod.BuffType("CloudyWaifu")) == -1)
				{
					player.AddBuff(mod.BuffType("CloudyWaifu"), 3600, true);
				}
				if (player.ownedProjectileCounts[mod.ProjectileType("CloudyWaifu")] < 1)
				{
					Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, mod.ProjectileType("CloudyWaifu"), (int)(45f * player.minionDamage), 2f, Main.myPlayer, 0f, 0f);
				}
			}
		}
	}
}

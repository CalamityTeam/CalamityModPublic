using CalamityMod.Projectiles.Typeless;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Typeless
{
	public class RelicOfDeliverance : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Relic of Deliverance");
            Tooltip.SetDefault("Summons a spear that causes you to lunge towards the mouse position\n" +
                               "The spear requires a charge-up. The longer the charge, the stronger the lunge\n" +
                               "The spear disappears immediately if you are not holding this item while charging\n" +
                               "If enough time has passed or the spear collides into a wall, the spear dies and the lunge ends");
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 46;
            item.damage = 1350;
            item.useTime = item.useAnimation = 25;
            item.reuseDelay = 15;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = SoundID.Item46;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
			item.value = CalamityGlobalItem.Rarity11BuyPrice;
			item.rare = ItemRarityID.Purple;
			item.shoot = ModContent.ProjectileType<RelicOfDeliveranceSpear>();
            item.Calamity().CannotBeEnchanted = true;
        }

		// Terraria seems to really dislike high crit values in SetDefaults
		public override void GetWeaponCrit(Player player, ref int crit) => crit += 16;

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;
    }
}

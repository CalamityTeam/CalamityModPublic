using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
	public class FinalDawn : RogueWeapon
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Final Dawn");
			Tooltip.SetDefault("'We shall ride into the sunrise once more'\n" +
							   "Attack enemies with a giant scythe swing to replenish stealth\n" +
							   "Press up and attack to throw the scythe \n" +
							   "Stealth Strikes allow you to perform a vertical swing that leaves a lingering fire aura\n" +
							   "Stealth Strike performed while pressing up allow you to fling yourself at the enemy and slice through them, causing homing fireballs to emerge");
		}
		public override void SafeSetDefaults()
		{
			item.damage = 5000;
			item.Calamity().rogue = true;
			item.width = 78;
			item.height = 66;
			item.noMelee = true;
			item.useTime = item.useAnimation = 15;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.knockBack = 4;
			item.rare = 10;
			item.Calamity().customRarity = CalamityRarity.Dedicated ;
			item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
			item.autoReuse = false;
			item.shoot = ModContent.ProjectileType<FinalDawnProjectile>();
			item.shootSpeed = 1f;
			item.useTurn = false;
            item.channel = true;
			item.noUseGraphic = true;
		}
		public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] +
			player.ownedProjectileCounts[ModContent.ProjectileType<FinalDawnFireSlash>()] +
			player.ownedProjectileCounts[ModContent.ProjectileType<FinalDawnHorizontalSlash>()] +
			player.ownedProjectileCounts[ModContent.ProjectileType<FinalDawnThrow>()] +
			player.ownedProjectileCounts[ModContent.ProjectileType<FinalDawnThrow2>()] <= 0;
    }
}
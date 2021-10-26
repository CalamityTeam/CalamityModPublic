using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
	public class PhotonRipper : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Photon Ripper");
			Tooltip.SetDefault("Projects a directed stream of hardlight teeth at ultra high velocity\n" +
				"This weapon and its projectiles function as a chainsaw");
		}

		public override void SetDefaults()
		{
			item.height = 134;
			item.width = 54;
			item.damage = 3725;

			// Displayed axe% is 1/5th of axePower here because trees have 500% hardness. This corrects for that.
			item.axe = 6000 / 5;
			item.melee = true;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.channel = true;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.useTime = 5;
			item.useAnimation = 25;
			item.knockBack = 12f;
			item.autoReuse = false;
			item.shoot = ModContent.ProjectileType<PhotonRipperProjectile>();
			item.shootSpeed = 1f;

			item.rare = ItemRarityID.Red;
			item.Calamity().customRarity = CalamityRarity.Violet;
			item.value = CalamityGlobalItem.RarityVioletBuyPrice;
		}

		public override void GetWeaponCrit(Player player, ref int crit) => crit += 18;

		public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f);
			return false;
		}
	}
}

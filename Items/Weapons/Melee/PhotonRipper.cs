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
			Tooltip.SetDefault("Unleashes a flurry of prismatic crystals at extremely fast speeds\n" +
				"");
		}

		public override void SetDefaults()
		{
			item.height = 134;
			item.width = 54;
			item.damage = 2581;
			item.axe = 3330;
			item.axe /= 5;
			item.melee = true;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.channel = true;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.useTime = 5;
			item.useAnimation = 25;
			item.knockBack = 12f;
			item.autoReuse = false;
			item.value = CalamityGlobalItem.RarityVioletBuyPrice;
			item.rare = ItemRarityID.Red;
			item.shoot = ModContent.ProjectileType<PhotonRipperProjectile>();
			item.shootSpeed = 1f;
			item.Calamity().customRarity = CalamityRarity.Violet;
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

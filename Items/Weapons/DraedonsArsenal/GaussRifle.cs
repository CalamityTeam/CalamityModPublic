using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class GaussRifle : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gauss Rifle");
			Tooltip.SetDefault("Fires an enormous pulse of energy");
		}

		public override void SetDefaults()
		{
			item.width = 112;
			item.height = 36;
			item.ranged = true;
			item.damage = 4000;
			item.knockBack = 30f;
			item.useTime = item.useAnimation = 28;
			item.autoReuse = true;

			item.useStyle = ItemUseStyleID.HoldingOut;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/GaussWeaponFire");
			item.noMelee = true;

			item.value = CalamityGlobalItem.RarityVioletBuyPrice;
			item.rare = ItemRarityID.Red;
			item.Calamity().customRarity = CalamityRarity.RareVariant; // In accordance with the other post-ML Arsenal weapons that Fabsol made.

			item.shoot = ModContent.ProjectileType<GaussRifleBlast>();
			item.shootSpeed = 27f;
			item.useAmmo = AmmoID.Bullet;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<GaussRifleBlast>(), damage, knockBack, player.whoAmI, 0f, 0f);
			return false;
		}
	}
}

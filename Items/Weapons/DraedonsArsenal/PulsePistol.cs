using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class PulsePistol : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Pulse Pistol");
			Tooltip.SetDefault("Fires a pulse that arcs to a new target on enemy hits\n" +
							   "Inflicts more damage to inorganic targets");
		}

		public override void SetDefaults()
		{
			item.width = 62;
			item.height = 22;
			item.ranged = true;
			item.damage = 22;
			item.knockBack = 0f;
			item.useTime = item.useAnimation = 20;
			item.autoReuse = true;

			item.useStyle = ItemUseStyleID.HoldingOut;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PulseRifleFire");
			item.noMelee = true;

			item.value = CalamityGlobalItem.Rarity3BuyPrice;
			item.rare = ItemRarityID.Orange;

			item.shoot = ModContent.ProjectileType<PulseRifleShot>();
			item.shootSpeed = 1.2f; // Keep in mind that the shot has extra updates.
			item.useAmmo = AmmoID.Bullet;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<PulsePistolShot>(), damage, knockBack, player.whoAmI, 0f, 0f);

			// Consume 6 ammo per shot
			CalamityGlobalItem.ConsumeAdditionalAmmo(player, item, 6);

			return false;
		}

		public override Vector2? HoldoutOffset() => new Vector2(10f, 0f);

		// Disable vanilla ammo consumption
		public override bool ConsumeAmmo(Player player) => false;
	}
}

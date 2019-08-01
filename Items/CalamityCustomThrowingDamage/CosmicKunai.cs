using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
    public class CosmicKunai : CalamityDamageItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cosmic Kunai");
			Tooltip.SetDefault("Fires a stream of short-range kunai");
		}

		public override void SafeSetDefaults()
		{
			item.width = 26;
			item.damage = 150;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useTime = 2;
			item.useAnimation = 10;
			item.useStyle = 1;
			item.knockBack = 5f;
			item.UseSound = SoundID.Item109;
			item.autoReuse = true;
			item.height = 48;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.shoot = mod.ProjectileType("CosmicKunai");
			item.shootSpeed = 28f;
			item.rare = 9;
			item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 13;
		}

		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
			return false;
		}
	}
}

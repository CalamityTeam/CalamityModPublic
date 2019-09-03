using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
    public class HeavenfallenStardisk : CalamityDamageItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Heavenfallen Stardisk");
			Tooltip.SetDefault("Throws a stardisk upwards which then launches itself towards your mouse cursor,\n" +
							   "explodes into several astral energy bolts if the thrower is moving vertically when throwing it and during its impact");
		}

		public override void SafeSetDefaults()
		{
			item.width = 38;
			item.height = 34;
			item.damage = 165;
			item.crit += 20;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useAnimation = 23;
			item.useStyle = 1;
			item.useTime = 23;
			item.knockBack = 8f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.value = Item.buyPrice(0, 60, 0, 0);
			item.rare = 7;
			item.shoot = mod.ProjectileType("HeavenfallenStardisk");
			item.shootSpeed = 10f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
		}

		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
	    {
			Projectile.NewProjectile(position.X, position.Y, 0f, -10f, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
			return false;
		}
	}
}

using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
    public class Mycoroot : CalamityDamageItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mycoroot");
			Tooltip.SetDefault("Fires a stream of short-range fungal roots");
		}

		public override void SafeSetDefaults()
		{
			item.width = 32;
			item.damage = 10;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useTime = 5;
			item.useAnimation = 5;
			item.useStyle = 1;
			item.knockBack = 1.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 32;
			item.rare = 2;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.shoot = mod.ProjectileType("Mycoroot");
			item.shootSpeed = 20f;
			item.Calamity().rogue = true;
		}

		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
		    float SpeedX = speedX + (float) Main.rand.Next(-30, 31) * 0.05f;
		    float SpeedY = speedY + (float) Main.rand.Next(-30, 31) * 0.05f;
		    Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
		    return false;
		}
	}
}

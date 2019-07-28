using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.RareVariants
{
    public class LeadWizard : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lead Wizard");
			Tooltip.SetDefault("Something's not right...\n" +
				"33% chance to not consume ammo");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 88;
	        item.ranged = true;
	        item.width = 66;
	        item.height = 34;
	        item.useTime = 2;
	        item.reuseDelay = 10;
	        item.useAnimation = 6;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 5f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
	        item.UseSound = SoundID.Item31;
	        item.autoReuse = true;
	        item.shoot = 10;
	        item.shootSpeed = 20f;
	        item.useAmmo = 97;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 22;
		}

	    public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}

		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			float SpeedX = speedX + (float)Main.rand.Next(-15, 16) * 0.05f;
			float SpeedY = speedY + (float)Main.rand.Next(-15, 16) * 0.05f;
			SpeedX += speedY + (float)Main.rand.Next(-85, 86) * 0.05f;
			SpeedY += speedX + (float)Main.rand.Next(-85, 86) * 0.05f;
			Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, 242, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
			return false;
		}

	    public override bool ConsumeAmmo(Player player)
	    {
	    	if (Main.rand.Next(0, 100) < 33)
	    		return false;
	    	return true;
	    }
	}
}

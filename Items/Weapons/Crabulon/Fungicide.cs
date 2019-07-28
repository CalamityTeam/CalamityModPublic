using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Crabulon
{
    public class Fungicide : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fungicide");
			Tooltip.SetDefault("Shoots fungal rounds that split on death");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 11;
	        item.ranged = true;
	        item.width = 40;
	        item.height = 26;
	        item.useTime = 20;
	        item.useAnimation = 20;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 2.5f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
	        item.UseSound = SoundID.Item61;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("FungiOrb");
	        item.shootSpeed = 14f;
	        item.useAmmo = 97;
	    }

	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("FungiOrb"), damage, knockBack, player.whoAmI, 0f, 0f);
	    	return false;
		}
	}
}

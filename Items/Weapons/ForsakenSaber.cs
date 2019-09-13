using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class ForsakenSaber : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Forsaken Saber");
			Tooltip.SetDefault("Shoots a sand blade that alters its velocity as it travels");
		}

		public override void SetDefaults()
		{
			item.width = 54;
			item.damage = 65;
			item.melee = true;
			item.useAnimation = 18;
			item.useStyle = 1;
			item.useTime = 18;
			item.useTurn = true;
			item.knockBack = 6;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 52;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
			item.shoot = mod.ProjectileType("SandBlade");
			item.shootSpeed = 5f;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.AncientBattleArmorMaterial, 2);
			recipe.AddIngredient(ItemID.AdamantiteBar, 5);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	        recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.AncientBattleArmorMaterial, 2);
			recipe.AddIngredient(ItemID.TitaniumBar, 5);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}

	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.NextBool(3))
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 159);
	        }
	    }
	}
}

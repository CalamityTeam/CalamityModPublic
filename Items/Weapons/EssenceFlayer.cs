using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class EssenceFlayer : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Essence Flayer");
			Tooltip.SetDefault("Shoots an essence scythe that generates healing spirits on enemy kills");
		}

		public override void SetDefaults()
		{
			item.width = 60;
			item.damage = 450;
			item.melee = true;
			item.useAnimation = 19;
			item.useStyle = 1;
			item.useTime = 19;
			item.useTurn = true;
			item.knockBack = 8f;
			item.UseSound = SoundID.Item71;
			item.autoReuse = true;
			item.height = 56;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.shoot = mod.ProjectileType("EssenceScythe");
			item.shootSpeed = 21f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 14;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "CosmiliteBar", 11);
			recipe.AddIngredient(null, "NightmareFuel", 5);
			recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "CosmiliteBar", 11);
			recipe.AddIngredient(null, "EndothermicEnergy", 5);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.NextBool(3))
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 173);
	        }
	    }

	    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
	    	target.AddBuff(mod.BuffType("GodSlayerInferno"), 300);
		}
	}
}

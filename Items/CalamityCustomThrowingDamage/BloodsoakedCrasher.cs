using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
    public class BloodsoakedCrasher : CalamityDamageItem //This weapon has been coded by Achilles|Termi|Ben
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bloodsoaked Crasher");
			Tooltip.SetDefault("Slows down when hitting an enemy. Speeds up otherwise\n" +
			"Heals on enemy hits\n" +
			"Stealth strikes spawn homing blood on enemy hits");
		}

		public override void SafeSetDefaults()
		{
			item.width = 66;
			item.damage = 400;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useAnimation = 18;
			item.useStyle = 1;
			item.useTime = 18;
			item.knockBack = 3f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 64;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 13;
			item.shoot = mod.ProjectileType("BloodsoakedCrashax");
			item.shootSpeed = 15f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
		}

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.GetCalamityPlayer().StealthStrikeAvailable()) //setting the stealth strike
            {
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), mod.ProjectileType("BloodsoakedCrashax"), damage, knockBack, player.whoAmI, 0f, 1f);
                return false;
            }
            return true;
		}

	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod); //post-Prov rogue weapon
	        recipe.AddIngredient(mod.ItemType("CrushsawCrasher"), 1);
	        recipe.AddIngredient(mod.ItemType("BloodstoneCore"), 12);
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class Omniblade : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Omniblade");
			Tooltip.SetDefault("An ancient blade forged by the legendary Omnir");
		}

		public override void SetDefaults()
		{
			item.width = 64;
			item.damage = 63;
			item.crit += 45;
			item.melee = true;
			item.useAnimation = 10;
			item.useStyle = 1;
			item.useTime = 10;
			item.useTurn = true;
			item.knockBack = 6f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 146;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
		}

		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
			if (Main.rand.NextBool(5))
			{
				target.AddBuff(mod.BuffType("WhisperingDeath"), 360);
			}
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Katana);
            recipe.AddIngredient(null, "BarofLife", 20);
			recipe.AddIngredient(null, "CoreofCalamity", 10);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}

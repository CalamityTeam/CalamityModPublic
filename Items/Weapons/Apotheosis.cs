using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
	public class Apotheosis : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Apotheosis");
			Tooltip.SetDefault("Unleashes interdimensional projection magic\n" +
                "Eat worms");
		}

		public override void SetDefaults()
		{
			item.damage = 333;
			item.magic = true;
			item.mana = 42;
			item.width = 16;
			item.height = 16;
			item.useTime = 42;
			item.useAnimation = 42;
			item.useStyle = 5;
			item.useTurn = false;
			item.noMelee = true;
			item.knockBack = 4f;
			item.value = 85000000;
			item.UseSound = SoundID.Item92;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("ApothMark");
            item.shootSpeed = 15;
		}

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(255, 0, 255);
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SubsumingVortex");
            recipe.AddIngredient(null, "CosmicDischarge");
            recipe.AddIngredient(null, "StaffoftheMechworm", 3);
            recipe.AddIngredient(null, "Excelsus", 2);
            recipe.AddIngredient(null, "DarksunFragment", 77);
            recipe.AddIngredient(null, "NightmareFuel", 77);
            recipe.AddIngredient(null, "EndothermicEnergy", 77);
            recipe.AddIngredient(null, "CosmiliteBar", 77);
            recipe.AddIngredient(null, "Phantoplasm", 77);
            recipe.AddIngredient(null, "ShadowspecBar", 5);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
	}
}

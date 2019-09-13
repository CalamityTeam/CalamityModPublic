using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
	public class DemonshadeBreastplate : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demonshade Breastplate");
            Tooltip.SetDefault("20% increased melee speed, 15% increased damage and critical strike chance\n" +
                       "Enemies take ungodly damage when they touch you\n" +
                       "Increased max life and mana by 200\n" +
                       "Standing still lets you absorb the shadows and boost your life regen");
        }

	    public override void SetDefaults()
	    {
	        item.width = 18;
	        item.height = 18;
			item.value = Item.buyPrice(4, 0, 0, 0);
			item.defense = 50;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 16;
		}

	    public override void UpdateEquip(Player player)
	    {
	    	CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
	    	modPlayer.shadeRegen = true;
	    	player.thorns = 100f;
	    	player.statLifeMax2 += 200;
	        player.statManaMax2 += 200;
			player.allDamage += 0.15f;
			modPlayer.AllCritBoost(15);
			player.meleeSpeed += 0.2f;
	    }

	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "ShadowspecBar", 15);
	        recipe.AddTile(null, "DraedonsForge");
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}

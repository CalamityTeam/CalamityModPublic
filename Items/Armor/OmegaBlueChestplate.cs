using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class OmegaBlueChestplate : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Omega Blue Chestplate");
            Tooltip.SetDefault(@"12% increased damage and 8% increased critical strike chance
Your attacks inflict Crush Depth
No positive life regen");
		}

		public override void SetDefaults()
		{
            item.width = 18;
            item.height = 18;
			item.value = Item.sellPrice(0, 38, 0, 0);
			item.rare = 13;
			item.defense = 28;
		}

        public override void UpdateEquip(Player player)
        {
            const float damageUp = 0.12f;
            const int critUp = 8;
            player.meleeDamage += damageUp;
            player.rangedDamage += damageUp;
            player.magicDamage += damageUp;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += damageUp;
            player.minionDamage += damageUp;
            player.meleeCrit += critUp;
            player.rangedCrit += critUp;
            player.magicCrit += critUp;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += critUp;

            //insert enable crush depth, disable positive life regen
            player.GetModPlayer<CalamityPlayer>().omegaBlueChestplate = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType("ReaperTooth"), 16);
            recipe.AddIngredient(mod.ItemType("Lumenite"), 8);
            recipe.AddIngredient(mod.ItemType("Tenebris"), 8);
            recipe.AddIngredient(mod.ItemType("RuinousSoul"), 4);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class MolluskShelleggings : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mollusk Shelleggings");
            Tooltip.SetDefault("5% increased damage and 4% increased critical strike chance\n" +
							   "5% decreased movement speed");
		}

		public override void SetDefaults()
		{
            item.width = 22;
            item.height = 18;
			item.value = Item.buyPrice(0, 15, 0, 0);
			item.rare = 5;
			item.defense = 15;
		}

        public override void UpdateEquip(Player player)
        {
            const float damageUp = 0.12f;
            const int critUp = 4;
            player.meleeDamage += damageUp;
            player.rangedDamage += damageUp;
            player.magicDamage += damageUp;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += damageUp;
            player.minionDamage += damageUp;
            player.meleeCrit += critUp;
            player.rangedCrit += critUp;
            player.magicCrit += critUp;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += critUp;
			player.moveSpeed -= 0.07f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType("SeaPrism"), 20);
            recipe.AddIngredient(mod.ItemType("MolluskHusk"), 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
	}
}

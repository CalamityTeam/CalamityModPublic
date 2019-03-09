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
    public class MolluskShellplate : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mollusk Shellplate");
            Tooltip.SetDefault("10% increased damage and 6% increased critical strike chance\n" +
							   "15% decreased movement speed");
		}

		public override void SetDefaults()
		{
            item.width = 30;
            item.height = 22;
			item.value = Item.buyPrice(0, 20, 0, 0);
			item.rare = 5;
			item.defense = 22;
		}

        public override void UpdateEquip(Player player)
        {
            const float damageUp = 0.1f;
            const int critUp = 6;
            player.meleeDamage += damageUp;
            player.rangedDamage += damageUp;
            player.magicDamage += damageUp;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += damageUp;
            player.minionDamage += damageUp;
            player.meleeCrit += critUp;
            player.rangedCrit += critUp;
            player.magicCrit += critUp;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += critUp;
			 player.moveSpeed -= 0.15f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType("SeaPrism"), 25);
            recipe.AddIngredient(mod.ItemType("MolluskHusk"), 15);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

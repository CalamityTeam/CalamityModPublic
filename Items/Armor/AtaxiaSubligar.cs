using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Armor;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class AtaxiaSubligar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ataxia Subligar");
            Tooltip.SetDefault("3% increased critical strike chance\n" +
                "15% increased movement speed");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
			item.value = Item.buyPrice(0, 18, 0, 0);
			item.rare = 8;
            item.defense = 14;
        }

        public override void UpdateEquip(Player player)
        {
            player.meleeCrit += 3;
            player.magicCrit += 3;
            player.rangedCrit += 3;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 3;
            player.moveSpeed += 0.15f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CruptixBar", 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
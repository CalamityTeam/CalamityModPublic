using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class AtaxiaArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ataxia Armor");
            Tooltip.SetDefault("+20 max life\n" +
                "8% increased damage and 4% increased critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
			item.value = Item.buyPrice(0, 24, 0, 0);
			item.rare = 8;
            item.defense = 20;
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 20;
            player.meleeCrit += 4;
            player.meleeDamage += 0.08f;
            player.magicCrit += 4;
            player.magicDamage += 0.08f;
            player.rangedCrit += 4;
            player.rangedDamage += 0.08f;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 4;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.08f;
            player.minionDamage += 0.08f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CruptixBar", 15);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Armor;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class ReaverScaleMail : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaver Scale Mail");
            Tooltip.SetDefault("9% increased damage and 4% increased critical strike chance\nProvides life regeneration\n+20 max life");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.lifeRegen = 1;
            item.value = 300000;
            item.rare = 6;
            item.defense = 19;
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 20;
            player.meleeCrit += 4;
            player.meleeDamage += 0.09f;
            player.magicCrit += 4;
            player.magicDamage += 0.09f;
            player.rangedCrit += 4;
            player.rangedDamage += 0.09f;
            player.thrownCrit += 4;
            player.thrownDamage += 0.09f;
            player.minionDamage += 0.09f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "DraedonBar", 15);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
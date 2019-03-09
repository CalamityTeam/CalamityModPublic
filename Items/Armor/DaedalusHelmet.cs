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
    [AutoloadEquip(EquipType.Head)]
    public class DaedalusHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daedalus Headgear");
            Tooltip.SetDefault("10% increased ranged damage and critical strike chance, reduces ammo cost by 20%\n" +
                "Immune to Cursed and gives control over gravity");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
			item.value = Item.buyPrice(0, 25, 0, 0);
			item.rare = 5;
            item.defense = 9; //39
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("DaedalusBreastplate") && legs.type == mod.ItemType("DaedalusLeggings");
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "5% increased ranged damage\n" +
                "Getting hit causes you to emit a blast of crystal shards";
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.daedalusShard = true;
            player.rangedDamage += 0.05f;
        }

        public override void UpdateEquip(Player player)
        {
            player.ammoCost80 = true;
            player.rangedDamage += 0.1f;
            player.rangedCrit += 10;
            player.AddBuff(BuffID.Gravitation, 2);
            player.buffImmune[BuffID.Cursed] = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "VerstaltiteBar", 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class AtaxiaHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ataxia Hood");
            Tooltip.SetDefault("12% increased throwing damage and 10% increased throwing critical strike chance\n" +
                "50% chance to not consume thrown items\n" +
                "Immune to lava and fire damage");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 450000;
            item.rare = 8;
            item.defense = 12; //49
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("AtaxiaArmor") && legs.type == mod.ItemType("AtaxiaSubligar");
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "5% increased throwing damage and critical strike chance\n" +
                "Inferno effect when below 50% life\n" +
                "Throwing weapons have a 10% chance to unleash a volley of chaos flames around the player that chase enemies when used\n" +
                "You have a 20% chance to emit a blazing explosion when you are hit";
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.ataxiaBlaze = true;
            modPlayer.ataxiaVolley = true;
            if (player.statLife <= (player.statLifeMax2 * 0.5f))
            {
                player.AddBuff(BuffID.Inferno, 2);
            }
            player.thrownDamage += 0.05f;
            player.thrownCrit += 5;
        }

        public override void UpdateEquip(Player player)
        {
            player.thrownCost50 = true;
            player.thrownDamage += 0.12f;
            player.thrownCrit += 10;
            player.lavaImmune = true;
            player.buffImmune[BuffID.OnFire] = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CruptixBar", 7);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
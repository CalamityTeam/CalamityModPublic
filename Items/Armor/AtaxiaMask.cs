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
    public class AtaxiaMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ataxia Mask");
            Tooltip.SetDefault("12% increased magic damage, reduces mana usage by 10%, and 10% increased magic critical strike chance\n" +
                "+100 Max Mana\n" +
                "Immune to lava and fire damage");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 450000;
            item.rare = 8;
            item.defense = 9; //45
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
            player.setBonus = "5% increased magic damage and critical strike chance\n" +
                "Inferno effect when below 50% life\n" +
                "Magic attacks summon damaging and healing flare orbs on hit\n" +
                "You have a 20% chance to emit a blazing explosion when you are hit";
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.ataxiaBlaze = true;
            modPlayer.ataxiaMage = true;
            if (player.statLife <= (player.statLifeMax2 * 0.5f))
            {
                player.AddBuff(BuffID.Inferno, 2);
            }
            player.magicDamage += 0.05f;
            player.magicCrit += 5;
        }

        public override void UpdateEquip(Player player)
        {
            player.manaCost *= 0.9f;
            player.statManaMax2 += 100;
            player.magicDamage += 0.12f;
            player.magicCrit += 10;
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
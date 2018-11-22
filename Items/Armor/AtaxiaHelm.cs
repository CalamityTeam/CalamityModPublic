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
    public class AtaxiaHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ataxia Helm");
            Tooltip.SetDefault("12% increased melee damage and 10% increased melee critical strike chance\n" +
                "Melee attacks inflict on fire\n" +
                "Immune to lava and fire damage");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 450000;
            item.rare = 8;
            item.defense = 25; //67
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
            player.setBonus = "5% increased melee damage and critical strike chance\n" +
                "Inferno effect when below 50% life\n" +
                "Melee attacks and projectiles cause chaos flames to erupt on enemy hits\n" +
                "You have a 20% chance to emit a blazing explosion when you are hit";
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.ataxiaBlaze = true;
            modPlayer.ataxiaGeyser = true;
            if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
            {
                player.AddBuff(BuffID.Inferno, 2);
            }
            player.meleeDamage += 0.05f;
            player.meleeCrit += 5;
        }

        public override void UpdateEquip(Player player)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.ataxiaFire = true;
            player.meleeDamage += 0.12f;
            player.meleeCrit += 10;
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
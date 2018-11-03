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
    public class XerocMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Xeroc Mask");
            Tooltip.SetDefault("12% increased damage and critical strike chance and +2 max minions\n" +
                "Immune to lava, cursed, fire, cursed inferno, and chilled\n" +
                "Wrath of the cosmos");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 500000;
            item.rare = 9;
            item.defense = 20; //71
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("XerocPlateMail") && legs.type == mod.ItemType("XerocCuisses");
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.xerocSet = true;
            player.setBonus = "7% increased damage and critical strike chance\n" +
                "All projectile types have special effects on enemy hits\n" +
                "Imbued with cosmic wrath and rage when you are damaged";
            if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
            {
                player.AddBuff(BuffID.Wrath, 2);
                player.AddBuff(BuffID.Rage, 2);
            }
            player.meleeDamage += 0.07f;
            player.meleeCrit += 7;
            player.rangedDamage += 0.07f;
            player.rangedCrit += 7;
            player.magicDamage += 0.07f;
            player.magicCrit += 7;
            player.thrownDamage += 0.07f;
            player.thrownCrit += 7;
            player.minionDamage += 0.07f;
        }

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 3;
            player.meleeDamage += 0.12f;
            player.meleeCrit += 12;
            player.rangedDamage += 0.12f;
            player.rangedCrit += 12;
            player.magicDamage += 0.12f;
            player.magicCrit += 12;
            player.thrownDamage += 0.12f;
            player.thrownCrit += 12;
            player.minionDamage += 0.12f;
            player.lavaImmune = true;
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[BuffID.CursedInferno] = true;
            player.buffImmune[BuffID.Cursed] = true;
            player.buffImmune[BuffID.Chilled] = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "MeldiateBar", 12);
            recipe.AddIngredient(ItemID.LunarBar, 8);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
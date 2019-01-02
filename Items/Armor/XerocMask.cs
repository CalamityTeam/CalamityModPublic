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
    [AutoloadEquip(EquipType.Head)]
    public class XerocMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Xeroc Mask");
            Tooltip.SetDefault("11% increased rogue damage and critical strike chance\n" +
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
            player.setBonus = "9% increased rogue damage and velocity\n" +
                "All projectile types have special effects on enemy hits\n" +
                "Imbued with cosmic wrath and rage when you are damaged";
            if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
            {
                player.AddBuff(BuffID.Wrath, 2);
                player.AddBuff(BuffID.Rage, 2);
            }
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.09f;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingVelocity += 0.09f;
        }

        public override void UpdateEquip(Player player)
        {
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.11f;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 11;
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
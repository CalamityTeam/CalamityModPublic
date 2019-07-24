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
    [AutoloadEquip(EquipType.Head)]
    public class ReaverCap : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaver Headgear");
            Tooltip.SetDefault("15% increased rogue damage, 5% increased rogue velocity and critical strike chance\n" +
                "20% increased movement speed and can move freely through liquids");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
			item.value = Item.buyPrice(0, 30, 0, 0);
			item.rare = 7;
            item.defense = 10; //43
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("ReaverScaleMail") && legs.type == mod.ItemType("ReaverCuisses");
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.reaverSpore = true;
			modPlayer.rogueStealthMax = 1.15f;
			player.setBonus = "5% increased rogue damage\n" +
                "You emit a cloud of spores when you are hit\n" +
				"Rogue stealth builds while not attacking and not moving, up to a max of 115\n" +
				"Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
				"The higher your rogue stealth the higher your rogue damage, crit, and movement speed";
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.05f;
        }

        public override void UpdateEquip(Player player)
        {
            player.ignoreWater = true;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.15f;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 5;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingVelocity += 0.05f;
            player.moveSpeed += 0.2f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "DraedonBar", 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
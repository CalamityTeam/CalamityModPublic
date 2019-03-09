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
    public class ReaverMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaver Mask");
            Tooltip.SetDefault("10% increased magic damage, 5% reduced mana cost, and 5% increased magic critical strike chance\n" +
                "10% increased movement speed and can move freely through liquids");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
			item.value = Item.buyPrice(0, 30, 0, 0);
			item.rare = 7;
            item.defense = 7; //40
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
            modPlayer.reaverBurst = true;
            player.setBonus = "5% increased magic damage\n" +
                "Your magic projectiles emit a burst of spore gas on enemy hits\n" +
                "Rage activates when you are damaged";
            player.magicDamage += 0.05f;
        }

        public override void UpdateEquip(Player player)
        {
            player.ignoreWater = true;
            player.magicDamage += 0.1f;
            player.magicCrit += 5;
            player.manaCost *= 0.95f;
            player.moveSpeed += 0.1f;
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
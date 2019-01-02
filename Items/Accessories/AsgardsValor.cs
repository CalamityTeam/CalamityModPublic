using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class AsgardsValor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Asgard's Valor");
            Tooltip.SetDefault("Grants immunity to fire blocks and knockback\n" +
                "Immune to most debuffs including Brimstone Flames, Holy Flames, and Glacial State\n" +
                "10% damage reduction while submerged in liquid\n" +
                "Increased defense by 10 when below 25% life\n" +
                "+20 max life\n" +
                "Grants an improved holy dash\n" +
                "Can be used to ram enemies\n" +
                "Toggle visibility of this accessory to enable/disable the dash");
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 44;
            item.value = Item.buyPrice(0, 45, 0, 0);
            item.rare = 9;
            item.defense = 5;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            if (!hideVisual) { modPlayer.dashMod = 2; }
            player.buffImmune[46] = true;
            player.buffImmune[44] = true;
            player.noKnockback = true;
            player.fireWalk = true;
            player.buffImmune[33] = true;
            player.buffImmune[36] = true;
            player.buffImmune[30] = true;
            player.buffImmune[20] = true;
            player.buffImmune[32] = true;
            player.buffImmune[31] = true;
            player.buffImmune[35] = true;
            player.buffImmune[23] = true;
            player.buffImmune[22] = true;
            player.buffImmune[mod.BuffType("BrimstoneFlames")] = true;
            player.buffImmune[mod.BuffType("HolyLight")] = true;
            player.buffImmune[mod.BuffType("GlacialState")] = true;
            player.statLifeMax2 += 20;
            if (player.statLife <= (int)((double)player.statLifeMax2 * 0.25)) { player.statDefense += 10; }
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir)) { player.endurance += 0.1f; }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.AnkhShield);
            recipe.AddIngredient(null, "OrnateShield");
            recipe.AddIngredient(null, "ShieldoftheOcean");
            recipe.AddIngredient(null, "Abaddon");
            recipe.AddIngredient(null, "CoreofEleum", 3);
            recipe.AddIngredient(null, "CoreofCinder", 3);
            recipe.AddIngredient(null, "CoreofChaos", 3);
            recipe.AddIngredient(ItemID.LifeFruit, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
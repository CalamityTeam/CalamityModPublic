using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.GreatSandShark
{
    public class ShiftingSands : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shifting Sands");
            Tooltip.SetDefault("Casts a sand shard that follows the mouse cursor");
        }

        public override void SetDefaults()
        {
            item.damage = 90;
            item.magic = true;
            item.channel = true;
            item.mana = 20;
            item.width = 58;
            item.height = 58;
            item.useTime = 18;
            item.useAnimation = 18;
            item.useStyle = 1;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = 100000;
            item.rare = 7;
            item.UseSound = SoundID.Item20;
            item.shoot = mod.ProjectileType("ShiftingSands");
            item.shootSpeed = 7f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.MagicMissile);
            recipe.AddIngredient(null, "GrandScale");
            recipe.AddIngredient(ItemID.AncientBattleArmorMaterial, 2);
            recipe.AddIngredient(ItemID.SpectreBar, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
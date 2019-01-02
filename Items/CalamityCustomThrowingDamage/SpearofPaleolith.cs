using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
    public class SpearofPaleolith : CalamityDamageItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spear of Paleolith");
            Tooltip.SetDefault("Throws an ancient spear that shatters enemy armor at high velocity\nSpears rain fossil shards as they travel");
        }

        public override void SafeSetDefaults()
        {
            item.width = 54;
            item.damage = 50;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 27;
            item.useStyle = 1;
            item.useTime = 27;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 54;
            item.maxStack = 1;
            item.value = 330000;
            item.rare = 5;
            item.shoot = mod.ProjectileType("SpearofPaleolith");
            item.shootSpeed = 35f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.AncientBattleArmorMaterial, 2);
            recipe.AddIngredient(ItemID.AdamantiteBar, 4);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.AncientBattleArmorMaterial, 2);
            recipe.AddIngredient(ItemID.TitaniumBar, 4);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

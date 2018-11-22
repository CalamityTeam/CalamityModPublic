using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items
{
    public class GallantPickaxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gallant Pickaxe");
            Tooltip.SetDefault("Can mine Uelibloom Ore");
        }

        public override void SetDefaults()
        {
            item.damage = 80;
            item.melee = true;
            item.width = 54;
            item.height = 54;
            item.useTime = 6;
            item.useAnimation = 12;
            item.useTurn = true;
            item.pick = 250;
            item.useStyle = 1;
            item.knockBack = 6f;
            item.value = 200000;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.tileBoost += 5;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(0, 255, 200);
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "MeldiateBar", 9);
            recipe.AddIngredient(null, "GalacticaSingularity");
            recipe.AddRecipeGroup("LunarPickaxe");
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.Next(5) == 0)
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 58);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, 500);
        }
    }
}
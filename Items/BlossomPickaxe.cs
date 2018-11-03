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
    public class BlossomPickaxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blossom Pickaxe");
        }

        public override void SetDefaults()
        {
            item.damage = 90;
            item.melee = true;
            item.width = 52;
            item.height = 56;
            item.useTime = 5;
            item.useAnimation = 10;
            item.useTurn = true;
            item.pick = 275;
            item.useStyle = 1;
            item.knockBack = 6.5f;
            item.value = 300000;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.tileBoost += 6;
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
            recipe.AddIngredient(null, "UeliaceBar", 7);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.Next(5) == 0)
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 75);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 200);
            target.AddBuff(BuffID.CursedInferno, 200);
        }
    }
}
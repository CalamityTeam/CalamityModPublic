using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class TemporalFloeSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Temporal Floe Sword");
            Tooltip.SetDefault("The iceman cometh...");
        }

        public override void SetDefaults()
        {
            item.width = 50;
            item.damage = 85;
            item.melee = true;
            item.useAnimation = 16;
            item.useStyle = 1;
            item.useTime = 16;
            item.useTurn = true;
            item.knockBack = 6;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 58;
            item.maxStack = 1;
            item.value = 1500000;
            item.rare = 8;
            item.shoot = mod.ProjectileType("TemporalFloeSwordProjectile");
            item.shootSpeed = 16f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CryoBar", 15);
            recipe.AddIngredient(ItemID.Ectoplasm, 5);
            recipe.AddTile(TileID.IceMachine);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.Next(3) == 0)
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 34);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.Next(3) == 0)
            {
                target.AddBuff(mod.BuffType("GlacialState"), 120);
            }
            target.AddBuff(BuffID.Frostburn, 600);
        }
    }
}

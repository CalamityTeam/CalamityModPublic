using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class TerraEdge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terra Edge");
            Tooltip.SetDefault("Heals the player on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 58;
            item.damage = 150;
            item.melee = true;
            item.useAnimation = 17;
            item.useStyle = 1;
            item.useTime = 17;
            item.useTurn = true;
            item.knockBack = 6.25f;
            item.UseSound = SoundID.Item60;
            item.autoReuse = true;
            item.height = 58;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.shoot = mod.ProjectileType("TerraEdgeBeam");
            item.shootSpeed = 12f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "TrueBloodyEdge");
            recipe.AddIngredient(ItemID.TrueExcalibur);
            recipe.AddIngredient(null, "LivingShard", 7);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.TrueNightsEdge);
            recipe.AddIngredient(ItemID.TrueExcalibur);
            recipe.AddIngredient(null, "LivingShard", 7);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.Next(5) == 0)
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 74);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (target.type == NPCID.TargetDummy)
            {
                return;
            }
            int healAmount = (Main.rand.Next(2) + 2);
            player.statLife += healAmount;
            player.HealEffect(healAmount);
        }
    }
}

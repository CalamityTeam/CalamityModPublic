using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class TyrantYharimsUltisword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tyrant Yharim's Ultisword");
            Tooltip.SetDefault("Necrotic blade of Jungle King Yharim\n" +
                "50% chance to give the player the tyrant's fury buff on enemy hits\n" +
                "This buff increases melee damage by 30% and melee crit chance by 10%");
        }

        public override void SetDefaults()
        {
            item.width = 88;
            item.damage = 64;
            item.melee = true;
            item.useAnimation = 26;
            item.useStyle = 1;
            item.useTime = 26;
            item.useTurn = true;
            item.knockBack = 5.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 88;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.shoot = mod.ProjectileType("BlazingPhantomBlade");
            item.shootSpeed = 12f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "TrueCausticEdge");
            recipe.AddIngredient(ItemID.BrokenHeroSword);
            recipe.AddIngredient(ItemID.FlaskofVenom, 5);
            recipe.AddIngredient(ItemID.FlaskofCursedFlames, 5);
            recipe.AddIngredient(ItemID.ChlorophyteBar, 15);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "TrueCausticEdge");
            recipe.AddIngredient(ItemID.BrokenHeroSword);
            recipe.AddIngredient(ItemID.FlaskofVenom, 5);
            recipe.AddIngredient(ItemID.FlaskofIchor, 5);
            recipe.AddIngredient(ItemID.ChlorophyteBar, 15);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.Next(5) == 0)
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 106);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.Next(2) == 0)
            {
                player.AddBuff(mod.BuffType("TyrantsFury"), 180);
            }
            target.AddBuff(BuffID.OnFire, 300);
            target.AddBuff(BuffID.Venom, 240);
            target.AddBuff(BuffID.CursedInferno, 180);
        }
    }
}

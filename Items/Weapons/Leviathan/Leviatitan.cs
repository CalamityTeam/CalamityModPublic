using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Leviathan
{
    public class Leviatitan : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Leviatitan");
        }

        public override void SetDefaults()
        {
            item.damage = 93;
            item.ranged = true;
            item.width = 82;
            item.height = 28;
            item.useTime = 9;
            item.useAnimation = 9;
            item.useStyle = 5;
            item.noMelee = true; //so the item's animation doesn't do damage
            item.knockBack = 5f;
            item.value = 750000;
            item.rare = 7;
            item.UseSound = SoundID.Item92;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("AquaBlast"); //idk why but all the guns in the vanilla source have this
            item.shootSpeed = 18f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "IOU");
            recipe.AddIngredient(null, "LivingShard");
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float SpeedX = speedX + (float)Main.rand.Next(-20, 21) * 0.05f;
            float SpeedY = speedY + (float)Main.rand.Next(-20, 21) * 0.05f;
            if (Main.rand.Next(3) == 0)
            {
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, mod.ProjectileType("AquaBlastToxic"), (int)((double)damage * 1.5f), knockBack, player.whoAmI, 0.0f, 0.0f);
            }
            else
            {
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, mod.ProjectileType("AquaBlast"), (int)((double)damage), knockBack, player.whoAmI, 0.0f, 0.0f);
            }
            return false;
        }
    }
}
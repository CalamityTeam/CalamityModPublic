using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.NPCs;

namespace CalamityMod.Items.Weapons.Cryogen
{
    public class GlacialCrusher : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glacial Crusher");
			Tooltip.SetDefault("Fires very slow frost projectiles that gain strength as they travel and freeze enemies\n" +
				"Enemies are frozen for longer the further the projectile travels\n" +
				"True melee strikes cause tremendous damage to frozen enemies\n" +
				"Enemies that cannot be frozen take increased damage");
		}

        public override void SetDefaults()
        {
            item.width = 60;
            item.damage = 59;
            item.melee = true;
            item.useAnimation = 27;
            item.useTime = 27;
            item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 58;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.shoot = mod.ProjectileType("Iceberg");
            item.shootSpeed = 3f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CryoBar", 10);
            recipe.AddTile(TileID.IceMachine);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.Next(3) == 0)
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 67);
            }
        }

		public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
		{
			if (target.buffImmune[mod.BuffType("GlacialState")])
			{
				damage *= 2;
				knockBack *= 2f;
			}
			else if (target.GetGlobalNPC<CalamityGlobalNPC>(mod).gState)
			{
				damage *= 3;
				knockBack *= 3f;
			}
		}
    }
}

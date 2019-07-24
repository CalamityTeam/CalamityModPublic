using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons
{
    public class AethersWhisper : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aether's Whisper");
            Tooltip.SetDefault("Inflicts several long-lasting debuffs and splits on tile hits\n" +
                "Right click to change from magic to ranged damage");
        }

        public override void SetDefaults()
        {
            item.damage = 1050;
            item.magic = true;
            item.mana = 30;
            item.width = 118;
            item.height = 38;
            item.useTime = 24;
            item.useAnimation = 24;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 5.5f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon");
            item.autoReuse = true;
            item.shootSpeed = 12f;
            item.shoot = mod.ProjectileType("AetherBeam");
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 13;
		}

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.ranged = true;
                item.magic = false;
                item.mana = 0;
            }
            else
            {
                item.ranged = false;
                item.magic = true;
                item.mana = 30;
            }
            return base.CanUseItem(player);
        }

		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			float ai0 = (player.altFunctionUse == 2 ? 1f : 0f);
			Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, ai0, 0f);
			return false;
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "PlasmaRod");
            recipe.AddIngredient(null, "Zapper");
            recipe.AddIngredient(null, "SpectreRifle");
            recipe.AddIngredient(null, "TwistingNether", 3);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class BloodBath : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Bath");
            Tooltip.SetDefault("Drenches your foes in blood");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 25;
            item.magic = true;
            item.mana = 10;
            item.width = 52;
            item.height = 50;
            item.useTime = 15;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5.75f;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
            item.UseSound = SoundID.Item21;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<BloodBeam>();
            item.shootSpeed = 9f;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(15, 15);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BloodSample>(), 8);
            recipe.AddIngredient(ItemID.Vertebrae, 3);
            recipe.AddIngredient(ItemID.CrimtaneBar, 2);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int bloodDropAmt = 2;
            if (Main.rand.NextBool(3))
            {
                bloodDropAmt++;
            }
            if (Main.rand.NextBool(3))
            {
                bloodDropAmt++;
            }
			CalamityUtils.ProjectileToMouse(player, bloodDropAmt, item.shootSpeed, 0f, 30f, type, damage, knockBack, player.whoAmI, true);
            return false;
        }
    }
}

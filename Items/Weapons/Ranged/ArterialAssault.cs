using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class ArterialAssault : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arterial Assault");
            Tooltip.SetDefault("Fires a chain of 5 arrows from the sky\n" +
                "Wooden arrows are converted into homing bloodfire arrows");
        }

        public override void SetDefaults()
        {
            item.damage = 128;
            item.ranged = true;
            item.width = 44;
            item.height = 100;
            item.useTime = 3;
            item.reuseDelay = 10;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4.25f;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.UseSound = SoundID.Item102;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<BloodfireArrowProj>();
            item.shootSpeed = 30f;
            item.useAmmo = AmmoID.Arrow;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BloodstoneCore>(), 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float num72 = item.shootSpeed;
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
            float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
            if (player.gravDir == -1f)
            {
                num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
            }
            float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
            if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
            {
                num78 = (float)player.direction;
                num79 = 0f;
                num80 = num72;
            }
            else
            {
                num80 = num72 / num80;
            }

            vector2 = new Vector2(player.position.X + (float)player.width * 0.5f + (-(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
            vector2.X = (vector2.X + player.Center.X) / 2f;
            vector2.Y -= 100f;
            num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
            num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
            if (num79 < 0f)
            {
                num79 *= -1f;
            }
            if (num79 < 20f)
            {
                num79 = 20f;
            }
            num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
            num80 = num72 / num80;
            num78 *= num80;
            num79 *= num80;
            float speedX4 = num78;
            float speedY5 = num79;
            if (type == ProjectileID.WoodenArrowFriendly)
            {
                int bloodfire = Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5, ModContent.ProjectileType<BloodfireArrowProj>(), damage, knockBack, player.whoAmI, 0f, 80f);
                Main.projectile[bloodfire].tileCollide = false;
            }
            else
            {
                int num121 = Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5, type, damage, knockBack, player.whoAmI);
                Main.projectile[num121].noDropItem = true;
            }
            return false;
        }
    }
}

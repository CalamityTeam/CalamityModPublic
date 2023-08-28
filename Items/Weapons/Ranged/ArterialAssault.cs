using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class ArterialAssault : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 128;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 44;
            Item.height = 100;
            Item.useTime = 3;
            Item.useAnimation = 15;
            Item.reuseDelay = 10;
            Item.useLimitPerAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4.25f;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.UseSound = SoundID.Item102;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BloodfireArrowProj>();
            Item.shootSpeed = 30f;
            Item.useAmmo = AmmoID.Arrow;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float num72 = Item.shootSpeed;
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
            if (CalamityUtils.CheckWoodenAmmo(type, player))
            {
                int bloodfire = Projectile.NewProjectile(source, vector2.X, vector2.Y, speedX4, speedY5, ModContent.ProjectileType<BloodfireArrowProj>(), damage, knockback, player.whoAmI, 0f, 80f);
                Main.projectile[bloodfire].tileCollide = false;
            }
            else
            {
                int num121 = Projectile.NewProjectile(source, vector2.X, vector2.Y, speedX4, speedY5, type, damage, knockback, player.whoAmI);
                Main.projectile[num121].noDropItem = true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BloodstoneCore>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}

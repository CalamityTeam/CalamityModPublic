using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class DivineRetribution : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Divine Retribution");
            Tooltip.SetDefault("Mage is superior to rogue; look, we got a donor weapon to prove it!");
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 39;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.width = 66;
            Item.height = 88;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;

            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.Calamity().donorItem = true;

            Item.UseSound = SoundID.Item73;
            Item.autoReuse = true;
            Item.shootSpeed = 19f;
            Item.shoot = ModContent.ProjectileType<DivineRetributionSpear>();
        }

        public override Vector2? HoldoutOrigin() => new Vector2(15, 15);

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
            }
            else
            {
                num80 = num72 / num80;
            }
            int numProjectiles = 5;
            for (int i = 0; i < numProjectiles; i++)
            {
                vector2 = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(51) * -(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - /* - */ player.position.X), player.MountedCenter.Y + 600f); //-
                vector2.X = (vector2.X + player.Center.X) / 2f + (float)Main.rand.Next(-50, 51); //200
                vector2.Y += (float)(100 * i); //-=
                num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X; //+ -
                num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y; //+ -
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
                float speedX6 = num78 + (float)Main.rand.Next(-60, 61) * 0.02f;
                float speedY7 = num79 + (float)Main.rand.Next(-60, 61) * 0.02f;
                float ai1 = Main.rand.NextFloat() + 0.5f;
                Projectile.NewProjectile(source, vector2.X, vector2.Y, speedX6, -speedY7, type, damage, knockback, player.whoAmI, 0.0f, ai1);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UndinesRetribution>().
                AddIngredient<DivineGeode>(8).
                AddIngredient<UelibloomBar>(6).
                AddIngredient<UnholyEssence>(10).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}

using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class WyvernsCall : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wyvern's Call");
            Tooltip.SetDefault(@"I call upon the mythical Wyvern to shower the lands with its grace
Fires wyverns and colored feathers from the sky that stick to enemies and tiles and explode");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 35;
            item.magic = true;
            item.mana = 10;
            item.width = 52;
            item.height = 74;
            item.useTime = 23;
            item.useAnimation = 23;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4.75f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.UseSound = SoundID.Item102;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<WyvernFeatherPurple>();
            item.shootSpeed = 18f;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(15, 15);
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

            int num107 = 3;
            for (int num108 = 0; num108 < num107; num108++)
            {
                float damageMult = 1f;
                float kbMult = 1f;
                if (Main.rand.NextBool(10))
                {
                    type = ModContent.ProjectileType<WyvernProjectile>();
                }
                else if (Main.rand.NextBool(3))
                {
                    type = ModContent.ProjectileType<WyvernFeatherGreen>();
                }
                else if (Main.rand.NextBool(2))
                {
                    type = ModContent.ProjectileType<WyvernFeatherPink>();
                }
                if (type == ModContent.ProjectileType<WyvernProjectile>())
                {
                    damageMult = 20f;
                    kbMult = 1.5f;
                }
                if (type == ModContent.ProjectileType<WyvernFeatherPink>())
                {
                    damageMult = 1.2f;
                    kbMult = 2f;
                }
                vector2 = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                vector2.X = (vector2.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
                vector2.Y -= (float)(100 * num108);
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
                float speedX4 = num78 + (float)Main.rand.Next(-30, 31) * 0.02f;
                float speedY5 = num79 + (float)Main.rand.Next(-30, 31) * 0.02f;
                int feather = Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5, type, (int)(damage * damageMult), (int)(knockBack * kbMult), player.whoAmI, 0f, (float)Main.rand.Next(15));
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SkyGlaze>());
            recipe.AddIngredient(ItemID.SoulofFlight, 15);
            recipe.AddRecipeGroup("AnyMythrilBar", 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

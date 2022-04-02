using CalamityMod.Projectiles.Magic;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ClamorNoctus : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Clamor Noctus");
            Tooltip.SetDefault(@"Let the sky fall
Fires night wyverns and stars from the sky that stick to enemies and tiles and explode");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 270;
            item.magic = true;
            item.mana = 12;
            item.width = 52;
            item.height = 74;
            item.useTime = 21;
            item.useAnimation = 21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5.2f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
            item.UseSound = SoundID.Item105;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<ClamorNoctusStar>();
            item.shootSpeed = 25f;
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
                    type = ModContent.ProjectileType<ClamorNoctusWyvern>();
                }
                if (type == ModContent.ProjectileType<ClamorNoctusWyvern>())
                {
                    damageMult = 25f;
                    kbMult = 1.5f;
                }
                vector2 = new Vector2(player.Center.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.Center.X), player.MountedCenter.Y - 600f);
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
                Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5, type, (int)(damage * damageMult), (int)(knockBack * kbMult), player.whoAmI, 0f, (float)Main.rand.Next(15));
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<WyvernsCall>());
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 6);
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 5);
            recipe.AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

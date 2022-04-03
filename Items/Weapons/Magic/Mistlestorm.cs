using CalamityMod.Items.Materials;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Mistlestorm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mistlestorm");
            Tooltip.SetDefault("Casts a storm of pine needles and leaves");
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 5;
            Item.width = 48;
            Item.height = 48;
            Item.useTime = 6;
            Item.useAnimation = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.UseSound = SoundID.Item39;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PineNeedleFriendly;
            Item.shootSpeed = 24f;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(15, 15);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int num106 = 2 + Main.rand.Next(3);
            for (int num107 = 0; num107 < num106; num107++)
            {
                float num110 = 0.025f * (float)num107;
                speedX += (float)Main.rand.Next(-35, 36) * num110;
                speedY += (float)Main.rand.Next(-35, 36) * num110;
                float num84 = (float)Math.Sqrt((double)(speedX * speedX + speedY * speedY));
                num84 = Item.shootSpeed / num84;
                speedX *= num84;
                speedY *= num84;
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, (float)Main.rand.Next(0, 10 * (num107 + 1)), 0f);
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ProjectileID.Leaf, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.Razorpine).AddIngredient(ItemID.LeafBlower).AddIngredient(ModContent.ItemType<UeliaceBar>(), 7).AddIngredient(ModContent.ItemType<DarkPlasma>()).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}

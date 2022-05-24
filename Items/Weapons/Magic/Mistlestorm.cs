using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Mistlestorm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mistlestorm");
            Tooltip.SetDefault("Casts a storm of pine needles and leaves");
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
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

        public override Vector2? HoldoutOrigin() => new Vector2(15, 15);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int num106 = 2 + Main.rand.Next(3);
            for (int num107 = 0; num107 < num106; num107++)
            {
                float num110 = 0.025f * (float)num107;
                velocity.X += (float)Main.rand.Next(-35, 36) * num110;
                velocity.Y += (float)Main.rand.Next(-35, 36) * num110;
                float num84 = velocity.Length();
                num84 = Item.shootSpeed / num84;
                velocity.X *= num84;
                velocity.Y *= num84;
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, (float)Main.rand.Next(0, 10 * (num107 + 1)), 0f);
                Projectile.NewProjectile(source, position, velocity, ProjectileID.Leaf, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Razorpine).
                AddIngredient(ItemID.LeafBlower).
                AddIngredient<UeliaceBar>(7).
                AddIngredient<DarkPlasma>().
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}

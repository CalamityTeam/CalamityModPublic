using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class NuclearFury : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nuclear Fury");
            Tooltip.SetDefault("Casts a torrent of cosmic typhoons");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 114;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 13;
            Item.width = 38;
            Item.height = 40;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item84;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<NuclearFuryProjectile>();
            Item.shootSpeed = 16f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float spread = 45f * 0.0174f;
            double startAngle = Math.Atan2(velocity.X, velocity.Y) - spread / 2;
            double deltaAngle = spread / 8f;
            double offsetAngle;
            int i;
            for (i = 0; i < 4; i++)
            {
                offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                Projectile.NewProjectile(source, position.X, position.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), type, damage, knockback, Main.myPlayer);
                Projectile.NewProjectile(source, position.X, position.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), type, damage, knockback, Main.myPlayer);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.RazorbladeTyphoon).
                AddIngredient<Poseidon>().
                AddIngredient(ItemID.LunarBar, 5).
                AddIngredient(ItemID.SoulofSight, 10).
                AddIngredient(ItemID.UnicornHorn, 5).
                AddIngredient<SeaPrism>(15).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}

using System;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class ResurrectionButterfly : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 46;
            Item.damage = 66; // clueless
            Item.mana = 10;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 1f;

            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.Calamity().donorItem = true;

            Item.UseSound = SoundID.Item44;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PinkButterfly>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 mouseDirection = Vector2.Normalize(Main.MouseWorld - player.Center) * Item.shootSpeed;

            int p = Projectile.NewProjectile(source, Main.MouseWorld, mouseDirection.RotatedBy(MathHelper.PiOver2), ModContent.ProjectileType<PinkButterfly>(), damage, knockback, Main.myPlayer, 0f, 0f);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            p = Projectile.NewProjectile(source, Main.MouseWorld, mouseDirection.RotatedBy(-MathHelper.PiOver2), ModContent.ProjectileType<PurpleButterfly>(), damage, knockback, Main.myPlayer, 0f, 0f);
            Main.projectile[p].originalDamage = Item.damage;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Silk, 40).
                AddIngredient(ItemID.Ectoplasm, 20).
                AddIngredient<LifeAlloy>(5).
                AddIngredient(ItemID.ButterflyDust, 2).
                AddTile(TileID.Loom).
                Register();
        }
    }
}

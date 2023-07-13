using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class VeeringWind : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 28;
            Item.height = 32;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 10f;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.Calamity().donorItem = true;
            Item.UseSound = SoundID.Item66;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<VeeringWindAirWave>();
            Item.shootSpeed = 12f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
                Item.UseSound = SoundID.Item43;
            else
                Item.UseSound = SoundID.Item66;
            return base.CanUseItem(player);
        }

        public override float UseSpeedMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 0.5f;
            return 1f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                type = ModContent.ProjectileType<VeeringWindFrostWave>();
                knockback *= 0.2f;
                velocity *= 0.5f;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int totalProjectiles = 24;
            for (int i = 0; i < totalProjectiles; i++)
            {
                Vector2 waveVelocity = ((MathHelper.TwoPi * i / (float)totalProjectiles) + velocity.ToRotation()).ToRotationVector2() * velocity.Length() * 0.5f;
                Projectile.NewProjectile(source, position, waveVelocity, type, damage, knockback, Main.myPlayer);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FrostBolt>().
                AddIngredient(ItemID.Feather, 3).
                AddIngredient(ItemID.FallenStar, 5).
                AddIngredient(ItemID.Cloud, 10).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}

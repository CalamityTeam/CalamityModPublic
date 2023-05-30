using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class NuclearFury : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Weapons.Magic";
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
            for (int i = 0; i < 8; i++)
            {
                Vector2 ringVelocity = ((MathHelper.TwoPi * i / 8f) + velocity.ToRotation()).ToRotationVector2() * velocity.Length() * 0.5f;
                Projectile.NewProjectile(source, position, ringVelocity, type, damage, knockback, Main.myPlayer);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.RazorbladeTyphoon).
                AddIngredient<Poseidon>().
                AddIngredient(ItemID.LunarBar, 5).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}

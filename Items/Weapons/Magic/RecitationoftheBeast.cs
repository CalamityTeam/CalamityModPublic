using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class RecitationoftheBeast : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetDefaults()
        {
            Item.mana = 17;
            Item.width = 38;
            Item.height = 54;
            Item.damage = 128;
            Item.noMelee = true;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 18;
            Item.knockBack = 8.5f;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.shoot = ModContent.ProjectileType<BeastScythe>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Magic;
            Item.rare = ModContent.RarityType<DarkBlue>();
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 20;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float spread = 60f * 0.0174f;
            double startAngle = Math.Atan2(velocity.X, velocity.Y) - spread / 2;
            double deltaAngle = spread / 6f;
            double offsetAngle;
            int i;
            for (i = 0; i < 3; i++)
            {
                offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                Projectile.NewProjectile(source, player.Center.X, player.Center.Y, (float)(Math.Sin(offsetAngle) * 2f), (float)(Math.Cos(offsetAngle) * 2f), type, damage, knockback, Main.myPlayer, 0f, 0f);
                Projectile.NewProjectile(source, player.Center.X, player.Center.Y, (float)(-Math.Sin(offsetAngle) * 2f), (float)(-Math.Cos(offsetAngle) * 2f), type, damage, knockback, Main.myPlayer, 0f, 0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.DemonScythe).
                AddIngredient<CoreofHavoc>(12).
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<NightmareFuel>(20).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}

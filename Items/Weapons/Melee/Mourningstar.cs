using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Mourningstar : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.damage = 112;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2.5f;
            Item.UseSound = SoundID.Item116;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.shootSpeed = 24f;
            Item.shoot = ModContent.ProjectileType<MourningstarFlail>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float ai3 = (Main.rand.NextFloat() - 0.75f) * 0.7853982f; //0.5
            float ai3X = (Main.rand.NextFloat() - 0.25f) * 0.7853982f; //0.5
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI, 0f, ai3);
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI, 0f, ai3X);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SolarEruption).
                AddIngredient<CoreofHavoc>(6).
                AddIngredient<CoreofSunlight>(6).
                AddIngredient<DivineGeode>(6).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}

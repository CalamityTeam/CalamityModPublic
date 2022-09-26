using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class MineralMortar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mineral Mortar");
            Tooltip.SetDefault("Shoots an onyx bomb that explodes into sand sharks on death");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 58;
            Item.height = 26;
            Item.useTime = 33;
            Item.useAnimation = 33;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = CalamityGlobalItem.Rarity6BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shootSpeed = 14f;
            Item.shoot = ModContent.ProjectileType<OnyxSharkBomb>();
            Item.useAmmo = AmmoID.Rocket;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<OnyxSharkBomb>(), damage, knockback, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyAdamantiteBar", 13).
                AddIngredient(ItemID.AncientBattleArmorMaterial, 2).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

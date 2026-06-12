using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Sounds;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class TyrannysEnd : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.width = 150;
            Item.height = 48;
            Item.damage = 2150;
            Item.knockBack = 9.5f;
            Item.DamageType = DamageClass.Ranged;
            Item.crit = 35;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.shoot = ProjectileID.BulletHighVelocity;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Bullet;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = CommonCalamitySounds.LargeWeaponFireSound;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.Calamity().donorItem = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-28, 0);

        public override void HoldItem(Player player) => player.scope = true;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<RubicoPrime>().
                AddIngredient<AntiMaterielRifle>().
                AddIngredient<AuricBar>(5).
                AddIngredient<LifeAlloy>(3).
                AddTile<CosmicAnvil>().
                Register();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.Bullet)
                type = ModContent.ProjectileType<PiercingBullet>();
        }
    }
}

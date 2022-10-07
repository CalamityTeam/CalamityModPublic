using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Sounds;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    [LegacyName("AMR")]
    public class AntiMaterielRifle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Anti-materiel Rifle");
            Tooltip.SetDefault("Converts musket balls into .50 caliber sniper rounds that rip apart enemy defense and DR\n" +
                "If you crit the target a second swarm of bullets will fire");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 1880;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 154;
            Item.height = 40;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 9.5f;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.UseSound = CommonCalamitySounds.LargeWeaponFireSound;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Bullet;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-30, 0);

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 20;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Shroomer>().
                AddIngredient<CosmiliteBar>(10).
                AddTile<CosmicAnvil>().
                Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (type == ProjectileID.Bullet)
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<AMRShot>(), damage, knockback, player.whoAmI);
            else
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

            return false;
        }
    }
}

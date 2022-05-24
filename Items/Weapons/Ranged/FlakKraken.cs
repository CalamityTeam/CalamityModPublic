using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Sounds;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class FlakKraken : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flak Kraken");
            Tooltip.SetDefault("Fires an energy reticle that becomes more powerful over time");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 54;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 152;
            Item.height = 58;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.reuseDelay = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 0f;
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = CommonCalamitySounds.LaserCannonSound;
            Item.shoot = ModContent.ProjectileType<FlakKrakenGun>();
            Item.shootSpeed = 30f; //30
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<FlakKrakenGun>(), damage, knockback, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FlakToxicannon>().
                AddIngredient<DepthCells>(20).
                AddIngredient<Lumenite>(10).
                AddIngredient<Tenebris>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    // TODO -- This weapon is a disgrace to its Armored Core heritage. It needs a full rework.
    public class Karasawa : ModItem
    {
        public static readonly SoundStyle FireSound = new("CalamityMod/Sounds/Item/MechGaussRifle");

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Karasawa");
            Tooltip.SetDefault("...This is heavy... too heavy.");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 94;
            Item.height = 44;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 900;
            Item.knockBack = 12f;
            Item.useTime = 52;
            Item.useAnimation = 52;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = FireSound;
            Item.noMelee = true;

            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.Calamity().donorItem = true;

            Item.shoot = ModContent.ProjectileType<KarasawaShot>();
            Item.shootSpeed = 1f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override bool CanUseItem(Player player)
        {
            return CalamityGlobalItem.HasEnoughAmmo(player, Item, 5);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            if (velocity.Length() > 5f)
            {
                velocity.Normalize();
                velocity *= 5f;
            }
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<KarasawaShot>(), damage, knockback, player.whoAmI, 0f, 0f);

            // Consume 5 ammo per shot
            CalamityGlobalItem.ConsumeAdditionalAmmo(player, Item, 5);

            return false;
        }

        // Disable vanilla ammo consumption
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-20, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LargeRuby).
                AddIngredient<MysteriousCircuitry>(15).
                AddIngredient<DubiousPlating>(25).
                AddIngredient<GalacticaSingularity>(5).
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<NightmareFuel>(20).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}

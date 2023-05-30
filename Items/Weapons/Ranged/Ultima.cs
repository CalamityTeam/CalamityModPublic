using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Ultima : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Weapons.Ranged";
        public const float FullChargeTime = 420f;
        public override void SetDefaults()
        {
            Item.damage = 116;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 44;
            Item.height = 58;
            Item.useTime = Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.UseSound = SoundID.Item33;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<UltimaBowProjectile>();
            Item.shootSpeed = 18f;
            Item.useAmmo = AmmoID.Arrow;
            Item.channel = true;
            Item.useTurn = false;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.Calamity().donorItem = true;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity.SafeNormalize(Vector2.UnitX * player.direction), ModContent.ProjectileType<UltimaBowProjectile>(), 0, 0f, player.whoAmI);
            return false;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.Next(0, 100) >= 90;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.PulseBow).
                AddIngredient(ItemID.LaserRifle).
                AddIngredient<TheStorm>().
                AddIngredient<AstralRepeater>().
                AddIngredient<ExodiumCluster>(15).
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<DarksunFragment>(8).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}

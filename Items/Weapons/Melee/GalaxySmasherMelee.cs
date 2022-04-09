using Terraria.DataStructures;
using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class GalaxySmasherMelee : ModItem
    {
        public static int BaseDamage = 264;
        public static float Speed = 18f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galaxy Smasher");
            Tooltip.SetDefault("Explodes and summons death lasers on enemy hits");
        }

        public override void SetDefaults()
        {
            Item.width = 86;
            Item.height = 72;
            Item.DamageType = DamageClass.Melee;
            Item.damage = BaseDamage;
            Item.knockBack = 9f;
            Item.useAnimation = 13;
            Item.useTime = 13;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;

            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.value = Item.buyPrice(platinum: 1, gold: 80);

            Item.shoot = ModContent.ProjectileType<GalaxySmasherHammer>();
            Item.shootSpeed = Speed;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<StellarContemptMelee>().
                AddIngredient<CosmiliteBar>(10).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}

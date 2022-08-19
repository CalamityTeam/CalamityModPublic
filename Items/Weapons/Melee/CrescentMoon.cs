using Terraria.DataStructures;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class CrescentMoon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crescent Moon");
            Tooltip.SetDefault("People wanted the moon, let's bring the moon to them.\n" +
            "Fires a whip sword that summons homing crescent moons");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.damage = 300;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.useAnimation = 18;
            Item.useTime = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item82;
            Item.value = Item.buyPrice(1, 40, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
            Item.shootSpeed = 24f;
            Item.shoot = ModContent.ProjectileType<CrescentMoonFlail>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float ai3 = (Main.rand.NextFloat() - 0.5f) * 0.7853982f; //0.5
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI, 0f, ai3);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Lumenyl>(8).
                AddIngredient<RuinousSoul>(3).
                AddIngredient<ExodiumCluster>(16).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}

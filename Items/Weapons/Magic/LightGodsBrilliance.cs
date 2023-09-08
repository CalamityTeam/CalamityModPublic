using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class LightGodsBrilliance : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetDefaults()
        {
            Item.damage = 111;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 6;
            Item.width = 42;
            Item.height = 48;
            Item.useTime = Item.useAnimation = 4;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.Calamity().donorItem = true;
            Item.UseSound = SoundID.Item9;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<LightBead>();
            Item.shootSpeed = 25f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 beadVelocity = velocity + Main.rand.NextVector2Square(-2.5f, 2.5f);
            Projectile.NewProjectile(source, position, beadVelocity, type, damage, knockback, player.whoAmI);

            if (Main.rand.NextBool(3))
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<LightBall>(), damage * 2, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ShadecrystalTome>().
                AddIngredient<AbyssalTome>().
                AddIngredient(ItemID.HolyWater, 10).
                AddIngredient(ItemID.SoulofLight, 30).
                AddIngredient<EffulgentFeather>(5).
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<NightmareFuel>(20).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}

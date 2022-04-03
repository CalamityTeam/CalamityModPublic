using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Magic
{
    public class TerraRay : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terra Ray");
            Tooltip.SetDefault("Casts an energy ray that splits into energy on enemy hits\n" +
                "More energy is created the farther along the ray the hit enemy is");
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 54;
            Item.height = 54;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.value = Item.buyPrice(0, 80, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item60;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<TerraBeam>();
            Item.shootSpeed = 6f;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(15, 15);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 shootVelocity = new Vector2(speedX, speedY);
            Vector2 shootPosition = position + shootVelocity * 8f;
            Projectile.NewProjectile(shootPosition, shootVelocity, type, damage, knockBack, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<NightsRay>()).AddIngredient(ModContent.ItemType<ValkyrieRay>()).AddIngredient(ModContent.ItemType<LivingShard>(), 7).AddTile(TileID.MythrilAnvil).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<CarnageRay>()).AddIngredient(ModContent.ItemType<ValkyrieRay>()).AddIngredient(ModContent.ItemType<LivingShard>(), 7).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}

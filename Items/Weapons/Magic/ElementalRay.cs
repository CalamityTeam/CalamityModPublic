using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ElementalRay : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 18;
            Item.width = 62;
            Item.height = 62;
            Item.useTime = 4;
            Item.useAnimation = 16;
            Item.reuseDelay = 14;
            Item.useLimitPerAnimation = 4;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7.5f;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item60;
            Item.autoReuse = true;
            Item.shoot = 1;
            Item.shootSpeed = 6f;
        }

        
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float offsetAngle = MathHelper.TwoPi * player.itemAnimation / player.itemAnimationMax;
            offsetAngle += MathHelper.PiOver4 + Main.rand.NextFloat(0f, 1.3f);
            float shootSpeed = 1f;

            if (player.itemAnimation == Item.useAnimation)
                type = ModContent.ProjectileType<SolarElementalBeam>();
            else if (player.itemAnimation == Item.useAnimation - Item.useTime)
            {
                type = ModContent.ProjectileType<NebulaElementalBeam>();
                offsetAngle -= NebulaElementalBeam.UniversalAngularSpeed * 0.5f;
            }
            else if (player.itemAnimation == Item.useAnimation - Item.useTime * 2)
            {
                type = ModContent.ProjectileType<VortexElementalBeam>();
                shootSpeed = 2f;
            }
            else if (player.itemAnimation == Item.useAnimation - Item.useTime * 3)
                type = ModContent.ProjectileType<StardustElementalBeam>();
            else
                return false;

            Vector2 spawnOffset = player.SafeDirectionTo(Main.MouseWorld, Vector2.UnitY).RotatedBy(offsetAngle) * -Main.rand.NextFloat(40f, 96f);
            Vector2 shootDirection = (Main.MouseWorld - (position + spawnOffset)).SafeNormalize(Vector2.UnitX * player.direction);
            int beam = Projectile.NewProjectile(source, position + spawnOffset, shootDirection * shootSpeed, type, damage, knockback, player.whoAmI);

            // Define specific values for fired lightning.
            if (type == ModContent.ProjectileType<VortexElementalBeam>())
            {
                Main.projectile[beam].ai[0] = shootDirection.ToRotation();
                Main.projectile[beam].ai[1] = Main.rand.Next(100);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<TerraRay>().
                AddIngredient(ItemID.LunarBar, 5).
                AddIngredient<LifeAlloy>(5).
                AddIngredient<GalacticaSingularity>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}

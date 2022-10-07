using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class AcidGun : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acid Gun");
            Tooltip.SetDefault("Releases three streams of acid");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 28;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 9;
            Item.width = 42;
            Item.height = 28;
            Item.useTime = Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item13;
            Item.autoReuse = true;
            Item.shootSpeed = 14f;
            Item.shoot = ModContent.ProjectileType<AcidGunStream>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float angle = MathHelper.Lerp(-0.145f, 0.145f, 1f / 3f);
            int acid1 = Projectile.NewProjectile(source, position, velocity.RotatedBy(angle), type, damage, knockback, player.whoAmI);

            angle = MathHelper.Lerp(-0.145f, 0.145f, 2f / 3f);
            int acid2 = Projectile.NewProjectile(source, position, velocity.RotatedBy(angle), type, damage, knockback, player.whoAmI);

            angle = MathHelper.Lerp(-0.145f, 0.145f, 1f);
            int acid3 = Projectile.NewProjectile(source, position, velocity.RotatedBy(angle), type, damage, knockback, player.whoAmI);

            // Keep track of the other 2 acid streams
            if (Main.projectile.IndexInRange(acid1))
            {
                Main.projectile[acid1].ai[0] = acid2;
                Main.projectile[acid1].ai[1] = acid3;
            }
            if (Main.projectile.IndexInRange(acid2))
            {
                Main.projectile[acid2].ai[0] = acid1;
                Main.projectile[acid2].ai[1] = acid3;
            }
            if (Main.projectile.IndexInRange(acid3))
            {
                Main.projectile[acid3].ai[0] = acid1;
                Main.projectile[acid3].ai[1] = acid2;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SulphuricScale>(10).
                AddIngredient<Acidwood>(35).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}

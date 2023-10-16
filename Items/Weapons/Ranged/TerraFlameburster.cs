using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class TerraFlameburster : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public int FlareCounter = 0;

        public override void SetDefaults()
        {
            Item.damage = 55;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 68;
            Item.height = 22;
            Item.useTime = 8;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.25f;
            Item.UseSound = SoundID.Item34;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<TerraFire>();
            Item.shootSpeed = 8f;
            Item.useAmmo = AmmoID.Gel;
            Item.consumeAmmoOnFirstShotOnly = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Fires a homing terra flare
            FlareCounter++;
            if (FlareCounter >= 4)
            {
                FlareCounter = 0;
                Vector2 newPos = position + velocity.SafeNormalize(Vector2.UnitX) * 36f;
                float randAngle = Main.rand.NextFloat(8f, 15f);
                Vector2 newVel = velocity.RotatedBy(MathHelper.ToRadians(randAngle)) * 1.5f;
                Projectile.NewProjectile(source, newPos, newVel, ModContent.ProjectileType<TerraFlare>(), damage, knockback, player.whoAmI);
                newVel = velocity.RotatedBy(MathHelper.ToRadians(-randAngle)) * 1.5f;
                Projectile.NewProjectile(source, newPos, newVel, ModContent.ProjectileType<TerraFlare>(), damage, knockback, player.whoAmI);
            }

            // Fires two flames, green and blue, slightly randomly spread
            for (int i = 0; i < 2; i++)
            {
                Vector2 newVel = velocity.RotatedByRandom(MathHelper.ToRadians(6f));
                Projectile.NewProjectile(source, position, newVel, type, damage, knockback, player.whoAmI, i);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Flamethrower).
                AddIngredient<Meowthrower>().
                AddIngredient<LivingShard>(12).
                AddIngredient<EssenceofSunlight>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

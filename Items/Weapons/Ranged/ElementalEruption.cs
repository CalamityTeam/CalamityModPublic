using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class ElementalEruption : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 77;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 64;
            Item.height = 34;
            Item.useTime = 12;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.UseSound = SoundID.Item34;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ElementalFire>();
            Item.shootSpeed = 9f;
            Item.useAmmo = AmmoID.Gel;
            Item.consumeAmmoOnFirstShotOnly = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 newVel = velocity.RotatedByRandom(MathHelper.ToRadians(8f));
                Projectile.NewProjectile(source, position, newVel, type, damage, knockback, player.whoAmI);
            }
            return true; // Fires one directly with no randomness, totaling 3 projectiles
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<TerraFlameburster>().
                AddIngredient<BlightSpewer>().
                AddIngredient<HavocsBreath>().
                AddIngredient(ItemID.LunarBar, 5).
                AddIngredient<LifeAlloy>(5).
                AddIngredient<GalacticaSingularity>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}

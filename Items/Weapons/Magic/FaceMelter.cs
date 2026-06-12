using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class FaceMelter : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 50;
            Item.damage = 130;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 14;
            Item.useAnimation = Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;

            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.Calamity().donorItem = true;

            Item.shoot = ModContent.ProjectileType<MelterNote1>();
            Item.UseSound = SoundID.Item47;
            Item.autoReuse = true;
            Item.shootSpeed = 20f;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-15, 0);

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, player.ClampedMouseWorld(), Vector2.Zero, ModContent.ProjectileType<MelterAmp>(), damage, knockback, player.whoAmI);
                return false;
            }
            else
            {
                Vector2 offset = velocity.SafeNormalize(Vector2.UnitX) * 8f;
                Projectile.NewProjectile(source, position + offset.RotatedBy(MathHelper.PiOver2), velocity, type, (int)(damage * 1.5f), knockback, player.whoAmI);
                Projectile.NewProjectile(source, position + offset.RotatedBy(-MathHelper.PiOver2), velocity * 1.5f, ModContent.ProjectileType<MelterNote2>(), damage, knockback, player.whoAmI);
                return false;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.TheAxe).
                AddIngredient(ItemID.SparkleGuitar). // Stellar Tune from Empress of Light
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<NightmareFuel>(20).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}

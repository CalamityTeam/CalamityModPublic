using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class FlarewingBow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 31;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 72;
            Item.useTime = 41;
            Item.useAnimation = 41;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 18f;
            Item.useAmmo = AmmoID.Arrow;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float tenthPi = 0.314159274f;
            Vector2 arrowVel = velocity;
            arrowVel.Normalize();
            arrowVel *= 50f;
            bool arrowHitsTiles = Collision.CanHit(vector2, 0, 0, vector2 + arrowVel, 0, 0);
            for (int i = 0; i < 3; i++)
            {
                float piOffsetValue = (float)i - 1f;
                Vector2 offsetSpawn = arrowVel.RotatedBy((double)(tenthPi * piOffsetValue), default);
                if (!arrowHitsTiles)
                {
                    offsetSpawn -= arrowVel;
                }
                if (CalamityUtils.CheckWoodenAmmo(type, player))
                {
                    int arrowSpawn = Projectile.NewProjectile(source, vector2.X + offsetSpawn.X, vector2.Y + offsetSpawn.Y, velocity.X, velocity.Y, ModContent.ProjectileType<FlareBat>(), damage, knockback, player.whoAmI);
                    Main.projectile[arrowSpawn].noDropItem = true;
                }
                else
                {
                    int arrowSpawn = Projectile.NewProjectile(source, vector2.X + offsetSpawn.X, vector2.Y + offsetSpawn.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
                    Main.projectile[arrowSpawn].noDropItem = true;
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HellwingBow).
                AddIngredient<EssenceofSunlight>(5).
                AddIngredient(ItemID.LivingFireBlock, 50).
                AddIngredient(ItemID.Obsidian, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}

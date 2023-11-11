using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    [LegacyName("NettlelineGreatbow")]
    public class NettlevineGreatbow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 73;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 36;
            Item.height = 64;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Arrow;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float tenthPi = 0.314159274f;
            Vector2 arrowVel = velocity;
            arrowVel.Normalize();
            arrowVel *= 40f;
            bool arrowHitsTiles = Collision.CanHit(vector2, 0, 0, vector2 + arrowVel, 0, 0);
            for (int i = 0; i < 4; i++)
            {
                float piOffsetAmt = (float)i - 3f / 2f;
                Vector2 offsetSpawn = arrowVel.RotatedBy((double)(tenthPi * piOffsetAmt), default);
                if (!arrowHitsTiles)
                {
                    offsetSpawn -= arrowVel;
                }
                int arrowSpawn = Projectile.NewProjectile(source, vector2.X + offsetSpawn.X, vector2.Y + offsetSpawn.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
                Main.projectile[arrowSpawn].noDropItem = true;
            }
            for (int i = 0; i < 2; i++)
            {
                float SpeedX = velocity.X + (float)Main.rand.Next(-40, 41) * 0.05f;
                float SpeedY = velocity.Y + (float)Main.rand.Next(-40, 41) * 0.05f;
                switch (Main.rand.Next(2))
                {
                    case 0:
                        type = ProjectileID.VenomArrow;
                        break;
                    case 1:
                        type = ProjectileID.ChlorophyteArrow;
                        break;
                    default:
                        break;
                }
                int index = Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI);
                Main.projectile[index].noDropItem = true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UelibloomBar>(12).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}

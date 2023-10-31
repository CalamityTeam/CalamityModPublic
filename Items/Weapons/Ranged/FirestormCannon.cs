using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class FirestormCannon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetStaticDefaults()
        {
                       ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 14;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 56;
            Item.height = 28;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Flare;
            Item.shootSpeed = 5.5f;
            Item.useAmmo = AmmoID.Flare;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.Next(100) >= 70;

        public override bool AltFunctionUse(Player player) => true;

        public override float UseSpeedMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 1 / 3f;
            return 1f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                velocity *= 2f;
                int flareAmt = Main.rand.Next(4, 6);
                for (int index = 0; index < flareAmt; ++index)
                {
                    float SpeedX = velocity.X + (float)Main.rand.Next(-50, 51) * 0.05f;
                    float SpeedY = velocity.Y + (float)Main.rand.Next(-50, 51) * 0.05f;
                    int flare = Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI);
                    if (flare.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[flare].penetrate = 1;
                        Main.projectile[flare].timeLeft = 600;
                        Main.projectile[flare].DamageType = DamageClass.Ranged;
                    }
                }
                return false;
            }
            else
            {
                int flareAmt = Main.rand.Next(1, 3);
                for (int index = 0; index < flareAmt; ++index)
                {
                    float SpeedX = velocity.X + (float)Main.rand.Next(-40, 41) * 0.05f;
                    float SpeedY = velocity.Y + (float)Main.rand.Next(-40, 41) * 0.05f;
                    int flare = Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI);
                    if (flare.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[flare].DamageType = DamageClass.Ranged;
                        Main.projectile[flare].timeLeft = 200;
                        Main.projectile[flare].penetrate = 3;
                    }
                }
                return false;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FlareGun).
                AddIngredient(ItemID.HellstoneBar, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}

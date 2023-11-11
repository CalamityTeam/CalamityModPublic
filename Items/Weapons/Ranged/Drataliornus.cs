using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Drataliornus : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        private const double RightClickDamageRatio = 0.6;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 129;
            Item.knockBack = 1f;
            Item.shootSpeed = 18f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 12;
            Item.useAnimation = 24;
            Item.reuseDelay = 48;
            Item.useLimitPerAnimation = 2;
            Item.width = 64;
            Item.height = 84;
            Item.UseSound = SoundID.Item5;
            Item.shoot = ModContent.ProjectileType<DrataliornusBow>();
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Ranged;
            Item.channel = true;
            Item.useTurn = false;
            Item.useAmmo = AmmoID.Arrow;
            Item.autoReuse = true;
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.noUseGraphic = false;
            }
            else
            {
                Item.noUseGraphic = true;
                if (player.ownedProjectileCounts[Item.shoot] > 0)
                {
                    return false;
                }
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2) //tsunami
            {
                int flameID = ModContent.ProjectileType<DrataliornusFlame>();
                const int numFlames = 5;
                int flameDamage = (int)(damage * RightClickDamageRatio);

                const float fifteenHundredthPi = 0.471238898f;
                Vector2 spinningpoint = velocity;
                spinningpoint.Normalize();
                spinningpoint *= 36f;
                for (int i = 0; i < numFlames; ++i)
                {
                    float piArrowOffset = i - (numFlames - 1) / 2;
                    Vector2 offsetSpawn = spinningpoint.RotatedBy(fifteenHundredthPi * piArrowOffset, new Vector2());
                    Projectile.NewProjectile(source, position.X + offsetSpawn.X, position.Y + offsetSpawn.Y, velocity.X, velocity.Y, flameID, flameDamage, knockback, player.whoAmI, 1f, 0f);
                }
            }
            else
            {
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<DrataliornusBow>(), 0, 0f, player.whoAmI);
            }

            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(4f, 0f);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BlossomFlux>().
                AddIngredient<EffulgentFeather>(12).
                AddIngredient<YharonSoulFragment>(4).
                AddIngredient<AuricBar>(5).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}

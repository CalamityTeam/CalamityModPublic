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
    public class AlphaRay : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
                       ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }


        public override void SetDefaults()
        {
            Item.damage = 160;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 8;
            Item.width = 84;
            Item.height = 74;
            Item.useTime = Item.useAnimation = 4;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.UseSound = SoundID.Item33;
            Item.autoReuse = true;
            Item.shootSpeed = 6f;
            Item.shoot = ModContent.ProjectileType<ParticleBeamofDoom>();
            Item.rare = ModContent.RarityType<DarkBlue>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X * 1.35f, velocity.Y * 1.35f, ModContent.ProjectileType<BigBeamofDeath>(), (int)(damage * 1.6625), knockback, player.whoAmI);
                int laserAmt = 3;
                float SpeedX = velocity.X + Main.rand.NextFloat(-1f, 1f);
                float SpeedY = velocity.Y + Main.rand.NextFloat(-1f, 1f);
                for (int i = 0; i < laserAmt; ++i)
                {
                    int laser = Projectile.NewProjectile(source, position.X, position.Y, SpeedX * 1.15f, SpeedY * 1.15f, ProjectileID.LaserMachinegunLaser, (int)(damage * 0.4), knockback * 0.4f, player.whoAmI);
                    Main.projectile[laser].timeLeft = 120;
                    Main.projectile[laser].tileCollide = false;
                }
            }
            else
            {
                Vector2 laserSpawnPos = player.RotatedRelativePoint(player.MountedCenter, true);
                float tenthPi = 0.314159274f;
                int laserAmt = 3;
                Vector2 laserVelocity = velocity;
                laserVelocity.Normalize();
                laserVelocity *= 80f;
                bool laserHitsTiles = Collision.CanHit(laserSpawnPos, 0, 0, laserSpawnPos + laserVelocity, 0, 0);
                for (int i = 0; i < laserAmt; i++)
                {
                    float laserOffset = (float)i - ((float)laserAmt - 1f) / 2f;
                    Vector2 offsetSpawnPos = laserVelocity.RotatedBy((double)(tenthPi * laserOffset), default);
                    if (!laserHitsTiles)
                    {
                        offsetSpawnPos -= laserVelocity;
                    }
                    Projectile.NewProjectile(source, laserSpawnPos.X + offsetSpawnPos.X, laserSpawnPos.Y + offsetSpawnPos.Y, velocity.X * 1.5f, velocity.Y * 1.5f, type, (int)(damage * 0.8), knockback, player.whoAmI);
                    int laser = Projectile.NewProjectile(source, laserSpawnPos.X + offsetSpawnPos.X, laserSpawnPos.Y + offsetSpawnPos.Y, velocity.X * 2f, velocity.Y * 2f, ProjectileID.LaserMachinegunLaser, (int)(damage * 0.4), knockback * 0.4f, player.whoAmI);
                    Main.projectile[laser].timeLeft = 120;
                    Main.projectile[laser].tileCollide = false;
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Genisis>().
                AddIngredient<Wingman>(2).
                AddIngredient<GalacticaSingularity>(5).
                AddIngredient<CosmiliteBar>(10).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}

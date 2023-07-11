using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Seadragon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        // Odd shots are bullets (including the first). Even shots are water jets.
        // The 9th shot adds a rocket. The 17th shot adds an ultra powerful muzzle blast, then resets the counter to 2.
        // This is intentional so you don't get the muzzle blast instantly when you start firing and have to play around it.
        private int shotCounter = 1;

        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 90;
            Item.height = 38;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.5f;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Both bullets and water jets have very slight inaccuracy
            float SpeedX = velocity.X + Main.rand.Next(-10, 11) * 0.05f;
            float SpeedY = velocity.Y + Main.rand.Next(-10, 11) * 0.05f;

            // Fire either the bullet or the water jet, depending on cadence.
            bool fireWater = shotCounter % 2 == 0;
            int projectileToFire = fireWater ? ModContent.ProjectileType<ArcherfishShot>() : type;
            Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, projectileToFire, damage, knockback, player.whoAmI, 0.0f, 0.0f);

            // Always fires a close range water blast.
            // It goes in the same direction as the main shot but has a minor velocity variation to be less monotonous.
            Vector2 waterRingVec = new Vector2(SpeedX, SpeedY) * Main.rand.NextFloat(0.45f, 0.65f);
            int waterRingDamage = (int)(damage * 0.5f);
            float boostedKB = knockback + 7f; // Same KB bonus as Megalodon. The muzzle blast will annihilate nearby enemies and send their gibs flying.
            Projectile.NewProjectile(source, position, waterRingVec, ModContent.ProjectileType<ArcherfishRing>(), waterRingDamage, boostedKB, player.whoAmI);

            // Check cadence position to determine which bonus projectile to add, if any.
            bool fireRocket = shotCounter == 9;
            bool muzzleBlast = shotCounter == 17;

            // Rockets have their own more chaotic spread than bullets.
            if (fireRocket)
            {
                Vector2 rocketVec = velocity.SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.25f, 0.25f)) * 25f;
                int rocketDamage = damage * 5;
                Projectile.NewProjectile(source, position, rocketVec, ModContent.ProjectileType<SeaDragonRocket>(), rocketDamage, knockback, player.whoAmI);
            }
            
            // Muzzle blasts are always directly in line with the gun's muzzle.
            if (muzzleBlast)
            {
                Vector2 muzzleBlastVec = velocity.SafeNormalize(Vector2.Zero) * 18f;
                int muzzleBlastDamage = damage * 40; // This is probably too much but it's every 18th shot...
                float muzzleBlastKB = knockback + 12f;
                Projectile.NewProjectile(source, position, muzzleBlastVec, ModContent.ProjectileType<SeaDragonMuzzleBlast>(), muzzleBlastDamage, muzzleBlastKB, player.whoAmI);

                // Play additional sound for muzzle blasts
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, position);
            }

            // Increment and then reset the shot counter so the pattern loops indefinitely. The muzzle blast replaces the first bullet in the repeating pattern.
            ++shotCounter;
            if (shotCounter > 17)
                shotCounter = 2;
            return false;
        }

        // Does not consume ammo when firing water jets.
        public override bool CanConsumeAmmo(Item ammo, Player player) => shotCounter % 2 == 1;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Megalodon>().
                AddIngredient<Phantoplasm>(9).
                AddIngredient<ArmoredShell>(3).
                AddIngredient<SeaPrism>(10).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}

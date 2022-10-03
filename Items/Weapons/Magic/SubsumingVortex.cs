using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class SubsumingVortex : ModItem
    {
        public const int RightClickVortexCount = 3;

        public const int VortexReleaseRate = 27;

        public const int VortexShootDelay = 56;

        public const int LargeVortexChargeupTime = 240;

        public const float RightClickSpeedFactor = 1.3f;

        public const float RightClickDamageFactor = 0.3f;

        public const float SmallVortexTargetRange = 1300f;

        public const float GiantVortexMouseDriftFactor = 0.09f;

        public const float ReleaseSpeed = 33f;

        public const float ReleaseDamageFactor = 4.35f;

        public static readonly SoundStyle ExplosionSound = new("CalamityMod/Sounds/Custom/SubsumingVortexExplosion");

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Subsuming Vortex");
            Tooltip.SetDefault("Left clicking releases a barrage of vortices that race towards enemies\n" +
                               "Right clicking casts a gigantic vortex in front of you with a bias towards the mouse\n" +
                               "When enemies are near the vortex, it sends multiple fast-moving smaller vortices towards them\n" +
                               "After enough time has passed the vortex stops shooting, and releasing the right mouse button fires the vortex towards the mouse");
            SacrificeTotal = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 533;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.width = 38;
            Item.height = 48;
            Item.UseSound = SoundID.Item84;
            Item.useTime = Item.useAnimation = 20;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.rare = ModContent.RarityType<Violet>();
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<EnormousConsumingVortex>();
            Item.shootSpeed = 7f;
        }

        public override void HoldItem(Player player)
        {
            Item.channel = player.altFunctionUse == 2;
            player.Calamity().rightClickListener = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool AltFunctionUse(Player player) => true;

        public override bool? CanAutoReuseItem(Player player) => true;

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Magic/SubsumingVortexGlow").Value);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                int vortexID = ModContent.ProjectileType<ExoVortex2>();
                for (int i = 0; i < RightClickVortexCount; i++)
                {
                    float hue = (i / (float)(RightClickVortexCount - 1f) + Main.rand.NextFloat(0.3f)) % 1f;
                    Vector2 vortexVelocity = velocity * RightClickSpeedFactor + Main.rand.NextVector2Square(-2.5f, 2.5f);
                    Projectile.NewProjectile(source, position, vortexVelocity, vortexID, (int)(damage * RightClickDamageFactor), knockback, player.whoAmI, hue);
                }
                return false;
            }
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AuguroftheElements>().
                AddIngredient<EventHorizon>().
                AddIngredient<TearsofHeaven>().
                AddIngredient<MiracleMatter>().
                AddTile<DraedonsForge>().
                Register();
        }
    }
}

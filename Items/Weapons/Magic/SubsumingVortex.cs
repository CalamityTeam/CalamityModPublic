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
using Terraria.GameContent;

namespace CalamityMod.Items.Weapons.Magic
{
    public class SubsumingVortex : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public const int RightClickVortexCount = 3;

        public const int VortexReleaseRate = 27;

        public const int VortexShootDelay = 56;

        public const int LargeVortexChargeupTime = 240;

        public const float RightClickSpeedFactor = 1.3f;

        public const float RightClickDamageFactor = 0.3f;

        public const float SmallVortexTargetRange = 1300f;

        public const float GiantVortexMouseDriftFactor = 0.09f;

        public const float ReleaseSpeed = 33f;

        public const float ReleaseDamageFactor = 4.65f;

        public static readonly SoundStyle ExplosionSound = new("CalamityMod/Sounds/Custom/SubsumingVortexExplosion");

        public override void SetStaticDefaults()
        {
                       ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 466;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = Item.useAnimation = 20;
            Item.shootSpeed = 7f;
            Item.mana = 22;
            Item.knockBack = 5f;

            Item.shoot = ModContent.ProjectileType<EnormousConsumingVortex>();

            Item.width = 86;
            Item.height = 104;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item84;
            Item.rare = ModContent.RarityType<Violet>();
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = true;
        }

        public override void HoldItem(Player player)
        {
            Item.channel = player.altFunctionUse == 2;
            player.Calamity().rightClickListener = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool AltFunctionUse(Player player) => true;

        public override bool? CanAutoReuseItem(Player player) => true;

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            CalamityUtils.DrawInventoryCustomScale(
                spriteBatch,
                texture: TextureAssets.Item[Type].Value,
                position,
                frame,
                drawColor,
                itemColor,
                origin,
                scale,
                wantedScale: 0.4f,
                drawOffset: default
            );
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Magic/SubsumingVortexGlow").Value);
        }

        public override Vector2? HoldoutOffset() => new Vector2(-6f, 0);

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

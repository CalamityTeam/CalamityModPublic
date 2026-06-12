using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Photoviscerator : ModItem, ILocalizedModType
    {
        public static readonly SoundStyle UseSound = new("CalamityMod/Sounds/Item/PhotoUseSound") { Volume = 0.35f };
        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/Item/PhotoHitSound") { Volume = 0.4f };
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        // Left-click stats
        public static int AmmoSavedPercent = 95;
        public static int LightBombCooldown = 10;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(AmmoSavedPercent);

        // Right-click stats
        public static float RightClickVelocityMult = 2.5f;
        public static int RightClickCooldown = 25;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 208;
            Item.height = 66;

            Item.damage = 495;
            Item.DamageType = DamageClass.Ranged;
            Item.useAnimation = Item.useTime = LightBombCooldown;
            Item.shootSpeed = 6f;
            Item.knockBack = 2f;
            Item.shoot = ModContent.ProjectileType<PhotovisceratorHoldout>();
            Item.useAmmo = AmmoID.Gel;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.rare = ModContent.RarityType<ExoticRainbow>();
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
        }

        public override bool AltFunctionUse(Player player) => true;

        // Spawning the holdout cannot consume ammo
        public override bool CanConsumeAmmo(Item ammo, Player player) => player.ownedProjectileCounts[Item.shoot] > 0;

        public override void HoldItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                player.Calamity().rightClickListener = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // The holdout will initially double up when right clicking otherwise
            if (player.altFunctionUse == 2f)
            {
                if (player.Calamity().mouseRight && player.whoAmI == Main.myPlayer && !Main.mapFullscreen && !Main.blockMouse)
                    Projectile.NewProjectile(source, position, Vector2.Zero, type, 0, 0f, player.whoAmI);
                else
                    return false;
            }
            else
                Projectile.NewProjectile(source, position, Vector2.Zero, type, 0, 0f, player.whoAmI);
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Ranged/PhotovisceratorGlow").Value);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ChromaticEruption>().
                AddIngredient<HalleysInferno>().
                AddIngredient<DeadSunsWind>().
                AddIngredient<MiracleMatter>().
                AddTile<DraedonsForge>().
                Register();
        }
    }
}

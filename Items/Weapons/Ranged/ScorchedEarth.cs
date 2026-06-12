using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class ScorchedEarth : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public static readonly SoundStyle RocketShoot = new("CalamityMod/Sounds/Item/ScorpioShot") { Volume = 0.45f };

        public static int AmmoSavedPercent = 50;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(AmmoSavedPercent);

        public static int OriginalUseTime = 60;
        public static int TimeBetweenBursts = 10;
        public static int ProjectilesPerBurst = 4;
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<Daybroken>(), BuffID.Oiled];
        }
        public override void SetDefaults()
        {
            Item.damage = 550;
            Item.DamageType = DamageClass.Ranged;
            Item.useAnimation = Item.useTime = OriginalUseTime;
            Item.shoot = ProjectileType<ScorchedEarthHoldout>();
            Item.shootSpeed = 15f;
            Item.knockBack = 6.5f;

            Item.width = 104;
            Item.height = 44;
            Item.noMelee = true;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.useAmmo = AmmoID.Rocket;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = new SoundStyle("CalamityMod/Sounds/Item/DudFire") with { Volume = .4f, Pitch = -.9f, PitchVariance = 0.1f };
            Item.Calamity().donorItem = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0;

        // Spawning the holdout won't consume ammo.
        public override bool CanConsumeAmmo(Item ammo, Player player) => player.ownedProjectileCounts[Item.shoot] > 0 && Main.rand.Next(100) >= AmmoSavedPercent;

        // Makes the rotation of the mouse around the player sync in multiplayer.
        public override void HoldItem(Player player) => player.Calamity().mouseRotationListener = true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile holdout = Projectile.NewProjectileDirect(source, player.MountedCenter, Vector2.Zero, ProjectileType<ScorchedEarthHoldout>(), 0, 0f, player.whoAmI, -30);

            // We set the rotation to the direction to the mouse so the first frame doesn't appear bugged out.
            holdout.velocity = (player.Calamity().mouseWorld - player.MountedCenter).SafeNormalize(Vector2.Zero);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Scorpio>().
                AddRecipeGroup("AnyAdamantiteBar", 15).
                AddIngredient<DarksunFragment>(10).
                AddIngredient(ItemID.FragmentSolar, 50).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}

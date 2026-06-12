using CalamityMod.CalPlayer;
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
    public class Omicron : ModItem, ILocalizedModType
    {
        public static float FireRate = 15f;
        public static float StarterWinup = 60f;
        public static float WingmanFireRate = 10f;
        
        public new string LocalizationCategory => "Items.Weapons.Magic";

        public override void SetDefaults()
        {
            Item.width = 122;
            Item.height = 54;
            Item.damage = 80;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.useAnimation = Item.useTime = 4;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.shootSpeed = 6f;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<OmicronHoldout>();
            Item.rare = ModContent.RarityType<CosmicPurple>();
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0 && player.ownedProjectileCounts[ModContent.ProjectileType<OmicronWingman>()] < 2;

        // Makes the rotation of the mouse around the player sync in multiplayer.
        public override void HoldItem(Player player)
        {
            CalamityPlayer calPlayer = player.Calamity();
            calPlayer.mouseWorldListener = true;
            calPlayer.mouseRotationListener = true;
            calPlayer.rightClickListener = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
            for (int i = 0; i < 2; i++)
            {
                Projectile holdout2 = Projectile.NewProjectileDirect(source, player.MountedCenter, Vector2.Zero, ModContent.ProjectileType<OmicronWingman>(), damage, knockback, player.whoAmI, 0, 0, i == 0 ? 1 : -1);
                holdout2.velocity = (player.Calamity().mouseWorld - player.MountedCenter).SafeNormalize(Vector2.Zero);
            }

            Projectile holdout = Projectile.NewProjectileDirect(source, player.MountedCenter, Vector2.Zero, ModContent.ProjectileType<OmicronHoldout>(), damage, knockback, player.whoAmI);
            holdout.velocity = (player.Calamity().mouseWorld - player.MountedCenter).SafeNormalize(Vector2.Zero);

            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Genesis>().
                AddIngredient<Wingman>().
                AddIngredient<CosmiliteBar>(10).
                AddIngredient(ItemID.FragmentNebula, 5).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}

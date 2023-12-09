using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Photoviscerator : ModItem, ILocalizedModType
    {
        public static readonly SoundStyle UseSound = new("CalamityMod/Sounds/Item/PhotoUseSound") { Volume = 0.35f };
        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/Item/PhotoHitSound") { Volume = 0.4f };
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        // Left-click stats
        public static float AmmoNotConsumeChance = 0.95f;
        public static int LightBombCooldown = 10;

        // Right-click stats
        public static float RightClickVelocityMult = 2.5f;
        public static int RightClickCooldown = 20;

        public override void SetDefaults()
        {
            Item.width = 208;
            Item.height = 66;

            Item.damage = 445;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = LightBombCooldown;
            Item.shootSpeed = 6f;
            Item.knockBack = 2f;
            Item.shoot = ModContent.ProjectileType<PhotovisceratorHoldout>();
            Item.useAmmo = AmmoID.Gel;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.rare = ModContent.RarityType<Violet>();
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
        }

        public override bool AltFunctionUse(Player player) => true;

        // Spawning the holdout cannot consume ammo
        public override bool CanConsumeAmmo(Item ammo, Player player) => player.ownedProjectileCounts[Item.shoot] > 0;

        public override void HoldItem(Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return;
            
            // Right-click channeling
            player.Calamity().rightClickListener = true;

            if (player.Calamity().mouseRight && CanUseItem(player) && !Main.mapFullscreen && !Main.blockMouse)
            {
                // Only one out at a time
                if (Main.projectile.Any(n => n.active && n.type == Item.shoot && n.owner == player.whoAmI))
                    return;
                // If you don't have any Gel don't even spawn the holdout
                if (!player.HasAmmo(Item))
                    return;

                var source = player.GetSource_ItemUse_WithPotentialAmmo(Item, ItemID.Gel);
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, Item.shoot, 0, 0f, player.whoAmI);
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // The holdout will initially double up when right clicking otherwise
            if (player.altFunctionUse == 2f)
                return false;
            
            Projectile.NewProjectile(source, position, Vector2.Zero, type, 0, 0f, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ElementalEruption>().
                AddIngredient<HalleysInferno>().
                AddIngredient<DeadSunsWind>().
                AddIngredient<MiracleMatter>().
                AddTile<DraedonsForge>().
                Register();
        }
    }
}

using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    [LegacyName("Thunderstorm")]
    public class Volterion : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.Electrified];
        }
        public override void SetDefaults()
        {
            Item.width = 220;
            Item.height = 60;
            Item.damage = 890;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 90;
            Item.useAnimation = Item.useTime = 80; // 42 frames of firing animation
            Item.knockBack = 2f;
            Item.shoot = ModContent.ProjectileType<VolterionHoldout>();
            Item.shootSpeed = 16f;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        // Cancels out the mana used to summon the holdout
        public override void OnConsumeMana(Player player, int manaConsumed)
        {
            if (player.ownedProjectileCounts[Item.shoot] <= 0)
                player.statMana += manaConsumed;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void HoldItem(Player player) => player.Calamity().mouseRotationListener = true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 spawnPosition = player.RotatedRelativePoint(player.MountedCenter, true);
            // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
            Projectile.NewProjectile(source, spawnPosition, player.Calamity().mouseWorld - spawnPosition, type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}

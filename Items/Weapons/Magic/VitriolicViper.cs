using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
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
    public class VitriolicViper : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<SulphuricPoisoning>(), ModContent.BuffType<Irradiated>()];
        }
        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 62;
            Item.damage = 2626; //Some kind of... brainstorm
            Item.DamageType = DamageClass.Magic;
            Item.mana = 60;
            Item.useAnimation = Item.useTime = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 6.5f;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<VitriolicViperHoldout>();
            Item.shootSpeed = 20f;

            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
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

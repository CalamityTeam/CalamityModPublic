using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class SeasSearing : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public override void SetStaticDefaults()
        {
            ItemID.Sets.IsRangedSpecialistWeapon[Type] = true;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.Venom, BuffID.Wet];
        }

        public override void SetDefaults()
        {
            Item.width = 88;
            Item.height = 44;
            Item.damage = 40;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 5;
            Item.useAnimation = 10;
            Item.reuseDelay = 16;
            Item.useLimitPerAnimation = 3;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SeasSearingBubble>();
            Item.shootSpeed = 13f;

            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, -5);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useTime = 30;
                Item.useAnimation = 30;
                Item.reuseDelay = 0;
            }
            else
            {
                Item.useTime = 5;
                Item.useAnimation = 10;
                Item.reuseDelay = 16;
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SeasSearingSecondary>(), (int)(damage * 1.22f), knockback, player.whoAmI);
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SeasSearingBubble>(), damage, knockback, player.whoAmI, 1f);
            }
            return false;
        }
    }
}

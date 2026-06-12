using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class RougeSlash : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<VermillionFlux>()];
        }
        public override void SetDefaults()
        {
            Item.mana = 30;
            Item.width = 28;
            Item.damage = 119;
            Item.DamageType = DamageClass.Magic;
            Item.height = 32;
            Item.useTime = 19;
            Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7.5f;
            Item.UseSound = SoundID.Item91;
            Item.value = CalamityGlobalItem.RarityPurpleBuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<RougeSlashLarge>();
            Item.shootSpeed = 24f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<RougeSlashLarge>(), damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, velocity * 0.8f, ModContent.ProjectileType<RougeSlashMedium>(), damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, velocity * 0.6f, ModContent.ProjectileType<RougeSlashSmall>(), damage / 2, knockback, player.whoAmI);
            return false;
        }
    }
}

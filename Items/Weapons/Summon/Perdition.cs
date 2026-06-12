using CalamityMod.Buffs.Summon;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class Perdition : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";

        public override void SetDefaults()
        {
            Item.width = Item.height = 56;
            Item.damage = 375;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<PerditionBuff>();
            Item.shoot = ModContent.ProjectileType<PerditionBeacon>();
            Item.sentry = true;
            Item.knockBack = 4f;

            Item.useAnimation = Item.useTime = 30;
            Item.mana = 10;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<CalamityRed>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.DD2_EtherianPortalOpen;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            CalamityUtils.KillShootProjectiles(true, type, player);
            var minion = Projectile.NewProjectileDirect(source, player.ClampedMouseWorld(), Vector2.Zero, type, damage, knockback, player.whoAmI);
            minion.originalDamage = Item.damage;
            player.UpdateMaxTurrets();
            return false;
        }
    }
}

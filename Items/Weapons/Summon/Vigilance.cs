using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Summon;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class Vigilance : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<BrimstoneFlames>()];
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.damage = 115;
            Item.mana = 10;
            Item.useAnimation = Item.useTime = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.DD2_BetsySummon;
            Item.autoReuse = true;
            Item.buffType = ModContent.BuffType<SoulSeekerBuff>();
            Item.shoot = ModContent.ProjectileType<SeekerSummonProj>();
            Item.DamageType = DamageClass.Summon;

            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<CalamityRed>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.maxMinions - player.slotsMinions >= 1f)
            {
                player.AddBuff(Item.buffType, 2);
                var minion = Projectile.NewProjectileDirect(source, player.ClampedMouseWorld(), Vector2.Zero, type, damage, knockback, player.whoAmI);
                minion.ai[0] = player.ownedProjectileCounts[type];
                minion.originalDamage = Item.damage;
            }
            return false;
        }
    }
}

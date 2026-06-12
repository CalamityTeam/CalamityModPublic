using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Summon;
using CalamityMod.Systems.Collections;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class MutatedTruffle : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";

        #region Minion Stats

        // In tiles; 1 tile = 16 units.
        public static float EnemyDistanceDetection = 8000f;

        public static int ToothballFireRate = 60; // In frames.
        public static int ToothballsUntilNextState = 5;
        public static float ToothballSpeed = 25f;
        public static float ToothballSpikeSpeed = 30f;

        public static float DashSpeed = 50f;
        public static int DashTime = 240;

        public static int VortexTimeUntilNextState = 300; // In frames.

        #endregion

        public override void SetStaticDefaults()
        {
            ItemID.Sets.StaffMinionSlotsRequired[Type] = 3f;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<SulphuricPoisoning>()];
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 26;
            Item.damage = 250;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<MutatedTruffleBuff>();
            Item.shoot = ModContent.ProjectileType<MutatedTruffleMinion>();
            Item.knockBack = 5f;

            Item.useAnimation = Item.useTime = 24;
            Item.mana = 10;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.NPCHit14;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
        }

        // Only one can be summoned.
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1 && player.maxMinions >= 3;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            CalamityUtils.KillShootProjectiles(true, type, player);
            var minion = Projectile.NewProjectileDirect(source, player.ClampedMouseWorld(), Main.rand.NextVector2Circular(2f, 2f), type, damage, knockback, player.whoAmI);
            minion.originalDamage = Item.damage;
            return false;
        }
    }
}

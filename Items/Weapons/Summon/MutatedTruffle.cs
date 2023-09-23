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

        public override void SetDefaults()
        {
            Item.damage = 275;
            Item.DamageType = DamageClass.Summon;
            Item.shoot = ModContent.ProjectileType<MutatedTruffleMinion>();
            Item.knockBack = 5f;

            Item.useTime = Item.useAnimation = 10;
            Item.mana = 10;
            Item.width = 24;
            Item.height = 26;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.NPCHit14;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Only one can be summoned.
            if (player.ownedProjectileCounts[type] < 1 && player.maxMinions >= 3)
                Projectile.NewProjectile(source, Main.MouseWorld, Main.rand.NextVector2Circular(2f, 2f), type, damage, knockback, player.whoAmI);

            return false;
        }
    }
}

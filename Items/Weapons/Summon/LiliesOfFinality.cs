using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Weapons.Summon
{
    public class LiliesOfFinality : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";

        #region Other Stats

        public static float MaxEnemyDistanceDetection = 2000f;
        public static int CommonDustID = 114;

        public static float Elster_DistanceFromTarget = 280f;
        public static float Elster_TargettingFlySpeed = 35f;
        public static float Elster_BulletProjectileSpeed = 20f;
        public static int Elster_BulletMaxUpdates = 2;

        public static float Ariane_BoltFireRate = 60f;
        public static float Ariane_TargettingFlySpeed = 20f;
        public static float Ariane_BoltProjectileSpeed = 25f;
        public static int Ariane_BoltTimeHoming = 600;
        public static float Ariane_MinTurnRate = 0.05f;
        public static float Ariane_MaxTurnRate = 0.2f;
        public static int Ariane_AoESize = 1050;
        public static float Ariane_AoEDMGMultiplier = 0.4f;

        #endregion

        public override void SetDefaults()
        {
            Item.damage = 512;
            Item.DamageType = DamageClass.Summon;
            Item.shoot = ProjectileType<LiliesOfFinalityElster>();
            Item.knockBack = 5f;

            Item.mana = 10;
            Item.useTime = Item.useAnimation = 15;
            Item.width = 36;
            Item.height = 50;
            Item.noMelee = true;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = RarityType<Violet>();
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = new("CalamityMod/Sounds/Item/LiliesOfFinalitySummonSpawn");
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0 && player.maxMinions - player.slotsMinions >= 2;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectileDirect(source, player.Calamity().mouseWorld, new Vector2(-1f, -1f) * 3f, type, damage, knockback, player.whoAmI);
            Projectile.NewProjectileDirect(source, player.Calamity().mouseWorld, new Vector2(1f, -1f) * 3f, ProjectileType<LiliesOfFinalityAriane>(), damage, knockback, player.whoAmI);

            if (Main.dedServ)
                return false;

            int dustAmount = Main.rand.Next(30, 40 + 1);
            for (int i = 0; i < dustAmount; i++)
            {
                float angle = MathHelper.TwoPi / dustAmount * i;
                Vector2 dustVelocity = angle.ToRotationVector2() * Main.rand.NextFloat(3f, 6f);
                Dust spawnDust = Dust.NewDustPerfect(
                    Main.MouseWorld,
                    CommonDustID,
                    dustVelocity,
                    Scale: Main.rand.NextFloat(1.2f, 1.5f));
                spawnDust.noGravity = true;
            }
            return false;
        }
    }
}

using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class InfectedRemote : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public const int DefaultIframes = 10;

        public const int ChargeIframes = 2;

        public const int RocketShootRate = 6;

        public const int BeeShootRate = 22;

        public const int MaxUpdatesWhenCharging = 2;

        public const float RegularChargeSpeed = 40f;

        public const float HorizontalRocketChargeSpeed = 22f;

        public const float RocketDamageFactor = 0.7f;

        public const float BeeDamageFactor = 0.65f;

        public const float MinionSlotRequirement = 3f;

        public const float EnemyTargetingRange = 1300f;

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.mana = 10;
            Item.width = 46;
            Item.height = 28;
            Item.useTime = Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item15; //phaseblade sound effect
            Item.shoot = ModContent.ProjectileType<PlaguePrincess>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;

            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.Calamity().donorItem = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            CalamityUtils.KillShootProjectiles(true, type, player);
            
            int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            return false;
        }
    }
}

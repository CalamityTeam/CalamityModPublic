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

        public static float EnemyDistanceDetection = 3200f; // In pixels. 1 tile = 16 pixels.
        public static float DashSpeed = 50f;
        public static float ToothballFireRate = 60f; // In frames.
        public static float DashTime = 240f;
        public static float ToothballsUntilNextState = 5f;
        public static float VortexTimeUntilNextState = 300f; // In frames.

        public override void SetDefaults()
        {
            Item.damage = 1000;
            Item.useTime = Item.useAnimation = 30;
            Item.mana = 10;
            Item.knockBack = 5f;
            Item.shoot = ModContent.ProjectileType<YoungDuke>();

            Item.DamageType = DamageClass.Summon;
            Item.width = 24;
            Item.height = 26;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item2;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, Main.MouseWorld, Main.rand.NextVector2Circular(2f, 2f), type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}

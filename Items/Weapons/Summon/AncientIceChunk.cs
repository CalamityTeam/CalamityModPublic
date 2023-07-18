using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class AncientIceChunk : ModItem, ILocalizedModType
    {
        #region Other stats

        public static int IFrames = 20;
        public static float EnemyDistanceDetection = 1200f;
        public static float MaxDistanceFromOwner = 400f; // Max distance the minions can be while shooting or idling.
        public static float DistanceToDash = 250f; // Min distance to start dashing.
        public static float DistanceToStopDash = 800f; // Max distance the player can be so the minions continue dashing.
        public static float MinVelocity = 12f;
        public static float TimeToShoot = 80f; // In frames.
        public static float ProjectileDMGMultiplier = 1.5f; // They're kinda' weak. 

        #endregion
        
        public new string LocalizationCategory => "Items.Weapons.Summon";

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(6, 6));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }
        
        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.mana = 10;
            Item.width = 30;
            Item.height = 30;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item30;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<IceClasperMinion>();
            Item.DamageType = DamageClass.Summon;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int clasper = Projectile.NewProjectile(source, Main.MouseWorld, new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)), type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(clasper))
                Main.projectile[clasper].originalDamage = Item.damage;

            return false;
        }
    }
}

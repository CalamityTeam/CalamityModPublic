using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class AncientIceChunk : ModItem, ILocalizedModType
    {
        #region Other Stats

        public static int IFrames = 20;
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
            Item.DamageType = DamageClass.Summon;
            Item.shoot = ModContent.ProjectileType<IceClasperMinion>();
            Item.knockBack = 2f;

            Item.useTime = Item.useAnimation = 25;
            Item.mana = 10;
            Item.width = 38;
            Item.height = 50;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item30;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectileDirect(source, Main.MouseWorld, Main.rand.NextVector2Circular(1f, 1f), type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}

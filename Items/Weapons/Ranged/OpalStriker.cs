using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class OpalStriker : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public static readonly SoundStyle Charge = new("CalamityMod/Sounds/Item/OpalCharge") { Volume = 0.5f};
        public static readonly SoundStyle ChargeLoop = new("CalamityMod/Sounds/Item/OpalChargeLoop") { Volume = 0.5f};
        internal static readonly int ChargeLoopSoundFrames = 120;
        public static readonly SoundStyle Fire = new("CalamityMod/Sounds/Item/OpalFire") { PitchVariance = 0.4f, Volume = 0.3f };
        public static readonly SoundStyle ChargedFire = new("CalamityMod/Sounds/Item/OpalChargedFire") { PitchVariance = 0.3f, Volume = 0.6f };

        public static int AftershotCooldownFrames = 17;
        public static int FullChargeFrames = 88;

        public override void SetDefaults()
        {
            Item.damage = 37;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 46;
            Item.height = 30;
            Item.useTime = Item.useAnimation = AftershotCooldownFrames;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = null;
            Item.autoReuse = false;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<OpalStrike>();
            Item.shootSpeed = 12f;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-15, 0);
        public override void HoldItem(Player player)
        {
            Item.noUseGraphic = true;
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<OpalStrikerHoldout>()] <= 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 shootVelocity = velocity;
            Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
            Projectile.NewProjectile(source, position, shootDirection, ModContent.ProjectileType<OpalStrikerHoldout>(), damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Marble, 25).
                AddIngredient(ItemID.MeteoriteBar, 10).
                AddIngredient(ItemID.Diamond, 3).
                AddIngredient(ItemID.Amber, 3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}

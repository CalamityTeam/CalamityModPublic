using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Condemnation : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public static int MaxLoadedArrows = 9;

        public override void SetDefaults()
        {
            Item.width = 130;
            Item.height = 42;
            Item.damage = 2130;
            Item.DamageType = DamageClass.Ranged;
            Item.useAnimation = Item.useTime = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<CalamityRed>();
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CondemnationArrow>();
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Arrow;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-50f, 0f);

        public override void HoldItem(Player player)
        {
            var calPlayer = player.Calamity();

            if (Main.myPlayer == player.whoAmI)
            {
                calPlayer.mouseRotationListener = true;
            }
            Item.noUseGraphic = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<CondemnationHoldout>()] <= 0;

        // Spawning the holdout cannot consume ammo
        public override bool CanConsumeAmmo(Item ammo, Player player) => player.altFunctionUse == 2f || player.ownedProjectileCounts[ModContent.ProjectileType<CondemnationHoldout>()] > 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 shootVelocity = velocity;
            Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
            Projectile.NewProjectile(source, position, shootDirection, ModContent.ProjectileType<CondemnationHoldout>(), 0, 0f, player.whoAmI);
            return false;
        }
    }
}

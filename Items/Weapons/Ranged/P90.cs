using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Ranged;
using Terraria.Audio;
using CalamityMod.Projectiles.Turret;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class P90 : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public bool fireShot = true;
        public override void SetDefaults()
        {
            Item.damage = 5;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 60;
            Item.height = 28;
            Item.useTime = Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item11 with { Volume = 0.6f};
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<P90Round>();
            Item.shootSpeed = 9f;
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-14, -1);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (fireShot)
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.03f), ModContent.ProjectileType<P90Round>(), damage, knockback, player.whoAmI, 0f, 0f);
            fireShot = !fireShot;
            return false;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => fireShot && Main.rand.NextFloat() < 0.25f;
    }
}

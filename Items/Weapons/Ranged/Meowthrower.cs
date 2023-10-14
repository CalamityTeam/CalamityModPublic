using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Meowthrower : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public override void SetDefaults()
        {
            Item.damage = 37;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 74;
            Item.height = 24;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.25f;
            Item.UseSound = SoundID.Item34;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MeowFire>();
            Item.shootSpeed = 8f;
            Item.useAmmo = AmmoID.Gel;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.Next(100) >= 85;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 newVel = velocity.RotatedByRandom(MathHelper.ToRadians(8f));
                Projectile.NewProjectile(source, position, newVel, type, damage, knockback, player.whoAmI, i);
            }
            return false;
        }
    }
}

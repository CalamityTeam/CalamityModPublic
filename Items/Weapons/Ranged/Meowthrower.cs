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

        public int CatCounter = 0;

        public override void SetDefaults()
        {
            Item.damage = 37;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 74;
            Item.height = 24;
            Item.useTime = 8;
            Item.useAnimation = 32;
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
            Item.consumeAmmoOnFirstShotOnly = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Fires a homing cat... thing
            CatCounter++;
            if (CatCounter >= 4)
            {
                CatCounter = 0;
                // They prefer to avoid the smokes
                Vector2 newPos = position + velocity.SafeNormalize(Vector2.UnitX) * 38f;
                Vector2 newVel = velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(6f, 15f) * Main.rand.NextBool().ToDirectionInt())) * 1.5f;
                Projectile.NewProjectile(source, newPos, newVel, ModContent.ProjectileType<MeowCreature>(), damage, knockback, player.whoAmI);
            }

            // Fires two flames, blue and pink, slightly randomly spread
            for (int i = 0; i < 2; i++)
            {
                Vector2 newVel = velocity.RotatedByRandom(MathHelper.ToRadians(6f));
                Projectile.NewProjectile(source, position, newVel, type, damage, knockback, player.whoAmI, i);
            }
            return false;
        }
    }
}

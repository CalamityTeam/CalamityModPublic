using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class EldritchTome : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.damage = 32;
            Item.DamageType = DamageClass.Magic;
            Item.crit = 5;
            Item.mana = 13;
            Item.useTime = 7;
            Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item103;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<EldritchTentacle>();
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 spreadVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(18f)) * Main.rand.NextFloat(0.8f, 1.2f);
            float tentacleYDirection = Main.rand.NextFloat(0.01f, 0.05f);
            if (Main.rand.NextBool())
                tentacleYDirection *= -1f;
            float tentacleXDirection = Main.rand.NextFloat(0.01f, 0.05f);
            if (Main.rand.NextBool())
                tentacleXDirection *= -1f;

            Projectile.NewProjectile(source, position, spreadVelocity, type, damage, knockback, Main.myPlayer, tentacleXDirection, tentacleYDirection);
            return false;
        }
    }
}

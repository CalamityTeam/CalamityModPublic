using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Sounds;

namespace CalamityMod.Items.Weapons.Magic
{
    public class SeethingDischarge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seething Discharge");
            Tooltip.SetDefault("Fires a barrage of brimstone blasts");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 58;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.width = 28;
            Item.height = 32;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6.75f;
            Item.UseSound = CommonCalamitySounds.FlareSound;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SeethingDischargeBrimstoneBarrage>();
            Item.shootSpeed = 6f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float SpeedX = velocity.X + 10f * 0.05f;
            float SpeedY = velocity.Y + 10f * 0.05f;
            float SpeedX2 = velocity.X - 10f * 0.05f;
            float SpeedY2 = velocity.Y - 10f * 0.05f;
            float SpeedX3 = velocity.X + 0f * 0.05f;
            float SpeedY3 = velocity.Y + 0f * 0.05f;
            Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI, 0f, 0f);
            Projectile.NewProjectile(source, position.X, position.Y, SpeedX2, SpeedY2, ModContent.ProjectileType<SeethingDischargeBrimstoneHellblast>(), damage, knockback, player.whoAmI, 0f, 0f);
            Projectile.NewProjectile(source, position.X, position.Y, SpeedX3, SpeedY3, type, damage, knockback, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}

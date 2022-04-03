using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Weapons.Magic
{
    public class SeethingDischarge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seething Discharge");
            Tooltip.SetDefault("Fires a barrage of brimstone blasts");
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
            Item.UseSound = Mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/FlareSound");
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SeethingDischargeBrimstoneBarrage>();
            Item.shootSpeed = 6f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float SpeedX = speedX + 10f * 0.05f;
            float SpeedY = speedY + 10f * 0.05f;
            float SpeedX2 = speedX - 10f * 0.05f;
            float SpeedY2 = speedY - 10f * 0.05f;
            float SpeedX3 = speedX + 0f * 0.05f;
            float SpeedY3 = speedY + 0f * 0.05f;
            Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            Projectile.NewProjectile(position.X, position.Y, SpeedX2, SpeedY2, ModContent.ProjectileType<SeethingDischargeBrimstoneHellblast>(), damage, knockBack, player.whoAmI, 0f, 0f);
            Projectile.NewProjectile(position.X, position.Y, SpeedX3, SpeedY3, type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}

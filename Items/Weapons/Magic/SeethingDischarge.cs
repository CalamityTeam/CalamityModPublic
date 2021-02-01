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
            item.damage = 58;
            item.magic = true;
            item.mana = 15;
            item.width = 28;
            item.height = 32;
            item.useTime = 24;
            item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6.75f;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/FlareSound");
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SeethingDischargeBrimstoneBarrage>();
            item.shootSpeed = 6f;
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

using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Waywasher : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Waywasher");
            Tooltip.SetDefault("Casts inaccurate water bolts");
        }

        public override void SetDefaults()
        {
            item.damage = 16;
            item.magic = true;
            item.mana = 4;
            item.width = 30;
            item.height = 30;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2.5f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item8;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<WaywasherProj>();
            item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float SpeedX = speedX + (float)Main.rand.Next(-20, 21) * 0.05f;
            float SpeedY = speedY + (float)Main.rand.Next(-20, 21) * 0.05f;
            Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<WaywasherProj>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

    }
}

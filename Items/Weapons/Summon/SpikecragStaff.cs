using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class SpikecragStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spikecrag Staff");
            Tooltip.SetDefault("Summons a spikecrag to protect you");
        }

        public override void SetDefaults()
        {
            item.damage = 56;
            item.mana = 10;
            item.summon = true;
            item.sentry = true;
            item.width = 50;
            item.height = 50;
            item.useTime = item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.autoReuse = true;
            item.shootSpeed = 20f;
            item.UseSound = SoundID.Item78;
            item.shoot = ModContent.ProjectileType<Spikecrag>();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            //CalamityUtils.OnlyOneSentry(player, type);
            Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI, 120f, 0f);
            player.UpdateMaxTurrets();
            return false;
        }
    }
}

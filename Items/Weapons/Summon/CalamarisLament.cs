using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class CalamarisLament : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Calamari's Lament");
            Tooltip.SetDefault("Summons a squid to fight for you");
        }

        public override void SetDefaults()
        {
            item.damage = 103;
            item.mana = 10;
            item.width = 62;
            item.height = 62;
            item.useTime = item.useAnimation = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 2.5f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item83;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<CalamariMinion>();
            item.shootSpeed = 10f;
            item.summon = true;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                position = Main.MouseWorld;
                speedX = 0;
                speedY = 0;
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}

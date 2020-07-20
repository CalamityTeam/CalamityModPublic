using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class DeepseaStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deepsea Staff");
            Tooltip.SetDefault("Summons an aquatic star to fight for you");
        }

        public override void SetDefaults()
        {
            item.damage = 42;
            item.mana = 10;
            item.width = 46;
            item.height = 46;
            item.useTime = item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.UseSound = SoundID.Item44;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<AquaticStarMinion>();
            item.shootSpeed = 10f;
            item.summon = true;
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

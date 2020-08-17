using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class StormjawStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stormjaw Staff");
            Tooltip.SetDefault("Summons a baby stormlion to fight for you");
        }

        public override void SetDefaults()
        {
            item.damage = 8;
            item.mana = 10;
            item.width = 32;
            item.height = 32;
            item.useTime = item.useAnimation = 35;
            item.scale = 0.75f;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.UseSound = SoundID.NPCDeath34;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<StormjawBaby>();
            item.shootSpeed = 10f;
            item.summon = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}

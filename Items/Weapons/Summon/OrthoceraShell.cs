using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class OrthoceraShell : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orthocera Shell");
            Tooltip.SetDefault("Summons a flying orthocera sentry at the mouse position");
        }

        public override void SetDefaults()
        {
            item.damage = 53;
            item.mana = 10;
            item.width = 34;
            item.height = 34;
            item.useTime = item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.UseSound = SoundID.Item42;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<FlyingOrthocera>();
            item.shootSpeed = 0f;
            item.summon = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI, 0f, 1f);
                    player.UpdateMaxTurrets();
                }
            }
            return false;
        }
    }
}

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
            Item.damage = 53;
            Item.mana = 10;
            Item.width = 34;
            Item.height = 34;
            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item42;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FlyingOrthocera>();
            Item.shootSpeed = 0f;
            Item.DamageType = DamageClass.Summon;
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

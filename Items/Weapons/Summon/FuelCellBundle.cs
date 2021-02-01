using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Weapons.Summon
{
    public class FuelCellBundle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fuel Cell Bundle");
            Tooltip.SetDefault("Releases a small, special variant of the plaguebringers");
        }

        public override void SetDefaults()
        {
            item.mana = 10;
            item.damage = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.width = 32;
            item.height = 32;
            item.useTime = item.useAnimation = 20;
            item.noMelee = true;
            item.knockBack = 7f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.UseSound = SoundID.Item106;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.shoot = ModContent.ProjectileType<PlaguebringerMK2>(); //not the flask, so this weapon works w/ minion targetting
            item.shootSpeed = 11f;
            item.summon = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2) //throws a flask
            {
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<MK2FlaskSummon>(), damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}

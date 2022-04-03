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
            Item.mana = 10;
            Item.damage = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = Item.useAnimation = 20;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.value = Item.buyPrice(0, 80, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item106;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<PlaguebringerMK2>(); //not the flask, so this weapon works w/ minion targetting
            Item.shootSpeed = 11f;
            Item.DamageType = DamageClass.Summon;
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

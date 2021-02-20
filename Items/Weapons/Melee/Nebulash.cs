using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Nebulash : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nebulash");
            Tooltip.SetDefault("Fires a whip sword that emits particle explosions on hit");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.damage = 60;
            item.rare = ItemRarityID.Lime;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.autoReuse = true;
            item.melee = true;
            item.useAnimation = 18;
            item.useTime = 18;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 2f;
            item.UseSound = SoundID.Item117;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.shootSpeed = 24f;
            item.shoot = ModContent.ProjectileType<NebulashFlail>();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float ai3 = (Main.rand.NextFloat() - 0.5f) * 0.7853982f; //0.5
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, ai3);
            return false;
        }
    }
}

using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Miasma : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Miasma");
            Tooltip.SetDefault("Releases gas clouds that stay still and explode after a while");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 123;
            item.magic = true;
            item.mana = 16;
            item.width = 50;
            item.height = 64;
            item.useTime = item.useAnimation = 27;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.UseSound = SoundID.Item8;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<MiasmaGas>();
            item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = 0; i < Main.rand.Next(5, 8 + 1); i++)
            {
                Vector2 velocity = new Vector2(speedX, speedY) * Main.rand.NextFloat(0.9f, 1.1f);
                float angle = Main.rand.NextFloat(-1f, 1f) * MathHelper.ToRadians(23f);
                Projectile.NewProjectile(position, velocity.RotatedBy(angle), type, damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}

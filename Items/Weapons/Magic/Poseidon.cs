using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Poseidon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Poseidon");
            Tooltip.SetDefault("Casts a poseidon typhoon");
        }

        public override void SetDefaults()
        {
            item.damage = 58;
            item.magic = true;
            item.mana = 15;
            item.width = 28;
            item.height = 32;
            item.useTime = 22;
            item.useAnimation = 22;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 6f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.UseSound = SoundID.Item84;
            item.rare = 5;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<PoseidonTyphoon>();
            item.shootSpeed = 10f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			int typhoon = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 1f);
			Main.projectile[typhoon].penetrate = Main.rand.Next(4,11);
			return false;
        }
    }
}

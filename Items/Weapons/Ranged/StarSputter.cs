using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class StarSputter : ModItem
    {
        private int counter = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Sputter");
            Tooltip.SetDefault("Fires a chain of comets\n" +
            "Fires a bigger, more powerful comet every four rounds\n" +
            "Look to the stars for a galaxy far, far away");
        }

        public override void SetDefaults()
        {
            item.damage = 112;
            item.ranged = true;
            item.width = 80;
            item.height = 26;
            item.useTime = 8;
            item.reuseDelay = 15;
            item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 15f;
            item.value = Item.buyPrice(0, 95, 0, 0);
            item.rare = ItemRarityID.Cyan;
            item.UseSound = SoundID.Item92;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SputterComet>();
            item.shootSpeed = 15f;
            item.useAmmo = AmmoID.FallenStar;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool ConsumeAmmo(Player player) //consume ammo only once per round
        {
            if (counter == 1 || counter == 2 || counter == 4 || counter == 5 || counter == 7 || counter == 8 || counter == 10 || counter == 11)
                return false;
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            counter++;
            if (counter == 10)
            {
                Projectile.NewProjectile(position.X, position.Y, speedX * 0.8f, speedY * 0.8f, ModContent.ProjectileType<SputterCometBig>(), (int)(damage * 1.5f), knockBack, player.whoAmI, 0f, 0f);
            }
            if (counter >= 12)
                counter = 0;
            return true;
        }
    }
}

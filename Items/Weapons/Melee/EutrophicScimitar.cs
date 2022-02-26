using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class EutrophicScimitar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eutrophic Scimitar");
            Tooltip.SetDefault("Fires a beam upon swing that stuns enemies");
        }

        public override void SetDefaults()
        {
            item.damage = 110;
            item.melee = true;
            item.width = 64;
            item.height = 78;
            item.useTime = 40;
            item.useAnimation = 40;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 2;
            item.shoot = ModContent.ProjectileType<EutrophicScimitarProj>();
            item.shootSpeed = 17;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = ItemRarityID.LightRed;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.useTurn = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = 0; i <= 21; i++)
            {
                Dust dust;
                dust = Main.dust[Dust.NewDust(new Vector2(position.X - 58 / 2, position.Y - 58 / 2), 58, 58, 226, 0f, 0f, 0, new Color(255, 255, 255), 0.4605263f)];
                dust.noGravity = true;
                dust.fadeIn = 0.9473684f;
            }

			for (int projectiles = 0; projectiles < 2; projectiles++)
			{
				float SpeedX = speedX + Main.rand.Next(-15, 16) * 0.05f;
				float SpeedY = speedY + Main.rand.Next(-15, 16) * 0.05f;
				Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, (int)(damage * 0.7), knockBack, player.whoAmI);
			}

			return false;
        }
    }
}

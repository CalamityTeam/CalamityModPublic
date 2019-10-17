using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items
{
    public class UtensilPoker : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Utensil Poker");
            Tooltip.SetDefault("Fires random utensils in bursts of three\n" +
                "Grants Well Fed on enemy hits\n" +
                "Stealth strikes launch an additional butcher knife");
        }

        public override void SafeSetDefaults()
        {
            item.width = 44;
            item.height = 66;
            item.damage = 500;
            item.Calamity().rogue = true;
            item.knockBack = 8f;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useStyle = 1;
            item.useTime = 15;
            item.reuseDelay = 15;
            item.useAnimation = 45;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 12;
            item.shoot = ModContent.ProjectileType<Projectiles.Fork>();
            item.shootSpeed = 16f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(position.X, position.Y, (int)((double)speedX * 1.2), (int)((double)speedY * 1.2), ModContent.ProjectileType<Projectiles.ButcherKnife>(), (int)((double)damage * 3), knockBack, Main.myPlayer);
                Main.projectile[p].Calamity().stealthStrike = true;
                if (Main.rand.NextBool(3))
                {
                    Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<Projectiles.Fork>(), (int)((double)damage * 1.1), knockBack * 2f, Main.myPlayer);
                }
                else if (Main.rand.NextBool(2))
                {
                    Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<Projectiles.Knife>(), (int)((double)damage * 1.2), knockBack, Main.myPlayer);
                }
                else
                {
                    Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<Projectiles.CarvingFork>(), damage, knockBack, Main.myPlayer);
                }
            }
            else if (Main.rand.NextBool(3))
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<Projectiles.Fork>(), (int)((double)damage * 1.1), knockBack * 2f, Main.myPlayer);
            }
            else if (Main.rand.NextBool(2))
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<Projectiles.Knife>(), (int)((double)damage * 1.2), knockBack, Main.myPlayer);
            }
            else
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<Projectiles.CarvingFork>(), damage, knockBack, Main.myPlayer);
            }
            return false;
        }
    }
}

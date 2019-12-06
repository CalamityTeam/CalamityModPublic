using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class LeviathanTeeth : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Leviathan Teeth");
            Tooltip.SetDefault("Rapidly throws a variety of poisonous fangs that stick to enemies\n" +
                "Stealth strikes cause 3 very fast teeth to be thrown, ignoring gravity and inflicting extreme knockback");
        }

        public override void SafeSetDefaults()
        {
            item.width = 36;
            item.damage = 70;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 10;
            item.useStyle = 1;
            item.useTime = 10;
            item.knockBack = 1f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 38;
            item.maxStack = 1;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
            item.shoot = ModContent.ProjectileType<LeviathanTooth>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            bool stealthStrike = false;
            int teethCount;
            if (player.Calamity().StealthStrikeAvailable())
            {
                teethCount = 3;
                stealthStrike = true;
            }
            else
            {
                teethCount = Main.rand.Next(1, 4);
            }

            float spreadAngle = MathHelper.ToRadians(10);

            for (int i = 0; i < teethCount; i++)
            {
                float toothType = Main.rand.Next(0, 3);
                int projectileType = type;
                switch (toothType)
                {
                    case 1:
                        projectileType = ModContent.ProjectileType<LeviathanTooth2>();
                        break;
                    case 2:
                        projectileType = ModContent.ProjectileType<LeviathanTooth3>();
                        break;
                }

                float offsetSpeedX = speedX + Main.rand.NextFloat(-4f, 4f);
                float offsetSpeedY = speedY + Main.rand.NextFloat(-4f, 4f);

                if (stealthStrike)
                {
                    int p = Projectile.NewProjectile(position.X, position.Y, offsetSpeedX * 1.5f, offsetSpeedY * 1.5f, projectileType, damage, 10f, player.whoAmI, 0f, 0f);
                    Main.projectile[p].Calamity().stealthStrike = true;
                }
                else
                {
                    int p = Projectile.NewProjectile(position.X, position.Y, offsetSpeedX, offsetSpeedY, projectileType, damage, knockBack, player.whoAmI, 0f, 0f);
                }
            }
            return false;
        }
    }
}

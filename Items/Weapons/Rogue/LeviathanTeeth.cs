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
            Item.width = 36;
            Item.damage = 50;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.knockBack = 1f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 38;
            Item.maxStack = 1;
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.shoot = ModContent.ProjectileType<LeviathanTooth>();
            Item.shootSpeed = 12f;
            Item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            bool stealthStrike = false;
            int teethCount;
            if (player.Calamity().StealthStrikeAvailable())
            {
                teethCount = 3;
                stealthStrike = true;
                damage = (int)(damage * 1.3f);
            }
            else
            {
                teethCount = Main.rand.Next(1, 4);
            }

            float spreadAngle = MathHelper.ToRadians(10);

            for (int i = 0; i < teethCount; i++)
            {
                float offsetSpeedX = speedX + Main.rand.NextFloat(-4f, 4f);
                float offsetSpeedY = speedY + Main.rand.NextFloat(-4f, 4f);

                if (stealthStrike)
                {
                    int tooth = Projectile.NewProjectile(position.X, position.Y, offsetSpeedX * 1.5f, offsetSpeedY * 1.5f, type, damage, knockBack * 10f, player.whoAmI);
                    if (tooth.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[tooth].Calamity().stealthStrike = true;
                        Main.projectile[tooth].Calamity().lineColor = Main.rand.Next(3);
                    }
                }
                else
                {
                    int tooth = Projectile.NewProjectile(position.X, position.Y, offsetSpeedX, offsetSpeedY, type, damage, knockBack, player.whoAmI);
                    if (tooth.WithinBounds(Main.maxProjectiles))
                        Main.projectile[tooth].Calamity().lineColor = Main.rand.Next(3);
                }
            }
            return false;
        }
    }
}

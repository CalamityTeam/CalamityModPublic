using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class TheReaper : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Reaper");
            Tooltip.SetDefault("Slice 'n dice\n" +
			"Stealth strikes throw four at once");
        }

        public override void SafeSetDefaults()
        {
            item.width = 80;
            item.damage = 325;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 22;
            item.useTime = 22;
            item.useStyle = 1;
            item.knockBack = 4f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 64;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<ReaperProjectile>();
            item.shootSpeed = 20f;
            item.Calamity().rogue = true;
            item.Calamity().customRarity = CalamityRarity.RareVariant;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int spread = 10;
                for (int i = 0; i < 4; i++)
                {
                    Vector2 perturbedspeed = new Vector2(speedX + Main.rand.Next(-3,4), speedY + Main.rand.Next(-3,4)).RotatedBy(MathHelper.ToRadians(spread));
                    int proj = Projectile.NewProjectile(position.X, position.Y, perturbedspeed.X, perturbedspeed.Y, type, damage / 2, knockBack, player.whoAmI, 0f, 0f);
                    Main.projectile[proj].Calamity().stealthStrike = true;
                    Main.projectile[proj].penetrate = 6;
                    spread -= Main.rand.Next(5,8);
                }
                return false;
            }
            return true;
        }
    }
}

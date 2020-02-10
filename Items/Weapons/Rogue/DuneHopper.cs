using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class DuneHopper : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dune Hopper");
            Tooltip.SetDefault(@"Throws a spear that bounces a lot
Stealth strikes throws three high speed spears");
        }

        public override void SafeSetDefaults()
        {
            item.width = 44;
            item.damage = 18;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 22;
            item.useStyle = 1;
            item.useTime = 22;
            item.knockBack = 4f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 44;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.shoot = ModContent.ProjectileType<DuneHopperProjectile>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
            item.Calamity().customRarity = CalamityRarity.RareVariant;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int numProj = 2;
                float rotation = MathHelper.ToRadians(3);
                for (int i = 0; i < numProj + 1; i++)
                {
                    Vector2 perturbedSpeed = new Vector2(speedX - 3f, speedY - 3f).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                    Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<DuneHopperProjectile>(), damage, knockBack, player.whoAmI, 0f, 0f);
                }
                return false;
            }
            return true;
        }
    }
}

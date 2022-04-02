using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class CursedDagger : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Dagger");
            Tooltip.SetDefault("Throws bouncing daggers\n" +
            "Stealth strikes are showered in cursed fireballs");
        }

        public override void SafeSetDefaults()
        {
            item.width = 34;
            item.damage = 34;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 16;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 16;
            item.knockBack = 4.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 34;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<CursedDaggerProj>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                    Main.projectile[stealth].usesLocalNPCImmunity = true;
                }
                return false;
            }
            return true;
        }
    }
}

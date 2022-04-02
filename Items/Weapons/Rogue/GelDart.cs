using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class GelDart : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gel Dart");
            Tooltip.SetDefault("Throws bouncing darts\n" +
            "Stealth strikes ignore gravity and bounce more vigorously\n" +
            "They additionally leak slime and cover enemies in dark sludge");
        }

        public override void SafeSetDefaults()
        {
            item.width = 14;
            item.damage = 28;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 11;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 11;
            item.knockBack = 2.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 28;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 0, 2, 50);
            item.rare = ItemRarityID.LightRed;
            item.shoot = ModContent.ProjectileType<GelDartProjectile>();
            item.shootSpeed = 14f;
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
                    Main.projectile[stealth].penetrate = 6;
                    Main.projectile[stealth].aiStyle = -1;
                }
                return false;
            }
            return true;
        }
    }
}

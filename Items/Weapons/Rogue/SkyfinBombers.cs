using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SkyfinBombers : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skyfin Bombers");
            Tooltip.SetDefault("Fishy bombers inbound!\n" +
            "Launches a skyfin nuke that homes in on enemies below it\n" +
            "Stealth strikes rapidly home in regardless of enemy position");
        }

        public override void SafeSetDefaults()
        {
            item.width = 28;
            item.damage = 46;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 35;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 35;
            item.knockBack = 6.5f;
            item.UseSound = SoundID.Item11;
            item.autoReuse = true;
            item.height = 30;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<SkyfinNuke>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}

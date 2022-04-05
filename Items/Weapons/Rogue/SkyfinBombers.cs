using Terraria.DataStructures;
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
            Item.width = 28;
            Item.damage = 46;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 35;
            Item.knockBack = 6.5f;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.height = 30;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<SkyfinNuke>();
            Item.shootSpeed = 12f;
            Item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}

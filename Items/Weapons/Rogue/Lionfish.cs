using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Lionfish : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lionfish");
            Tooltip.SetDefault("Sticks to enemies and injects a potent toxin\n" +
            "Stealth strikes are trailed by homing urchin spikes");
        }

        public override void SafeSetDefaults()
        {
            item.width = 40;
            item.damage = 45;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 22;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 22;
            item.knockBack = 2.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 40;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.shoot = ModContent.ProjectileType<LionfishProj>();
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

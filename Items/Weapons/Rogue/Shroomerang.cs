using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Shroomerang : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shroomerang");
            Tooltip.SetDefault(@"Fires a slow, long-ranged boomerang
Stealth strikes grant the Mushy buff to the user on enemy hits and summon homing spores");
        }

        public override void SafeSetDefaults()
        {
            item.width = 26;
            item.damage = 18;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 1.5f;
            item.UseSound = SoundID.Item1;
            item.height = 50;
            item.rare = ItemRarityID.Green;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.shoot = ModContent.ProjectileType<ShroomerangProj>();
            item.shootSpeed = 15f;
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

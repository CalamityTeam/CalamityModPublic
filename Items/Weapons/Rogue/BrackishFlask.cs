using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class BrackishFlask : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brackish Flask");
            Tooltip.SetDefault("Explodes into poisonous seawater blasts\n" +
            "Stealth strikes summon a brackish spear spike");
        }

        public override void SafeSetDefaults()
        {
            item.width = 28;
            item.damage = 60;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 35;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 35;
            item.knockBack = 6.5f;
            item.UseSound = SoundID.Item106;
            item.autoReuse = true;
            item.height = 30;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.shoot = ModContent.ProjectileType<BrackishFlaskProj>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, (int)(damage * 0.85f), knockBack, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}

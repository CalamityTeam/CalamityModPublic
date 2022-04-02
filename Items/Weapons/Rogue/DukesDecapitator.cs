using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class DukesDecapitator : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Duke's Decapitator");
            Tooltip.SetDefault("Throws a hydro axe which shreds enemies when it comes into contact with them\n"
                              +"The faster it’s spinning, the more times it hits before disappearing\n"
                              +"Stealth Strikes make it emit short-ranged bubbles.");
        }

        public override void SafeSetDefaults()
        {
            item.width = 60;
            item.height = 64;
            item.damage = 90;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 30;
            item.knockBack = 2f;
            item.UseSound = SoundID.Item1;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.shoot = ModContent.ProjectileType<DukesDecapitatorProj>();
            item.shootSpeed = 15f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
                damage = (int)(damage * 1.2f);

            int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            if (Main.projectile.IndexInRange(proj))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }
    }
}

using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Icebreaker : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Icebreaker");
            Tooltip.SetDefault("Stealth strikes spawn a cosmic explosion and freeze nearby enemies on enemy hits");
        }

        public override void SafeSetDefaults()
        {
            item.width = 60;
            item.damage = 60;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 14;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 14;
            item.knockBack = 6.75f;
            item.UseSound = SoundID.Item1;
            item.melee = true;
            item.height = 60;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<IcebreakerHammer>();
            item.shootSpeed = 16f;
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

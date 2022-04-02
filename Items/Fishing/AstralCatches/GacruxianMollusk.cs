using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.AstralCatches
{
    public class GacruxianMollusk : RogueWeapon
    {
        public static int BaseDamage = 36;
        public static float Knockback = 5f;
        public static float Speed = 15f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gacruxian Mollusk");
            Tooltip.SetDefault("Releases homing sparks while traveling\n" +
            "Stealth strikes release homing snails that create even more sparks");
        }

        public override void SafeSetDefaults()
        {
            item.damage = BaseDamage;
            item.knockBack = Knockback;
            item.autoReuse = true;
            item.useTime = 26;
            item.useAnimation = 26;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.width = 24;
            item.height = 22;
            item.UseSound = SoundID.Item1;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.shoot = ModContent.ProjectileType<GacruxianProj>();
            item.shootSpeed = Speed;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = ItemRarityID.LightRed;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<GacruxianProj>(), damage, knockBack, player.whoAmI, 0f, 1f);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}

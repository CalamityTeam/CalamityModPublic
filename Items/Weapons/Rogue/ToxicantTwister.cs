using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class ToxicantTwister : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Toxicant Twister");
            Tooltip.SetDefault("Releases a fast boomerang\n" +
                               "Stealth strikes cause the boomerang to go much faster and release sand rapidly");
        }

        public override void SafeSetDefaults()
        {
            item.width = 42;
            item.height = 46;
            item.damage = 400;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = item.useTime = 30;
            item.useStyle = 1;
            item.knockBack = 5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.shoot = ModContent.ProjectileType<ToxicantTwisterProjectile>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f);

            Main.projectile[stealth].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            if (Main.projectile[stealth].Calamity().stealthStrike)
            {
                Main.projectile[stealth].velocity *= 1.5f;
                Main.projectile[stealth].timeLeft = 420;
                Main.projectile[stealth].penetrate = -1;
                Main.projectile[stealth].usesLocalNPCImmunity = true;
                Main.projectile[stealth].localNPCHitCooldown = 6;
            }
            return false;
        }
    }
}

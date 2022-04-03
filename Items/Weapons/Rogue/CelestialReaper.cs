using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class CelestialReaper : RogueWeapon
    {
        public const int BaseDamage = 140;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Celestial Reaper");
            Tooltip.SetDefault("Throws a fast homing scythe\n" +
                               "The scythe will bounce after hitting an enemy up to six times\n" +
                               "Stealth strikes create damaging afterimages");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = BaseDamage;
            Item.width = 66;
            Item.height = 76;
            Item.useAnimation = 31;
            Item.useTime = 31;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6f;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item71;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(platinum: 1); //sell price of 20 gold
            Item.shoot = ModContent.ProjectileType<CelestialReaperProjectile>();
            Item.shootSpeed = 20f;
            Item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            bool usingStealth = player.Calamity().StealthStrikeAvailable();
            float strikeValue = usingStealth.ToInt(); //0 if false, 1 if true

            Vector2 velocity = new Vector2(speedX, speedY);
            // Directly nerf stealth strikes by 10%, but only stealth strikes.
            int finalDamage = (int)(damage * (usingStealth ? 0.9f : 1.0f));
            int p = Projectile.NewProjectile(position, velocity, ModContent.ProjectileType<CelestialReaperProjectile>(), finalDamage, knockBack, player.whoAmI, strikeValue);
            if (player.Calamity().StealthStrikeAvailable() && p.WithinBounds(Main.maxProjectiles))
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }
    }
}

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
            item.damage = BaseDamage;
            item.width = 66;
            item.height = 76;
            item.useAnimation = 31;
            item.useTime = 31;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6f;
            item.rare = ItemRarityID.Red;
            item.UseSound = SoundID.Item71;
            item.autoReuse = true;
            item.value = Item.buyPrice(platinum: 1); //sell price of 20 gold
            item.shoot = ModContent.ProjectileType<CelestialReaperProjectile>();
            item.shootSpeed = 20f;
            item.Calamity().rogue = true;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 velocity = new Vector2(speedX, speedY);
            float strikeValue = player.Calamity().StealthStrikeAvailable().ToInt(); //0 if false, 1 if true
            int p = Projectile.NewProjectile(position, velocity, ModContent.ProjectileType<CelestialReaperProjectile>(), damage, knockBack, player.whoAmI, strikeValue);
            if (player.Calamity().StealthStrikeAvailable() && p.WithinBounds(Main.maxProjectiles))
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }
    }
}

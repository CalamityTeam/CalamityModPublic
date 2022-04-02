using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class RefractionRotor : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Refraction Rotor");
            Tooltip.SetDefault("Fires a huge prismatic disk shuriken\n" +
                "The shuriken shatters moments after impact into homing rockets\n" +
                "Stealth strikes shatter into many more rockets");
        }

        public override void SafeSetDefaults()
        {
            item.width = item.height = 120;
            item.damage = 616;
            item.knockBack = 8.5f;
            item.useAnimation = item.useTime = 17;
            item.Calamity().rogue = true;
            item.autoReuse = true;
            item.shootSpeed = 18f;
            item.shoot = ModContent.ProjectileType<RefractionRotorProjectile>();

            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
                damage = (int)(damage * 0.75D);

            int shuriken = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && Main.projectile.IndexInRange(shuriken))
                Main.projectile[shuriken].Calamity().stealthStrike = true;
            return false;
        }
    }
}

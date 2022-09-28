using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 120;
            Item.damage = 616;
            Item.knockBack = 8.5f;
            Item.useAnimation = Item.useTime = 17;
            Item.DamageType = RogueDamageClass.Instance;
            Item.autoReuse = true;
            Item.shootSpeed = 18f;
            Item.shoot = ModContent.ProjectileType<RefractionRotorProjectile>();

            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<Violet>();
        }

		public override float StealthDamageMultiplier => 0.75f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int shuriken = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && Main.projectile.IndexInRange(shuriken))
                Main.projectile[shuriken].Calamity().stealthStrike = true;
            return false;
        }
    }
}

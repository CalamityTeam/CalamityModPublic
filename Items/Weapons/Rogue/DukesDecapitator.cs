using Terraria.DataStructures;
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
                              +"The faster it's spinning, the more times it hits before disappearing\n"
                              +"Stealth Strikes make it emit short-ranged bubbles.");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 64;
            Item.damage = 90;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 30;
            Item.knockBack = 2f;
            Item.UseSound = SoundID.Item1;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<DukesDecapitatorProj>();
            Item.shootSpeed = 15f;
            Item.DamageType = RogueDamageClass.Instance;
        }

		public override float StealthDamageMultiplier => 1.2f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(proj))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }
    }
}

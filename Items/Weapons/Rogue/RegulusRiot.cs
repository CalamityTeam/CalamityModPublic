using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class RegulusRiot : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Regulus Riot");
            Tooltip.SetDefault(@"Fires a swift homing disk
Stealth strikes explode into energy stars");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 116;
            Item.knockBack = 4.5f;

            Item.width = 28;
            Item.height = 34;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Cyan;
            Item.DamageType = RogueDamageClass.Instance;

            Item.autoReuse = true;
            Item.shootSpeed = 8f;
            Item.shoot = ModContent.ProjectileType<RegulusRiotProj>();
        }

		public override float StealthDamageMultiplier => 1.2f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}

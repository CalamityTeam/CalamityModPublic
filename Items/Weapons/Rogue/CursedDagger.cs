using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class CursedDagger : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.damage = 34;
            Item.useAnimation = Item.useTime = 8;
            Item.shootSpeed = 12f;
            Item.knockBack = 4.5f;

            Item.shoot = ModContent.ProjectileType<CursedDaggerProj>();

            Item.width = 14;
            Item.height = 48;
            Item.DamageType = RogueDamageClass.Instance;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Pink;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                    Main.projectile[stealth].usesLocalNPCImmunity = true;
                }
                return false;
            }
            return true;
        }
    }
}

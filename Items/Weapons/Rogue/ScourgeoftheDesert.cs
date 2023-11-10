using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class ScourgeoftheDesert : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 82;
            Item.height = 82;
            Item.damage = 16;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 24;
            Item.knockBack = 3.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<ScourgeoftheDesertProj>();
            Item.shootSpeed = 12f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int index = 5;
                for (int i = -index; i <= index; i += index)
                {
                    Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.ToRadians(i));
                    int stealth = Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
                    if (stealth.WithinBounds(Main.maxProjectiles))
                        Main.projectile[stealth].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
                }
                return false;
            }
            return true;
        }
    }
}

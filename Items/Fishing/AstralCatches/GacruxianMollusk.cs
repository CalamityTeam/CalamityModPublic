using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.AstralCatches
{
    // TODO -- This is a rogue weapon. It should be in Items/Weapons/Rogue. I do not care if it comes from fishing.
    public class GacruxianMollusk : ModItem
    {
        public static int BaseDamage = 36;
        public static float Knockback = 5f;
        public static float Speed = 15f;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Gacruxian Mollusk");
            Tooltip.SetDefault("Releases homing sparks while traveling\n" +
            "Stealth strikes release homing snails that create even more sparks");
        }

        public override void SetDefaults()
        {
            Item.DamageType = RogueDamageClass.Instance;
            Item.damage = BaseDamage;
            Item.knockBack = Knockback;
            Item.autoReuse = true;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.width = 24;
            Item.height = 22;
            Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<GacruxianProj>();
            Item.shootSpeed = Speed;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<GacruxianProj>(), damage, knockback, player.whoAmI, 0f, 1f);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}

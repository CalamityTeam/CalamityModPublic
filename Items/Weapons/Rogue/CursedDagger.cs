using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using log4net.Core;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class CursedDagger : RogueWeapon
    {
        public static readonly SoundStyle ThrowSound = new("CalamityMod/Sounds/Item/CursedDaggerThrow") { Volume = 0.3f, PitchVariance = 0.4f };
        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.useAnimation = Item.useTime = 18;
            Item.shootSpeed = 19f;
            Item.knockBack = 4.5f;

            Item.shoot = ModContent.ProjectileType<CursedDaggerProj>();

            Item.width = 14;
            Item.height = 48;
            Item.DamageType = RogueDamageClass.Instance;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = ThrowSound;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }
        public override float StealthDamageMultiplier => 0.75f;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 newVel = velocity.RotatedByRandom(MathHelper.ToRadians(23f)) * 0.6f;
            Vector2 newVel2 = velocity.RotatedByRandom(MathHelper.ToRadians(23f)) * 0.8f;
            if (!player.Calamity().StealthStrikeAvailable())
            {
                Projectile.NewProjectile(source, position, newVel, type, damage / 2, knockback, player.whoAmI);
                Projectile.NewProjectile(source, position, newVel2, type, damage / 2, knockback, player.whoAmI);
            }

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

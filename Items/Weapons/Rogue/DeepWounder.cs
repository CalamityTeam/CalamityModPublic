using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class DeepWounder : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deep Wounder");
            Tooltip.SetDefault("Throws an abyssal hatchet that inflicts Armor Crunch and Crush Depth to the enemies it hits\n" +
                "Stealth strikes cause the hatchet to be thrown faster and trail water, inflicting Marked for Death in addition to the other debuffs");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.damage = 106;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 23;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 48;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Lime;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.shoot = ModContent.ProjectileType<DeepWounderProjectile>();
            Item.shootSpeed = 14f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override float StealthVelocityMultiplier => 1.1f;
        public override float StealthDamageMultiplier => 1.5f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 1f);
                if (p.WithinBounds(Main.maxProjectiles))
                    Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}

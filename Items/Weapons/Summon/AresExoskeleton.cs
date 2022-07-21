using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Summon.SmallAresArms;

namespace CalamityMod.Items.Weapons.Summon
{
    public class AresExoskeleton : ModItem
    {
        public const int PlasmaCannonShootRate = 30;

        public const int TeslaCannonShootRate = 30;

        public const int LaserCannonNormalShootRate = 30;

        public const int GaussNukeShootRate = 210;

        public const float TargetingDistance = 1020f;

        public const float MinionSlotsPerCannon = 3f;

        public const float NukeDamageFactor = 2.7f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ares' Exoskeleton");
            Tooltip.SetDefault("Ares arms. STRONG cannons. BIG explosions. FUN");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.mana = 80;
            Item.damage = 972;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noUseGraphic = true;
            Item.width = Item.height = 56;
            Item.useTime = Item.useAnimation = 9;
            Item.noMelee = true;
            Item.knockBack = 1f;

            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Violet;

            Item.UseSound = SoundID.Item117;
            Item.shoot = ModContent.ProjectileType<ExoskeletonPlasmaCannon>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
            Item.Calamity().CannotBeEnchanted = true;
        }
        
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int panelID = ModContent.ProjectileType<ExoskeletonPanel>();

            // If the player owns a panel, make it fade away.
            if (player.ownedProjectileCounts[panelID] >= 1)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].type != panelID || Main.projectile[i].owner != player.whoAmI || !Main.projectile[i].active)
                        continue;

                    Main.projectile[i].ai[0] = 1f;
                    Main.projectile[i].netUpdate = true;
                }
            }

            // Otherwise, create one. While it doesn't do damage on its own, it does store it for reference by the cannons that might be spawned.
            else
            {
                int panel = Projectile.NewProjectile(source, position, Vector2.Zero, panelID, damage, 0f, player.whoAmI);
                if (Main.projectile.IndexInRange(panel))
                    Main.projectile[panel].originalDamage = Item.damage;
            }

            return false;
        }
    }
}

using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class Cosmilamp : ModItem
    {
        public const int BeamShootRate = 105;

        public const float MaxTargetingDistance = 1360f;

        public const float BeamHomeSpeed = 17f;

        public const float LanternSummonCost = 2f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmilamp");
            Tooltip.SetDefault("Summons a cosmic lantern above your head to fight for you\n" +
                $"Each lantern takes up {LanternSummonCost} minion slots");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 127;
            Item.mana = 10;
            Item.width = 42;
            Item.height = 60;
            Item.useTime = Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CosmilampMinion>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override bool CanUseItem(Player player) => player.maxMinions >= 2;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                // Reset the timer for all lamps, to re-align the formation.
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].type == type && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].active)
                    {
                        Main.projectile[i].ModProjectile<CosmilampMinion>().Timer = 0f;
                        Main.projectile[i].netUpdate = true;
                    }
                }

                int existingLamps = player.ownedProjectileCounts[type];
                int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
                if (Main.projectile.IndexInRange(p))
                {
                    Main.projectile[p].originalDamage = Item.damage;
                    Main.projectile[p].ai[0] = existingLamps;
                }
            }
            return false;
        }
    }
}

using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class BladecrestOathsword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bladecrest Oathsword");
            Tooltip.SetDefault("Fires bursts of demonic blades that exponentially decelerate and explode\n" +
                "Sword of an ancient demon lord");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 56;
            Item.damage = 25;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 21;
            Item.useTime = 21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.useTurn = true;
            Item.channel = true;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.shootSpeed = 6f;
            // Set so the item isn't classified as true melee
            Item.shoot = ModContent.ProjectileType<BladecrestOathswordProj>();
        }

        public override bool CanUseItem(Player player)
        {
            int bladeProjID = ModContent.ProjectileType<BladecrestOathswordProj>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].type != bladeProjID || !Main.projectile[i].active || Main.projectile[i].owner != player.whoAmI)
                    continue;

                return Main.projectile[i].ModProjectile<BladecrestOathswordProj>().PostSwingRepositionDelay <= 0f;
            }

            return base.CanUseItem(player);
        }

        // Return false because Shoot code is not actually relevant
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo spawnSource, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => false;

        public override bool? CanHitNPC(Player player, NPC target) => false;

        public override bool CanHitPvp(Player player, Player target) => false;
    }
}

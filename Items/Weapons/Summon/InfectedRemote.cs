using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class InfectedRemote : ModItem
    {
        int viruliSlots;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infected Remote");
            Tooltip.SetDefault("There’s a faded note written on it in green\n" +
                "Only the first line is readable: 'She won’t afflict you, I promise!'\n" +
                "Summons the harbinger of the plague...?\n" +
                "Consumes all of the remaining minion slots on use\n" +
                "Must be used from the hotbar\n" +
                "Increased power based on the number of minion slots used");
        }

        public override void SetDefaults()
        {
            Item.damage = 75;
            Item.mana = 10;
            Item.width = 46;
            Item.height = 28;
            Item.useTime = Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item15; //phaseblade sound effect
            Item.shoot = ModContent.ProjectileType<PlaguePrincess>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;

            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.Calamity().donorItem = true;
        }

        public override void HoldItem(Player player)
        {
            double minionCount = 0;
            for (int j = 0; j < Main.projectile.Length; j++)
            {
                Projectile projectile = Main.projectile[j];
                if (projectile.active && projectile.owner == player.whoAmI && projectile.minion && projectile.type != ModContent.ProjectileType<PlaguePrincess>())
                {
                    minionCount += projectile.minionSlots;
                }
            }
            viruliSlots = (int)((double)player.maxMinions - minionCount);
        }

        public override bool CanUseItem(Player player)
        {
            return viruliSlots >= 1 && player.ownedProjectileCounts[Item.shoot] <= 0;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            CalamityUtils.KillShootProjectiles(true, type, player);
            float damageMult = ((float)Math.Log(viruliSlots, 10f)) + 1f;
            position = Main.MouseWorld;
            velocity.X = 0;
            velocity.Y = 0;
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, (int)(damage * damageMult), knockback, player.whoAmI, viruliSlots, 1f);
            return false;
        }
    }
}

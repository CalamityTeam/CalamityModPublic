using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Summon
{
    public class AbandonedSlimeStaff : ModItem
    {
        int slimeSlots;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abandoned Slime Staff");
            Tooltip.SetDefault("Cast down from the heavens in disgust, this relic sings a song of quiet tragedy...\n" +
                "Consumes all of the remaining minion slots on use\n" +
                "Must be used from the hotbar\n" +
                "Increased power and size based on the number of minion slots used\n" +
                "Holding this weapon grants 10% increased jump speed");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 62;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item44;

            Item.DamageType = DamageClass.Summon;
            Item.mana = 40;
            Item.damage = 56;
            Item.knockBack = 3f;
            Item.useTime = Item.useAnimation = 20;
            Item.shoot = ModContent.ProjectileType<AstrageldonSummon>();
            Item.shootSpeed = 10f;

            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.Calamity().donorItem = true;
        }

        public override void HoldItem(Player player)
        {
            player.jumpSpeedBoost += 0.5f;

            double minionCount = 0;
            for (int j = 0; j < Main.projectile.Length; j++)
            {
                Projectile projectile = Main.projectile[j];
                if (projectile.active && projectile.owner == player.whoAmI && projectile.minion && projectile.type != ModContent.ProjectileType<AstrageldonSummon>())
                {
                    minionCount += projectile.minionSlots;
                }
            }
            slimeSlots = (int)(player.maxMinions - minionCount);
        }

        public override bool CanUseItem(Player player)
        {
            return slimeSlots >= 1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            CalamityUtils.KillShootProjectiles(true, type, player);
            float damageMult = ((float)Math.Log(slimeSlots, 8f)) + 1f;
            position = Main.MouseWorld;
            velocity.X = 0;
            velocity.Y = 0;
            int slime = Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, (int)(damage * damageMult), knockback, player.whoAmI);
            Main.projectile[slime].originalDamage = (int)(Item.damage * damageMult);
            Main.projectile[slime].minionSlots = slimeSlots;
            return false;
        }
    }
}

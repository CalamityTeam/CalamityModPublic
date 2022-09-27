using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Rarities;

namespace CalamityMod.Items.Weapons.Summon
{
    public class WarloksMoonFist : ModItem
    {
        public const int SlotCount = 4;

        public const int PunchCooldownTime = 36;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Warloks' Moon Fist");
            Tooltip.SetDefault("Summons a celestial fist to punch enemies for you\n" +
                               $"Takes up {SlotCount} minion slots");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 330;
            Item.mana = 10;
            Item.width = 50;
            Item.height = 50;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.UseSound = SoundID.Item104;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<MoonFist>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                int existingFists = player.ownedProjectileCounts[type];
                int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
                if (Main.projectile.IndexInRange(p))
                {
                    Main.projectile[p].originalDamage = Item.damage;
                    Main.projectile[p].ai[0] = existingFists;
                }
            }
            return false;
        }
    }
}

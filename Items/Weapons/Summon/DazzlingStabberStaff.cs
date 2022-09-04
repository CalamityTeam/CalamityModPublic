using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class DazzlingStabberStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dazzling Stabber Staff");
            Tooltip.SetDefault("Summons a holy blade to fight for you");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 52;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.UseSound = SoundID.DD2_DarkMageHealImpact;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.damage = 56;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<DazzlingStabber>();
            Item.shootSpeed = 13f;

            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
            }
            float angleMax = MathHelper.ToRadians(45f);
            if (CalamityUtils.CountProjectiles(type) == 1)
                angleMax = 0f;
            float index = 1f;
            if (player.ownedProjectileCounts[Item.shoot] > 8)
            {
                angleMax += MathHelper.ToRadians((player.ownedProjectileCounts[Item.shoot] - 8) * 2.5f);
            }
            angleMax = angleMax > MathHelper.ToRadians(105f) ? MathHelper.ToRadians(105f) : angleMax; // More intuative than using a min function
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == type && Main.projectile[i].owner == player.whoAmI)
                {
                    Main.projectile[i].ai[1] = (index / CalamityUtils.CountProjectiles(type)) * angleMax - angleMax / 2f;
                    Main.projectile[i].netUpdate = true;
                    index++;
                }
            }
            return false;
        }
    }
}

using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class Vigilance : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vigilance");
            Tooltip.SetDefault("Summons a soul seeker to fight for you");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 150;
            Item.mana = 10;
            Item.width = Item.height = 32;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.DD2_BetsySummon;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SeekerSummonProj>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;

            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                if (player.maxMinions - player.slotsMinions >= 1f)
                {
                    int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
                    if (Main.projectile.IndexInRange(p))
                    {
                        Main.projectile[p].ai[0] = player.ownedProjectileCounts[type];
                        Main.projectile[p].originalDamage = Item.damage;
                    }
                }
            }
            return false;
        }
    }
}

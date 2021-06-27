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
        }

        public override void SetDefaults()
        {
            item.damage = 333;
            item.mana = 12;
            item.width = item.height = 32;
            item.useTime = item.useAnimation = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 4f;
            item.UseSound = SoundID.DD2_BetsySummon;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SeekerSummonProj>();
            item.shootSpeed = 10f;
            item.summon = true;

            item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                int seekerIndex = 0;
                int totalSeekers = 0;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].type != type || !Main.projectile[i].active || Main.projectile[i].owner != player.whoAmI)
                        continue;

                    totalSeekers++;
                }

                Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI);

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].type != type || !Main.projectile[i].active || Main.projectile[i].owner != player.whoAmI)
                        continue;
                    Main.projectile[i].ai[0] = seekerIndex / (float)totalSeekers;
                    Main.projectile[i].netUpdate = true;

                    seekerIndex++;
                }
            }
            return false;
        }
    }
}

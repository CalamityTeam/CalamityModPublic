using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class ViralSprout : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Viral Sprout");
            Tooltip.SetDefault("Summons a sage spirit to fight for you\n" +
                "Inflicts Sage Poison, a debuff that becomes stronger the more spirits you own");
        }

        public override void SetDefaults()
        {
            item.damage = 24;
            item.mana = 10;
            item.width = 48;
            item.height = 56;
            item.useTime = item.useAnimation = 19;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = CalamityGlobalItem.Rarity7BuyPrice;
            item.rare = ItemRarityID.Lime;
            item.UseSound = SoundID.Item44;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SageSpirit>();
            item.shootSpeed = 10f;
            item.summon = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI);

                int minionCount = 0;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].type != type || !Main.projectile[i].active || Main.projectile[i].owner != player.whoAmI)
                        continue;

                    Main.projectile[i].localAI[0] = minionCount;
                    Main.projectile[i].netUpdate = true;
                    minionCount++;
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DraedonBar>(), 12);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

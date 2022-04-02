using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class WitherBlossomsStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wither Blossoms Staff");
            Tooltip.SetDefault("Summons four decaying flowers over your head\n" +
                               "The combined flowers consume two minion slots");
        }

        public override void SetDefaults()
        {
            item.damage = 61;
            item.mana = 10;
            item.width = 52;
            item.height = 60;
            item.useTime = item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 3f;
            item.value = CalamityGlobalItem.Rarity8BuyPrice;
            item.rare = ItemRarityID.Yellow;
            item.UseSound = SoundID.Item46;
            item.shoot = ModContent.ProjectileType<WitherBlossom>();
            item.shootSpeed = 10f;
            item.summon = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] < 4; //If you already have all 4, no need to resummon

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            CalamityUtils.KillShootProjectiles(false, type, player);
            for (int i = 0; i < 4; i++)
            {
                Projectile blossom = Projectile.NewProjectileDirect(player.Center, Vector2.Zero, type, damage, knockBack, player.whoAmI, 0f, 0f);
                blossom.ai[0] = MathHelper.TwoPi * i / 4f;
                blossom.rotation = blossom.ai[0];
            }
            return false;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<TundraFlameBlossomsStaff>());
            recipe.AddIngredient(ModContent.ItemType<PlagueCellCluster>(), 15);
            recipe.AddIngredient(ModContent.ItemType<CoreofCalamity>(), 5);
            recipe.AddIngredient(ModContent.ItemType<BarofLife>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

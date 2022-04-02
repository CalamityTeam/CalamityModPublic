using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class MajesticGuard : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Majestic Guard");
            Tooltip.SetDefault("Lowers enemy defense by 1 with every strike\n" +
                "If enemy defense is 0 or below your attacks will heal you");
        }

        public override void SetDefaults()
        {
            item.width = 100;
            item.height = 100;
            item.scale = 1.5f;
            item.damage = 70;
            item.melee = true;
            item.useAnimation = 22;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 22;
            item.useTurn = true;
            item.knockBack = 7.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (target.Calamity().miscDefenseLoss < target.defense)
                target.Calamity().miscDefenseLoss += 1;

            // Healing effect does not trigger versus dummies
            if (player.moonLeech)
                return;

            if (target.Calamity().miscDefenseLoss >= target.defense && target.canGhostHeal)
            {
                player.statLife += 3;
                player.HealEffect(3);
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.AdamantiteSword);
            recipe.AddIngredient(ItemID.SoulofMight, 15);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.TitaniumSword);
            recipe.AddIngredient(ItemID.SoulofMight, 15);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

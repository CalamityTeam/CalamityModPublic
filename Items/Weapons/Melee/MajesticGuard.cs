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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 100;
            Item.height = 100;
            Item.scale = 1.5f;
            Item.damage = 70;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 22;
            Item.useTurn = true;
            Item.knockBack = 7.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
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
            CreateRecipe().
                AddIngredient(ItemID.AdamantiteSword).
                AddIngredient(ItemID.SoulofMight, 15).
                AddTile(TileID.MythrilAnvil).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.TitaniumSword).
                AddIngredient(ItemID.SoulofMight, 15).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

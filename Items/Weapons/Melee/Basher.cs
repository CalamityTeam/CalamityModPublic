using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Basher : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Basher");
            Tooltip.SetDefault("Inflicts irradiated on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 56;
            item.height = 60;
            item.damage = 30;
            item.scale = 1.05f;
            item.melee = true;
            item.useAnimation = item.useTime = 26;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 7f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.useTurn = true;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SulfuricScale>(), 12);
            recipe.AddIngredient(ModContent.ItemType<Acidwood>(), 30);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 300);
        }
    }
}

using Terraria.ModLoader;
using Terraria.ID;
using TerrariaAudio = Terraria.Audio;
using CalamityMod.Projectiles.Summon;

namespace CalamityMod.Items.Weapons.Summon
{
    public class FleshOfInfidelity : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flesh of Infidelity");
            Tooltip.SetDefault("Summons a tentacled ball of flesh that splashes blood onto enemies");
        }

        public override void SetDefaults()
        {
            item.damage = 23;
            item.mana = 10;
            item.width = item.height = 48;
            item.useTime = item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 1f;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Orange;

            // SoundID has no Zombie24 sound instance, so we must create one ourselves.
            item.UseSound = new TerrariaAudio.LegacySoundStyle(SoundID.Zombie, 24, TerrariaAudio.SoundType.Sound);
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<FleshBallMinion>();
            item.shootSpeed = 10f;
            item.summon = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BelladonnaSpiritStaff>());
            recipe.AddIngredient(ModContent.ItemType<StaffOfNecrosteocytes>());
            recipe.AddIngredient(ModContent.ItemType<ScabRipper>());
            recipe.AddIngredient(ItemID.ImpStaff);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

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
            Item.damage = 23;
            Item.mana = 10;
            Item.width = Item.height = 48;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;

            // SoundID has no Zombie24 sound instance, so we must create one ourselves.
            Item.UseSound = new TerrariaAudio.LegacySoundStyle(SoundID.Zombie, 24, TerrariaAudio.SoundType.Sound);
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FleshBallMinion>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<BelladonnaSpiritStaff>()).AddIngredient(ModContent.ItemType<StaffOfNecrosteocytes>()).AddIngredient(ModContent.ItemType<ScabRipper>()).AddIngredient(ItemID.ImpStaff).AddTile(TileID.DemonAltar).Register();
        }
    }
}

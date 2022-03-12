using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class CosmicImmaterializer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Immaterializer");
            Tooltip.SetDefault("Summons a cosmic energy spiral to fight for you\n" +
                               "The orb will fire swarms of homing energy bolts when enemies are detected by it\n" +
                               "Requires 10 minion slots to use and there can only be one energy spiral\n" +
                               "Without a summoner armor set bonus this minion will deal less damage");
        }

        public override void SetDefaults()
        {
            item.mana = 10;
            item.damage = 560;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.width = 74;
            item.height = 72;
            item.useTime = item.useAnimation = 10;
            item.noMelee = true;
            item.knockBack = 0f;
            item.value = CalamityGlobalItem.Rarity15BuyPrice;
            item.rare = ItemRarityID.Red;
            item.UseSound = SoundID.Item60;
            item.shoot = ModContent.ProjectileType<CosmicEnergySpiral>();
            item.shootSpeed = 10f;
            item.summon = true;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0 && player.maxMinions >= 10;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            CalamityUtils.KillShootProjectiles(true, type, player);
            CalamityPlayer modPlayer = player.Calamity();
            bool hasSummonerSet = modPlayer.WearingPostMLSummonerSet;
            Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, (int)(damage * (hasSummonerSet ? 1 : 0.66)), knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ElementalAxe>());
            recipe.AddIngredient(ModContent.ItemType<CorvidHarbringerStaff>());
            recipe.AddIngredient(ModContent.ItemType<AncientIceChunk>());
            recipe.AddIngredient(ModContent.ItemType<EnergyStaff>());
            recipe.AddIngredient(ModContent.ItemType<MiracleMatter>());
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

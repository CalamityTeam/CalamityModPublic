using Terraria.DataStructures;
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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.mana = 10;
            Item.damage = 560;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.width = 74;
            Item.height = 72;
            Item.useTime = Item.useAnimation = 10;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item60;
            Item.shoot = ModContent.ProjectileType<CosmicEnergySpiral>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
            Item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0 && player.maxMinions >= 10;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            CalamityUtils.KillShootProjectiles(true, type, player);
            CalamityPlayer modPlayer = player.Calamity();
            bool hasSummonerSet = modPlayer.WearingPostMLSummonerSet;
            int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, (int)(damage * (hasSummonerSet ? 1 : 0.66)), knockback, player.whoAmI, 0f, 0f);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = (int)(Item.damage * (hasSummonerSet ? 1f : 0.66f));
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ElementalAxe>().
                AddIngredient<CorvidHarbringerStaff>().
                AddIngredient<AncientIceChunk>().
                AddIngredient<EnergyStaff>().
                AddIngredient<MiracleMatter>().
                AddTile<DraedonsForge>().
                Register();
        }
    }
}

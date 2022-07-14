using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("DraedonsExoblade")]
    public class Exoblade : ModItem
    {
        public bool RMBchannel = false;

        public const float NotTrueMeleeDamagePenalty = 0.67f;

        public const float ExplosionDamageFactor = 1.8f;

        public const float LungeDamageFactor = 1.75f;

        public const float LungeSpeed = 37f;

        public const float ReboundSpeed = 10.5f;

        public const float PercentageOfAnimationSpentLunging = 0.6f;

        public const float AnticipationOffsetRatio = 0.27f;

        public const int OpportunityForBigSlash = 37;

        public const float BigSlashUpscaleFactor = 2.3f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exoblade");
            Tooltip.SetDefault("Ancient blade of Yharim's weapons and armors expert, Draedon\n" +
                               "Left clicks release multiple energy beams that home in on enemies and slice them on hit\n" +
                               "Right clicks makes you dash in the direction of the cursor with the blade\n" +
                               "Enemy hits from the blade during the dash result in massive damage and a rebound\n" +
                               "Left clicks briefly after a rebound are far stronger and create explosions on enemy hits");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 80;
            Item.height = 114;
            Item.damage = 1475;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 49;
            Item.useAnimation = 49;
            Item.useTurn = true;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 9f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.shootSpeed = 9f;
            Item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool? CanHitNPC(Player player, NPC target) => false;

        public override bool CanHitPvp(Player player, Player target) => false;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Terratomere>().
                AddIngredient<AnarchyBlade>().
                AddIngredient<FlarefrostBlade>().
                AddIngredient<StellarStriker>().
                AddIngredient<MiracleMatter>().
                AddTile(ModContent.TileType<DraedonsForge>()).
                Register();
        }
    }
}

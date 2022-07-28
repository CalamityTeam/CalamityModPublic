using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System.Linq;
using CalamityMod.Projectiles.Melee;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("DraedonsExoblade")]
    public class Exoblade : ModItem
    {
        public static readonly SoundStyle SwingSound = new("CalamityMod/Sounds/Item/ExobladeSwing") { MaxInstances = 3, PitchVariance = 0.6f, Volume = 0.6f };

        public bool RMBchannel = false;

        public const int BeamNoHomeTime = 24;

        public const float NotTrueMeleeDamagePenalty = 0.46f;

        public const float ExplosionDamageFactor = 1.8f;

        public const float LungeDamageFactor = 1.75f;

        public const float LungeSpeed = 37f;

        public const float ReboundSpeed = 6f;

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
            Item.damage = 2625;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 49;
            Item.useAnimation = 49;
            Item.useTurn = true;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 9f;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.shoot = ProjectileType<ExobladeProj>();
            Item.rare = ItemRarityID.Red;
            Item.shootSpeed = 9f;
            Item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool CanShoot(Player player) => !Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ProjectileType<ExobladeProj>());

        public override void HoldItem(Player player)
        {
            player.Calamity().rightClickListener = true;
            player.Calamity().mouseWorldListener = true;
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

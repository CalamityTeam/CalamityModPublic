using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class BloodsoakedCrasher : ModItem //This weapon has been coded by Ben || Termi
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodsoaked Crasher");
            Tooltip.SetDefault("Slows down when hitting an enemy. Speeds up otherwise\n" +
            "Heals on enemy hits\n" +
            "Stealth strikes spawn homing blood on enemy hits");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 245;
            Item.knockBack = 3f;
            Item.autoReuse = true;
            Item.DamageType = RogueDamageClass.Instance;
            Item.useAnimation = Item.useTime = 24;
            Item.shootSpeed = 9f;
            Item.shoot = ModContent.ProjectileType<BloodsoakedCrashax>();

            Item.width = 66;
            Item.height = 64;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.RogueWeapon;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CrushsawCrasher>().
                AddIngredient<BloodstoneCore>(12).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}

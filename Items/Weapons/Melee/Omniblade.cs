using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Omniblade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Omniblade");
            Tooltip.SetDefault("An ancient blade forged by the legendary Omnir");
        }

        public override void SetDefaults()
        {
            item.width = 64;
            item.damage = 100;
            item.melee = true;
            item.useAnimation = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 10;
            item.useTurn = true;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 146;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.shoot = ModContent.ProjectileType<OmnibladeSwing>();
            item.shootSpeed = 24f;
            item.rare = ItemRarityID.Yellow;
            item.Calamity().trueMelee = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 45;

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Katana);
            recipe.AddIngredient(ModContent.ItemType<BarofLife>(), 20);
            recipe.AddIngredient(ModContent.ItemType<CoreofCalamity>(), 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

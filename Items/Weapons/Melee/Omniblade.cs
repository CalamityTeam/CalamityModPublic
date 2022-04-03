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
            Item.width = 64;
            Item.damage = 100;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useTurn = true;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 146;
            Item.value = Item.buyPrice(0, 80, 0, 0);
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<OmnibladeSwing>();
            Item.shootSpeed = 24f;
            Item.rare = ItemRarityID.Yellow;
            Item.Calamity().trueMelee = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 45;

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.Katana).AddIngredient(ModContent.ItemType<BarofLife>(), 20).AddIngredient(ModContent.ItemType<CoreofCalamity>(), 10).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}

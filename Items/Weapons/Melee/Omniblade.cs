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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 64;
            Item.damage = 100;
            Item.DamageType = TrueMeleeNoSpeedDamageClass.Instance;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 12;
            Item.useTurn = true;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 146;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<OmnibladeSwing>();
            Item.shootSpeed = 24f;
            Item.rare = ItemRarityID.Yellow;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 45;

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Katana).
                AddIngredient<LifeAlloy>(20).
                AddIngredient<CoreofCalamity>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

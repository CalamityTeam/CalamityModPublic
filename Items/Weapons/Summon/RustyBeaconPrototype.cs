using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class RustyBeaconPrototype : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rusty Beacon Prototype");
            Tooltip.SetDefault("Summons a long-abandoned drone to support you\n" +
                               "Clicking on an enemy gives them a tiny prick, causing them to become aggravated\n" +
                               "The drone hovers above nearby enemies and inflicts irradiated");
        }

        public override void SetDefaults()
        {
            Item.mana = 10;
            Item.width = 28;
            Item.height = 20;
            Item.useTime = Item.useAnimation = 34;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 0.5f;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item15; // Phaseblade sound effect
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<RustyDrone>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI, 0f, 1f);
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<SulfuricScale>(), 20).AddRecipeGroup("IronBar", 10).AddTile(TileID.Anvils).Register();
        }
    }
}

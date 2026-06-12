using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class LemonNade : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 30;
            Item.damage = 33;
            Item.DamageType = RogueDamageClass.Instance;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.channel = true;
            Item.shootSpeed = 13f;
            Item.shoot = ModContent.ProjectileType<LemonNadeHoldout>();
        }

        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[Item.shoot] <= 0)
                player.Calamity().rogueStealth = 0;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Lemon).
                AddIngredient(ItemID.BouncyGrenade, 99).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}

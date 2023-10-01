using CalamityMod.Projectiles.Ranged;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    [LegacyName("PrideHuntersPlanarRipper")]
    public class PridefulHuntersPlanarRipper : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        private int counter = 0;

        public override void SetDefaults()
        {
            Item.damage = 86;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 68;
            Item.height = 32;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1f;

            Item.UseSound = SoundID.Item11;
            Item.shoot = ProjectileID.Bullet;
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 15f;

            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().donorItem = true;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-12, -6);

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.Next(0, 100) >= 33;

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // If using standard musket balls (or Silver Bullets actually), fire the special lightning bolts and have special properties.
            if (type == ProjectileID.Bullet)
                type = ModContent.ProjectileType<PlanarRipperBolt>();

            // Every 4th shot deals 35% increased damage and resets the counter.
            counter++;
            if (counter == 4)
            {
                damage = (int)(damage * 1.35f);
                counter = 0;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<P90>().
                AddIngredient(ItemID.Uzi).
                AddIngredient<PearlGod>(). //This should be removed
                AddIngredient(ItemID.LunarBar, 5).
                AddIngredient(ItemID.FragmentVortex, 10).
                AddIngredient<GalacticaSingularity>(6). //This should be removed
                AddIngredient<CoreofCalamity>(). // This should be removed
                AddIngredient<Stardust>(25). // This should be removed
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}

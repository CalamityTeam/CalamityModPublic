using CalamityMod.Projectiles.Ranged;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class PrideHuntersPlanarRipper : ModItem
    {
        private int counter = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prideful Hunter's Planar Ripper");
            Tooltip.SetDefault("Every fourth shot deals 135% damage\n" +
                "Converts musket balls into lightning bolts\n" +
                "Lightning bolts travel extremely fast and explode on enemy kills\n" +
                "Lightning bolt crits grant a stacking speed boost to the player\n" +
                "This stacks up to 20 percent bonus movement speed and acceleration\n" +
                "The boost will reset if the player holds a different item\n" +
                "33% chance to not consume ammo");
        }

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

        public override bool ConsumeAmmo(Player player) => Main.rand.Next(0, 100) >= 33;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
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

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<P90>()).AddIngredient(ItemID.Uzi).AddIngredient(ModContent.ItemType<PearlGod>()).AddIngredient(ItemID.LunarBar, 5).AddIngredient(ItemID.FragmentVortex, 10).AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 6).AddIngredient(ModContent.ItemType<CoreofCalamity>(), 3).AddIngredient(ModContent.ItemType<Stardust>(), 25).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}

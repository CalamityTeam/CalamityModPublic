using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class TheWand : ModItem
    {
        public static int BaseDamage = 599;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Wand");
            Tooltip.SetDefault("The ultimate wand");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.damage = 1;
            item.mana = 150;
            item.magic = true;
            item.noMelee = true;
            item.useAnimation = 19;
            item.useTime = 19;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 0.5f;
            item.UseSound = SoundID.Item102;
            item.autoReuse = true;
            item.height = 36;
            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = ItemRarityID.Red;
            item.shoot = ModContent.ProjectileType<SparkInfernal>();
            item.shootSpeed = 24f;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool CanUseItem(Player player)
        {
            int numWandBolts = player.ownedProjectileCounts[ModContent.ProjectileType<SparkInfernal>()];
            int numTornadoStarters = player.ownedProjectileCounts[ModContent.ProjectileType<InfernadoMarkFriendly>()];
            int numTornadoPieces = player.ownedProjectileCounts[ModContent.ProjectileType<InfernadoFriendly>()];
            return numWandBolts + numTornadoStarters + numTornadoPieces < 1;
        }

        public override Vector2? HoldoutOrigin() => new Vector2(10, 10);

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 6);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.WandofSparking);
            recipe.AddIngredient(ModContent.ItemType<HellcasterFragment>(), 5);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

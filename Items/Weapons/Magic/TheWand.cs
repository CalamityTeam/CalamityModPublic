using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
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
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.damage = 1;
            Item.mana = 150;
            Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
            Item.useAnimation = 19;
            Item.useTime = 19;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0.5f;
            Item.UseSound = SoundID.Item102;
            Item.autoReuse = true;
            Item.height = 36;
            Item.value = Item.buyPrice(2, 50, 0, 0);
            Item.shoot = ModContent.ProjectileType<SparkInfernal>();
            Item.shootSpeed = 24f;
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override bool CanUseItem(Player player)
        {
            int numWandBolts = player.ownedProjectileCounts[ModContent.ProjectileType<SparkInfernal>()];
            int numTornadoStarters = player.ownedProjectileCounts[ModContent.ProjectileType<InfernadoMarkFriendly>()];
            int numTornadoPieces = player.ownedProjectileCounts[ModContent.ProjectileType<InfernadoFriendly>()];
            return numWandBolts + numTornadoStarters + numTornadoPieces < 1;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 6);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.WandofSparking).
                AddIngredient<YharonSoulFragment>(8).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}

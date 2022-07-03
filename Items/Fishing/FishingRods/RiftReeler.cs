using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using Terraria.DataStructures;

namespace CalamityMod.Items.Fishing.FishingRods
{
    [LegacyName("ChaoticSpreadRod")]
    public class RiftReeler : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            ItemID.Sets.CanFishInLava[Item.type] = true;

            DisplayName.SetDefault("Rift Reeler");
            Tooltip.SetDefault("Fires three to five lines at once. Can fish in lava.\n" +
                "The battlefield is a scene of constant chaos.\n" + //Napoleon Bonaparte quote reference
                "The winner will be the one who controls that chaos, both the pole and the fish.");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.useAnimation = 8;
            Item.useTime = 8;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.fishingPole = 45;
            Item.shootSpeed = 17f;
            Item.shoot = ModContent.ProjectileType<RiftReelerBobber>();
            Item.value = Item.buyPrice(0, 80, 0, 0);
            Item.rare = ItemRarityID.Yellow;
        }
        
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int index = 0; index < Main.rand.Next(3,6); ++index) //3 to 5 bobbers
            {
                float SpeedX = velocity.X + Main.rand.NextFloat(-3.75f, 3.75f);
                float SpeedY = velocity.Y + Main.rand.NextFloat(-3.75f, 3.75f);
                Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, 0, 0f, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HotlineFishingHook).
                AddIngredient<ScoriaBar>(8).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

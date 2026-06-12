using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.FishingRods
{
    [LegacyName("ChaoticSpreadRod")]
    public class RiftReeler : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Fishing";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanFishInLava[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.useAnimation = Item.useTime = 8;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.fishingPole = 45;
            Item.shootSpeed = 17f;
            Item.shoot = ModContent.ProjectileType<RiftReelerBobber>();
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 4; i++) //4 bobbers
            {
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(45f)), type, 10, 1f, player.whoAmI, ai2: Main.rand.Next(2));
            }
            return false;
        }

        public override void ModifyFishingLine(Projectile bobber, ref Vector2 lineOriginOffset, ref Color lineColor)
        {
            lineOriginOffset = new Vector2(67f, -33f);
            if (bobber.ai[2] == 0f)
                lineColor = new Color(255, 165, 0, 100);
            else
                lineColor = new Color(200, 206, 209, 100);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HotlineFishingHook).
                AddIngredient<ScoriaBar>(6).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

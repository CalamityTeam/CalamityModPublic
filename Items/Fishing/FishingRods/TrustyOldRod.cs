using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.FishingRods
{
    public class TrustyOldRod : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Fishing";
        public static (int, int) enemyChance = (1, 3); // x/x Chance to pull up enemies
        public override void SetDefaults()
        {
            Item.width = 70;
            Item.height = 70;
            Item.useAnimation = 8;
            Item.useTime = 8;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.fishingPole = 45;
            Item.shootSpeed = 14f;
            Item.shoot = ModContent.ProjectileType<TrustyOldBobber>();
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 35); // Same price as Sitting Duck's Fishing Pole; Sold by Shady Salesman
        }
        public override void ModifyFishingLine(Projectile bobber, ref Vector2 lineOriginOffset, ref Color lineColor)
        {
            lineOriginOffset = new Vector2(68f, -52f);
            lineColor = new Color(171, 171, 171, 0);
        }
    }
}

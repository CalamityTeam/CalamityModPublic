using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Fishing.SunkenSeaCatches
{
    public class SparklingEmpress : ModItem
    {
        public static int BaseDamage = 10;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sparkling Empress");
            Tooltip.SetDefault("Fires a concentrated laser to annihilate your foes\n" +
                "Defeated enemies explode into prism shards");
            Item.staff[item.type] = true; //so it doesn't look weird af when holding it
        }

        public override void SetDefaults()
        {
            item.damage = BaseDamage;
            item.noMelee = true;
            item.magic = true;
            item.channel = true; //Channel so that you can hold the weapon [Important]
            item.rare = ItemRarityID.Green;
            item.mana = 5;
            item.width = 42;
            item.height = 34;
            item.useTime = 20;
            item.useAnimation = 20;
            item.UseSound = SoundID.Item13;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.shootSpeed = 14f;
            item.shoot = ModContent.ProjectileType<SparklingBeam>();
            item.value = Item.sellPrice(silver: 40);
        }

        public override Vector2? HoldoutOrigin() //so it looks normal when holding
        {
            return new Vector2(10, 10);
        }
    }
}

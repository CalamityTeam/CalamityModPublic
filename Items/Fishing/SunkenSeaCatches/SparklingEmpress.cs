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
            Item.staff[Item.type] = true; //so it doesn't look weird af when holding it
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = BaseDamage;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Magic;
            Item.channel = true; //Channel so that you can hold the weapon [Important]
            Item.rare = ItemRarityID.Green;
            Item.mana = 5;
            Item.width = 42;
            Item.height = 34;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.UseSound = SoundID.Item13;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shootSpeed = 14f;
            Item.shoot = ModContent.ProjectileType<SparklingBeam>();
            Item.value = Item.sellPrice(silver: 40);
        }

        public override Vector2? HoldoutOrigin() //so it looks normal when holding
        {
            return new Vector2(10, 10);
        }
    }
}

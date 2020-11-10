using CalamityMod.Projectiles.Hybrid;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class EradicatorMelee : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eradicator");
            Tooltip.SetDefault("Throws a disk that fires lasers at nearby enemies");
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.damage = 300;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 19;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 19;
            item.knockBack = 7f;
            item.UseSound = SoundID.Item1;
            item.melee = true;
            item.height = 54;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<EradicatorProjectile>();
            item.shootSpeed = 12f;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
			item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.GetTexture("CalamityMod/Items/Weapons/Melee/EradicatorMeleeGlow"));
        }
    }
}

using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TerrorBlade : ModItem
    {
        internal const float TerrorBlastMultiplier = 0.3f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terror Blade");
            Tooltip.SetDefault("Fires a terror beam that bounces off tiles\n" +
                "On every bounce it emits an explosion");
        }

        public override void SetDefaults()
        {
            item.width = 88;
            item.damage = 630;
            item.melee = true;
            item.useAnimation = 18;
            item.useTime = 18;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 8.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 80;
            item.shoot = ModContent.ProjectileType<TerrorBeam>();
            item.shootSpeed = 20f;

            item.value = CalamityGlobalItem.Rarity13BuyPrice;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.GetTexture("CalamityMod/Items/Weapons/Melee/TerrorBladeGlow"));
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 60);
        }
    }
}

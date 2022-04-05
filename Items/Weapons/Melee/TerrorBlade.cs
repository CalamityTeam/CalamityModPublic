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
            Item.width = 88;
            Item.damage = 630;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 18;
            Item.useTime = 18;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 8.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 80;
            Item.shoot = ModContent.ProjectileType<TerrorBeam>();
            Item.shootSpeed = 20f;

            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/TerrorBladeGlow").Value);
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 60);
        }
    }
}

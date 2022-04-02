using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Magic;

namespace CalamityMod.Items.Weapons.Magic
{
    public class AbyssShocker : ModItem
    {
        public override void SetStaticDefaults() 
        {
            DisplayName.SetDefault("Abyss Shocker");
            Tooltip.SetDefault("Fires an erratic lightning bolt that arcs and bounces between enemies");
        }

        public override void SetDefaults() 
        {
            item.damage = 28;
            item.noMelee = true;
            item.magic = true;
            item.channel = true;
            item.width = 86;
            item.height = 32;
            item.useTime = 19;
            item.useAnimation = 19;
            item.UseSound = SoundID.Item13;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.mana = 10;

            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Orange;
            item.Calamity().donorItem = true;

            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<LightningArc>();
            item.shootSpeed = 14f;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.GetTexture("CalamityMod/Items/Weapons/Magic/AbyssShocker_mask"));
        }

        public override Vector2? HoldoutOffset() => new Vector2(-14, 0);
    }
}
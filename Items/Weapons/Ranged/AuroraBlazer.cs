using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class AuroraBlazer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aurora Blazer");
            Tooltip.SetDefault("Spews astral flames that travel in a star-shaped patterns\n" +
            "60% chance to not consume gel");
        }

        public override void SetDefaults()
        {
            item.damage = 49;
            item.ranged = true;
            item.useTime = 18;
            item.useAnimation = 18;
            item.knockBack = 2f;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<AuroraFire>();
            item.shootSpeed = 7.5f;
            item.useAmmo = AmmoID.Gel;

            item.width = 68;
            item.height = 36;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.UseSound = SoundID.Item34;
            item.value = CalamityGlobalItem.Rarity7BuyPrice;
            item.rare = ItemRarityID.Lime;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float SpeedX = speedX + Main.rand.NextFloat(-15f, 15f) * 0.05f;
            float SpeedY = speedY + Main.rand.NextFloat(-15f, 15f) * 0.05f;
            Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, Main.rand.Next(4, 11), 0f);
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.GetTexture("CalamityMod/Items/Weapons/Ranged/AuroraBlazerGlow"));
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool ConsumeAmmo(Player player) => Main.rand.Next(100) >= 60;
    }
}

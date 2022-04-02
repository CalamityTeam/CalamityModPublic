using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class PristineFury : ModItem
    {
        public int frameCounter = 0;
        public int frame = 0;
        public static int BaseDamage = 77;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pristine Fury");
            Tooltip.SetDefault("Fires an intense helix of flames that explode into a column of fire\n" +
                "Right click to fire a short ranged cloud of lingering flames");
        }

        public override void SetDefaults()
        {
            item.damage = BaseDamage;
            item.ranged = true;
            item.width = 100;
            item.height = 46;
            item.useTime = 3;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5f;
            item.UseSound = SoundID.Item34;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<PristineFire>();
            item.shootSpeed = 11f;
            item.useAmmo = AmmoID.Gel;

            item.rare = ItemRarityID.Purple;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-25, -10);

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.useTime = 5;
                item.useAnimation = 20;
            }
            else
            {
                item.useTime = 3;
                item.useAnimation = 15;
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                int flameAmt = 3;
                damage = (int)(damage * 1.2);
                for (int index = 0; index < flameAmt; ++index)
                {
                    float SpeedX = speedX + Main.rand.NextFloat(-1.25f, 1.25f);
                    float SpeedY = speedY + Main.rand.NextFloat(-1.25f, 1.25f);
                    Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<PristineSecondary>(), (int)(damage * 0.8f), knockBack, player.whoAmI);
                }
            }
            else
            {
                damage = (int)(damage * 0.94);
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<PristineFire>(), damage, knockBack, player.whoAmI);
            }
            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frameI, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Ranged/PristineFury_Animated");
            spriteBatch.Draw(texture, position, item.GetCurrentFrame(ref frame, ref frameCounter, 5, 4), Color.White, 0f, origin, scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Ranged/PristineFury_Animated");
            spriteBatch.Draw(texture, item.position - Main.screenPosition, item.GetCurrentFrame(ref frame, ref frameCounter, 5, 4), lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Ranged/PristineFuryGlow");
            spriteBatch.Draw(texture, item.position - Main.screenPosition, item.GetCurrentFrame(ref frame, ref frameCounter, 5, 4, false), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }
    }
}

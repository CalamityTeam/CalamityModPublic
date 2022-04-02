using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Excelsus : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Excelsus");
            Tooltip.SetDefault("Fires a spread of spinning blades\n" +
                "Summons laser fountains on hit");
        }

        public override void SetDefaults()
        {
            item.width = 78;
            item.damage = 279;
            item.melee = true;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 15;
            item.useTurn = true;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 94;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.shoot = ModContent.ProjectileType<ExcelsusMain>();
            item.shootSpeed = 12f;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.GetTexture("CalamityMod/Items/Weapons/Melee/ExcelsusGlow"));
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int index = 0; index < 3; ++index)
            {
                float SpeedX = speedX + Main.rand.NextFloat(-1.5f, 1.5f);
                float SpeedY = speedY + Main.rand.NextFloat(-1.5f, 1.5f);
                switch (index)
                {
                    case 0:
                        type = ModContent.ProjectileType<ExcelsusMain>();
                        break;
                    case 1:
                        type = ModContent.ProjectileType<ExcelsusBlue>();
                        break;
                    case 2:
                        type = ModContent.ProjectileType<ExcelsusPink>();
                        break;
                }
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<LaserFountain>(), 0, 0, player.whoAmI);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<LaserFountain>(), 0, 0, player.whoAmI);
        }
    }
}

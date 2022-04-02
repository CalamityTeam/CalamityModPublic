using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class LeonidProgenitor : RogueWeapon
    {
        public static readonly Color blueColor = new Color(48, 208, 255);
        public static readonly Color purpleColor = new Color(208, 125, 218);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Leonid Progenitor");
            Tooltip.SetDefault("Throws a bombshell that explodes, summoning a meteor to impact the site\n" +
                "Right click to throw a spread of gravity affected comets that explode, leaving behind a star\n" +
                "Stealth strikes lob a bombshell that additionally splits into comets on hit");
        }

        public override void SafeSetDefaults()
        {
            item.damage = 64;
            item.Calamity().rogue = true;
            item.knockBack = 3f;
            item.useTime = item.useAnimation = 15;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<LeonidProgenitorBombshell>();
            item.shootSpeed = 12f;

            item.width = 32;
            item.height = 48;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item61;

            item.value = CalamityGlobalItem.Rarity7BuyPrice;
            item.rare = ItemRarityID.Lime;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.Calamity().StealthStrikeAvailable() || player.altFunctionUse != 2)
            {
                item.UseSound = SoundID.Item61;
                item.shoot = ModContent.ProjectileType<LeonidProgenitorBombshell>();
            }
            else
            {
                item.UseSound = SoundID.Item88;
                item.shoot = ModContent.ProjectileType<LeonidCometSmall>();
            }
            return base.CanUseItem(player);
        }

        public override float SafeSetUseTimeMultiplier(Player player)
        {
            if (player.Calamity().StealthStrikeAvailable() || player.altFunctionUse != 2)
                return 1f;
            return 0.75f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float stealthDamageFactor = player.Calamity().StealthStrikeAvailable() ? 1.25f : 1f;

            if (player.Calamity().StealthStrikeAvailable() || player.altFunctionUse != 2)
            {
                int bomb = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, (int)(damage * stealthDamageFactor), knockBack, player.whoAmI);
                if (bomb.WithinBounds(Main.maxProjectiles))
                    Main.projectile[bomb].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
                return false;
            }
            else
            {
                float dmgMult = 0.5f;
                for (float i = -2.5f; i < 3f; ++i)
                {
                    Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(i));
                    Projectile.NewProjectile(position, perturbedSpeed, type, (int)(damage * dmgMult * stealthDamageFactor), knockBack, player.whoAmI);
                }
            }
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.GetTexture("CalamityMod/Items/Weapons/Rogue/LeonidProgenitorGlow"));
        }
    }
}

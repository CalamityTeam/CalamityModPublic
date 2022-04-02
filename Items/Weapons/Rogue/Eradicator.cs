using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Eradicator : RogueWeapon
    {
        public static float Speed = 10.5f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eradicator");
            Tooltip.SetDefault("Throws a disk that fires lasers at nearby enemies\n" +
            "Stealth strikes stick to enemies and unleash a barrage of lasers in all directions");
        }

        public override void SafeSetDefaults()
        {
            item.width = 38;
            item.damage = 563;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 33;
            item.useAnimation = 33;
            item.knockBack = 7f;
            item.UseSound = SoundID.Item1;
            item.height = 54;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.shoot = ModContent.ProjectileType<EradicatorProjectile>();
            item.shootSpeed = Speed;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
                damage = (int)(damage * 0.5f);

            int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && proj.WithinBounds(Main.maxProjectiles))
            {
                Main.projectile[proj].timeLeft += EradicatorProjectile.StealthExtraLifetime;
                Main.projectile[proj].Calamity().stealthStrike = true;
            }
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.GetTexture("CalamityMod/Items/Weapons/Rogue/EradicatorGlow"));
        }
    }
}

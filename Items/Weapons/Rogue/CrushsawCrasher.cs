using System.Linq;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class CrushsawCrasher : RogueWeapon
    {
        bool HasHoveredOverNameInGFB = false;
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<HeavyBleeding>()];
        }
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 22;
            Item.damage = 57;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 18;
            Item.knockBack = 7f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.shoot = ModContent.ProjectileType<Crushax>();
            Item.shootSpeed = 13.5f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int spread = 3;
                for (int i = 0; i < 6; i++)
                {
                    Vector2 perturbedspeed = new Vector2(velocity.X + Main.rand.Next(-3, 4), velocity.Y + Main.rand.Next(-3, 4)).RotatedBy(MathHelper.ToRadians(spread));
                    int proj = Projectile.NewProjectile(source, position, perturbedspeed, type, damage, knockback, player.whoAmI);
                    if (proj.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[proj].Calamity().stealthStrike = true;
                        Main.projectile[proj].penetrate = 1;
                    }
                    spread -= Main.rand.Next(1, 4);
                }
                return false;
            }
            return true;
        }

        public override void UpdateInventory(Player player)
        {
            if (Main.zenithWorld)
            {
                if (Main.HoverItem.type == Item.type)
                {
                    if (!HasHoveredOverNameInGFB)
                    {
                        HasHoveredOverNameInGFB = true;
                        string[] firstWords = this.GetLocalizedValue("GFBFirstWords").Split('\n', '\r').Select(str => str.Trim()).ToArray();
                        string[] lastWords = this.GetLocalizedValue("GFBLastWords").Split('\n', '\r').Select(str => str.Trim()).ToArray();

                        string firstWord = firstWords[Main.rand.Next(firstWords.Length)];
                        string lastWord = lastWords[Main.rand.Next(lastWords.Length)];
                        string separator = this.GetLocalizedValue("GFBWordSeparator");
                        Item.SetNameOverride(firstWord + separator + lastWord);
                    }
                }
                else
                    HasHoveredOverNameInGFB = false;
            }
            else
                Item.ClearNameOverride();
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HeavyBleeding>(), 300);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<HeavyBleeding>(), 300);
        }
    }
}
